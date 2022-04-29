using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Builders;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using UnityEngine.AddressableAssets;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionCharacterPresentations;
using static SolastaModApi.DatabaseHelper.ItemDefinitions;
using static SolastaModApi.DatabaseHelper.MerchantDefinitions;
using static SolastaModApi.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Models
{
    internal static class ItemOptionsContext
    {
        private sealed class WandIdentifyBuilder : ItemDefinitionBuilder
        {
            internal static readonly HashSet<StockUnitDescription> StockFocus = new();

            private WandIdentifyBuilder(string name, string guid, string title, string description, ItemDefinition original) : base(original, name, guid)
            {
                Definition.GuiPresentation.Title = title;
                Definition.GuiPresentation.Description = description;
                Definition.UsableDeviceDescription.DeviceFunctions[0].SetSpellDefinition(Identify);

                var stockFocus = new StockUnitDescription();

                stockFocus.SetItemDefinition(Definition);
                stockFocus.SetInitialAmount(1);
                stockFocus.SetInitialized(true);
                stockFocus.SetFactionStatus("Indifference");
                stockFocus.SetMaxAmount(2);
                stockFocus.SetMinAmount(1);
                stockFocus.SetStackCount(1);
                stockFocus.SetReassortAmount(1);
                stockFocus.SetReassortRateValue(1);
                stockFocus.SetReassortRateType(RuleDefinitions.DurationType.Day);

                StockFocus.Add(stockFocus);
            }

            private static ItemDefinition CreateAndAddToDB(string name, string guid, string title, string description, ItemDefinition original)
            {
                return new WandIdentifyBuilder(name, guid, title, description, original).AddToDB();
            }

            internal static readonly ItemDefinition WandIdentify = CreateAndAddToDB(
                "WandIdentify",
                "46ae7624-4d24-455a-98f9-d41403b0ae19",
                "Equipment/&WandIdentifyTitle",
                "Equipment/&WandIdentifyDescription",
                WandMagicMissile);
        }

        private sealed class FocusDefinitionBuilder : ItemDefinitionBuilder
        {
            internal static readonly HashSet<StockUnitDescription> StockFocus = new();

            private FocusDefinitionBuilder(
                string name,
                string guid,
                string title,
                string description,
                ItemDefinition original,
                EquipmentDefinitions.FocusType type,
                AssetReferenceSprite assetReferenceSprite,
                params string[] slotTypes) : base(original, name, guid)
            {
                // Use IsXXXItem = true/SetIsXXXItem(true) before using the XXXItemDescription
                Definition.IsFocusItem = true;
                Definition.FocusItemDescription.SetFocusType(type);
                Definition.GuiPresentation.Title = title;
                Definition.GuiPresentation.Description = description;

                if (assetReferenceSprite != null)
                {
                    Definition.GuiPresentation.SetSpriteReference(assetReferenceSprite);
                }

                Definition.SetCosts(ComponentPouch.Costs);

                if (slotTypes.Length > 0)
                {
                    Definition.SlotTypes.SetRange(slotTypes);
                    Definition.SlotTypes.Add(EquipmentDefinitions.SlotTypeContainer);
                    Definition.SlotsWhereActive.SetRange(slotTypes);
                }

                var stockFocus = new StockUnitDescription();

                stockFocus.SetItemDefinition(Definition);
                stockFocus.SetInitialAmount(1);
                stockFocus.SetInitialized(true);
                stockFocus.SetFactionStatus("Indifference");
                stockFocus.SetMaxAmount(2);
                stockFocus.SetMinAmount(1);
                stockFocus.SetStackCount(1);
                stockFocus.SetReassortAmount(1);
                stockFocus.SetReassortRateValue(1);
                stockFocus.SetReassortRateType(RuleDefinitions.DurationType.Day);

                StockFocus.Add(stockFocus);
            }

            private static ItemDefinition CreateAndAddToDB(
                string name,
                string guid,
                string title,
                string description,
                ItemDefinition original,
                EquipmentDefinitions.FocusType type,
                AssetReferenceSprite assetReferenceSprite,
                params string[] slotTypes)
            {
                return new FocusDefinitionBuilder(name, guid, title, description, original, type, assetReferenceSprite, slotTypes).AddToDB();
            }

            internal static readonly ItemDefinition ArcaneStaff = CreateAndAddToDB(
                "ArcaneStaff",
                "991e1fec-9777-4635-948f-5bedcb96147d",
                "Equipment/&ArcaneStaffTitle",
                "Equipment/&ArcaneStaffDescription",
                Quarterstaff,
                EquipmentDefinitions.FocusType.Arcane,
                QuarterstaffPlus1.GuiPresentation.SpriteReference);

            internal static readonly ItemDefinition DruidicAmulet = CreateAndAddToDB(
                "DruidicAmulet",
                "3487d3b2-1058-4c0f-8009-9e4f525cb0e0",
                "Equipment/&DruidicAmuletTitle",
                "Equipment/&DruidicAmuletDescription",
                ComponentPouch_ArcaneAmulet,
                EquipmentDefinitions.FocusType.Druidic,
                BeltOfGiantHillStrength.GuiPresentation.SpriteReference);

            internal static readonly ItemDefinition LivewoodClub = CreateAndAddToDB(
                "LivewoodClub",
                "dd27119b-01e0-4a47-a043-98b89dc930a1",
                "Equipment/&LivewoodClubTitle",
                "Equipment/&LivewoodClubDescription",
                Club,
                EquipmentDefinitions.FocusType.Druidic,
                null);

            internal static readonly ItemDefinition LivewoodStaff = CreateAndAddToDB(
                "LivewoodStaff",
                "ff3ec29c-734f-4ef6-8d6e-ceb961d9a8a0",
                "Equipment/&LivewoodStaffTitle",
                "Equipment/&LivewoodStaffDescription",
                Quarterstaff,
                EquipmentDefinitions.FocusType.Druidic,
                StaffOfHealing.GuiPresentation.SpriteReference);
        }

        private static ItemPresentation EmpressGarbOriginalItemPresentation { get; set; }

        internal static readonly string[] EmpressGarbAppearances =
        {
            "Normal",
            "Barbarian Clothes",
            "Druid Leather",
            "Elven Chain",
            "Plain Shirt",
            "Sorcerer's Armor",
            "Studded Leather",
            "Sylvan Armor",
            "Wizard Clothes"
        };

        private static readonly List<ItemDefinition> Crowns = new()
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
            if (!Main.Settings.StockGorimStoreWithAllNonMagicalClothing)
            {
                return;
            }

            foreach (ItemDefinition item in DatabaseRepository.GetDatabase<ItemDefinition>().Where(
                x => x.ArmorDescription.ArmorType == "ClothesType" && !x.Magical && !x.SlotsWhereActive.Contains("TabardSlot") && x != ClothesCommon_Tattoo && x != ClothesWizard_B))
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

        internal static void SwitchSetBeltOfDwarvenKindBeardChances()
        {
            CharacterPresentationBeltOfDwarvenKind.SetOccurencePercentage(Main.Settings.SetBeltOfDwarvenKindBeardChances);
            CharacterPresentationBeltOfDwarvenKind.GuiPresentation.SetDescription(Gui.Format("Feature/&AlwaysBeardDescription", Main.Settings.SetBeltOfDwarvenKindBeardChances.ToString()));
        }

        internal static void SwitchCrownOfTheMagister()
        {
            foreach (var itemPresentation in Crowns.Select(x => x.ItemPresentation))
            {
                var maleBodyPartBehaviours = itemPresentation.GetBodyPartBehaviours(RuleDefinitions.CreatureSex.Male);

                maleBodyPartBehaviours[0] = Main.Settings.EnableInvisibleCrownOfTheMagister ? GraphicsCharacterDefinitions.BodyPartBehaviour.Shape : GraphicsCharacterDefinitions.BodyPartBehaviour.Armor;
            }
        }

        internal static void SwitchDruidAllowMetalArmor()
        {
            var active = Main.Settings.AllowDruidToWearMetalArmor;

            if (active)
            {
                DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyDruidArmor.ForbiddenItemTags.Clear();
            }
            else
            {
                if (!DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyDruidArmor.ForbiddenItemTags.Contains(TagsDefinitions.ItemTagMetal))
                {
                    DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyDruidArmor.ForbiddenItemTags.Add(TagsDefinitions.ItemTagMetal);
                }
            }
        }

        internal static void SwitchEmpressGarb()
        {
            if (EmpressGarbOriginalItemPresentation == null)
            {
                EmpressGarbOriginalItemPresentation = Enchanted_ChainShirt_Empress_war_garb.ItemPresentation;
            }

            switch (Main.Settings.EmpressGarbAppearance)
            {
                case "Normal":
                    Enchanted_ChainShirt_Empress_war_garb.SetItemPresentation(EmpressGarbOriginalItemPresentation);
                    break;

                case "Barbarian Clothes":
                    Enchanted_ChainShirt_Empress_war_garb.SetItemPresentation(BarbarianClothes.ItemPresentation);
                    break;

                case "Druid Leather":
                    Enchanted_ChainShirt_Empress_war_garb.SetItemPresentation(LeatherDruid.ItemPresentation);
                    break;

                case "Elven Chain":
                    Enchanted_ChainShirt_Empress_war_garb.SetItemPresentation(ElvenChain.ItemPresentation);
                    break;

                case "Plain Shirt":
                    Enchanted_ChainShirt_Empress_war_garb.SetItemPresentation(EmpressGarbOriginalItemPresentation);
                    Enchanted_ChainShirt_Empress_war_garb.ItemPresentation.SetUseCustomArmorMaterial(false);
                    break;

                case "Studded Leather":
                    Enchanted_ChainShirt_Empress_war_garb.SetItemPresentation(StuddedLeather.ItemPresentation);
                    break;

                case "Sylvan Armor":
                    Enchanted_ChainShirt_Empress_war_garb.SetItemPresentation(GreenmageArmor.ItemPresentation);
                    break;

                case "Wizard Clothes":
                    Enchanted_ChainShirt_Empress_war_garb.SetItemPresentation(WizardClothes_Alternate.ItemPresentation);
                    break;

                case "Sorcerer's Armor":
                    Enchanted_ChainShirt_Empress_war_garb.SetItemPresentation(SorcererArmor.ItemPresentation);
                    break;
            }
        }

        internal static void SwitchFociItems()
        {
            if (Main.Settings.StockHugoStoreWithAdditionalFoci)
            {
                Store_Merchant_Hugo_Requer_Cyflen_Potions.StockUnitDescriptions.AddRange(WandIdentifyBuilder.StockFocus);
                Store_Merchant_Hugo_Requer_Cyflen_Potions.StockUnitDescriptions.AddRange(FocusDefinitionBuilder.StockFocus);
            }
            else
            {
                foreach (var stockFocus in WandIdentifyBuilder.StockFocus)
                {
                    Store_Merchant_Hugo_Requer_Cyflen_Potions.StockUnitDescriptions.Remove(stockFocus);
                }

                foreach (var stockFocus in FocusDefinitionBuilder.StockFocus)
                {
                    Store_Merchant_Hugo_Requer_Cyflen_Potions.StockUnitDescriptions.Remove(stockFocus);
                }
            }

            WandIdentifyBuilder.WandIdentify.GuiPresentation.SetHidden(!Main.Settings.StockHugoStoreWithAdditionalFoci);

            FocusDefinitionBuilder.ArcaneStaff.GuiPresentation.SetHidden(!Main.Settings.StockHugoStoreWithAdditionalFoci);
            FocusDefinitionBuilder.DruidicAmulet.GuiPresentation.SetHidden(!Main.Settings.StockHugoStoreWithAdditionalFoci);
            FocusDefinitionBuilder.LivewoodClub.GuiPresentation.SetHidden(!Main.Settings.StockHugoStoreWithAdditionalFoci);
            FocusDefinitionBuilder.LivewoodStaff.GuiPresentation.SetHidden(!Main.Settings.StockHugoStoreWithAdditionalFoci);
        }

        internal static void SwitchFociItemsDungeonMaker()
        {
            FocusDefinitionBuilder.ArcaneStaff.SetInDungeonEditor(Main.Settings.EnableAdditionalFociInDungeonMaker);
            FocusDefinitionBuilder.DruidicAmulet.SetInDungeonEditor(Main.Settings.EnableAdditionalFociInDungeonMaker);
            FocusDefinitionBuilder.LivewoodClub.SetInDungeonEditor(Main.Settings.EnableAdditionalFociInDungeonMaker);
            FocusDefinitionBuilder.LivewoodStaff.SetInDungeonEditor(Main.Settings.EnableAdditionalFociInDungeonMaker);
        }

        internal static void SwitchMagicStaffFoci()
        {
            foreach (ItemDefinition item in DatabaseRepository.GetDatabase<ItemDefinition>()
                .Where(x => x.IsWeapon) // WeaponDescription could be null
                .Where(x => x.WeaponDescription.WeaponType == EquipmentDefinitions.WeaponTypeQuarterstaff)
                .Where(x => x.Magical && !x.Name.Contains("OfHealing")))
            {
                if (Main.Settings.MakeAllMagicStaveArcaneFoci)
                {
                    item.IsFocusItem = true;
                    item.FocusItemDescription.SetFocusType(EquipmentDefinitions.FocusType.Arcane);
                }
            }
        }

        internal static void SwitchRestockAntiquarian()
        {
            if (!Main.Settings.RestockAntiquarians)
            {
                return;
            }

            foreach (var stock in Store_Merchant_Antiquarians_Halman_Summer.StockUnitDescriptions.Where(
                x => !x.ItemDefinition.Name.Contains("Manual") && !x.ItemDefinition.Name.Contains("Tome")))
            {
                stock.SetReassortAmount(1);
                stock.SetReassortRateValue(7);
            }
        }

        internal static void SwitchRestockArcaneum()
        {
            if (!Main.Settings.RestockArcaneum)
            {
                return;
            }

            foreach (StockUnitDescription stock in Store_Merchant_Arcaneum_Heddlon_Surespell.StockUnitDescriptions)
            {
                stock.SetReassortAmount(1);
            }
        }

        internal static void SwitchRestockCircleOfDanantar()
        {
            if (!Main.Settings.RestockCircleOfDanantar)
            {
                return;
            }

            foreach (StockUnitDescription stock in Store_Merchant_CircleOfDanantar_Joriel_Foxeye.StockUnitDescriptions)
            {
                stock.SetReassortAmount(1);
            }
        }

        internal static void SwitchRestockTowerOfKnowledge()
        {
            if (!Main.Settings.RestockTowerOfKnowledge)
            {
                return;
            }

            foreach (StockUnitDescription stock in Store_Merchant_TowerOfKnowledge_Maddy_Greenisle.StockUnitDescriptions)
            {
                stock.SetReassortAmount(1);
            }
        }

        internal static void SwitchUniversalSylvanArmor()
        {
            GreenmageArmor.RequiredAttunementClasses.Clear();

            if (!Main.Settings.AllowAnyClassToWearSylvanArmor)
            {
                GreenmageArmor.RequiredAttunementClasses.Add(Wizard);
            }
        }

        internal static void LoadRemoveIdentification()
        {
            if (Main.Settings.RemoveIdentifcationRequirements)
            {
                foreach (ItemDefinition item in DatabaseRepository.GetDatabase<ItemDefinition>())
                {
                    item.SetRequiresIdentification(false);
                }
            }

            if (Main.Settings.RemoveAttunementRequirements)
            {
                foreach (ItemDefinition item in DatabaseRepository.GetDatabase<ItemDefinition>())
                {
                    item.SetRequiresAttunement(false);
                }
            }
        }

        internal static void Load()
        {
            LoadRemoveIdentification();
            LoadClothingGorimStock();
            SwitchSetBeltOfDwarvenKindBeardChances();
            SwitchCrownOfTheMagister();
            SwitchDruidAllowMetalArmor();
            SwitchEmpressGarb();
            SwitchFociItems();
            SwitchFociItemsDungeonMaker();
            SwitchMagicStaffFoci();
            SwitchRestockAntiquarian();
            SwitchRestockArcaneum();
            SwitchRestockCircleOfDanantar();
            SwitchRestockTowerOfKnowledge();
            SwitchUniversalSylvanArmor();
        }
    }
}
