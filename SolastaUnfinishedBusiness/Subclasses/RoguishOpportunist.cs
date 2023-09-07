using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class RoguishOpportunist : AbstractSubclass
{
    private const string RefreshSneakAttack = "RefreshSneakAttack";

    public RoguishOpportunist()
    {
        var onComputeAttackModifierOpportunistQuickStrike = FeatureDefinitionBuilder
            .Create("OnComputeAttackModifierOpportunistQuickStrike")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        onComputeAttackModifierOpportunistQuickStrike.SetCustomSubFeatures(
            new ModifyAttackActionModifierOpportunistQuickStrike(onComputeAttackModifierOpportunistQuickStrike));

        var savingThrowAffinityConditionOpportunistDebilitated = FeatureDefinitionSavingThrowAffinityBuilder
            .Create("SavingThrowAffinityOpportunistDebilitatingStrike")
            .SetGuiPresentationNoContent(true)
            .SetModifiers(FeatureDefinitionSavingThrowAffinity.ModifierType.RemoveDice,
                DieType.D6, 1, false,
                AttributeDefinitions.Charisma, AttributeDefinitions.Constitution,
                AttributeDefinitions.Dexterity, AttributeDefinitions.Intelligence,
                AttributeDefinitions.Strength, AttributeDefinitions.Wisdom)
            .AddToDB();

        var powerOpportunistDebilitatingStrike = FeatureDefinitionAdditionalDamageBuilder
            .Create(FeatureDefinitionAdditionalDamages.AdditionalDamageRogueSneakAttack,
                "PowerOpportunistDebilitatingStrike")
            .SetGuiPresentation(Category.Feature)
            // use flat bonus to allow it to interact correct with sneak attack
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.FlatBonus)
            .SetConditionOperations(
                new ConditionOperationDescription
                {
                    operation = ConditionOperationDescription.ConditionOperation.Add,
                    hasSavingThrow = true,
                    conditionDefinition = ConditionDefinitionBuilder
                        .Create("ConditionOpportunistDebilitated")
                        .SetGuiPresentation(Category.Condition, ConditionBaned)
                        .SetSilent(Silent.WhenAddedOrRemoved)
                        .SetSpecialDuration(DurationType.Round, 1)
                        .SetFeatures(savingThrowAffinityConditionOpportunistDebilitated)
                        .AddToDB(),
                    saveAffinity = EffectSavingThrowType.Negates
                })
            .SetSavingThrowData(EffectDifficultyClassComputation.CustomAbilityModifierAndProficiency,
                EffectSavingThrowType.Negates, AttributeDefinitions.Constitution, AttributeDefinitions.Dexterity)
            .AddToDB();

        var featureRoguishOpportunistSeizeTheChance = FeatureDefinitionBuilder
            .Create("FeatureRoguishOpportunistSeizeTheChance")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new TryAlterOutcomeFailedSavingThrowSeizeTheChance())
            .AddToDB();

        var combatAffinityOpportunistExposingWeakness = FeatureDefinitionCombatAffinityBuilder
            .Create("CombatAffinityOpportunistExposingWeakness")
            .SetGuiPresentation("PowerOpportunistExposingWeakness", Category.Feature)
            .SetAttackOnMeAdvantage(AdvantageType.Advantage)
            .AddToDB();

        var powerOpportunistExposingWeakness = FeatureDefinitionAdditionalDamageBuilder
            .Create(FeatureDefinitionAdditionalDamages.AdditionalDamageRogueSneakAttack,
                "PowerOpportunistExposingWeakness")
            .SetGuiPresentation(Category.Feature)
            // use flat bonus to allow it to interact correct with sneak attack
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.FlatBonus)
            .SetConditionOperations(
                new ConditionOperationDescription
                {
                    operation = ConditionOperationDescription.ConditionOperation.Add,
                    hasSavingThrow = true,
                    conditionDefinition = ConditionDefinitionBuilder
                        .Create("ConditionOpportunistExposed")
                        .SetGuiPresentation(Category.Condition, ConditionBaned)
                        .SetSilent(Silent.WhenAddedOrRemoved)
                        .SetSpecialDuration(DurationType.Round, 1)
                        .SetFeatures(
                            savingThrowAffinityConditionOpportunistDebilitated,
                            combatAffinityOpportunistExposingWeakness)
                        .AddToDB(),
                    saveAffinity = EffectSavingThrowType.Negates
                })
            .SetSavingThrowData(EffectDifficultyClassComputation.CustomAbilityModifierAndProficiency,
                EffectSavingThrowType.Negates, AttributeDefinitions.Constitution, AttributeDefinitions.Dexterity)
            //.SetCustomSubFeatures(new CustomBehaviorExposingWeakness(powerOpportunistDebilitatingStrike))
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("RoguishOpportunist")
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite("RoguishOpportunist", Resources.RoguishOpportunist, 256))
            .AddFeaturesAtLevel(3, onComputeAttackModifierOpportunistQuickStrike)
            .AddFeaturesAtLevel(9, powerOpportunistDebilitatingStrike)
            .AddFeaturesAtLevel(13, featureRoguishOpportunistSeizeTheChance)
            .AddFeaturesAtLevel(17, powerOpportunistExposingWeakness)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Rogue;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class ModifyAttackActionModifierOpportunistQuickStrike : IModifyAttackActionModifier
    {
        private readonly FeatureDefinition _featureDefinition;

        public ModifyAttackActionModifierOpportunistQuickStrike(FeatureDefinition featureDefinition)
        {
            _featureDefinition = featureDefinition;
        }

        public void OnAttackComputeModifier(
            RulesetCharacter myself,
            RulesetCharacter defender,
            BattleDefinitions.AttackProximity attackProximity,
            RulesetAttackMode attackMode,
            ref ActionModifier attackModifier)
        {
            if (attackProximity != BattleDefinitions.AttackProximity.PhysicalRange &&
                attackProximity != BattleDefinitions.AttackProximity.PhysicalReach)
            {
                return;
            }

            var hero = GameLocationCharacter.GetFromActor(myself);
            var target = GameLocationCharacter.GetFromActor(defender);

            if (attackMode == null || hero == null || target == null)
            {
                return;
            }

            // refresh sneak attack
            if (attackMode.AttackTags.Contains(RefreshSneakAttack))
            {
                hero.UsedSpecialFeatures.Remove(
                    FeatureDefinitionAdditionalDamages.AdditionalDamageRogueSneakAttack.Name);
            }

            // grant advantage if attacker is performing an opportunity attack or has higher initiative.
            if (hero.LastInitiative <= target.LastInitiative &&
                attackMode.actionType != ActionDefinitions.ActionType.Reaction)
            {
                return;
            }

            attackModifier.attackAdvantageTrends.Add(
                new TrendInfo(1, FeatureSourceType.CharacterFeature, _featureDefinition.Name, _featureDefinition));
        }
    }

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
