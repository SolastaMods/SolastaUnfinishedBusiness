using System.Collections;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class WayOfTheDragon : AbstractSubclass
{
    private const string Name = "WayOfTheDragon";

    internal WayOfTheDragon()
    {
        var featureWayOfDragonAncestry = FeatureDefinitionFeatureSetBuilder
            .Create(FeatureSetPathClawDragonAncestry, $"FeatureSet{Name}Ancestry")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .AddToDB();

        var featureWayOfDragonResistance = FeatureDefinitionFeatureSetBuilder
            .Create(FeatureSetSorcererDraconicResilience, $"FeatureSet{Name}Resistance")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .AddToDB();

        featureWayOfDragonResistance.ancestryType = AncestryType.BarbarianClaw;

        var featureWayOfTheDragonProficiency = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.SkillOrExpertise, SkillDefinitions.Stealth, SkillDefinitions.Perception)
            .AddToDB();

        var conditionReactiveHide = ConditionDefinitionBuilder
            .Create(ConditionFiendishResilienceFire, "ConditionReactiveHide")
            .SetGuiPresentation(Category.Feature, "PowerReactiveHide")
            .SetPossessive()
            .AddFeatures(
                DamageAffinityAcidResistance,
                DamageAffinityBludgeoningResistance,
                DamageAffinityColdResistance,
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
            .Create("PowerReactiveHide")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.KiPoints)
            .SetCustomSubFeatures(new ReactToAttackReactiveHide())
            .SetReactionContext(ReactionTriggerContext.DEPRECATED_DamagedBySpecificElement)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetParticleEffectParameters(PowerPatronHiveReactiveCarapace.EffectDescription
                        .effectParticleParameters)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder
                        .Create()
                        .SetConditionForm(
                            conditionReactiveHide,
                            ConditionForm.ConditionOperation.Add,
                            true,
                            true)
                        .Build())
                    .Build())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, SorcerousDraconicBloodline)
            .AddFeaturesAtLevel(3,
                powerReactiveHide,
                featureWayOfDragonAncestry,
                featureWayOfDragonResistance,
                SenseDarkvision,
                featureWayOfTheDragonProficiency)
            .AddFeaturesAtLevel(6, BuildDragonFeatureSet())
            .AddFeaturesAtLevel(11, BuildDragonFuryFeatureSet())
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        DatabaseHelper.FeatureDefinitionSubclassChoices.SubclassChoiceMonkMonasticTraditions;

    private static FeatureDefinitionFeatureSet BuildDragonFeatureSet()
    {
        var powerBlackElementalBreath = FeatureDefinitionPowerBuilder
            .Create(PowerDragonbornBreathWeaponBlack, "PowerBlackElementalBreath")
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.KiPoints, 3)
            .SetGuiPresentation(Category.Feature)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerDragonbornBreathWeaponBlack.EffectDescription)
                    .SetParticleEffectParameters(PowerDragonbornBreathWeaponBlack.EffectDescription
                        .effectParticleParameters)
                    .SetSavingThrowData(false,
                        AttributeDefinitions.Constitution,
                        false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                        AttributeDefinitions.Wisdom,
                        20)
                    .Build())
            .AddToDB();

        var powerBlueElementalBreath = FeatureDefinitionPowerBuilder
            .Create(PowerDragonbornBreathWeaponBlue, "PowerBlueElementalBreath")
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.KiPoints, 3)
            .SetGuiPresentation(Category.Feature, PowerDragonbornBreathWeaponBlue)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerDragonbornBreathWeaponBlue.EffectDescription)
                    .SetParticleEffectParameters(PowerDragonbornBreathWeaponBlue.EffectDescription
                        .effectParticleParameters)
                    .SetSavingThrowData(false,
                        AttributeDefinitions.Constitution,
                        false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                        AttributeDefinitions.Wisdom,
                        20)
                    .Build())
            .AddToDB();

        var powerGreenElementalBreath = FeatureDefinitionPowerBuilder
            .Create(PowerDragonbornBreathWeaponGreen, "PowerGreenElementalBreath")
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.KiPoints, 3)
            .SetGuiPresentation(Category.Feature, PowerDragonbornBreathWeaponGreen)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerDragonbornBreathWeaponGreen.EffectDescription)
                    .SetParticleEffectParameters(PowerDragonbornBreathWeaponGreen.EffectDescription
                        .effectParticleParameters)
                    .SetSavingThrowData(false,
                        AttributeDefinitions.Constitution,
                        false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                        AttributeDefinitions.Wisdom,
                        20)
                    .Build())
            .AddToDB();

        var powerGoldElementalBreath = FeatureDefinitionPowerBuilder
            .Create(PowerDragonbornBreathWeaponGold, "PowerGoldElementalBreath")
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.KiPoints, 3)
            .SetGuiPresentation(Category.Feature, PowerDragonbornBreathWeaponGold)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerDragonbornBreathWeaponGold.EffectDescription)
                    .SetParticleEffectParameters(PowerDragonbornBreathWeaponGold.EffectDescription
                        .effectParticleParameters)
                    .SetSavingThrowData(false,
                        AttributeDefinitions.Constitution,
                        false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                        AttributeDefinitions.Wisdom,
                        20)
                    .Build())
            .AddToDB();

        var powerSilverElementalBreath = FeatureDefinitionPowerBuilder
            .Create(PowerDragonbornBreathWeaponSilver, "PowerSilverElementalBreath")
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.KiPoints, 3)
            .SetGuiPresentation(Category.Feature, PowerDragonbornBreathWeaponSilver)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerDragonbornBreathWeaponSilver)
                    .SetParticleEffectParameters(PowerDragonbornBreathWeaponSilver.EffectDescription
                        .effectParticleParameters)
                    .SetSavingThrowData(false,
                        AttributeDefinitions.Constitution,
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
            .AddToDB();

        featureWayOfDragonBreath.ancestryType = AncestryType.BarbarianClaw;

        return featureWayOfDragonBreath;
    }

    private static FeatureDefinitionFeatureSet BuildDragonFuryFeatureSet()
    {
        const string NOTIFICATION_TAG = "DragonFury";

        var additionalDamageAcid = FeatureDefinitionAdditionalDamageBuilder
            .Create("AdditionalDamageDragonFuryAcid")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag(NOTIFICATION_TAG)
            .SetAdditionalDamageType(AdditionalDamageType.Specific)
            .SetImpactParticleReference(AcidSplash.EffectDescription.EffectParticleParameters.impactParticleReference)
            .SetSpecificDamageType(DamageTypeAcid)
            .SetDamageDice(DieType.D6, 2)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Unarmed)
            .AddToDB();

        var conditionDragonFuryAcid = ConditionDefinitionBuilder
            .Create("ConditionDragonFuryAcid")
            .SetGuiPresentation(Category.Condition, ConditionPactChainPseudodragon)
            .AddFeatures(additionalDamageAcid)
            .AddToDB();

        var powerDragonFuryAcid = FeatureDefinitionPowerBuilder
            .Create("PowerDragonFuryAcid")
            .SetGuiPresentation(Category.Feature, AcidSplash)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 2)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetParticleEffectParameters(AcidSplash.EffectDescription.effectParticleParameters)
                    .SetEffectForms(EffectFormBuilder
                        .Create()
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
            .Create("AdditionalDamageDragonFuryLightning")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag(NOTIFICATION_TAG)
            .SetAdditionalDamageType(AdditionalDamageType.Specific)
            .SetImpactParticleReference(
                LightningBolt.EffectDescription.effectParticleParameters.impactParticleReference)
            .SetSpecificDamageType(DamageTypeLightning)
            .SetDamageDice(DieType.D6, 2)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Unarmed)
            .AddToDB();

        var conditionDragonFuryLightning = ConditionDefinitionBuilder
            .Create("ConditionDragonFuryLightning")
            .SetGuiPresentation(Category.Condition, ConditionPactChainPseudodragon)
            .AddFeatures(additionalDamageLightning)
            .AddToDB();

        var powerDragonFuryLightning = FeatureDefinitionPowerBuilder
            .Create("PowerDragonFuryLightning")
            .SetGuiPresentation(Category.Feature, ShockingGrasp)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 2)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetParticleEffectParameters(LightningBolt.EffectDescription.effectParticleParameters)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder
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
            .Create("AdditionalDamageDragonFuryPoison").SetGuiPresentation(Category.Feature)
            .SetNotificationTag(NOTIFICATION_TAG)
            .SetAdditionalDamageType(AdditionalDamageType.Specific)
            .SetImpactParticleReference(PoisonSpray.EffectDescription.effectParticleParameters.impactParticleReference)
            .SetSpecificDamageType(DamageTypePoison)
            .SetDamageDice(DieType.D6, 2)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Unarmed)
            .AddToDB();

        var conditionDragonFuryPoison = ConditionDefinitionBuilder
            .Create("ConditionDragonFuryPoison")
            .SetGuiPresentation(Category.Condition, ConditionPactChainPseudodragon)
            .AddFeatures(additionalDamagePoison)
            .AddToDB();

        var powerDragonFuryPoison = FeatureDefinitionPowerBuilder
            .Create("PowerDragonFuryPoison")
            .SetGuiPresentation(Category.Feature, PoisonSpray)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 2)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetParticleEffectParameters(PoisonSpray.EffectDescription.effectParticleParameters)
                    .SetEffectForms(EffectFormBuilder
                        .Create()
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
            .Create("AdditionalDamageDragonFuryFire")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag(NOTIFICATION_TAG)
            .SetAdditionalDamageType(AdditionalDamageType.Specific)
            .SetSpecificDamageType(DamageTypeFire)
            .SetImpactParticleReference(Fireball.EffectDescription.effectParticleParameters.impactParticleReference)
            .SetDamageDice(DieType.D6, 2)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Unarmed)
            .AddToDB();

        var conditionDragonFuryFire = ConditionDefinitionBuilder
            .Create("ConditionDragonFuryFire")
            .SetGuiPresentation(Category.Condition, ConditionPactChainPseudodragon)
            .AddFeatures(additionalDamageFire)
            .AddToDB();

        var powerDragonFuryFire = FeatureDefinitionPowerBuilder
            .Create("PowerDragonFuryFire")
            .SetGuiPresentation(Category.Feature, ProduceFlame)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 2)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetParticleEffectParameters(Fireball.EffectDescription
                        .effectParticleParameters)
                    .SetEffectForms(EffectFormBuilder
                        .Create()
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
            .Create("AdditionalDamageDragonFuryCold")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag(NOTIFICATION_TAG)
            .SetAdditionalDamageType(AdditionalDamageType.Specific)
            .SetImpactParticleReference(ConeOfCold.EffectDescription.effectParticleParameters.impactParticleReference)
            .SetSpecificDamageType(DamageTypeCold)
            .SetDamageDice(DieType.D6, 2)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Unarmed)
            .AddToDB();

        var conditionDragonFuryCold = ConditionDefinitionBuilder
            .Create("ConditionDragonFuryCold")
            .SetGuiPresentation(Category.Condition, ConditionPactChainPseudodragon)
            .AddFeatures(additionalDamageCold)
            .AddToDB();

        var powerDragonFuryCold = FeatureDefinitionPowerBuilder
            .Create("PowerDragonFuryCold")
            .SetGuiPresentation(Category.Feature, RayOfFrost)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 2)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetParticleEffectParameters(ConeOfCold.EffectDescription.effectParticleParameters)
                    .SetEffectForms(EffectFormBuilder
                        .Create()
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
            .AddToDB();

        featureWayOfDragonFury.ancestryType = AncestryType.BarbarianClaw;

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

            if (!me.RulesetCharacter.HasConditionOfType("ConditionReactiveHide"))
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

            TryGetAncestryDamageTypeFromCharacter(me.Guid, AncestryType.BarbarianClaw, out var damageType);

            switch (damageType)
            {
                case null:
                    yield break;
                case DamageTypeAcid:
                {
                    var myLevel = me.RulesetCharacter.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;

                    var damageInt = myLevel switch
                    {
                        <= 4 => 8,
                        <= 10 => 12,
                        <= 16 => 16,
                        <= 20 => 20,
                        _ => 0
                    };

                    if (savingOutcome == RollOutcome.Success)
                    {
                        attacker.RulesetCharacter.SustainDamage(damageInt / 2, DamageTypeAcid, false, me.Guid, null,
                            out _);
                    }
                    else
                    {
                        attacker.RulesetCharacter.SustainDamage(damageInt, DamageTypeAcid, false, me.Guid, null,
                            out _);
                    }

                    yield break;
                }
                case DamageTypeLightning when savingOutcome == RollOutcome.Success:
                    yield break;
                case DamageTypeLightning:
                    ApplyReactiveHideDebuff(attacker.RulesetCharacter, ConditionShocked);
                    yield break;
                case DamageTypeFire when savingOutcome == RollOutcome.Success:
                    yield break;
                case DamageTypeFire:
                    ApplyReactiveHideDebuff(attacker.RulesetCharacter, ConditionOnFire);
                    yield break;
                case DamageTypePoison when savingOutcome == RollOutcome.Success:
                    yield break;
                case DamageTypePoison:
                    ApplyReactiveHideDebuff(attacker.RulesetCharacter,
                        DatabaseHelper.ConditionDefinitions.ConditionPoisoned);
                    yield break;
                case DamageTypeCold when savingOutcome == RollOutcome.Success:
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
