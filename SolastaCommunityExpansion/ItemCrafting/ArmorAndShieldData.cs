using SolastaModApi;
using System;
using System.Collections.Generic;
using static SolastaCommunityExpansion.ItemCrafting.ItemCollection;

namespace SolastaCommunityExpansion.ItemCrafting
{
    internal static class ArmorAndShieldData
    {
        private static ItemCollection items;

        internal static ItemCollection Items
        {
            get
            {
                return items ?? (items = new ItemCollection()
                    {
                        BaseGuid = new Guid("16757d1b-518f-4669-af43-1ddf5d23c223"),
                        BaseWeapons = new List<ItemDefinition>()
                        {
                            DatabaseHelper.ItemDefinitions.Shield_Wooden,
                            DatabaseHelper.ItemDefinitions.Shield,
                            DatabaseHelper.ItemDefinitions.HideArmor,
                            DatabaseHelper.ItemDefinitions.StuddedLeather,
                        },
                        PossiblePrimedItemsToReplace = new List<ItemDefinition>()
                        {
                            DatabaseHelper.ItemDefinitions.Primed_HalfPlate,
                            DatabaseHelper.ItemDefinitions.Primed_Leather_Armor,
                            DatabaseHelper.ItemDefinitions.Primed_ScaleMail,
                            DatabaseHelper.ItemDefinitions.Primed_Breastplate,
                        },
                        MagicToCopy = new List<MagicItemDataHolder>()
                        {
                            new MagicItemDataHolder("Sturdiness", DatabaseHelper.ItemDefinitions.Enchanted_HalfPlateOfSturdiness,
                                DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_HalfplateOfSturdiness),
                            new MagicItemDataHolder("Robustness", DatabaseHelper.ItemDefinitions.Enchanted_HalfPlateOfRobustness,
                                DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_HalfplateOfRobustness),
                            new MagicItemDataHolder("Survival", DatabaseHelper.ItemDefinitions.Enchanted_LeatherArmorOfSurvival,
                                DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_LeatherArmorOfSurvival),
                            new MagicItemDataHolder("FlameDancing", DatabaseHelper.ItemDefinitions.Enchanted_LeatherArmorOfFlameDancing,
                                DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_LeatherArmorOfFlameDancing),
                            new MagicItemDataHolder("FrostWalking", DatabaseHelper.ItemDefinitions.Enchanted_ScaleMailOfIceDancing,
                                DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_ScaleMailOfIceDancing),
                            new MagicItemDataHolder("Deflection", DatabaseHelper.ItemDefinitions.Enchanted_BreastplateOfDeflection,
                                DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_BreastplateOfDeflection),
                        }
                    });
            }
            set => items = value;
        }
    }
}
