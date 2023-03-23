using System.Collections;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class WayOfTheDragon : AbstractSubclass
{
    internal const string Name = "WayOfTheDragon";

    internal static readonly FeatureDefinitionFeatureSet FeatureSetPathOfTheDragonDisciple =
        FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}Disciple")
            .SetGuiPresentation("PathClawDragonAncestry", Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
            .SetAncestryType(ExtraAncestryType.WayOfTheDragon)
            .AddToDB();

    internal WayOfTheDragon()
    {
        var damageAffinityAncestry = FeatureDefinitionDamageAffinityBuilder
            .Create($"DamageAffinity{Name}Ancestry")
            .SetGuiPresentation("DragonbornDamageResistance", Category.Feature)
            .SetAncestryType(ExtraAncestryType.WayOfTheDragon)
            .SetDamageAffinityType(DamageAffinityType.Resistance)
            .AddToDB();

        var powerReactiveHide = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ReactiveHide")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.KiPoints)
            .SetCustomSubFeatures(new ReactToAttackReactiveHide())
            .SetReactionContext(ExtraReactionContext.Custom)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Round, 1)
                .SetParticleEffectParameters(PowerPatronHiveReactiveCarapace)
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(ConditionDefinitionBuilder
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
                            .AddToDB(),
                        ConditionForm.ConditionOperation.Add,
                        true,
                        true)
                    .Build())
                .Build())
            .AddToDB();

        /*
        Level 17 - Ascension
        As a free action, you may spend 4 Ki points to grow a pair of wings and gain the effects of Fly spell, without needing to concentrate for up to 1 minute. While this ability lasts, you gain +2 AC and access to Wing Sweep ability.
        */
        var conditionAscension = ConditionDefinitionBuilder
            .Create(ConditionFlying12, $"Condition{Name}Ascension")
            .SetGuiPresentation(Category.Condition, ConditionFlying12)
            .AddFeatures(FeatureDefinitionAttributeModifiers.AttributeModifierHasted)
            .AddToDB();

        var powerAscension = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Ascension")
            .SetGuiPresentation(Category.Feature, Fly)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 4)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .AddEffectForms(EffectFormBuilder
                    .Create()
                    .SetConditionForm(conditionAscension, ConditionForm.ConditionOperation.Add)
                    .Build())
                .SetDurationData(DurationType.Minute, 1)
                .Build())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite("WayOfTheDragon", Resources.WayOfTheDragon, 256))
            .AddFeaturesAtLevel(3, BuildDiscipleFeatureSet(), damageAffinityAncestry, powerReactiveHide)
            .AddFeaturesAtLevel(6, BuildDragonFeatureSet())
            .AddFeaturesAtLevel(11, BuildDragonFuryFeatureSet())
            .AddFeaturesAtLevel(17, powerAscension)
            .AddToDB();
    }

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
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetGuiPresentation(Category.Feature, PowerDragonbornBreathWeaponBlack)
            .SetEffectDescription(EffectDescriptionBuilder
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

        var powerBlueElementalBreath = FeatureDefinitionPowerBuilder
            .Create(PowerDragonbornBreathWeaponBlue, $"Power{Name}ElementalBreathBlue")
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetGuiPresentation(Category.Feature, PowerDragonbornBreathWeaponBlue)
            .SetEffectDescription(EffectDescriptionBuilder
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

        var effectGreenElementalBreath = EffectProxyDefinitionBuilder
            .Create(EffectProxyDefinitions.ProxyStinkingCloud, "EffectGreenElementalBreath")
            .AddToDB();

        var powerGreenElementalBreath = FeatureDefinitionPowerBuilder
            .Create(PowerDragonbornBreathWeaponGreen, $"Power{Name}ElementalBreathGreen")
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetGuiPresentation(Category.Feature, PowerDragonbornBreathWeaponGreen)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create(PowerDragonbornBreathWeaponGreen.EffectDescription)
                .SetParticleEffectParameters(StinkingCloud)
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Sphere, 3)
                .SetDurationData(DurationType.Round, 3)
                .AddEffectForms(EffectFormBuilder.Create()
                        .SetSummonEffectProxyForm(effectGreenElementalBreath)
                        .Build(),
                    StinkingCloud.EffectDescription
                        .effectForms.Find(e => e.formType == EffectForm.EffectFormType.Topology))
                .SetSavingThrowData(false,
                    AttributeDefinitions.Dexterity,
                    false,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    AttributeDefinitions.Wisdom,
                    20)
                .Build())
            .AddToDB();

        var powerGoldElementalBreath = FeatureDefinitionPowerBuilder
            .Create(PowerDragonbornBreathWeaponGold, $"Power{Name}ElementalBreathGold")
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetGuiPresentation(Category.Feature, PowerDragonbornBreathWeaponGold)
            .SetEffectDescription(EffectDescriptionBuilder
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

        var powerSilverElementalBreath = FeatureDefinitionPowerBuilder
            .Create(PowerDragonbornBreathWeaponSilver, $"Power{Name}ElementalBreathSilver")
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetGuiPresentation(Category.Feature, PowerDragonbornBreathWeaponSilver)
            .SetEffectDescription(EffectDescriptionBuilder
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

        var featureWayOfDragonBreath = FeatureDefinitionFeatureSetBuilder
            .Create(FeatureSetDragonbornBreathWeapon, $"FeatureSet{Name}Breath")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .ClearFeatureSet()
            .AddFeatureSetNoSort(
                powerBlackElementalBreath,
                powerBlueElementalBreath,
                powerGoldElementalBreath,
                powerGreenElementalBreath,
                powerSilverElementalBreath)
            .SetAncestryType(ExtraAncestryType.WayOfTheDragon)
            .AddToDB();

        return featureWayOfDragonBreath;
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
            .SetCustomSubFeatures(ValidatorsPowerUse.HasNoneOfConditions(conditionDragonFuryAcid.Name))
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 2)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Round, 1)
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetParticleEffectParameters(AcidSplash)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(
                        conditionDragonFuryAcid,
                        ConditionForm.ConditionOperation.Add,
                        true,
                        true
                    )
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
            .SetCustomSubFeatures(ValidatorsPowerUse.HasNoneOfConditions(conditionDragonFuryLightning.Name))
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 2)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Round, 1)
                .SetParticleEffectParameters(LightningBolt)
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetEffectForms(EffectFormBuilder.Create()
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
            .SetCustomSubFeatures(ValidatorsPowerUse.HasNoneOfConditions(conditionDragonFuryPoison.Name))
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 2)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Round, 1)
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetParticleEffectParameters(PoisonSpray)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(
                        conditionDragonFuryPoison,
                        ConditionForm.ConditionOperation.Add,
                        true,
                        true
                    )
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
            .SetCustomSubFeatures(ValidatorsPowerUse.HasNoneOfConditions(conditionDragonFuryFire.Name))
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 2)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Round, 1)
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetParticleEffectParameters(Fireball)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(
                        conditionDragonFuryFire,
                        ConditionForm.ConditionOperation.Add,
                        true,
                        true
                    )
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
            .SetCustomSubFeatures(ValidatorsPowerUse.HasNoneOfConditions(conditionDragonFuryCold.Name))
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 2)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Round, 1)
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetParticleEffectParameters(ConeOfCold)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetConditionForm(
                        conditionDragonFuryCold,
                        ConditionForm.ConditionOperation.Add,
                        true,
                        true
                    )
                    .Build())
                .Build())
            .AddToDB();

        var featureWayOfDragonFury = FeatureDefinitionFeatureSetBuilder
            .Create(FeatureSetDragonbornBreathWeapon, $"FeatureSet{Name}Fury")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .ClearFeatureSet()
            .AddFeatureSetNoSort(
                powerDragonFuryAcid,
                powerDragonFuryLightning,
                powerDragonFuryFire,
                powerDragonFuryPoison,
                powerDragonFuryCold)
            .SetAncestryType(ExtraAncestryType.WayOfTheDragon)
            .AddToDB();

        return featureWayOfDragonFury;
    }

    private sealed class ReactToAttackReactiveHide : IReactToAttackOnMeFinished
    {
        public IEnumerator HandleReactToAttackOnMeFinished(
            GameLocationCharacter attacker,
            GameLocationCharacter me,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode mode,
            ActionModifier modifier)
        {
            if (outcome is RollOutcome.Failure or RollOutcome.CriticalFailure)
            {
                yield break;
            }

            if (me.RulesetCharacter.RemainingKiPoints == 0)
            {
                yield break;
            }

            if (!me.RulesetCharacter.HasConditionOfType($"Condition{Name}ReactiveHide"))
            {
                yield break;
            }

            var modifierTrend = attacker.RulesetCharacter.actionModifier.savingThrowModifierTrends;
            var advantageTrends = attacker.RulesetCharacter.actionModifier.savingThrowAdvantageTrends;
            var attackerConModifier = AttributeDefinitions.ComputeAbilityScoreModifier(attacker.RulesetCharacter
                .GetAttribute(AttributeDefinitions.Constitution).CurrentValue);
            var profBonus = AttributeDefinitions.ComputeProficiencyBonus(me.RulesetCharacter
                .GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue);
            var myWisModifier = AttributeDefinitions.ComputeAbilityScoreModifier(me.RulesetCharacter
                .GetAttribute(AttributeDefinitions.Wisdom).CurrentValue);

            attacker.RulesetCharacter.RollSavingThrow(0, AttributeDefinitions.Constitution, null, modifierTrend,
                advantageTrends, attackerConModifier, 8 + profBonus + myWisModifier, false, out var savingOutcome,
                out _);

            if (savingOutcome == RollOutcome.Success)
            {
                yield break;
            }

            TryGetAncestryDamageTypeFromCharacter(me.Guid, (AncestryType)ExtraAncestryType.WayOfTheDragon,
                out var damageType);

            var myLevel = me.RulesetCharacter.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;

            var damageInt = myLevel switch
            {
                <= 4 => 8,
                <= 10 => 12,
                <= 16 => 16,
                <= 20 => 20,
                _ => 0
            };

            switch (damageType)
            {
                case null:
                    yield break;
                case DamageTypeAcid when attacker.RulesetCharacter.HasConditionOfType(ConditionAcidArrowed.Name):
                    attacker.RulesetCharacter.SustainDamage(damageInt, DamageTypeAcid, false, me.Guid, null,
                        out _);
                    yield break;
                case DamageTypeAcid:
                    ApplyReactiveHideDebuff(attacker.RulesetCharacter, ConditionAcidArrowed);
                    yield break;
                case DamageTypeLightning when attacker.RulesetCharacter.HasConditionOfType(ConditionShocked.Name):
                    attacker.RulesetCharacter.SustainDamage(damageInt, DamageTypeLightning, false, me.Guid, null,
                        out _);
                    yield break;
                case DamageTypeLightning:
                    ApplyReactiveHideDebuff(attacker.RulesetCharacter, ConditionShocked);
                    yield break;
                case DamageTypeFire when attacker.RulesetCharacter.HasConditionOfType(ConditionOnFire.Name):
                    attacker.RulesetCharacter.SustainDamage(damageInt, DamageTypeFire, false, me.Guid, null,
                        out _);
                    yield break;
                case DamageTypeFire:
                    ApplyReactiveHideDebuff(attacker.RulesetCharacter, ConditionOnFire);
                    yield break;
                case DamageTypePoison
                    when attacker.RulesetCharacter.HasConditionOfType(ConditionDefinitions.ConditionPoisoned.Name):
                    attacker.RulesetCharacter.SustainDamage(damageInt, DamageTypePoison, false, me.Guid, null,
                        out _);
                    yield break;
                case DamageTypePoison:
                    ApplyReactiveHideDebuff(attacker.RulesetCharacter,
                        ConditionDefinitions.ConditionPoisoned);
                    yield break;
                case DamageTypeCold when attacker.RulesetCharacter.HasConditionOfType(ConditionHindered_By_Frost.Name):
                    attacker.RulesetCharacter.SustainDamage(damageInt, DamageTypeCold, false, me.Guid, null,
                        out _);
                    yield break;
                case DamageTypeCold:
                    ApplyReactiveHideDebuff(attacker.RulesetCharacter, ConditionHindered_By_Frost);
                    yield break;
            }
        }

        private static void ApplyReactiveHideDebuff(RulesetCharacter attacker, ConditionDefinition debuff)
        {
            var rulesetCondition = RulesetCondition.CreateActiveCondition(
                attacker.Guid,
                debuff,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                attacker.Guid,
                attacker.CurrentFaction.Name
            );

            attacker.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
        }
    }
}
