using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class WayOfTheDragon : AbstractSubclass
{
    internal const string Name = "WayOfTheDragon";

    public WayOfTheDragon()
    {
        var damageAffinityAncestry = FeatureDefinitionDamageAffinityBuilder
            .Create($"DamageAffinity{Name}Ancestry")
            .SetGuiPresentation("DragonbornDamageResistance", Category.Feature)
            .SetAncestryType(ExtraAncestryType.WayOfTheDragon)
            .SetDamageAffinityType(DamageAffinityType.Resistance)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite("WayOfTheDragon", Resources.WayOfTheDragon, 256))
            .AddFeaturesAtLevel(3, BuildDiscipleFeatureSet(), BuildDragonFeatureSet(), damageAffinityAncestry)
            .AddFeaturesAtLevel(6, BuildReactiveHidePower())
            .AddFeaturesAtLevel(11, BuildDragonFuryFeatureSet())
            .AddFeaturesAtLevel(17, BuildAscensionFeatureSet())
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Monk;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceMonkMonasticTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static FeatureDefinitionFeatureSet BuildDiscipleFeatureSet()
    {
        var featureSetDisciple = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}Disciple")
            .SetGuiPresentation("PathClawDragonAncestry", Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
            .SetAncestryType(ExtraAncestryType.WayOfTheDragon)
            .AddToDB();

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var featureDefinitionAncestry in DatabaseRepository.GetDatabase<FeatureDefinitionAncestry>()
                     .Where(x => x.Type == AncestryType.BarbarianClaw)
                     .ToList())
        {
            var newAncestryName = featureDefinitionAncestry.Name.Replace("PathClaw", Name);
            var ancestry = FeatureDefinitionAncestryBuilder
                .Create(featureDefinitionAncestry, newAncestryName)
                .SetAncestry(ExtraAncestryType.WayOfTheDragon)
                .AddToDB();

            featureSetDisciple.FeatureSet.Add(ancestry);
        }

        return featureSetDisciple;
    }

    private static FeatureDefinitionFeatureSet BuildDragonFeatureSet()
    {
        var powerBlackElementalBreath = FeatureDefinitionPowerBuilder
            .Create(PowerDragonbornBreathWeaponBlack, $"Power{Name}ElementalBreathBlack")
            .SetUsesProficiencyBonus(ActivationTime.NoCost)
            .SetGuiPresentation(Category.Feature, PowerDragonbornBreathWeaponBlack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerDragonbornBreathWeaponBlack.EffectDescription)
                    .SetParticleEffectParameters(PowerDragonbornBreathWeaponBlack)
                    .SetSavingThrowData(false,
                        AttributeDefinitions.Dexterity,
                        false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                        AttributeDefinitions.Wisdom,
                        20)
                    .Build())
            .AddToDB();

        powerBlackElementalBreath.EffectDescription.EffectForms[0].DamageForm.diceNumber = 3;
        powerBlackElementalBreath.EffectDescription.EffectForms[0].diceByLevelTable = [];
        powerBlackElementalBreath.AddCustomSubFeatures(
            ValidatorsValidatePowerUse.HasMainAttackAvailable,
            new CustomBehaviorElementalBreathProficiency(powerBlackElementalBreath));

        var powerBlackElementalBreathPoints = FeatureDefinitionPowerBuilder
            .Create(powerBlackElementalBreath, $"Power{Name}ElementalBreathBlackPoints")
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 2, 2)
            .AddToDB();

        powerBlackElementalBreathPoints.AddCustomSubFeatures(
            ValidatorsValidatePowerUse.HasMainAttackAvailable,
            new CustomBehaviorElementalBreathPoints(powerBlackElementalBreathPoints, powerBlackElementalBreath));

        var featureSetElementalBreathBlack = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}ElementalBreathBlack")
            .SetGuiPresentationNoContent(true)
            .AddFeatureSet(powerBlackElementalBreath, powerBlackElementalBreathPoints)
            .AddToDB();

        var powerBlueElementalBreath = FeatureDefinitionPowerBuilder
            .Create(PowerDragonbornBreathWeaponBlue, $"Power{Name}ElementalBreathBlue")
            .SetUsesProficiencyBonus(ActivationTime.NoCost)
            .SetGuiPresentation(Category.Feature, PowerDragonbornBreathWeaponBlue)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerDragonbornBreathWeaponBlue.EffectDescription)
                    .SetParticleEffectParameters(PowerDragonbornBreathWeaponBlue)
                    .SetSavingThrowData(false,
                        AttributeDefinitions.Dexterity,
                        false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                        AttributeDefinitions.Wisdom,
                        20)
                    .Build())
            .AddToDB();

        powerBlueElementalBreath.EffectDescription.EffectForms[0].DamageForm.diceNumber = 3;
        powerBlueElementalBreath.EffectDescription.EffectForms[0].diceByLevelTable = [];
        powerBlueElementalBreath.AddCustomSubFeatures(
            ValidatorsValidatePowerUse.HasMainAttackAvailable,
            new CustomBehaviorElementalBreathProficiency(powerBlueElementalBreath));

        var powerBlueElementalBreathPoints = FeatureDefinitionPowerBuilder
            .Create(powerBlueElementalBreath, $"Power{Name}ElementalBreathBluePoints")
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 2, 2)
            .AddToDB();

        powerBlueElementalBreathPoints.AddCustomSubFeatures(
            ValidatorsValidatePowerUse.HasMainAttackAvailable,
            new CustomBehaviorElementalBreathPoints(powerBlueElementalBreathPoints, powerBlueElementalBreath));

        var featureSetElementalBreathBlue = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}ElementalBreathBlue")
            .SetGuiPresentationNoContent(true)
            .AddFeatureSet(powerBlueElementalBreath, powerBlueElementalBreathPoints)
            .AddToDB();

        var powerGreenElementalBreath = FeatureDefinitionPowerBuilder
            .Create(PowerDragonbornBreathWeaponGreen, $"Power{Name}ElementalBreathGreen")
            .SetGuiPresentation(Category.Feature, PowerDragonbornBreathWeaponGreen)
            .SetUsesProficiencyBonus(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerDragonbornBreathWeaponGreen.EffectDescription)
                    .SetParticleEffectParameters(StinkingCloud)
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Sphere, 3)
                    .SetDurationData(DurationType.Round, 3)
                    .SetSavingThrowData(false,
                        AttributeDefinitions.Dexterity,
                        false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                        AttributeDefinitions.Wisdom,
                        20)
                    .AddEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetSummonEffectProxyForm(
                                EffectProxyDefinitionBuilder
                                    .Create(EffectProxyDefinitions.ProxyStinkingCloud, "ProxyElementalBreathGreen")
                                    .SetOrUpdateGuiPresentation($"Power{Name}ElementalBreathGreen", Category.Feature)
                                    .AddToDB())
                            .Build(),
                        EffectFormBuilder.TopologyForm(TopologyForm.Type.SightImpaired, true),
                        EffectFormBuilder.TopologyForm(TopologyForm.Type.DangerousZone, true))
                    .Build())
            .AddToDB();

        powerGreenElementalBreath.EffectDescription.EffectForms[0].DamageForm.diceNumber = 3;
        powerGreenElementalBreath.EffectDescription.EffectForms[0].diceByLevelTable = [];
        powerGreenElementalBreath.AddCustomSubFeatures(
            ValidatorsValidatePowerUse.HasMainAttackAvailable,
            new CustomBehaviorElementalBreathProficiency(powerGreenElementalBreath));

        var powerGreenElementalBreathPoints = FeatureDefinitionPowerBuilder
            .Create(powerGreenElementalBreath, $"Power{Name}ElementalBreathGreenPoints")
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 2, 2)
            .AddToDB();

        powerGreenElementalBreathPoints.AddCustomSubFeatures(
            ValidatorsValidatePowerUse.HasMainAttackAvailable,
            new CustomBehaviorElementalBreathPoints(powerGreenElementalBreathPoints, powerGreenElementalBreath));

        var featureSetElementalBreathGreen = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}ElementalBreathGreen")
            .SetGuiPresentationNoContent(true)
            .AddFeatureSet(powerGreenElementalBreath, powerGreenElementalBreathPoints)
            .AddToDB();

        var powerGoldElementalBreath = FeatureDefinitionPowerBuilder
            .Create(PowerDragonbornBreathWeaponGold, $"Power{Name}ElementalBreathGold")
            .SetUsesProficiencyBonus(ActivationTime.NoCost)
            .SetGuiPresentation(Category.Feature, PowerDragonbornBreathWeaponGold)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerDragonbornBreathWeaponGold.EffectDescription)
                    .SetParticleEffectParameters(PowerDragonbornBreathWeaponGold)
                    .SetSavingThrowData(false,
                        AttributeDefinitions.Dexterity,
                        false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                        AttributeDefinitions.Wisdom,
                        20)
                    .Build())
            .AddToDB();

        powerGoldElementalBreath.EffectDescription.EffectForms[0].DamageForm.diceNumber = 3;
        powerGoldElementalBreath.EffectDescription.EffectForms[0].diceByLevelTable = [];
        powerGoldElementalBreath.AddCustomSubFeatures(
            ValidatorsValidatePowerUse.HasMainAttackAvailable,
            new CustomBehaviorElementalBreathProficiency(powerGoldElementalBreath));

        var powerGoldElementalBreathPoints = FeatureDefinitionPowerBuilder
            .Create(powerGoldElementalBreath, $"Power{Name}ElementalBreathGoldPoints")
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 2, 2)
            .AddToDB();

        powerGoldElementalBreathPoints.AddCustomSubFeatures(
            ValidatorsValidatePowerUse.HasMainAttackAvailable,
            new CustomBehaviorElementalBreathPoints(powerGoldElementalBreathPoints, powerGoldElementalBreath));

        var featureSetElementalBreathGold = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}ElementalBreathGold")
            .SetGuiPresentationNoContent(true)
            .AddFeatureSet(powerGoldElementalBreath, powerGoldElementalBreathPoints)
            .AddToDB();

        var powerSilverElementalBreath = FeatureDefinitionPowerBuilder
            .Create(PowerDragonbornBreathWeaponSilver, $"Power{Name}ElementalBreathSilver")
            .SetUsesProficiencyBonus(ActivationTime.NoCost)
            .SetGuiPresentation(Category.Feature, PowerDragonbornBreathWeaponSilver)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerDragonbornBreathWeaponSilver)
                    .SetParticleEffectParameters(PowerDragonbornBreathWeaponSilver)
                    .SetSavingThrowData(false,
                        AttributeDefinitions.Dexterity,
                        false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                        AttributeDefinitions.Wisdom,
                        20)
                    .Build())
            .AddToDB();

        powerSilverElementalBreath.EffectDescription.EffectForms[0].DamageForm.diceNumber = 3;
        powerSilverElementalBreath.EffectDescription.EffectForms[0].diceByLevelTable = [];
        powerSilverElementalBreath.AddCustomSubFeatures(
            ValidatorsValidatePowerUse.HasMainAttackAvailable,
            new CustomBehaviorElementalBreathProficiency(powerSilverElementalBreath));

        var powerSilverElementalBreathPoints = FeatureDefinitionPowerBuilder
            .Create(powerSilverElementalBreath, $"Power{Name}ElementalBreathSilverPoints")
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 2, 2)
            .AddToDB();

        powerSilverElementalBreathPoints.AddCustomSubFeatures(
            ValidatorsValidatePowerUse.HasMainAttackAvailable,
            new CustomBehaviorElementalBreathPoints(powerSilverElementalBreathPoints, powerSilverElementalBreath));

        var featureSetElementalBreathSilver = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}ElementalBreathSilver")
            .SetGuiPresentationNoContent(true)
            .AddFeatureSet(powerSilverElementalBreath, powerSilverElementalBreathPoints)
            .AddToDB();

        var featureWayOfDragonBreath = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}Breath")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.DeterminedByAncestry)
            .AddFeatureSet(
                featureSetElementalBreathBlack,
                featureSetElementalBreathSilver,
                featureSetElementalBreathGold,
                featureSetElementalBreathBlue,
                featureSetElementalBreathGreen)
            .SetAncestryType(ExtraAncestryType.WayOfTheDragon,
                DamageTypeAcid, DamageTypeCold, DamageTypeFire, DamageTypeLightning, DamageTypePoison)
            .AddToDB();

        return featureWayOfDragonBreath;
    }


    private static FeatureDefinitionPower BuildReactiveHidePower()
    {
        var conditionReactiveHide = ConditionDefinitionBuilder
            .Create($"Condition{Name}ReactiveHide")
            .SetGuiPresentation(Category.Condition)
            .SetPossessive()
            .AddFeatures(
                DamageAffinityAcidResistance,
                DamageAffinityBludgeoningResistance,
                DamageAffinityColdResistance,
                DamageAffinityFireResistance,
                DamageAffinityForceDamageResistance,
                DamageAffinityLightningResistance,
                DamageAffinityNecroticResistance,
                DamageAffinityPiercingResistance,
                DamageAffinityPoisonResistance,
                DamageAffinityPsychicResistance,
                DamageAffinityRadiantResistance,
                DamageAffinitySlashingResistance,
                DamageAffinityThunderResistance)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var powerReactiveHide = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ReactiveHide")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionReactiveHide))
                    .SetParticleEffectParameters(PowerPatronHiveReactiveCarapace)
                    .Build())
            .AddToDB();

        powerReactiveHide.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new CustomBehaviorReactiveHide(powerReactiveHide, conditionReactiveHide));

        return powerReactiveHide;
    }

    private static FeatureDefinitionFeatureSet BuildDragonFuryFeatureSet()
    {
        const string NOTIFICATION_TAG = "DragonFury";

        var additionalDamageAcid = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}DragonFuryAcid")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag(NOTIFICATION_TAG + DamageTypeAcid)
            .SetAdditionalDamageType(AdditionalDamageType.Specific)
            .SetImpactParticleReference(AcidSplash)
            .SetSpecificDamageType(DamageTypeAcid)
            .SetDamageDice(DieType.D6, 2)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Unarmed)
            .AddToDB();

        var conditionDragonFuryAcid = ConditionDefinitionBuilder
            .Create($"Condition{Name}DragonFuryAcid")
            .SetGuiPresentation(Category.Condition, ConditionPactChainPseudodragon)
            .SetPossessive()
            .AddFeatures(additionalDamageAcid)
            .AddToDB();

        var powerDragonFuryAcid = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DragonFuryAcid")
            .SetGuiPresentation(Category.Feature, AcidSplash)
            .AddCustomSubFeatures(ValidatorsValidatePowerUse.HasNoneOfConditions(conditionDragonFuryAcid.Name))
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 2, 2)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetParticleEffectParameters(AcidSplash)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionDragonFuryAcid))
                    .Build())
            .AddToDB();

        var additionalDamageLightning = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}DragonFuryLightning")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag(NOTIFICATION_TAG + DamageTypeLightning)
            .SetAdditionalDamageType(AdditionalDamageType.Specific)
            .SetImpactParticleReference(LightningBolt)
            .SetSpecificDamageType(DamageTypeLightning)
            .SetDamageDice(DieType.D6, 2)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Unarmed)
            .AddToDB();

        var conditionDragonFuryLightning = ConditionDefinitionBuilder
            .Create($"Condition{Name}DragonFuryLightning")
            .SetGuiPresentation(Category.Condition, ConditionPactChainPseudodragon)
            .SetPossessive()
            .AddFeatures(additionalDamageLightning)
            .AddToDB();

        var powerDragonFuryLightning = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DragonFuryLightning")
            .SetGuiPresentation(Category.Feature, ShockingGrasp)
            .AddCustomSubFeatures(ValidatorsValidatePowerUse.HasNoneOfConditions(conditionDragonFuryLightning.Name))
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 2, 2)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetParticleEffectParameters(LightningBolt)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionDragonFuryLightning))
                    .Build())
            .AddToDB();

        var additionalDamagePoison = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}DragonFuryPoison").SetGuiPresentation(Category.Feature)
            .SetNotificationTag(NOTIFICATION_TAG + DamageTypePoison)
            .SetAdditionalDamageType(AdditionalDamageType.Specific)
            .SetImpactParticleReference(PoisonSpray)
            .SetSpecificDamageType(DamageTypePoison)
            .SetDamageDice(DieType.D6, 2)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Unarmed)
            .AddToDB();

        var conditionDragonFuryPoison = ConditionDefinitionBuilder
            .Create($"Condition{Name}DragonFuryPoison")
            .SetGuiPresentation(Category.Condition, ConditionPactChainPseudodragon)
            .SetPossessive()
            .AddFeatures(additionalDamagePoison)
            .AddToDB();

        var powerDragonFuryPoison = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DragonFuryPoison")
            .SetGuiPresentation(Category.Feature, PoisonSpray)
            .AddCustomSubFeatures(ValidatorsValidatePowerUse.HasNoneOfConditions(conditionDragonFuryPoison.Name))
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 2, 2)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetParticleEffectParameters(PoisonSpray)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionDragonFuryPoison))
                    .Build())
            .AddToDB();

        var additionalDamageFire = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}DragonFuryFire")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag(NOTIFICATION_TAG + DamageTypeFire)
            .SetAdditionalDamageType(AdditionalDamageType.Specific)
            .SetSpecificDamageType(DamageTypeFire)
            .SetImpactParticleReference(FireBolt)
            .SetDamageDice(DieType.D6, 2)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Unarmed)
            .AddToDB();

        var conditionDragonFuryFire = ConditionDefinitionBuilder
            .Create($"Condition{Name}DragonFuryFire")
            .SetGuiPresentation(Category.Condition, ConditionPactChainPseudodragon)
            .SetPossessive()
            .AddFeatures(additionalDamageFire)
            .AddToDB();

        var powerDragonFuryFire = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DragonFuryFire")
            .SetGuiPresentation(Category.Feature, ProduceFlame)
            .AddCustomSubFeatures(ValidatorsValidatePowerUse.HasNoneOfConditions(conditionDragonFuryFire.Name))
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 2, 2)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetParticleEffectParameters(Fireball)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionDragonFuryFire))
                    .Build())
            .AddToDB();

        var additionalDamageCold = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}DragonFuryCold")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag(NOTIFICATION_TAG + DamageTypeCold)
            .SetAdditionalDamageType(AdditionalDamageType.Specific)
            .SetImpactParticleReference(ConeOfCold)
            .SetSpecificDamageType(DamageTypeCold)
            .SetDamageDice(DieType.D6, 2)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Unarmed)
            .AddToDB();

        var conditionDragonFuryCold = ConditionDefinitionBuilder
            .Create($"Condition{Name}DragonFuryCold")
            .SetGuiPresentation(Category.Condition, ConditionPactChainPseudodragon)
            .SetPossessive()
            .AddFeatures(additionalDamageCold)
            .AddToDB();

        var powerDragonFuryCold = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DragonFuryCold")
            .SetGuiPresentation(Category.Feature, RayOfFrost)
            .AddCustomSubFeatures(ValidatorsValidatePowerUse.HasNoneOfConditions(conditionDragonFuryCold.Name))
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 2, 2)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetParticleEffectParameters(ConeOfCold)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionDragonFuryCold))
                    .Build())
            .AddToDB();

        var featureWayOfDragonFury = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}Fury")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.DeterminedByAncestry)
            .AddFeatureSet(
                powerDragonFuryAcid,
                powerDragonFuryLightning,
                powerDragonFuryFire,
                powerDragonFuryPoison,
                powerDragonFuryCold)
            .SetAncestryType(ExtraAncestryType.WayOfTheDragon,
                DamageTypeAcid, DamageTypeCold, DamageTypeFire, DamageTypeLightning, DamageTypePoison)
            .AddToDB();

        return featureWayOfDragonFury;
    }

    private static FeatureDefinitionFeatureSet BuildAscensionFeatureSet()
    {
        var conditionAscension = ConditionDefinitionBuilder
            .Create($"Condition{Name}Ascension")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create($"AttributeModifier{Name}Ascension")
                    .SetGuiPresentation($"Power{Name}Ascension", Category.Feature)
                    .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 2)
                    .AddToDB())
            .AddToDB();

        var powerAscension = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Ascension")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("FlightSprout", Resources.PowerAngelicFormSprout, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Permanent)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitions.ConditionFlyingAdaptive,
                                ConditionForm.ConditionOperation.Add)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                conditionAscension,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(new ValidatorsValidatePowerUse(
                ValidatorsCharacter.HasNoneOfConditions(RuleDefinitions.ConditionFlyingAdaptive)))
            .AddToDB();

        var powerAscensionDismiss = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}AscensionDismiss")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("FlightDismiss", Resources.PowerAngelicFormDismiss, 256, 128))
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitions.ConditionFlyingAdaptive,
                                ConditionForm.ConditionOperation.Remove)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                conditionAscension,
                                ConditionForm.ConditionOperation.Remove)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(new ValidatorsValidatePowerUse(
                ValidatorsCharacter.HasAnyOfConditions(RuleDefinitions.ConditionFlyingAdaptive)))
            .AddToDB();

        var featureSetAscension = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}Ascension")
            .SetGuiPresentation($"Power{Name}Ascension", Category.Feature)
            .AddFeatureSet(powerAscension, powerAscensionDismiss)
            .AddToDB();

        return featureSetAscension;
    }

    //
    // Elemental Breath Fixed
    //

    private sealed class CustomBehaviorElementalBreathProficiency(
        FeatureDefinitionPower powerElementalBreathProficiency) :
        IValidatePowerUse, IMagicEffectFinishedByMe, IModifyEffectDescription
    {
        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var actingCharacter = action.ActingCharacter;

            actingCharacter.BurnOneMainAttack();
            actingCharacter.UsedSpecialFeatures.TryAdd("ElementalBreath", 1);

            yield break;
        }

        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerElementalBreathProficiency;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var damageForm = effectDescription.FindFirstDamageForm();

            if (damageForm == null)
            {
                return effectDescription;
            }

            if (character.GetClassLevel(CharacterClassDefinitions.Monk) >= 17)
            {
                damageForm.DiceNumber = 4;
            }

            damageForm.DieType = character.GetMonkDieType();

            return effectDescription;
        }

        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower featureDefinitionPower)
        {
            var glc = GameLocationCharacter.GetFromActor(character);

            if (glc == null || !glc.OnceInMyTurnIsValid("ElementalBreath"))
            {
                return false;
            }

            return character.GetRemainingPowerUses(powerElementalBreathProficiency) > 0;
        }
    }

    //
    // Elemental Breath Points
    //

    private sealed class CustomBehaviorElementalBreathPoints(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerElementalBreathPoints,
        FeatureDefinitionPower powerElementalBreathProficiency)
        :
            IValidatePowerUse, IMagicEffectFinishedByMe, IModifyEffectDescription
    {
        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var actingCharacter = action.ActingCharacter;

            actingCharacter.BurnOneMainAttack();
            actingCharacter.UsedSpecialFeatures.TryAdd("ElementalBreath", 1);

            yield break;
        }

        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerElementalBreathPoints;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var damageForm = effectDescription.FindFirstDamageForm();

            if (damageForm == null)
            {
                return effectDescription;
            }

            if (character.GetClassLevel(CharacterClassDefinitions.Monk) >= 17)
            {
                damageForm.DiceNumber = 4;
            }

            damageForm.DieType = character.GetMonkDieType();

            return effectDescription;
        }

        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower featureDefinitionPower)
        {
            var glc = GameLocationCharacter.GetFromActor(character);

            if (glc == null || !glc.OnceInMyTurnIsValid("ElementalBreath"))
            {
                return false;
            }

            return character.GetRemainingPowerUses(powerElementalBreathProficiency) == 0;
        }
    }

    //
    // Reactive Hide
    //

    private sealed class CustomBehaviorReactiveHide(
        FeatureDefinitionPower powerReactiveHide,
        ConditionDefinition conditionReactiveHide) : ITryAlterOutcomeAttack, IPhysicalAttackFinishedOnMe
    {
        public IEnumerator OnPhysicalAttackFinishedOnMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            if (rollOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure)
            {
                yield break;
            }

            if (!defender.CanAct())
            {
                yield break;
            }

            var rulesetCharacter = defender.RulesetCharacter;

            if (rulesetCharacter.RemainingKiPoints == 0)
            {
                yield break;
            }

            if (!rulesetCharacter.HasConditionOfType(conditionReactiveHide))
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (!TryGetAncestryDamageTypeFromCharacter(
                    defender.Guid, (AncestryType)ExtraAncestryType.WayOfTheDragon, out var damageType))
            {
                yield break;
            }

            var dieType = rulesetCharacter.GetMonkDieType();
            var rolls = new List<int>();
            DamageForm damageForm;
            int damageRoll;

            switch (damageType)
            {
                //
                // ACID
                //

                case DamageTypeAcid:
                {
                    damageForm = new DamageForm
                    {
                        DamageType = DamageTypeAcid, DieType = dieType, DiceNumber = 1, BonusDamage = 0
                    };
                    damageRoll =
                        rulesetCharacter.RollDamage(damageForm, 0, false, 0, 0, 1, false, false, false, rolls);

                    InflictDamage(PowerDragonbornBreathWeaponBlack);

                    yield break;
                }

                //
                // LIGHTNING
                //

                case DamageTypeLightning:
                {
                    damageForm = new DamageForm
                    {
                        DamageType = DamageTypeLightning, DieType = dieType, DiceNumber = 1, BonusDamage = 0
                    };
                    damageRoll =
                        rulesetCharacter.RollDamage(damageForm, 0, false, 0, 0, 1, false, false, false, rolls);

                    InflictDamage(PowerDragonbornBreathWeaponBlue);

                    yield break;
                }

                //
                // FIRE
                //

                case DamageTypeFire:
                {
                    damageForm = new DamageForm
                    {
                        DamageType = DamageTypeFire, DieType = dieType, DiceNumber = 1, BonusDamage = 0
                    };
                    damageRoll =
                        rulesetCharacter.RollDamage(damageForm, 0, false, 0, 0, 1, false, false, false, rolls);

                    InflictDamage(PowerDragonbornBreathWeaponGold);

                    yield break;
                }

                //
                // POISON
                //

                case DamageTypePoison:
                {
                    damageForm = new DamageForm
                    {
                        DamageType = DamageTypePoison, DieType = dieType, DiceNumber = 1, BonusDamage = 0
                    };
                    damageRoll =
                        rulesetCharacter.RollDamage(damageForm, 0, false, 0, 0, 1, false, false, false, rolls);

                    InflictDamage(PowerDragonbornBreathWeaponGreen);

                    yield break;
                }

                //
                // COLD
                //

                case DamageTypeCold:
                {
                    damageForm = new DamageForm
                    {
                        DamageType = DamageTypeCold, DieType = dieType, DiceNumber = 1, BonusDamage = 0
                    };
                    damageRoll =
                        rulesetCharacter.RollDamage(damageForm, 0, false, 0, 0, 1, false, false, false, rolls);

                    InflictDamage(PowerDragonbornBreathWeaponSilver);

                    yield break;
                }
            }

            yield break;

            void InflictDamage(IMagicEffect magicEffect)
            {
                var applyFormsParams = new RulesetImplementationDefinitions.ApplyFormsParams
                {
                    sourceCharacter = rulesetCharacter,
                    targetCharacter = rulesetAttacker,
                    position = attacker.LocationPosition
                };

                RulesetActor.InflictDamage(
                    damageRoll,
                    damageForm,
                    damageForm.DamageType,
                    applyFormsParams,
                    rulesetAttacker,
                    false,
                    rulesetCharacter.Guid,
                    false,
                    [],
                    new RollInfo(dieType, rolls, 0),
                    false,
                    out _);
                EffectHelpers.StartVisualEffect(attacker, defender, magicEffect);
            }
        }

        public int HandlerPriority => 10;

        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager instance,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect)
        {
            var battleManager = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (!battleManager)
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            if (helper != defender ||
                !defender.CanReact() ||
                rulesetEffect != null ||
                !ValidatorsWeapon.IsMelee(attackMode) ||
                rulesetDefender.GetRemainingPowerUses(powerReactiveHide) == 0)
            {
                yield break;
            }

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerReactiveHide, rulesetDefender);
            var actionParams =
                new CharacterActionParams(defender, ActionDefinitions.Id.PowerReaction)
                {
                    StringParameter = "ReactiveHide",
                    ActionModifiers = { new ActionModifier() },
                    RulesetEffect = implementationManager
                        .MyInstantiateEffectPower(rulesetDefender, usablePower, false),
                    UsablePower = usablePower,
                    TargetCharacters = { defender }
                };
            var count = actionService.PendingReactionRequestGroups.Count;

            actionService.ReactToUsePower(actionParams, "UsePower", defender);

            yield return battleManager.WaitForReactions(attacker, actionService, count);
        }
    }
}
