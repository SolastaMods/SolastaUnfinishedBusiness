using System;
using System.Collections.Generic;
using HarmonyLib;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Properties;
using SolastaCommunityExpansion.Utils;

namespace SolastaCommunityExpansion.Models;

internal static class CeContentPackContext
{
    internal const GamingPlatformDefinitions.ContentPack
        CeContentPack = (GamingPlatformDefinitions.ContentPack)9999;

    public const string CETag = "CommunityExpansion";

    private static readonly ContentPackDefinition ContentPackDefinition = CreateContentPackDefinition();

    private static ContentPackDefinition CreateContentPackDefinition()
    {
        var sprite = CustomIcons.CreateAssetReferenceSprite("ContentPack", Resources.ContentPack, 128);

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

    private sealed class
        ContentPackDefinitionBuilder : DefinitionBuilder<ContentPackDefinition, ContentPackDefinitionBuilder>
    {
        #region Constructors

        internal ContentPackDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        #endregion
    }
}
