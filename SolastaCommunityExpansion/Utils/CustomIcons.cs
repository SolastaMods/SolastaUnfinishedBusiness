using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Diagnostics;
using SolastaCommunityExpansion.Builders;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityGraphics = UnityEngine.Graphics;

namespace SolastaCommunityExpansion.Utils;

internal static class CustomIcons
{
    private static readonly Dictionary<string, Sprite> SpritesByGuid = new();

    /// <summary>
    ///     Convert a bitmap stored as an embedded resource to a Sprite.
    /// </summary>
    [NotNull]
    internal static Sprite GetOrCreateSprite(
        string name,
        Byte[] bitmap,
        int size)
    {
        return GetOrCreateSprite(name, bitmap, size, size);
    }

    [NotNull]
    private static Sprite GetOrCreateSprite(
        string name,
        Byte[] bitmap,
        int sizeX,
        int sizeY)
    {
        var (id, guid) = GetSpriteId(name, sizeX, sizeY);

        if (SpritesByGuid.TryGetValue(guid, out var sprite))
        {
            throw new SolastaCommunityExpansionException(
                $"A sprite with name {name} and size [{sizeX},{sizeY}] already exists.");
        }

        var texture = new Texture2D(sizeX, sizeY, TextureFormat.DXT5, false);

        texture.LoadImage(bitmap);
        sprite = Sprite.Create(texture, new Rect(0, 0, sizeX, sizeY), new Vector2(0, 0));

        SpritesByGuid[guid] = sprite;
        sprite.name = id;

        return sprite;
    }

    [CanBeNull]
    internal static Sprite GetSpriteByGuid([NotNull] string guid)
    {
        return SpritesByGuid.TryGetValue(guid, out var sprite) ? sprite : null;
    }

    /// <summary>
    ///     Create a unique Id to serve as id of a sprite in our internal cache and as the guid for AssetReference
    /// </summary>
    /// <param name="name"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private static (string id, string guid) GetSpriteId(string name, int x, int y)
    {
        var id = $"_CE_{name}[{x},{y}]";

        return (id, GetSpriteGuid(id));
    }

    [NotNull]
    private static string GetSpriteGuid([NotNull] string id)
    {
        return GuidHelper.Create(DefinitionBuilder.CENamespaceGuid, id).ToString("n");
    }

    [NotNull]
    internal static AssetReferenceSprite CreateAssetReferenceSprite(
        string name,
        Byte[] bitmap,
        int size)
    {
        return CreateAssetReferenceSprite(name, bitmap, size, size);
    }

    [NotNull]
    internal static AssetReferenceSprite CreateAssetReferenceSprite(
        string name,
        Byte[] bitmap,
        int sizeX,
        int sizeY)
    {
        var sprite = GetOrCreateSprite(name, bitmap, sizeX, sizeY);
        return new AssetReferenceSprite(GetSpriteGuid(sprite.name));
    }
}
