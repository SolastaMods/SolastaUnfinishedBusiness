using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Properties;
using SolastaCommunityExpansion.Utils;

namespace SolastaCommunityExpansion.Models;

internal static class CeContentPackContext
{
    internal const GamingPlatformDefinitions.ContentPack
        CeContentPack = (GamingPlatformDefinitions.ContentPack)9999;

    public const string CeTag = "CommunityExpansion";

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
        _ = CreateContentPackDefinition();

        GamingPlatformManager.automaticallyUnlockedContentPacks.Add(CeContentPack);
    }
    
    public static void AddCETag(BaseDefinition item, Dictionary<string,TagsDefinitions.Criticity> tags)
    {
        if (DiagnosticsContext.IsCeDefinition(item))
        {
            tags.TryAdd(CeTag, TagsDefinitions.Criticity.Normal);
        }
    }
    
    public static void AddCESpellTag(SpellDefinition spell, Dictionary<string,TagsDefinitions.Criticity> tags)
    {
        if (SpellsContext.Spells.TryGetValue(spell, out _))
        {
            tags.TryAdd(CeTag, TagsDefinitions.Criticity.Normal);
        }
        else // Not all CE spells are registered in SpellsContext
        {
            AddCESpellTag(spell, tags);
        }
    }

    private sealed class
        // ReSharper disable once ClassNeverInstantiated.Local
        ContentPackDefinitionBuilder : DefinitionBuilder<ContentPackDefinition, ContentPackDefinitionBuilder>
    {
        #region Constructors

        internal ContentPackDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        #endregion
    }
}
