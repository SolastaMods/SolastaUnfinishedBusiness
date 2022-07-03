using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api;
using static SolastaCommunityExpansion.ItemCrafting.ItemCollection;

namespace SolastaCommunityExpansion.ItemCrafting;

internal static class CrossbowData
{
    private static ItemCollection _crossbowItems;

    [NotNull]
    internal static ItemCollection CrossbowItems =>
        _crossbowItems ??= new ItemCollection
        {
            BaseGuid = new Guid("6eff8e23-1b2f-4e48-8cde-3abda9d4bc3b"),
            BaseWeapons =
                new List<ItemDefinition>
                {
                    DatabaseHelper.ItemDefinitions.LightCrossbow, DatabaseHelper.ItemDefinitions.HeavyCrossbow
                },
            PossiblePrimedItemsToReplace = new List<ItemDefinition>
            {
                DatabaseHelper.ItemDefinitions.Primed_Longbow, DatabaseHelper.ItemDefinitions.Primed_Shortbow
            },
            MagicToCopy = new List<MagicItemDataHolder>
            {
                // Same as +1
                new("Accuracy", DatabaseHelper.ItemDefinitions.Enchanted_Longbow_Of_Accurary,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_LongbowOfAcurracy),
                // Same as +2
                new("Sharpshooting", DatabaseHelper.ItemDefinitions.Enchanted_Shortbow_Of_Sharpshooting,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_ShortbowOfSharpshooting),
                new("Lightbringer", DatabaseHelper.ItemDefinitions.Enchanted_Longbow_Lightbringer,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_LongbowLightbringer),
                new("Stormbow", DatabaseHelper.ItemDefinitions.Enchanted_Longbow_Stormbow,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_LongsbowStormbow),
                new("Medusa", DatabaseHelper.ItemDefinitions.Enchanted_Shortbow_Medusa,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_ShortbowMedusa)
            }
        };
}
