using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using static SolastaUnfinishedBusiness.Models.CraftingContext;

namespace SolastaUnfinishedBusiness.ItemCrafting;

internal static class GreatSwordData
{
    private static ItemCollection _items;

    [NotNull]
    internal static ItemCollection Items =>
        _items ??= new ItemCollection
        {
            BaseWeapons =
                new List<ItemDefinition> { DatabaseHelper.ItemDefinitions.Greatsword },
            PossiblePrimedItemsToReplace = new List<ItemDefinition> { DatabaseHelper.ItemDefinitions.Primed_Battleaxe },
            MagicToCopy = new List<ItemCollection.MagicItemDataHolder>
            {
                new("Punisher", DatabaseHelper.ItemDefinitions.Enchanted_Battleaxe_Punisher,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_BattleaxePunisher)
            }
        };
}
