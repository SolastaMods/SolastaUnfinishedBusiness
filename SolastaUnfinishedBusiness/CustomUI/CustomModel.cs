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

public static class CustomModel
{
    private static readonly Dictionary<string, GameObject> PrefabsByGuid = [];
    private static readonly Dictionary<string, Texture2D> TexturesByGuid = [];

    // Gets or creates a prefab and returns an asset reference
    [NotNull]
    internal static AssetReference GetCustomModelPrefab(string name)
    {
        var guid = GetPrefabGuid(name);

        if (!PrefabsByGuid.ContainsKey(guid))
        {
            ObjectLoader.Shared.LoadModel(name);
        }

        return new AssetReference(guid);
    }

    private static string GetPrefabGuid(string name)
    {
        return GuidHelper.Create(DefinitionBuilder.CeNamespaceGuid, name).ToString();
    }

    private static ObjectFile ReadObjectFile(string path)
    {
        var obj = new ObjectFile();
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

        return obj;
    }

    private static MaterialFile ReadMaterialFile(string path)
    {
        var mtl = new MaterialFile();
        var lines = File.ReadAllLines(path);

        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
            {
                continue;
            }

            var token = line.Split(' ');
            NewMtl currentMaterial;

            switch (token[0])
            {
                case "newmtl":
                    currentMaterial = new NewMtl { newmtl = token[1] };
                    mtl.Materials.Add(currentMaterial);
                    break;
                case "Ns":
                    currentMaterial.Ns = float.Parse(token[1]);
                    break;
                case "Ka":
                    currentMaterial.Ka =
                        new Vector3(float.Parse(token[1]), float.Parse(token[2]), float.Parse(token[3]));
                    break;
                case "Kd":
                    currentMaterial.Kd =
                        new Vector3(float.Parse(token[1]), float.Parse(token[2]), float.Parse(token[3]));
                    break;
                case "Ks":
                    currentMaterial.Ks =
                        new Vector3(float.Parse(token[1]), float.Parse(token[2]), float.Parse(token[3]));
                    break;
                case "Ke":
                    currentMaterial.Ke =
                        new Vector3(float.Parse(token[1]), float.Parse(token[2]), float.Parse(token[3]));
                    break;
                case "Ni":
                    currentMaterial.Ni = float.Parse(token[1]);
                    break;
                case "d":
                    currentMaterial.d = float.Parse(token[1]);
                    break;
                case "ileum":
                    currentMaterial.illum = float.Parse(token[1]);
                    break;
                case "map_Kd":
                    currentMaterial.map_Kd = token[1];
                    break;
                case "map_Bump":
                    currentMaterial.map_Bump = token[1];
                    break;
            }
        }

        return mtl;
    }

    internal sealed class ObjectLoader : MonoBehaviour
    {
        private static ObjectLoader _shared;
        public string directoryPath;

        [NotNull]
        internal static ObjectLoader Shared
        {
            get
            {
                if (_shared)
                {
                    return _shared;
                }

                _shared = new GameObject().AddComponent<ObjectLoader>();
                DontDestroyOnLoad(_shared.gameObject);

                return _shared;
            }
        }

        public void LoadModel(string filename)
        {
            directoryPath = Path.Combine(Path.Combine(Main.ModFolder, "BlenderModels"), filename);
            StartCoroutine(ConstructModel(filename));
        }

        private IEnumerator ConstructModel(string filename)
        {
            var obj = ReadObjectFile(Path.Combine(directoryPath, $"{filename}.obj"));
            var mtl = ReadMaterialFile(Path.Combine(directoryPath, $"{filename}.mtl"));
            var prefab = new GameObject(name);

            yield return null;

            var meshFilter = prefab.AddComponent<MeshFilter>();
            var meshRenderer = prefab.AddComponent<MeshRenderer>();

            yield return null;

            meshFilter.mesh = PopulateMesh(obj);
            meshRenderer.materials = DefineMaterial(obj, mtl);

            var guid = GetPrefabGuid(filename);

            PrefabsByGuid[guid] = prefab;
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

        private static Material[] DefineMaterial(ObjectFile obj, MaterialFile mtl)
        {
            var materials = new Material[obj.usemtl.Count];

            for (var i = 0; i < obj.usemtl.Count; i += 1)
            {
                var materialName = obj.usemtl[i];
                var newMtl = mtl.Materials.FirstOrDefault(x => x.newmtl == materialName);
                var material = new Material(Shader.Find("Standard")) { name = materialName };

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

                ApplyTexturesToMaterial(material, newMtl.map_Kd, newMtl.map_Bump);

                materials[i] = material;
            }

            return materials;
        }
    }

    private readonly struct MaterialFile
    {
        public readonly List<NewMtl> Materials = [];

        public MaterialFile()
        {
        }
    }

    private struct ObjectFile
    {
        // ReSharper disable file InconsistentNaming
        public string o;
        public string mtllib;
        public List<string> usemtl;
        public List<Vector3> v;
        public List<Vector3> vn;
        public List<Vector2> vt;
        public List<List<int[]>> f;
    }
    // ReSharper enable file InconsistentNaming

    private struct NewMtl
    {
        // ReSharper disable file InconsistentNaming
        public string newmtl;
        public float Ns;
        public Vector3 Ka;
        public Vector3 Kd;
        public Vector3 Ks;
        public Vector3 Ke;
        public float Ni;
        public float d;
        public float illum;
        public string map_Kd;
        public string map_Bump;
    }
    // ReSharper enable file InconsistentNaming

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

    private static void ApplyTexturesToMaterial(Material material, string diffuseTextureKey, string normalMapKey)
    {
        if (!string.IsNullOrEmpty(normalMapKey))
        {
            var normalMap = GetOrCreateTexture(material.name, normalMapKey);

            if (normalMap)
            {
                material.SetTexture(BumpMap, normalMap);
                material.EnableKeyword("_NORMALMAP");
            }
        }
                
        // ReSharper disable once InvertIf
        if (!string.IsNullOrEmpty(diffuseTextureKey))
        {
            var diffuseTexture = GetOrCreateTexture(material.name,diffuseTextureKey);

            if (diffuseTexture)
            {
                material.SetTexture(MainTex, diffuseTexture);
            }
        }
    }

    [CanBeNull]
    private static Texture2D GetOrCreateTexture(string modelName, string name)
    {
        var guid = GuidHelper.Create(DefinitionBuilder.CeNamespaceGuid, name).ToString();

        if (TexturesByGuid.TryGetValue(guid, out var texture))
        {
            return texture;
        }
        
        try
        {
            var directory = Path.Combine(Path.Combine(Main.ModFolder, "BlenderModels"), modelName);
            var filename = Path.Combine(directory, name);
            var texData = File.ReadAllBytes(filename);
            
            texture = new Texture2D(2, 2);

            if (!texture.LoadImage(texData))
            {
                Main.Error($"Failed to load texture from LoadTextureFromFile:\n {texData}");
            }
        }
        catch
        {
            Main.Error($"Failed to load texture from {name}.");
        }

        TexturesByGuid[guid] = texture;
        
        return texture;
    }
}
