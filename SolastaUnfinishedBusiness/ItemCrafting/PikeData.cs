using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Models.CraftingContext;

namespace SolastaUnfinishedBusiness.ItemCrafting;

internal static class PikeData
{
    private static ItemCollection _items;

    [NotNull]
    internal static ItemCollection Items =>
        _items ??= new ItemCollection
        {
            BaseItems =
                new List<(ItemDefinition item, ItemDefinition presentation)>
                {
                    (CustomWeaponsContext.Pike, CustomWeaponsContext.PikePlus2)
                },
            CustomSubFeatures = new List<object> { new CustomScale(z: 3.5f) },
            PossiblePrimedItemsToReplace = new List<ItemDefinition> { CustomWeaponsContext.PikePrimed },
            MagicToCopy = new List<ItemCollection.MagicItemDataHolder>
            {
                // Same as +1
                new("Acuteness", ItemDefinitions.Enchanted_Mace_Of_Acuteness,
                    RecipeDefinitions.Recipe_Enchantment_MaceOfAcuteness),
                new("Stormblade", ItemDefinitions.Enchanted_Longsword_Stormblade,
                    RecipeDefinitions.Recipe_Enchantment_LongswordStormblade),
                new("Frostburn", ItemDefinitions.Enchanted_Longsword_Frostburn,
                    RecipeDefinitions.Recipe_Enchantment_LongswordFrostburn),
                new("Lightbringer", ItemDefinitions.Enchanted_Greatsword_Lightbringer,
                    RecipeDefinitions.Recipe_Enchantment_GreatswordLightbringer),
                new("Dragonblade", ItemDefinitions.Enchanted_Longsword_Dragonblade,
                    RecipeDefinitions.Recipe_Enchantment_LongswordDragonblade),
                new("Warden", ItemDefinitions.Enchanted_Longsword_Warden,
                    RecipeDefinitions.Recipe_Enchantment_LongswordWarden),
                new("Whiteburn", ItemDefinitions.Enchanted_Shortsword_Whiteburn,
                    RecipeDefinitions.Recipe_Enchantment_ShortswordWhiteburn),
                new("Souldrinker", ItemDefinitions.Enchanted_Dagger_Souldrinker,
                    RecipeDefinitions.Recipe_Enchantment_DaggerSouldrinker),
                new("Bearclaw", ItemDefinitions.Enchanted_Morningstar_Bearclaw,
                    RecipeDefinitions.Recipe_Enchantment_MorningstarBearclaw)
            }
        };
}
