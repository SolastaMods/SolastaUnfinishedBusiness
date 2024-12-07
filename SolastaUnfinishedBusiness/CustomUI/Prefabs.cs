using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Resources = SolastaUnfinishedBusiness.Properties.Resources;

namespace SolastaUnfinishedBusiness.CustomUI;

public static class Prefabs
{
    #region Custom Prefabs

    private static readonly Dictionary<string, GameObject> PrefabsByGuid = new();

    // Gets or loads a prefab by its GUID.
    [CanBeNull]
    public static GameObject GetPrefabByGuid([NotNull] string guid)
    {
        return PrefabsByGuid.TryGetValue(guid, out var prefab) ? prefab : null;
    }
    // Gets or creates a prefab and returns an asset reference
    [NotNull]
    private static AssetReference GetPrefab(string name, string resourceName)
    {
        var guid = GetPrefabGuid(name);

        if (!PrefabsByGuid.ContainsKey(guid))
        {
            GetOrCreatePrefab(name, resourceName);
        }

        return new AssetReference(guid);
    }

    // Loads or creates a prefab from an embedded .obj resource.
    private static void GetOrCreatePrefab(string name, string resourceKey)
    {
        var guid = GetPrefabGuid(name);
        Debug.LogWarning($"{name} {guid}");
        if (PrefabsByGuid.TryGetValue(guid, out _))
        {
            return;
        }

        // Access the resource from .resx using the provided resource key
        if (Resources.ResourceManager.GetObject(resourceKey) is not byte[] fileBytes)
        {
            Debug.LogError($"Resource not found: {resourceKey}");
            return;
        }
        var objData = Encoding.UTF8.GetString(fileBytes);
        if (string.IsNullOrEmpty(objData))
        {
            Debug.LogError($"Resource not found: {resourceKey}");
            return;
        }

        var mesh = LoadMeshFromFile(objData, out var materialKey);
        if (!mesh)
        {
            Debug.LogError($"Failed to create mesh from resource: {resourceKey}");
            return;
        }

        var prefab = new GameObject(name);
        var meshFilter = prefab.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        // Add a MeshRenderer to the prefab and assign the material
        var meshRenderer = prefab.AddComponent<MeshRenderer>();
        if (!string.IsNullOrEmpty(materialKey))
        {
            //var material = Materials.GetMaterialByGuid(materialKey);
            var material = Materials.GetOrCreateMaterial(materialKey, materialKey);
            if (material != null)
            {
                meshRenderer.material = material;
            }
            else
            {
                Debug.LogWarning($"Material not found in Mesh Renderer for GetMaterialByGuid lookup: {materialKey}");
            }
        }

        PrefabsByGuid[guid] = prefab;
    }

    // Generates a GUID for a prefab based on its name.
    private static string GetPrefabGuid(string name)
    {
        return GuidHelper.Create(DefinitionBuilder.CeNamespaceGuid, name).ToString();
    }

    // Loads a mesh from .obj data.
    private static Mesh LoadMeshFromFile(string objText, out string materialKey)
    {
        var mesh = new Mesh();
        var vertices = new List<Vector3>();
        var normals = new List<Vector3>();
        var uv = new List<Vector2>();
        var triangles = new List<int>();
        materialKey = null;

        // A dictionary to track already-seen (vertex, normal, uv) triplets.
        var uniqueVertices = new Dictionary<(Vector3, Vector3, Vector2), int>();

        try
        {
            // Split the objText into lines
            var lines = objText.Split(['\n'], StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                if (line.StartsWith("mtllib "))
                {
                    // Handle material library
                    var parts = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 1)
                    {
                        materialKey = Path.GetFileNameWithoutExtension(parts[1].Replace("\r", "").Replace("\n", "").Replace("..", "")); // Trim line endings
                    }
                }
                else if (line.StartsWith("v "))
                {
                    // Parse vertices
                    var parts = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);
                    vertices.Add(new Vector3(
                        float.Parse(parts[1]),
                        float.Parse(parts[2]),
                        float.Parse(parts[3])));
                }
                else if (line.StartsWith("vn "))
                {
                    // Parse normals
                    var parts = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);
                    normals.Add(new Vector3(
                        float.Parse(parts[1]),
                        float.Parse(parts[2]),
                        float.Parse(parts[3])));
                }
                else if (line.StartsWith("vt "))
                {
                    // Parse UVs
                    var parts = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);
                    uv.Add(new Vector2(
                        float.Parse(parts[1]),
                        float.Parse(parts[2])));
                }
                else if (line.StartsWith("f "))
                {
                    // Parse faces
                    var parts = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 1; i < parts.Length; i++)
                    {
                        var part = parts[i];
                        var vertexData = part.Split('/');

                        // Extract indices (1-based)
                        int vertexIndex = int.Parse(vertexData[0]) - 1;
                        int uvIndex = vertexData.Length > 1 && !string.IsNullOrEmpty(vertexData[1]) ? int.Parse(vertexData[1]) - 1 : -1;
                        int normalIndex = vertexData.Length > 2 && !string.IsNullOrEmpty(vertexData[2]) ? int.Parse(vertexData[2]) - 1 : -1;

                        // Track the unique combination of vertex, normal, and uv
                        var vertexTuple = (vertices[vertexIndex], normals[normalIndex], uv[uvIndex]);

                        // If this combination hasn't been added before, add it
                        if (!uniqueVertices.TryGetValue(vertexTuple, out var value))
                        {
                            value = uniqueVertices.Count;
                            uniqueVertices[vertexTuple] = value;
                        }

                        triangles.Add(value);
                    }
                }
            }

            // Create the mesh with the parsed data
            var finalVertices = new List<Vector3>();
            var finalNormals = new List<Vector3>();
            var finalUVs = new List<Vector2>();

            foreach (var key in uniqueVertices.Keys)
            {
                finalVertices.Add(key.Item1);
                finalNormals.Add(key.Item2);
                finalUVs.Add(key.Item3);
            }

            mesh.SetVertices(finalVertices);
            mesh.SetNormals(finalNormals);
            mesh.SetUVs(0, finalUVs);
            mesh.SetTriangles(triangles, 0);

            Debug.Log($"Mesh vertices: {finalVertices.Count}, normals: {finalNormals.Count}, UVs: {finalUVs.Count}, Triangles: {triangles.Count}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error parsing .obj data: {ex.Message}");
            return null;
        }

        return mesh;
    }
    #endregion

    #region Weapon Prefabs

    #region Katana Prefabs

    private static AssetReference _katanaPrefab;

    public static AssetReference GetKatanaPrefab()
    {
        return _katanaPrefab ??= GetPrefab(
            "prefab_katana",
            "prefab_katana"
        );
    }

    #endregion

    #region Guantlet Prefabs

    private static AssetReference _launcherPrefab;

    public static AssetReference GetLauncherPrefab()
    {
        return _launcherPrefab ??= GetPrefab(
            "prefab_lightning_launcher",
            "prefab_lightning_launcher"
        );
    }

    private static AssetReference _thunderGauntletPrefab;

    public static AssetReference GetThunderGauntletPrefab()
    {
        return _thunderGauntletPrefab ??= GetPrefab(
            "prefab_thunder_gauntlet",
            "prefab_thunder_gauntlet"
        );
    }

    #endregion

    #region Long Mace Prefabs

    private static AssetReference _longMacePrefab;

    public static AssetReference GetLongMacePrefab()
    {
        return _longMacePrefab ??= GetPrefab(
            "prefab_longmace",
            "prefab_longmace"
        );
    }

    #endregion

    #region Pike Prefabs

    private static AssetReference _pikePrefab;

    public static AssetReference GetPikePrefab()
    {
        return _pikePrefab ??= GetPrefab(
            "prefab_pike",
            "prefab_pike"
        );
    }

    #endregion

    #endregion
}
