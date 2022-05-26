using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Utils;
using SolastaModApi.Infrastructure;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace SolastaCommunityExpansion.Models
{
    internal static class ResourceLocatorContext
    {
        public static void Load()
        {
            // Add our resource provider - no public API?
            Addressables.ResourceManager
                .GetField<IList<IResourceProvider>>("m_ResourceProviders")
                .TryAdd(SpriteResourceProvider.Instance);

            // Add our resource locator - there is a public API.
            Addressables.AddResourceLocator(SpriteResourceLocator.Instance);
        }
    }

    // ResourceProvider provides the resource given the resource location
    internal class SpriteResourceProvider : ResourceProviderBase
    {
        public static SpriteResourceProvider Instance { get; } = new();

        public override void Provide(ProvideHandle provideHandle)
        {
            var location = (SpriteResourceLocation)provideHandle.Location;

            Main.Log(
                $"SpriteResourceProvider.Provide: InternalId='{location.InternalId}', ProviderId='{location.ProviderId}', PrimaryKey='{location.PrimaryKey}', SpriteName='{location.Sprite.name}'.");

            provideHandle.Complete(location.Sprite, true, null);
        }

        public override bool CanProvide(Type t, IResourceLocation location)
        {
            var canProvide = base.CanProvide(t, location);

            Main.Log(
                $"SpriteResourceProvider.CanProvide: TypeName='{t.Name}', InternalId='{location.InternalId}', CanProvide={canProvide}");

            return canProvide;
        }

        public override Type GetDefaultType(IResourceLocation location)
        {
            return typeof(Sprite);
        }

        protected SpriteResourceProvider() { }
    }

    // ResourceLocator returns location of resource
    internal class SpriteResourceLocator : IResourceLocator
    {
        private static readonly Dictionary<string, SpriteResourceLocation> locationsCache = new();
        private static readonly List<IResourceLocation> emptyList = new();

        public static SpriteResourceLocator Instance { get; } = new();

        // These two properties don't seem to be used
        public string LocatorId => GetType().FullName;
        public IEnumerable<object> Keys => locationsCache.Keys;

        public bool Locate(object key, Type type, out IList<IResourceLocation> locations)
        {
            var id = key.ToString();
            var sprite = CustomIcons.GetSpriteByGuid(id);

            if (sprite != null)
            {
                Main.Log($"SpriteResourceLocator.Locate: key={key}, type={type}, sprite={sprite.name}");

                if (!locationsCache.TryGetValue(id, out var location))
                {
                    location = new SpriteResourceLocation(sprite, sprite.name, id);
                    locationsCache.Add(id, location);
                }

                locations = new List<IResourceLocation> {location};
                return true;
            }

            locations = emptyList;
            return false;
        }

        protected SpriteResourceLocator() { }
    }

    // ResourceLocation of sprite used by ResourceProvider.  We're using it to directly hold the sprite.
    internal class SpriteResourceLocation : ResourceLocationBase
    {
        private Sprite sprite;

        public Sprite Sprite
        {
            get
            {
                Main.Log($"SpriteResourceLocation.GetSprite: {sprite.name}");

                return sprite;
            }
            private set => sprite = value;
        }

        public SpriteResourceLocation(Sprite sprite, string name, string id)
            : base(name, id, typeof(SpriteResourceProvider).FullName, typeof(Sprite))
        {
            Sprite = sprite;
        }
    }
}
