using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.CustomUI;

public static class CustomModel
{
    private static readonly Dictionary<string, GameObject> PrefabsByGuid = [];
    
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

        mtl.newmtl = [];
        mtl.mapKd = [];

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
                    mtl.newmtl.Add(token[1]);
                    break;
                case "map_Kd":
                    mtl.mapKd.Add(token[1]);
                    break;
                case "map_Bump":
                    mtl.mapBump.Add(token[1]);
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
            var meshFilter = prefab.AddComponent<MeshFilter>();
            var meshRenderer = prefab.AddComponent<MeshRenderer>();

            meshFilter.mesh = PopulateMesh(obj);
            meshRenderer.materials = DefineMaterial(obj, mtl);

            var guid = GetPrefabGuid(filename);

            PrefabsByGuid[guid] = prefab;

            yield break;
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
            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uvs;
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

        private Material[] DefineMaterial(ObjectFile obj, MaterialFile mtl)
        {
            var materials = new Material[obj.usemtl.Count];

            for (var i = 0; i < obj.usemtl.Count; i += 1)
            {
                var index = mtl.newmtl.IndexOf(obj.usemtl[i]);
                var texture = new Texture2D(1, 1);

                texture.LoadImage(File.ReadAllBytes(Path.Combine(directoryPath, mtl.mapKd[index])));

                materials[i] = new Material(Shader.Find("Diffuse")) { name = mtl.newmtl[index], mainTexture = texture };
            }

            return materials;
        }
    }

    // ReSharper disable file InconsistentNaming
    private struct ObjectFile
    {
        public string o;
        public string mtllib;
        public List<string> usemtl;
        public List<Vector3> v;
        public List<Vector3> vn;
        public List<Vector2> vt;
        public List<List<int[]>> f;
    }

    private struct MaterialFile
    {
        public List<string> newmtl;
        public List<string> mapKd;
        public List<string> mapBump;
    }
    // ReSharper enable file InconsistentNaming
}
