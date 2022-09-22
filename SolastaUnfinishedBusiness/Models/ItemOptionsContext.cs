using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using UnityEngine.AddressableAssets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionCharacterPresentations;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ItemDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MerchantDefinitions;

namespace SolastaUnfinishedBusiness.Models;

internal static class ItemOptionsContext
{
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

    internal static string[] EmpressGarbAppearances { get; } =
    {
        Gui.Localize("Modal/&TravelPaceNormalTitle"), Gui.Localize("Equipment/&Barbarian_Clothes_Title"),
        Gui.Localize("Equipment/&Druid_Leather_Title"), Gui.Localize("Equipment/&ElvenChain_Unidentified_Title"),
        Gui.Localize("Equipment/&Armor_Commoner_ClothesTitle"),
        CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Gui.Localize("Equipment/&Armor_Sorcerer_Outfit_Title")),
        Gui.Localize("Equipment/&Armor_StuddedLeatherTitle"), Gui.Localize("Equipment/&GreenmageArmor_Title"),
        Gui.Localize("Equipment/&Armor_Adventuring_Wizard_OutfitTitle")
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
            var stockClothing = new StockUnitDescription
            {
                itemDefinition = item,
                initialAmount = 2,
                initialized = true,
                factionStatus = "Indifference",
                maxAmount = 4,
                minAmount = 2,
                stackCount = 1,
                reassortAmount = 1,
                reassortRateValue = 1,
                reassortRateType = RuleDefinitions.DurationType.Day
            };

            Store_Merchant_Gorim_Ironsoot_Cyflen_GeneralStore.StockUnitDescriptions.Add(stockClothing);
        }

        //rename valley noble's clothes by color to avoid confusion
        var silverNoble = ClothesNoble_Valley_Silver;
        silverNoble.GuiPresentation.title = "Equipment/&Armor_Noble_ClothesTitle_Silver";

        var redNoble = ClothesNoble_Valley_Red;
        redNoble.GuiPresentation.title = "Equipment/&Armor_Noble_ClothesTitle_Red";

        var purpleNoble = ClothesNoble_Valley_Purple;
        purpleNoble.GuiPresentation.title = "Equipment/&Armor_Noble_ClothesTitle_Purple";

        var pinkNoble = ClothesNoble_Valley_Pink;
        pinkNoble.GuiPresentation.title = "Equipment/&Armor_Noble_ClothesTitle_Pink";

        var orangeNoble = ClothesNoble_Valley_Orange;
        orangeNoble.GuiPresentation.title = "Equipment/&Armor_Noble_ClothesTitle_Orange";

        var greenNoble = ClothesNoble_Valley_Green;
        greenNoble.GuiPresentation.title = "Equipment/&Armor_Noble_ClothesTitle_Green";

        var cherryNoble = ClothesNoble_Valley_Cherry;
        cherryNoble.GuiPresentation.title = "Equipment/&Armor_Noble_ClothesTitle_Cherry";

        var valleyNoble = ClothesNoble_Valley;
        valleyNoble.GuiPresentation.title = "Equipment/&Armor_Noble_ClothesTitle_Valley";
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

        switch (Main.Settings.EmpressGarbAppearanceIndex)
        {
            case 0: //"Normal":
                Enchanted_ChainShirt_Empress_war_garb.itemPresentation = EmpressGarbOriginalItemPresentation;
                break;

            case 1: // Barbarian Clothes
                Enchanted_ChainShirt_Empress_war_garb.itemPresentation = BarbarianClothes.ItemPresentation;
                break;

            case 2: // Druid Leather
                Enchanted_ChainShirt_Empress_war_garb.itemPresentation = LeatherDruid.ItemPresentation;
                break;

            case 3: // Elven Chain
                Enchanted_ChainShirt_Empress_war_garb.itemPresentation = ElvenChain.ItemPresentation;
                break;

            case 4: // Plain Shirt
                Enchanted_ChainShirt_Empress_war_garb.itemPresentation = EmpressGarbOriginalItemPresentation;
                Enchanted_ChainShirt_Empress_war_garb.ItemPresentation.useCustomArmorMaterial = false;
                break;

            case 5: // Sorcerer's Armor
                Enchanted_ChainShirt_Empress_war_garb.itemPresentation = SorcererArmor.ItemPresentation;
                break;

            case 6: // Studded Leather
                Enchanted_ChainShirt_Empress_war_garb.itemPresentation = StuddedLeather.ItemPresentation;
                break;

            case 7: // Sylvan Armor
                Enchanted_ChainShirt_Empress_war_garb.itemPresentation = GreenmageArmor.ItemPresentation;
                break;

            case 8: // Wizard Clothes
                Enchanted_ChainShirt_Empress_war_garb.itemPresentation = WizardClothes_Alternate.ItemPresentation;
                break;
        }
    }

    internal static void SwitchFociItems()
    {
        if (Main.Settings.StockHugoStoreWithAdditionalFoci)
        {
            Store_Merchant_Hugo_Requer_Cyflen_Potions.StockUnitDescriptions.AddRange(FocusDefinitionBuilder
                .StockFocus);
        }
        else
        {
            foreach (var stockFocus in FocusDefinitionBuilder.StockFocus)
            {
                Store_Merchant_Hugo_Requer_Cyflen_Potions.StockUnitDescriptions.Remove(stockFocus);
            }
        }

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

    internal static void SwitchAttuneArcaneShieldstaff()
    {
        if (Main.Settings.AllowAnyClassToUseArcaneShieldstaff)
        {
            ArcaneShieldstaff.RequiredAttunementClasses.Clear();
        }
        else
        {
            ArcaneShieldstaff.RequiredAttunementClasses.SetRange(Wizard, Cleric, Paladin, Ranger, Sorcerer, Warlock);
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
        if (Main.Settings.RemoveIdentificationRequirements)
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
        SwitchAttuneArcaneShieldstaff();
    }

    private sealed class FocusDefinitionBuilder : ItemDefinitionBuilder
    {
        internal static readonly HashSet<StockUnitDescription> StockFocus = new();

        internal static readonly ItemDefinition ArcaneStaff = CreateAndAddToDB(
            "CEArcaneStaff",
            Quarterstaff,
            EquipmentDefinitions.FocusType.Arcane,
            QuarterstaffPlus1.GuiPresentation.SpriteReference);

        internal static readonly ItemDefinition DruidicAmulet = CreateAndAddToDB(
            "CEDruidicAmulet",
            ComponentPouch_ArcaneAmulet,
            EquipmentDefinitions.FocusType.Druidic,
            BeltOfGiantHillStrength.GuiPresentation.SpriteReference);

        internal static readonly ItemDefinition LivewoodClub = CreateAndAddToDB(
            "CELivewoodClub",
            Club,
            EquipmentDefinitions.FocusType.Druidic,
            null);

        internal static readonly ItemDefinition LivewoodStaff = CreateAndAddToDB(
            "CELivewoodStaff",
            Quarterstaff,
            EquipmentDefinitions.FocusType.Druidic,
            StaffOfHealing.GuiPresentation.SpriteReference);

        private FocusDefinitionBuilder(
            string name,
            string guid,
            ItemDefinition original,
            EquipmentDefinitions.FocusType type,
            [CanBeNull] AssetReferenceSprite assetReferenceSprite,
            [NotNull] params string[] slotTypes) : base(original, name, guid)
        {
            // Use IsXXXItem = true/SetIsXXXItem(true) before using the XXXItemDescription
            Definition.IsFocusItem = true;
            Definition.FocusItemDescription.focusType = type;
            Definition.GuiPresentation = GuiPresentationBuilder.Build(name, Category.Item);

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

        private static ItemDefinition CreateAndAddToDB(
            string name,
            ItemDefinition original,
            EquipmentDefinitions.FocusType type,
            AssetReferenceSprite assetReferenceSprite,
            [NotNull] params string[] slotTypes)
        {
            var guid = GuidHelper.Create(CENamespaceGuid, name).ToString();

            return new FocusDefinitionBuilder(name, guid, original, type, assetReferenceSprite, slotTypes).AddToDB();
        }
    }
}
