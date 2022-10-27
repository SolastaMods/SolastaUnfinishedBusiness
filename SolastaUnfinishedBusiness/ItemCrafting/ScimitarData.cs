using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using static SolastaUnfinishedBusiness.Models.CraftingContext;

namespace SolastaUnfinishedBusiness.ItemCrafting;

internal static class ScimitarData
{
    private static ItemCollection _items;

    [NotNull]
    internal static ItemCollection Items =>
        _items ??= new ItemCollection
        {
            BaseWeapons = new List<ItemDefinition> { DatabaseHelper.ItemDefinitions.Scimitar },
            PossiblePrimedItemsToReplace = new List<ItemDefinition>
            {
                DatabaseHelper.ItemDefinitions.Primed_Longsword,
                DatabaseHelper.ItemDefinitions.Primed_Greatsword,
                DatabaseHelper.ItemDefinitions.Primed_Shortsword,
                DatabaseHelper.ItemDefinitions.Primed_Dagger
            },
            MagicToCopy = new List<ItemCollection.MagicItemDataHolder>
            {
                new("Stormblade", DatabaseHelper.ItemDefinitions.Enchanted_Longsword_Stormblade,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_LongswordStormblade),
                new("Frostburn", DatabaseHelper.ItemDefinitions.Enchanted_Longsword_Frostburn,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_LongswordFrostburn),
                new("Lightbringer", DatabaseHelper.ItemDefinitions.Enchanted_Greatsword_Lightbringer,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_GreatswordLightbringer),
                new("Dragonblade", DatabaseHelper.ItemDefinitions.Enchanted_Longsword_Dragonblade,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_LongswordDragonblade),
                new("Warden", DatabaseHelper.ItemDefinitions.Enchanted_Longsword_Warden,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_LongswordWarden),
                new("Whiteburn", DatabaseHelper.ItemDefinitions.Enchanted_Shortsword_Whiteburn,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_ShortswordWhiteburn),
                new("Souldrinker", DatabaseHelper.ItemDefinitions.Enchanted_Dagger_Souldrinker,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_DaggerSouldrinker)
            }
        };
}
