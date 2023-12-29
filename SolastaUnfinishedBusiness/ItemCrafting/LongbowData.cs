using JetBrains.Annotations;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Models.CraftingContext;

namespace SolastaUnfinishedBusiness.ItemCrafting;

internal static class LongbowData
{
    private static ItemCollection _items;

    [NotNull]
    internal static ItemCollection Items =>
        _items ??= new ItemCollection
        {
            BaseItems =
                [(ItemDefinitions.Longbow, ItemDefinitions.LongbowPlus2)],
            PossiblePrimedItemsToReplace = [ItemDefinitions.Primed_Longbow],
            MagicToCopy =
            [
                new ItemCollection.MagicItemDataHolder("Medusa", ItemDefinitions.Enchanted_Shortbow_Medusa,
                    RecipeDefinitions.Recipe_Enchantment_ShortbowMedusa),

                new ItemCollection.MagicItemDataHolder("Souldrinker", ItemDefinitions.Enchanted_Dagger_Souldrinker,
                    RecipeDefinitions.Recipe_Enchantment_DaggerSouldrinker),

                new ItemCollection.MagicItemDataHolder("Bearclaw", ItemDefinitions.Enchanted_Morningstar_Bearclaw,
                    RecipeDefinitions.Recipe_Enchantment_MorningstarBearclaw),

                new ItemCollection.MagicItemDataHolder("Frostburn", ItemDefinitions.Enchanted_Dagger_Frostburn,
                    RecipeDefinitions.Recipe_Enchantment_DaggerFrostburn),

                new ItemCollection.MagicItemDataHolder("Whiteburn", ItemDefinitions.Enchanted_Shortsword_Whiteburn,
                    RecipeDefinitions.Recipe_Enchantment_ShortswordWhiteburn)
            ]
        };
}
