using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;

namespace SolastaUnfinishedBusiness.CustomUI;

public static class CustomModels
{
    // Gets or creates a prefab from a Blender Model on local disk, and returns an asset reference
    [NotNull]
    internal static AssetReference GetBlenderModel(string name)
    {
        var prefabGuid = GetGuid(name);

        if (!PrefabsByGuid.ContainsKey(prefabGuid))
        {
            BlenderModelLoader.Shared.LoadModel(name);
        }

        return new AssetReference(prefabGuid);
    }

    internal static void AlertIfModelsNotFound()
    {
        var blenderModels = Path.Combine(Main.ModFolder, "BlenderModels");

        string[] expectedModels = ["Katana", "LightningLauncher", "LongMace", "Pike", "ThunderGauntlet"];

        if (expectedModels.Any(x => !Directory.Exists(Path.Combine(blenderModels, x))))
        {
            Gui.GuiService.ShowMessage(
                MessageModal.Severity.Attention2,
                "Message/&MessageModWelcomeTitle",
                "ModUi/&MissingBlenderModelsDescription",
                "Message/&MessageOkTitle",
                "Message/&MessageCancelTitle",
                () => { },
                () => { });
        }
    }

    private static string GetGuid(string name)
    {
        return GuidHelper.Create(DefinitionBuilder.CeNamespaceGuid, name).ToString();
    }

    #region Blender Model Loader

    internal sealed class BlenderModelLoader : MonoBehaviour
    {
        private static readonly GameObject Template =
            new("BlenderModelLoader", typeof(MeshFilter), typeof(MeshRenderer));

        private static readonly int Alpha = Shader.PropertyToID("_Alpha");
        private static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        private static readonly int SpecColor = Shader.PropertyToID("_SpecColor");
        private static readonly int AmbientColor = Shader.PropertyToID("_AmbientColor");
        private static readonly int Glossiness = Shader.PropertyToID("_Glossiness");
        private static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
        private static readonly int ZWrite = Shader.PropertyToID("_ZWrite");
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");
        private static readonly int BumpMap = Shader.PropertyToID("_BumpMap");
        private static BlenderModelLoader _shared;
        public string directoryPath;

        [NotNull]
        internal static BlenderModelLoader Shared
        {
            get
            {
                if (_shared)
                {
                    return _shared;
                }

                _shared = new GameObject().AddComponent<BlenderModelLoader>();
                DontDestroyOnLoad(_shared.gameObject);

                return _shared;
            }
        }

        public void LoadModel(string filename)
        {
            // should get better performance by instantiating the prefab outside the coroutine
            var prefab = Instantiate(Template);
            var meshFilter = prefab.GetComponent<MeshFilter>();
            var meshRenderer = prefab.GetComponent<MeshRenderer>();

            directoryPath = Path.Combine(Path.Combine(Main.ModFolder, "BlenderModels"), filename);
            StartCoroutine(ConstructModel(filename, prefab, meshFilter, meshRenderer));
        }

        private IEnumerator ConstructModel(
            string filename, GameObject prefab, MeshFilter meshFilter, MeshRenderer meshRenderer)
        {
            if (!TryReadObjectFile(Path.Combine(directoryPath, $"{filename}.obj"), out var obj))
            {
                yield break;
            }

            if (!TryReadMaterialFile(Path.Combine(directoryPath, $"{filename}.mtl"), out var mtl))
            {
                yield break;
            }

            var prefabGuid = GetGuid(filename);

            meshFilter.mesh = PopulateMesh(obj);
            meshRenderer.materials = DefineMaterial(obj, mtl);
            meshRenderer.enabled = false;
            PrefabsByGuid[prefabGuid] = prefab;

            Main.Info($"Model {filename} loaded.");
        }

        private static Mesh PopulateMesh(ObjectFile obj)
        {
            var mesh = new Mesh();
            var triplets = new List<int[]>();
            var subMeshes = new List<int>();

            for (var i = 0; i < obj.f.Count; i += 1)
            {
                for (var j = 0; j < obj.f[i].Count; j += 1)
                {
                    triplets.Add(obj.f[i][j]);
                }

                subMeshes.Add(obj.f[i].Count);
            }

            var vertices = new Vector3[triplets.Count];
            var normals = new Vector3[triplets.Count];
            var uvs = new Vector2[triplets.Count];

            for (var i = 0; i < triplets.Count; i += 1)
            {
                vertices[i] = obj.v[triplets[i][0] - 1];
                normals[i] = obj.vn[triplets[i][2] - 1];

                if (triplets[i][1] > 0)
                {
                    uvs[i] = obj.vt[triplets[i][1] - 1];
                }
            }

            mesh.name = obj.o;
            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            mesh.SetUVs(0, uvs);
            mesh.subMeshCount = subMeshes.Count;

            var vertex = 0;

            for (var i = 0; i < subMeshes.Count; i += 1)
            {
                var triangles = new int[subMeshes[i]];

                for (var j = 0; j < subMeshes[i]; j += 1)
                {
                    triangles[j] = vertex;
                    vertex += 1;
                }

                mesh.SetTriangles(triangles, i);
            }

            mesh.RecalculateBounds();
            mesh.Optimize();

            return mesh;
        }

        [CanBeNull]
        private static Texture2D LoadTexture(string modelName, string name)
        {
            var directoryPath = Path.Combine(Path.Combine(Main.ModFolder, "BlenderModels"), modelName);
            var filename = Path.Combine(directoryPath, name);

            try
            {
                var textureData = File.ReadAllBytes(filename);
                var texture = new Texture2D(2, 2);

                if (!texture.LoadImage(textureData))
                {
                    Main.Error($"Failed to load texture from {filename}");
                }

                return texture;
            }
            catch
            {
                Main.Error($"Failed to open file {filename}");
            }

            return null;
        }

        private static Material[] DefineMaterial(ObjectFile obj, MaterialFile mtl)
        {
            var materials = new Material[obj.usemtl.Count];

            for (var i = 0; i < obj.usemtl.Count; i += 1)
            {
                var materialName = obj.usemtl[i];
                var newMtl = mtl.Materials.FirstOrDefault(x => x.newmtl == materialName);
                var material = new Material(Shader.Find("Standard")) { name = materialName };

                if (newMtl != null)
                {
                    material.SetFloat(Alpha, newMtl.d);
                    material.SetFloat(Glossiness, Mathf.Clamp01(newMtl.Ns / 1000f));
                    material.SetColor(AmbientColor, new Color(newMtl.Ka.x, newMtl.Ka.y, newMtl.Ka.z));
                    material.SetColor(SpecColor, new Color(newMtl.Ks.x, newMtl.Ks.y, newMtl.Ks.z));
                    material.SetColor(EmissionColor, new Color(newMtl.Ke.x, newMtl.Ke.y, newMtl.Ke.z));
                    material.SetInt(SrcBlend, (int)BlendMode.SrcAlpha);
                    material.SetInt(DstBlend, (int)BlendMode.OneMinusSrcAlpha);
                    material.SetInt(ZWrite, 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.renderQueue = 3000;

                    if (!string.IsNullOrEmpty(newMtl.map_Kd))
                    {
                        var mainTex = LoadTexture(obj.o, newMtl.map_Kd);

                        if (mainTex)
                        {
                            material.SetTexture(MainTex, mainTex);
                        }
                    }

                    if (!string.IsNullOrEmpty(newMtl.map_Bump))
                    {
                        var bumpMap = LoadTexture(obj.o, newMtl.map_Bump);

                        if (bumpMap)
                        {
                            material.SetTexture(BumpMap, bumpMap);
                            material.EnableKeyword("_NORMALMAP");
                        }
                    }
                }
                else
                {
                    Main.Error($"Cannot find material {materialName} in {obj.mtllib}.");
                }

                materials[i] = material;

                var materialGuid = GetGuid(materialName);

                MaterialsByGuid[materialGuid] = material;
            }

            return materials;
        }
    }

    #endregion

    #region Patch Helpers

    private static readonly Dictionary<string, GameObject> PrefabsByGuid = [];
    private static readonly Dictionary<string, Material> MaterialsByGuid = [];

    [CanBeNull]
    internal static GameObject GetPrefabByGuid([NotNull] string guid)
    {
        return PrefabsByGuid.TryGetValue(guid, out var prefab) ? prefab : null;
    }

    internal static Material GetMaterialByGuid([NotNull] string guid)
    {
        return MaterialsByGuid.TryGetValue(guid, out var material) ? material : null;
    }

    #endregion

    #region Blender Model Parser

    private sealed class MaterialFile
    {
        public readonly List<NewMtl> Materials = [];
    }

    private sealed class ObjectFile
    {
        // ReSharper disable file InconsistentNaming
        public string o;

        // ReSharper disable once NotAccessedField.Local
        public string mtllib;
        public List<string> usemtl;
        public List<Vector3> v;
        public List<Vector3> vn;
        public List<Vector2> vt;
        public List<List<int[]>> f;
    }
    // ReSharper enable file InconsistentNaming

    private sealed class NewMtl
    {
        // ReSharper disable file InconsistentNaming
        public string newmtl;
        public float Ns;
        public Vector3 Ka;
        [UsedImplicitly] public Vector3 Kd;
        public Vector3 Ks;
        public Vector3 Ke;
        [UsedImplicitly] public float Ni;
        public float d;
        [UsedImplicitly] public float illum;
        public string map_Kd;
        public string map_Bump;
    }
    // ReSharper enable file InconsistentNaming


    private static bool TryReadObjectFile(string path, out ObjectFile obj)
    {
        obj = new ObjectFile();

        try
        {
            var lines = File.ReadAllLines(path);

            obj.usemtl = [];
            obj.v = [];
            obj.vn = [];
            obj.vt = [];
            obj.f = [];

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                {
                    continue;
                }

                var token = line.Split(' ');

                switch (token[0])
                {
                    case "o":
                        obj.o = token[1];
                        break;
                    case "mtllib":
                        obj.mtllib = token[1];
                        break;
                    case "usemtl":
                        obj.usemtl.Add(token[1]);
                        obj.f.Add([]);
                        break;
                    case "v":
                        obj.v.Add(new Vector3(
                            float.Parse(token[1]),
                            float.Parse(token[2]),
                            float.Parse(token[3])));
                        break;
                    case "vn":
                        obj.vn.Add(new Vector3(
                            float.Parse(token[1]),
                            float.Parse(token[2]),
                            float.Parse(token[3])));
                        break;
                    case "vt":
                        obj.vt.Add(new Vector3(
                            float.Parse(token[1]),
                            float.Parse(token[2])));
                        break;
                    case "f":
                        for (var i = 1; i < 4; i += 1)
                        {
                            var triplet = Array.ConvertAll(token[i].Split('/'),
                                x => string.IsNullOrEmpty(x) ? 0 : int.Parse(x));
                            obj.f[obj.f.Count - 1].Add(triplet);
                        }

                        break;
                }
            }

            return true;
        }
        catch
        {
            Main.Error($"Failed to read object file {path}");

            return false;
        }
    }

    private static bool TryReadMaterialFile(string path, out MaterialFile mtl)
    {
        mtl = new MaterialFile();

        try
        {
            var lines = File.ReadAllLines(path);

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                {
                    continue;
                }

                var token = line.Split(' ');

                switch (token[0])
                {
                    case "newmtl":
                        mtl.Materials.Add(new NewMtl { newmtl = token[1] });
                        break;
                    case "Ns":
                        mtl.Materials[mtl.Materials.Count - 1].Ns = float.Parse(token[1]);
                        break;
                    case "Ka":
                        mtl.Materials[mtl.Materials.Count - 1].Ka =
                            new Vector3(float.Parse(token[1]), float.Parse(token[2]), float.Parse(token[3]));
                        break;
                    case "Kd":
                        mtl.Materials[mtl.Materials.Count - 1].Kd =
                            new Vector3(float.Parse(token[1]), float.Parse(token[2]), float.Parse(token[3]));
                        break;
                    case "Ks":
                        mtl.Materials[mtl.Materials.Count - 1].Ks =
                            new Vector3(float.Parse(token[1]), float.Parse(token[2]), float.Parse(token[3]));
                        break;
                    case "Ke":
                        mtl.Materials[mtl.Materials.Count - 1].Ke =
                            new Vector3(float.Parse(token[1]), float.Parse(token[2]), float.Parse(token[3]));
                        break;
                    case "Ni":
                        mtl.Materials[mtl.Materials.Count - 1].Ni = float.Parse(token[1]);
                        break;
                    case "d":
                        mtl.Materials[mtl.Materials.Count - 1].d = float.Parse(token[1]);
                        break;
                    case "ileum":
                        mtl.Materials[mtl.Materials.Count - 1].illum = float.Parse(token[1]);
                        break;
                    case "map_Kd":
                        mtl.Materials[mtl.Materials.Count - 1].map_Kd = token[1];
                        break;
                    case "map_Bump":
                        mtl.Materials[mtl.Materials.Count - 1].map_Bump = token[1];
                        break;
                }
            }

            return true;
        }
        catch
        {
            Main.Error($"Failed to read material file {path}");

            return false;
        }
    }

    #endregion
}
