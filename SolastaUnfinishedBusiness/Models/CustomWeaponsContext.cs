using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.ItemCrafting;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Utils;
using UnityEngine.AddressableAssets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Models.ItemPropertyDescriptionsContext;
using static RuleDefinitions.ItemRarity;

namespace SolastaUnfinishedBusiness.Models;

internal static class CustomWeaponsContext
{
    internal const string CeHandXbowType = "CEHandXbowType";
    private const string PolearmWeaponTag = "PolearmWeapon";
    internal static WeaponTypeDefinition HalberdWeaponType, PikeWeaponType, LongMaceWeaponType, HandXbowWeaponType;
    internal static ItemDefinition HandwrapsPlus1, HandwrapsPlus2, HandwrapsOfForce, HandwrapsOfPulling;
    internal static ItemDefinition Halberd, HalberdPrimed, HalberdPlus1, HalberdPlus2, HalberdLightning;
    internal static ItemDefinition Pike, PikePrimed, PikePlus1, PikePlus2, PikePsychic;
    internal static ItemDefinition LongMace, LongMacePrimed, LongMacePlus1, LongMacePlus2, LongMaceThunder;
    internal static ItemDefinition HandXbow, HandXbowPrimed, HandXbowPlus1, HandXbowPlus2, HandXbowAcid;
    internal static ItemDefinition ProducedFlameDart;

    internal static readonly MerchantFilter GenericMelee = new() { IsMeleeWeapon = true };
    internal static readonly MerchantFilter MagicMelee = new() { IsMagicalMeleeWeapon = true };
    internal static readonly MerchantFilter PrimedMelee = new() { IsPrimedMeleeWeapon = true };
    internal static readonly MerchantFilter GenericRanged = new() { IsRangeWeapon = true };
    internal static readonly MerchantFilter MagicRanged = new() { IsMagicalRangeWeapon = true };
    internal static readonly MerchantFilter PrimedRanged = new() { IsPrimedRangeWeapon = true };
    internal static readonly MerchantFilter CraftingManual = new() { IsDocument = true };

    internal static readonly ShopItemType ShopGenericMelee = new(FactionStatusDefinitions.Indifference, GenericMelee);
    internal static readonly ShopItemType ShopPrimedMelee = new(FactionStatusDefinitions.Sympathy, PrimedMelee);
    internal static readonly ShopItemType ShopMeleePlus1 = new(FactionStatusDefinitions.Alliance, MagicMelee);
    internal static readonly ShopItemType ShopMeleePlus2 = new(FactionStatusDefinitions.Brotherhood, MagicMelee);
    internal static readonly ShopItemType ShopGenericRanged = new(FactionStatusDefinitions.Indifference, GenericRanged);
    internal static readonly ShopItemType ShopPrimedRanged = new(FactionStatusDefinitions.Sympathy, PrimedRanged);
    internal static readonly ShopItemType ShopRangedPlus1 = new(FactionStatusDefinitions.Alliance, MagicRanged);
    internal static readonly ShopItemType ShopRangedPlus2 = new(FactionStatusDefinitions.Brotherhood, MagicRanged);
    internal static readonly ShopItemType ShopCrafting = new(FactionStatusDefinitions.Alliance, CraftingManual);

    internal static readonly List<string> PolearmWeaponTypes = new()
    {
        WeaponTypeDefinitions.QuarterstaffType.Name, WeaponTypeDefinitions.SpearType.Name
    };

    private static readonly List<(ItemDefinition, ShopItemType)> ShopItems = new();
    private static StockUnitDescriptionBuilder _stockBuilder;
    private static StockUnitDescriptionBuilder StockBuilder => _stockBuilder ??= BuildStockBuilder();

    internal static void Load()
    {
        BuildHandwraps();
        BuildHalberds();
        BuildPikes();
        BuildLongMaces();
        BuildHandXbow();
        WeaponizeProducedFlame();
        AddToShops();
        AddToEditor();

        PolearmWeaponTypes.AddRange(new[] { HalberdWeaponType.Name, PikeWeaponType.Name, LongMaceWeaponType.Name });
    }

    [NotNull]
    internal static ItemPresentation BuildPresentation(
        string unIdentifiedName,
        [NotNull] ItemPresentation basePresentation,
        float scale = 1.0f, bool hasUnidentifiedDescription = false)
    {
        //TODO: either create a builder for ItemPresentation, or add setter with custom values to ItemDefinitionBuilder
        var presentation = new ItemPresentation(basePresentation);

        presentation.ItemFlags.Clear();
        presentation.assetReference = basePresentation.AssetReference;
        presentation.unidentifiedTitle = GuiPresentationBuilder.CreateTitleKey(unIdentifiedName, Category.Item);
        presentation.unidentifiedDescription = hasUnidentifiedDescription
            ? GuiPresentationBuilder.CreateDescriptionKey(unIdentifiedName, Category.Item)
            : Gui.NoLocalization;

        presentation.scaleFactorWhileWielded = scale;

        return presentation;
    }

    [NotNull]
    private static ItemDefinition BuildWeapon(string name, ItemDefinition baseItem, int goldCost, bool noDescription,
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
            .Create(baseItem, name)
            .SetGold(goldCost)
            .SetMerchantCategory(MerchantCategoryDefinitions.Weapon)
            .SetStaticProperties(properties)
            .SetWeaponDescription(baseDescription)
            .SetItemPresentation(BuildPresentation($"{name}Unidentified", basePresentation, scale))
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

        if (!properties.Empty())
        {
            _ = builder.MakeMagical();

            if (needId)
            {
                _ = builder.SetRequiresIdentification(true);
            }
        }

        if (noDescription)
        {
            _ = builder.SetGuiPresentation(Category.Item, Gui.NoLocalization, icon);
        }
        else
        {
            _ = builder.SetGuiPresentation(Category.Item, icon);
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
        HandwrapsOfForce.WeaponDescription.EffectDescription.effectForms.Add(EffectFormBuilder
            .Create()
            .SetDamageForm(RuleDefinitions.DamageTypeForce, 1, RuleDefinitions.DieType.D4)
            .Build());

        HandwrapsOfPulling = BuildHandwrapsCommon("HandwrapsOfPulling", 2000, true, false, Rare, WeaponPlus1AttackOnly);
        HandwrapsOfPulling.IsUsableDevice = true;
        HandwrapsOfPulling.usableDeviceDescription = new UsableDeviceDescriptionBuilder()
            .SetRecharge(RuleDefinitions.RechargeRate.AtWill)
            .SetSaveDc(EffectHelpers.BasedOnUser)
            .AddFunctions(new DeviceFunctionDescriptionBuilder()
                .SetPower(FeatureDefinitionPowerBuilder
                    .Create("PowerHandwrapsOfPulling")
                    .SetGuiPresentation(Category.Feature)
                    .SetActivationTime(RuleDefinitions.ActivationTime.BonusAction)
                    .SetUsesFixed(1)
                    .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill)
                    .SetEffectDescription(EffectDescriptionBuilder
                        .Create()
                        .SetTargetingData(RuleDefinitions.Side.All, RuleDefinitions.RangeType.Distance, 3,
                            RuleDefinitions.TargetType.Individuals)
                        .ExcludeCaster()
                        .SetSavingThrowData(
                            true,
                            AttributeDefinitions.Strength,
                            false,
                            RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency)
                        .SetParticleEffectParameters(FeatureDefinitionPowers.PowerShadowTamerRopeGrapple)
                        .SetDurationData(RuleDefinitions.DurationType.Instantaneous)
                        .SetEffectForms(EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.DragToOrigin, 2)
                            .Build())
                        .Build())
                    .AddToDB())
                .Build())
            .Build();

        ShopItems.Add((BuildManual(BuildRecipe(HandwrapsPlus1, 24, 10,
            ItemDefinitions.Ingredient_Enchant_Oil_Of_Acuteness)), ShopCrafting));

        ShopItems.Add((BuildManual(BuildRecipe(HandwrapsPlus2, 48, 16,
            ItemDefinitions.Ingredient_Enchant_Blood_Gem)), ShopCrafting));

        ShopItems.Add((BuildManual(BuildRecipe(HandwrapsOfForce, 48, 16,
            ItemDefinitions.Ingredient_Enchant_Soul_Gem)), ShopCrafting));

        ShopItems.Add((BuildManual(BuildRecipe(HandwrapsOfPulling, 48, 16,
            ItemDefinitions.Ingredient_Enchant_Slavestone)), ShopCrafting));
    }

    [NotNull]
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
            .Create(WeaponTypeDefinitions.GreataxeType, "CEHalberdType")
            .SetGuiPresentation(Category.Item, Gui.NoLocalization)
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
            950, true, Rare, icon: HalberdP1Icon, properties: new[] { WeaponPlus1 });
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
            properties: new[] { WeaponPlus2 });
        HalberdPlus2.SetCustomSubFeatures(scale);
        ShopItems.Add((HalberdPlus2, ShopMeleePlus2));
        ShopItems.Add((BuildRecipeManual(HalberdPlus2, 48, 16,
                HalberdPrimed,
                ItemDefinitions.Ingredient_Enchant_Blood_Gem),
            ShopCrafting));

        HalberdLightning = BuildWeapon("CEHalberdLightning", Halberd,
            2500, true, VeryRare,
            itemDefinition.ItemPresentation, icon: HalberdLightningIcon, needId: false,
            properties: new[] { LightningImpactVFX, WeaponPlus1AttackOnly });
        HalberdLightning.SetCustomSubFeatures(scale);
        HalberdLightning.WeaponDescription.EffectDescription.effectForms.Add(EffectFormBuilder
            .Create()
            .SetDamageForm(RuleDefinitions.DamageTypeLightning, 1, RuleDefinitions.DieType.D8)
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
            .Create(WeaponTypeDefinitions.SpearType, "CEPikeType")
            .SetGuiPresentation(Category.Item, Gui.NoLocalization)
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
            950, true, Rare, icon: PikeP1Icon, properties: new[] { WeaponPlus1 });
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
            properties: new[] { WeaponPlus2 });
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
            properties: new[] { PsychicImpactVFX, WeaponPlus1AttackOnly });
        PikePsychic.SetCustomSubFeatures(scale);
        PikePsychic.WeaponDescription.EffectDescription.effectForms.Add(EffectFormBuilder
            .Create()
            .SetDamageForm(RuleDefinitions.DamageTypePsychic, 1, RuleDefinitions.DieType.D8)
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
            .Create(WeaponTypeDefinitions.MaulType, "CELongMaceType")
            .SetGuiPresentation(Category.Item, Gui.NoLocalization)
            .SetWeaponCategory(WeaponCategoryDefinitions.MartialWeaponCategory)
            .AddToDB();

        var baseItem = ItemDefinitions.Warhammer;
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
            950, true, Rare, icon: LongMaceP1Icon, properties: new[] { WeaponPlus1 });
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
            properties: new[] { WeaponPlus2 });
        LongMacePlus2.SetCustomSubFeatures(scale);
        ShopItems.Add((LongMacePlus2, ShopMeleePlus2));
        ShopItems.Add((BuildRecipeManual(LongMacePlus2, 48, 16,
                LongMacePrimed,
                ItemDefinitions.Ingredient_Enchant_Blood_Gem),
            ShopCrafting));

        LongMaceThunder = BuildWeapon("CELongMaceThunder", LongMace,
            2500, true, VeryRare,
            itemDefinition.ItemPresentation, icon: LongMaceThunderIcon, needId: false,
            properties: new[] { ThunderImpactVFX, WeaponPlus1AttackOnly });
        LongMaceThunder.SetCustomSubFeatures(scale);
        LongMaceThunder.WeaponDescription.EffectDescription.effectForms.Add(EffectFormBuilder
            .Create()
            .SetDamageForm(RuleDefinitions.DamageTypeThunder, 1, RuleDefinitions.DieType.D8)
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
            .Create(WeaponTypeDefinitions.LightCrossbowType, CeHandXbowType)
            .SetGuiPresentation(Category.Item, Gui.NoLocalization)
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
            properties: new[] { WeaponPlus1 });
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
            properties: new[] { WeaponPlus2 });
        HandXbowPlus2.SetCustomSubFeatures(scale);
        ShopItems.Add((HandXbowPlus2, ShopRangedPlus2));
        ShopItems.Add((BuildRecipeManual(HandXbowPlus2, 48, 16,
                HandXbowPrimed,
                ItemDefinitions.Ingredient_Enchant_Blood_Gem),
            ShopCrafting));

        HandXbowAcid = BuildWeapon("CEHandXbowAcid", HandXbow,
            2500, true, VeryRare,
            itemDefinition.ItemPresentation, icon: HandXbowAcidIcon, needId: false, twoHanded: false,
            properties: new[] { AcidImpactVFX, WeaponPlus1AttackOnly });
        HandXbowAcid.SetCustomSubFeatures(scale);
        HandXbowAcid.WeaponDescription.EffectDescription.effectForms.Add(EffectFormBuilder
            .Create()
            .SetDamageForm(RuleDefinitions.DamageTypeAcid, 1, RuleDefinitions.DieType.D8)
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

        weapon.EffectDescription.effectForms.Add(EffectFormBuilder
            .Create()
            .SetDamageForm(RuleDefinitions.DamageTypeFire, 1, RuleDefinitions.DieType.D8)
            .Build());
        flame.staticProperties.Add(BuildFrom(FeatureDefinitionBuilder
            .Create("ProducedFlameThrower")
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

    internal static void TryAddItemsToUserMerchant(MerchantDefinition merchant)
    {
        if (Main.Settings.AddNewWeaponsAndRecipesToShops)
        {
            GiveAssortment(ShopItems, merchant, MerchantTypeContext.GetMerchantType(merchant));
        }
    }

    private static void AddToEditor()
    {
        if (!Main.Settings.AddNewWeaponsAndRecipesToEditor)
        {
            return;
        }

        foreach (var (item, _) in ShopItems)
        {
            item.inDungeonEditor = true;
        }
    }

    //TODO: move this to the separate shop context file
    private static void GiveAssortment(List<(ItemDefinition, ShopItemType)> items,
        [NotNull] IEnumerable<(MerchantDefinition, MerchantTypeContext.MerchantType)> merchants)
    {
        foreach (var (merchant, type) in merchants)
        {
            GiveAssortment(items, merchant, type);
        }
    }

    private static void GiveAssortment([NotNull] List<(ItemDefinition, ShopItemType)> items,
        MerchantDefinition merchant,
        MerchantTypeContext.MerchantType type)
    {
        foreach (var (item, itemType) in items)
        {
            if (itemType.Filter.Matches(type))
            {
                StockItem(merchant, item, itemType.Status);
            }
        }
    }

    private static void StockItem([NotNull] MerchantDefinition merchant, ItemDefinition item,
        [NotNull] BaseDefinition status)
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

    // internal static RecipeDefinition BuildRecipe([NotNull] ItemDefinition item, int hours, int difficulty,
    //     params ItemDefinition[] ingredients)
    // {
    //     return BuildRecipe(item, hours, difficulty, ingredients);
    // }

    private static RecipeDefinition BuildRecipe([NotNull] ItemDefinition item, int hours, int difficulty,
        params ItemDefinition[] ingredients)
    {
        return RecipeDefinitionBuilder
            .Create($"RecipeEnchant{item.Name}")
            .SetGuiPresentation(item.GuiPresentation.Title, GuiPresentationBuilder.EmptyString)
            .SetCraftedItem(item)
            .SetCraftingCheckData(hours, difficulty, ToolTypeDefinitions.EnchantingToolType)
            .AddIngredients(ingredients)
            .AddToDB();
    }

    [NotNull]
    private static ItemDefinition BuildRecipeManual([NotNull] ItemDefinition item, int hours, int difficulty,
        params ItemDefinition[] ingredients)
    {
        return BuildManual(BuildRecipe(item, hours, difficulty, ingredients));
    }

    // [NotNull]
    // internal static ItemDefinition BuildManual([NotNull] RecipeDefinition recipe)
    // {
    //     return BuildManual(recipe);
    // }

    [NotNull]
    private static ItemDefinition BuildManual([NotNull] RecipeDefinition recipe)
    {
        var reference = ItemDefinitions.CraftingManualScrollOfVampiricTouch;
        var manual = ItemDefinitionBuilder
            .Create($"CraftingManual{recipe.Name}")
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

    [NotNull]
    private static ItemDefinition BuildPrimingManual(ItemDefinition item, ItemDefinition primed)
    {
        return BuildManual(ItemRecipeGenerationHelper.CreatePrimingRecipe(item, primed));
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

    internal static void AddCustomTags(ItemDefinition item, Dictionary<string, TagsDefinitions.Criticity> tags)
    {
        if (ValidatorsWeapon.IsPolearm(item))
        {
            tags.TryAdd(PolearmWeaponTag, TagsDefinitions.Criticity.Normal);
        }
    }

    #region Halberd Icons

    private static AssetReferenceSprite _halberdIcon,
        _halberdPrimedIcon,
        _halberdP1Icon,
        _halberdP2Icon,
        _halberdLightningIcon;

    [NotNull]
    private static AssetReferenceSprite HalberdIcon =>
        _halberdIcon ??= CustomIcons.CreateAssetReferenceSprite("Halberd", Resources.Halberd, 128);

    [NotNull]
    private static AssetReferenceSprite HalberdPrimedIcon => _halberdPrimedIcon ??=
        CustomIcons.CreateAssetReferenceSprite("HalberdPrimed", Resources.HalberdPrimed, 128);

    [NotNull]
    private static AssetReferenceSprite HalberdP1Icon => _halberdP1Icon ??=
        CustomIcons.CreateAssetReferenceSprite("Halberd_1", Resources.Halberd_1, 128);

    [NotNull]
    private static AssetReferenceSprite HalberdP2Icon => _halberdP2Icon ??=
        CustomIcons.CreateAssetReferenceSprite("Halberd_2", Resources.Halberd_2, 128);

    [NotNull]
    private static AssetReferenceSprite HalberdLightningIcon => _halberdLightningIcon ??=
        CustomIcons.CreateAssetReferenceSprite("HalberdLightning", Resources.HalberdLightning, 128);

    #endregion

    #region Pike Icons

    private static AssetReferenceSprite _pikeIcon,
        _pikePrimedIcon,
        _pikeP1Icon,
        _pikeP2Icon,
        _pikeLightningIcon;

    [NotNull]
    private static AssetReferenceSprite PikeIcon =>
        _pikeIcon ??= CustomIcons.CreateAssetReferenceSprite("Pike", Resources.Pike, 128);

    [NotNull]
    private static AssetReferenceSprite PikePrimedIcon => _pikePrimedIcon ??=
        CustomIcons.CreateAssetReferenceSprite("PikePrimed", Resources.PikePrimed, 128);

    [NotNull]
    private static AssetReferenceSprite PikeP1Icon => _pikeP1Icon ??=
        CustomIcons.CreateAssetReferenceSprite("Pike_1", Resources.Pike_1, 128);

    [NotNull]
    private static AssetReferenceSprite PikeP2Icon => _pikeP2Icon ??=
        CustomIcons.CreateAssetReferenceSprite("Pike_2", Resources.Pike_2, 128);

    [NotNull]
    private static AssetReferenceSprite PikePsychicIcon => _pikeLightningIcon ??=
        CustomIcons.CreateAssetReferenceSprite("PikePsychic", Resources.PikePsychic, 128);

    #endregion

    #region Long Mace Icons

    private static AssetReferenceSprite _longMaceIcon,
        _longMacePrimedIcon,
        _longMaceP1Icon,
        _longMaceP2Icon,
        _longMaceLightningIcon;

    [NotNull]
    private static AssetReferenceSprite LongMaceIcon =>
        _longMaceIcon ??= CustomIcons.CreateAssetReferenceSprite("LongMace", Resources.LongMace, 128);

    [NotNull]
    private static AssetReferenceSprite LongMacePrimedIcon => _longMacePrimedIcon ??=
        CustomIcons.CreateAssetReferenceSprite("LongMacePrimed", Resources.LongMacePrimed, 128);

    [NotNull]
    private static AssetReferenceSprite LongMaceP1Icon => _longMaceP1Icon ??=
        CustomIcons.CreateAssetReferenceSprite("LongMace_1", Resources.LongMace_1, 128);

    [NotNull]
    private static AssetReferenceSprite LongMaceP2Icon => _longMaceP2Icon ??=
        CustomIcons.CreateAssetReferenceSprite("LongMace_2", Resources.LongMace_2, 128);

    [NotNull]
    private static AssetReferenceSprite LongMaceThunderIcon => _longMaceLightningIcon ??=
        CustomIcons.CreateAssetReferenceSprite("LongMaceThunder", Resources.LongMaceThunder, 128);

    #endregion

    #region Hand Crossbow Icons

    private static AssetReferenceSprite _handXbowIcon,
        _handXbowPrimedIcon,
        _handXbowP1Icon,
        _handXbowP2Icon,
        _handXbowAcidIcon;

    [NotNull]
    private static AssetReferenceSprite HandXbowIcon =>
        _handXbowIcon ??= CustomIcons.CreateAssetReferenceSprite("HandXbow", Resources.HandXbow, 128);

    [NotNull]
    private static AssetReferenceSprite HandXbowPrimedIcon => _handXbowPrimedIcon ??=
        CustomIcons.CreateAssetReferenceSprite("HandXbowPrimed", Resources.HandXbowPrimed, 128);

    [NotNull]
    private static AssetReferenceSprite HandXbowP1Icon => _handXbowP1Icon ??=
        CustomIcons.CreateAssetReferenceSprite("HandXbow_1", Resources.HandXbow_1, 128);

    [NotNull]
    private static AssetReferenceSprite HandXbowP2Icon => _handXbowP2Icon ??=
        CustomIcons.CreateAssetReferenceSprite("HandXbow_2", Resources.HandXbow_2, 128);

    [NotNull]
    private static AssetReferenceSprite HandXbowAcidIcon => _handXbowAcidIcon ??=
        CustomIcons.CreateAssetReferenceSprite("HandXbowAcid", Resources.HandXbowAcid, 128);

    #endregion

    #region Produced Flame Icons

    private static AssetReferenceSprite _producedFlameThrow;

    [NotNull]
    private static AssetReferenceSprite ProducedFlameThrow => _producedFlameThrow ??=
        CustomIcons.CreateAssetReferenceSprite("ProducedFlameThrow", Resources.ProducedFlameThrow, 128);

    #endregion
}

//TODO: move this to the separate shop context file
internal sealed class ShopItemType
{
    internal readonly MerchantFilter Filter;
    internal readonly FactionStatusDefinition Status;

    internal ShopItemType(FactionStatusDefinition status, MerchantFilter filter)
    {
        Status = status;
        Filter = filter;
    }
}

internal sealed class MerchantFilter
{
    internal bool? IsAmmunition = null;
    internal bool? IsArmor = null;
    internal bool? IsDocument;

    internal bool? IsMagicalAmmunition = null;
    internal bool? IsMagicalArmor = null;
    internal bool? IsMagicalMeleeWeapon;
    internal bool? IsMagicalRangeWeapon;
    internal bool? IsMeleeWeapon;

    internal bool? IsPrimedArmor = null;
    internal bool? IsPrimedMeleeWeapon;
    internal bool? IsPrimedRangeWeapon;
    internal bool? IsRangeWeapon;

    internal bool Matches(MerchantTypeContext.MerchantType merchantType)
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

internal sealed class ModifyProducedFlameDice : ModifyAttackModeForWeaponBase
{
    internal ModifyProducedFlameDice() : base((_, weapon, _) =>
        weapon != null && weapon.ItemDefinition == ItemDefinitions.ProducedFlame)
    {
    }

    protected override void TryModifyAttackMode(
        RulesetCharacter character,
        [NotNull] RulesetAttackMode attackMode,
        RulesetItem weapon)
    {
        DamageForm damage = null;

        foreach (var effectForm in attackMode.EffectDescription.effectForms
                     .Where(effectForm => effectForm.FormType == EffectForm.EffectFormType.Damage))
        {
            damage = effectForm.DamageForm;
        }

        if (damage == null)
        {
            return;
        }

        var casterLevel = character.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;

        damage.diceNumber = 1 + RuleDefinitions.SpellAdvancementByCasterLevel[casterLevel - 1];
    }
}

internal sealed class AddThrowProducedFlameAttack : AddExtraAttackBase
{
    internal AddThrowProducedFlameAttack() : base(ActionDefinitions.ActionType.Main, false)
    {
    }

    [NotNull]
    protected override List<RulesetAttackMode> GetAttackModes([NotNull] RulesetCharacterHero hero)
    {
        var result = new List<RulesetAttackMode>();
        AddItemAttack(result, EquipmentDefinitions.SlotTypeMainHand, hero);
        AddItemAttack(result, EquipmentDefinitions.SlotTypeOffHand, hero);
        return result;
    }

    private static void AddItemAttack(ICollection<RulesetAttackMode> attackModes, [NotNull] string slot,
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
