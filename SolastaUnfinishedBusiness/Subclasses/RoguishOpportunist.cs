using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static AttributeDefinitions;
using static FeatureDefinitionSavingThrowAffinity;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class RoguishOpportunist : AbstractSubclass
{
    private const string Name = "RoguishOpportunist";

    public RoguishOpportunist()
    {
        // LEVEL 03

        // Debilitating Strike

        var savingThrowAffinityDebilitatingStrike = FeatureDefinitionSavingThrowAffinityBuilder
            .Create($"SavingThrowAffinity{Name}DebilitatingStrike")
            .SetGuiPresentation($"Condition{Name}Debilitated", Category.Condition, Gui.NoLocalization)
            .SetModifiers(ModifierType.RemoveDice, DieType.D4, 1, false,
                Charisma,
                Constitution,
                Dexterity,
                Intelligence,
                Strength,
                Wisdom)
            .AddToDB();

        var conditionDebilitated = ConditionDefinitionBuilder
            .Create($"Condition{Name}Debilitated")
            .SetGuiPresentation(Category.Condition, ConditionBaned)
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(savingThrowAffinityDebilitatingStrike)
            .CopyParticleReferences(ConditionSlowed)
            .AddToDB();

        var powerDebilitatingStrike = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DebilitatingStrike")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Individuals)
                    .SetSavingThrowData(false, Constitution, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, Dexterity, 8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionDebilitated, ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetParticleEffectParameters(InflictWounds)
                    .Build())
            .AddToDB();

        powerDebilitatingStrike.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new PhysicalAttackBeforeHitConfirmedOnEnemyDebilitatingStrike(powerDebilitatingStrike));

        // Opportunity

        var featureOpportunity = FeatureDefinitionBuilder
            .Create($"Feature{Name}Opportunity")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureOpportunity.AddCustomSubFeatures(new ModifyAttackActionModifierOpportunity(featureOpportunity));

        // LEVEL 09

        // Seize the Chance

        var featureSeizeTheChance = FeatureDefinitionBuilder
            .Create($"Feature{Name}SeizeTheChance")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new TryAlterOutcomeSavingThrowSeizeTheChance())
            .AddToDB();

        // LEVEL 13

        // Improved Debilitating Strike

        var savingThrowAffinityImprovedDebilitatingStrike = FeatureDefinitionSavingThrowAffinityBuilder
            .Create($"SavingThrowAffinity{Name}ImprovedDebilitatingStrike")
            .SetGuiPresentation($"Condition{Name}ImprovedDebilitated", Category.Condition, Gui.NoLocalization)
            .SetModifiers(ModifierType.RemoveDice, DieType.D6, 1, false,
                Charisma,
                Constitution,
                Dexterity,
                Intelligence,
                Strength,
                Wisdom)
            .AddToDB();

        var conditionImprovedDebilitated = ConditionDefinitionBuilder
            .Create(ConditionHindered, $"Condition{Name}ImprovedDebilitated")
            .SetGuiPresentation(Category.Condition, ConditionBaned)
            .SetConditionType(ConditionType.Detrimental)
            .AddFeatures(savingThrowAffinityImprovedDebilitatingStrike)
            .CopyParticleReferences(ConditionSlowed)
            .AddToDB();

        var powerImprovedDebilitatingStrike = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ImprovedDebilitatingStrike")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Individuals)
                    .SetSavingThrowData(false, Constitution, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, Dexterity, 8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionImprovedDebilitated, ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetParticleEffectParameters(InflictWounds)
                    .Build())
            .SetOverriddenPower(powerDebilitatingStrike)
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        // LEVEL 17

        // Exposed Weakness

        var combatAffinityOpportunistExposingWeakness = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}ExposedWeakness")
            .SetGuiPresentation($"Condition{Name}Exposed", Category.Condition)
            .SetAttackOnMeAdvantage(AdvantageType.Advantage)
            .AddToDB();

        var conditionExposed = ConditionDefinitionBuilder
            .Create(ConditionHindered, $"Condition{Name}Exposed")
            .SetGuiPresentation(Category.Condition, ConditionBaned)
            .SetConditionType(ConditionType.Detrimental)
            .AddFeatures(savingThrowAffinityImprovedDebilitatingStrike, combatAffinityOpportunistExposingWeakness)
            .CopyParticleReferences(ConditionSlowed)
            .AddToDB();

        var powerExposedWeakness = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ExposedWeakness")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnSneakAttackHitAuto)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Individuals)
                    .SetSavingThrowData(false, Constitution, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, Dexterity, 8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionExposed, ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetParticleEffectParameters(InflictWounds)
                    .Build())
            .SetOverriddenPower(powerImprovedDebilitatingStrike)
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.RoguishOpportunist, 256))
            .AddFeaturesAtLevel(3, featureOpportunity, powerDebilitatingStrike)
            .AddFeaturesAtLevel(9, featureSeizeTheChance)
            .AddFeaturesAtLevel(13, powerImprovedDebilitatingStrike)
            .AddFeaturesAtLevel(17, powerExposedWeakness)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Rogue;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Debilitating Strike
    //

    private sealed class PhysicalAttackBeforeHitConfirmedOnEnemyDebilitatingStrike(
        FeatureDefinitionPower powerDebilitatingStrike) : IPhysicalAttackBeforeHitConfirmedOnEnemy
    {
        public IEnumerator OnPhysicalAttackBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            if (!CharacterContext.IsSneakAttackValid(actionModifier, attacker, defender))
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            var implementationManagerService =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerDebilitatingStrike, rulesetAttacker);
            // must be spend power otherwise it'll trigger after cunning strike
            var actionParams = new CharacterActionParams(defender, ActionDefinitions.Id.SpendPower)
            {
                RulesetEffect = implementationManagerService
                    .MyInstantiateEffectPower(rulesetAttacker, usablePower, false),
                UsablePower = usablePower
            };

            ServiceRepository.GetService<IGameLocationActionService>()?
                .ExecuteAction(actionParams, null, false);
        }
    }

    //
    // Opportunity
    //

    private sealed class ModifyAttackActionModifierOpportunity(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition featureOpportunistQuickStrike) : IModifyAttackActionModifier
    {
        public void OnAttackComputeModifier(
            RulesetCharacter rulesetAttacker,
            RulesetCharacter rulesetDefender,
            BattleDefinitions.AttackProximity attackProximity,
            RulesetAttackMode attackMode,
            string effectName,
            ref ActionModifier attackModifier)
        {
            if (Gui.Battle == null ||
                attackMode == null ||
                attackProximity is not
                    (BattleDefinitions.AttackProximity.PhysicalRange
                    or BattleDefinitions.AttackProximity.PhysicalReach))
            {
                return;
            }

            var attacker = GameLocationCharacter.GetFromActor(rulesetAttacker);
            var defender = GameLocationCharacter.GetFromActor(rulesetDefender);

            if (attacker == null || defender == null)
            {
                return;
            }

            // grant advantage if first round or attacker is performing an opportunity attack
            if (Gui.Battle.CurrentRound > 1 &&
                attackMode.actionType != ActionDefinitions.ActionType.Reaction)
            {
                return;
            }

            attackModifier.attackAdvantageTrends.Add(new TrendInfo(
                1, FeatureSourceType.CharacterFeature,
                featureOpportunistQuickStrike.Name, featureOpportunistQuickStrike));
        }
    }

    //
    // Seize the Chance
    //

    private sealed class TryAlterOutcomeSavingThrowSeizeTheChance : ITryAlterOutcomeSavingThrow
    {
        public IEnumerator OnTryAlterOutcomeSavingThrow(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier saveModifier,
            bool hasHitVisual,
            bool hasBorrowedLuck)
        {
            var gameLocationActionManager =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (gameLocationActionManager == null ||
                helper.Side == defender.Side ||
                !action.RolledSaveThrow ||
                action.SaveOutcome != RollOutcome.Failure ||
                helper.IsMyTurn() ||
                !helper.CanReact() ||
                !helper.CanPerceiveTarget(defender))
            {
                yield break;
            }

            var attackMode = helper.FindActionAttackMode(ActionDefinitions.Id.AttackMain);

            if (attackMode == null)
            {
                yield break;
            }

            var actionModifier = new ActionModifier();
            var attackParam = new BattleDefinitions.AttackEvaluationParams();

            if (attackMode.Ranged)
            {
                attackParam.FillForPhysicalRangeAttack(helper, helper.LocationPosition, attackMode,
                    defender, defender.LocationPosition, actionModifier);
            }
            else
            {
                attackParam.FillForPhysicalReachAttack(helper, helper.LocationPosition, attackMode,
                    defender, defender.LocationPosition, actionModifier);
            }

            if (!battleManager.CanAttack(attackParam))
            {
                yield break;
            }

            var count = gameLocationActionManager.PendingReactionRequestGroups.Count;
            var reactionParams = new CharacterActionParams(
                helper,
                ActionDefinitions.Id.AttackOpportunity,
                helper.RulesetCharacter.AttackModes[0],
                defender,
                actionModifier) { StringParameter2 = "SeizeTheChance" };

            gameLocationActionManager.ReactForOpportunityAttack(reactionParams);

            yield return battleManager.WaitForReactions(attacker, gameLocationActionManager, count);
        }
    }
}
