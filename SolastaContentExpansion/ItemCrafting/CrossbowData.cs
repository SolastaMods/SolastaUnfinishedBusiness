using SolastaModApi;
using System;
using System.Collections.Generic;
using static SolastaContentExpansion.ItemCrafting.ItemCollection;

namespace SolastaContentExpansion.ItemCrafting
{
    class CrossbowData
    {
        private static ItemCollection crossbowItems;

        internal static ItemCollection CrossbowItems
        {
            get
            {
                if (crossbowItems == null)
                {
                    crossbowItems = new ItemCollection()
                    {
                        BaseGuid = new Guid("6eff8e23-1b2f-4e48-8cde-3abda9d4bc3b"),
                        BaseWeapons = new List<ItemDefinition>()
            {
                DatabaseHelper.ItemDefinitions.LightCrossbow,
                DatabaseHelper.ItemDefinitions.HeavyCrossbow,
            },
                        PossiblePrimedItemsToReplace = new List<ItemDefinition>()
            {
                DatabaseHelper.ItemDefinitions.Primed_Longbow,
                DatabaseHelper.ItemDefinitions.Primed_Shortbow,
            },
                        MagicToCopy = new List<MagicItemDataHolder>()
            {
                // Same as +1
                new MagicItemDataHolder("Accuracy", DatabaseHelper.ItemDefinitions.Enchanted_Longbow_Of_Accurary,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_LongbowOfAcurracy),
                // Same as +2
                new MagicItemDataHolder("Sharpshooting", DatabaseHelper.ItemDefinitions.Enchanted_Shortbow_Of_Sharpshooting,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_ShortbowOfSharpshooting),
                new MagicItemDataHolder("Lightbringer", DatabaseHelper.ItemDefinitions.Enchanted_Longbow_Lightbringer,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_LongbowLightbringer),
                new MagicItemDataHolder("Stormbow", DatabaseHelper.ItemDefinitions.Enchanted_Longbow_Stormbow,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_LongsbowStormbow),
                new MagicItemDataHolder("Medusa", DatabaseHelper.ItemDefinitions.Enchanted_Shortbow_Medusa,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_ShortbowMedusa),
            }
                    };
                }
                return crossbowItems;
            }
            set => crossbowItems = value;
        }
    }
}
