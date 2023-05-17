using System.Collections;
using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class RoguishDuelist : AbstractSubclass
{
    internal const string Name = "RoguishDuelist";
    internal const string ConditionReflexiveParry = $"Condition{Name}ReflexiveParry";
    private const string SureFooted = "SureFooted";
    private const string MasterDuelist = "MasterDuelist";

    internal RoguishDuelist()
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
            .SetCustomSubFeatures(new RogueHolder())
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
            .SetAllowedActionTypes()
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

        actionAffinityReflexiveParry.SetCustomSubFeatures(
            new PhysicalAttackBeforeHitConfirmedOnMeReflexiveParty(actionAffinityReflexiveParry,
                conditionReflexiveParry));

        var powerMasterDuelist = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{MasterDuelist}")
            .SetGuiPresentation($"FeatureSet{Name}MasterDuelist", Category.Feature, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest)
            .AddToDB();

        var featureMasterDuelist = FeatureDefinitionBuilder
            .Create($"Feature{Name}{MasterDuelist}")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(new PhysicalAttackTryAlterOutcomeMasterDuelist(powerMasterDuelist))
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

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class RogueHolder : IClassHoldingFeature
    {
        public CharacterClassDefinition Class => CharacterClassDefinitions.Rogue;
    }

    //
    // Reflexive Party
    //

    private sealed class PhysicalAttackBeforeHitConfirmedOnMeReflexiveParty : IPhysicalAttackBeforeHitConfirmedOnMe,
        IReactToAttackOnMeFinished
    {
        private readonly ConditionDefinition _conditionDefinition;
        private readonly FeatureDefinition _featureDefinition;

        public PhysicalAttackBeforeHitConfirmedOnMeReflexiveParty(
            FeatureDefinition featureDefinition,
            ConditionDefinition conditionDefinition)
        {
            _conditionDefinition = conditionDefinition;
            _featureDefinition = featureDefinition;
        }

        public IEnumerator OnAttackBeforeHitConfirmed(
            GameLocationBattleManager battle,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool criticalHit,
            bool firstTarget)
        {
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender == null ||
                rulesetDefender.IsDeadOrDyingOrUnconscious ||
                rulesetDefender.HasAnyConditionOfType(
                    _conditionDefinition.Name,
                    ConditionDefinitions.ConditionIncapacitated.Name,
                    ConditionDefinitions.ConditionShocked.Name,
                    ConditionDefinitions.ConditionSlowed.Name))
            {
                yield break;
            }

            attackModifier.DefenderDamageMultiplier *= 0.5f;
            rulesetDefender.DamageHalved(rulesetDefender, _featureDefinition);
        }

        public IEnumerator HandleReactToAttackOnMeFinished(
            GameLocationCharacter attacker,
            GameLocationCharacter me,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode mode,
            ActionModifier modifier)
        {
            if (outcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                yield break;
            }

            var rulesetDefender = me.RulesetCharacter;

            rulesetDefender?.InflictCondition(
                _conditionDefinition.Name,
                _conditionDefinition.DurationType,
                _conditionDefinition.DurationParameter,
                _conditionDefinition.TurnOccurence,
                AttributeDefinitions.TagCombat,
                rulesetDefender.Guid,
                rulesetDefender.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
        }
    }

    //
    // Master Duelist
    //

    private class PhysicalAttackTryAlterOutcomeMasterDuelist : IPhysicalAttackTryAlterOutcome
    {
        private readonly FeatureDefinitionPower _power;

        public PhysicalAttackTryAlterOutcomeMasterDuelist(FeatureDefinitionPower power)
        {
            _power = power;
        }

        public IEnumerator OnAttackTryAlterOutcome(
            GameLocationBattleManager battle,
            CharacterAction action,
            GameLocationCharacter me,
            GameLocationCharacter target,
            ActionModifier attackModifier)
        {
            var attackMode = action.actionParams.attackMode;
            var rulesetDefender = me.RulesetCharacter;

            if (rulesetDefender == null || rulesetDefender.GetRemainingPowerCharges(_power) <= 0)
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

            rulesetDefender.RollAttack(
                attackMode.toHitBonus,
                target.RulesetCharacter,
                attackMode.sourceDefinition,
                attackModifier.attackToHitTrends,
                attackModifier.ignoreAdvantage,
                new List<TrendInfo> { new(1, FeatureSourceType.CharacterFeature, _power.Name, _power) },
                attackMode.ranged,
                false, // check this
                attackModifier.attackRollModifier,
                out var outcome,
                out _,
                -1,
                false);

            action.AttackRollOutcome = outcome;

            GameConsoleHelper.LogCharacterUsedPower(rulesetDefender, _power);
        }
    }
}
