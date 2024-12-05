using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static EquipmentDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FactionStatusDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionCharacterPresentations;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ItemDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MerchantDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;

namespace SolastaUnfinishedBusiness.Models;

internal static class ItemCraftingMerchantContext
{
    internal static void Load()
    {
        CraftingContext.Load();
        PickPocketContext.Load();
        LoadCustomIcons();
        LoadRemoveAttunementRequirements();
        SwitchAttuneArcaneShieldstaff();
        SwitchSetBeltOfDwarvenKindBeardChances();
        LoadClothingGorimStock();
        LoadInstrumentsGorimStock();
        SwitchFociItems();
        SwitchFociItemsDungeonMaker();
        SwitchRestockAntiquarian();
        SwitchRestockArcaneum();
        SwitchRestockCircleOfDanantar();
        SwitchRestockTowerOfKnowledge();
        SwitchStackableArtItems();
        SwitchStackableAxesAndDaggers();
        SwitchVersatileInventorySlots();
        LoadDontDisplayHelmets();
    }

    private static void LoadDontDisplayHelmets()
    {
        if (!SettingsContext.GuiModManagerInstance.HideHelmets)
        {
            return;
        }

        foreach (var armor in DatabaseRepository.GetDatabase<ItemDefinition>()
                     .Where(x => x.IsArmor))
        {
            if (armor.ItemPresentation.maleBodyPartBehaviours.Length == GraphicsCharacterDefinitions.BodyPartCount)
            {
                armor.ItemPresentation.maleBodyPartBehaviours[0] = GraphicsCharacterDefinitions.BodyPartBehaviour.Shape;
                armor.ItemPresentation.maleBodyPartBehaviours[1] = GraphicsCharacterDefinitions.BodyPartBehaviour.Shape;
            }

            // ReSharper disable once InvertIf
            if (armor.ItemPresentation.femaleBodyPartBehaviours.Length == GraphicsCharacterDefinitions.BodyPartCount)
            {
                armor.ItemPresentation.femaleBodyPartBehaviours[0] =
                    GraphicsCharacterDefinitions.BodyPartBehaviour.Shape;
                armor.ItemPresentation.femaleBodyPartBehaviours[1] =
                    GraphicsCharacterDefinitions.BodyPartBehaviour.Shape;
            }
        }
    }

    private static void LoadCustomIcons()
    {
        if (!Main.Settings.AddCustomIconsToOfficialItems)
        {
            return;
        }

        Bolt_Alchemy_Corrosive.GuiPresentation.spriteReference =
            Sprites.GetSprite("AcidBolt", Resources.AcidBolt, 120, 125);

        Bolt_Alchemy_Flaming.GuiPresentation.spriteReference =
            Sprites.GetSprite("FlamingBolt", Resources.FlamingBolt, 120, 125);

        Bolt_Alchemy_Flash.GuiPresentation.spriteReference =
            Sprites.GetSprite("RadiantBolt", Resources.RadiantBolt, 120, 125);

        ArtisanToolSmithTools.GuiPresentation.spriteReference =
            Sprites.GetSprite("ArtisanToolSmithTools", Resources.ArtisanToolSmithTools, 118, 120);

        HerbalismKit.GuiPresentation.spriteReference =
            Sprites.GetSprite("RecipeRadiantBolt", Resources.HerbalismKit, 120, 121);
    }

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
                factionStatus = Indifference.Name,
                maxAmount = 4,
                minAmount = 2,
                stackCount = 1,
                reassortAmount = 1,
                reassortRateValue = 1,
                reassortRateType = DurationType.Day
            };

            Store_Merchant_Gorim_Ironsoot_Cyflen_GeneralStore.StockUnitDescriptions.Add(stockClothing);
        }

        //rename valley noble's clothes by color to avoid confusion
        var silverNoble = ClothesNoble_Valley_Silver;
        silverNoble.GuiPresentation.title = "Item/&Armor_Noble_ClothesTitle_Silver";

        var redNoble = ClothesNoble_Valley_Red;
        redNoble.GuiPresentation.title = "Item/&Armor_Noble_ClothesTitle_Red";

        var purpleNoble = ClothesNoble_Valley_Purple;
        purpleNoble.GuiPresentation.title = "Item/&Armor_Noble_ClothesTitle_Purple";

        var pinkNoble = ClothesNoble_Valley_Pink;
        pinkNoble.GuiPresentation.title = "Item/&Armor_Noble_ClothesTitle_Pink";

        var orangeNoble = ClothesNoble_Valley_Orange;
        orangeNoble.GuiPresentation.title = "Item/&Armor_Noble_ClothesTitle_Orange";

        var greenNoble = ClothesNoble_Valley_Green;
        greenNoble.GuiPresentation.title = "Item/&Armor_Noble_ClothesTitle_Green";

        var cherryNoble = ClothesNoble_Valley_Cherry;
        cherryNoble.GuiPresentation.title = "Item/&Armor_Noble_ClothesTitle_Cherry";

        var valleyNoble = ClothesNoble_Valley;
        valleyNoble.GuiPresentation.title = "Item/&Armor_Noble_ClothesTitle_Valley";
    }

    private static void LoadInstrumentsGorimStock()
    {
        if (!Main.Settings.StockGorimStoreWithAllNonMagicalInstruments)
        {
            return;
        }

        foreach (var item in DatabaseRepository.GetDatabase<ItemDefinition>().Where(
                     x => x.IsMusicalInstrument && !x.Magical))
        {
            var stockInstruments = new StockUnitDescription
            {
                itemDefinition = item,
                initialAmount = 2,
                initialized = true,
                factionStatus = Indifference.Name,
                maxAmount = 4,
                minAmount = 2,
                stackCount = 1,
                reassortAmount = 1,
                reassortRateValue = 1,
                reassortRateType = DurationType.Day
            };

            Store_Merchant_Gorim_Ironsoot_Cyflen_GeneralStore.StockUnitDescriptions.Add(stockInstruments);
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

    internal static void SwitchFociItems()
    {
        if (Main.Settings.StockHugoStoreWithAdditionalFoci)
        {
            Store_Merchant_Hugo_Requer_Cyflen_Potions.StockUnitDescriptions.AddRange(
                FocusDefinitionBuilder
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

    internal static void SwitchStackableArtItems()
    {
        foreach (var art in DatabaseRepository.GetDatabase<ItemDefinition>()
                     .Where(x => x.Name.Contains("Art_Item")))
        {
            art.canBeStacked = Main.Settings.EnableStackableArtItems;
        }
    }

    internal static void SwitchStackableAxesAndDaggers()
    {
        foreach (var weapon in DatabaseRepository.GetDatabase<ItemDefinition>()
                     .Where(x =>
                         x.IsWeapon &&
                         (x.WeaponDescription.WeaponTypeDefinition == DaggerType ||
                          x.WeaponDescription.WeaponTypeDefinition == HandaxeType)))
        {
            weapon.canBeStacked = Main.Settings.EnableStackableAxesAndDaggers;
            weapon.stackSize = 5;
            weapon.defaultStackCount = -1;
        }
    }

    internal static void SwitchVersatileInventorySlots()
    {
        foreach (var item in DatabaseRepository.GetDatabase<ItemDefinition>()
                     .Where(a => a.UsableDeviceDescription is { UsableDeviceTags: not null } &&
                                 (a.UsableDeviceDescription.UsableDeviceTags.Contains("Potion") ||
                                  a.UsableDeviceDescription.UsableDeviceTags.Contains("Scroll"))))
        {
            if (Main.Settings.EnableVersatileAmmunitionSlots && Main.Settings.EnableVersatileOffHandSlot)
            {
                item.SlotTypes.SetRange("UtilitySlot",
                    "ContainerSlot",
                    "AmmunitionSlot",
                    "OffHandSlot");
                item.SlotsWhereActive.SetRange("UtilitySlot",
                    "AmmunitionSlot",
                    "OffHandSlot");
            }

            if (Main.Settings.EnableVersatileAmmunitionSlots && !Main.Settings.EnableVersatileOffHandSlot)
            {
                item.SlotTypes.SetRange("UtilitySlot",
                    "ContainerSlot",
                    "AmmunitionSlot");
                item.SlotsWhereActive.SetRange("UtilitySlot",
                    "AmmunitionSlot");
            }

            if (!Main.Settings.EnableVersatileAmmunitionSlots && Main.Settings.EnableVersatileOffHandSlot)
            {
                item.SlotTypes.SetRange("UtilitySlot",
                    "ContainerSlot",
                    "OffHandSlot");
                item.SlotsWhereActive.SetRange("UtilitySlot",
                    "OffHandSlot");
            }

            // ReSharper disable once InvertIf
            if (!Main.Settings.EnableVersatileAmmunitionSlots && !Main.Settings.EnableVersatileOffHandSlot)
            {
                item.SlotTypes.SetRange("UtilitySlot",
                    "ContainerSlot");
                item.SlotsWhereActive.SetRange("UtilitySlot");
            }
        }
    }

    private static void LoadRemoveAttunementRequirements()
    {
        if (!Main.Settings.RemoveAttunementRequirements)
        {
            return;
        }

        foreach (var item in DatabaseRepository.GetDatabase<ItemDefinition>())
        {
            item.requiresAttunement = false;

            foreach (var staticProperty in item.StaticProperties
                         .Where(x => x.KnowledgeAffinity == KnowledgeAffinity.InactiveAndHidden))
            {
                staticProperty.knowledgeAffinity = KnowledgeAffinity.ActiveAndVisible;
            }
        }
    }

    private sealed class FocusDefinitionBuilder : ItemDefinitionBuilder
    {
        internal static readonly HashSet<StockUnitDescription> StockFocus = [];

        internal static readonly ItemDefinition ArcaneStaff = CreateAndAddToDB(
            "CEArcaneStaff",
            Quarterstaff,
            FocusType.Arcane,
            QuarterstaffPlus1.GuiPresentation.SpriteReference);

        internal static readonly ItemDefinition DruidicAmulet = CreateAndAddToDB(
            "CEDruidicAmulet",
            ComponentPouch_ArcaneAmulet,
            FocusType.Druidic,
            BeltOfGiantHillStrength.GuiPresentation.SpriteReference);

        internal static readonly ItemDefinition LivewoodClub = CreateAndAddToDB(
            "CELivewoodClub",
            Club,
            FocusType.Druidic,
            Sprites.GetSprite("LivewoodClub", Resources.LivewoodClub, 128));

        internal static readonly ItemDefinition LivewoodStaff = CreateAndAddToDB(
            "CELivewoodStaff",
            Quarterstaff,
            FocusType.Druidic,
            StaffOfHealing.GuiPresentation.SpriteReference);

        private FocusDefinitionBuilder(
            string name,
            ItemDefinition original,
            FocusType type,
            [CanBeNull] AssetReferenceSprite assetReferenceSprite,
            [NotNull] params string[] slotTypes) : base(original, name, CeNamespaceGuid)
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
                Definition.SlotTypes.Add(SlotTypeContainer);
                Definition.SlotsWhereActive.SetRange(slotTypes);
            }

            var stockFocus = new StockUnitDescription
            {
                itemDefinition = Definition,
                initialAmount = 1,
                initialized = true,
                factionStatus = Indifference.Name,
                maxAmount = 2,
                minAmount = 1,
                stackCount = 1,
                reassortAmount = 1,
                reassortRateValue = 1,
                reassortRateType = DurationType.Day
            };

            StockFocus.Add(stockFocus);
        }

        private static ItemDefinition CreateAndAddToDB(
            string name,
            ItemDefinition original,
            FocusType type,
            AssetReferenceSprite assetReferenceSprite,
            [NotNull] params string[] slotTypes)
        {
            return new FocusDefinitionBuilder(name, original, type, assetReferenceSprite, slotTypes).AddToDB();
        }
    }
}
