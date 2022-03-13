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
            Addressables.ResourceManager.GetField<IList<IResourceProvider>>("m_ResourceProviders").TryAdd(new CeSpriteResourceProvider());

            // Add our resource locator - there is a public API.
            Addressables.AddResourceLocator(new CeSpriteResourceLocator());

            // Test 
            //var sprite = Gui.LoadAssetSync<Sprite>(new AssetReferenceSprite("ce-test"));

            //Main.Log($"Loaded sprite: {sprite.name ?? "NULL"}");
        }
    }

    // Provider provides resource given location - TODO: support other resource types if required
    class CeSpriteResourceProvider : ResourceProviderBase
    {
        public override void Provide(ProvideHandle provideHandle)
        {
            var location = (CeSpriteResourceLocation)provideHandle.Location;

            Main.Log($"CeSpriteResourceProvider.Provide: {location.InternalId}, {location.ProviderId}, {location.PrimaryKey}, {location.Sprite.name}");

            provideHandle.Complete(location.Sprite, true, null);
        }

        public override bool CanProvide(Type t, IResourceLocation location)
        {
            var canProvide = base.CanProvide(t, location);
            Main.Log($"CeSpriteResourceProvider.CanProvide: {t.Name}, {location.InternalId}, {canProvide}");
            return canProvide;
        }

        public override Type GetDefaultType(IResourceLocation location)
        {
            return typeof(Sprite);
        }
    }

    // Locator returns location of resource - TODO: support other resource types if required
    class CeSpriteResourceLocator : IResourceLocator
    {
        private static List<IResourceLocation> locations = new()
        {
            new CeSpriteResourceLocation(CustomIcons.GetOrCreateSprite("ContentPack", Properties.Resources.ContentPack, 128), "ce-test", "ce-test")
        };

        public string LocatorId
        {
            get
            {
                Main.Log($"CeSpriteResourceLocator - get LocatorId");
                return "CE sprite locator";
            }
        }

        public IEnumerable<object> Keys => new List<string> { "ce-test" };

        public bool Locate(object key, Type type, out IList<IResourceLocation> locations)
        {
            if ((string)key == "ce-test")
            {
                Main.Log($"CeSpriteResourceLocator.Location: {key}, {type}");
                locations = CeSpriteResourceLocator.locations;
                return true;
            }
            else
            {
                locations = new List<IResourceLocation>();
                return false;
            }
        }
    }

    // Location of sprite used by ResourceProvider.  We're using it to directly hold the sprite.
    class CeSpriteResourceLocation : ResourceLocationBase
    {
        private Sprite sprite;

        public Sprite Sprite
        {
            get
            {
                Main.Log($"CeSpriteResourceLocation.GetSprite: {sprite.name}");

                return sprite;
            }
            private set
            {
                sprite = value;
            }
        }

        public CeSpriteResourceLocation(Sprite sprite, string name, string id) : base(name, id, typeof(CeSpriteResourceProvider).FullName, typeof(Sprite))
        {
            Sprite = sprite;
        }
    }
}
