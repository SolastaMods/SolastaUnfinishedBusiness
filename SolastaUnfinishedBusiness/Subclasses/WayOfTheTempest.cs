using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class WayOfTheTempest : AbstractSubclass
{
    private const string Name = "WayOfTheTempest";

    internal WayOfTheTempest()
    {
        // LEVEL 03

        // Tempest's Swiftness

        var movementAffinityTempestSwiftness = FeatureDefinitionMovementAffinityBuilder
            .Create($"MovementAffinity{Name}TempestSwiftness")
            .SetGuiPresentation(Category.Feature)
            .SetBaseSpeedAdditiveModifier(2)
            .SetCustomSubFeatures(new OnAfterActionTempestSwiftness())
            .AddToDB();

        // LEVEL 06

        // Storm Surge

        var conditionStormSurge = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionFrightened, $"Condition{Name}StormSurge")
            .SetSilent(Silent.None)
            .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
            .AddToDB();

        var powerStormSurge = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}StormSurge")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.KiPoints)
            .SetReactionContext(ExtraReactionContext.Custom)
            .AddToDB();

        powerStormSurge.SetCustomSubFeatures(new CustomBehaviorStormSurge(powerStormSurge, conditionStormSurge));

        // LEVEL 11

        // Tempest’s Fury

        var powerTempestFury = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}TempestFury")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("TempestFury", Resources.PowerTempestFury, 256, 128))
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 3)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Round, 1)
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerMonkFlurryOfBlows)
                    .AddEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitions.ConditionMonkFlurryOfBlowsUnarmedStrikeBonus,
                                ConditionForm.ConditionOperation.Add, true)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitions.ConditionDisengaging,
                                ConditionForm.ConditionOperation.Add, true)
                            .Build())
                    .Build())
            .AddToDB();

        powerTempestFury.SetCustomSubFeatures(
            ValidatorsPowerUse.InCombat,
            new PerformAttackAfterMagicEffectUseTempestFury(),
            new ValidatorsPowerUse(ValidatorsCharacter.HasAttacked));

        // LEVEL 17

        // Unfettered Deluge

        var combatAffinityTempestSwiftness = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}UnfetteredDeluge")
            .SetGuiPresentation($"Condition{Name}AppliedUnfetteredDeluge", Category.Condition)
            .SetMyAttackAdvantage(AdvantageType.Disadvantage)
            .AddToDB();

        var conditionAppliedUnfetteredDeluge = ConditionDefinitionBuilder
            .Create($"Condition{Name}AppliedUnfetteredDeluge")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDistracted)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .SetSilent(Silent.WhenAdded)
            .CopyParticleReferences(ConditionDefinitions.ConditionDistracted)
            .SetSpecialDuration(DurationType.Round, 1)
            .SetFeatures(combatAffinityTempestSwiftness)
            .AddToDB();

        var conditionUnfetteredDeluge = ConditionDefinitionBuilder
            .Create($"Condition{Name}UnfetteredDeluge")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBaned)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .SetSilent(Silent.WhenRemoved)
            .CopyParticleReferences(ConditionDefinitions.ConditionBaned)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .SetCancellingConditions(conditionAppliedUnfetteredDeluge)
            .AddToDB();

        var additionalDamageUnfetteredDeluge = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}UnfetteredDeluge")
            .SetGuiPresentationNoContent(true)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Unarmed)
            .SetConditionOperations(new ConditionOperationDescription
            {
                ConditionDefinition = conditionUnfetteredDeluge,
                Operation = ConditionOperationDescription.ConditionOperation.Add
            })
            .AddToDB();

        var additionalDamageAppliedUnfetteredDeluge = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}AppliedUnfetteredDeluge")
            .SetGuiPresentationNoContent(true)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Unarmed)
            .SetTargetCondition(conditionUnfetteredDeluge, AdditionalDamageTriggerCondition.TargetHasCondition)
            .SetConditionOperations(new ConditionOperationDescription
            {
                ConditionDefinition = conditionAppliedUnfetteredDeluge,
                Operation = ConditionOperationDescription.ConditionOperation.Add
            })
            .AddToDB();

        var featureSetUnfetteredDeluge = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}UnfetteredDeluge")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(additionalDamageUnfetteredDeluge, additionalDamageAppliedUnfetteredDeluge)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.WayOfTheTempest, 256))
            .AddFeaturesAtLevel(3, movementAffinityTempestSwiftness)
            .AddFeaturesAtLevel(6, powerStormSurge)
            .AddFeaturesAtLevel(11, powerTempestFury)
            .AddFeaturesAtLevel(17, featureSetUnfetteredDeluge)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceMonkMonasticTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Tempest Swiftness
    //

    private sealed class OnAfterActionTempestSwiftness : IOnAfterActionFeature
    {
        public void OnAfterAction(CharacterAction action)
        {
            if (action is not CharacterActionUsePower characterActionUsePower ||
                characterActionUsePower.activePower.PowerDefinition != FeatureDefinitionPowers.PowerMonkFlurryOfBlows)
            {
                return;
            }

            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;
            var conditionDisengaging = ConditionDefinitions.ConditionDisengaging;
            var rulesetCondition = RulesetCondition.CreateActiveCondition(
                rulesetCharacter.guid,
                conditionDisengaging,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name);

            rulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
        }
    }

    //
    // Storm Surge
    //

    private class CustomBehaviorStormSurge : IReactToAttackOnMeFinished, IAfterAttackEffect
    {
        private const string StormSurge = "StormSurge";

        private static FeatureDefinitionPower _powerStormSurge;
        private static ConditionDefinition _conditionStormSurge;

        public CustomBehaviorStormSurge(FeatureDefinitionPower powerStormSurge, ConditionDefinition conditionStormSurge)
        {
            _powerStormSurge = powerStormSurge;
            _conditionStormSurge = conditionStormSurge;
        }

        public void AfterOnAttackHit(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            if (!attackMode.AttackTags.Contains(StormSurge))
            {
                return;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetAttacker == null || rulesetDefender == null)
            {
                return;
            }

            if (rulesetDefender.IsDeadOrDyingOrUnconscious)
            {
                return;
            }

            var modifierTrend = rulesetDefender.actionModifier.savingThrowModifierTrends;
            var advantageTrends = rulesetDefender.actionModifier.savingThrowAdvantageTrends;
            var attackerWisdomModifier = AttributeDefinitions.ComputeAbilityScoreModifier(
                rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.Wisdom));
            var attackerProficiencyBonus =
                rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
            var defenderWisdomModifier = AttributeDefinitions.ComputeAbilityScoreModifier(
                rulesetDefender.TryGetAttributeValue(AttributeDefinitions.Wisdom));

            rulesetDefender.RollSavingThrow(0, AttributeDefinitions.Constitution, null, modifierTrend,
                advantageTrends, defenderWisdomModifier, 8 + attackerProficiencyBonus + attackerWisdomModifier,
                false,
                out var savingOutcome,
                out _);

            if (savingOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                return;
            }

            var rulesetCondition = RulesetCondition.CreateActiveCondition(
                defender.Guid,
                _conditionStormSurge,
                _conditionStormSurge.DurationType,
                _conditionStormSurge.DurationParameter,
                _conditionStormSurge.TurnOccurence,
                attacker.Guid,
                attacker.RulesetCharacter.CurrentFaction.Name);

            defender.RulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
        }

        public IEnumerator HandleReactToAttackOnMeFinished(
            GameLocationCharacter attacker,
            GameLocationCharacter me,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode mode,
            ActionModifier modifier)
        {
            if (outcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                yield break;
            }

            if (!me.CanReact())
            {
                yield break;
            }

            var rulesetCharacter = me.RulesetCharacter;

            if (!rulesetCharacter.CanUsePower(_powerStormSurge))
            {
                yield break;
            }

            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var battle = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (manager == null || battle == null)
            {
                yield break;
            }

            var (retaliationMode, retaliationModifier) = me.GetFirstMeleeModeThatCanAttack(attacker);

            if (retaliationMode == null)
            {
                (retaliationMode, retaliationModifier) = me.GetFirstRangedModeThatCanAttack(attacker);

                if (retaliationMode == null)
                {
                    yield break;
                }
            }

            // do I need to check this as well?
            if (!battle.IsWithinBattleRange(me, attacker))
            {
                yield break;
            }

            retaliationMode.AddAttackTagAsNeeded(AttacksOfOpportunity.NotAoOTag);
            retaliationMode.AddAttackTagAsNeeded(StormSurge);

            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var reactionParams = new CharacterActionParams(me, ActionDefinitions.Id.AttackOpportunity)
            {
                TargetCharacters = { attacker },
                ActionModifiers = { retaliationModifier },
                AttackMode = retaliationMode,
                StringParameter2 = $"Reaction/&ReactionAttack{StormSurge}Description"
            };

            var reactionRequest = new ReactionRequestReactionAttack(StormSurge, reactionParams)
            {
                Resource = new ReactionResourcePower(_powerStormSurge, Sprites.KiPointResourceIcon)
            };

            manager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(attacker, manager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            // rulesetCharacter.UsePower(UsablePowersProvider.Get(_powerStormSurge, rulesetCharacter));
            rulesetCharacter.ForceKiPointConsumption(_powerStormSurge.CostPerUse);
        }
    }

    //
    // Tempest Fury
    //

    private sealed class PerformAttackAfterMagicEffectUseTempestFury : IPerformAttackAfterMagicEffectUse
    {
        public IPerformAttackAfterMagicEffectUse.CanAttackHandler CanAttack { get; } =
            CanMeleeAttack;

        public IPerformAttackAfterMagicEffectUse.GetAttackAfterUseHandler PerformAttackAfterUse { get; } =
            DefaultAttackHandler;

        public IPerformAttackAfterMagicEffectUse.CanUseHandler CanBeUsedToAttack { get; } =
            DefaultCanUseHandler;

        private static bool CanMeleeAttack([NotNull] GameLocationCharacter caster, GameLocationCharacter target)
        {
            var attackMode = caster.FindActionAttackMode(ActionDefinitions.Id.AttackOff);

            if (attackMode == null)
            {
                return false;
            }

            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (battleService == null)
            {
                return false;
            }

            var attackModifier = new ActionModifier();
            var evalParams = new BattleDefinitions.AttackEvaluationParams();

            evalParams.FillForPhysicalReachAttack(caster, caster.LocationPosition, attackMode, target,
                target.LocationPosition, attackModifier);

            return battleService.CanAttack(evalParams);
        }

        [NotNull]
        private static List<CharacterActionParams> DefaultAttackHandler([CanBeNull] CharacterActionMagicEffect effect)
        {
            var attacks = new List<CharacterActionParams>();
            var actionParams = effect?.ActionParams;

            if (actionParams == null)
            {
                return attacks;
            }

            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (battleService == null)
            {
                return attacks;
            }

            var caster = actionParams.ActingCharacter;
            var targets = battleService.Battle.AllContenders
                .Where(x => x.Side != caster.Side && battleService.IsWithin1Cell(caster, x))
                .ToList();

            if (caster == null || targets.Empty())
            {
                return attacks;
            }

            var attackMode = caster.FindActionAttackMode(ActionDefinitions.Id.AttackOff);

            if (attackMode == null)
            {
                return attacks;
            }

            //get copy to be sure we don't break existing mode
            var rulesetAttackModeCopy = RulesetAttackMode.AttackModesPool.Get();

            rulesetAttackModeCopy.Copy(attackMode);

            attackMode = rulesetAttackModeCopy;

            //set action type to be same as the one used for the magic effect
            attackMode.ActionType = effect.ActionType;

            var attackModifier = new ActionModifier();

            foreach (var target in targets.Where(t => CanMeleeAttack(caster, t)))
            {
                var attackActionParams =
                    new CharacterActionParams(caster, ActionDefinitions.Id.AttackFree) { AttackMode = attackMode };

                attackActionParams.TargetCharacters.Add(target);
                attackActionParams.ActionModifiers.Add(attackModifier);
                attacks.Add(attackActionParams);
            }

            return attacks;
        }

        private static bool DefaultCanUseHandler(
            [NotNull] CursorLocationSelectTarget targeting,
            GameLocationCharacter caster,
            GameLocationCharacter target, [NotNull] out string failure)
        {
            failure = string.Empty;

            return true;
        }
    }
}
