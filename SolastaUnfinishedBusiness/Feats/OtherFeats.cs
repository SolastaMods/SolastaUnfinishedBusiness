using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Feats;

internal static class OtherFeats
{
    internal const string SentinelFeat = "FeatSentinel";
    internal const string WarcasterFeat = "FeatWarCaster";
    internal const string MagicAffinityWarcaster = "MagicAffinityFeatWarCaster";

    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var featHealer = BuildHealerMedKit();
        var featPickPocket = BuildPickPocket();
        var featTough = BuildTough();
        var featWarCaster = BuildWarcaster();

        feats.Add(BuildInspiringLeader());
        feats.Add(BuildPolearmExpert());
        feats.Add(BuildSentinel());
        feats.Add(BuildTorchbearer());

        feats.Add(featHealer);
        feats.Add(featPickPocket);
        feats.Add(featTough);
        feats.Add(featWarCaster);

        GroupFeats.MakeGroup("FeatGroupBodyResilience", null,
            FeatDefinitions.BadlandsMarauder,
            FeatDefinitions.BlessingOfTheElements,
            FeatDefinitions.Enduring_Body,
            FeatDefinitions.FocusedSleeper,
            FeatDefinitions.HardToKill,
            FeatDefinitions.Hauler,
            FeatDefinitions.Robust,
            featTough);

        GroupFeats.MakeGroup("FeatGroupSkills", null,
            FeatDefinitions.Manipulator,
            featHealer,
            featPickPocket);

        GroupFeats.MakeGroup("FeatGroupSpellCombat", null,
            FeatDefinitions.FlawlessConcentration,
            FeatDefinitions.PowerfulCantrip,
            featWarCaster);
    }

    private static FeatDefinition BuildHealerMedKit()
    {
        var powerFeatHealerMedKit = FeatureDefinitionPowerBuilder
            .Create("PowerFeatHealerMedKit")
            .SetGuiPresentation(Category.Feature, PowerFunctionGoodberryHealingOther)
            .SetUsesAbilityBonus(ActivationTime.Action, RechargeRate.ShortRest, AttributeDefinitions.Wisdom)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Touch, 1, TargetType.Individuals)
                .SetCreatedByCharacter()
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetHealingForm(
                            HealingComputation.Dice,
                            4,
                            DieType.D6,
                            1,
                            false,
                            HealingCap.MaximumHitPoints)
                        .SetLevelAdvancement(EffectForm.LevelApplianceType.AddBonus, LevelSourceType.CharacterLevel)
                        .CreatedByCharacter()
                        .Build())
                .SetEffectAdvancement(EffectIncrementMethod.None)
                .SetParticleEffectParameters(SpellDefinitions.MagicWeapon)
                .Build())
            .AddToDB();

        var powerFeatHealerResuscitate = FeatureDefinitionPowerBuilder
            .Create("PowerFeatHealerResuscitate")
            .SetGuiPresentation(Category.Feature, PowerDomainLifePreserveLife)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Touch, 1, TargetType.Individuals)
                .SetTargetFiltering(
                    TargetFilteringMethod.CharacterOnly,
                    TargetFilteringTag.No,
                    5,
                    DieType.D8)
                .SetCreatedByCharacter()
                .SetDurationData(DurationType.Permanent)
                .SetRequiredCondition(ConditionDefinitions.ConditionDead)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetReviveForm(12, ReviveHitPoints.One)
                        .CreatedByCharacter()
                        .Build())
                .SetEffectAdvancement(EffectIncrementMethod.None)
                .SetParticleEffectParameters(SpellDefinitions.MagicWeapon)
                .Build())
            .AddToDB();

        var powerFeatHealerStabilize = FeatureDefinitionPowerBuilder
            .Create("PowerFeatHealerStabilize")
            .SetGuiPresentation(Category.Feature, PowerDomainLifePreserveLife)
            .SetUsesAbilityBonus(ActivationTime.Action, RechargeRate.ShortRest, AttributeDefinitions.Wisdom)
            .SetEffectDescription(SpellDefinitions.SpareTheDying.EffectDescription)
            .AddToDB();

        var proficiencyFeatHealerMedicine = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyFeatHealerMedicine")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.SkillOrExpertise, SkillDefinitions.Medecine)
            .AddToDB();

        return FeatDefinitionBuilder
            .Create("FeatHealer")
            .SetGuiPresentation(Category.Feat, PowerFunctionGoodberryHealingOther)
            .SetFeatures(
                powerFeatHealerMedKit,
                powerFeatHealerResuscitate,
                powerFeatHealerStabilize,
                proficiencyFeatHealerMedicine)
            .AddToDB();
    }

    private static FeatDefinition BuildInspiringLeader()
    {
        var powerFeatInspiringLeader = FeatureDefinitionPowerBuilder
            .Create("PowerFeatInspiringLeader")
            .SetGuiPresentation("FeatInspiringLeader", Category.Feat, PowerOathOfTirmarGoldenSpeech)
            .SetUsesFixed(ActivationTime.Minute10, RechargeRate.ShortRest)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.Individuals, 6)
                .SetCreatedByCharacter()
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetTempHpForm()
                        .SetLevelAdvancement(EffectForm.LevelApplianceType.AddBonus, LevelSourceType.CharacterLevel)
                        .CreatedByCharacter()
                        .SetBonusMode(AddBonusMode.AbilityBonus)
                        .Build())
                .SetEffectAdvancement(EffectIncrementMethod.None)
                .SetParticleEffectParameters(SpellDefinitions.MagicWeapon)
                .Build())
            .AddToDB();

        return FeatDefinitionBuilder
            .Create("FeatInspiringLeader")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(powerFeatInspiringLeader)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
            .AddToDB();
    }

    private static FeatDefinition BuildPickPocket()
    {
        var abilityCheckAffinityFeatPickPocket = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create(FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityFeatLockbreaker,
                "AbilityCheckAffinityFeatPickPocket")
            .SetGuiPresentation("FeatPickPocket", Category.Feat)
            .AddToDB();

        var pickpocketAbilityCheckAffinityGroup = new FeatureDefinitionAbilityCheckAffinity.AbilityCheckAffinityGroup
        {
            abilityScoreName = AttributeDefinitions.Dexterity,
            proficiencyName = SkillDefinitions.SleightOfHand,
            affinity = CharacterAbilityCheckAffinity.Advantage
        };

        abilityCheckAffinityFeatPickPocket.AffinityGroups.SetRange(pickpocketAbilityCheckAffinityGroup);

        var proficiencyFeatPickPocket = FeatureDefinitionProficiencyBuilder
            .Create(FeatureDefinitionProficiencys.ProficiencyFeatLockbreaker,
                "ProficiencyFeatPickPocket")
            .SetGuiPresentation("FeatPickPocket", Category.Feat)
            .AddToDB();

        proficiencyFeatPickPocket.proficiencyType = ProficiencyType.SkillOrExpertise;
        proficiencyFeatPickPocket.Proficiencies.Clear();
        proficiencyFeatPickPocket.Proficiencies.Add(SkillDefinitions.SleightOfHand);

        return FeatDefinitionBuilder
            .Create(FeatDefinitions.Lockbreaker, "FeatPickPocket")
            .SetFeatures(abilityCheckAffinityFeatPickPocket, proficiencyFeatPickPocket)
            .SetGuiPresentation(Category.Feat)
            .AddToDB();
    }

    private static FeatDefinition BuildPolearmExpert()
    {
        return FeatDefinitionBuilder
            .Create("FeatPolearmExpert")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(FeatureDefinitionBuilder
                .Create("FeatureFeatPolearm")
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(
                    new CanMakeAoOOnReachEntered(ValidatorsCharacter.HasPolearm),
                    new AddPolearmFollowupAttack())
                .AddToDB())
            .AddToDB();
    }

    private static FeatDefinition BuildSentinel()
    {
        return FeatDefinitionBuilder
            .Create(SentinelFeat)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(FeatureDefinitionBuilder
                .Create("OnAttackHitEffectFeatSentinel")
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(
                    AttacksOfOpportunity.CanIgnoreDisengage,
                    AttacksOfOpportunity.SentinelFeatMarker,
                    new OnAttackHitEffectFeatSentinel(CustomConditionsContext.StopMovement))
                .AddToDB())
            .AddToDB();
    }

    private static FeatDefinition BuildTorchbearer()
    {
        return FeatDefinitionBuilder
            .Create("FeatTorchbearer")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(FeatureDefinitionPowerBuilder
                .Create("PowerTorchbearer")
                .SetGuiPresentation(Category.Feature)
                .SetUsesFixed(ActivationTime.BonusAction)
                .SetEffectDescription(EffectDescriptionBuilder
                    .Create(SpellDefinitions.Fireball.EffectDescription)
                    .SetCanBePlacedOnCharacter(false)
                    .SetCreatedByCharacter()
                    .SetDurationData(DurationType.Round, 3)
                    .SetSpeed(SpeedType.Instant, 11f)
                    .SetTargetingData(Side.Enemy, RangeType.Touch, 30, TargetType.Individuals)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitions.ConditionOnFire1D4,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Dexterity,
                        false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                        AttributeDefinitions.Dexterity,
                        15)
                    .Build())
                .SetShowCasting(false)
                .SetCustomSubFeatures(new ValidatorsPowerUse(ValidatorsCharacter.OffHandHasLightSource))
                .AddToDB())
            .AddToDB();
    }

    private static FeatDefinition BuildTough()
    {
        return FeatDefinitionBuilder
            .Create("FeatTough")
            .SetFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create("AttributeModifierFeatTough")
                    .SetGuiPresentationNoContent(true)
                    .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                        AttributeDefinitions.HitPointBonusPerLevel, 2)
                    .AddToDB())
            .SetGuiPresentation(Category.Feat)
            .AddToDB();
    }

    private static FeatDefinition BuildWarcaster()
    {
        var warcaster = FeatDefinitionBuilder
            .Create(WarcasterFeat)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionMagicAffinityBuilder
                    .Create(MagicAffinityWarcaster)
                    .SetGuiPresentation(WarcasterFeat, Category.Feat)
                    .SetCastingModifiers(0, SpellParamsModifierType.FlatValue, 0,
                        SpellParamsModifierType.None)
                    .SetConcentrationModifiers(ConcentrationAffinity.Advantage, 0)
                    .SetHandsFullCastingModifiers(true, true, true)
                    .AddToDB())
            .SetMustCastSpellsPrerequisite()
            .AddToDB();

        return warcaster;
    }

    //
    // HELPERS
    //

    private sealed class OnAttackHitEffectFeatSentinel : IAfterAttackEffect
    {
        private readonly ConditionDefinition _conditionSentinelStopMovement;

        internal OnAttackHitEffectFeatSentinel(ConditionDefinition conditionSentinelStopMovement)
        {
            _conditionSentinelStopMovement = conditionSentinelStopMovement;
        }

        public void AfterOnAttackHit(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            if (outcome != RollOutcome.Success && outcome != RollOutcome.CriticalSuccess)
            {
                return;
            }

            if (attackMode is not { ActionType: ActionDefinitions.ActionType.Reaction })
            {
                return;
            }

            if (attackMode.AttackTags.Contains(AttacksOfOpportunity.NotAoOTag))
            {
                return;
            }

            var character = defender.RulesetCharacter;

            character.AddConditionOfCategory(AttributeDefinitions.TagCombat,
                RulesetCondition.CreateActiveCondition(character.Guid,
                    _conditionSentinelStopMovement,
                    DurationType.Round,
                    1,
                    TurnOccurenceType.StartOfTurn,
                    attacker.Guid,
                    string.Empty
                ));
        }
    }
}
