using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomUI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace SolastaUnfinishedBusiness.Models;

internal static class ResourceLocatorContext
{
    internal static void Load()
    {
        // Add our resource provider - no internal API?
        Addressables.ResourceManager
            .GetField<IList<IResourceProvider>>("m_ResourceProviders")
            .TryAdd(SpriteResourceProvider.Instance);

        // Add our resource locator - there is a internal API.
        Addressables.AddResourceLocator(SpriteResourceLocator.Instance);
    }
}

// ResourceProvider provides the resource given the resource location
internal sealed class SpriteResourceProvider : ResourceProviderBase
{
    private SpriteResourceProvider() { }
    internal static SpriteResourceProvider Instance { get; } = new();

    public override void Provide(ProvideHandle provideHandle)
    {
        var location = (SpriteResourceLocation)provideHandle.Location;

        provideHandle.Complete(location.Sprite, true, null);
    }

    public override bool CanProvide(Type t, IResourceLocation location)
    {
        var canProvide = base.CanProvide(t, location);

        return canProvide;
    }

    [NotNull]
    public override Type GetDefaultType(IResourceLocation location)
    {
        return typeof(Sprite);
    }
}

// ResourceLocator returns location of resource
internal sealed class SpriteResourceLocator : IResourceLocator
{
    private static readonly Dictionary<string, SpriteResourceLocation> LocationsCache = [];
    private static readonly List<IResourceLocation> EmptyList = [];

    private SpriteResourceLocator() { }

    internal static SpriteResourceLocator Instance { get; } = new();

    // These two properties don't seem to be used
    [CanBeNull] public string LocatorId => GetType().FullName;
    [NotNull] public IEnumerable<object> Keys => LocationsCache.Keys;

    public bool Locate([NotNull] object key, Type type, out IList<IResourceLocation> locations)
    {
        var id = key.ToString();
        var sprite = Sprites.GetSpriteByGuid(id);

        if (sprite != null)
        {
            if (!LocationsCache.TryGetValue(id, out var location))
            {
                location = new SpriteResourceLocation(sprite, sprite.name, id);
                LocationsCache.Add(id, location);
            }

            locations = new List<IResourceLocation> { location };
            return true;
        }

        locations = EmptyList;
        return false;
    }
}

// ResourceLocation of sprite used by ResourceProvider.  We're using it to directly hold the sprite.
internal sealed class SpriteResourceLocation : ResourceLocationBase
{
    internal SpriteResourceLocation(Sprite sprite, string name, string id)
        : base(name, id, typeof(SpriteResourceProvider).FullName, typeof(Sprite))
    {
        Sprite = sprite;
    }

    internal Sprite Sprite { get; }
}
