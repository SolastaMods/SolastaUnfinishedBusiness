using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Models
{
    internal static class CeContentPackContext
    {
        internal const GamingPlatformDefinitions.ContentPack CeContentPack = (GamingPlatformDefinitions.ContentPack)9999;

        internal static ContentPackDefinition ContentPackDefinition = CreateContentPackDefinition();

        private static ContentPackDefinition CreateContentPackDefinition()
        {
            var sprite = new CEAssetReferenceSprite(CustomIcons.CreateSpriteFromResource(Properties.Resources.ContentPack, 128));

            return ContentPackDefinitionBuilder
                .Create("CommunityExpansionPack", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.ContentPack, sprite)
                .AddToDB();
        }

        public static void Load()
        {
            _ = ContentPackDefinition;

            var autoUnlockedPacks = (List<GamingPlatformDefinitions.ContentPack>)
                AccessTools.Field(typeof(GamingPlatformManager), "automaticallyUnlockedContentPacks").GetValue(null);

            autoUnlockedPacks.Add(CeContentPack);
        }

        private class ContentPackDefinitionBuilder : DefinitionBuilder<ContentPackDefinition, ContentPackDefinitionBuilder>
        {
            #region Constructors
            protected ContentPackDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
            {
            }
            #endregion 
        }
    }

    // Works in conjuction with 2 patches below
    class CEAssetReferenceSprite : AssetReferenceSprite
    {
        public CEAssetReferenceSprite(Sprite sprite) : base(string.Empty)
        {
            Sprite = sprite;
        }

        public Sprite Sprite { get; }
        public override UnityEngine.Object Asset => Sprite;
        public override bool RuntimeKeyIsValid()
        {
            return true;
        }
    }

    [HarmonyPatch]
    internal static class Gui_LoadAssetAsync
    {
        internal static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(Gui), "LoadAssetSync", new Type[] { typeof(AssetReference) }, new Type[] { typeof(Sprite) });
        }

        internal static bool Prefix(AssetReference asset)
        {
            // If it's a CEAssetReferenceSprite prevent async load
            return asset is not CEAssetReferenceSprite;
        }

        internal static void Postfix(AssetReference asset, ref Sprite __result)
        {
            if(asset is CEAssetReferenceSprite reference)
            {
                Main.Log($"Providing sprite {reference.Sprite.name}");

                // Return our sprite
                __result = reference.Sprite;
            }
        }
    }

    [HarmonyPatch(typeof(Gui), "ReleaseAddressableAsset")]
    internal static class Gui_ReleaseAddressableAsset
    {
        internal static bool Prefix(UnityEngine.Object asset)
        {
            // If it's a CE provided sprite stop it being unloaded
            bool retval = !CustomIcons.IsCachedSprite(asset as Sprite);

            if (!retval)
            {
                Main.Log($"Not releasing {asset.name}");
            }

            return retval;
        }
    }
}
