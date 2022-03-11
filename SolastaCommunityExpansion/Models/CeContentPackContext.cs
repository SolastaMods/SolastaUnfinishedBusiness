using System;
using System.Collections.Generic;
using HarmonyLib;
using SolastaCommunityExpansion.Builders;
using static SolastaModApi.DatabaseHelper.ContentPackDefinitions;

namespace SolastaCommunityExpansion.Models
{
    internal static class CeContentPackContext
    {
        internal const GamingPlatformDefinitions.ContentPack CeContentPack = (GamingPlatformDefinitions.ContentPack)9999;

        internal static ContentPackDefinition ContentPackDefinition = CreateContentPackDefinition();

        private static ContentPackDefinition CreateContentPackDefinition()
        {
            return ContentPackDefinitionBuilder
                .Create("CommunityExpansionPack", DefinitionBuilder.CENamespaceGuid)
                // TODO: need our own sprite
                .SetGuiPresentation(Category.ContentPack, SupporterPack.GuiPresentation.SpriteReference)
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
