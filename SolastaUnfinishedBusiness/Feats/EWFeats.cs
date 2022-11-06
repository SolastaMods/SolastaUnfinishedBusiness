using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Feats;

internal static class EwFeats
{
    internal const string MagicAffinityWarcaster = "MagicAffinityFeatWarCaster";
    internal const string SentinelFeat = "FeatSentinel";
    private const string PolearmExpertFeat = "FeatPolearmExpert";
    private const string RangedExpertFeat = "FeatRangedExpert";
    private const string RecklessAttackFeat = "FeatRecklessAttack";
    internal const string WarcasterFeat = "FeatWarCaster";

    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        feats.Add(BuildSentinel());
        feats.Add(BuildPolearmExpert());
        feats.Add(BuildRangedExpert());
        feats.Add(BuildRecklessAttack());
        feats.Add(BuildPowerAttack());
        feats.Add(BuildWarcaster());
    }

    private static FeatDefinition BuildSentinel()
    {
        var conditionRestrained = ConditionDefinitions.ConditionRestrained;

        var conditionSentinelStopMovement = ConditionDefinitionBuilder
            .Create("ConditionSentinelStopMovement")
            .SetGuiPresentation(Category.Condition, Gui.NoLocalization,
                conditionRestrained.GuiPresentation.SpriteReference)
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(
                FeatureDefinitionMovementAffinitys.MovementAffinityConditionRestrained,
                FeatureDefinitionActionAffinitys.ActionAffinityConditionRestrained)
            .AddToDB();

        return FeatDefinitionBuilder
            .Create(SentinelFeat)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(FeatureDefinitionBuilder
                .Create("OnAttackHitEffectFeatSentinel")
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(
                    AttacksOfOpportunity.CanIgnoreDisengage,
                    AttacksOfOpportunity.SentinelFeatMarker,
                    new OnAttackHitEffectFeatSentinel(conditionSentinelStopMovement))
                .AddToDB())
            .AddToDB();
    }

    private static FeatDefinition BuildPolearmExpert()
    {
        return FeatDefinitionBuilder
            .Create(PolearmExpertFeat)
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

    private static FeatDefinition BuildRangedExpert()
    {
        return FeatDefinitionBuilder
            .Create(RangedExpertFeat)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(FeatureDefinitionBuilder
                .Create("FeatureFeatRangedExpert")
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(
                    new RangedAttackInMeleeDisadvantageRemover(),
                    new AddExtraRangedAttack(IsOneHandedRanged, ActionDefinitions.ActionType.Bonus,
                        ValidatorsCharacter.HasAttacked))
                .AddToDB())
            .AddToDB();
    }

    private static FeatDefinition BuildRecklessAttack()
    {
        return FeatDefinitionWithPrerequisitesBuilder
            .Create(RecklessAttackFeat)
            .SetGuiPresentation("RecklessAttack", Category.Action)
            .SetFeatures(FeatureDefinitionActionAffinitys.ActionAffinityBarbarianRecklessAttack)
            .SetValidators(ValidatorsFeat.ValidateNotClass(CharacterClassDefinitions.Barbarian))
            .AddToDB();
    }

    private static bool IsOneHandedRanged(RulesetAttackMode mode, RulesetItem weapon, RulesetCharacter character)
    {
        return ValidatorsWeapon.IsRanged(weapon) && ValidatorsWeapon.IsOneHanded(weapon);
    }

    private static FeatDefinition BuildPowerAttack()
    {
        var concentrationProvider = new StopPowerConcentrationProvider("PowerAttack",
            "Tooltip/&PowerAttackConcentration", Sprites.GetSprite("PowerAttackConcentrationIcon",
                Resources.PowerAttackConcentrationIcon, 64, 64));

        var conditionPowerAttackTrigger = ConditionDefinitionBuilder
            .Create("ConditionPowerAttackTrigger")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetDuration(DurationType.Permanent)
            .SetFeatures(FeatureDefinitionBuilder
                .Create("TriggerFeaturePowerAttack")
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(concentrationProvider)
                .AddToDB())
            .AddToDB();

        var conditionPowerAttack = ConditionDefinitionBuilder
            .Create("ConditionPowerAttack")
            .SetGuiPresentation("PowerAttack", Category.Feature, ConditionDefinitions.ConditionHeraldOfBattle)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetAllowMultipleInstances(false)
            .SetFeatures(
                FeatureDefinitionBuilder
                    .Create("ModifyAttackModeForWeaponFeatPowerAttack")
                    .SetGuiPresentation("PowerAttack", Category.Feature)
                    .SetCustomSubFeatures(new ModifyPowerAttackPower())
                    .AddToDB())
            .SetDuration(DurationType.Round, 1)
            .AddToDB();

        var powerAttack = FeatureDefinitionPowerBuilder
            .Create("PowerAttack")
            .SetGuiPresentation("FeatPowerAttack", Category.Feat,
                Sprites.GetSprite("PowerAttackIcon", Resources.PowerAttackIcon, 128, 64))
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(conditionPowerAttackTrigger, ConditionForm.ConditionOperation.Add)
                        .Build(),
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(conditionPowerAttack, ConditionForm.ConditionOperation.Add)
                        .Build())
                .Build())
            .AddToDB();

        Global.PowersThatIgnoreInterruptions.Add(powerAttack);

        var powerTurnOffPowerAttack = FeatureDefinitionPowerBuilder
            .Create("PowerTurnOffPowerAttack")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
                .SetDurationData(DurationType.Round, 1)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(conditionPowerAttackTrigger, ConditionForm.ConditionOperation.Remove)
                        .Build(),
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(conditionPowerAttack, ConditionForm.ConditionOperation.Remove)
                        .Build())
                .Build())
            .AddToDB();

        Global.PowersThatIgnoreInterruptions.Add(powerTurnOffPowerAttack);
        concentrationProvider.StopPower = powerTurnOffPowerAttack;

        return FeatDefinitionBuilder
            .Create("FeatPowerAttack")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                powerAttack,
                powerTurnOffPowerAttack
            )
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

    private sealed class OnAttackHitEffectFeatSentinel : IOnAttackHitEffect
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

    private sealed class ModifyPowerAttackPower : IModifyAttackModeForWeapon
    {
        public void ModifyAttackMode(RulesetCharacter character, [CanBeNull] RulesetAttackMode attackMode)
        {
            var damage = attackMode?.EffectDescription?.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            if (!ValidatorsWeapon.IsMelee(attackMode))
            {
                return;
            }

            var proficiency = character.GetAttribute(AttributeDefinitions.ProficiencyBonus).CurrentValue;
            const int TO_HIT = -3;
            var toDamage = 3 + proficiency;

            attackMode.ToHitBonus += TO_HIT;
            attackMode.ToHitBonusTrends.Add(new TrendInfo(TO_HIT,
                FeatureSourceType.Power, "PowerAttack", null));

            damage.BonusDamage += toDamage;
            damage.DamageBonusTrends.Add(new TrendInfo(toDamage,
                FeatureSourceType.Power, "PowerAttack", null));
        }
    }
}
