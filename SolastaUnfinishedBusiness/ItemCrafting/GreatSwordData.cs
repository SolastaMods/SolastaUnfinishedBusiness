using System.Collections.Generic;
using JetBrains.Annotations;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Models.CraftingContext;

namespace SolastaUnfinishedBusiness.ItemCrafting;

internal static class GreatSwordData
{
    private static ItemCollection _items;

    [NotNull]
    internal static ItemCollection Items =>
        _items ??= new ItemCollection
        {
            BaseItems =
                new List<(ItemDefinition item, ItemDefinition presentation)>
                {
                    (ItemDefinitions.Greatsword, ItemDefinitions.GreataxePlus2)
                },
            PossiblePrimedItemsToReplace = new List<ItemDefinition> { ItemDefinitions.Primed_Battleaxe },
            MagicToCopy = new List<ItemCollection.MagicItemDataHolder>
            {
                new("Punisher", ItemDefinitions.Enchanted_Battleaxe_Punisher,
                    RecipeDefinitions.Recipe_Enchantment_BattleaxePunisher)
            }
        };
}
