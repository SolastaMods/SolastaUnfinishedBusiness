using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using static SolastaUnfinishedBusiness.Models.CraftingContext;

namespace SolastaUnfinishedBusiness.ItemCrafting;

internal static class BattleAxeData
{
    private static ItemCollection _items;

    [NotNull]
    internal static ItemCollection Items =>
        _items ??= new ItemCollection
        {
            BaseWeapons =
                new List<ItemDefinition> { DatabaseHelper.ItemDefinitions.Battleaxe },
            PossiblePrimedItemsToReplace = new List<ItemDefinition>
            {
                DatabaseHelper.ItemDefinitions.Primed_Morningstar,
                DatabaseHelper.ItemDefinitions.Primed_Mace,
                DatabaseHelper.ItemDefinitions.Primed_Greatsword,
                DatabaseHelper.ItemDefinitions.Primed_Battleaxe
            },
            MagicToCopy = new List<ItemCollection.MagicItemDataHolder>
            {
                // Same as +1
                new("Acuteness", DatabaseHelper.ItemDefinitions.Enchanted_Mace_Of_Acuteness,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_MaceOfAcuteness),
                new("Bearclaw", DatabaseHelper.ItemDefinitions.Enchanted_Morningstar_Bearclaw,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_MorningstarBearclaw),
                new("Power", DatabaseHelper.ItemDefinitions.Enchanted_Morningstar_Of_Power,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_MorningstarOfPower),
                new("Lightbringer", DatabaseHelper.ItemDefinitions.Enchanted_Greatsword_Lightbringer,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_GreatswordLightbringer),
                new("Punisher", DatabaseHelper.ItemDefinitions.Enchanted_Battleaxe_Punisher,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_BattleaxePunisher)
            }
        };
}
