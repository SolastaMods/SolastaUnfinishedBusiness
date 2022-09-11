using System;
using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Utils;

namespace SolastaUnfinishedBusiness.Models;

internal static class CeContentPackContext
{
    internal const GamingPlatformDefinitions.ContentPack
        CeContentPack = (GamingPlatformDefinitions.ContentPack)9999;

    public const string CeTag = "UnfinishedBusiness";

    private static ContentPackDefinition CreateContentPackDefinition()
    {
        var sprite = CustomIcons.CreateAssetReferenceSprite("ContentPack", Resources.ContentPack, 128);

        //TODO: figure out how to fix CeContentPack being transfromed into '9999' by ToString in `FeatureDefinitionItem`
        //it uses ToString on value cast to base enum (ContentPack) and since it doesn't have value for our 9999, it just returns it as number
        return ContentPackDefinitionBuilder
            .Create(CeContentPack.ToString(), DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.ContentPack, sprite)
            .AddToDB();
    }

    public static void Load()
    {
        _ = CreateContentPackDefinition();

        GamingPlatformManager.automaticallyUnlockedContentPacks.Add(CeContentPack);
    }

    public static void AddCETag(BaseDefinition item, Dictionary<string, TagsDefinitions.Criticity> tags)
    {
        if (DiagnosticsContext.IsCeDefinition(item))
        {
            tags.TryAdd(CeTag, TagsDefinitions.Criticity.Normal);
        }
    }

    public static void AddCESpellTag(SpellDefinition spell, Dictionary<string, TagsDefinitions.Criticity> tags)
    {
        if (SpellsContext.Spells.TryGetValue(spell, out _))
        {
            tags.TryAdd(CeTag, TagsDefinitions.Criticity.Normal);
        }
        else // Not all CE spells are registered in SpellsContext
        {
            AddCETag(spell, tags);
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
