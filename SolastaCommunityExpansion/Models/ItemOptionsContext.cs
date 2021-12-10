using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionCharacterPresentations;
using static SolastaModApi.DatabaseHelper.ItemDefinitions;
using static SolastaModApi.DatabaseHelper.MerchantDefinitions;

namespace SolastaCommunityExpansion.Models
{
    internal static class ItemOptionsContext
    {
        private static readonly List<ItemDefinition> Crowns = new List<ItemDefinition>
        {
            CrownOfTheMagister,
            CrownOfTheMagister01,
            CrownOfTheMagister02,
            CrownOfTheMagister03,
            CrownOfTheMagister04,
            CrownOfTheMagister05,
            CrownOfTheMagister06,
            CrownOfTheMagister07,
            CrownOfTheMagister08,
            CrownOfTheMagister09,
            CrownOfTheMagister10,
            CrownOfTheMagister11,
            CrownOfTheMagister12
        };

        internal static void LoadClothingGorimStock()
        {
            if (!Main.Settings.EnableClothingGorimStock)
            {
                return;
            }
            
            foreach (ItemDefinition item in DatabaseRepository.GetDatabase<ItemDefinition>().Where(x => x.ArmorDescription.ArmorType == "ClothesType" && !x.Magical && !x.Name.Contains("_Tattoo") && !x.SlotsWhereActive.Contains("TabardSlot")))
            {
                var stockClothing = new StockUnitDescription();

                stockClothing.SetItemDefinition(item);
                stockClothing.SetInitialAmount(2);
                stockClothing.SetInitialized(true);
                stockClothing.SetFactionStatus("Indifference");
                stockClothing.SetMaxAmount(4);
                stockClothing.SetMinAmount(2);
                stockClothing.SetStackCount(1);
                stockClothing.SetReassortAmount(1);
                stockClothing.SetReassortRateValue(1);
                stockClothing.SetReassortRateType(RuleDefinitions.DurationType.Day);

                Store_Merchant_Gorim_Ironsoot_Cyflen_GeneralStore.StockUnitDescriptions.Add(stockClothing);
            }
        }

        internal static void SwitchBeltOfDwarvenKindBeardChances()
        {
            CharacterPresentationBeltOfDwarvenKind.SetOccurencePercentage(Main.Settings.BeltOfDwarvenKindBeardChances);
            CharacterPresentationBeltOfDwarvenKind.GuiPresentation.SetDescription(Gui.Format("Feature/&AlwaysBeardDescription", Main.Settings.BeltOfDwarvenKindBeardChances.ToString()));
        }

        internal static void SwitchCrownOfTheMagister()
        {
            foreach (var itemPresentation in Crowns.Select(x => x.ItemPresentation))
            {
                var maleBodyPartBehaviours = AccessTools.Field(itemPresentation.GetType(), "maleBodyPartBehaviours").GetValue(itemPresentation) as GraphicsCharacterDefinitions.BodyPartBehaviour[];

                maleBodyPartBehaviours[0] = Main.Settings.EnableInvisibleCrownOfTheMagister ? GraphicsCharacterDefinitions.BodyPartBehaviour.Shape : GraphicsCharacterDefinitions.BodyPartBehaviour.Armor;
            }
        }

        internal static void SwitchMagicStaffFoci()
        {
            foreach (ItemDefinition item in DatabaseRepository.GetDatabase<ItemDefinition>().Where(x => x.WeaponDescription.WeaponType == EquipmentDefinitions.WeaponTypeQuarterstaff && x.Magical && !x.Name.Contains("OfHealing")))
            {

                if (Main.Settings.EnableMagicStaffFoci)
                {
                    item.SetIsFocusItem(Main.Settings.EnableMagicStaffFoci);
                    item.FocusItemDescription.SetFocusType(Main.Settings.EnableMagicStaffFoci ? EquipmentDefinitions.FocusType.Arcane : EquipmentDefinitions.FocusType.None);
                }
            }
        }

        internal static void SwitchRestockAntiquarian()
        {
            foreach (var stock in DatabaseHelper.MerchantDefinitions.Store_Merchant_Antiquarians_Halman_Summer.StockUnitDescriptions)
            {
                if (!stock.ItemDefinition.Name.Contains("Manual"))
                {
                    if (!stock.ItemDefinition.Name.Contains("Tome"))
                    {
                        stock.SetReassortAmount(Main.Settings.EnableRestockAntiquarians ? 1 : 0);
                        stock.SetReassortRateValue(Main.Settings.EnableRestockAntiquarians ? 7 : 21);
                    }
                }
            }
        }

        internal static void SwitchRestockArcaneum()
        {
            foreach (StockUnitDescription stock in DatabaseHelper.MerchantDefinitions.Store_Merchant_Arcaneum_Heddlon_Surespell.StockUnitDescriptions)
            {
                stock.SetReassortAmount(Main.Settings.EnableRestockArcaneum ? 1 : 0);
            }
        }

        internal static void SwitchRestockCircleOfDanantar()
        {
            foreach (StockUnitDescription stock in DatabaseHelper.MerchantDefinitions.Store_Merchant_CircleOfDanantar_Joriel_Foxeye.StockUnitDescriptions)
            {
                stock.SetReassortAmount(Main.Settings.EnableRestockCircleOfDanantar ? 1 : 0);
            }

            foreach (StockUnitDescription stock in DatabaseHelper.MerchantDefinitions.Store_Merchant_TowerOfKnowledge_Maddy_Greenisle.StockUnitDescriptions)
            {
                stock.SetReassortAmount(Main.Settings.EnableRestockTowerOfKnowledge ? 1 : 0);
            }
        }

        internal static void SwitchRestockTowerOfKnowledge()
        {
            foreach (StockUnitDescription stock in DatabaseHelper.MerchantDefinitions.Store_Merchant_TowerOfKnowledge_Maddy_Greenisle.StockUnitDescriptions)
            {
                stock.SetReassortAmount(Main.Settings.EnableRestockTowerOfKnowledge ? 1 : 0);
            }
        }

        internal static void SwitchUniversalSylvanArmor()
        {
            GreenmageArmor.RequiredAttunementClasses.Clear();

            if (!Main.Settings.EnableUniversalSylvanArmor)
            {
                GreenmageArmor.RequiredAttunementClasses.Add(Wizard);
            }
        }

        internal static void Load()
        {
            LoadClothingGorimStock();
            SwitchBeltOfDwarvenKindBeardChances();
            SwitchCrownOfTheMagister();
            SwitchMagicStaffFoci();
            SwitchRestockAntiquarian();
            SwitchRestockArcaneum();
            SwitchRestockCircleOfDanantar();
            SwitchRestockTowerOfKnowledge();
            SwitchUniversalSylvanArmor();
        }
    }
}
