using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomUI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace SolastaUnfinishedBusiness.Api.Helpers;

internal static class ResourceLocatorHelper
{
    internal static void Load()
    {
        // Add our Sprite resource provider
        Addressables.ResourceManager
            .GetField<IList<IResourceProvider>>("m_ResourceProviders")
            .TryAdd(SpriteResourceProvider.Instance);

        // Add our Sprite resource locator
        Addressables.AddResourceLocator(SpriteResourceLocator.Instance);

        // Add our Prefab resource provider
        Addressables.ResourceManager
            .GetField<IList<IResourceProvider>>("m_ResourceProviders")
            .TryAdd(PrefabResourceProvider.Instance);

        // Add our Prefab resource locator
        Addressables.AddResourceLocator(PrefabResourceLocator.Instance);

        // Add our Material resource provider
        Addressables.ResourceManager
            .GetField<IList<IResourceProvider>>("m_ResourceProviders")
            .TryAdd(MaterialResourceProvider.Instance);

        // Add our Material resource locator
        Addressables.AddResourceLocator(MaterialResourceLocator.Instance);
    }
}

#region Custom Sprite Resources

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

        if (sprite)
        {
            if (!LocationsCache.TryGetValue(id, out var location))
            {
                location = new SpriteResourceLocation(sprite, sprite.name, id);
                LocationsCache.Add(id, location);
            }

            locations = [location];

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

#endregion

#region Custom Prefab Resources

// ResourceProvider provides the prefab given the resource location
internal sealed class PrefabResourceProvider : ResourceProviderBase
{
    private PrefabResourceProvider() { }
    internal static PrefabResourceProvider Instance { get; } = new();

    public override void Provide(ProvideHandle provideHandle)
    {
        var location = (PrefabResourceLocation)provideHandle.Location;

        provideHandle.Complete(location.Prefab, true, null);
    }

    public override bool CanProvide(Type t, IResourceLocation location)
    {
        var canProvide = t == typeof(GameObject) && location is PrefabResourceLocation;

        return canProvide;
    }

    [NotNull]
    public override Type GetDefaultType(IResourceLocation location)
    {
        return typeof(GameObject);
    }
}

// ResourceLocator returns location of prefab resource
internal sealed class PrefabResourceLocator : IResourceLocator
{
    private static readonly Dictionary<string, PrefabResourceLocation> LocationsCache = [];
    private static readonly List<IResourceLocation> EmptyList = [];

    private PrefabResourceLocator() { }

    internal static PrefabResourceLocator Instance { get; } = new();

    [CanBeNull] public string LocatorId => GetType().FullName;
    [NotNull] public IEnumerable<object> Keys => LocationsCache.Keys;

    public bool Locate([NotNull] object key, Type type, out IList<IResourceLocation> locations)
    {
        var id = key.ToString();
        var prefab = CustomModels.GetPrefabByGuid(id);

        if (prefab)
        {
            if (!LocationsCache.TryGetValue(id, out var location))
            {
                location = new PrefabResourceLocation(prefab, prefab.name, id);
                LocationsCache.Add(id, location);
            }

            locations = [location];

            return true;
        }

        locations = EmptyList;

        return false;
    }
}

// ResourceLocation of prefab used by ResourceProvider
internal sealed class PrefabResourceLocation : ResourceLocationBase
{
    internal PrefabResourceLocation(GameObject prefab, string name, string id)
        : base(name, id, typeof(PrefabResourceProvider).FullName, typeof(GameObject))
    {
        Prefab = prefab;
    }

    internal GameObject Prefab { get; }
}

#endregion

#region Custom Material Resources

// ResourceProvider provides the Material given the resource location
internal sealed class MaterialResourceProvider : ResourceProviderBase
{
    private MaterialResourceProvider() { }
    internal static MaterialResourceProvider Instance { get; } = new();

    public override void Provide(ProvideHandle provideHandle)
    {
        var location = (MaterialResourceLocation)provideHandle.Location;

        provideHandle.Complete(location.Material, true, null);
    }

    public override bool CanProvide(Type t, IResourceLocation location)
    {
        var canProvide = t == typeof(Material) && location is MaterialResourceLocation;

        return canProvide;
    }

    [NotNull]
    public override Type GetDefaultType(IResourceLocation location)
    {
        return typeof(Material);
    }
}

// ResourceLocator returns location of Material resource
internal sealed class MaterialResourceLocator : IResourceLocator
{
    private static readonly Dictionary<string, MaterialResourceLocation> LocationsCache = [];
    private static readonly List<IResourceLocation> EmptyList = [];

    private MaterialResourceLocator() { }

    internal static MaterialResourceLocator Instance { get; } = new();

    [CanBeNull] public string LocatorId => GetType().FullName;
    [NotNull] public IEnumerable<object> Keys => LocationsCache.Keys;

    public bool Locate([NotNull] object key, Type type, out IList<IResourceLocation> locations)
    {
        var id = key.ToString();
        var material = CustomModels.GetMaterialByGuid(id);

        if (material)
        {
            if (!LocationsCache.TryGetValue(id, out var location))
            {
                location = new MaterialResourceLocation(material, material.name, id);
                LocationsCache.Add(id, location);
            }

            locations = [location];

            return true;
        }

        locations = EmptyList;

        return false;
    }
}

// ResourceLocation of Material used by ResourceProvider
internal sealed class MaterialResourceLocation : ResourceLocationBase
{
    internal MaterialResourceLocation(Material material, string name, string id)
        : base(name, id, typeof(MaterialResourceProvider).FullName, typeof(Material))
    {
        Material = material;
    }

    internal Material Material { get; }
}

#endregion
