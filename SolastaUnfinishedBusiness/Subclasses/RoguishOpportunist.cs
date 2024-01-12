using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
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
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
            .SetFeatures(savingThrowAffinityDebilitatingStrike)
            .CopyParticleReferences(ConditionSlowed)
            .AddToDB();

        var powerDebilitatingStrike = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DebilitatingStrike")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnSneakAttackHitAuto)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
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

        // Opportunity

        var featureOpportunity = FeatureDefinitionBuilder
            .Create($"Feature{Name}Opportunity")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureOpportunity.AddCustomSubFeatures(
            new ModifyAttackActionModifierOpportunity(featureOpportunity));

        // LEVEL 09

        // Seize the Chance

        var featureSeizeTheChance = FeatureDefinitionBuilder
            .Create($"Feature{Name}SeizeTheChance")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new TryAlterOutcomeFailedSavingThrowSeizeTheChance())
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
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
            .AddFeatures(savingThrowAffinityImprovedDebilitatingStrike)
            .CopyParticleReferences(ConditionSlowed)
            .AddToDB();

        var powerImprovedDebilitatingStrike = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ImprovedDebilitatingStrike")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnSneakAttackHitAuto)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
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
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
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

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("RoguishOpportunist")
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite("RoguishOpportunist", Resources.RoguishOpportunist, 256))
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
    // Opportunity
    //

    private sealed class ModifyAttackActionModifierOpportunity(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition featureOpportunistQuickStrike)
        : IModifyAttackActionModifier
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

    private sealed class TryAlterOutcomeFailedSavingThrowSeizeTheChance : ITryAlterOutcomeFailedSavingThrow
    {
        public IEnumerator OnFailedSavingTryAlterOutcome(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier saveModifier,
            bool hasHitVisual,
            bool hasBorrowedLuck)
        {
            //do not trigger on my own turn, so won't retaliate on AoO
            if (Gui.Battle?.ActiveContenderIgnoringLegendary == helper)
            {
                yield break;
            }

            if (!ShouldTrigger(defender, helper))
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

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var count = actionService.PendingReactionRequestGroups.Count;
            var reactionParams = new CharacterActionParams(
                helper,
                ActionDefinitions.Id.AttackOpportunity,
                helper.RulesetCharacter.AttackModes[0],
                defender,
                actionModifier) { stringParameter2 = "SeizeTheChance" };

            actionService.ReactForOpportunityAttack(reactionParams);

            yield return battleManager.WaitForReactions(helper, actionService, count);
        }

        private static bool ShouldTrigger(
            GameLocationCharacter defender,
            GameLocationCharacter helper)
        {
            return helper.CanReact() && helper.PerceivedFoes.Contains(defender);
        }
    }
}
