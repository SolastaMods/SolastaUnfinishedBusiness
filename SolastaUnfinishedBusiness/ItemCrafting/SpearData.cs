using JetBrains.Annotations;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Models.CraftingContext;

namespace SolastaUnfinishedBusiness.ItemCrafting;

internal static class SpearData
{
    private static ItemCollection _items;

    [NotNull]
    internal static ItemCollection Items =>
        _items ??= new ItemCollection
        {
            BaseItems = [(ItemDefinitions.Spear, ItemDefinitions.SpearPlus2)],
            MagicToCopy =
            [
                new ItemCollection.MagicItemDataHolder("BlackViper", ItemDefinitions.Enchanted_Rapier_Blackadder,
                    RecipeDefinitions.Recipe_Enchantment_RapierBlackAdder),

                new ItemCollection.MagicItemDataHolder("Doomblade", ItemDefinitions.Enchanted_Rapier_Doomblade,
                    RecipeDefinitions.Recipe_Enchantment_RapierDoomblade),

                new ItemCollection.MagicItemDataHolder("Acuteness", ItemDefinitions.Enchanted_Rapier_Of_Acuteness,
                    RecipeDefinitions.Recipe_Enchantment_RapierOfAcuteness),

                new ItemCollection.MagicItemDataHolder("Whiteburn", ItemDefinitions.Enchanted_Shortsword_Whiteburn,
                    RecipeDefinitions.Recipe_Enchantment_ShortswordWhiteburn),

                new ItemCollection.MagicItemDataHolder("Lightbringer",
                    ItemDefinitions.Enchanted_Shortsword_Lightbringer,
                    RecipeDefinitions.Recipe_Enchantment_ShortswordLightbringer),

                new ItemCollection.MagicItemDataHolder("Sharpness", ItemDefinitions.Enchanted_Shortsword_of_Sharpness,
                    RecipeDefinitions.Recipe_Enchantment_ShortwordOfSharpness),

                new ItemCollection.MagicItemDataHolder("Souldrinker", ItemDefinitions.Enchanted_Dagger_Souldrinker,
                    RecipeDefinitions.Recipe_Enchantment_DaggerSouldrinker),

                new ItemCollection.MagicItemDataHolder("Bearclaw", ItemDefinitions.Enchanted_Morningstar_Bearclaw,
                    RecipeDefinitions.Recipe_Enchantment_MorningstarBearclaw)
            ]
        };
}
