using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class RoguishDuelist : AbstractSubclass
{
    internal const string Name = "RoguishDuelist";
    internal const string ConditionReflexiveParry = $"Condition{Name}ReflexiveParry";
    private const string SureFooted = "SureFooted";
    private const string MasterDuelist = "MasterDuelist";

    public RoguishDuelist()
    {
        var additionalDamageDaringDuel = FeatureDefinitionAdditionalDamageBuilder
            .Create(AdditionalDamageRogueSneakAttack, $"AdditionalDamage{Name}DaringDuel")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag(TagsDefinitions.AdditionalDamageSneakAttackTag)
            .SetDamageDice(DieType.D6, 1)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 2)
            .SetTriggerCondition(ExtraAdditionalDamageTriggerCondition.TargetIsDuelingWithYou)
            .SetRequiredProperty(RestrictedContextRequiredProperty.FinesseOrRangeWeapon)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn) // yes Once Per Turn off sneak attack pattern
            .AddCustomSubFeatures(new RogueHolder())
            .AddToDB();

        var attributeModifierSureFooted = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}{SureFooted}")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 2)
            .SetSituationalContext(ExtraSituationalContext.WearingNoArmorOrLightArmorWithoutShield)
            .AddToDB();

        var featureSetSureFooted = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}{SureFooted}")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                FeatureDefinitionCombatAffinitys.CombatAffinityEagerForBattle,
                attributeModifierSureFooted)
            .AddToDB();

        var actionAffinitySwirlingDance = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Name}SwirlingDance")
            .SetGuiPresentation(Category.Feature)
            .SetAuthorizedActions(ActionDefinitions.Id.SwirlingDance)
            .AddToDB();

        var conditionReflexiveParry = ConditionDefinitionBuilder
            .Create(ConditionReflexiveParry)
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var actionAffinityReflexiveParry = FeatureDefinitionBuilder
            .Create($"Feature{Name}ReflexiveParry")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        actionAffinityReflexiveParry.AddCustomSubFeatures(
            new AttackBeforeHitConfirmedOnMeReflexiveParty(actionAffinityReflexiveParry,
                conditionReflexiveParry));

        var powerMasterDuelist = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{MasterDuelist}")
            .SetGuiPresentation($"FeatureSet{Name}MasterDuelist", Category.Feature, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest, 2)
            .AddToDB();

        var featureMasterDuelist = FeatureDefinitionBuilder
            .Create($"Feature{Name}{MasterDuelist}")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(new TryAlterOutcomePhysicalAttackMasterDuelist(powerMasterDuelist))
            .AddToDB();

        var featureSetMasterDuelist = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}{MasterDuelist}")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(featureMasterDuelist, powerMasterDuelist)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite("RoguishDuelist", Resources.RoguishDuelist, 256))
            .AddFeaturesAtLevel(3, additionalDamageDaringDuel, featureSetSureFooted)
            .AddFeaturesAtLevel(9, actionAffinitySwirlingDance)
            .AddFeaturesAtLevel(13, actionAffinityReflexiveParry)
            .AddFeaturesAtLevel(17, featureSetMasterDuelist)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Rogue;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class RogueHolder : IModifyAdditionalDamageClassLevel
    {
        public CharacterClassDefinition Class => CharacterClassDefinitions.Rogue;
    }

    //
    // Reflexive Party
    //

    private sealed class AttackBeforeHitConfirmedOnMeReflexiveParty(
        FeatureDefinition featureDefinition,
        ConditionDefinition conditionDefinition)
        :
            IAttackBeforeHitConfirmedOnMe, IPhysicalAttackFinishedOnMe
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
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false } ||
                rulesetDefender.HasAnyConditionOfTypeOrSubType(
                    conditionDefinition.Name,
                    ConditionDefinitions.ConditionIncapacitated.Name,
                    ConditionDefinitions.ConditionShocked.Name,
                    ConditionDefinitions.ConditionSlowed.Name))
            {
                yield break;
            }

            attackModifier.DefenderDamageMultiplier *= 0.5f;
            rulesetDefender.DamageHalved(rulesetDefender, featureDefinition);
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
            if (attackRollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                yield break;
            }

            if (!defender.CanAct())
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            rulesetDefender.InflictCondition(
                conditionDefinition.Name,
                conditionDefinition.DurationType,
                conditionDefinition.DurationParameter,
                conditionDefinition.TurnOccurence,
                AttributeDefinitions.TagEffect,
                rulesetDefender.Guid,
                rulesetDefender.CurrentFaction.Name,
                1,
                conditionDefinition.Name,
                0,
                0,
                0);
        }
    }

    //
    // Master Duelist
    //

    private class TryAlterOutcomePhysicalAttackMasterDuelist(FeatureDefinitionPower power)
        : ITryAlterOutcomePhysicalAttack
    {
        public IEnumerator OnAttackTryAlterOutcome(
            GameLocationBattleManager battle,
            CharacterAction action,
            GameLocationCharacter me,
            GameLocationCharacter target,
            ActionModifier attackModifier)
        {
            if (action.AttackRollOutcome is not (RollOutcome.Failure or RollOutcome.CriticalFailure))
            {
                yield break;
            }

            var attackMode = action.actionParams.attackMode;
            var rulesetAttacker = me.RulesetCharacter;

            if (rulesetAttacker.GetRemainingPowerCharges(power) <= 0 || !me.CanPerceiveTarget(target))
            {
                yield break;
            }

            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (manager == null)
            {
                yield break;
            }

            var reactionParams = new CharacterActionParams(me, (ActionDefinitions.Id)ExtraActionId.DoNothingFree)
            {
                StringParameter = $"Reaction/&CustomReaction{Name}{MasterDuelist}ReactDescription"
            };
            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom($"{Name}{MasterDuelist}", reactionParams);

            manager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(me, manager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            rulesetAttacker.UpdateUsageForPower(power, power.CostPerUse);

            var totalRoll = (action.AttackRoll + attackMode.ToHitBonus).ToString();
            var rollCaption = action.AttackRoll == 1
                ? "Feedback/&RollCheckCriticalFailureTitle"
                : "Feedback/&CriticalAttackFailureOutcome";

            rulesetAttacker.LogCharacterUsedPower(power,
                "Feedback/&TriggerRerollLine",
                false,
                (ConsoleStyleDuplet.ParameterType.Base, $"{action.AttackRoll}+{attackMode.ToHitBonus}"),
                (ConsoleStyleDuplet.ParameterType.FailedRoll, Gui.Format(rollCaption, totalRoll)));

            var roll = rulesetAttacker.RollAttack(
                attackMode.toHitBonus,
                target.RulesetCharacter,
                attackMode.sourceDefinition,
                attackModifier.attackToHitTrends,
                false,
                [new TrendInfo(1, FeatureSourceType.CharacterFeature, power.Name, power)],
                attackMode.ranged,
                false,
                attackModifier.attackRollModifier,
                out var outcome,
                out var successDelta,
                -1,
                // testMode true avoids the roll to display on combat log as the original one will get there with altered results
                true);

            attackModifier.ignoreAdvantage = false;
            attackModifier.attackAdvantageTrends =
                [new TrendInfo(1, FeatureSourceType.CharacterFeature, power.Name, power)];
            action.AttackRollOutcome = outcome;
            action.AttackSuccessDelta = successDelta;
            action.AttackRoll = roll;
        }
    }
}
