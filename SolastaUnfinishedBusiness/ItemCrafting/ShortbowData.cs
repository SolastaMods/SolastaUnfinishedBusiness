using JetBrains.Annotations;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Models.CraftingContext;

namespace SolastaUnfinishedBusiness.ItemCrafting;

internal static class ShortbowData
{
    private static ItemCollection _items;

    [NotNull]
    internal static ItemCollection Items =>
        _items ??= new ItemCollection
        {
            BaseItems =
                [(ItemDefinitions.Shortbow, ItemDefinitions.ShortbowPlus2)],
            MagicToCopy =
            [
                new ItemCollection.MagicItemDataHolder("Lightbringer", ItemDefinitions.Enchanted_Longbow_Lightbringer,
                    RecipeDefinitions.Recipe_Enchantment_LongbowLightbringer),

                new ItemCollection.MagicItemDataHolder("Stormbow", ItemDefinitions.Enchanted_Longbow_Stormbow,
                    RecipeDefinitions.Recipe_Enchantment_LongsbowStormbow),

                new ItemCollection.MagicItemDataHolder("Bearclaw", ItemDefinitions.Enchanted_Morningstar_Bearclaw,
                    RecipeDefinitions.Recipe_Enchantment_MorningstarBearclaw),

                new ItemCollection.MagicItemDataHolder("Frostburn", ItemDefinitions.Enchanted_Dagger_Frostburn,
                    RecipeDefinitions.Recipe_Enchantment_DaggerFrostburn)
            ]
        };
}
