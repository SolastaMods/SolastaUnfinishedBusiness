using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Models;
using static SolastaUnfinishedBusiness.Models.CraftingContext;

namespace SolastaUnfinishedBusiness.ItemCrafting;

internal static class LongMaceData
{
    private static ItemCollection _items;

    [NotNull]
    internal static ItemCollection Items =>
        _items ??= new ItemCollection
        {
            BaseWeapons = new List<ItemDefinition> { CustomWeaponsContext.LongMace },
            CustomSubFeatures = new List<object> { new CustomScale(z: 3.5f) },
            PossiblePrimedItemsToReplace = new List<ItemDefinition> { CustomWeaponsContext.LongMacePrimed },
            MagicToCopy = new List<ItemCollection.MagicItemDataHolder>
            {
                // Same as +1
                new("Acuteness", DatabaseHelper.ItemDefinitions.Enchanted_Mace_Of_Acuteness,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_MaceOfAcuteness),
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
