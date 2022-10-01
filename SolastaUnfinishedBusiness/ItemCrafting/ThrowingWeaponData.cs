using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using static SolastaUnfinishedBusiness.Models.CraftingContext;

namespace SolastaUnfinishedBusiness.ItemCrafting;

internal static class ThrowingWeaponData
{
    private static ItemCollection _items;

    [NotNull]
    internal static ItemCollection Items =>
        _items ??= new ItemCollection
        {
            BaseWeapons =
                new List<ItemDefinition>
                {
                    DatabaseHelper.ItemDefinitions.Javelin, DatabaseHelper.ItemDefinitions.Dart
                },
            PossiblePrimedItemsToReplace = new List<ItemDefinition> { DatabaseHelper.ItemDefinitions.Primed_Dagger },
            MagicToCopy = new List<ItemCollection.MagicItemDataHolder>
            {
                // Same as +1
                new("Acuteness", DatabaseHelper.ItemDefinitions.Enchanted_Dagger_of_Acuteness,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_DaggerOfAcuteness),
                // Same as +2
                new("Sharpness", DatabaseHelper.ItemDefinitions.Enchanted_Dagger_of_Sharpness,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_DaggerOfSharpness),
                new("Souldrinker", DatabaseHelper.ItemDefinitions.Enchanted_Dagger_Souldrinker,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_DaggerSouldrinker),
                new("Frostburn", DatabaseHelper.ItemDefinitions.Enchanted_Dagger_Frostburn,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_DaggerFrostburn)
            },
            NumProduced = 3
        };
}
