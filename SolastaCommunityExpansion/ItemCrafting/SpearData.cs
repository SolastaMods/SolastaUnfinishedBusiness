using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api;
using static SolastaCommunityExpansion.ItemCrafting.ItemCollection;

namespace SolastaCommunityExpansion.ItemCrafting;

internal static class SpearData
{
    private static ItemCollection _items;

    [NotNull]
    internal static ItemCollection Items =>
        _items ??= new ItemCollection
        {
            BaseGuid = new Guid("16757d1b-518f-4669-af43-1ddf5d23c223"),
            BaseWeapons = new List<ItemDefinition> {DatabaseHelper.ItemDefinitions.Spear},
            PossiblePrimedItemsToReplace = new List<ItemDefinition>
            {
                DatabaseHelper.ItemDefinitions.Primed_Rapier, DatabaseHelper.ItemDefinitions.Primed_Shortsword
            },
            MagicToCopy = new List<MagicItemDataHolder>
            {
                new("BlackViper", DatabaseHelper.ItemDefinitions.Enchanted_Rapier_Blackadder,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_RapierBlackAdder),
                new("Doomblade", DatabaseHelper.ItemDefinitions.Enchanted_Rapier_Doomblade,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_RapierDoomblade),
                new("Acuteness", DatabaseHelper.ItemDefinitions.Enchanted_Rapier_Of_Acuteness,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_RapierOfAcuteness),
                new("Whiteburn", DatabaseHelper.ItemDefinitions.Enchanted_Shortsword_Whiteburn,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_ShortswordWhiteburn),
                new("Lightbringer", DatabaseHelper.ItemDefinitions.Enchanted_Shortsword_Lightbringer,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_ShortswordLightbringer),
                new("Sharpness", DatabaseHelper.ItemDefinitions.Enchanted_Shortsword_of_Sharpness,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_ShortwordOfSharpness)
            }
        };
}
