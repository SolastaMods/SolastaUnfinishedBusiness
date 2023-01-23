using System.Collections;
using System.Linq;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
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
    private const string Name = "WayOfTheDragon";

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
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetParticleEffectParameters(
                        PowerPatronHiveReactiveCarapace.EffectDescription.effectParticleParameters)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder
                        .Create()
                        .SetConditionForm(
                            ConditionDefinitionBuilder
                                .Create(ConditionFiendishResilienceFire, $"Condition{Name}ReactiveHide")
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

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, SorcerousDraconicBloodline)
            .AddFeaturesAtLevel(3, BuildDiscipleFeatureSet(), damageAffinityAncestry, powerReactiveHide)
            .AddFeaturesAtLevel(6, BuildDragonFeatureSet())
            .AddFeaturesAtLevel(11, BuildDragonFuryFeatureSet())
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceMonkMonasticTraditions;

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
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.KiPoints, 3)
            .SetGuiPresentation(Category.Feature, PowerDragonbornBreathWeaponBlack)
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
            .Create(PowerDragonbornBreathWeaponBlue, $"Power{Name}ElementalBreathBlue")
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

        var effectGreenElementalBreath = EffectProxyDefinitionBuilder
            .Create(EffectProxyDefinitions.ProxyStinkingCloud, "EffectGreenElementalBreath")
            .AddToDB();

        var powerGreenElementalBreath = FeatureDefinitionPowerBuilder
            .Create(PowerDragonbornBreathWeaponGreen, $"Power{Name}ElementalBreathGreen")
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.KiPoints, 3)
            .SetGuiPresentation(Category.Feature, PowerDragonbornBreathWeaponGreen)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerDragonbornBreathWeaponGreen.EffectDescription)
                    .SetParticleEffectParameters(PowerDragonbornBreathWeaponGreen)
                    .SetTargetingData(Side.All, RangeType.Self, 2, TargetType.Sphere, 2)
                    .SetDurationData(DurationType.Round, 3)
                    .AddEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetSummonEffectProxyForm(effectGreenElementalBreath)
                            .Build(),
                        StinkingCloud.EffectDescription
                            .effectForms.Find(e => e.formType == EffectForm.EffectFormType.Topology))
                    .SetSavingThrowData(false,
                        AttributeDefinitions.Constitution,
                        false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                        AttributeDefinitions.Wisdom,
                        20)
                    .Build())
            .AddToDB();

        var powerGoldElementalBreath = FeatureDefinitionPowerBuilder
            .Create(PowerDragonbornBreathWeaponGold, $"Power{Name}ElementalBreathGold")
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
            .Create(PowerDragonbornBreathWeaponSilver, $"Power{Name}ElementalBreathSilver")
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
            .SetNotificationTag(NOTIFICATION_TAG)
            .SetAdditionalDamageType(AdditionalDamageType.Specific)
            .SetImpactParticleReference(AcidSplash.EffectDescription.EffectParticleParameters.impactParticleReference)
            .SetSpecificDamageType(DamageTypeAcid)
            .SetDamageDice(DieType.D6, 2)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Unarmed)
            .AddToDB();

        var conditionDragonFuryAcid = ConditionDefinitionBuilder
            .Create($"Condition{Name}DragonFuryAcid")
            .SetGuiPresentation(Category.Condition, ConditionPactChainPseudodragon)
            .AddFeatures(additionalDamageAcid)
            .AddToDB();

        var powerDragonFuryAcid = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DragonFuryAcid")
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
            .Create($"AdditionalDamage{Name}DragonFuryLightning")
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
            .Create($"Condition{Name}DragonFuryLightning")
            .SetGuiPresentation(Category.Condition, ConditionPactChainPseudodragon)
            .AddFeatures(additionalDamageLightning)
            .AddToDB();

        var powerDragonFuryLightning = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DragonFuryLightning")
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
            .Create($"AdditionalDamage{Name}DragonFuryPoison").SetGuiPresentation(Category.Feature)
            .SetNotificationTag(NOTIFICATION_TAG)
            .SetAdditionalDamageType(AdditionalDamageType.Specific)
            .SetImpactParticleReference(PoisonSpray.EffectDescription.effectParticleParameters.impactParticleReference)
            .SetSpecificDamageType(DamageTypePoison)
            .SetDamageDice(DieType.D6, 2)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Unarmed)
            .AddToDB();

        var conditionDragonFuryPoison = ConditionDefinitionBuilder
            .Create($"Condition{Name}DragonFuryPoison")
            .SetGuiPresentation(Category.Condition, ConditionPactChainPseudodragon)
            .AddFeatures(additionalDamagePoison)
            .AddToDB();

        var powerDragonFuryPoison = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DragonFuryPoison")
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
            .Create($"AdditionalDamage{Name}DragonFuryFire")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag(NOTIFICATION_TAG)
            .SetAdditionalDamageType(AdditionalDamageType.Specific)
            .SetSpecificDamageType(DamageTypeFire)
            .SetImpactParticleReference(Fireball.EffectDescription.effectParticleParameters.impactParticleReference)
            .SetDamageDice(DieType.D6, 2)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Unarmed)
            .AddToDB();

        var conditionDragonFuryFire = ConditionDefinitionBuilder
            .Create($"Condition{Name}DragonFuryFire")
            .SetGuiPresentation(Category.Condition, ConditionPactChainPseudodragon)
            .AddFeatures(additionalDamageFire)
            .AddToDB();

        var powerDragonFuryFire = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DragonFuryFire")
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
            .Create($"AdditionalDamage{Name}DragonFuryCold")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag(NOTIFICATION_TAG)
            .SetAdditionalDamageType(AdditionalDamageType.Specific)
            .SetImpactParticleReference(ConeOfCold.EffectDescription.effectParticleParameters.impactParticleReference)
            .SetSpecificDamageType(DamageTypeCold)
            .SetDamageDice(DieType.D6, 2)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Unarmed)
            .AddToDB();

        var conditionDragonFuryCold = ConditionDefinitionBuilder
            .Create($"Condition{Name}DragonFuryCold")
            .SetGuiPresentation(Category.Condition, ConditionPactChainPseudodragon)
            .AddFeatures(additionalDamageCold)
            .AddToDB();

        var powerDragonFuryCold = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DragonFuryCold")
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
                case DamageTypeAcid when savingOutcome == RollOutcome.Success:
                    yield break;
                case DamageTypeAcid when attacker.RulesetCharacter.HasConditionOfType(ConditionAcidArrowed.Name):
                    attacker.RulesetCharacter.SustainDamage(damageInt, DamageTypeAcid, false, me.Guid, null,
                        out _);
                    yield break;
                case DamageTypeAcid:
                    ApplyReactiveHideDebuff(attacker.RulesetCharacter, ConditionAcidArrowed);
                    yield break;
                case DamageTypeLightning when savingOutcome == RollOutcome.Success:
                    yield break;
                case DamageTypeLightning when attacker.RulesetCharacter.HasConditionOfType(ConditionShocked.Name):
                    attacker.RulesetCharacter.SustainDamage(damageInt, DamageTypeLightning, false, me.Guid, null,
                        out _);
                    yield break;
                case DamageTypeLightning:
                    ApplyReactiveHideDebuff(attacker.RulesetCharacter, ConditionShocked);
                    yield break;
                case DamageTypeFire when savingOutcome == RollOutcome.Success:
                    yield break;
                case DamageTypeFire when attacker.RulesetCharacter.HasConditionOfType(ConditionOnFire.Name):
                    attacker.RulesetCharacter.SustainDamage(damageInt, DamageTypeFire, false, me.Guid, null,
                        out _);
                    yield break;
                case DamageTypeFire:
                    ApplyReactiveHideDebuff(attacker.RulesetCharacter, ConditionOnFire);
                    yield break;
                case DamageTypePoison when savingOutcome == RollOutcome.Success:
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
                case DamageTypeCold when savingOutcome == RollOutcome.Success:
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
