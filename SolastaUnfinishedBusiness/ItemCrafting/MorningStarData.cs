using JetBrains.Annotations;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Models.CraftingContext;

namespace SolastaUnfinishedBusiness.ItemCrafting;

internal static class MorningStarData
{
    private static ItemCollection _items;

    [NotNull]
    internal static ItemCollection Items =>
        _items ??= new ItemCollection
        {
            BaseItems =
                [(ItemDefinitions.Morningstar, ItemDefinitions.Morningstar)],
            PossiblePrimedItemsToReplace = [ItemDefinitions.Primed_Morningstar],
            MagicToCopy =
            [
                new ItemCollection.MagicItemDataHolder("Acuteness", ItemDefinitions.Enchanted_Mace_Of_Acuteness,
                    RecipeDefinitions.Recipe_Enchantment_MaceOfAcuteness),

                new ItemCollection.MagicItemDataHolder("Stormblade", ItemDefinitions.Enchanted_Longsword_Stormblade,
                    RecipeDefinitions.Recipe_Enchantment_LongswordStormblade),

                new ItemCollection.MagicItemDataHolder("Frostburn", ItemDefinitions.Enchanted_Longsword_Frostburn,
                    RecipeDefinitions.Recipe_Enchantment_LongswordFrostburn),

                new ItemCollection.MagicItemDataHolder("Lightbringer",
                    ItemDefinitions.Enchanted_Greatsword_Lightbringer,
                    RecipeDefinitions.Recipe_Enchantment_GreatswordLightbringer),

                new ItemCollection.MagicItemDataHolder("Dragonblade", ItemDefinitions.Enchanted_Longsword_Dragonblade,
                    RecipeDefinitions.Recipe_Enchantment_LongswordDragonblade),

                new ItemCollection.MagicItemDataHolder("Warden", ItemDefinitions.Enchanted_Longsword_Warden,
                    RecipeDefinitions.Recipe_Enchantment_LongswordWarden),

                new ItemCollection.MagicItemDataHolder("Whiteburn", ItemDefinitions.Enchanted_Shortsword_Whiteburn,
                    RecipeDefinitions.Recipe_Enchantment_ShortswordWhiteburn),

                new ItemCollection.MagicItemDataHolder("Souldrinker", ItemDefinitions.Enchanted_Dagger_Souldrinker,
                    RecipeDefinitions.Recipe_Enchantment_DaggerSouldrinker)
            ]
        };
}
