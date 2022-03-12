using System;
using System.Collections.Generic;
using HarmonyLib;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Models
{
    internal static class CeContentPackContext
    {
        // Works in conjuction with "custom resources enablement patch" (patches are marked with this comment)
        internal class CEAssetReferenceSprite : AssetReferenceSprite
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
}
