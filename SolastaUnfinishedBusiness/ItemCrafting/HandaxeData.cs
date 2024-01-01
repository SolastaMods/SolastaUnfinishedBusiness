using JetBrains.Annotations;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Models.CraftingContext;

namespace SolastaUnfinishedBusiness.ItemCrafting;

internal static class HandaxeData
{
    private static ItemCollection _items;

    [NotNull]
    internal static ItemCollection Items =>
        _items ??= new ItemCollection
        {
            BaseItems =
                [(ItemDefinitions.Handaxe, ItemDefinitions.HandaxePlus2)],
            PossiblePrimedItemsToReplace = [ItemDefinitions.Primed_Dagger],
            MagicToCopy =
            [
                new ItemCollection.MagicItemDataHolder("Acuteness", ItemDefinitions.Enchanted_Dagger_of_Acuteness,
                    RecipeDefinitions.Recipe_Enchantment_DaggerOfAcuteness),
                // Same as +2

                new ItemCollection.MagicItemDataHolder("Sharpness", ItemDefinitions.Enchanted_Dagger_of_Sharpness,
                    RecipeDefinitions.Recipe_Enchantment_DaggerOfSharpness),

                new ItemCollection.MagicItemDataHolder("Souldrinker", ItemDefinitions.Enchanted_Dagger_Souldrinker,
                    RecipeDefinitions.Recipe_Enchantment_DaggerSouldrinker),

                new ItemCollection.MagicItemDataHolder("Frostburn", ItemDefinitions.Enchanted_Dagger_Frostburn,
                    RecipeDefinitions.Recipe_Enchantment_DaggerFrostburn),

                new ItemCollection.MagicItemDataHolder("Bearclaw", ItemDefinitions.Enchanted_Morningstar_Bearclaw,
                    RecipeDefinitions.Recipe_Enchantment_MorningstarBearclaw),

                new ItemCollection.MagicItemDataHolder("Whiteburn", ItemDefinitions.Enchanted_Shortsword_Whiteburn,
                    RecipeDefinitions.Recipe_Enchantment_ShortswordWhiteburn)
            ]
        };
}
