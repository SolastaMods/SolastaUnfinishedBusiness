using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static RuleDefinitions.ItemRarity;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Models.ItemPropertyDescriptionsContext;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;

// ReSharper disable InconsistentNaming
namespace SolastaUnfinishedBusiness.Models;

internal static class CustomWeaponsContext
{
    private const string PolearmWeaponTag = "PolearmWeapon";
    private const string CeHandXbowType = "CEHandXbowType";
    internal const string AttackedWithLauncherConditionName = "ConditionLauncherAttackMarker";

    internal static WeaponTypeDefinition
        HandXbowWeaponType,
        LightningLauncherType,
        ThunderGauntletType,
        HalberdWeaponType,
        PikeWeaponType,
        LongMaceWeaponType;

    private static ItemDefinition Katana;

    internal static ItemDefinition LightningLauncher, ProducedFlameDart, ThunderGauntlet;
    private static ItemDefinition HandwrapsOfForce, HandwrapsOfPulling;

    internal static ItemDefinition Halberd, HalberdPrimed, HalberdPlus2;
    private static ItemDefinition HalberdPlus1, HalberdPlus3, HalberdLightning;

    internal static ItemDefinition Pike, PikePrimed, PikePlus2;
    private static ItemDefinition PikePlus1, PikePlus3, PikePsychic;

    internal static ItemDefinition LongMace, LongMacePrimed, LongMacePlus2;
    private static ItemDefinition LongMacePlus1, LongMacePlus3, LongMaceThunder;

    internal static ItemDefinition HandXbow, HandXbowPrimed, HandXbowPlus2;
    private static ItemDefinition HandXbowPlus1, HandXbowPlus3;

    internal static ItemDefinition HandXbowAcid;

    internal static ItemDefinition UnarmedStrikeClaws;

    internal static void Load()
    {
        BuildHandwraps();
        BuildHalberds();
        BuildPikes();
        BuildLongMaces();
        BuildLongswords();
        BuildHandXbow();
        WeaponizeProducedFlame();
        BuildThunderGauntlet();
        BuildLightningLauncher();
        BuildUnarmedStrikeClaws();
        UpdateHandWrapsUseGauntletSlot();
    }

    [NotNull]
    internal static ItemPresentation BuildPresentation(
        string unIdentifiedName,
        ItemPresentation basePresentation,
        AssetReference customPresentation = null,
        float scale = 1.0f, bool hasUnidentifiedDescription = false)
    {
        //TODO: either create a builder for ItemPresentation, or add setter with custom values to ItemDefinitionBuilder
        var presentation = new ItemPresentation(basePresentation)
        {
            assetReference = customPresentation ?? basePresentation.AssetReference
        };

        presentation.ItemFlags.Clear();
        presentation.unidentifiedTitle = GuiPresentationBuilder.CreateTitleKey(unIdentifiedName, Category.Item);
        presentation.unidentifiedDescription = hasUnidentifiedDescription
            ? GuiPresentationBuilder.CreateDescriptionKey(unIdentifiedName, Category.Item)
            : GuiPresentationBuilder.EmptyString;

        // This is necessary for scale to work on the basic version of items (i.e LongMace, Pike) but I don't understand why
        presentation.scaleFactorWhileWielded = scale;

        return presentation;
    }

    [NotNull]
    internal static ItemDefinition BuildWeapon(string name, ItemDefinition baseItem, AssetReference customPresentation,
        int goldCost, bool noDescription,
        ItemRarity rarity,
        ItemPresentation basePresentation = null,
        WeaponDescription baseDescription = null,
        AssetReferenceSprite icon = null,
        bool needId = true,
        float scale = 1.0f,
        bool twoHanded = true,
        params ItemPropertyDescription[] properties)
    {
        basePresentation ??= baseItem.ItemPresentation;
        baseDescription ??= new WeaponDescription(baseItem.WeaponDescription);
        icon ??= baseItem.GuiPresentation.SpriteReference;

        var builder = ItemDefinitionBuilder
            .Create(baseItem, name)
            .SetGold(goldCost)
            .SetMerchantCategory(MerchantCategoryDefinitions.Weapon)
            .SetStaticProperties(properties)
            .SetWeaponDescription(baseDescription)
            .SetItemPresentation(BuildPresentation($"{name}Unidentified", basePresentation, customPresentation, scale))
            .SetItemRarity(rarity);

        if (twoHanded)
        {
            _ = builder
                .SetSlotTypes(SlotTypeDefinitions.MainHandSlot, SlotTypeDefinitions.ContainerSlot)
                .SetSlotsWhereActive(SlotTypeDefinitions.MainHandSlot);
        }
        else
        {
            _ = builder
                .SetSlotTypes(SlotTypeDefinitions.MainHandSlot, SlotTypeDefinitions.OffHandSlot,
                    SlotTypeDefinitions.ContainerSlot)
                .SetSlotsWhereActive(SlotTypeDefinitions.MainHandSlot, SlotTypeDefinitions.OffHandSlot);
        }

        if (properties.Length != 0)
        {
            _ = builder.MakeMagical();

            if (needId)
            {
                _ = builder.SetRequiresIdentification(true);
            }
        }

        _ = noDescription
            ? builder.SetGuiPresentation(Category.Item, Gui.NoLocalization, icon)
            : builder.SetGuiPresentation(Category.Item, icon);

        var weapon = builder.AddToDB();

        weapon.inDungeonEditor = Main.Settings.AddNewWeaponsAndRecipesToEditor;

        return weapon;
    }

    internal static void AddPolearmWeaponTag(ItemDefinition item, Dictionary<string, TagsDefinitions.Criticity> tags)
    {
        if (ValidatorsWeapon.IsWeaponType(item,
                QuarterstaffType, SpearType, HalberdWeaponType, PikeWeaponType, LongMaceWeaponType))
        {
            tags.TryAdd(PolearmWeaponTag, TagsDefinitions.Criticity.Normal);
        }
    }

    internal static ItemDefinition GetStandardWeaponOfType(string type)
    {
        //Darts for some reason are not marked as `Standard`, so return regular Dart for this type 
        if (type == DartType.Name)
        {
            return ItemDefinitions.Dart;
        }

        var allElements = DatabaseRepository.GetDatabase<ItemDefinition>().GetAllElements();

        return allElements
            .FirstOrDefault(item =>
                item.ItemTags.Contains(TagsDefinitions.ItemTagStandard) && item.IsWeapon &&
                item.WeaponDescription.WeaponTypeDefinition.Name == type);
    }

    #region Longswords

    private static void BuildLongswords()
    {
        var baseItem = ItemDefinitions.Longsword;
        var baseDescription = new WeaponDescription(baseItem.WeaponDescription)
        {
            reachRange = 1,
            weaponType = LongswordType.Name,
            weaponTags =
            [
                TagsDefinitions.WeaponTagVersatile
            ]
        };
        var damageForm = baseDescription.EffectDescription
            .GetFirstFormOfType(EffectForm.EffectFormType.Damage).DamageForm;

        damageForm.dieType = DieType.D8;
        damageForm.versatileDieType = DieType.D10;
        damageForm.diceNumber = 1;

        var katanaPrefab = Prefabs.GetKatanaPrefab();

        Katana = BuildWeapon("Katana", baseItem, katanaPrefab, 20, true, Common, null, baseDescription,
            Sprites.KatanaIcon,
            scale: 0.005f
        );

        MerchantContext.AddItem(Katana, ShopItemType.ShopGenericMelee);
    }

    #endregion

    #region Halberds

    private static void BuildHalberds()
    {
        HalberdWeaponType = WeaponTypeDefinitionBuilder
            .Create(GreataxeType, "CEHalberdType")
            .SetGuiPresentation(Category.Item, GuiPresentationBuilder.EmptyString)
            .SetWeaponCategory(WeaponCategoryDefinitions.MartialWeaponCategory)
            .AddCustomSubFeatures(new CustomScale(z: 3.5f))
            .AddToDB();

        var baseItem = ItemDefinitions.Greataxe;
        var basePresentation = ItemDefinitions.Battleaxe.ItemPresentation;
        var baseDescription = new WeaponDescription(baseItem.WeaponDescription)
        {
            reachRange = 2,
            weaponType = HalberdWeaponType.Name,
            weaponTags =
            [
                TagsDefinitions.WeaponTagHeavy,
                TagsDefinitions.WeaponTagReach,
                TagsDefinitions.WeaponTagTwoHanded
            ]
        };
        var damageForm = baseDescription.EffectDescription
            .GetFirstFormOfType(EffectForm.EffectFormType.Damage).DamageForm;

        damageForm.dieType = DieType.D10;
        damageForm.diceNumber = 1;

        Halberd = BuildWeapon("CEHalberd", baseItem, null,
            20, true, Common, basePresentation, baseDescription, Sprites.HalberdIcon);

        MerchantContext.AddItem(Halberd, ShopItemType.ShopGenericMelee);

        HalberdPrimed = BuildWeapon("CEHalberdPrimed", baseItem, null,
            40, true, Uncommon, basePresentation, baseDescription, Sprites.HalberdPrimedIcon);
        HalberdPrimed.ItemTags.Add(TagsDefinitions.ItemTagIngredient);
        HalberdPrimed.ItemTags.Remove(TagsDefinitions.ItemTagStandard);

        MerchantContext.AddItem(HalberdPrimed, ShopItemType.ShopPrimedMelee);
        MerchantContext.AddItem(RecipeHelper.BuildPrimeManual(Halberd, HalberdPrimed), ShopItemType.ShopCrafting);

        HalberdPlus1 = BuildWeapon("CEHalberd+1", Halberd, null,
            1050, true, Rare, icon: Sprites.HalberdP1Icon, properties: [WeaponPlus1]);

        MerchantContext.AddItem(HalberdPlus1, ShopItemType.ShopMeleePlus1);
        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(HalberdPlus1, 12, 14,
            HalberdPrimed,
            ItemDefinitions.Ingredient_Enchant_Oil_Of_Acuteness), ShopItemType.ShopCrafting);

        var itemDefinition = ItemDefinitions.BattleaxePlus2;

        HalberdPlus2 = BuildWeapon("CEHalberd+2", Halberd, null,
            4000, true, VeryRare,
            itemDefinition.ItemPresentation, icon: Sprites.HalberdP2Icon,
            properties: [WeaponPlus2]);

        MerchantContext.AddItem(HalberdPlus2, ShopItemType.ShopMeleePlus2);
        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(HalberdPlus2, 14, 18,
            HalberdPrimed,
            ItemDefinitions.Ingredient_Enchant_Blood_Gem), ShopItemType.ShopCrafting);

        itemDefinition = ItemDefinitions.BattleaxePlus3;

        HalberdPlus3 = BuildWeapon("CEHalberd+3", Halberd, null,
            16000, true, VeryRare,
            itemDefinition.ItemPresentation, icon: Sprites.HalberdP3Icon,
            properties: [WeaponPlus3]);

        MerchantContext.AddItem(HalberdPlus3, ShopItemType.ShopMeleePlus3);
        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(HalberdPlus3, 16, 22,
            HalberdPrimed,
            ItemDefinitions.Ingredient_Enchant_Blood_Gem,
            ItemDefinitions.Ingredient_Enchant_Blood_Gem), ShopItemType.ShopCrafting);

        HalberdLightning = BuildWeapon("CEHalberdLightning", Halberd, null,
            4000, true, VeryRare,
            itemDefinition.ItemPresentation, icon: Sprites.HalberdLightningIcon, needId: false,
            properties: [LightningImpactVFX, WeaponPlus1AttackOnly]);

        HalberdLightning.WeaponDescription.EffectDescription.effectForms.Add(
            EffectFormBuilder
                .Create()
                .SetDamageForm(DamageTypeLightning, 1, DieType.D8)
                .SetCreatedBy(false, false)
                .Build());
        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(HalberdLightning, 14, 18,
            HalberdPrimed,
            ItemDefinitions.Ingredient_Enchant_Stardust), ShopItemType.ShopCrafting);
    }

    #endregion

    #region Pikes

    private static void BuildPikes()
    {
        PikeWeaponType = WeaponTypeDefinitionBuilder
            .Create(SpearType, "CEPikeType")
            .SetGuiPresentation(Category.Item, GuiPresentationBuilder.EmptyString)
            .SetWeaponCategory(WeaponCategoryDefinitions.MartialWeaponCategory)
            //.AddCustomSubFeatures(new CustomScale(z: 3.5f))
            .AddToDB();

        var baseItem = ItemDefinitions.Spear;
        var baseDescription = new WeaponDescription(baseItem.WeaponDescription)
        {
            reachRange = 2,
            weaponType = PikeWeaponType.Name,
            weaponTags =
            [
                TagsDefinitions.WeaponTagHeavy,
                TagsDefinitions.WeaponTagReach,
                TagsDefinitions.WeaponTagTwoHanded
            ]
        };
        var damageForm = baseDescription.EffectDescription
            .GetFirstFormOfType(EffectForm.EffectFormType.Damage).DamageForm;

        damageForm.dieType = DieType.D10;
        damageForm.diceNumber = 1;

        var pikePrefab = Prefabs.GetPikePrefab();

        Pike = BuildWeapon("CEPike", baseItem, pikePrefab, 20, true, Common, null, baseDescription,
            Sprites.PikeIcon
        );

        Pike.ItemTags.Remove(TagsDefinitions.ItemTagMonk);
        MerchantContext.AddItem(Pike, ShopItemType.ShopGenericMelee);

        PikePrimed = BuildWeapon("CEPikePrimed", baseItem, pikePrefab, 40, true, Uncommon, null, baseDescription,
            Sprites.PikePrimedIcon);
        PikePrimed.ItemTags.Add(TagsDefinitions.ItemTagIngredient);
        PikePrimed.ItemTags.Remove(TagsDefinitions.ItemTagStandard);
        PikePrimed.ItemTags.Remove(TagsDefinitions.ItemTagMonk);

        MerchantContext.AddItem(PikePrimed, ShopItemType.ShopPrimedMelee);
        MerchantContext.AddItem(RecipeHelper.BuildPrimeManual(Pike, PikePrimed), ShopItemType.ShopCrafting);

        PikePlus1 = BuildWeapon("CEPike+1", Pike, pikePrefab, 1050, true, Rare,
            icon: Sprites.PikeP1Icon,
            properties: [WeaponPlus1]
        );

        PikePlus1.ItemTags.Remove(TagsDefinitions.ItemTagMonk);
        MerchantContext.AddItem(PikePlus1, ShopItemType.ShopMeleePlus1);
        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(PikePlus1, 12, 14,
            PikePrimed,
            ItemDefinitions.Ingredient_Enchant_Oil_Of_Acuteness), ShopItemType.ShopCrafting);

        PikePlus2 = BuildWeapon("CEPike+2", Pike, pikePrefab, 4000, true, VeryRare,
            icon: Sprites.PikeP2Icon,
            properties: [WeaponPlus2]
        );

        PikePlus2.ItemTags.Remove(TagsDefinitions.ItemTagMonk);
        MerchantContext.AddItem(PikePlus2, ShopItemType.ShopMeleePlus2);
        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(PikePlus2, 14, 18,
            PikePrimed,
            ItemDefinitions.Ingredient_Enchant_Blood_Gem), ShopItemType.ShopCrafting);

        PikePlus3 = BuildWeapon("CEPike+3", Pike, pikePrefab, 16000, true, VeryRare,
            icon: Sprites.PikeP3Icon,
            properties: [WeaponPlus3]
        );

        PikePlus3.ItemTags.Remove(TagsDefinitions.ItemTagMonk);
        MerchantContext.AddItem(PikePlus3, ShopItemType.ShopMeleePlus3);
        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(PikePlus3, 16, 22,
            PikePrimed,
            ItemDefinitions.Ingredient_Enchant_Blood_Gem,
            ItemDefinitions.Ingredient_Enchant_Blood_Gem), ShopItemType.ShopCrafting);

        PikePsychic = BuildWeapon("CEPikePsychic", Pike, pikePrefab, 4000, true, VeryRare,
            icon: Sprites.PikePsychicIcon,
            needId: false,
            properties: [PsychicImpactVFX, WeaponPlus1AttackOnly]
        );

        PikePsychic.ItemTags.Remove(TagsDefinitions.ItemTagMonk);
        PikePsychic.WeaponDescription.EffectDescription.effectForms.Add(
            EffectFormBuilder
                .Create()
                .SetDamageForm(DamageTypePsychic, 1, DieType.D8)
                .SetCreatedBy(false, false)
                .Build());
        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(PikePsychic, 14, 18,
            PikePrimed,
            ItemDefinitions.Ingredient_Enchant_Stardust), ShopItemType.ShopCrafting);
    }

    #endregion

    #region Long Maces

    private static void BuildLongMaces()
    {
        LongMaceWeaponType = WeaponTypeDefinitionBuilder
            .Create(MaulType, "CELongMaceType")
            .SetGuiPresentation(Category.Item, GuiPresentationBuilder.EmptyString)
            .SetWeaponCategory(WeaponCategoryDefinitions.MartialWeaponCategory)
            .AddToDB();

        var longmacePrefab = Prefabs.GetLongMacePrefab();

        var baseItem = ItemDefinitions.Warhammer;
        var baseDescription = new WeaponDescription(baseItem.WeaponDescription)
        {
            reachRange = 2,
            weaponType = LongMaceWeaponType.Name,
            weaponTags =
            [
                TagsDefinitions.WeaponTagHeavy,
                TagsDefinitions.WeaponTagReach,
                TagsDefinitions.WeaponTagTwoHanded
            ]
        };
        var damageForm = baseDescription.EffectDescription
            .GetFirstFormOfType(EffectForm.EffectFormType.Damage).DamageForm;

        damageForm.dieType = DieType.D10;
        damageForm.diceNumber = 1;

        LongMace = BuildWeapon("CELongMace", baseItem, longmacePrefab, 20, true, Common, null, baseDescription,
            Sprites.LongMaceIcon
        );

        MerchantContext.AddItem(LongMace, ShopItemType.ShopGenericMelee);

        LongMacePrimed = BuildWeapon("CELongMacePrimed", baseItem, longmacePrefab, 40, true, Uncommon, null,
            baseDescription, Sprites.LongMacePrimedIcon);
        LongMacePrimed.ItemTags.Add(TagsDefinitions.ItemTagIngredient);
        LongMacePrimed.ItemTags.Remove(TagsDefinitions.ItemTagStandard);

        MerchantContext.AddItem(LongMacePrimed, ShopItemType.ShopPrimedMelee);
        MerchantContext.AddItem(RecipeHelper.BuildPrimeManual(LongMace, LongMacePrimed), ShopItemType.ShopCrafting);

        LongMacePlus1 = BuildWeapon("CELongMace+1", LongMace, longmacePrefab, 1050, true, Rare,
            icon: Sprites.LongMaceP1Icon,
            properties: [WeaponPlus1]
        );

        MerchantContext.AddItem(LongMacePlus1, ShopItemType.ShopMeleePlus1);
        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(LongMacePlus1, 12, 14,
            LongMacePrimed,
            ItemDefinitions.Ingredient_Enchant_Oil_Of_Acuteness), ShopItemType.ShopCrafting);

        LongMacePlus2 = BuildWeapon("CELongMace+2", LongMace, longmacePrefab, 4000, true, VeryRare,
            icon: Sprites.LongMaceP2Icon,
            properties: [WeaponPlus2]
        );

        MerchantContext.AddItem(LongMacePlus2, ShopItemType.ShopMeleePlus2);
        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(LongMacePlus2, 14, 18,
            LongMacePrimed,
            ItemDefinitions.Ingredient_Enchant_Blood_Gem), ShopItemType.ShopCrafting);

        LongMacePlus3 = BuildWeapon("CELongMace+3", LongMace, longmacePrefab, 16000, true, VeryRare,
            icon: Sprites.LongMaceP3Icon,
            properties: [WeaponPlus3]
        );

        MerchantContext.AddItem(LongMacePlus3, ShopItemType.ShopMeleePlus3);
        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(LongMacePlus3, 16, 22,
            LongMacePrimed,
            ItemDefinitions.Ingredient_Enchant_Blood_Gem,
            ItemDefinitions.Ingredient_Enchant_Blood_Gem), ShopItemType.ShopCrafting);

        LongMaceThunder = BuildWeapon("CELongMaceThunder", LongMace, longmacePrefab, 4000, true, VeryRare,
            icon: Sprites.LongMaceThunderIcon,
            needId: false,
            properties: [ThunderImpactVFX, WeaponPlus1AttackOnly]
        );

        LongMaceThunder.WeaponDescription.EffectDescription.effectForms.Add(
            EffectFormBuilder
                .Create()
                .SetDamageForm(DamageTypeThunder, 1, DieType.D8)
                .SetCreatedBy(false, false)
                .Build());
        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(LongMaceThunder, 14, 18,
            LongMacePrimed,
            ItemDefinitions.Ingredient_Enchant_Stardust), ShopItemType.ShopCrafting);
    }

    #endregion

    #region Hand Crossbows

    private static void BuildHandXbow()
    {
        HandXbowWeaponType = WeaponTypeDefinitionBuilder
            .Create(LightCrossbowType, CeHandXbowType)
            .SetGuiPresentation(Category.Item, GuiPresentationBuilder.EmptyString)
            .SetWeaponCategory(WeaponCategoryDefinitions.MartialWeaponCategory)
            .AddCustomSubFeatures(new CustomScale(0.5f))
            .SetAnimationTag("Rapier")
            .AddToDB();

        var baseItem = ItemDefinitions.LightCrossbow;
        var basePresentation = new ItemPresentation(baseItem.ItemPresentation);
        var baseDescription = new WeaponDescription(baseItem.WeaponDescription)
        {
            weaponType = HandXbowWeaponType.Name,
            closeRange = 6,
            maxRange = 24,
            weaponTags =
            [
                TagsDefinitions.WeaponTagLight,
                TagsDefinitions.WeaponTagRange,
                TagsDefinitions.WeaponTagLoading,
                TagsDefinitions.WeaponTagAmmunition
            ]
        };
        var damageForm = baseDescription.EffectDescription
            .GetFirstFormOfType(EffectForm.EffectFormType.Damage).DamageForm;

        damageForm.dieType = DieType.D6;
        damageForm.diceNumber = 1;

        //add hand xbow proficiency to rogues
        var rogueHandXbowProficiency = FeatureDefinitionProficiencys.ProficiencyRogueWeapon;

        rogueHandXbowProficiency.Proficiencies.Add(HandXbowWeaponType.Name);

        HandXbow = BuildWeapon("CEHandXbow", baseItem, null,
            20, true, Common, basePresentation, baseDescription, Sprites.HandXbowIcon,
            twoHanded: false);

        MerchantContext.AddItem(HandXbow, ShopItemType.ShopGenericRanged);

        HandXbowPrimed = BuildWeapon("CEHandXbowPrimed", HandXbow, null,
            40, true, Uncommon, icon: Sprites.HandXbowPrimedIcon, twoHanded: false);

        HandXbowPrimed.ItemTags.Add(TagsDefinitions.ItemTagIngredient);
        HandXbowPrimed.ItemTags.Remove(TagsDefinitions.ItemTagStandard);
        MerchantContext.AddItem(HandXbowPrimed, ShopItemType.ShopPrimedRanged);
        MerchantContext.AddItem(RecipeHelper.BuildPrimeManual(HandXbow, HandXbowPrimed), ShopItemType.ShopCrafting);

        HandXbowPlus1 = BuildWeapon("CEHandXbow+1", HandXbow, null,
            1050, true, Rare, icon: Sprites.HandXbowP1Icon, twoHanded: false,
            properties: [WeaponPlus1]);

        MerchantContext.AddItem(HandXbowPlus1, ShopItemType.ShopRangedPlus1);
        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(HandXbowPlus1, 12, 14,
            HandXbowPrimed,
            ItemDefinitions.Ingredient_Enchant_Oil_Of_Acuteness), ShopItemType.ShopCrafting);

        var itemDefinition = ItemDefinitions.LightCrossbowPlus2;

        HandXbowPlus2 = BuildWeapon("CEHandXbow+2", HandXbow, null,
            4000, true, VeryRare,
            itemDefinition.ItemPresentation, icon: Sprites.HandXbowP2Icon, twoHanded: false,
            properties: [WeaponPlus2]);

        MerchantContext.AddItem(HandXbowPlus2, ShopItemType.ShopRangedPlus2);
        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(HandXbowPlus2, 14, 18,
            HandXbowPrimed,
            ItemDefinitions.Ingredient_Enchant_Blood_Gem), ShopItemType.ShopCrafting);

        HandXbowPlus3 = BuildWeapon("CEHandXbow+3", HandXbow, null,
            16000, true, VeryRare,
            itemDefinition.ItemPresentation, icon: Sprites.HandXbowP3Icon, twoHanded: false,
            properties: [WeaponPlus3]);

        MerchantContext.AddItem(HandXbowPlus3, ShopItemType.ShopRangedPlus3);
        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(HandXbowPlus3, 16, 22,
            HandXbowPrimed,
            ItemDefinitions.Ingredient_Enchant_Blood_Gem,
            ItemDefinitions.Ingredient_Enchant_Blood_Gem), ShopItemType.ShopCrafting);

        HandXbowAcid = BuildWeapon("CEHandXbowAcid", HandXbow, null,
            4000, true, VeryRare,
            itemDefinition.ItemPresentation, icon: Sprites.HandXbowAcidIcon, needId: false, twoHanded: false,
            properties: [AcidImpactVFX, WeaponPlus1AttackOnly]);

        HandXbowAcid.WeaponDescription.EffectDescription.effectForms.Add(
            EffectFormBuilder
                .Create()
                .SetDamageForm(DamageTypeAcid, 1, DieType.D8)
                .SetCreatedBy(false, false)
                .Build());
        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(HandXbowAcid, 14, 18,
            HandXbowPrimed,
            ItemDefinitions.Ingredient_Enchant_Stardust), ShopItemType.ShopCrafting);
    }

    #endregion

    #region Thunder Guantlet

    private static void BuildThunderGauntlet()
    {
        ThunderGauntletType = WeaponTypeDefinitionBuilder
            .Create(UnarmedStrikeType, "CEThunderGauntletType")
            .SetGuiPresentation("Item/&CEThunderGauntletTitle", GuiPresentationBuilder.EmptyString)
            .SetWeaponCategory(WeaponCategoryDefinitions.SimpleWeaponCategory)
            .AddToDB();

        var thunderGauntletPrefab = Prefabs.GetThunderGauntletPrefab();
        var baseItem = ItemDefinitions.UnarmedStrikeBase;
        var baseDescription = new WeaponDescription(baseItem.WeaponDescription)
        {
            reachRange = 1, weaponType = ThunderGauntletType.Name, weaponTags = []
        };
        var damageForm = baseDescription.EffectDescription
            .GetFirstFormOfType(EffectForm.EffectFormType.Damage).DamageForm;

        damageForm.dieType = DieType.D8;
        damageForm.diceNumber = 1;
        damageForm.damageType = DamageTypeThunder;

        baseDescription.EffectDescription.DurationType = DurationType.Round;
        baseDescription.EffectDescription.DurationParameter = 1;
        baseDescription.EffectDescription.EndOfEffect = (TurnOccurenceType)ExtraTurnOccurenceType.StartOfSourceTurn;
        baseDescription.EffectDescription.EffectForms.Add(
            EffectFormBuilder.ConditionForm(CustomConditionsContext.Taunted));

        var effectParticleParameters = new EffectParticleParameters();

        effectParticleParameters.Copy(SpellDefinitions.Shatter.EffectDescription.EffectParticleParameters);
        effectParticleParameters.casterParticleReference = new AssetReference();
        baseDescription.EffectDescription.effectParticleParameters = effectParticleParameters;

        ThunderGauntlet = BuildWeapon("CEThunderGauntlet", baseItem, thunderGauntletPrefab, 0, true, Common, null,
            baseDescription,
            Sprites.GetSprite("ItemThunderGauntlet", Resources.ItemThunderGauntlet, 128),
            scale: 0.25f,
            properties: [ThunderImpactVFX]
        );
        ThunderGauntlet.inDungeonEditor = false;
    }

    #endregion

    #region Lightning Launcher

    private static void BuildLightningLauncher()
    {
        LightningLauncherType = WeaponTypeDefinitionBuilder
            .Create(ShortbowType, "CELightningLauncherType")
            .SetGuiPresentation("Item/&CELightningLauncherTitle", GuiPresentationBuilder.EmptyString)
            .SetWeaponCategory(WeaponCategoryDefinitions.SimpleWeaponCategory)
            .SetAnimationTag("Rapier")
            .AddToDB();

        var launcherPrefab = Prefabs.GetLauncherPrefab();
        var baseItem = ItemDefinitions.Shortbow;
        var baseDescription = new WeaponDescription(baseItem.WeaponDescription)
        {
            //TODO: add custom ammunition that looks like lightning
            closeRange = 18, maxRange = 60, weaponType = LightningLauncherType.Name, weaponTags = []
        };
        var damageForm = baseDescription.EffectDescription
            .GetFirstFormOfType(EffectForm.EffectFormType.Damage).DamageForm;

        damageForm.dieType = DieType.D6;
        damageForm.diceNumber = 1;
        damageForm.damageType = DamageTypeLightning;

        baseDescription.EffectDescription.EffectForms.Add(
            EffectFormBuilder
                .Create()
                .SetConditionForm(
                    ConditionDefinitionBuilder
                        .Create(AttackedWithLauncherConditionName)
                        .SetGuiPresentationNoContent(true)
                        .SetSilent(Silent.WhenAddedOrRemoved)
                        .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                        .AddToDB(), ConditionForm.ConditionOperation.Add, true)
                .Build());

        baseDescription.EffectDescription.effectParticleParameters =
            SpellDefinitions.LightningBolt.EffectDescription.EffectParticleParameters;

        LightningLauncher = BuildWeapon("CELightningLauncher", baseItem, launcherPrefab, 0, true, Common, null,
            baseDescription,
            Sprites.GetSprite("ItemGemLightning", Resources.ItemGemLightning, 128),
            properties: [LightningImpactVFX],
            scale: 0.25f
        );
        LightningLauncher.inDungeonEditor = false;
    }

    #endregion

    #region Natural Weapons

    private static void BuildUnarmedStrikeClaws()
    {
        var baseItem = ItemDefinitions.UnarmedStrikeBase;
        var basePresentation = baseItem.ItemPresentation;
        var baseDescription = new WeaponDescription(baseItem.WeaponDescription);
        var damageForm = baseDescription.EffectDescription
            .GetFirstFormOfType(EffectForm.EffectFormType.Damage).DamageForm;

        damageForm.dieType = DieType.D6;
        damageForm.diceNumber = 1;
        damageForm.damageType = DamageTypeSlashing;

        UnarmedStrikeClaws = BuildWeapon("CEUnarmedStrikeClaws", baseItem, null, 0, true, Common,
            basePresentation, baseDescription,
            Sprites.GetSprite("UnarmedStrikeClaws", Resources.UnarmedStrikeClaws, 128));
    }

    #endregion

    #region Handwraps

    private static void BuildHandwraps()
    {
        HandwrapsOfForce = BuildHandwrapsCommon("HandwrapsOfForce", 2000, true, false, Rare, ForceImpactVFX,
            WeaponPlus1AttackOnly);
        HandwrapsOfForce.WeaponDescription.EffectDescription.effectForms.Add(
            EffectFormBuilder
                .Create()
                .SetDamageForm(DamageTypeForce, 1, DieType.D4)
                .SetCreatedBy(false, false)
                .Build());

        HandwrapsOfPulling = BuildHandwrapsCommon("HandwrapsOfPulling", 2000, true, false, Rare, WeaponPlus1AttackOnly);
        HandwrapsOfPulling.IsUsableDevice = true;
        HandwrapsOfPulling.usableDeviceDescription = new UsableDeviceDescriptionBuilder()
            .SetRecharge(RechargeRate.AtWill)
            .SetSaveDc(EffectHelpers.BasedOnUser)
            .AddFunctions(new DeviceFunctionDescriptionBuilder()
                .SetPower(
                    FeatureDefinitionPowerBuilder
                        .Create("PowerHandwrapsOfPulling")
                        .SetGuiPresentation(Category.Feature)
                        .SetUsesFixed(ActivationTime.BonusAction)
                        .SetEffectDescription(
                            EffectDescriptionBuilder
                                .Create()
                                .SetTargetingData(Side.All, RangeType.Distance, 3, TargetType.IndividualsUnique)
                                .ExcludeCaster()
                                .SetSavingThrowData(
                                    true,
                                    AttributeDefinitions.Strength,
                                    false,
                                    EffectDifficultyClassComputation.AbilityScoreAndProficiency)
                                .SetParticleEffectParameters(FeatureDefinitionPowers.PowerShadowTamerRopeGrapple)
                                .SetEffectForms(
                                    EffectFormBuilder
                                        .Create()
                                        .SetMotionForm(MotionForm.MotionType.DragToOrigin, 2)
                                        .SetCreatedBy(false, false)
                                        .Build())
                                .Build())
                        .AddToDB())
                .Build())
            .Build();

        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(HandwrapsOfForce, 14, 18,
            ItemDefinitions.Ingredient_Enchant_Soul_Gem, ItemDefinitions.Primed_Gauntlet), ShopItemType.ShopCrafting);

        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(HandwrapsOfPulling, 14, 18,
            ItemDefinitions.Ingredient_Enchant_Stardust, ItemDefinitions.Primed_Gauntlet), ShopItemType.ShopCrafting);
    }

    [NotNull]
    private static ItemDefinition BuildHandwrapsCommon(string name, int goldCost, bool noDescription, bool needId,
        ItemRarity rarity,
        params ItemPropertyDescription[] properties)
    {
        return BuildWeapon(
            name,
            ItemDefinitions.Primed_Gauntlet,
            null,
            goldCost,
            noDescription, rarity, needId: needId,
            properties: properties
        );
    }

    private static void UpdateHandWrapsUseGauntletSlot()
    {
        foreach (var item in DatabaseRepository.GetDatabase<ItemDefinition>())
        {
            if (item is not { WeaponDescription.weaponType: EquipmentDefinitions.WeaponTypeUnarmedStrike })
            {
                continue;
            }

            if (item == ItemDefinitions.UnarmedStrikeBase) { continue; }

            if (Main.Settings.EnableMonkHandwrapsUseGauntletSlot)
            {
                item.SlotTypes.Add(EquipmentDefinitions.SlotTypeGloves);
                item.SlotsWhereActive.Add(EquipmentDefinitions.SlotTypeGloves);
            }
            else
            {
                item.SlotTypes.Remove(EquipmentDefinitions.SlotTypeGloves);
                item.SlotsWhereActive.Remove(EquipmentDefinitions.SlotTypeGloves);
            }
        }
    }

    internal static void ModifyUnarmedAttackWithGauntlet(RulesetCharacterHero hero, ref ItemDefinition itemDefinition,
        ref WeaponDescription weaponDescription, ref RulesetItem weapon)
    {
        if (!Main.Settings.EnableMonkHandwrapsUseGauntletSlot
            || weapon != null || itemDefinition != ItemDefinitions.UnarmedStrikeBase)
        {
            return;
        }

        var item = hero.CharacterInventory.InventorySlotsByType[EquipmentDefinitions.SlotTypeGloves][0]?.EquipedItem;

        if (item is not { ItemDefinition.WeaponDescription.WeaponType: EquipmentDefinitions.WeaponTypeUnarmedStrike })
        {
            return;
        }

        itemDefinition = item.ItemDefinition;
        weaponDescription = itemDefinition.WeaponDescription;
        weapon = item;
    }


    //TODO: not sure this is the best place for this method
    internal static void TryAddMainActionUnarmedAttacks(RulesetCharacterHero hero)
    {
        if (!Main.Settings.EnableUnarmedMainAttackAction)
        {
            return;
        }

        //skip if we already have main unarmed attack
        if (hero.AttackModes.Any(m => m is
                {
                    ActionType: ActionDefinitions.ActionType.Main,
                    SourceDefinition: ItemDefinition
                    {
                        WeaponDescription.WeaponType: EquipmentDefinitions.WeaponTypeUnarmedStrike
                    }
                }
            ))
        {
            return;
        }

        //checkin only extra glove slot, because gauntlets in main hand slot would already add attack
        var item = hero.CharacterInventory.InventorySlotsByType[EquipmentDefinitions.SlotTypeGloves][0]?.EquipedItem;
        var definition = item?.ItemDefinition;

        if (item is not { ItemDefinition.WeaponDescription.WeaponType: EquipmentDefinitions.WeaponTypeUnarmedStrike })
        {
            item = null;
            definition = null;
        }

        definition ??= ItemDefinitions.UnarmedStrikeBase;

        if (hero.GetClassLevel(CharacterClassDefinitions.Monk) == 0 && item == null)
        {
            return;
        }

        hero.AttackModes.Add(hero.RefreshAttackMode(ActionDefinitions.ActionType.Main, definition,
            definition.WeaponDescription, hero.HasFreeHandSlot(), true, EquipmentDefinitions.SlotTypeMainHand,
            hero.attackModifiers, hero.FeaturesOrigin, item));
    }

    #endregion

    #region Produce Flame

    private static void WeaponizeProducedFlame()
    {
        var flame = ItemDefinitions.ProducedFlame;

        flame.GuiPresentation = new GuiPresentationBuilder(flame.GuiPresentation)
            .SetTitle("Item/&CEProducedFlameTitle")
            .Build();

        ProducedFlameDart = BuildWeapon("CEProducedFlameDart", ItemDefinitions.Dart, null, 0, true, Common,
            flame.ItemPresentation, icon: Sprites.ProducedFlameThrow);
        ProducedFlameDart.inDungeonEditor = false;

        var damageForm = ProducedFlameDart.WeaponDescription.EffectDescription.FindFirstDamageForm();

        damageForm.damageType = DamageTypeFire;
        damageForm.dieType = DieType.D8;

        var weapon = new WeaponDescription(ItemDefinitions.UnarmedStrikeBase.weaponDefinition);

        weapon.EffectDescription.effectForms.Add(
            EffectFormBuilder
                .Create()
                .SetDamageForm(DamageTypeFire, 1, DieType.D8)
                .SetCreatedBy(false, false)
                .Build());
        flame.staticProperties.Add(
            BuildFrom(
                FeatureDefinitionBuilder
                    .Create("FeatureProducedFlameThrower")
                    .SetGuiPresentationNoContent()
                    .AddCustomSubFeatures(
                        new ModifyWeaponProducedFlameDice(),
                        new AddThrowProducedFlameAttack())
                    .AddToDB(), false));

        flame.IsWeapon = true;
        flame.weaponDefinition = weapon;
    }

    internal static void ProcessProducedFlameAttack([NotNull] RulesetCharacterHero hero,
        [NotNull] RulesetAttackMode mode)
    {
        var num = hero.characterInventory.CurrentConfiguration;
        var configurations = hero.characterInventory.WieldedItemsConfigurations;

        if (num == configurations.Count - 1)
        {
            num = configurations[num].MainHandSlot.ShadowedSlot != configurations[0].MainHandSlot ? 1 : 0;
        }

        var itemsConfiguration = configurations[num];
        RulesetItem item = null;

        if (mode.SlotName == EquipmentDefinitions.SlotTypeMainHand)
        {
            item = itemsConfiguration.MainHandSlot.EquipedItem;
        }
        else if (mode.SlotName == EquipmentDefinitions.SlotTypeOffHand)
        {
            item = itemsConfiguration.OffHandSlot.EquipedItem;
        }

        if (item == null || item.ItemDefinition != ItemDefinitions.ProducedFlame)
        {
            return;
        }

        hero.CharacterInventory.DefineWieldedItemsConfiguration(num, null, mode.SlotName);
    }

    #endregion
}

internal sealed class ModifyWeaponProducedFlameDice : ModifyWeaponAttackModeBase
{
    internal ModifyWeaponProducedFlameDice() : base((_, weapon, _) =>
        weapon != null && weapon.ItemDefinition == ItemDefinitions.ProducedFlame)
    {
    }

    protected override void TryModifyAttackMode(
        RulesetCharacter character,
        RulesetAttackMode attackMode)
    {
        var damage = attackMode.EffectDescription.FindFirstDamageForm();

        if (damage == null)
        {
            return;
        }

        var casterLevel = character.TryGetAttributeValue(AttributeDefinitions.CharacterLevel);

        damage.diceNumber = 1 + SpellAdvancementByCasterLevel[casterLevel - 1];
    }
}

internal sealed class AddThrowProducedFlameAttack : AddExtraAttackBase
{
    internal AddThrowProducedFlameAttack() : base(ActionDefinitions.ActionType.Main)
    {
    }

    protected override List<RulesetAttackMode> GetAttackModes([NotNull] RulesetCharacter character)
    {
        if (character is not RulesetCharacterHero hero)
        {
            return null;
        }

        var result = new List<RulesetAttackMode>();

        AddItemAttack(result, EquipmentDefinitions.SlotTypeMainHand, hero);
        AddItemAttack(result, EquipmentDefinitions.SlotTypeOffHand, hero);

        return result;
    }

    // ReSharper disable once SuggestBaseTypeForParameter
    private static void AddItemAttack(
        List<RulesetAttackMode> attackModes,
        [NotNull] string slot,
        [NotNull] RulesetCharacterHero hero)
    {
        var item = hero.CharacterInventory.InventorySlotsByName[slot].EquipedItem;

        if (item == null || item.ItemDefinition != ItemDefinitions.ProducedFlame)
        {
            return;
        }

        var strikeDefinition = CustomWeaponsContext.ProducedFlameDart;

        var action = slot == EquipmentDefinitions.SlotTypeOffHand
            ? ActionDefinitions.ActionType.Bonus
            : ActionDefinitions.ActionType.Main;

        var attackMode = hero.RefreshAttackMode(
            action,
            strikeDefinition,
            strikeDefinition.WeaponDescription,
            false,
            false,
            slot,
            hero.attackModifiers,
            hero.FeaturesOrigin,
            item
        );

        attackMode.closeRange = attackMode.maxRange = 6;
        attackMode.Reach = false;
        attackMode.Ranged = true;
        attackMode.Thrown = true;
        attackMode.AttackTags.Remove(TagsDefinitions.WeaponTagMelee);

        attackModes.Add(attackMode);
    }
}

internal sealed class CustomScale
{
    internal readonly float X, Y, Z;

    internal CustomScale(float s) : this(s, s, s)
    {
    }

    internal CustomScale(float x = 1f, float y = 1f, float z = 1f)
    {
        X = x;
        Y = y;
        Z = z;
    }
}

//using System.Collections.Generic;
//using System.Linq;
//using JetBrains.Annotations;
//using SolastaUnfinishedBusiness.Api.GameExtensions;
//using SolastaUnfinishedBusiness.Api.Helpers;
//using SolastaUnfinishedBusiness.Behaviors;
//using SolastaUnfinishedBusiness.Builders;
//using SolastaUnfinishedBusiness.Builders.Features;
//using SolastaUnfinishedBusiness.CustomUI;
//using SolastaUnfinishedBusiness.Properties;
//using SolastaUnfinishedBusiness.Validators;
//using UnityEngine;
//using UnityEngine.AddressableAssets;
//using UnityEngine.ResourceManagement;
//using static RuleDefinitions;
//using static RuleDefinitions.ItemRarity;
//using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
//using static SolastaUnfinishedBusiness.Models.ItemPropertyDescriptionsContext;
//using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;
//using UnityEngine.InputSystem;
//using UnityEngine.AddressableAssets.ResourceLocators;
//using UnityEngine.ResourceManagement.ResourceLocations;
//using System;
//using System.Threading.Tasks;

//// ReSharper disable InconsistentNaming
//namespace SolastaUnfinishedBusiness.Models;

//internal static class CustomWeaponsContext
//{
//    private const string PolearmWeaponTag = "PolearmWeapon";
//    private const string CeHandXbowType = "CEHandXbowType";
//    internal const string AttackedWithLauncherConditionName = "ConditionLauncherAttackMarker";

//    internal static WeaponTypeDefinition
//        HandXbowWeaponType,
//        LightningLauncherType,
//        ThunderGauntletType,
//        HalberdWeaponType,
//        PikeWeaponType,
//        LongMaceWeaponType;

//    internal static ItemDefinition Katana;

//    internal static ItemDefinition LightningLauncher, ProducedFlameDart, ThunderGauntlet;
//    private static ItemDefinition HandwrapsOfForce, HandwrapsOfPulling;

//    internal static ItemDefinition Halberd, HalberdPrimed, HalberdPlus2;
//    private static ItemDefinition HalberdPlus1, HalberdPlus3, HalberdLightning;

//    internal static ItemDefinition Pike, PikePrimed, PikePlus2;
//    private static ItemDefinition PikePlus1, PikePlus3, PikePsychic;

//    internal static ItemDefinition LongMace, LongMacePrimed, LongMacePlus2;
//    private static ItemDefinition LongMacePlus1, LongMacePlus3, LongMaceThunder;

//    internal static ItemDefinition HandXbow, HandXbowPrimed, HandXbowPlus2;
//    private static ItemDefinition HandXbowPlus1, HandXbowPlus3;

//    internal static ItemDefinition HandXbowAcid;

//    internal static ItemDefinition UnarmedStrikeClaws;

//    internal static void Load()
//    {
//        BuildHandwraps();
//        BuildHalberds();
//        BuildPikes();
//        BuildLongMaces();
//        BuildHandXbow();
//        WeaponizeProducedFlame();
//        BuildThunderGauntlet();
//        BuildLightningLauncher();
//        BuildUnarmedStrikeClaws();
//        UpdateHandWrapsUseGauntletSlot();
//    }

//    [NotNull]
//    internal static ItemPresentation BuildPresentation(
//        string unIdentifiedName,
//        ItemPresentation basePresentation = null,
//        AssetReference customPresentation = null,
//        float scale = 1.0f, bool hasUnidentifiedDescription = false)
//    {
//        //TODO: either create a builder for ItemPresentation, or add setter with custom values to ItemDefinitionBuilder
//        var presentation = new ItemPresentation(basePresentation);

//        if (customPresentation == null) { presentation.assetReference = basePresentation.AssetReference; }
//        else { presentation.assetReference = customPresentation; }

//        presentation.ItemFlags.Clear();
//        presentation.unidentifiedTitle = GuiPresentationBuilder.CreateTitleKey(unIdentifiedName, Category.Item);
//        presentation.unidentifiedDescription = hasUnidentifiedDescription
//            ? GuiPresentationBuilder.CreateDescriptionKey(unIdentifiedName, Category.Item)
//            : GuiPresentationBuilder.EmptyString;

//        presentation.scaleFactorWhileWielded = scale;

//        return presentation;
//    }

//    [NotNull]
//    internal static ItemDefinition BuildWeapon(string name, ItemDefinition baseItem, AssetReference customPresentation, int goldCost, bool noDescription,
//        ItemRarity rarity,
//        ItemPresentation basePresentation = null,
//        WeaponDescription baseDescription = null,
//        AssetReferenceSprite icon = null,
//        bool needId = true,
//        float scale = 1.0f,
//        bool twoHanded = true,
//        params ItemPropertyDescription[] properties)
//    {
//        basePresentation ??= baseItem.ItemPresentation;
//        baseDescription ??= new WeaponDescription(baseItem.WeaponDescription);
//        icon ??= baseItem.GuiPresentation.SpriteReference;

//        var builder = ItemDefinitionBuilder
//            .Create(baseItem, name)
//            .SetGold(goldCost)
//            .SetMerchantCategory(MerchantCategoryDefinitions.Weapon)
//            .SetStaticProperties(properties)
//            .SetWeaponDescription(baseDescription)
//            .SetItemPresentation(BuildPresentation($"{name}Unidentified", basePresentation, customPresentation, scale))
//            .SetItemRarity(rarity);

//        if (twoHanded)
//        {
//            _ = builder
//                .SetSlotTypes(SlotTypeDefinitions.MainHandSlot, SlotTypeDefinitions.ContainerSlot)
//                .SetSlotsWhereActive(SlotTypeDefinitions.MainHandSlot);
//        }
//        else
//        {
//            _ = builder
//                .SetSlotTypes(SlotTypeDefinitions.MainHandSlot, SlotTypeDefinitions.OffHandSlot,
//                    SlotTypeDefinitions.ContainerSlot)
//                .SetSlotsWhereActive(SlotTypeDefinitions.MainHandSlot, SlotTypeDefinitions.OffHandSlot);
//        }

//        if (properties.Length != 0)
//        {
//            _ = builder.MakeMagical();

//            if (needId)
//            {
//                _ = builder.SetRequiresIdentification(true);
//            }
//        }

//        _ = noDescription
//            ? builder.SetGuiPresentation(Category.Item, Gui.NoLocalization, icon)
//            : builder.SetGuiPresentation(Category.Item, icon);

//        var weapon = builder.AddToDB();

//        weapon.inDungeonEditor = Main.Settings.AddNewWeaponsAndRecipesToEditor;

//        return weapon;
//    }

//    internal static void AddPolearmWeaponTag(ItemDefinition item, Dictionary<string, TagsDefinitions.Criticity> tags)
//    {
//        if (ValidatorsWeapon.IsWeaponType(item,
//                QuarterstaffType, SpearType, HalberdWeaponType, PikeWeaponType, LongMaceWeaponType))
//        {
//            tags.TryAdd(PolearmWeaponTag, TagsDefinitions.Criticity.Normal);
//        }
//    }

//    internal static ItemDefinition GetStandardWeaponOfType(string type)
//    {
//        //Darts for some reason are not marked as `Standard`, so return regular Dart for this type 
//        if (type == DartType.Name)
//        {
//            return ItemDefinitions.Dart;
//        }

//        var allElements = DatabaseRepository.GetDatabase<ItemDefinition>().GetAllElements();

//        return allElements
//            .FirstOrDefault(item =>
//                item.ItemTags.Contains(TagsDefinitions.ItemTagStandard) && item.IsWeapon &&
//                item.WeaponDescription.WeaponTypeDefinition.Name == type);
//    }

//    #region Longswords
//    private static void BuildLongswords()
//    {
//        var baseItem = ItemDefinitions.Longsword;
//        var basePresentation = ItemDefinitions.Longsword.ItemPresentation;
//        var baseDescription = new WeaponDescription(baseItem.WeaponDescription)
//        {
//            reachRange = 1,
//            weaponType = LongswordType.Name,
//            weaponTags =
//            [
//                TagsDefinitions.WeaponTagVersatile
//            ]
//        };
//        var damageForm = baseDescription.EffectDescription
//            .GetFirstFormOfType(EffectForm.EffectFormType.Damage).DamageForm;

//        damageForm.dieType = DieType.D8;
//        damageForm.versatileDieType = DieType.D10;
//        damageForm.diceNumber = 1;

//        var katanaPrefab = Prefabs.GetKatanaPrefab();

//        Katana = BuildWeapon("Katana", baseItem, katanaPrefab, 20, true, Common, null, baseDescription,
//            icon: Sprites.KatanaIcon,
//            scale: 0.005f
//        );
//    }

//    #endregion

//    #region Handwraps
//    private static void BuildHandwraps()
//    {
//        HandwrapsOfForce = BuildHandwrapsCommon("HandwrapsOfForce", 2000, true, false, Rare, ForceImpactVFX,
//            WeaponPlus1AttackOnly);
//        HandwrapsOfForce.WeaponDescription.EffectDescription.effectForms.Add(
//            EffectFormBuilder
//                .Create()
//                .SetDamageForm(DamageTypeForce, 1, DieType.D4)
//                .SetCreatedBy(false, false)
//                .Build());

//        HandwrapsOfPulling = BuildHandwrapsCommon("HandwrapsOfPulling", 2000, true, false, Rare, WeaponPlus1AttackOnly);
//        HandwrapsOfPulling.IsUsableDevice = true;
//        HandwrapsOfPulling.usableDeviceDescription = new UsableDeviceDescriptionBuilder()
//            .SetRecharge(RechargeRate.AtWill)
//            .SetSaveDc(EffectHelpers.BasedOnUser)
//            .AddFunctions(new DeviceFunctionDescriptionBuilder()
//                .SetPower(
//                    FeatureDefinitionPowerBuilder
//                        .Create("PowerHandwrapsOfPulling")
//                        .SetGuiPresentation(Category.Feature)
//                        .SetUsesFixed(ActivationTime.BonusAction)
//                        .SetEffectDescription(
//                            EffectDescriptionBuilder
//                                .Create()
//                                .SetTargetingData(Side.All, RangeType.Distance, 3, TargetType.IndividualsUnique)
//                                .ExcludeCaster()
//                                .SetSavingThrowData(
//                                    true,
//                                    AttributeDefinitions.Strength,
//                                    false,
//                                    EffectDifficultyClassComputation.AbilityScoreAndProficiency)
//                                .SetParticleEffectParameters(FeatureDefinitionPowers.PowerShadowTamerRopeGrapple)
//                                .SetEffectForms(
//                                    EffectFormBuilder
//                                        .Create()
//                                        .SetMotionForm(MotionForm.MotionType.DragToOrigin, 2)
//                                        .SetCreatedBy(false, false)
//                                        .Build())
//                                .Build())
//                        .AddToDB())
//                .Build())
//            .Build();

//        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(HandwrapsOfForce, 14, 18,
//            ItemDefinitions.Ingredient_Enchant_Soul_Gem, ItemDefinitions.Primed_Gauntlet), ShopItemType.ShopCrafting);

//        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(HandwrapsOfPulling, 14, 18,
//            ItemDefinitions.Ingredient_Enchant_Stardust, ItemDefinitions.Primed_Gauntlet), ShopItemType.ShopCrafting);
//    }

//    [NotNull]
//    private static ItemDefinition BuildHandwrapsCommon(string name, int goldCost, bool noDescription, bool needId,
//        ItemRarity rarity,
//        params ItemPropertyDescription[] properties)
//    {
//        return BuildWeapon(
//            name,
//            ItemDefinitions.Primed_Gauntlet,
//            null,
//            goldCost,
//            noDescription, rarity, needId: needId,
//            properties: properties
//        );
//    }

//    internal static void UpdateHandWrapsUseGauntletSlot()
//    {
//        foreach (var item in DatabaseRepository.GetDatabase<ItemDefinition>())
//        {
//            if (item is not { WeaponDescription.weaponType: EquipmentDefinitions.WeaponTypeUnarmedStrike })
//            {
//                continue;
//            }

//            if (item == ItemDefinitions.UnarmedStrikeBase) { continue; }

//            if (Main.Settings.EnableMonkHandwrapsUseGauntletSlot)
//            {
//                item.SlotTypes.Add(EquipmentDefinitions.SlotTypeGloves);
//                item.SlotsWhereActive.Add(EquipmentDefinitions.SlotTypeGloves);
//            }
//            else
//            {
//                item.SlotTypes.Remove(EquipmentDefinitions.SlotTypeGloves);
//                item.SlotsWhereActive.Remove(EquipmentDefinitions.SlotTypeGloves);
//            }
//        }
//    }

//    internal static void ModifyUnarmedAttackWithGauntlet(RulesetCharacterHero hero, ref ItemDefinition itemDefinition,
//        ref WeaponDescription weaponDescription, ref RulesetItem weapon)
//    {
//        if (!Main.Settings.EnableMonkHandwrapsUseGauntletSlot
//            || weapon != null || itemDefinition != ItemDefinitions.UnarmedStrikeBase)
//        {
//            return;
//        }

//        var item = hero.CharacterInventory.InventorySlotsByType[EquipmentDefinitions.SlotTypeGloves][0]?.EquipedItem;

//        if (item is not { ItemDefinition.WeaponDescription.WeaponType: EquipmentDefinitions.WeaponTypeUnarmedStrike })
//        {
//            return;
//        }

//        itemDefinition = item.ItemDefinition;
//        weaponDescription = itemDefinition.WeaponDescription;
//        weapon = item;
//    }

//    //TODO: not sure this is the best place for this method
//    internal static void TryAddMainActionUnarmedAttacks(RulesetCharacterHero hero)
//    {
//        if (!Main.Settings.EnableUnarmedMainAttackAction)
//        {
//            return;
//        }

//        //skip if we already have main unarmed attack
//        if (hero.AttackModes.Any(m => m is
//            {
//                ActionType: ActionDefinitions.ActionType.Main,
//                SourceDefinition: ItemDefinition
//                {
//                    WeaponDescription.WeaponType: EquipmentDefinitions.WeaponTypeUnarmedStrike
//                }
//            }
//            ))
//        {
//            return;
//        }

//        //checkin only extra glove slot, because gauntlets in main hand slot would already add attack
//        var item = hero.CharacterInventory.InventorySlotsByType[EquipmentDefinitions.SlotTypeGloves][0]?.EquipedItem;
//        var definition = item?.ItemDefinition;

//        if (item is not { ItemDefinition.WeaponDescription.WeaponType: EquipmentDefinitions.WeaponTypeUnarmedStrike })
//        {
//            item = null;
//            definition = null;
//        }

//        definition ??= ItemDefinitions.UnarmedStrikeBase;

//        if (hero.GetClassLevel(CharacterClassDefinitions.Monk) == 0 && item == null)
//        {
//            return;
//        }

//        hero.AttackModes.Add(hero.RefreshAttackMode(ActionDefinitions.ActionType.Main, definition,
//            definition.WeaponDescription, hero.HasFreeHandSlot(), true, EquipmentDefinitions.SlotTypeMainHand,
//            hero.attackModifiers, hero.FeaturesOrigin, item));
//    }

//    #endregion

//    #region Halberds
//    private static void BuildHalberds()
//    {
//        HalberdWeaponType = WeaponTypeDefinitionBuilder
//            .Create(GreataxeType, "CEHalberdType")
//            .SetGuiPresentation(Category.Item, GuiPresentationBuilder.EmptyString)
//            .SetWeaponCategory(WeaponCategoryDefinitions.MartialWeaponCategory)
//            .AddCustomSubFeatures(new CustomScale(z: 3.5f))
//            .AddToDB();

//        var baseItem = ItemDefinitions.Greataxe;
//        var basePresentation = ItemDefinitions.Battleaxe.ItemPresentation;
//        var baseDescription = new WeaponDescription(baseItem.WeaponDescription)
//        {
//            reachRange = 2,
//            weaponType = HalberdWeaponType.Name,
//            weaponTags =
//            [
//                TagsDefinitions.WeaponTagHeavy,
//                TagsDefinitions.WeaponTagReach,
//                TagsDefinitions.WeaponTagTwoHanded
//            ]
//        };
//        var damageForm = baseDescription.EffectDescription
//            .GetFirstFormOfType(EffectForm.EffectFormType.Damage).DamageForm;

//        damageForm.dieType = DieType.D10;
//        damageForm.diceNumber = 1;

//        Halberd = BuildWeapon("CEHalberd", baseItem, null,
//            20, true, Common, basePresentation, baseDescription, Sprites.HalberdIcon);

//        MerchantContext.AddItem(Halberd, ShopItemType.ShopGenericMelee);

//        HalberdPrimed = BuildWeapon("CEHalberdPrimed", baseItem, null,
//            40, true, Uncommon, basePresentation, baseDescription, Sprites.HalberdPrimedIcon);
//        HalberdPrimed.ItemTags.Add(TagsDefinitions.ItemTagIngredient);
//        HalberdPrimed.ItemTags.Remove(TagsDefinitions.ItemTagStandard);

//        MerchantContext.AddItem(HalberdPrimed, ShopItemType.ShopPrimedMelee);
//        MerchantContext.AddItem(RecipeHelper.BuildPrimeManual(Halberd, HalberdPrimed), ShopItemType.ShopCrafting);

//        HalberdPlus1 = BuildWeapon("CEHalberd+1", Halberd, null,
//            1050, true, Rare, icon: Sprites.HalberdP1Icon, properties: [WeaponPlus1]);

//        MerchantContext.AddItem(HalberdPlus1, ShopItemType.ShopMeleePlus1);
//        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(HalberdPlus1, 12, 14,
//            HalberdPrimed,
//            ItemDefinitions.Ingredient_Enchant_Oil_Of_Acuteness), ShopItemType.ShopCrafting);

//        var itemDefinition = ItemDefinitions.BattleaxePlus2;

//        HalberdPlus2 = BuildWeapon("CEHalberd+2", Halberd, null,
//            4000, true, VeryRare,
//            itemDefinition.ItemPresentation, icon: Sprites.HalberdP2Icon,
//            properties: [WeaponPlus2]);

//        MerchantContext.AddItem(HalberdPlus2, ShopItemType.ShopMeleePlus2);
//        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(HalberdPlus2, 14, 18,
//            HalberdPrimed,
//            ItemDefinitions.Ingredient_Enchant_Blood_Gem), ShopItemType.ShopCrafting);

//        itemDefinition = ItemDefinitions.BattleaxePlus3;

//        HalberdPlus3 = BuildWeapon("CEHalberd+3", Halberd, null,
//            16000, true, VeryRare,
//            itemDefinition.ItemPresentation, icon: Sprites.HalberdP3Icon,
//            properties: [WeaponPlus3]);

//        MerchantContext.AddItem(HalberdPlus3, ShopItemType.ShopMeleePlus3);
//        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(HalberdPlus3, 16, 22,
//            HalberdPrimed,
//            ItemDefinitions.Ingredient_Enchant_Blood_Gem,
//            ItemDefinitions.Ingredient_Enchant_Blood_Gem), ShopItemType.ShopCrafting);

//        HalberdLightning = BuildWeapon("CEHalberdLightning", Halberd, null,
//            4000, true, VeryRare,
//            itemDefinition.ItemPresentation, icon: Sprites.HalberdLightningIcon, needId: false,
//            properties: [LightningImpactVFX, WeaponPlus1AttackOnly]);

//        HalberdLightning.WeaponDescription.EffectDescription.effectForms.Add(
//            EffectFormBuilder
//                .Create()
//                .SetDamageForm(DamageTypeLightning, 1, DieType.D8)
//                .SetCreatedBy(false, false)
//                .Build());
//        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(HalberdLightning, 14, 18,
//            HalberdPrimed,
//            ItemDefinitions.Ingredient_Enchant_Stardust), ShopItemType.ShopCrafting);
//    }

//    #endregion

//    #region Pikes
//    private static void BuildPikes()
//    {
//        PikeWeaponType = WeaponTypeDefinitionBuilder
//            .Create(SpearType, "CEPikeType")
//            .SetGuiPresentation(Category.Item, GuiPresentationBuilder.EmptyString)
//            .SetWeaponCategory(WeaponCategoryDefinitions.MartialWeaponCategory)
//            //.AddCustomSubFeatures(new CustomScale(z: 3.5f))
//            .AddToDB();

//        var baseItem = ItemDefinitions.Spear;
//        var basePresentation = ItemDefinitions.Morningstar.ItemPresentation;
//        var baseDescription = new WeaponDescription(baseItem.WeaponDescription)
//        {
//            reachRange = 2,
//            weaponType = PikeWeaponType.Name,
//            weaponTags =
//            [
//                TagsDefinitions.WeaponTagHeavy,
//                TagsDefinitions.WeaponTagReach,
//                TagsDefinitions.WeaponTagTwoHanded
//            ]
//        };
//        var damageForm = baseDescription.EffectDescription
//            .GetFirstFormOfType(EffectForm.EffectFormType.Damage).DamageForm;

//        damageForm.dieType = DieType.D10;
//        damageForm.diceNumber = 1;

//        var pikePrefab = Prefabs.GetPikePrefab();

//        Pike = BuildWeapon("CEPike", baseItem, pikePrefab, 20, true, Common, null, baseDescription,
//            icon: Sprites.PikeIcon
//        );

//        Pike.ItemTags.Remove(TagsDefinitions.ItemTagMonk);
//        MerchantContext.AddItem(Pike, ShopItemType.ShopGenericMelee);

//        PikePrimed = BuildWeapon("CEPikePrimed", baseItem, pikePrefab, 40, true, Uncommon, null, baseDescription, Sprites.PikePrimedIcon);
//        PikePrimed.ItemTags.Add(TagsDefinitions.ItemTagIngredient);
//        PikePrimed.ItemTags.Remove(TagsDefinitions.ItemTagStandard);
//        PikePrimed.ItemTags.Remove(TagsDefinitions.ItemTagMonk);

//        MerchantContext.AddItem(PikePrimed, ShopItemType.ShopPrimedMelee);
//        MerchantContext.AddItem(RecipeHelper.BuildPrimeManual(Pike, PikePrimed), ShopItemType.ShopCrafting);

//        PikePlus1 = BuildWeapon("CEPike+1", Pike, pikePrefab, 1050, true, Rare,
//            icon: Sprites.PikeP1Icon,
//            properties: [WeaponPlus1]
//        );

//        PikePlus1.ItemTags.Remove(TagsDefinitions.ItemTagMonk);
//        MerchantContext.AddItem(PikePlus1, ShopItemType.ShopMeleePlus1);
//        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(PikePlus1, 12, 14,
//            PikePrimed,
//            ItemDefinitions.Ingredient_Enchant_Oil_Of_Acuteness), ShopItemType.ShopCrafting);

//        var itemDefinition = ItemDefinitions.MorningstarPlus2;

//        PikePlus2 = BuildWeapon("CEPike+2", Pike, pikePrefab, 4000, true, VeryRare,
//            icon: Sprites.PikeP2Icon,
//            properties: [WeaponPlus2]
//        );

//        PikePlus2.ItemTags.Remove(TagsDefinitions.ItemTagMonk);
//        MerchantContext.AddItem(PikePlus2, ShopItemType.ShopMeleePlus2);
//        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(PikePlus2, 14, 18,
//            PikePrimed,
//            ItemDefinitions.Ingredient_Enchant_Blood_Gem), ShopItemType.ShopCrafting);

//        itemDefinition = ItemDefinitions.MorningstarPlus3;

//        PikePlus3 = BuildWeapon("CEPike+3", Pike, pikePrefab, 16000, true, VeryRare,
//            icon: Sprites.PikeP3Icon,
//            properties: [WeaponPlus3]
//        );

//        PikePlus3.ItemTags.Remove(TagsDefinitions.ItemTagMonk);
//        MerchantContext.AddItem(PikePlus3, ShopItemType.ShopMeleePlus3);
//        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(PikePlus3, 16, 22,
//            PikePrimed,
//            ItemDefinitions.Ingredient_Enchant_Blood_Gem,
//            ItemDefinitions.Ingredient_Enchant_Blood_Gem), ShopItemType.ShopCrafting);

//        PikePsychic = BuildWeapon("CEPikePsychic", Pike, pikePrefab, 4000, true, VeryRare,
//            icon: Sprites.PikePsychicIcon,
//            needId: false,
//            properties: [PsychicImpactVFX, WeaponPlus1AttackOnly]
//        );

//        PikePsychic.ItemTags.Remove(TagsDefinitions.ItemTagMonk);
//        PikePsychic.WeaponDescription.EffectDescription.effectForms.Add(
//            EffectFormBuilder
//                .Create()
//                .SetDamageForm(DamageTypePsychic, 1, DieType.D8)
//                .SetCreatedBy(false, false)
//                .Build());
//        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(PikePsychic, 14, 18,
//            PikePrimed,
//            ItemDefinitions.Ingredient_Enchant_Stardust), ShopItemType.ShopCrafting);
//    }

//    #endregion

//    #region Long Maces
//    private static void BuildLongMaces()
//    {
//        LongMaceWeaponType = WeaponTypeDefinitionBuilder
//            .Create(MaulType, "CELongMaceType")
//            .SetGuiPresentation(Category.Item, GuiPresentationBuilder.EmptyString)
//            .SetWeaponCategory(WeaponCategoryDefinitions.MartialWeaponCategory)
//            //.AddCustomSubFeatures(new CustomScale(z: 3.5f))
//            .AddToDB();

//        var longmacePrefab = Prefabs.GetLongMacePrefab();

//        var baseItem = ItemDefinitions.Warhammer;
//        var basePresentation = ItemDefinitions.Mace.ItemPresentation;
//        var baseDescription = new WeaponDescription(baseItem.WeaponDescription)
//        {
//            reachRange = 2,
//            weaponType = LongMaceWeaponType.Name,
//            weaponTags =
//            [
//                TagsDefinitions.WeaponTagHeavy,
//                TagsDefinitions.WeaponTagReach,
//                TagsDefinitions.WeaponTagTwoHanded
//            ]
//        };
//        var damageForm = baseDescription.EffectDescription
//            .GetFirstFormOfType(EffectForm.EffectFormType.Damage).DamageForm;

//        damageForm.dieType = DieType.D10;
//        damageForm.diceNumber = 1;

//        LongMace = BuildWeapon("CELongMace", baseItem, longmacePrefab, 20, true, Common, null, baseDescription, Sprites.LongMaceIcon,
//            scale: 0.035f
//        );

//        MerchantContext.AddItem(LongMace, ShopItemType.ShopGenericMelee);

//        LongMacePrimed = BuildWeapon("CELongMacePrimed", baseItem, longmacePrefab, 40, true, Uncommon, null, baseDescription, Sprites.LongMacePrimedIcon);
//        LongMacePrimed.ItemTags.Add(TagsDefinitions.ItemTagIngredient);
//        LongMacePrimed.ItemTags.Remove(TagsDefinitions.ItemTagStandard);

//        MerchantContext.AddItem(LongMacePrimed, ShopItemType.ShopPrimedMelee);
//        MerchantContext.AddItem(RecipeHelper.BuildPrimeManual(LongMace, LongMacePrimed), ShopItemType.ShopCrafting);

//        LongMacePlus1 = BuildWeapon("CELongMace+1", LongMace, longmacePrefab, 1050, true, Rare,
//            scale: 0.035f,
//            icon: Sprites.LongMaceP1Icon,
//            properties: [WeaponPlus1]
//        );

//        MerchantContext.AddItem(LongMacePlus1, ShopItemType.ShopMeleePlus1);
//        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(LongMacePlus1, 12, 14,
//            LongMacePrimed,
//            ItemDefinitions.Ingredient_Enchant_Oil_Of_Acuteness), ShopItemType.ShopCrafting);

//        var itemDefinition = ItemDefinitions.MacePlus2;

//        LongMacePlus2 = BuildWeapon("CELongMace+2", LongMace, longmacePrefab, 4000, true, VeryRare,
//            scale: 0.035f,
//            icon: Sprites.LongMaceP2Icon,
//            properties: [WeaponPlus2]
//        );

//        MerchantContext.AddItem(LongMacePlus2, ShopItemType.ShopMeleePlus2);
//        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(LongMacePlus2, 14, 18,
//            LongMacePrimed,
//            ItemDefinitions.Ingredient_Enchant_Blood_Gem), ShopItemType.ShopCrafting);

//        itemDefinition = ItemDefinitions.MacePlus3;

//        LongMacePlus3 = BuildWeapon("CELongMace+3", LongMace, longmacePrefab, 16000, true, VeryRare,
//            scale: 0.035f,
//            icon: Sprites.LongMaceP3Icon,
//            properties: [WeaponPlus3]
//        );

//        MerchantContext.AddItem(LongMacePlus3, ShopItemType.ShopMeleePlus3);
//        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(LongMacePlus3, 16, 22,
//            LongMacePrimed,
//            ItemDefinitions.Ingredient_Enchant_Blood_Gem,
//            ItemDefinitions.Ingredient_Enchant_Blood_Gem), ShopItemType.ShopCrafting);

//        LongMaceThunder = BuildWeapon("CELongMaceThunder", LongMace, longmacePrefab, 4000, true, VeryRare,
//            scale: 0.035f,
//            icon: Sprites.LongMaceThunderIcon,
//            needId: false,
//            properties: [ThunderImpactVFX, WeaponPlus1AttackOnly]
//        );

//        LongMaceThunder.WeaponDescription.EffectDescription.effectForms.Add(
//            EffectFormBuilder
//                .Create()
//                .SetDamageForm(DamageTypeThunder, 1, DieType.D8)
//                .SetCreatedBy(false, false)
//                .Build());
//        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(LongMaceThunder, 14, 18,
//            LongMacePrimed,
//            ItemDefinitions.Ingredient_Enchant_Stardust), ShopItemType.ShopCrafting);
//    }

//    #endregion

//    #region Hand Crossbows
//    private static void BuildHandXbow()
//    {
//        HandXbowWeaponType = WeaponTypeDefinitionBuilder
//            .Create(LightCrossbowType, CeHandXbowType)
//            .SetGuiPresentation(Category.Item, GuiPresentationBuilder.EmptyString)
//            .SetWeaponCategory(WeaponCategoryDefinitions.MartialWeaponCategory)
//            .AddCustomSubFeatures(new CustomScale(0.5f))
//            .SetAnimationTag("Rapier")
//            .AddToDB();

//        var baseItem = ItemDefinitions.LightCrossbow;
//        var basePresentation = new ItemPresentation(baseItem.ItemPresentation);
//        var baseDescription = new WeaponDescription(baseItem.WeaponDescription)
//        {
//            weaponType = HandXbowWeaponType.Name,
//            closeRange = 6,
//            maxRange = 24,
//            weaponTags =
//            [
//                TagsDefinitions.WeaponTagLight,
//                TagsDefinitions.WeaponTagRange,
//                TagsDefinitions.WeaponTagLoading,
//                TagsDefinitions.WeaponTagAmmunition
//            ]
//        };
//        var damageForm = baseDescription.EffectDescription
//            .GetFirstFormOfType(EffectForm.EffectFormType.Damage).DamageForm;

//        damageForm.dieType = DieType.D6;
//        damageForm.diceNumber = 1;

//        //add hand xbow proficiency to rogues
//        var rogueHandXbowProficiency = FeatureDefinitionProficiencys.ProficiencyRogueWeapon;

//        rogueHandXbowProficiency.Proficiencies.Add(HandXbowWeaponType.Name);

//        HandXbow = BuildWeapon("CEHandXbow", baseItem, null,
//            20, true, Common, basePresentation, baseDescription, Sprites.HandXbowIcon,
//            twoHanded: false);

//        MerchantContext.AddItem(HandXbow, ShopItemType.ShopGenericRanged);

//        HandXbowPrimed = BuildWeapon("CEHandXbowPrimed", HandXbow, null,
//            40, true, Uncommon, icon: Sprites.HandXbowPrimedIcon, twoHanded: false);

//        HandXbowPrimed.ItemTags.Add(TagsDefinitions.ItemTagIngredient);
//        HandXbowPrimed.ItemTags.Remove(TagsDefinitions.ItemTagStandard);
//        MerchantContext.AddItem(HandXbowPrimed, ShopItemType.ShopPrimedRanged);
//        MerchantContext.AddItem(RecipeHelper.BuildPrimeManual(HandXbow, HandXbowPrimed), ShopItemType.ShopCrafting);

//        HandXbowPlus1 = BuildWeapon("CEHandXbow+1", HandXbow, null,
//            1050, true, Rare, icon: Sprites.HandXbowP1Icon, twoHanded: false,
//            properties: [WeaponPlus1]);

//        MerchantContext.AddItem(HandXbowPlus1, ShopItemType.ShopRangedPlus1);
//        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(HandXbowPlus1, 12, 14,
//            HandXbowPrimed,
//            ItemDefinitions.Ingredient_Enchant_Oil_Of_Acuteness), ShopItemType.ShopCrafting);

//        var itemDefinition = ItemDefinitions.LightCrossbowPlus2;

//        HandXbowPlus2 = BuildWeapon("CEHandXbow+2", HandXbow, null,
//            4000, true, VeryRare,
//            itemDefinition.ItemPresentation, icon: Sprites.HandXbowP2Icon, twoHanded: false,
//            properties: [WeaponPlus2]);

//        MerchantContext.AddItem(HandXbowPlus2, ShopItemType.ShopRangedPlus2);
//        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(HandXbowPlus2, 14, 18,
//            HandXbowPrimed,
//            ItemDefinitions.Ingredient_Enchant_Blood_Gem), ShopItemType.ShopCrafting);

//        HandXbowPlus3 = BuildWeapon("CEHandXbow+3", HandXbow, null,
//            16000, true, VeryRare,
//            itemDefinition.ItemPresentation, icon: Sprites.HandXbowP3Icon, twoHanded: false,
//            properties: [WeaponPlus3]);

//        MerchantContext.AddItem(HandXbowPlus3, ShopItemType.ShopRangedPlus3);
//        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(HandXbowPlus3, 16, 22,
//            HandXbowPrimed,
//            ItemDefinitions.Ingredient_Enchant_Blood_Gem,
//            ItemDefinitions.Ingredient_Enchant_Blood_Gem), ShopItemType.ShopCrafting);

//        HandXbowAcid = BuildWeapon("CEHandXbowAcid", HandXbow, null,
//            4000, true, VeryRare,
//            itemDefinition.ItemPresentation, icon: Sprites.HandXbowAcidIcon, needId: false, twoHanded: false,
//            properties: [AcidImpactVFX, WeaponPlus1AttackOnly]);

//        HandXbowAcid.WeaponDescription.EffectDescription.effectForms.Add(
//            EffectFormBuilder
//                .Create()
//                .SetDamageForm(DamageTypeAcid, 1, DieType.D8)
//                .SetCreatedBy(false, false)
//                .Build());
//        MerchantContext.AddItem(RecipeHelper.BuildRecipeManual(HandXbowAcid, 14, 18,
//            HandXbowPrimed,
//            ItemDefinitions.Ingredient_Enchant_Stardust), ShopItemType.ShopCrafting);
//    }

//    #endregion

//    #region Produce Flame
//    private static void WeaponizeProducedFlame()
//    {
//        var flame = ItemDefinitions.ProducedFlame;

//        flame.GuiPresentation = new GuiPresentationBuilder(flame.GuiPresentation)
//            .SetTitle("Item/&CEProducedFlameTitle")
//            .Build();

//        ProducedFlameDart = BuildWeapon("CEProducedFlameDart", ItemDefinitions.Dart, null, 0, true, Common,
//            flame.ItemPresentation, icon: Sprites.ProducedFlameThrow);
//        ProducedFlameDart.inDungeonEditor = false;

//        var damageForm = ProducedFlameDart.WeaponDescription.EffectDescription.FindFirstDamageForm();

//        damageForm.damageType = DamageTypeFire;
//        damageForm.dieType = DieType.D8;

//        var weapon = new WeaponDescription(ItemDefinitions.UnarmedStrikeBase.weaponDefinition);

//        weapon.EffectDescription.effectForms.Add(
//            EffectFormBuilder
//                .Create()
//                .SetDamageForm(DamageTypeFire, 1, DieType.D8)
//                .SetCreatedBy(false, false)
//                .Build());
//        flame.staticProperties.Add(
//            BuildFrom(
//                FeatureDefinitionBuilder
//                    .Create("FeatureProducedFlameThrower")
//                    .SetGuiPresentationNoContent()
//                    .AddCustomSubFeatures(
//                        new ModifyWeaponProducedFlameDice(),
//                        new AddThrowProducedFlameAttack())
//                    .AddToDB(), false));

//        flame.IsWeapon = true;
//        flame.weaponDefinition = weapon;
//    }

//    internal static void ProcessProducedFlameAttack([NotNull] RulesetCharacterHero hero,
//     [NotNull] RulesetAttackMode mode)
//    {
//        var num = hero.characterInventory.CurrentConfiguration;
//        var configurations = hero.characterInventory.WieldedItemsConfigurations;

//        if (num == configurations.Count - 1)
//        {
//            num = configurations[num].MainHandSlot.ShadowedSlot != configurations[0].MainHandSlot ? 1 : 0;
//        }

//        var itemsConfiguration = configurations[num];
//        RulesetItem item = null;

//        if (mode.SlotName == EquipmentDefinitions.SlotTypeMainHand)
//        {
//            item = itemsConfiguration.MainHandSlot.EquipedItem;
//        }
//        else if (mode.SlotName == EquipmentDefinitions.SlotTypeOffHand)
//        {
//            item = itemsConfiguration.OffHandSlot.EquipedItem;
//        }

//        if (item == null || item.ItemDefinition != ItemDefinitions.ProducedFlame)
//        {
//            return;
//        }

//        hero.CharacterInventory.DefineWieldedItemsConfiguration(num, null, mode.SlotName);
//    }

//    #endregion

//    #region Thunder Guantlet
//    private static void BuildThunderGauntlet()
//    {
//        ThunderGauntletType = WeaponTypeDefinitionBuilder
//            .Create(UnarmedStrikeType, "CEThunderGauntletType")
//            .SetGuiPresentation("Item/&CEThunderGauntletTitle", GuiPresentationBuilder.EmptyString)
//            .SetWeaponCategory(WeaponCategoryDefinitions.SimpleWeaponCategory)
//            .AddToDB();

//        var thunderGauntletPrefab = Prefabs.GetThunderGauntletPrefab();
//        var baseItem = ItemDefinitions.UnarmedStrikeBase;
//        var basePresentation = baseItem.ItemPresentation;
//        var baseDescription = new WeaponDescription(baseItem.WeaponDescription)
//        {
//            reachRange = 1,
//            weaponType = ThunderGauntletType.Name,
//            weaponTags = []
//        };
//        var damageForm = baseDescription.EffectDescription
//            .GetFirstFormOfType(EffectForm.EffectFormType.Damage).DamageForm;

//        damageForm.dieType = DieType.D8;
//        damageForm.diceNumber = 1;
//        damageForm.damageType = DamageTypeThunder;

//        baseDescription.EffectDescription.DurationType = DurationType.Round;
//        baseDescription.EffectDescription.DurationParameter = 1;
//        baseDescription.EffectDescription.EndOfEffect = (TurnOccurenceType)ExtraTurnOccurenceType.StartOfSourceTurn;
//        baseDescription.EffectDescription.EffectForms.Add(
//            EffectFormBuilder.ConditionForm(CustomConditionsContext.Taunted));

//        var effectParticleParameters = new EffectParticleParameters();

//        effectParticleParameters.Copy(SpellDefinitions.Shatter.EffectDescription.EffectParticleParameters);
//        effectParticleParameters.casterParticleReference = new AssetReference();
//        baseDescription.EffectDescription.effectParticleParameters = effectParticleParameters;

//        ThunderGauntlet = BuildWeapon("CEThunderGauntlet", baseItem, thunderGauntletPrefab, 0, true, Common, null, baseDescription,
//            Sprites.GetSprite("ItemThunderGauntlet", SolastaUnfinishedBusiness.Properties.Resources.ItemThunderGauntlet, 128),
//            scale: 0.25f,
//            properties: [ThunderImpactVFX]
//        );
//        ThunderGauntlet.inDungeonEditor = false;
//    }

//    #endregion

//    #region Lightning Launcher
//    private static void BuildLightningLauncher()
//    {
//        LightningLauncherType = WeaponTypeDefinitionBuilder
//            .Create(ShortbowType, "CELightningLauncherType")
//            .SetGuiPresentation("Item/&CELightningLauncherTitle", GuiPresentationBuilder.EmptyString)
//            .SetWeaponCategory(WeaponCategoryDefinitions.SimpleWeaponCategory)
//            .SetAnimationTag("Rapier")
//            .AddToDB();

//        var launcherPrefab = Prefabs.GetLauncherPrefab();
//        var baseItem = ItemDefinitions.Shortbow;
//        var basePresentation = baseItem.ItemPresentation;
//        var baseDescription = new WeaponDescription(baseItem.WeaponDescription)
//        {
//            //TODO: add custom ammunition that looks like lightning
//            closeRange = 18,
//            maxRange = 60,
//            weaponType = LightningLauncherType.Name,
//            weaponTags = []
//        };
//        var damageForm = baseDescription.EffectDescription
//            .GetFirstFormOfType(EffectForm.EffectFormType.Damage).DamageForm;

//        damageForm.dieType = DieType.D6;
//        damageForm.diceNumber = 1;
//        damageForm.damageType = DamageTypeLightning;

//        baseDescription.EffectDescription.EffectForms.Add(
//            EffectFormBuilder
//                .Create()
//                .SetConditionForm(
//                    ConditionDefinitionBuilder
//                        .Create(AttackedWithLauncherConditionName)
//                        .SetGuiPresentationNoContent(true)
//                        .SetSilent(Silent.WhenAddedOrRemoved)
//                        .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
//                        .AddToDB(), ConditionForm.ConditionOperation.Add, true)
//                .Build());

//        baseDescription.EffectDescription.effectParticleParameters =
//            SpellDefinitions.LightningBolt.EffectDescription.EffectParticleParameters;

//        LightningLauncher = BuildWeapon("CELightningLauncher", baseItem, launcherPrefab, 0, true, Common, null, baseDescription,
//            Sprites.GetSprite("ItemGemLightning", SolastaUnfinishedBusiness.Properties.Resources.ItemGemLightning, 128),
//            properties: [LightningImpactVFX],
//            scale: 0.25f
//        );
//        LightningLauncher.inDungeonEditor = false;
//    }

//    #endregion

//    #region Natural Weapons
//    private static void BuildUnarmedStrikeClaws()
//    {
//        var baseItem = ItemDefinitions.UnarmedStrikeBase;
//        var basePresentation = baseItem.ItemPresentation;
//        var baseDescription = new WeaponDescription(baseItem.WeaponDescription);
//        var damageForm = baseDescription.EffectDescription
//            .GetFirstFormOfType(EffectForm.EffectFormType.Damage).DamageForm;

//        damageForm.dieType = DieType.D6;
//        damageForm.diceNumber = 1;
//        damageForm.damageType = DamageTypeSlashing;

//        UnarmedStrikeClaws = BuildWeapon("CEUnarmedStrikeClaws", baseItem, null, 0, true, Common,
//            basePresentation, baseDescription,
//            Sprites.GetSprite("UnarmedStrikeClaws", SolastaUnfinishedBusiness.Properties.Resources.UnarmedStrikeClaws, 128));
//    }

//    #endregion

//}

//internal sealed class ModifyWeaponProducedFlameDice : ModifyWeaponAttackModeBase
//{
//    internal ModifyWeaponProducedFlameDice() : base((_, weapon, _) =>
//        weapon != null && weapon.ItemDefinition == ItemDefinitions.ProducedFlame)
//    {
//    }

//    protected override void TryModifyAttackMode(
//        RulesetCharacter character,
//        RulesetAttackMode attackMode)
//    {
//        var damage = attackMode.EffectDescription.FindFirstDamageForm();

//        if (damage == null)
//        {
//            return;
//        }

//        var casterLevel = character.TryGetAttributeValue(AttributeDefinitions.CharacterLevel);

//        damage.diceNumber = 1 + SpellAdvancementByCasterLevel[casterLevel - 1];
//    }
//}

//internal sealed class AddThrowProducedFlameAttack : AddExtraAttackBase
//{
//    internal AddThrowProducedFlameAttack() : base(ActionDefinitions.ActionType.Main)
//    {
//    }

//    protected override List<RulesetAttackMode> GetAttackModes([NotNull] RulesetCharacter character)
//    {
//        if (character is not RulesetCharacterHero hero)
//        {
//            return null;
//        }

//        var result = new List<RulesetAttackMode>();

//        AddItemAttack(result, EquipmentDefinitions.SlotTypeMainHand, hero);
//        AddItemAttack(result, EquipmentDefinitions.SlotTypeOffHand, hero);

//        return result;
//    }

//    // ReSharper disable once SuggestBaseTypeForParameter
//    private static void AddItemAttack(
//        List<RulesetAttackMode> attackModes,
//        [NotNull] string slot,
//        [NotNull] RulesetCharacterHero hero)
//    {
//        var item = hero.CharacterInventory.InventorySlotsByName[slot].EquipedItem;

//        if (item == null || item.ItemDefinition != ItemDefinitions.ProducedFlame)
//        {
//            return;
//        }

//        var strikeDefinition = CustomWeaponsContext.ProducedFlameDart;

//        var action = slot == EquipmentDefinitions.SlotTypeOffHand
//            ? ActionDefinitions.ActionType.Bonus
//            : ActionDefinitions.ActionType.Main;

//        var attackMode = hero.RefreshAttackMode(
//            action,
//            strikeDefinition,
//            strikeDefinition.WeaponDescription,
//            false,
//            false,
//            slot,
//            hero.attackModifiers,
//            hero.FeaturesOrigin,
//            item
//        );

//        attackMode.closeRange = attackMode.maxRange = 6;
//        attackMode.Reach = false;
//        attackMode.Ranged = true;
//        attackMode.Thrown = true;
//        attackMode.AttackTags.Remove(TagsDefinitions.WeaponTagMelee);

//        attackModes.Add(attackMode);
//    }
//}

//internal sealed class CustomScale
//{
//    internal readonly float X, Y, Z;

//    internal CustomScale(float s) : this(s, s, s)
//    {
//    }

//    internal CustomScale(float x = 1f, float y = 1f, float z = 1f)
//    {
//        X = x;
//        Y = y;
//        Z = z;
//    }
//}
