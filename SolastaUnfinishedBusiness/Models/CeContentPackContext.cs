using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;

namespace SolastaUnfinishedBusiness.Models;

internal static class CeContentPackContext
{
    internal const GamingPlatformDefinitions.ContentPack CeContentPack = (GamingPlatformDefinitions.ContentPack)9999;

    internal const string CeTag = "UnfinishedBusiness";

    internal static void Load()
    {
        var sprite = Sprites.GetSprite("ContentPack", Resources.ContentPack, 64);

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
}
