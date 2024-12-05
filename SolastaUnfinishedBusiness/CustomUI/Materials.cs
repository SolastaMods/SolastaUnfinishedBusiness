using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Resources = SolastaUnfinishedBusiness.Properties.Resources;

// To Do:  Allow multiple materials per prefab based on .mtl file
public static class Materials
{
    #region Materials

    private static readonly Dictionary<string, Material> MaterialsByGuid = new();

    public static Material GetMaterialByGuid([NotNull] string guid)
    {
        return MaterialsByGuid.TryGetValue(guid, out var material) ? material : null;
    }

    private static string GetMaterialGuid(string name)
    {
        return GuidHelper.Create(DefinitionBuilder.CeNamespaceGuid, name).ToString();
    }

    public static Material GetOrCreateMaterial(string name, string resourceKey)
    {
        var guid = GetMaterialGuid(name);

        if (MaterialsByGuid.TryGetValue(guid, out var material))
        {
            return material;
        }

        // Access the resource from .resx using the provided resource key
        var mtlData = Resources.ResourceManager.GetString(resourceKey);
        if (string.IsNullOrEmpty(mtlData))
        {
            Debug.LogError($"Resource not found: {name}:{resourceKey}");
            return null;
        }
        
        material = LoadMaterialFromFile(mtlData);
        if (material == null)
        {
            Debug.LogError($"Failed to create material from GetOrCreateMaterial: {name}:{resourceKey}");
            return null;
        }

        MaterialsByGuid[guid] = material; 

        return material;
    }

    private static Material LoadMaterialFromFile(string mtlData)
    {
        Material material = null;
        string currentMaterialName = null;
        string diffuseTextureKey = null;
        string normalMapKey = null;

        try
        {
            var lines = mtlData.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                if (trimmedLine.StartsWith("newmtl"))
                {
                    if (currentMaterialName != null && material != null)
                    {
                        ApplyTexturesToMaterial(material, diffuseTextureKey, normalMapKey);
                        MaterialsByGuid[currentMaterialName] = material;
                    }

                    currentMaterialName = trimmedLine.Substring(7).Trim();
                    material = new Material(Shader.Find("Standard"));
                    diffuseTextureKey = null;
                    normalMapKey = null;
                }
                else if (trimmedLine.StartsWith("Ns"))
                {
                    if (float.TryParse(trimmedLine.Substring(3), NumberStyles.Float, CultureInfo.InvariantCulture, out var nsValue))
                    {
                        material.SetFloat("_Glossiness", Mathf.Clamp01(nsValue / 1000f)); // Scale Ns to Unity range
                    }
                }
                else if (trimmedLine.StartsWith("Ka"))
                {
                    var kaValues = ParseVector3(trimmedLine.Substring(3));
                    if (kaValues != Vector3.zero)
                    {
                        material.SetColor("_AmbientColor", new Color(kaValues.x, kaValues.y, kaValues.z));
                    }
                }
                else if (trimmedLine.StartsWith("Ks"))
                {
                    var ksValues = ParseVector3(trimmedLine.Substring(3));
                    if (ksValues != Vector3.zero)
                    {
                        material.SetColor("_SpecColor", new Color(ksValues.x, ksValues.y, ksValues.z));
                    }
                }
                else if (trimmedLine.StartsWith("Ke"))
                {
                    var keValues = ParseVector3(trimmedLine.Substring(3));
                    if (keValues != Vector3.zero)
                    {
                        material.SetColor("_EmissionColor", new Color(keValues.x, keValues.y, keValues.z));
                        material.EnableKeyword("_EMISSION");
                    }
                }
                else if (trimmedLine.StartsWith("d"))
                {
                    if (float.TryParse(trimmedLine.Substring(2), NumberStyles.Float, CultureInfo.InvariantCulture, out var dValue))
                    {
                        material.SetFloat("_Alpha", dValue);
                        if (dValue < 1.0f)
                        {
                            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                            material.SetInt("_ZWrite", 0);
                            material.DisableKeyword("_ALPHATEST_ON");
                            material.EnableKeyword("_ALPHABLEND_ON");
                            material.renderQueue = 3000;
                        }
                    }
                }
                else if (trimmedLine.StartsWith("map_Kd"))
                {
                    var fullPath = trimmedLine.Substring(7).Trim();
                    diffuseTextureKey = Path.GetFileNameWithoutExtension(fullPath);
                }
                else if (trimmedLine.StartsWith("map_Bump") || trimmedLine.StartsWith("bump"))
                {
                    var fullPath = trimmedLine.Substring(trimmedLine.IndexOf(' ') + 1).Trim();
                    normalMapKey = Path.GetFileNameWithoutExtension(fullPath);
                }
            }

            if (currentMaterialName != null && material != null)
            {
                ApplyTexturesToMaterial(material, diffuseTextureKey, normalMapKey);
                MaterialsByGuid[currentMaterialName] = material;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error parsing MTL resource: {ex.Message}");
            return null;
        }
        return material;
    }

    private static void ApplyTexturesToMaterial(Material material, string diffuseTextureKey, string normalMapKey)
    {
        if (!string.IsNullOrEmpty(diffuseTextureKey))
        {
            var diffuseTexture = GetOrCreateTexture(diffuseTextureKey, diffuseTextureKey);
            if (diffuseTexture != null)
            {
                material.SetTexture("_MainTex", diffuseTexture);
            }
            else
            {
                Debug.LogError($"_MainTex is null at ApplyTexturesToMaterial for {material} : {diffuseTextureKey}");
            }
        }

        if (!string.IsNullOrEmpty(normalMapKey))
        {
            var normalMap = GetOrCreateTexture(normalMapKey, normalMapKey);
            if (normalMap != null)
            {
                material.SetTexture("_BumpMap", normalMap);
                material.EnableKeyword("_NORMALMAP");
            }
            else
            {
                Debug.LogError($"_BumpMap is null at ApplyTexturesToMaterial for {material} : {normalMapKey}");
            }
        }
    }

    // public static void ValidateAndRecalculateMeshNormals(Mesh mesh)
    //{
    //    if (mesh.vertices.Length != mesh.normals.Length)
    //    {
    //        mesh.RecalculateNormals();
    //    }
    //}

    private static Vector3 ParseVector3(string input)
    {
        var values = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (values.Length == 3 &&
            float.TryParse(values[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var x) &&
            float.TryParse(values[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var y) &&
            float.TryParse(values[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var z))
        {
            return new Vector3(x, y, z);
        }

        return Vector3.zero;
    }

    #endregion

    #region Textures

    private static readonly Dictionary<string, Texture2D> TexturesByGuid = new();

    private static string GetTextureGuid(string name)
    {
        return GuidHelper.Create(DefinitionBuilder.CeNamespaceGuid, name).ToString();
    }

    public static Texture2D GetOrCreateTexture(string name, string resourceKey)
    {
        var guid = GetTextureGuid(name);

        if (TexturesByGuid.TryGetValue(guid, out var texture))
        {
            return texture;
        }

        // Access the resource from .resx using the provided resource key
        Byte[] texData = Resources.ResourceManager.GetObject(resourceKey) as Byte[];
        if (texData == null)
        {
            Debug.LogError($"Resource not found at GetOrCreateTexture: {name}:{resourceKey}");
            return null;
        }

        texture = LoadTextureFromFile(texData);
        if (texture == null)
        {
            Debug.LogError($"Failed to load Texture from GetOrCreateTexture: {name}:{resourceKey}");
            return null;
        }

        TexturesByGuid[guid] = texture;

        return texture;
    }

    private static Texture2D LoadTextureFromFile(Byte[] texData)
    {

        try
        {
            if (texData != null)
            {
                Texture2D texture = new Texture2D(2, 2); // Create a new texture
                if (texture.LoadImage(texData)) // Load the image from byte array
                {
                    return texture;
                }
                else
                {
                    Debug.LogError($"Failed to load texture from LoadTextureFromFile. The data is invalid: {texData}");
                }
            }
            else
            {
                Debug.LogError($"Texture file empty or not found and LoadTextureFromFile");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Unexpected error while loading texture with contents {texData}. Error: {ex.Message}");
        }

        return null;
    }

    #endregion
}


