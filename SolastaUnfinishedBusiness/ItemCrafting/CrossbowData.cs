using JetBrains.Annotations;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Models.CraftingContext;

namespace SolastaUnfinishedBusiness.ItemCrafting;

internal static class CrossbowData
{
    private static ItemCollection _items;

    [NotNull]
    internal static ItemCollection Items =>
        _items ??= new ItemCollection
        {
            BaseItems =
            [
                (ItemDefinitions.LightCrossbow, ItemDefinitions.LightCrossbowPlus2),
                (ItemDefinitions.HeavyCrossbow, ItemDefinitions.HeavyCrossbowPlus2)
            ],
            PossiblePrimedItemsToReplace =
                [ItemDefinitions.Primed_Longbow, ItemDefinitions.Primed_Shortbow],
            MagicToCopy =
            [
                new ItemCollection.MagicItemDataHolder("Accuracy", ItemDefinitions.Enchanted_Longbow_Of_Accurary,
                    RecipeDefinitions.Recipe_Enchantment_LongbowOfAcurracy),
                // Same as +2

                new ItemCollection.MagicItemDataHolder("Sharpshooting",
                    ItemDefinitions.Enchanted_Shortbow_Of_Sharpshooting,
                    RecipeDefinitions.Recipe_Enchantment_ShortbowOfSharpshooting),

                new ItemCollection.MagicItemDataHolder("Lightbringer", ItemDefinitions.Enchanted_Longbow_Lightbringer,
                    RecipeDefinitions.Recipe_Enchantment_LongbowLightbringer),

                new ItemCollection.MagicItemDataHolder("Stormbow", ItemDefinitions.Enchanted_Longbow_Stormbow,
                    RecipeDefinitions.Recipe_Enchantment_LongsbowStormbow),

                new ItemCollection.MagicItemDataHolder("Medusa", ItemDefinitions.Enchanted_Shortbow_Medusa,
                    RecipeDefinitions.Recipe_Enchantment_ShortbowMedusa),

                new ItemCollection.MagicItemDataHolder("Bearclaw", ItemDefinitions.Enchanted_Morningstar_Bearclaw,
                    RecipeDefinitions.Recipe_Enchantment_MorningstarBearclaw),

                new ItemCollection.MagicItemDataHolder("Frostburn", ItemDefinitions.Enchanted_Dagger_Frostburn,
                    RecipeDefinitions.Recipe_Enchantment_DaggerFrostburn),

                new ItemCollection.MagicItemDataHolder("Whiteburn", ItemDefinitions.Enchanted_Shortsword_Whiteburn,
                    RecipeDefinitions.Recipe_Enchantment_ShortswordWhiteburn)
            ]
        };
}
