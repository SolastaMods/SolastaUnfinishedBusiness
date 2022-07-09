using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Classes.Monk;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.ItemCrafting;
using SolastaCommunityExpansion.Properties;
using SolastaCommunityExpansion.Utils;
using UnityEngine.AddressableAssets;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using static SolastaCommunityExpansion.Models.ItemPropertyDescriptions;
using static RuleDefinitions.ItemRarity;

namespace SolastaCommunityExpansion.Models;

public static class CustomWeaponsContext
{
    public const string PolearmWeaponTag = "PolearmWeapon";
    public static WeaponTypeDefinition HalberdWeaponType, PikeWeaponType, LongMaceWeaponType, HandXbowWeaponType;
    public static ItemDefinition HandwrapsPlus1, HandwrapsPlus2, HandwrapsOfForce, HandwrapsOfPulling;
    public static ItemDefinition Halberd, HalberdPrimed, HalberdPlus1, HalberdPlus2, HalberdLightning;
    public static ItemDefinition Pike, PikePrimed, PikePlus1, PikePlus2, PikePsychic;
    public static ItemDefinition LongMace, LongMacePrimed, LongMacePlus1, LongMacePlus2, LongMaceThunder;
    public static ItemDefinition HandXbow, HandXbowPrimed, HandXbowPlus1, HandXbowPlus2, HandXbowAcid;
    public static ItemDefinition ProducedFlameDart;

    public static readonly MerchantFilter GenericMelee = new() {IsMeleeWeapon = true};
    public static readonly MerchantFilter MagicMelee = new() {IsMagicalMeleeWeapon = true};
    public static readonly MerchantFilter PrimedMelee = new() {IsPrimedMeleeWeapon = true};
    public static readonly MerchantFilter GenericRanged = new() {IsRangeWeapon = true};
    public static readonly MerchantFilter MagicRanged = new() {IsMagicalRangeWeapon = true};
    public static readonly MerchantFilter PrimedRanged = new() {IsPrimedRangeWeapon = true};
    public static readonly MerchantFilter CraftingManual = new() {IsDocument = true};

    public static readonly ShopItemType ShopGenericMelee = new(FactionStatusDefinitions.Indifference, GenericMelee);
    public static readonly ShopItemType ShopPrimedMelee = new(FactionStatusDefinitions.Sympathy, PrimedMelee);
    public static readonly ShopItemType ShopMeleePlus1 = new(FactionStatusDefinitions.Alliance, MagicMelee);
    public static readonly ShopItemType ShopMeleePlus2 = new(FactionStatusDefinitions.Brotherhood, MagicMelee);
    public static readonly ShopItemType ShopGenericRanged = new(FactionStatusDefinitions.Indifference, GenericRanged);
    public static readonly ShopItemType ShopPrimedRanged = new(FactionStatusDefinitions.Sympathy, PrimedRanged);
    public static readonly ShopItemType ShopRangedPlus1 = new(FactionStatusDefinitions.Alliance, MagicRanged);
    public static readonly ShopItemType ShopRangedPlus2 = new(FactionStatusDefinitions.Brotherhood, MagicRanged);
    public static readonly ShopItemType ShopCrafting = new(FactionStatusDefinitions.Alliance, CraftingManual);

    public static readonly List<string> PolearmWeaponTypes = new()
    {
        WeaponTypeDefinitions.QuarterstaffType.Name, WeaponTypeDefinitions.SpearType.Name
    };

    private static readonly List<(ItemDefinition, ShopItemType)> ShopItems = new();
    private static StockUnitDescriptionBuilder _stockBuilder;
    private static StockUnitDescriptionBuilder StockBuilder => _stockBuilder ??= BuildStockBuilder();

    public static void Load()
    {
        BuildHandwraps();
        BuildHalberds();
        BuildPikes();
        BuildLongMaces();
        BuildHandXbow();
        WeaponizeProducedFlame();
        AddToShops();
        AddToEditor();

        PolearmWeaponTypes.AddRange(new[] {HalberdWeaponType.Name, PikeWeaponType.Name, LongMaceWeaponType.Name});
    }

    public static ItemPresentation BuildPresentation(string unIdentifiedName, ItemPresentation basePresentation,
        float scale = 1.0f, bool hasUnidDescription = false)
    {
        var presentation = new ItemPresentation(basePresentation);
        presentation.ItemFlags.Clear();
        presentation.assetReference = basePresentation.AssetReference;
        presentation.unidentifiedTitle = GuiPresentationBuilder.CreateTitleKey(unIdentifiedName, Category.Item);
        presentation.unidentifiedDescription = hasUnidDescription
            ? GuiPresentationBuilder.CreateDescriptionKey(unIdentifiedName, Category.Item)
            : Gui.NoLocalization;

        presentation.scaleFactorWhileWielded = scale;
        return presentation;
    }

    public static ItemDefinition BuildWeapon(string name, ItemDefinition baseItem, int goldCost, bool noDescription,
        RuleDefinitions.ItemRarity rarity,
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
            .Create(baseItem, name, DefinitionBuilder.CENamespaceGuid)
            .SetGold(goldCost)
            .SetMerchantCategory(MerchantCategoryDefinitions.Weapon)
            .SetStaticProperties(properties)
            .SetWeaponDescription(baseDescription)
            .SetItemPresentation(BuildPresentation($"{name}Unidentified", basePresentation, scale))
            .SetItemRarity(rarity);

        if (twoHanded)
        {
            builder.SetSlotTypes(SlotTypeDefinitions.MainHandSlot, SlotTypeDefinitions.ContainerSlot)
                .SetSlotsWhereActive(SlotTypeDefinitions.MainHandSlot);
        }
        else
        {
            builder.SetSlotTypes(SlotTypeDefinitions.MainHandSlot, SlotTypeDefinitions.OffHandSlot,
                    SlotTypeDefinitions.ContainerSlot)
                .SetSlotsWhereActive(SlotTypeDefinitions.MainHandSlot, SlotTypeDefinitions.OffHandSlot);
        }

        if (!properties.Empty())
        {
            builder.MakeMagical();
            if (needId)
            {
                builder.SetRequiresIdentification(true);
            }
        }

        if (noDescription)
        {
            builder.SetGuiPresentation(Category.Item, Gui.NoLocalization, icon);
        }
        else
        {
            builder.SetGuiPresentation(Category.Item, icon);
        }


        var weapon = builder.AddToDB();

        //TODO: add to editor only if option turned on
        weapon.inDungeonEditor = true;

        return weapon;
    }

    private static void BuildHandwraps()
    {
        ItemDefinitions.UnarmedStrikeBase.WeaponDescription.WeaponTags.Add(TagsDefinitions.WeaponTagLight);

        HandwrapsPlus1 = BuildHandwrapsCommon("Handwraps+1", 400, true, true, Uncommon, WeaponPlus1);
        HandwrapsPlus2 = BuildHandwrapsCommon("Handwraps+2", 1500, true, true, Rare, WeaponPlus2);

        ShopItems.Add((HandwrapsPlus1, ShopMeleePlus1));
        ShopItems.Add((HandwrapsPlus2, ShopMeleePlus2));

        HandwrapsOfForce = BuildHandwrapsCommon("HandwrapsOfForce", 2000, true, false, Rare, ForceImpactVFX,
            WeaponPlus1AttackOnly);
        HandwrapsOfForce.WeaponDescription.EffectDescription.AddEffectForms(new EffectFormBuilder()
            .SetDamageForm(diceNumber: 1, dieType: RuleDefinitions.DieType.D4,
                damageType: RuleDefinitions.DamageTypeForce)
            .Build());

        HandwrapsOfPulling = BuildHandwrapsCommon("HandwrapsOfPulling", 2000, true, false, Rare, WeaponPlus1AttackOnly);
        HandwrapsOfPulling.IsUsableDevice = true;
        HandwrapsOfPulling.usableDeviceDescription = new UsableDeviceDescriptionBuilder()
            .SetRecharge(RuleDefinitions.RechargeRate.AtWill)
            .SetSaveDC(-1)
            .AddFunctions(new DeviceFunctionDescriptionBuilder()
                .SetPower(FeatureDefinitionPowerBuilder
                    .Create("PowerFunctionHandwrapsOfPulling", Monk.Guid)
                    .SetGuiPresentation(Category.Power)
                    .SetActivationTime(RuleDefinitions.ActivationTime.BonusAction)
                    .SetUsesFixed(1)
                    .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill)
                    .SetEffectDescription(new EffectDescriptionBuilder()
                        .SetTargetingData(RuleDefinitions.Side.All, RuleDefinitions.RangeType.Distance, 3,
                            RuleDefinitions.TargetType.Individuals)
                        .ExcludeCaster()
                        .SetSavingThrowData(
                            true,
                            true,
                            AttributeDefinitions.Strength,
                            false,
                            RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                            AttributeDefinitions.Wisdom)
                        .SetParticleEffectParameters(FeatureDefinitionPowers.PowerShadowTamerRopeGrapple
                            .EffectDescription.EffectParticleParameters)
                        .SetDurationData(RuleDefinitions.DurationType.Instantaneous)
                        .SetEffectForms(new EffectFormBuilder()
                            .SetMotionForm(MotionForm.MotionType.DragToOrigin, 2)
                            .Build())
                        .Build())
                    .AddToDB())
                .Build())
            .Build();

        ShopItems.Add((BuildManual(BuildRecipe(HandwrapsPlus1, 24, 10, Monk.Guid,
            ItemDefinitions.Ingredient_Enchant_Oil_Of_Acuteness), Monk.Guid), ShopCrafting));

        ShopItems.Add((BuildManual(BuildRecipe(HandwrapsPlus2, 48, 16, Monk.Guid,
            ItemDefinitions.Ingredient_Enchant_Blood_Gem), Monk.Guid), ShopCrafting));

        ShopItems.Add((BuildManual(BuildRecipe(HandwrapsOfForce, 48, 16, Monk.Guid,
            ItemDefinitions.Ingredient_Enchant_Soul_Gem), Monk.Guid), ShopCrafting));

        ShopItems.Add((BuildManual(BuildRecipe(HandwrapsOfPulling, 48, 16, Monk.Guid,
            ItemDefinitions.Ingredient_Enchant_Slavestone), Monk.Guid), ShopCrafting));
    }

    private static ItemDefinition BuildHandwrapsCommon(string name, int goldCost, bool noDescription, bool needId,
        RuleDefinitions.ItemRarity rarity,
        params ItemPropertyDescription[] properties)
    {
        return BuildWeapon(
            name,
            ItemDefinitions.UnarmedStrikeBase,
            goldCost,
            noDescription, rarity, needId: needId,
            properties: properties
        );
    }

    private static void BuildHalberds()
    {
        var scale = new CustomScale(z: 3.5f);
        HalberdWeaponType = WeaponTypeDefinitionBuilder
            .Create(WeaponTypeDefinitions.GreataxeType, "CEHalberdType", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Equipment, Gui.NoLocalization)
            .SetWeaponCategory(WeaponCategoryDefinitions.MartialWeaponCategory)
            .AddToDB();
        var baseItem = ItemDefinitions.Greataxe;
        var basePresentation = ItemDefinitions.Battleaxe.ItemPresentation;
        var baseDescription = new WeaponDescription(baseItem.WeaponDescription)
        {
            reachRange = 2,
            weaponType = HalberdWeaponType.Name,
            weaponTags = new List<string>
            {
                TagsDefinitions.WeaponTagHeavy,
                TagsDefinitions.WeaponTagReach,
                TagsDefinitions.WeaponTagTwoHanded
            }
        };
        var damageForm = baseDescription.EffectDescription
            .GetFirstFormOfType(EffectForm.EffectFormType.Damage).DamageForm;
        damageForm.dieType = RuleDefinitions.DieType.D10;
        damageForm.diceNumber = 1;

        Halberd = BuildWeapon("CEHalberd", baseItem,
            20, true, Common, basePresentation, baseDescription, HalberdIcon);
        Halberd.SetCustomSubFeatures(scale);
        ShopItems.Add((Halberd, ShopGenericMelee));

        HalberdPrimed = BuildWeapon("CEHalberdPrimed", baseItem,
            40, true, Uncommon, basePresentation, baseDescription, HalberdPrimedIcon);
        HalberdPrimed.ItemTags.Add(TagsDefinitions.ItemTagIngredient);
        HalberdPrimed.ItemTags.Remove(TagsDefinitions.ItemTagStandard);
        HalberdPrimed.SetCustomSubFeatures(scale);
        ShopItems.Add((HalberdPrimed, ShopPrimedMelee));
        ShopItems.Add((BuildPrimingManual(Halberd, HalberdPrimed), ShopCrafting));

        HalberdPlus1 = BuildWeapon("CEHalberd+1", Halberd,
            950, true, Rare, icon: HalberdP1Icon, properties: new[] {WeaponPlus1});
        HalberdPlus1.SetCustomSubFeatures(scale);
        ShopItems.Add((HalberdPlus1, ShopMeleePlus1));
        ShopItems.Add((BuildRecipeManual(HalberdPlus1, 24, 10,
                HalberdPrimed,
                ItemDefinitions.Ingredient_Enchant_Oil_Of_Acuteness),
            ShopCrafting));

        var itemDefinition = ItemDefinitions.BattleaxePlus1;
        HalberdPlus2 = BuildWeapon("CEHalberd+2", Halberd,
            2500, true, VeryRare,
            itemDefinition.ItemPresentation, icon: HalberdP2Icon,
            properties: new[] {WeaponPlus2});
        HalberdPlus2.SetCustomSubFeatures(scale);
        ShopItems.Add((HalberdPlus2, ShopMeleePlus2));
        ShopItems.Add((BuildRecipeManual(HalberdPlus2, 48, 16,
                HalberdPrimed,
                ItemDefinitions.Ingredient_Enchant_Blood_Gem),
            ShopCrafting));

        HalberdLightning = BuildWeapon("CEHalberdLightning", Halberd,
            2500, true, VeryRare,
            itemDefinition.ItemPresentation, icon: HalberdLightningIcon, needId: false,
            properties: new[] {LightningImpactVFX, WeaponPlus1AttackOnly});
        HalberdLightning.SetCustomSubFeatures(scale);
        HalberdLightning.WeaponDescription.EffectDescription.AddEffectForms(new EffectFormBuilder()
            .SetDamageForm(diceNumber: 1, dieType: RuleDefinitions.DieType.D8,
                damageType: RuleDefinitions.DamageTypeLightning)
            .Build());
        ShopItems.Add((BuildRecipeManual(HalberdLightning, 48, 16,
                HalberdPrimed,
                ItemDefinitions.Ingredient_Enchant_Stardust),
            ShopCrafting));
    }

    private static void BuildPikes()
    {
        var scale = new CustomScale(z: 3.5f);
        PikeWeaponType = WeaponTypeDefinitionBuilder
            .Create(WeaponTypeDefinitions.SpearType, "CEPikeType", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Equipment, Gui.NoLocalization)
            .SetWeaponCategory(WeaponCategoryDefinitions.MartialWeaponCategory)
            .AddToDB();
        var baseItem = ItemDefinitions.Spear;
        var basePresentation = ItemDefinitions.Morningstar.ItemPresentation;
        var baseDescription = new WeaponDescription(baseItem.WeaponDescription)
        {
            reachRange = 2,
            weaponType = PikeWeaponType.Name,
            weaponTags = new List<string>
            {
                TagsDefinitions.WeaponTagHeavy,
                TagsDefinitions.WeaponTagReach,
                TagsDefinitions.WeaponTagTwoHanded
            }
        };
        var damageForm = baseDescription.EffectDescription
            .GetFirstFormOfType(EffectForm.EffectFormType.Damage).DamageForm;
        damageForm.dieType = RuleDefinitions.DieType.D10;
        damageForm.diceNumber = 1;

        Pike = BuildWeapon("CEPike", baseItem,
            20, true, Common, basePresentation, baseDescription, PikeIcon);
        Pike.SetCustomSubFeatures(scale);
        ShopItems.Add((Pike, ShopGenericMelee));

        PikePrimed = BuildWeapon("CEPikePrimed", baseItem,
            40, true, Uncommon, basePresentation, baseDescription, PikePrimedIcon);
        PikePrimed.ItemTags.Add(TagsDefinitions.ItemTagIngredient);
        PikePrimed.ItemTags.Remove(TagsDefinitions.ItemTagStandard);
        PikePrimed.SetCustomSubFeatures(scale);
        ShopItems.Add((PikePrimed, ShopPrimedMelee));
        ShopItems.Add((BuildPrimingManual(Pike, PikePrimed), ShopCrafting));

        PikePlus1 = BuildWeapon("CEPike+1", Pike,
            950, true, Rare, icon: PikeP1Icon, properties: new[] {WeaponPlus1});
        PikePlus1.SetCustomSubFeatures(scale);
        ShopItems.Add((PikePlus1, ShopMeleePlus1));
        ShopItems.Add((BuildRecipeManual(PikePlus1, 24, 10,
                PikePrimed,
                ItemDefinitions.Ingredient_Enchant_Oil_Of_Acuteness),
            ShopCrafting));

        var itemDefinition = ItemDefinitions.MorningstarPlus2;
        PikePlus2 = BuildWeapon("CEPike+2", Pike,
            2500, true, VeryRare,
            itemDefinition.ItemPresentation,
            icon: PikeP2Icon,
            properties: new[] {WeaponPlus2});
        PikePlus2.SetCustomSubFeatures(scale);
        ShopItems.Add((PikePlus2, ShopMeleePlus2));
        ShopItems.Add((BuildRecipeManual(PikePlus2, 48, 16,
                PikePrimed,
                ItemDefinitions.Ingredient_Enchant_Blood_Gem),
            ShopCrafting));

        PikePsychic = BuildWeapon("CEPikePsychic", Pike,
            2500, true, VeryRare,
            itemDefinition.ItemPresentation,
            icon: PikePsychicIcon, needId: false,
            properties: new[] {PsychicImpactVFX, WeaponPlus1AttackOnly});
        PikePsychic.SetCustomSubFeatures(scale);
        PikePsychic.WeaponDescription.EffectDescription.AddEffectForms(new EffectFormBuilder()
            .SetDamageForm(diceNumber: 1, dieType: RuleDefinitions.DieType.D8,
                damageType: RuleDefinitions.DamageTypePsychic)
            .Build());
        ShopItems.Add((BuildRecipeManual(PikePsychic, 48, 16,
                PikePrimed,
                ItemDefinitions.Ingredient_Enchant_Stardust),
            ShopCrafting));
    }

    private static void BuildLongMaces()
    {
        var scale = new CustomScale(z: 3.5f);
        LongMaceWeaponType = WeaponTypeDefinitionBuilder
            .Create(WeaponTypeDefinitions.MaulType, "CELongMaceType", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Equipment, Gui.NoLocalization)
            .SetWeaponCategory(WeaponCategoryDefinitions.MartialWeaponCategory)
            .AddToDB();
        var baseItem = ItemDefinitions.Mace;
        var basePresentation = ItemDefinitions.Mace.ItemPresentation;
        var baseDescription = new WeaponDescription(baseItem.WeaponDescription)
        {
            reachRange = 2,
            weaponType = LongMaceWeaponType.Name,
            weaponTags = new List<string>
            {
                TagsDefinitions.WeaponTagHeavy,
                TagsDefinitions.WeaponTagReach,
                TagsDefinitions.WeaponTagTwoHanded
            }
        };
        var damageForm = baseDescription.EffectDescription
            .GetFirstFormOfType(EffectForm.EffectFormType.Damage).DamageForm;
        damageForm.dieType = RuleDefinitions.DieType.D10;
        damageForm.diceNumber = 1;

        LongMace = BuildWeapon("CELongMace", baseItem,
            20, true, Common, basePresentation, baseDescription, LongMaceIcon);
        LongMace.SetCustomSubFeatures(scale);
        ShopItems.Add((LongMace, ShopGenericMelee));

        LongMacePrimed = BuildWeapon("CELongMacePrimed", baseItem,
            40, true, Uncommon, basePresentation, baseDescription, LongMacePrimedIcon);
        LongMacePrimed.ItemTags.Add(TagsDefinitions.ItemTagIngredient);
        LongMacePrimed.ItemTags.Remove(TagsDefinitions.ItemTagStandard);
        LongMacePrimed.SetCustomSubFeatures(scale);
        ShopItems.Add((LongMacePrimed, ShopPrimedMelee));
        ShopItems.Add((BuildPrimingManual(LongMace, LongMacePrimed), ShopCrafting));

        LongMacePlus1 = BuildWeapon("CELongMace+1", LongMace,
            950, true, Rare, icon: LongMaceP1Icon, properties: new[] {WeaponPlus1});
        LongMacePlus1.SetCustomSubFeatures(scale);
        ShopItems.Add((LongMacePlus1, ShopMeleePlus1));
        ShopItems.Add((BuildRecipeManual(LongMacePlus1, 24, 10,
                LongMacePrimed,
                ItemDefinitions.Ingredient_Enchant_Oil_Of_Acuteness),
            ShopCrafting));

        var itemDefinition = ItemDefinitions.MacePlus2;
        LongMacePlus2 = BuildWeapon("CELongMace+2", LongMace,
            2500, true, VeryRare,
            itemDefinition.ItemPresentation, icon: LongMaceP2Icon,
            properties: new[] {WeaponPlus2});
        LongMacePlus2.SetCustomSubFeatures(scale);
        ShopItems.Add((LongMacePlus2, ShopMeleePlus2));
        ShopItems.Add((BuildRecipeManual(LongMacePlus2, 48, 16,
                LongMacePrimed,
                ItemDefinitions.Ingredient_Enchant_Blood_Gem),
            ShopCrafting));

        LongMaceThunder = BuildWeapon("CELongMaceThunder", LongMace,
            2500, true, VeryRare,
            itemDefinition.ItemPresentation, icon: LongMaceThunderIcon, needId: false,
            properties: new[] {ThunderImpactVFX, WeaponPlus1AttackOnly});
        LongMaceThunder.SetCustomSubFeatures(scale);
        LongMaceThunder.WeaponDescription.EffectDescription.AddEffectForms(new EffectFormBuilder()
            .SetDamageForm(diceNumber: 1, dieType: RuleDefinitions.DieType.D8,
                damageType: RuleDefinitions.DamageTypeThunder)
            .Build());
        ShopItems.Add((BuildRecipeManual(LongMaceThunder, 48, 16,
                LongMacePrimed,
                ItemDefinitions.Ingredient_Enchant_Stardust),
            ShopCrafting));
    }

    private static void BuildHandXbow()
    {
        var scale = new CustomScale(0.5f);
        HandXbowWeaponType = WeaponTypeDefinitionBuilder
            .Create(WeaponTypeDefinitions.LightCrossbowType, "CEHandXbowType", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Equipment, Gui.NoLocalization)
            .SetWeaponCategory(WeaponCategoryDefinitions.MartialWeaponCategory)
            .SetAnimationTag("Rapier")
            .AddToDB();
        var baseItem = ItemDefinitions.LightCrossbow;
        var basePresentation = new ItemPresentation(baseItem.ItemPresentation);
        var baseDescription = new WeaponDescription(baseItem.WeaponDescription)
        {
            weaponType = HandXbowWeaponType.Name,
            closeRange = 6,
            maxRange = 24,
            weaponTags = new List<string>
            {
                TagsDefinitions.WeaponTagLight,
                TagsDefinitions.WeaponTagRange,
                TagsDefinitions.WeaponTagLoading,
                TagsDefinitions.WeaponTagAmmunition
            }
        };
        var damageForm = baseDescription.EffectDescription
            .GetFirstFormOfType(EffectForm.EffectFormType.Damage).DamageForm;
        damageForm.dieType = RuleDefinitions.DieType.D6;
        damageForm.diceNumber = 1;

        //add hand xbow proficiency to rogues
        var rogueHandXbowProficiency = FeatureDefinitionProficiencys.ProficiencyRogueWeapon;
        rogueHandXbowProficiency.Proficiencies.Add(HandXbowWeaponType.Name);

        HandXbow = BuildWeapon("CEHandXbow", baseItem,
            20, true, Common, basePresentation, baseDescription, HandXbowIcon,
            twoHanded: false);
        HandXbow.SetCustomSubFeatures(scale);
        ShopItems.Add((HandXbow, ShopGenericRanged));

        HandXbowPrimed = BuildWeapon("CEHandXbowPrimed", HandXbow,
            40, true, Uncommon, icon: HandXbowPrimedIcon, twoHanded: false);
        HandXbowPrimed.SetCustomSubFeatures(scale);
        HandXbowPrimed.ItemTags.Add(TagsDefinitions.ItemTagIngredient);
        HandXbowPrimed.ItemTags.Remove(TagsDefinitions.ItemTagStandard);
        ShopItems.Add((HandXbowPrimed, ShopPrimedRanged));
        ShopItems.Add((BuildPrimingManual(HandXbow, HandXbowPrimed), ShopCrafting));

        HandXbowPlus1 = BuildWeapon("CEHandXbow+1", HandXbow,
            950, true, Rare, icon: HandXbowP1Icon, twoHanded: false,
            properties: new[] {WeaponPlus1});
        HandXbowPlus1.SetCustomSubFeatures(scale);
        ShopItems.Add((HandXbowPlus1, ShopRangedPlus1));
        ShopItems.Add((BuildRecipeManual(HandXbowPlus1, 24, 10,
                HandXbowPrimed,
                ItemDefinitions.Ingredient_Enchant_Oil_Of_Acuteness),
            ShopCrafting));

        var itemDefinition = ItemDefinitions.LightCrossbowPlus2;
        HandXbowPlus2 = BuildWeapon("CEHandXbow+2", HandXbow,
            2500, true, VeryRare,
            itemDefinition.ItemPresentation, icon: HandXbowP2Icon, twoHanded: false,
            properties: new[] {WeaponPlus2});
        HandXbowPlus2.SetCustomSubFeatures(scale);
        ShopItems.Add((HandXbowPlus2, ShopRangedPlus2));
        ShopItems.Add((BuildRecipeManual(HandXbowPlus2, 48, 16,
                HandXbowPrimed,
                ItemDefinitions.Ingredient_Enchant_Blood_Gem),
            ShopCrafting));

        HandXbowAcid = BuildWeapon("CEHandXbowAcid", HandXbow,
            2500, true, VeryRare,
            itemDefinition.ItemPresentation, icon: HandXbowAcidIcon, needId: false, twoHanded: false,
            properties: new[] {AcidImpactVFX, WeaponPlus1AttackOnly});
        HandXbowAcid.SetCustomSubFeatures(scale);
        HandXbowAcid.WeaponDescription.EffectDescription.AddEffectForms(new EffectFormBuilder()
            .SetDamageForm(diceNumber: 1, dieType: RuleDefinitions.DieType.D8,
                damageType: RuleDefinitions.DamageTypeAcid)
            .Build());
        ShopItems.Add((BuildRecipeManual(HandXbowAcid, 48, 16,
                HandXbowPrimed,
                ItemDefinitions.Ingredient_Enchant_Stardust),
            ShopCrafting));
    }

    private static void WeaponizeProducedFlame()
    {
        var flame = ItemDefinitions.ProducedFlame;
        flame.GuiPresentation = new GuiPresentationBuilder(flame.GuiPresentation)
            .SetTitle("Item/&CEProducedFlameTitle")
            .Build();

        ProducedFlameDart = BuildWeapon("CEProducedFlameDart", ItemDefinitions.Dart, 0, true, Common,
            flame.ItemPresentation, icon: ProducedFlameThrow);

        var damageForm = ProducedFlameDart.WeaponDescription.EffectDescription.FindFirstDamageForm();
        damageForm.damageType = RuleDefinitions.DamageTypeFire;
        damageForm.dieType = RuleDefinitions.DieType.D8;

        var weapon = new WeaponDescription(ItemDefinitions.UnarmedStrikeBase.weaponDefinition);
        weapon.EffectDescription.AddEffectForms(new EffectFormBuilder()
            .SetDamageForm(dieType: RuleDefinitions.DieType.D8, diceNumber: 1,
                damageType: RuleDefinitions.DamageTypeFire)
            .Build());
        flame.staticProperties.Add(BuildFrom(FeatureDefinitionBuilder
            .Create("CEProducedFlameThrower", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentationNoContent()
            .SetCustomSubFeatures(
                new ModifyProducedFlameDice(),
                new AddThrowProducedFlameAttack()
            )
            .AddToDB(), false, EquipmentDefinitions.KnowledgeAffinity.ActiveAndHidden));

        flame.IsWeapon = true;
        flame.weaponDefinition = weapon;
    }

    private static void AddToShops()
    {
        if (Main.Settings.AddNewWeaponsAndRecipesToShops)
        {
            GiveAssortment(ShopItems, MerchantTypeContext.MerchantTypes);
        }
    }

    public static void TryAddItemsToUserMerchant(MerchantDefinition merchant)
    {
        if (Main.Settings.AddNewWeaponsAndRecipesToShops)
        {
            GiveAssortment(ShopItems, merchant, MerchantTypeContext.GetMerchantType(merchant));
        }
    }

    private static void AddToEditor()
    {
        if (Main.Settings.AddNewWeaponsAndRecipesToEditor)
        {
            foreach (var (item, _) in ShopItems)
            {
                item.inDungeonEditor = true;
            }
        }
    }

    //TODO: move this to the separate shop context file
    private static void GiveAssortment(List<(ItemDefinition, ShopItemType)> items,
        ICollection<(MerchantDefinition, MerchantTypeContext.MerchantType)> merchants)
    {
        foreach (var (merchant, type) in merchants)
        {
            GiveAssortment(items, merchant, type);
        }
    }

    private static void GiveAssortment(List<(ItemDefinition, ShopItemType)> items, MerchantDefinition merchant,
        MerchantTypeContext.MerchantType type)
    {
        foreach (var (item, itemType) in items)
        {
            if (itemType.filter.Matches(type))
            {
                StockItem(merchant, item, itemType.status);
            }
        }
    }

    private static void StockItem(MerchantDefinition merchant, ItemDefinition item, FactionStatusDefinition status)
    {
        merchant.StockUnitDescriptions.Add(StockBuilder
            .SetItem(item)
            .SetFaction(merchant.FactionAffinity, status.Name)
            .Build()
        );
    }

    private static StockUnitDescriptionBuilder BuildStockBuilder()
    {
        return new StockUnitDescriptionBuilder()
            .SetStock(initialAmount: 1)
            .SetRestock(1);
    }

    public static RecipeDefinition BuildRecipe(ItemDefinition item, int hours, int difficulty,
        params ItemDefinition[] ingredients)
    {
        return BuildRecipe(item, hours, difficulty, DefinitionBuilder.CENamespaceGuid, ingredients);
    }

    public static RecipeDefinition BuildRecipe(ItemDefinition item, int hours, int difficulty, Guid guid,
        params ItemDefinition[] ingredients)
    {
        return RecipeDefinitionBuilder
            .Create($"RecipeEnchant{item.Name}", guid)
            .SetGuiPresentation(item.GuiPresentation.Title, GuiPresentationBuilder.EmptyString)
            .SetCraftedItem(item)
            .SetCraftingCheckData(hours, difficulty, ToolTypeDefinitions.EnchantingToolType)
            .AddIngredients(ingredients)
            .AddToDB();
    }

    public static ItemDefinition BuildRecipeManual(ItemDefinition item, int hours, int difficulty,
        params ItemDefinition[] ingredients)
    {
        return BuildManual(BuildRecipe(item, hours, difficulty, DefinitionBuilder.CENamespaceGuid, ingredients),
            DefinitionBuilder.CENamespaceGuid);
    }

    public static ItemDefinition BuildManual(RecipeDefinition recipe)
    {
        return BuildManual(recipe, DefinitionBuilder.CENamespaceGuid);
    }

    public static ItemDefinition BuildManual(RecipeDefinition recipe, Guid guid)
    {
        var reference = ItemDefinitions.CraftingManualScrollOfVampiricTouch;
        var manual = ItemDefinitionBuilder
            .Create($"CraftingManual{recipe.Name}", guid)
            .SetGuiPresentation(Category.Item, reference.GuiPresentation.SpriteReference)
            .SetItemPresentation(reference.ItemPresentation)
            .SetMerchantCategory(MerchantCategoryDefinitions.Crafting)
            .SetSlotTypes(SlotTypeDefinitions.ContainerSlot)
            .SetItemTags(TagsDefinitions.ItemTagStandard, TagsDefinitions.ItemTagPaper)
            .SetDocumentInformation(recipe, reference.DocumentDescription.ContentFragments)
            .SetGold(Main.Settings.RecipeCost)
            .AddToDB();

        //TODO: add only if option enabled in mod settings
        manual.inDungeonEditor = true;

        return manual;
    }

    public static ItemDefinition BuildPrimingManual(ItemDefinition item, ItemDefinition primed, Guid guid)
    {
        return BuildManual(ItemRecipeGenerationHelper.CreatePrimingRecipe(guid, item, primed), guid);
    }

    public static ItemDefinition BuildPrimingManual(ItemDefinition item, ItemDefinition primed)
    {
        return BuildPrimingManual(item, primed, DefinitionBuilder.CENamespaceGuid);
    }

    public static void ProcessProducedFlameAttack(RulesetCharacterHero hero, RulesetAttackMode mode)
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

    #region Halberd Icons

    private static AssetReferenceSprite _halberdIcon,
        _halberdPrimedIcon,
        _halberdP1Icon,
        _halberdP2Icon,
        _halberdLightningIcon;

    private static AssetReferenceSprite HalberdIcon =>
        _halberdIcon ??= CustomIcons.CreateAssetReferenceSprite("Halberd", Resources.Halberd, 128);

    private static AssetReferenceSprite HalberdPrimedIcon => _halberdPrimedIcon ??=
        CustomIcons.CreateAssetReferenceSprite("HalberdPrimed", Resources.HalberdPrimed, 128);

    private static AssetReferenceSprite HalberdP1Icon => _halberdP1Icon ??=
        CustomIcons.CreateAssetReferenceSprite("Halberd_1", Resources.Halberd_1, 128);

    private static AssetReferenceSprite HalberdP2Icon => _halberdP2Icon ??=
        CustomIcons.CreateAssetReferenceSprite("Halberd_2", Resources.Halberd_2, 128);

    private static AssetReferenceSprite HalberdLightningIcon => _halberdLightningIcon ??=
        CustomIcons.CreateAssetReferenceSprite("HalberdLightning", Resources.HalberdLightning, 128);

    #endregion

    #region Pike Icons

    private static AssetReferenceSprite _pikeIcon,
        _pikePrimedIcon,
        _pikeP1Icon,
        _pikeP2Icon,
        _pikeLightningIcon;

    private static AssetReferenceSprite PikeIcon =>
        _pikeIcon ??= CustomIcons.CreateAssetReferenceSprite("Pike", Resources.Pike, 128);

    private static AssetReferenceSprite PikePrimedIcon => _pikePrimedIcon ??=
        CustomIcons.CreateAssetReferenceSprite("PikePrimed", Resources.PikePrimed, 128);

    private static AssetReferenceSprite PikeP1Icon => _pikeP1Icon ??=
        CustomIcons.CreateAssetReferenceSprite("Pike_1", Resources.Pike_1, 128);

    private static AssetReferenceSprite PikeP2Icon => _pikeP2Icon ??=
        CustomIcons.CreateAssetReferenceSprite("Pike_2", Resources.Pike_2, 128);

    private static AssetReferenceSprite PikePsychicIcon => _pikeLightningIcon ??=
        CustomIcons.CreateAssetReferenceSprite("PikePsychic", Resources.PikePsychic, 128);

    #endregion

    #region Long Mace Icons

    private static AssetReferenceSprite _longMaceIcon,
        _longMacePrimedIcon,
        _longMaceP1Icon,
        _longMaceP2Icon,
        _longMaceLightningIcon;

    private static AssetReferenceSprite LongMaceIcon =>
        _longMaceIcon ??= CustomIcons.CreateAssetReferenceSprite("LongMace", Resources.LongMace, 128);

    private static AssetReferenceSprite LongMacePrimedIcon => _longMacePrimedIcon ??=
        CustomIcons.CreateAssetReferenceSprite("LongMacePrimed", Resources.LongMacePrimed, 128);

    private static AssetReferenceSprite LongMaceP1Icon => _longMaceP1Icon ??=
        CustomIcons.CreateAssetReferenceSprite("LongMace_1", Resources.LongMace_1, 128);

    private static AssetReferenceSprite LongMaceP2Icon => _longMaceP2Icon ??=
        CustomIcons.CreateAssetReferenceSprite("LongMace_2", Resources.LongMace_2, 128);

    private static AssetReferenceSprite LongMaceThunderIcon => _longMaceLightningIcon ??=
        CustomIcons.CreateAssetReferenceSprite("LongMaceThunder", Resources.LongMaceThunder, 128);

    #endregion

    #region Hand Crossbow Icons

    private static AssetReferenceSprite _handXbowIcon,
        _handXbowPrimedIcon,
        _handXbowP1Icon,
        _handXbowP2Icon,
        _handXbowAcidIcon;

    private static AssetReferenceSprite HandXbowIcon =>
        _handXbowIcon ??= CustomIcons.CreateAssetReferenceSprite("HandXbow", Resources.HandXbow, 128);

    private static AssetReferenceSprite HandXbowPrimedIcon => _handXbowPrimedIcon ??=
        CustomIcons.CreateAssetReferenceSprite("HandXbowPrimed", Resources.HandXbowPrimed, 128);

    private static AssetReferenceSprite HandXbowP1Icon => _handXbowP1Icon ??=
        CustomIcons.CreateAssetReferenceSprite("HandXbow_1", Resources.HandXbow_1, 128);

    private static AssetReferenceSprite HandXbowP2Icon => _handXbowP2Icon ??=
        CustomIcons.CreateAssetReferenceSprite("HandXbow_2", Resources.HandXbow_2, 128);

    private static AssetReferenceSprite HandXbowAcidIcon => _handXbowAcidIcon ??=
        CustomIcons.CreateAssetReferenceSprite("HandXbowAcid", Resources.HandXbowAcid, 128);

    #endregion

    #region Produced Flame Icons

    private static AssetReferenceSprite _producedFlameThrow;

    private static AssetReferenceSprite ProducedFlameThrow => _producedFlameThrow ??=
        CustomIcons.CreateAssetReferenceSprite("ProducedFlameThrow", Resources.ProducedFlameThrow, 128);

    #endregion
}

//TODO: move this to the separate shop context file
public class ShopItemType
{
    public readonly MerchantFilter filter;
    public readonly FactionStatusDefinition status;

    public ShopItemType(FactionStatusDefinition status, MerchantFilter filter)
    {
        this.status = status;
        this.filter = filter;
    }
}

public class MerchantFilter
{
    public bool? IsAmmunition = null;
    public bool? IsArmor = null;
    public bool? IsDocument;

    public bool? IsMagicalAmmunition = null;
    public bool? IsMagicalArmor = null;
    public bool? IsMagicalMeleeWeapon;
    public bool? IsMagicalRangeWeapon;
    public bool? IsMeleeWeapon;

    public bool? IsPrimedArmor = null;
    public bool? IsPrimedMeleeWeapon;
    public bool? IsPrimedRangeWeapon;
    public bool? IsRangeWeapon;

    public bool Matches(MerchantTypeContext.MerchantType merchantType)
    {
        return (IsAmmunition == null || IsAmmunition == merchantType.IsAmmunition) &&
               (IsArmor == null || IsArmor == merchantType.IsArmor) &&
               (IsDocument == null || IsDocument == merchantType.IsDocument) &&
               (IsMagicalAmmunition == null || IsMagicalAmmunition == merchantType.IsMagicalAmmunition) &&
               (IsMagicalArmor == null || IsMagicalArmor == merchantType.IsMagicalArmor) &&
               (IsMagicalMeleeWeapon == null || IsMagicalMeleeWeapon == merchantType.IsMagicalMeleeWeapon) &&
               (IsMagicalRangeWeapon == null || IsMagicalRangeWeapon == merchantType.IsMagicalRangeWeapon) &&
               (IsMeleeWeapon == null || IsMeleeWeapon == merchantType.IsMeleeWeapon) &&
               (IsPrimedArmor == null || IsPrimedArmor == merchantType.IsPrimedArmor) &&
               (IsPrimedMeleeWeapon == null || IsPrimedMeleeWeapon == merchantType.IsPrimedMeleeWeapon) &&
               (IsPrimedRangeWeapon == null || IsPrimedRangeWeapon == merchantType.IsPrimedRangeWeapon) &&
               (IsRangeWeapon == null || IsRangeWeapon == merchantType.IsRangeWeapon);
    }
}

public class ModifyProducedFlameDice : ModifyAttackModeForWeaponBase
{
    public ModifyProducedFlameDice() : base((_, weapon, _) =>
        weapon != null && weapon.ItemDefinition == ItemDefinitions.ProducedFlame)
    {
    }

    protected override void TryModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode,
        RulesetItem weapon)
    {
        var damage = attackMode.EffectDescription.FindLastDamageForm();
        if (damage != null)
        {
            var casterLevel = character.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;
            damage.diceNumber = 1 + RuleDefinitions.SpellAdvancementByCasterLevel[casterLevel - 1];
        }
    }
}

public class AddThrowProducedFlameAttack : AddExtraAttackBase
{
    public AddThrowProducedFlameAttack() : base(ActionDefinitions.ActionType.Main, false)
    {
    }

    protected override List<RulesetAttackMode> GetAttackModes(RulesetCharacterHero hero)
    {
        var result = new List<RulesetAttackMode>();
        AddItemAttack(result, EquipmentDefinitions.SlotTypeMainHand, hero);
        AddItemAttack(result, EquipmentDefinitions.SlotTypeOffHand, hero);
        return result;
    }

    private void AddItemAttack(List<RulesetAttackMode> attackModes, string slot, RulesetCharacterHero hero)
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

public class CustomScale
{
    public readonly float X, Y, Z;

    public CustomScale(float s) : this(s, s, s)
    {
    }

    public CustomScale(float x = 1f, float y = 1f, float z = 1f)
    {
        X = x;
        Y = y;
        Z = z;
    }
}
