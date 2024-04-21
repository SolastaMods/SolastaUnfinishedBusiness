using JetBrains.Annotations;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Models.CraftingContext;

namespace SolastaUnfinishedBusiness.ItemCrafting;

internal static class ArmorAndShieldData
{
    private static ItemCollection _items;

    [NotNull]
    internal static ItemCollection Items =>
        _items ??= new ItemCollection
        {
            BaseItems =
            [
                (ItemDefinitions.Shield_Wooden, null),
                (ItemDefinitions.Shield, ItemDefinitions.ShieldPlus2),
                (ItemDefinitions.HideArmor, ItemDefinitions.HideArmor_plus_two),
                (ItemDefinitions.LeatherDruid, ItemDefinitions.Enchanted_Druid_Armor_Of_The_Forest),
                (ItemDefinitions.StuddedLeather, ItemDefinitions.StuddedLeather_plus_two),
                (ItemDefinitions.ChainShirt, ItemDefinitions.ChainShirtPlus2),
                (ItemDefinitions.PaddedLeather, null),
                (ItemDefinitions.Leather, ItemDefinitions.LeatherArmorPlus2),
                (ItemDefinitions.ScaleMail, ItemDefinitions.ScaleMailPlus2),
                (ItemDefinitions.Breastplate, ItemDefinitions.BreastplatePlus2),
                (ItemDefinitions.HalfPlate, ItemDefinitions.HalfPlatePlus2),
                (ItemDefinitions.ChainMail, ItemDefinitions.ChainmailPlus2),
                (ItemDefinitions.Plate, ItemDefinitions.PlatePlus2),
                (ItemDefinitions.SorcererArmor, null),
                (ItemDefinitions.BarbarianClothes, null),
                (ItemDefinitions.Warlock_Armor, null),
                (ItemDefinitions.MonkArmor, null),
                (ItemDefinitions.ClothesWizard, null)
            ],
            MagicToCopy =
            [
                new ItemCollection.MagicItemDataHolder("Sturdiness", ItemDefinitions.Enchanted_HalfPlateOfSturdiness,
                    RecipeDefinitions.Recipe_Enchantment_HalfplateOfSturdiness),

                new ItemCollection.MagicItemDataHolder("Robustness", ItemDefinitions.Enchanted_HalfPlateOfRobustness,
                    RecipeDefinitions.Recipe_Enchantment_HalfplateOfRobustness),

                new ItemCollection.MagicItemDataHolder("Survival", ItemDefinitions.Enchanted_LeatherArmorOfSurvival,
                    RecipeDefinitions.Recipe_Enchantment_LeatherArmorOfSurvival),

                new ItemCollection.MagicItemDataHolder("FlameDancing",
                    ItemDefinitions.Enchanted_LeatherArmorOfFlameDancing,
                    RecipeDefinitions.Recipe_Enchantment_LeatherArmorOfFlameDancing),

                new ItemCollection.MagicItemDataHolder("FrostWalking", ItemDefinitions.Enchanted_ScaleMailOfIceDancing,
                    RecipeDefinitions.Recipe_Enchantment_ScaleMailOfIceDancing),

                new ItemCollection.MagicItemDataHolder("Deflection", ItemDefinitions.Enchanted_BreastplateOfDeflection,
                    RecipeDefinitions.Recipe_Enchantment_BreastplateOfDeflection)
            ]
        };
}
