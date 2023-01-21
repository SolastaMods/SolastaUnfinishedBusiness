using System.Collections;
using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Subclasses;
internal sealed class WayOfTheDragon : AbstractSubclass
{
    internal WayOfTheDragon()
    {

        const string NAME = "WayOfTheDragon";

        var featureWayOfDragonAncestry = FeatureDefinitionFeatureSetBuilder
            .Create(FeatureDefinitionFeatureSets.FeatureSetPathClawDragonAncestry, $"Feature{NAME}Ancestry")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .AddToDB();


        var featureWayOfDragonResistance = FeatureDefinitionFeatureSetBuilder
            .Create(FeatureDefinitionFeatureSets.FeatureSetSorcererDraconicResilience, $"Feature{NAME}Resistance")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .AddToDB();

        featureWayOfDragonResistance.ancestryType = AncestryType.BarbarianClaw;

        var featureWayOfTheDragonProficiency = FeatureDefinitionProficiencyBuilder
            .Create($"Feature{NAME}Proficiency")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.SkillOrExpertise, SkillDefinitions.Stealth, SkillDefinitions.Perception)
            .AddToDB();

        var conditionReactiveHide = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionFiendishResilienceFire,"ConditionReactiveHide")
            .SetGuiPresentation(Category.Feature,"PowerReactiveHide")
            .SetPossessive()
            .AddFeatures(DamageAffinityAcidResistance, DamageAffinityBludgeoningResistance, DamageAffinityColdResistance, DamageAffinityLightningResistance,
            DamageAffinityNecroticResistance, DamageAffinityForceDamageResistance, DamageAffinityPiercingResistance, DamageAffinityRadiantResistance,
            DamageAffinityPoisonResistance, DamageAffinityPsychicResistance, DamageAffinitySlashingResistance, DamageAffinityThunderResistance)
            .AddSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var powerReactiveHide = FeatureDefinitionPowerBuilder
         .Create("PowerReactiveHide")
         .SetGuiPresentation(Category.Feature)
         .SetUsesFixed(ActivationTime.Reaction, RechargeRate.KiPoints)
         .SetCustomSubFeatures(new ReactToAttckReactiveHide())
         .SetReactionContext(ReactionTriggerContext.DEPRECATED_DamagedBySpecificElement)
         .SetEffectDescription(
             EffectDescriptionBuilder
                 .Create()
                 .SetDurationData(DurationType.Round, 1)
                 .SetParticleEffectParameters(PowerPatronHiveReactiveCarapace.EffectDescription.effectParticleParameters)
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
            .Create(NAME)
            .SetGuiPresentation(Category.Subclass, SorcerousDraconicBloodline)
            .AddFeaturesAtLevel(3, powerReactiveHide, featureWayOfDragonAncestry, featureWayOfDragonResistance, FeatureDefinitionSenses.SenseDarkvision,
            featureWayOfTheDragonProficiency)
            .AddFeaturesAtLevel(6, BuildDragonFeatureSet())
            .AddFeaturesAtLevel(11, BuildDragonFuryFeatureSet())
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }


    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceMonkMonasticTraditions;

    private static FeatureDefinitionFeatureSet BuildDragonFeatureSet()
    {

        const string NAME = "WayOfTheDragon";

        var powerBlackElementalBreath = FeatureDefinitionPowerBuilder
    .Create(PowerDragonbornBreathWeaponBlack, "PowerBlackElementalBreath")
    .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.KiPoints, 3)
    .SetGuiPresentation(Category.Feature, PowerDragonbornBreathWeaponBlack)
    .SetEffectDescription(
        EffectDescriptionBuilder
        .Create(PowerDragonbornBreathWeaponBlack.EffectDescription)
        .SetParticleEffectParameters(PowerDragonbornBreathWeaponBlack.EffectDescription.effectParticleParameters)
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
        .SetParticleEffectParameters(PowerDragonbornBreathWeaponBlue.EffectDescription.effectParticleParameters)
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
        .SetParticleEffectParameters(PowerDragonbornBreathWeaponGreen.EffectDescription.effectParticleParameters)
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
        .SetParticleEffectParameters(PowerDragonbornBreathWeaponGold.EffectDescription.effectParticleParameters)
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
        .SetParticleEffectParameters(PowerDragonbornBreathWeaponSilver.EffectDescription.effectParticleParameters)
        .SetSavingThrowData(false,
        AttributeDefinitions.Constitution,
        false,
        EffectDifficultyClassComputation.AbilityScoreAndProficiency,
        AttributeDefinitions.Wisdom,
        20)
        .Build())
    .AddToDB();
 
        var featureWayOfDragonBreath = FeatureDefinitionFeatureSetBuilder
             .Create(FeatureDefinitionFeatureSets.FeatureSetDragonbornBreathWeapon, $"Feature{NAME}Breath")
             .SetOrUpdateGuiPresentation(Category.Feature)
             .ClearFeatureSet()
             .AddFeatureSetNoSort(powerBlackElementalBreath,
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

        const string NAME = "WayOfTheDragon";

        const string NAME2 = "Dragon's Fury";

        var additionalDamageAcid = FeatureDefinitionAdditionalDamageBuilder
        .Create("AdditionalDamageDragonFuryAcid")
        .SetGuiPresentation(Category.Feature)
        .SetNotificationTag(NAME2)
        .SetAdditionalDamageType(AdditionalDamageType.Specific)
        .SetImpactParticleReference(SpellDefinitions.AcidSplash.EffectDescription.EffectParticleParameters.impactParticleReference)
        .SetSpecificDamageType(DamageTypeAcid)
        .SetDamageDice(DieType.D6, 2)
        .SetRequiredProperty(RestrictedContextRequiredProperty.Unarmed)
        .AddToDB();

        var conditionDragonFuryAcid = ConditionDefinitionBuilder
    .Create("ConditionDragonFuryAcid")
    .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionPactChainPseudodragon)
    .AddFeatures(additionalDamageAcid)
    .AddToDB();

        var powerDragonFuryAcid = FeatureDefinitionPowerBuilder
            .Create("PowerDragonFuryAcid")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.AcidSplash)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 2)
            .SetEffectDescription(
                EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Round, 1)
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetParticleEffectParameters(SpellDefinitions.AcidSplash.EffectDescription.effectParticleParameters)
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
            .SetNotificationTag(NAME2)
            .SetAdditionalDamageType(AdditionalDamageType.Specific)
            .SetImpactParticleReference(SpellDefinitions.LightningBolt.EffectDescription.effectParticleParameters.impactParticleReference)
            .SetSpecificDamageType(DamageTypeLightning)
            .SetDamageDice(DieType.D6, 2)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Unarmed)
            .AddToDB();

        var conditionDragonFuryLightning = ConditionDefinitionBuilder
            .Create("ConditionDragonFuryLightning")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionPactChainPseudodragon)
            .AddFeatures(additionalDamageLightning)
            .AddToDB();

        var powerDragonFuryLightning = FeatureDefinitionPowerBuilder
            .Create("PowerDragonFuryLightning")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.ShockingGrasp)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 2)
            .SetEffectDescription(
                EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Round, 1)
                .SetParticleEffectParameters(SpellDefinitions.LightningBolt.EffectDescription.effectParticleParameters)
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetEffectForms(EffectFormBuilder
                    .Create()
                    .SetConditionForm(
                    conditionDragonFuryLightning,
                    ConditionForm.ConditionOperation.Add,
                    true,
                    true
                     )
                    .Build())
                .Build())
            .AddToDB();

        var additionalDamagePoison = FeatureDefinitionAdditionalDamageBuilder
            .Create("AdditionalDamageDragonFuryPoison")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag(NAME2)
            .SetAdditionalDamageType(AdditionalDamageType.Specific)
            .SetImpactParticleReference(SpellDefinitions.PoisonSpray.EffectDescription.effectParticleParameters.impactParticleReference)
            .SetSpecificDamageType(DamageTypePoison)
            .SetDamageDice(DieType.D6, 2)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Unarmed)
            .AddToDB();

        var conditionDragonFuryPoison = ConditionDefinitionBuilder
            .Create("ConditionDragonFuryPoison")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionPactChainPseudodragon)
            .AddFeatures(additionalDamagePoison)
            .AddToDB();

        var powerDragonFuryPoison = FeatureDefinitionPowerBuilder
            .Create("PowerDragonFuryPoison")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.PoisonSpray)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 2)
            .SetEffectDescription(
                EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Round, 1)
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetParticleEffectParameters(SpellDefinitions.PoisonSpray.EffectDescription.effectParticleParameters)
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
            .SetNotificationTag(NAME2)
            .SetAdditionalDamageType(AdditionalDamageType.Specific)
            .SetSpecificDamageType(DamageTypeFire)
            .SetImpactParticleReference(SpellDefinitions.Fireball.EffectDescription.effectParticleParameters.impactParticleReference)
            .SetDamageDice(DieType.D6, 2)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Unarmed)
            .AddToDB();

        var conditionDragonFuryFire = ConditionDefinitionBuilder
            .Create("ConditionDragonFuryFire")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionPactChainPseudodragon)
            .AddFeatures(additionalDamageFire)
            .AddToDB();

        var powerDragonFuryFire = FeatureDefinitionPowerBuilder
            .Create("PowerDragonFuryFire")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.ProduceFlame)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 2)
            .SetEffectDescription(
                EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Round, 1)
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetParticleEffectParameters(SpellDefinitions.Fireball.EffectDescription.effectParticleParameters)
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
            .SetNotificationTag(NAME2)
            .SetAdditionalDamageType(AdditionalDamageType.Specific)
            .SetImpactParticleReference(SpellDefinitions.ConeOfCold.EffectDescription.effectParticleParameters.impactParticleReference)
            .SetSpecificDamageType(DamageTypeCold)
            .SetDamageDice(DieType.D6, 2)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Unarmed)
            .AddToDB();

        var conditionDragonFuryCold = ConditionDefinitionBuilder
            .Create("ConditionDragonFuryCold")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionPactChainPseudodragon)
            .AddFeatures(additionalDamageCold)
            .AddToDB();

        var powerDragonFuryCold = FeatureDefinitionPowerBuilder
        .Create("PowerDragonFuryCold")
        .SetGuiPresentation(Category.Feature, SpellDefinitions.RayOfFrost)
        .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 2)
        .SetEffectDescription(
        EffectDescriptionBuilder
        .Create()
        .SetDurationData(DurationType.Round, 1)
        .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
        .SetParticleEffectParameters(SpellDefinitions.ConeOfCold.EffectDescription.effectParticleParameters)
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
        .Create(FeatureDefinitionFeatureSets.FeatureSetDragonbornBreathWeapon, $"Feature{NAME}Fury")
        .SetOrUpdateGuiPresentation(Category.Feature)
         .ClearFeatureSet()
         .AddFeatureSetNoSort(powerDragonFuryAcid,
         powerDragonFuryLightning,
         powerDragonFuryFire,
         powerDragonFuryPoison,
         powerDragonFuryCold)
        .AddToDB();

        featureWayOfDragonFury.ancestryType = AncestryType.BarbarianClaw;

        return featureWayOfDragonFury;
    }


    private sealed class ReactToAttckReactiveHide : IReactToAttackOnMeFinished
    {

        private void ApplyReactiveHideDebuff(RulesetCharacter attacker, ConditionDefinition debuff)
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

        public IEnumerator HandleReactToAttackOnMeFinished(
        GameLocationCharacter attacker,
        GameLocationCharacter me,
        RuleDefinitions.RollOutcome outcome,
        CharacterActionParams actionParams,
        RulesetAttackMode mode,
        ActionModifier modifier)
        {
            if(outcome is RollOutcome.Failure or RollOutcome.CriticalFailure)
            {
                yield break;
            }

            if(me.RulesetCharacter.RemainingKiPoints == 0)
            {
                yield break;
            }
           
            if(me.RulesetCharacter.HasConditionOfType("ConditionReactiveHide"))
            {
                RollOutcome savingOutcome;
                int savingdeltaOutcome;

                List<RuleDefinitions.TrendInfo> modifierTrend = attacker.RulesetCharacter.actionModifier.savingThrowModifierTrends;
                List<RuleDefinitions.TrendInfo> advantageTrends = attacker.RulesetCharacter.actionModifier.savingThrowAdvantageTrends;
                var attackerConModifier = AttributeDefinitions.ComputeAbilityScoreModifier(attacker.RulesetCharacter.GetAttribute(AttributeDefinitions.Constitution).CurrentValue);
                var profBonus = AttributeDefinitions.ComputeProficiencyBonus(me.RulesetCharacter.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue);
                var myWisModifier = AttributeDefinitions.ComputeAbilityScoreModifier(me.RulesetCharacter.GetAttribute(AttributeDefinitions.Wisdom).CurrentValue);

                attacker.RulesetCharacter.RollSavingThrow(0, AttributeDefinitions.Constitution, null, modifierTrend, advantageTrends, attackerConModifier, 8 + profBonus + myWisModifier, false, out savingOutcome, out savingdeltaOutcome);

                string damageType;

                TryGetAncestryDamageTypeFromCharacter(me.Guid, AncestryType.BarbarianClaw, out damageType);

                if (damageType == null)
                {
                    yield break;
                }

                if (damageType == DamageTypeAcid)
                {
                    var myLevel = me.RulesetCharacter.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;
                    int damageInt = 0;

                    if (myLevel <= 4)
                    {
                        damageInt = 8;
                    }
                    else if (myLevel <= 10)
                    {
                        damageInt = 12;
                    }
                    else if (myLevel <= 16)
                    {
                        damageInt = 16;
                    }
                    else if (myLevel <= 20)
                    {
                        damageInt = 20;
                    }

                    bool damageTempPoints;

                    if (savingOutcome == RollOutcome.Success)
                    {
                        attacker.RulesetCharacter.SustainDamage(damageInt / 2, DamageTypeAcid, false, me.Guid, null, out damageTempPoints);
                    }
                    else
                    {
                        attacker.RulesetCharacter.SustainDamage(damageInt, DamageTypeAcid, false, me.Guid, null, out damageTempPoints);
                    }

                }
                else if (damageType == DamageTypeLightning)
                {
                    if (savingOutcome == RollOutcome.Success)
                    {
                        yield break;
                    }
                    ApplyReactiveHideDebuff(attacker.RulesetCharacter, ConditionDefinitions.ConditionShocked);
                }
                else if (damageType == DamageTypeFire)
                {
                    if (savingOutcome == RollOutcome.Success)
                    {
                        yield break;
                    }
                    ApplyReactiveHideDebuff(attacker.RulesetCharacter, ConditionDefinitions.ConditionOnFire);
                }
                else if (damageType == DamageTypePoison)
                {
                    if (savingOutcome == RollOutcome.Success)
                    {
                       yield break;
                    }
                    ApplyReactiveHideDebuff(attacker.RulesetCharacter, ConditionDefinitions.ConditionPoisoned);
                }
                else if (damageType == DamageTypeCold)
                {
                    if (savingOutcome == RollOutcome.Success)
                    {
                        yield break;
                    }
                    ApplyReactiveHideDebuff(attacker.RulesetCharacter, ConditionDefinitions.ConditionHindered_By_Frost);
                    }
                
            }
            
        }
    }
}


