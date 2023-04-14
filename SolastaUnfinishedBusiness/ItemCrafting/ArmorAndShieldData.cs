using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using static SolastaUnfinishedBusiness.Models.CraftingContext;

namespace SolastaUnfinishedBusiness.ItemCrafting;

internal static class ArmorAndShieldData
{
    private static ItemCollection _items;

    [NotNull]
    internal static ItemCollection Items =>
        _items ??= new ItemCollection
        {
            BaseWeapons =
                new List<ItemDefinition>
                {
                    DatabaseHelper.ItemDefinitions.Shield_Wooden,
                    DatabaseHelper.ItemDefinitions.Shield,
                    DatabaseHelper.ItemDefinitions.HideArmor,
                    DatabaseHelper.ItemDefinitions.LeatherDruid,
                    DatabaseHelper.ItemDefinitions.StuddedLeather,
                    DatabaseHelper.ItemDefinitions.ChainShirt,
                    DatabaseHelper.ItemDefinitions.PaddedLeather,
                    DatabaseHelper.ItemDefinitions.Leather,
                    DatabaseHelper.ItemDefinitions.ScaleMail,
                    DatabaseHelper.ItemDefinitions.Breastplate,
                    DatabaseHelper.ItemDefinitions.HalfPlate,
                    DatabaseHelper.ItemDefinitions.Ringmail,
                    DatabaseHelper.ItemDefinitions.ChainMail,
                    DatabaseHelper.ItemDefinitions.SplintArmor,
                    DatabaseHelper.ItemDefinitions.Plate,
                    DatabaseHelper.ItemDefinitions.SorcererArmor,
                    DatabaseHelper.ItemDefinitions.BarbarianClothes,
                    DatabaseHelper.ItemDefinitions.Warlock_Armor
                },
            PossiblePrimedItemsToReplace = new List<ItemDefinition>
            {
                DatabaseHelper.ItemDefinitions.Primed_HalfPlate,
                DatabaseHelper.ItemDefinitions.Primed_Leather_Armor,
                DatabaseHelper.ItemDefinitions.Primed_LeatherDruid,
                DatabaseHelper.ItemDefinitions.Primed_ScaleMail,
                DatabaseHelper.ItemDefinitions.Primed_Breastplate,
                DatabaseHelper.ItemDefinitions.Primed_ChainMail,
                DatabaseHelper.ItemDefinitions.Primed_ChainShirt
            },
            MagicToCopy = new List<ItemCollection.MagicItemDataHolder>
            {
                new("Sturdiness", DatabaseHelper.ItemDefinitions.Enchanted_HalfPlateOfSturdiness,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_HalfplateOfSturdiness),
                new("Robustness", DatabaseHelper.ItemDefinitions.Enchanted_HalfPlateOfRobustness,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_HalfplateOfRobustness),
                new(SkillDefinitions.Survival, DatabaseHelper.ItemDefinitions.Enchanted_LeatherArmorOfSurvival,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_LeatherArmorOfSurvival),
                new("FlameDancing", DatabaseHelper.ItemDefinitions.Enchanted_LeatherArmorOfFlameDancing,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_LeatherArmorOfFlameDancing),
                new("FrostWalking", DatabaseHelper.ItemDefinitions.Enchanted_ScaleMailOfIceDancing,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_ScaleMailOfIceDancing),
                new("Deflection", DatabaseHelper.ItemDefinitions.Enchanted_BreastplateOfDeflection,
                    DatabaseHelper.RecipeDefinitions.Recipe_Enchantment_BreastplateOfDeflection)
            }
        };
}
