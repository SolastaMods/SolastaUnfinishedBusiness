using System;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Classes.Artisan.Subclasses;
using SolastaUnfinishedBusiness.CustomDefinitions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static SolastaUnfinishedBusiness.Classes.Artisan.ArtisanHelpers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using FeatureDefinitionMagicAffinityBuilder =
    SolastaUnfinishedBusiness.Builders.Features.FeatureDefinitionMagicAffinityBuilder;

namespace SolastaUnfinishedBusiness.Classes.Artisan;

internal static class ArtisanInfusionHelper
{
    internal const string ArtisanInfusePrefix = "PowerSharedPoolArtisanInfuse";
    private static FeatureDefinitionPower _artificialServant;
    private static FeatureDefinitionPower _enhancedFocus;
    private static FeatureDefinitionPower _enhancedDefense;
    private static FeatureDefinitionPower _enhancedWeapon;
    private static FeatureDefinitionPower _mindSharpener;
    private static FeatureDefinitionPower _armorOfMagicalStrength;
    private static FeatureDefinitionPower _bagOfHolding;
    private static FeatureDefinitionPower _gogglesOfNight;
    private static FeatureDefinitionPower _resistantArmor;
    private static FeatureDefinitionPower _spellRefuelingRing;
    private static FeatureDefinitionPower _blindingWeapon;
    private static FeatureDefinitionPower _improvedEnhancedFocus;
    private static FeatureDefinitionPower _improvedEnhancedDefense;
    private static FeatureDefinitionPower _cloakOfProtection;
    private static FeatureDefinitionPower _bootsOfElvenKind;
    private static FeatureDefinitionPower _cloakOfElvenKind;
    private static FeatureDefinitionPower _bootsOfStridingAndSpringing;
    private static FeatureDefinitionPower _bootsOfTheWinterland;
    private static FeatureDefinitionPower _bracersOfArchery;
    private static FeatureDefinitionPower _broochOfShielding;
    private static FeatureDefinitionPower _gauntletsOfOgrePower;
    private static FeatureDefinitionPower _glovesOfMissileSnaring;
    private static FeatureDefinitionPower _slippersOfSpiderClimbing;
    private static FeatureDefinitionPower _headbandOfIntellect;
    private static FeatureDefinitionPower _amuletOfHealth;
    private static FeatureDefinitionPower _beltOfGiantHillStrength;
    private static FeatureDefinitionPower _bracersOfDefense;
    private static FeatureDefinitionPower _cloakOfBat;
    private static FeatureDefinitionPower _ringProtectionPlus1;
    private static FeatureDefinitionPower _improvedEnhancedWeapon;

    public static FeatureDefinitionPower ArtificialServant => _artificialServant ??= BuildArtificialServant();

    public static FeatureDefinitionPower EnhancedFocus => _enhancedFocus ??= BuildEnhancedFocus();

    public static FeatureDefinitionPower ImprovedEnhancedFocus =>
        _improvedEnhancedFocus ??= BuildImprovedEnhancedFocus();

    public static FeatureDefinitionPower EnhancedDefense => _enhancedDefense ??= BuildEnhancedDefense();

    public static FeatureDefinitionPower ImprovedEnhancedDefense =>
        _improvedEnhancedDefense ??= BuildImprovedEnhancedDefense();

    public static FeatureDefinitionPower EnhancedWeapon => _enhancedWeapon ??= BuildEnhancedWeapon();

    public static FeatureDefinitionPower ImprovedEnhancedWeapon =>
        _improvedEnhancedWeapon ??= BuildImprovedEnhancedWeapon();

    public static FeatureDefinitionPower BagOfHolding => _bagOfHolding ??= BuildBagOfHolding();

    public static FeatureDefinitionPower GogglesOfNight => _gogglesOfNight ??= BuildGogglesOfNight();

    public static FeatureDefinitionPower MindSharpener => _mindSharpener ??= BuildMindSharpener();

    public static FeatureDefinitionPower ArmorOfMagicalStrength =>
        _armorOfMagicalStrength ??= BuildArmorOfMagicalStrength();

    public static FeatureDefinitionPower ResistantArmor => _resistantArmor ??= BuildResistantArmor();

    public static FeatureDefinitionPower SpellRefuelingRing => _spellRefuelingRing ??= BuildSpellRefuelingRing();

    public static FeatureDefinitionPower BlindingWeapon => _blindingWeapon ??= BuildBlindingWeapon();

    public static FeatureDefinitionPower CloakOfProtection => _cloakOfProtection ??=
        PowerMimicsItem(ItemDefinitions.CloakOfProtection, "CloakOfProtection");

    public static FeatureDefinitionPower BootsOfElvenKind => _bootsOfElvenKind ??=
        PowerMimicsItem(ItemDefinitions.BootsOfElvenKind, "BootsOfElvenKind");

    public static FeatureDefinitionPower CloakOfElvenKind => _cloakOfElvenKind ??=
        PowerMimicsItem(ItemDefinitions.CloakOfElvenkind, "CloakOfElvenKind");

    public static FeatureDefinitionPower BootsOfStridingAndSpringing => _bootsOfStridingAndSpringing ??=
        PowerMimicsItem(ItemDefinitions.BootsOfStridingAndSpringing, "BootsOfStridingAndSpringing");

    public static FeatureDefinitionPower BootsOfTheWinterland => _bootsOfTheWinterland ??=
        PowerMimicsItem(ItemDefinitions.BootsOfTheWinterland, "BootsOfTheWinterland");

    public static FeatureDefinitionPower BracersOfArchery => _bracersOfArchery ??=
        PowerMimicsItem(ItemDefinitions.Bracers_Of_Archery, "BracersOfArchery");

    public static FeatureDefinitionPower BroochOfShielding => _broochOfShielding ??=
        PowerMimicsItem(ItemDefinitions.BroochOfShielding, "BroochOfShielding");

    public static FeatureDefinitionPower GauntletsOfOgrePower => _gauntletsOfOgrePower ??=
        PowerMimicsItem(ItemDefinitions.GauntletsOfOgrePower, "GauntletsOfOgrePower");

    public static FeatureDefinitionPower GlovesOfMissileSnaring => _glovesOfMissileSnaring ??=
        PowerMimicsItem(ItemDefinitions.GlovesOfMissileSnaring, "GlovesOfMissileSnaring");

    public static FeatureDefinitionPower SlippersOfSpiderClimbing => _slippersOfSpiderClimbing ??=
        PowerMimicsItem(ItemDefinitions.SlippersOfSpiderClimbing, "SlippersOfSpiderClimbing");

    public static FeatureDefinitionPower HeadbandOfIntellect => _headbandOfIntellect ??=
        PowerMimicsItem(ItemDefinitions.HeadbandOfIntellect, "HeadbandOfIntellect");

    public static FeatureDefinitionPower AmuletOfHealth => _amuletOfHealth ??=
        PowerMimicsItem(ItemDefinitions.AmuletOfHealth, "AmuletOfHealth");

    public static FeatureDefinitionPower BeltOfGiantHillStrength => _beltOfGiantHillStrength ??=
        PowerMimicsItem(ItemDefinitions.BeltOfGiantHillStrength, "BeltOfGiantHillStrength");

    public static FeatureDefinitionPower BracersOfDefense => _bracersOfDefense ??=
        PowerMimicsItem(ItemDefinitions.Bracers_Of_Defense, "BracersOfDefense");

    public static FeatureDefinitionPower CloakOfBat =>
        _cloakOfBat ??= PowerMimicsItem(ItemDefinitions.CloakOfBat, "CloakOfBat");

    public static FeatureDefinitionPower RingProtectionPlus1 => _ringProtectionPlus1 ??=
        PowerMimicsItem(ItemDefinitions.RingProtectionPlus1, "RingProtectionPlus1");

    private static FeatureDefinitionMagicAffinity BuildMagicAffinityModifiers(
        string name,
        int attackModifier,
        int dcModifier,
        string guiName,
        AssetReferenceSprite spriteReference)
    {
        return FeatureDefinitionMagicAffinityBuilder
            .Create(name)
            .SetGuiPresentation(guiName, Category.Feature, spriteReference)
            .SetCastingModifiers(
                attackModifier,
                RuleDefinitions.SpellParamsModifierType.FlatValue,
                dcModifier,
                RuleDefinitions.SpellParamsModifierType.FlatValue,
                false,
                false,
                false)
            .AddToDB();
    }

    private static FeatureDefinitionPowerSharedPoolBuilder BuildBasicInfusionPower(
        string name,
        EffectDescription effectDescription)
    {
        return FeatureDefinitionPowerSharedPoolBuilder
            .Create(ArtisanInfusePrefix + name)
            .Configure(
                ArtisanClass.PowerPoolArtisanInfusion,
                RuleDefinitions.RechargeRate.LongRest,
                RuleDefinitions.ActivationTime.NoCost,
                1,
                false,
                false,
                AttributeDefinitions.Intelligence,
                effectDescription,
                true);
    }

    private static FeatureDefinitionPower BuildArtificialServant()
    {
        var artificialServantEffect = new EffectDescriptionBuilder();

        artificialServantEffect.SetDurationData(
            RuleDefinitions.DurationType.UntilLongRest,
            1,
            RuleDefinitions.TurnOccurenceType.EndOfTurn);

        artificialServantEffect.SetTargetingData(
            RuleDefinitions.Side.Ally,
            RuleDefinitions.RangeType.Distance,
            1,
            RuleDefinitions.TargetType.Position,
            1,
            1,
            ActionDefinitions.ItemSelectionType.Equiped);

        artificialServantEffect.AddEffectForm(
            new EffectFormBuilder().SetSummonForm(
                    SummonForm.Type.Creature,
                    ScriptableObject.CreateInstance<ItemDefinition>(),
                    1,
                    ArtificialServantBuilder.ArtificialServant.name,
                    ConditionFlyingBootsWinged,
                    true,
                    null,
                    ScriptableObject.CreateInstance<EffectProxyDefinition>())
                .Build());

        return BuildBasicInfusionPower(
                "SummonArtificialServant",
                artificialServantEffect.Build())
            .SetGuiPresentation(Category.Feature, SpellDefinitions.ConjureGoblinoids.GuiPresentation.SpriteReference)
            .AddToDB();
    }

    private static FeatureDefinitionPower BuildEnhancedFocus()
    {
        var focusPlus1 = BuildMagicAffinityModifiers(
            "MagicAffinityArtisanInfuseEnhancedFocus",
            1,
            1,
            ArtisanInfusePrefix + "EnhancedFocus",
            FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon.GuiPresentation.SpriteReference);

        var infusedFocusCondition = BuildCondition(
            "ConditionArtisanInfuseEnhancedFocus",
            RuleDefinitions.DurationType.UntilLongRest,
            1,
            false,
            focusPlus1.GuiPresentation,
            focusPlus1);

        return BuildItemConditionInfusion(infusedFocusCondition,
                "EnhancedFocus",
                FeatureDefinitionPowers.PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference)
            .AddToDB();
    }

    private static FeatureDefinitionPower BuildImprovedEnhancedFocus()
    {
        var focusPlus2 = BuildMagicAffinityModifiers(
            "MagicAffinityArtisanInfuseImprovedEnhancedFocus",
            2,
            2,
            ArtisanInfusePrefix + "ImprovedEnhancedFocus",
            FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon.GuiPresentation.SpriteReference);

        var infusedEnhancedFocusCondition = BuildCondition(
            "ConditionArtisanInfuseImprovedEnhancedFocus",
            RuleDefinitions.DurationType.UntilLongRest,
            1,
            false,
            focusPlus2.GuiPresentation,
            focusPlus2);

        return BuildItemConditionInfusion(infusedEnhancedFocusCondition,
                "ImprovedEnhancedFocus",
                FeatureDefinitionPowers.PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference)
            .SetOverriddenPower(EnhancedFocus)
            .AddToDB();
    }

    private static FeatureDefinitionPower BuildEnhancedDefense()
    {
        var artisanInfuseEnhancedArmor = BuildAttributeModifier(
            "AttributeModifierArtisanInfuseEnhancedArmor",
            FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
            AttributeDefinitions.ArmorClass,
            1,
            ArtisanInfusePrefix + "EnhancedArmor",
            ConditionAuraOfProtection.GuiPresentation.SpriteReference);

        return BuildItemModifierInfusion(artisanInfuseEnhancedArmor,
                ActionDefinitions.ItemSelectionType.Equiped,
                "EnhancedArmor",
                FeatureDefinitionPowers.PowerPaladinAuraOfProtection.GuiPresentation.SpriteReference)
            .AddToDB();
    }

    private static FeatureDefinitionPower BuildImprovedEnhancedDefense()
    {
        var artisanInfuseImprovedEnhancedArmor = BuildAttributeModifier(
            "AttributeModifierArtisanInfuseImprovedEnhancedArmor",
            FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
            AttributeDefinitions.ArmorClass,
            2,
            ArtisanInfusePrefix + "ImprovedEnhancedArmor",
            ConditionAuraOfProtection.GuiPresentation.SpriteReference);

        return BuildItemModifierInfusion(artisanInfuseImprovedEnhancedArmor,
                ActionDefinitions.ItemSelectionType.Equiped,
                "ImprovedEnhancedArmor",
                FeatureDefinitionPowers.PowerPaladinAuraOfProtection.GuiPresentation.SpriteReference)
            .SetOverriddenPower(EnhancedDefense)
            .AddToDB();
    }

    private static FeatureDefinitionPower BuildEnhancedWeapon()
    {
        return BuildItemModifierInfusion(FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon,
                ActionDefinitions.ItemSelectionType.WeaponNonMagical,
                "EnhancedWeapon",
                FeatureDefinitionPowers.PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference)
            .AddToDB();
    }

    private static FeatureDefinitionPower BuildImprovedEnhancedWeapon()
    {
        return BuildItemModifierInfusion(FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon2,
                ActionDefinitions.ItemSelectionType.WeaponNonMagical,
                "ImprovedEnhancedWeapon",
                FeatureDefinitionPowers.PowerDomainElementalLightningBlade.GuiPresentation.SpriteReference)
            .SetOverriddenPower(EnhancedWeapon)
            .AddToDB();
    }

    private static FeatureDefinitionPower BuildBagOfHolding()
    {
        var equipmentAffinity = FeatureDefinitionEquipmentAffinityBuilder
            .Create("EquipmentAffinityArtisanInfuseBagOfHolding")
            .SetGuiPresentation(
                ArtisanInfusePrefix + "BagOfHolding",
                Category.Feature, ConditionBullsStrength.GuiPresentation.SpriteReference)
            .SetCarryingCapacityMultiplier(1.0f, 500.0f)
            .AddToDB();

        var artisanInfuseBagOfHolding = BuildCondition(
            "ConditionArtisanInfuseBagOfHolding",
            RuleDefinitions.DurationType.UntilLongRest,
            1,
            false,
            equipmentAffinity.GuiPresentation,
            equipmentAffinity);

        return BuildItemConditionInfusion(artisanInfuseBagOfHolding,
                "BagOfHolding",
                FeatureDefinitionPowers.PowerFunctionPotionOfGiantStrengthCloud.GuiPresentation.SpriteReference)
            .AddToDB();
    }

    private static FeatureDefinitionPower BuildGogglesOfNight()
    {
        var artisanInfusedDarkvision = BuildCondition(
            "ConditionArtisanInfusedDarkvision",
            RuleDefinitions.DurationType.UntilLongRest,
            1,
            false,
            FeatureDefinitionSenses.SenseSuperiorDarkvision.GuiPresentation,
            FeatureDefinitionSenses.SenseSuperiorDarkvision);

        return BuildItemConditionInfusion(artisanInfusedDarkvision,
                "Darkvision",
                FeatureDefinitionPowers.PowerDomainBattleDivineWrath.GuiPresentation.SpriteReference)
            .AddToDB();
    }

    private static FeatureDefinitionPower BuildMindSharpener()
    {
        var artisanInfuseMindSharpener = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityArtisanInfuseMindSharpener")
            .SetGuiPresentation(
                ArtisanInfusePrefix + "MindSharpener",
                Category.Feature,
                ConditionBearsEndurance.GuiPresentation.SpriteReference)
            .SetConcentrationModifiers(RuleDefinitions.ConcentrationAffinity.Advantage, 20)
            .AddToDB();

        var conditionMindSharpener = BuildCondition(
            "ArtisanInfusedConditionMindSharpener",
            RuleDefinitions.DurationType.UntilLongRest,
            1,
            false,
            artisanInfuseMindSharpener.GuiPresentation,
            artisanInfuseMindSharpener);

        return BuildItemConditionInfusion(conditionMindSharpener,
                "MindSharpener",
                FeatureDefinitionPowers.PowerFunctionTomeOfQuickThought.GuiPresentation.SpriteReference)
            .AddToDB();
    }

    private static FeatureDefinitionPower BuildArmorOfMagicalStrength()
    {
        var infuseArmorMagicalStrengthCondition = new GuiPresentationBuilder(
            "Feature/&PowerSharedPoolArtisanInfuseMagicalStrengthTitle",
            "Feature/&PowerSharedPoolArtisanInfuseMagicalStrengthDescription");
        infuseArmorMagicalStrengthCondition.SetSpriteReference(ConditionBullsStrength.GuiPresentation
            .SpriteReference);
        var strengthAbilityAffinity = BuildAbilityAffinity(
            "AbilityAffinityInfusionMagicalStrength",
            new List<Tuple<string, string>> { new(AttributeDefinitions.Strength, "") },
            0,
            RuleDefinitions.DieType.D1,
            RuleDefinitions.CharacterAbilityCheckAffinity.Advantage,
            infuseArmorMagicalStrengthCondition.Build());
        var strengthSaveAffinity = BuildSavingThrowAffinity("SaveAffinityInfusionMagicalStrength",
            new List<string> { AttributeDefinitions.Strength },
            RuleDefinitions.CharacterSavingThrowAffinity.Advantage,
            FeatureDefinitionSavingThrowAffinity.ModifierType.AddDice, 0, RuleDefinitions.DieType.D1, false,
            infuseArmorMagicalStrengthCondition.Build());
        var armorMagicalStrengthCondition = BuildCondition("ArtisanInfuseArmorMagicalStrengthCondition",
            RuleDefinitions.DurationType.UntilLongRest, 1, false, infuseArmorMagicalStrengthCondition.Build(),
            strengthAbilityAffinity, strengthSaveAffinity,
            FeatureDefinitionConditionAffinitys.ConditionAffinityProneImmunity);

        return BuildItemConditionInfusion(armorMagicalStrengthCondition,
                "MagicalStrength",
                FeatureDefinitionPowers.PowerFunctionManualGainfulExercise.GuiPresentation.SpriteReference)
            .AddToDB();
    }

    private static FeatureDefinitionPower BuildResistantArmor()
    {
        var infuseResistantArmor = new GuiPresentationBuilder(
            "Feature/&PowerSharedPoolArtisanInfuseResistantArmorTitle",
            "Feature/&PowerSharedPoolArtisanInfuseResistantArmorDescription");
        infuseResistantArmor.SetSpriteReference(FeatureDefinitionPowers
            .PowerDomainElementalDiscipleOfTheElementsLightning.GuiPresentation.SpriteReference);
        var conditionArmorResistance = new GuiPresentationBuilder(
            "Feature/&PowerSharedPoolArtisanInfuseResistantArmorTitle",
            "Feature/&PowerSharedPoolArtisanInfuseResistantArmorDescription");
        conditionArmorResistance.SetSpriteReference(ConditionAuraOfProtection.GuiPresentation.SpriteReference);
        var armorResistance = BuildCondition("ConditionArtisanResistantArmor",
            RuleDefinitions.DurationType.UntilLongRest, 1, false, infuseResistantArmor.Build(),
            FeatureDefinitionDamageAffinitys.DamageAffinityAcidResistance,
            FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance,
            FeatureDefinitionDamageAffinitys.DamageAffinityFireResistance,
            FeatureDefinitionDamageAffinitys.DamageAffinityForceDamageResistance,
            FeatureDefinitionDamageAffinitys.DamageAffinityLightningResistance,
            FeatureDefinitionDamageAffinitys.DamageAffinityNecroticResistance,
            FeatureDefinitionDamageAffinitys.DamageAffinityPoisonResistance,
            FeatureDefinitionDamageAffinitys.DamageAffinityPsychicResistance,
            FeatureDefinitionDamageAffinitys.DamageAffinityRadiantResistance,
            FeatureDefinitionDamageAffinitys.DamageAffinityThunderResistance);
        return BuildItemConditionInfusion(armorResistance, "ResistantArmor",
            FeatureDefinitionPowers
                .PowerDomainElementalDiscipleOfTheElementsLightning.GuiPresentation.SpriteReference).AddToDB();
    }

    private static FeatureDefinitionPower BuildSpellRefuelingRing()
    {
        var infuseSpellRefuelingRing = new GuiPresentationBuilder(
            "Feature/&PowerSharedPoolArtisanInfuseSpellRefuelingRingTitle",
            "Feature/&PowerSharedPoolArtisanInfuseSpellRefuelingRingDescription");
        infuseSpellRefuelingRing.SetSpriteReference(FeatureDefinitionPowers
            .PowerDomainElementalDiscipleOfTheElementsLightning.GuiPresentation.SpriteReference);

        var spellEffect = new EffectDescriptionBuilder();
        spellEffect.SetDurationData(RuleDefinitions.DurationType.UntilLongRest, 1,
            RuleDefinitions.TurnOccurenceType.EndOfTurn);
        spellEffect.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1,
            RuleDefinitions.TargetType.Self);
        spellEffect.AddEffectForm(new EffectFormBuilder().SetSpellForm(9).Build());

        return new FeatureDefinitionPowerSharedPoolBuilder(ArtisanInfusePrefix + "SpellRefuelingRing",
            GuidHelper.Create(ArtisanClass.GuidNamespace, ArtisanInfusePrefix + "SpellRefuelingRing")
                .ToString(),
            ArtisanClass.PowerPoolArtisanInfusion, RuleDefinitions.RechargeRate.LongRest,
            RuleDefinitions.ActivationTime.NoCost, 1, false, false, AttributeDefinitions.Intelligence,
            spellEffect.Build(), infuseSpellRefuelingRing.Build(), true /* unique instance */).AddToDB();
    }

    private static FeatureDefinitionPower BuildBlindingWeapon()
    {
        var radiantWeaponEffectGui = new GuiPresentationBuilder(
            "Feature/&PowerSharedPoolArtisanInfuseBlindingWeaponTitle",
            "Feature/&PowerSharedPoolArtisanInfuseBlindingWeaponDescription");
        radiantWeaponEffectGui.SetSpriteReference(FeatureDefinitionAttackModifiers.AttackModifierMagicWeapon
            .GuiPresentation.SpriteReference);

        var addBlindingCondition = new ConditionOperationDescription
        {
            Operation = ConditionOperationDescription.ConditionOperation.Add,
            ConditionDefinition = ConditionBlinded,
            saveAffinity = RuleDefinitions.EffectSavingThrowType.Negates,
            conditionName = ConditionBlinded.Name
        };

        var radiantDamage = new ArtisanHelpers.FeatureDefinitionAdditionalDamageBuilder(
                "AdditionalDamageArtisanRadiantWeapon",
                ArtisanClass.GuidNamespace, "BlindingWeaponStrike",
                RuleDefinitions.FeatureLimitedUsage.OncePerTurn,
                RuleDefinitions.AdditionalDamageValueDetermination.Die,
                RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive,
                RuleDefinitions.RestrictedContextRequiredProperty.None, true, RuleDefinitions.DieType.D4, 1,
                RuleDefinitions.AdditionalDamageType.Specific,
                RuleDefinitions.DamageTypeRadiant, RuleDefinitions.AdditionalDamageAdvancement.None,
                new List<DiceByRank>(), true,
                AttributeDefinitions.Constitution, 15, RuleDefinitions.EffectSavingThrowType.None,
                new List<ConditionOperationDescription> { addBlindingCondition }, radiantWeaponEffectGui.Build())
            .AddToDB();

        return BuildItemModifierInfusion(radiantDamage,
                ActionDefinitions.ItemSelectionType.Weapon, "BlindingWeapon",
                FeatureDefinitionPowers.PowerDomainSunIndomitableLight.GuiPresentation.SpriteReference)
            .AddToDB();
    }

    internal static FeatureDefinitionPowerSharedPoolBuilder BuildItemModifierInfusion(
        FeatureDefinition itemFeature,
        ActionDefinitions.ItemSelectionType itemType,
        string name,
        AssetReferenceSprite spriteReference)
    {
        var itemEffect = new EffectDescriptionBuilder();

        itemEffect.SetDurationData(
            RuleDefinitions.DurationType.UntilLongRest,
            1,
            RuleDefinitions.TurnOccurenceType.EndOfTurn);
        itemEffect.SetTargetingData(
            RuleDefinitions.Side.Ally,
            RuleDefinitions.RangeType.Touch,
            1,
            RuleDefinitions.TargetType.Item,
            1,
            1,
            itemType);
        itemEffect.AddEffectForm(new EffectFormBuilder()
            .SetItemPropertyForm(
                new List<FeatureUnlockByLevel> { new(itemFeature, 0) },
                RuleDefinitions.ItemPropertyUsage.Unlimited,
                1)
            .Build());

        return BuildBasicInfusionPower(name, itemEffect.Build())
            .SetGuiPresentation(Category.Feature, spriteReference)
            .SetCustomSubFeatures(FeatureDefinitionSkipEffectRemovalOnLocationChange.Always);
    }

    private static FeatureDefinitionPowerSharedPoolBuilder BuildItemConditionInfusion(
        ConditionDefinition condition,
        string name,
        AssetReferenceSprite spriteReference)
    {
        var conditionEffect = new EffectDescriptionBuilder();

        conditionEffect.SetDurationData(
            RuleDefinitions.DurationType.UntilLongRest,
            1,
            RuleDefinitions.TurnOccurenceType.EndOfTurn);
        conditionEffect.SetTargetingData(
            RuleDefinitions.Side.Ally,
            RuleDefinitions.RangeType.Touch,
            1,
            RuleDefinitions.TargetType.Individuals,
            1,
            1,
            ActionDefinitions.ItemSelectionType.Equiped);
        conditionEffect.AddEffectForm(
            new EffectFormBuilder()
                .SetConditionForm(
                    condition,
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false,
                    new List<ConditionDefinition>())
                .Build());

        return BuildBasicInfusionPower(name, conditionEffect.Build())
            .SetGuiPresentation(Category.Feature, spriteReference)
            .SetCustomSubFeatures(FeatureDefinitionSkipEffectRemovalOnLocationChange.Always);
    }

    private static FeatureDefinitionPower PowerMimicsItem(ItemDefinition item, string name)
    {
        var itemCondition = BuildCondition(
            "ConditionArtisanInfuse" + name,
            RuleDefinitions.DurationType.UntilLongRest,
            1,
            false,
            item.GuiPresentation,
            item.StaticProperties.Select(p => p.FeatureDefinition).ToArray());

        var itemEffect = new EffectDescriptionBuilder();

        itemEffect.SetDurationData(
            RuleDefinitions.DurationType.UntilLongRest,
            1,
            RuleDefinitions.TurnOccurenceType.EndOfTurn);
        itemEffect.SetTargetingData(
            RuleDefinitions.Side.Ally,
            RuleDefinitions.RangeType.Touch,
            1,
            RuleDefinitions.TargetType.Individuals,
            1,
            1,
            ActionDefinitions.ItemSelectionType.Equiped);
        itemEffect.AddEffectForm(
            new EffectFormBuilder()
                .SetConditionForm(
                    itemCondition,
                    ConditionForm.ConditionOperation.Add,
                    false,
                    false,
                    new List<ConditionDefinition>())
                .Build());

        return BuildBasicInfusionPower(name, itemEffect.Build())
            .SetGuiPresentation(item.GuiPresentation)
            .SetCustomSubFeatures(FeatureDefinitionSkipEffectRemovalOnLocationChange.Always)
            .AddToDB();
    }
}
