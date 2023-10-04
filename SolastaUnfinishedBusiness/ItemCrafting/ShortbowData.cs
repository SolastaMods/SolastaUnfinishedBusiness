using System.Collections.Generic;
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
                new List<(ItemDefinition item, ItemDefinition presentation)>
                {
                    (ItemDefinitions.Shortbow, ItemDefinitions.ShortbowPlus2)
                },
            PossiblePrimedItemsToReplace = new List<ItemDefinition> { ItemDefinitions.Primed_Shortbow },
            MagicToCopy = new List<ItemCollection.MagicItemDataHolder>
            {
                new("Lightbringer", ItemDefinitions.Enchanted_Longbow_Lightbringer,
                    RecipeDefinitions.Recipe_Enchantment_LongbowLightbringer),
                new("Stormbow", ItemDefinitions.Enchanted_Longbow_Stormbow,
                    RecipeDefinitions.Recipe_Enchantment_LongsbowStormbow),
                new("Bearclaw", ItemDefinitions.Enchanted_Morningstar_Bearclaw,
                    RecipeDefinitions.Recipe_Enchantment_MorningstarBearclaw),
                new("Frostburn", ItemDefinitions.Enchanted_Dagger_Frostburn,
                    RecipeDefinitions.Recipe_Enchantment_DaggerFrostburn),
            }
        };
}
