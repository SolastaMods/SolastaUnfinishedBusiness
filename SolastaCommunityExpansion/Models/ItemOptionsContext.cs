using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;
using UnityEngine.AddressableAssets;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionCharacterPresentations;
using static SolastaCommunityExpansion.Api.DatabaseHelper.ItemDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.MerchantDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Models;

internal static class ItemOptionsContext
{
    internal static readonly string[] EmpressGarbAppearances =
    {
        "Normal", "Barbarian Clothes", "Druid Leather", "Elven Chain", "Plain Shirt", "Sorcerer's Armor",
        "Studded Leather", "Sylvan Armor", "Wizard Clothes"
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

    private static ItemPresentation EmpressGarbOriginalItemPresentation { get; set; }

    private static void LoadClothingGorimStock()
    {
        if (!Main.Settings.StockGorimStoreWithAllNonMagicalClothing)
        {
            return;
        }

        foreach (var item in DatabaseRepository.GetDatabase<ItemDefinition>().Where(
                     x => x.ArmorDescription?.ArmorType == "ClothesType" && !x.Magical &&
                          !x.SlotsWhereActive.Contains("TabardSlot") && x != ClothesCommon_Tattoo &&
                          x != ClothesWizard_B))
        {
            var stockClothing = new StockUnitDescription();

            stockClothing.itemDefinition = item;
            stockClothing.initialAmount = 2;
            stockClothing.initialized = true;
            stockClothing.factionStatus = "Indifference";
            stockClothing.maxAmount = 4;
            stockClothing.minAmount = 2;
            stockClothing.stackCount = 1;
            stockClothing.reassortAmount = 1;
            stockClothing.reassortRateValue = 1;
            stockClothing.reassortRateType = RuleDefinitions.DurationType.Day;

            Store_Merchant_Gorim_Ironsoot_Cyflen_GeneralStore.StockUnitDescriptions.Add(stockClothing);
        }
    }

    internal static void SwitchSetBeltOfDwarvenKindBeardChances()
    {
        CharacterPresentationBeltOfDwarvenKind.occurencePercentage =
            Main.Settings.SetBeltOfDwarvenKindBeardChances;
        CharacterPresentationBeltOfDwarvenKind.GuiPresentation.description = Gui.Format(
            "Feature/&AlwaysBeardDescription",
            Main.Settings.SetBeltOfDwarvenKindBeardChances.ToString());
    }

    internal static void SwitchCrownOfTheMagister()
    {
        foreach (var itemPresentation in Crowns.Select(x => x.ItemPresentation))
        {
            var maleBodyPartBehaviours = itemPresentation.GetBodyPartBehaviours(RuleDefinitions.CreatureSex.Male);

            maleBodyPartBehaviours[0] = Main.Settings.EnableInvisibleCrownOfTheMagister
                ? GraphicsCharacterDefinitions.BodyPartBehaviour.Shape
                : GraphicsCharacterDefinitions.BodyPartBehaviour.Armor;
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
            if (!DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyDruidArmor.ForbiddenItemTags.Contains(
                    TagsDefinitions.ItemTagMetal))
            {
                DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyDruidArmor.ForbiddenItemTags.Add(
                    TagsDefinitions.ItemTagMetal);
            }
        }
    }

    internal static void SwitchEmpressGarb()
    {
        EmpressGarbOriginalItemPresentation ??= Enchanted_ChainShirt_Empress_war_garb.ItemPresentation;

        switch (Main.Settings.EmpressGarbAppearance)
        {
            case "Normal":
                Enchanted_ChainShirt_Empress_war_garb.itemPresentation = EmpressGarbOriginalItemPresentation;
                break;

            case "Barbarian Clothes":
                Enchanted_ChainShirt_Empress_war_garb.itemPresentation = BarbarianClothes.ItemPresentation;
                break;

            case "Druid Leather":
                Enchanted_ChainShirt_Empress_war_garb.itemPresentation = LeatherDruid.ItemPresentation;
                break;

            case "Elven Chain":
                Enchanted_ChainShirt_Empress_war_garb.itemPresentation = ElvenChain.ItemPresentation;
                break;

            case "Plain Shirt":
                Enchanted_ChainShirt_Empress_war_garb.itemPresentation = EmpressGarbOriginalItemPresentation;
                Enchanted_ChainShirt_Empress_war_garb.ItemPresentation.useCustomArmorMaterial = false;
                break;

            case "Studded Leather":
                Enchanted_ChainShirt_Empress_war_garb.itemPresentation = StuddedLeather.ItemPresentation;
                break;

            case "Sylvan Armor":
                Enchanted_ChainShirt_Empress_war_garb.itemPresentation = GreenmageArmor.ItemPresentation;
                break;

            case "Wizard Clothes":
                Enchanted_ChainShirt_Empress_war_garb.itemPresentation = WizardClothes_Alternate.ItemPresentation;
                break;

            case "Sorcerer's Armor":
                Enchanted_ChainShirt_Empress_war_garb.itemPresentation = SorcererArmor.ItemPresentation;
                break;
        }
    }

    internal static void SwitchFociItems()
    {
        if (Main.Settings.StockHugoStoreWithAdditionalFoci)
        {
            Store_Merchant_Hugo_Requer_Cyflen_Potions.StockUnitDescriptions
                .AddRange(WandIdentifyBuilder.StockFocus);
            Store_Merchant_Hugo_Requer_Cyflen_Potions.StockUnitDescriptions.AddRange(FocusDefinitionBuilder
                .StockFocus);
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

        WandIdentifyBuilder.WandIdentify.GuiPresentation.hidden = !Main.Settings.StockHugoStoreWithAdditionalFoci;

        FocusDefinitionBuilder.ArcaneStaff.GuiPresentation.hidden = !Main.Settings.StockHugoStoreWithAdditionalFoci;
        FocusDefinitionBuilder.DruidicAmulet.GuiPresentation.hidden = !Main.Settings
            .StockHugoStoreWithAdditionalFoci;
        FocusDefinitionBuilder.LivewoodClub.GuiPresentation.hidden = !Main.Settings
            .StockHugoStoreWithAdditionalFoci;
        FocusDefinitionBuilder.LivewoodStaff.GuiPresentation.hidden = !Main.Settings
            .StockHugoStoreWithAdditionalFoci;
    }

    internal static void SwitchFociItemsDungeonMaker()
    {
        FocusDefinitionBuilder.ArcaneStaff.inDungeonEditor = Main.Settings.EnableAdditionalFociInDungeonMaker;
        FocusDefinitionBuilder.DruidicAmulet.inDungeonEditor = Main.Settings.EnableAdditionalFociInDungeonMaker;
        FocusDefinitionBuilder.LivewoodClub.inDungeonEditor = Main.Settings.EnableAdditionalFociInDungeonMaker;
        FocusDefinitionBuilder.LivewoodStaff.inDungeonEditor = Main.Settings.EnableAdditionalFociInDungeonMaker;
    }

    internal static void SwitchMagicStaffFoci()
    {
        foreach (var item in DatabaseRepository.GetDatabase<ItemDefinition>()
                     .Where(x => x.IsWeapon) // WeaponDescription could be null
                     .Where(x => x.WeaponDescription.WeaponType == EquipmentDefinitions.WeaponTypeQuarterstaff)
                     .Where(x => x.Magical && !x.Name.Contains("OfHealing")))
        {
            if (!Main.Settings.MakeAllMagicStaveArcaneFoci)
            {
                continue;
            }

            item.IsFocusItem = true;
            item.FocusItemDescription.focusType = EquipmentDefinitions.FocusType.Arcane;
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
            stock.reassortAmount = 1;
            stock.reassortRateValue = 7;
        }
    }

    internal static void SwitchRestockArcaneum()
    {
        if (!Main.Settings.RestockArcaneum)
        {
            return;
        }

        foreach (var stock in Store_Merchant_Arcaneum_Heddlon_Surespell.StockUnitDescriptions)
        {
            stock.reassortAmount = 1;
        }
    }

    internal static void SwitchRestockCircleOfDanantar()
    {
        if (!Main.Settings.RestockCircleOfDanantar)
        {
            return;
        }

        foreach (var stock in Store_Merchant_CircleOfDanantar_Joriel_Foxeye.StockUnitDescriptions)
        {
            stock.reassortAmount = 1;
        }
    }

    internal static void SwitchRestockTowerOfKnowledge()
    {
        if (!Main.Settings.RestockTowerOfKnowledge)
        {
            return;
        }

        foreach (var stock in Store_Merchant_TowerOfKnowledge_Maddy_Greenisle.StockUnitDescriptions)
        {
            stock.reassortAmount = 1;
        }
    }

    internal static void SwitchUniversalSylvanArmorAndLightbringer()
    {
        GreenmageArmor.RequiredAttunementClasses.Clear();
        WizardClothes_Alternate.RequiredAttunementClasses.Clear();

        if (Main.Settings.AllowAnyClassToWearSylvanArmor)
        {
            return;
        }

        GreenmageArmor.RequiredAttunementClasses.Add(Wizard);
        WizardClothes_Alternate.RequiredAttunementClasses.Add(Wizard);
    }

    private static void LoadRemoveIdentification()
    {
        if (Main.Settings.RemoveIdentifcationRequirements)
        {
            foreach (var item in DatabaseRepository.GetDatabase<ItemDefinition>())
            {
                item.requiresIdentification = false;
            }
        }

        if (!Main.Settings.RemoveAttunementRequirements)
        {
            return;
        }

        {
            foreach (var item in DatabaseRepository.GetDatabase<ItemDefinition>())
            {
                item.requiresAttunement = false;
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
        SwitchUniversalSylvanArmorAndLightbringer();
    }

    private sealed class WandIdentifyBuilder : ItemDefinitionBuilder
    {
        internal static readonly HashSet<StockUnitDescription> StockFocus = new();

        internal static readonly ItemDefinition WandIdentify = CreateAndAddToDB(
            "WandIdentify",
            "46ae7624-4d24-455a-98f9-d41403b0ae19",
            "Equipment/&WandIdentifyTitle",
            "Equipment/&WandIdentifyDescription",
            WandMagicMissile);

        private WandIdentifyBuilder(string name, string guid, string title, string description,
            ItemDefinition original) : base(original, name, guid)
        {
            Definition.GuiPresentation.Title = title;
            Definition.GuiPresentation.Description = description;
            Definition.UsableDeviceDescription.DeviceFunctions[0].spellDefinition = Identify;

            var stockFocus = new StockUnitDescription
            {
                itemDefinition = Definition,
                initialAmount = 1,
                initialized = true,
                factionStatus = "Indifference",
                maxAmount = 2,
                minAmount = 1,
                stackCount = 1,
                reassortAmount = 1,
                reassortRateValue = 1,
                reassortRateType = RuleDefinitions.DurationType.Day
            };

            StockFocus.Add(stockFocus);
        }

        private static ItemDefinition CreateAndAddToDB(string name, string guid, string title, string description,
            ItemDefinition original)
        {
            return new WandIdentifyBuilder(name, guid, title, description, original).AddToDB();
        }
    }

    private sealed class FocusDefinitionBuilder : ItemDefinitionBuilder
    {
        internal static readonly HashSet<StockUnitDescription> StockFocus = new();

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
            Definition.FocusItemDescription.focusType = type;
            Definition.GuiPresentation.Title = title;
            Definition.GuiPresentation.Description = description;

            if (assetReferenceSprite != null)
            {
                Definition.GuiPresentation.spriteReference = assetReferenceSprite;
            }

            Definition.costs = ComponentPouch.Costs;

            if (slotTypes.Length > 0)
            {
                Definition.SlotTypes.SetRange(slotTypes);
                Definition.SlotTypes.Add(EquipmentDefinitions.SlotTypeContainer);
                Definition.SlotsWhereActive.SetRange(slotTypes);
            }

            var stockFocus = new StockUnitDescription();

            stockFocus.itemDefinition = Definition;
            stockFocus.initialAmount = 1;
            stockFocus.initialized = true;
            stockFocus.factionStatus = "Indifference";
            stockFocus.maxAmount = 2;
            stockFocus.minAmount = 1;
            stockFocus.stackCount = 1;
            stockFocus.reassortAmount = 1;
            stockFocus.reassortRateValue = 1;
            stockFocus.reassortRateType = RuleDefinitions.DurationType.Day;

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
            return new FocusDefinitionBuilder(name, guid, title, description, original, type, assetReferenceSprite,
                slotTypes).AddToDB();
        }
    }
}
