using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Models.CraftingContext;

namespace SolastaUnfinishedBusiness.ItemCrafting;

internal static class KatanaData
{
    private static ItemCollection _items;

    [NotNull]
    internal static ItemCollection Items =>
        _items ??= new ItemCollection
        {
            BaseItems =
                [(CustomWeaponsContext.Katana, CustomWeaponsContext.KatanaPlus2)],
            MagicToCopy =
            [
                new ItemCollection.MagicItemDataHolder("Lightbringer",
                    ItemDefinitions.Enchanted_Greatsword_Lightbringer,
                    RecipeDefinitions.Recipe_Enchantment_GreatswordLightbringer),

                new ItemCollection.MagicItemDataHolder("Whiteburn", ItemDefinitions.Enchanted_Shortsword_Whiteburn,
                    RecipeDefinitions.Recipe_Enchantment_ShortswordWhiteburn),

                new ItemCollection.MagicItemDataHolder("Souldrinker", ItemDefinitions.Enchanted_Dagger_Souldrinker,
                    RecipeDefinitions.Recipe_Enchantment_DaggerSouldrinker),

                new ItemCollection.MagicItemDataHolder("Bearclaw", ItemDefinitions.Enchanted_Morningstar_Bearclaw,
                    RecipeDefinitions.Recipe_Enchantment_MorningstarBearclaw)
            ]
        };
}
