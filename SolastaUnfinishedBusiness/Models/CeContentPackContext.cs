using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;

namespace SolastaUnfinishedBusiness.Models;

internal static class CeContentPackContext
{
    internal const GamingPlatformDefinitions.ContentPack CeContentPack = (GamingPlatformDefinitions.ContentPack)9999;

    private const string CeTag = "UnfinishedBusiness";

    internal static void Load()
    {
        var sprite = Sprites.GetSprite("ContentPack", Resources.ContentPack, 128);

        _ = ContentPackDefinitionBuilder
            .Create(CeContentPack.ToString())
            .SetGuiPresentation(Category.ContentPack, sprite)
            .AddToDB();

        GamingPlatformManager.automaticallyUnlockedContentPacks.Add(CeContentPack);
    }

    internal static void AddCeTag(BaseDefinition item, Dictionary<string, TagsDefinitions.Criticity> tags)
    {
        if (DiagnosticsContext.IsCeDefinition(item))
        {
            tags.TryAdd(CeTag, TagsDefinitions.Criticity.Normal);
        }
    }

    internal static void AddCeSpellTag(SpellDefinition spell, Dictionary<string, TagsDefinitions.Criticity> tags)
    {
        if (SpellsContext.Spells.TryGetValue(spell, out _))
        {
            tags.TryAdd(CeTag, TagsDefinitions.Criticity.Normal);
        }
        else // Not all CE spells are registered in SpellsContext
        {
            AddCeTag(spell, tags);
        }
    }
}
