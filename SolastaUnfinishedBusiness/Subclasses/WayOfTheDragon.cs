using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Properties;
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

    internal static readonly FeatureDefinitionFeatureSet FeatureSetPathOfTheDragonDisciple =
        FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}Disciple")
            .SetGuiPresentation("PathClawDragonAncestry", Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
            .SetAncestryType(ExtraAncestryType.WayOfTheDragon)
            .AddToDB();

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

            FeatureSetPathOfTheDragonDisciple.FeatureSet.Add(ancestry);
        }

        return FeatureSetPathOfTheDragonDisciple;
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
                                    .Create(EffectProxyDefinitions.ProxyStinkingCloud, "EffectGreenElementalBreath")
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
            .AddSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var powerReactiveHide = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ReactiveHide")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.KiPoints)
            .SetReactionContext(ExtraReactionContext.Custom)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetParticleEffectParameters(PowerPatronHiveReactiveCarapace)
                    .Build())
            .AddToDB();

        powerReactiveHide.AddCustomSubFeatures(
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
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var powerDragonFuryAcid = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DragonFuryAcid")
            .SetGuiPresentation(Category.Feature, AcidSplash)
            .AddCustomSubFeatures(ValidatorsValidatePowerUse.HasNoneOfConditions(conditionDragonFuryAcid.Name))
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 2, 2)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetParticleEffectParameters(AcidSplash)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                conditionDragonFuryAcid,
                                ConditionForm.ConditionOperation.Add,
                                true,
                                true)
                            .Build())
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
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var powerDragonFuryLightning = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DragonFuryLightning")
            .SetGuiPresentation(Category.Feature, ShockingGrasp)
            .AddCustomSubFeatures(ValidatorsValidatePowerUse.HasNoneOfConditions(conditionDragonFuryLightning.Name))
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 2, 2)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetParticleEffectParameters(LightningBolt)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                conditionDragonFuryLightning,
                                ConditionForm.ConditionOperation.Add,
                                true,
                                true)
                            .Build())
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
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var powerDragonFuryPoison = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DragonFuryPoison")
            .SetGuiPresentation(Category.Feature, PoisonSpray)
            .AddCustomSubFeatures(ValidatorsValidatePowerUse.HasNoneOfConditions(conditionDragonFuryPoison.Name))
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 2, 2)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetParticleEffectParameters(PoisonSpray)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                conditionDragonFuryPoison,
                                ConditionForm.ConditionOperation.Add,
                                true,
                                true)
                            .Build())
                    .Build())
            .AddToDB();

        var additionalDamageFire = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}DragonFuryFire")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag(NOTIFICATION_TAG + DamageTypeFire)
            .SetAdditionalDamageType(AdditionalDamageType.Specific)
            .SetSpecificDamageType(DamageTypeFire)
            .SetImpactParticleReference(Fireball)
            .SetDamageDice(DieType.D6, 2)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Unarmed)
            .AddToDB();

        var conditionDragonFuryFire = ConditionDefinitionBuilder
            .Create($"Condition{Name}DragonFuryFire")
            .SetGuiPresentation(Category.Condition, ConditionPactChainPseudodragon)
            .SetPossessive()
            .AddFeatures(additionalDamageFire)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var powerDragonFuryFire = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DragonFuryFire")
            .SetGuiPresentation(Category.Feature, ProduceFlame)
            .AddCustomSubFeatures(ValidatorsValidatePowerUse.HasNoneOfConditions(conditionDragonFuryFire.Name))
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 2, 2)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetParticleEffectParameters(Fireball)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                conditionDragonFuryFire,
                                ConditionForm.ConditionOperation.Add,
                                true,
                                true)
                            .Build())
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
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var powerDragonFuryCold = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DragonFuryCold")
            .SetGuiPresentation(Category.Feature, RayOfFrost)
            .AddCustomSubFeatures(ValidatorsValidatePowerUse.HasNoneOfConditions(conditionDragonFuryCold.Name))
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 2, 2)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetParticleEffectParameters(ConeOfCold)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                conditionDragonFuryCold,
                                ConditionForm.ConditionOperation.Add,
                                true,
                                true)
                            .Build())
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

            damageForm.DieType = character.GetMonkDieType();

            return effectDescription;
        }

        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower featureDefinitionPower)
        {
            var usablePower = UsablePowersProvider.Get(powerElementalBreathProficiency, character);

            if (Gui.Battle == null)
            {
                return usablePower.RemainingUses > 0;
            }

            var glc = GameLocationCharacter.GetFromActor(character);

            if (glc == null || !glc.OnceInMyTurnIsValid("ElementalBreath"))
            {
                return false;
            }

            return usablePower.RemainingUses > 0;
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

            damageForm.DieType = character.GetMonkDieType();

            return effectDescription;
        }

        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower featureDefinitionPower)
        {
            var usablePower = UsablePowersProvider.Get(powerElementalBreathProficiency, character);

            if (Gui.Battle == null)
            {
                return usablePower.RemainingUses == 0;
            }

            var glc = GameLocationCharacter.GetFromActor(character);

            if (glc == null || !glc.OnceInMyTurnIsValid("ElementalBreath"))
            {
                return false;
            }

            return usablePower.RemainingUses == 0;
        }
    }

    //
    // Reactive Hide
    //

    private sealed class CustomBehaviorReactiveHide(
        FeatureDefinitionPower powerReactiveHide,
        ConditionDefinition conditionReactiveHide)
        :
            IAttackBeforeHitConfirmedOnMe, IMagicalAttackBeforeHitConfirmedOnMe, IPhysicalAttackFinishedOnMe
    {
        public IEnumerator OnAttackBeforeHitConfirmedOnMe(GameLocationBattleManager battle,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool firstTarget,
            bool criticalHit)
        {
            if (attackMode != null)
            {
                yield return HandleReaction(defender);
            }
        }

        public IEnumerator OnMagicalAttackBeforeHitConfirmedOnMe(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier magicModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            yield return HandleReaction(defender);
        }

        public IEnumerator OnPhysicalAttackFinishedOnMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackerAttackMode,
            RollOutcome attackRollOutcome,
            int damageAmount)
        {
            if (attackRollOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure)
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
                        rulesetAttacker.RollDamage(damageForm, 0, false, 0, 0, 1, false, false, false, rolls);

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
                        rulesetAttacker.RollDamage(damageForm, 0, false, 0, 0, 1, false, false, false, rolls);

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
                        rulesetAttacker.RollDamage(damageForm, 0, false, 0, 0, 1, false, false, false, rolls);

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
                        rulesetAttacker.RollDamage(damageForm, 0, false, 0, 0, 1, false, false, false, rolls);

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
                        rulesetAttacker.RollDamage(damageForm, 0, false, 0, 0, 1, false, false, false, rolls);

                    InflictDamage(PowerDragonbornBreathWeaponSilver);

                    yield break;
                }
            }

            yield break;

            void InflictDamage(IMagicEffect magicEffect)
            {
                RulesetActor.InflictDamage(
                    damageRoll,
                    damageForm,
                    damageForm.DamageType,
                    new RulesetImplementationDefinitions.ApplyFormsParams { targetCharacter = rulesetAttacker },
                    rulesetAttacker,
                    false,
                    rulesetAttacker.Guid,
                    false,
                    attackerAttackMode.AttackTags,
                    new RollInfo(dieType, rolls, 0),
                    true,
                    out _);
                EffectHelpers.StartVisualEffect(attacker, defender, magicEffect);
            }
        }

        private IEnumerator HandleReaction(GameLocationCharacter defender)
        {
            if (!defender.CanReact())
            {
                yield break;
            }

            var rulesetMe = defender.RulesetCharacter;

            if (!rulesetMe.CanUsePower(powerReactiveHide))
            {
                yield break;
            }

            var gameLocationActionService =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var gameLocationBattleService =
                ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (gameLocationActionService == null || gameLocationBattleService is not { IsBattleInProgress: true })
            {
                yield break;
            }

            var usablePower = UsablePowersProvider.Get(powerReactiveHide, rulesetMe);
            var reactionParams =
                new CharacterActionParams(defender, (ActionDefinitions.Id)ExtraActionId.DoNothingReaction)
                {
                    StringParameter = $"{Name}ReactiveHide", UsablePower = usablePower
                };

            var previousReactionCount = gameLocationActionService.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestSpendPower(reactionParams);

            gameLocationActionService.AddInterruptRequest(reactionRequest);

            yield return gameLocationBattleService.WaitForReactions(
                defender, gameLocationActionService, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            rulesetMe.UsePower(usablePower);
            rulesetMe.InflictCondition(
                conditionReactiveHide.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetMe.guid,
                rulesetMe.CurrentFaction.Name,
                1,
                conditionReactiveHide.Name,
                0,
                0,
                0);
        }
    }
}
