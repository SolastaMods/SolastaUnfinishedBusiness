using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static AttributeDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class RoguishOpportunist : AbstractSubclass
{
    private const string Name = "RoguishOpportunist";
    private const string RefreshSneakAttack = "RefreshSneakAttack";

    public RoguishOpportunist()
    {
        // LEVEL 03

        // Debilitating Strike

        var savingThrowAffinityDebilitatingStrike = FeatureDefinitionSavingThrowAffinityBuilder
            .Create($"SavingThrowAffinity{Name}DebilitatingStrike")
            .SetGuiPresentation($"Condition{Name}Debilitated", Category.Condition)
            .SetModifiers(FeatureDefinitionSavingThrowAffinity.ModifierType.RemoveDice,
                DieType.D4, 1, false,
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
            .SetSpecialDuration(DurationType.Round, 1)
            .SetFeatures(savingThrowAffinityDebilitatingStrike)
            .AddToDB();

        var powerDebilitatingStrike = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DebilitatingStrike")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnSneakAttackHitAuto)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Individuals)
                    .SetSavingThrowData(false, Dexterity, false,
                        EffectDifficultyClassComputation.CustomAbilityModifierAndProficiency)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionDebilitated, ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .Build())
            .AddToDB();

        // Opportunity

        var featureOpportunity = FeatureDefinitionBuilder
            .Create($"Feature{Name}Opportunity")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureOpportunity.SetCustomSubFeatures(
            new ModifyAttackActionModifierOpportunity(featureOpportunity));

        // LEVEL 09

        // Improved Debilitating Strike

        var savingThrowAffinityImprovedDebilitatingStrike = FeatureDefinitionSavingThrowAffinityBuilder
            .Create($"SavingThrowAffinity{Name}ImprovedDebilitatingStrike")
            .SetGuiPresentation($"Condition{Name}Debilitated", Category.Condition)
            .SetModifiers(FeatureDefinitionSavingThrowAffinity.ModifierType.RemoveDice,
                DieType.D6, 1, false,
                Charisma,
                Constitution,
                Dexterity,
                Intelligence,
                Strength,
                Wisdom)
            .AddToDB();

        var conditionImprovedDebilitated = ConditionDefinitionBuilder
            .Create($"Condition{Name}ImprovedDebilitated")
            .SetGuiPresentation($"Condition{Name}Debilitated", Category.Condition, ConditionBaned)
            .SetSpecialDuration(DurationType.Round, 1)
            .SetFeatures(savingThrowAffinityImprovedDebilitatingStrike)
            .AddToDB();

        var powerImprovedDebilitatingStrike = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ImprovedDebilitatingStrike")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnSneakAttackHitAuto)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Individuals)
                    .SetSavingThrowData(false, Dexterity, false,
                        EffectDifficultyClassComputation.CustomAbilityModifierAndProficiency)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionImprovedDebilitated, ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .Build())
            .SetOverriddenPower(powerDebilitatingStrike)
            .AddToDB();

        // Seize the Chance

        var featureSeizeTheChance = FeatureDefinitionBuilder
            .Create($"Feature{Name}SeizeTheChance")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new TryAlterOutcomeFailedSavingThrowSeizeTheChance())
            .AddToDB();

        // LEVEL 13

        // Blindside

        var combatAffinityBlindside = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}Blindside")
            .SetGuiPresentation($"Condition{Name}Blindsided", Category.Condition)
            .SetMyAttackAdvantage(AdvantageType.Disadvantage)
            .AddToDB();

        var conditionBlindside = ConditionDefinitionBuilder
            .Create($"Condition{Name}Blindsided")
            .SetGuiPresentation(Category.Condition, ConditionBaned)
            .SetSpecialDuration(DurationType.Round, 1)
            .SetFeatures(savingThrowAffinityImprovedDebilitatingStrike, combatAffinityBlindside)
            .AddToDB();

        var powerBlindside = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Blindside")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnSneakAttackHitAuto)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Individuals)
                    .SetSavingThrowData(false, Dexterity, false,
                        EffectDifficultyClassComputation.CustomAbilityModifierAndProficiency)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionBlindside, ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .Build())
            .SetOverriddenPower(powerImprovedDebilitatingStrike)
            .AddToDB();

        // LEVEL 17

        // Exposed Weakness

        var combatAffinityOpportunistExposingWeakness = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}ExposedWeakness")
            .SetGuiPresentation($"Condition{Name}Exposed", Category.Condition)
            .SetAttackOnMeAdvantage(AdvantageType.Advantage)
            .AddToDB();

        var conditionExposed = ConditionDefinitionBuilder
            .Create($"Condition{Name}Exposed")
            .SetGuiPresentation(Category.Condition, ConditionBaned)
            .SetSpecialDuration(DurationType.Round, 1)
            .SetFeatures(
                savingThrowAffinityImprovedDebilitatingStrike,
                combatAffinityBlindside,
                combatAffinityOpportunistExposingWeakness)
            .AddToDB();

        var powerExposedWeakness = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ExposedWeakness")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnSneakAttackHitAuto)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Individuals)
                    .SetSavingThrowData(false, Dexterity, false,
                        EffectDifficultyClassComputation.CustomAbilityModifierAndProficiency)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionExposed, ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .Build())
            .SetOverriddenPower(powerBlindside)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("RoguishOpportunist")
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite("RoguishOpportunist", Resources.RoguishOpportunist, 256))
            .AddFeaturesAtLevel(3, featureOpportunity, powerDebilitatingStrike)
            .AddFeaturesAtLevel(9, featureSeizeTheChance, powerImprovedDebilitatingStrike)
            .AddFeaturesAtLevel(13, powerBlindside)
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

    private sealed class ModifyAttackActionModifierOpportunity : IModifyAttackActionModifier
    {
        private readonly FeatureDefinition _featureOpportunistOpportunity;

        public ModifyAttackActionModifierOpportunity(FeatureDefinition featureOpportunistQuickStrike)
        {
            _featureOpportunistOpportunity = featureOpportunistQuickStrike;
        }

        public void OnAttackComputeModifier(
            RulesetCharacter rulesetAttacker,
            RulesetCharacter rulesetDefender,
            BattleDefinitions.AttackProximity attackProximity,
            RulesetAttackMode attackMode,
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

            // refresh sneak attack
            if (attackMode.AttackTags.Contains(RefreshSneakAttack))
            {
                attacker.UsedSpecialFeatures.Remove(
                    AdditionalDamageRogueSneakAttack.Name);
            }

            // grant advantage if first round or attacker is performing an opportunity attack
            if (Gui.Battle.CurrentRound == 1 &&
                attackMode.actionType != ActionDefinitions.ActionType.Reaction)
            {
                return;
            }

            attackModifier.attackAdvantageTrends.Add(
                new TrendInfo(
                    1, FeatureSourceType.CharacterFeature,
                    _featureOpportunistOpportunity.Name, _featureOpportunistOpportunity));
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
            if (!ShouldTrigger(action, defender, helper))
            {
                yield break;
            }

            var attackMode = helper.FindActionAttackMode(ActionDefinitions.Id.AttackMain);

            if (attackMode == null)
            {
                yield break;
            }

            attackMode.AttackTags.Add(RefreshSneakAttack);

            var attackParam = new BattleDefinitions.AttackEvaluationParams();

            if (attackMode.Ranged)
            {
                attackParam.FillForPhysicalRangeAttack(helper, helper.LocationPosition, attackMode,
                    defender, defender.LocationPosition, new ActionModifier());
            }
            else
            {
                attackParam.FillForPhysicalReachAttack(helper, helper.LocationPosition, attackMode,
                    defender, defender.LocationPosition, new ActionModifier());
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
                new ActionModifier());

            actionService.ReactForOpportunityAttack(reactionParams);

            yield return battleManager.WaitForReactions(helper, actionService, count);
        }

        private static bool ShouldTrigger(
            CharacterAction action,
            GameLocationCharacter defender,
            GameLocationCharacter helper)
        {
            return action.RolledSaveThrow
                   && action.SaveOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure
                   && helper.CanReact()
                   && defender.IsOppositeSide(helper.Side);
        }
    }
}
