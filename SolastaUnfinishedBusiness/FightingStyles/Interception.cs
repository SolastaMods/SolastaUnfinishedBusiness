using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class Interception : AbstractFightingStyle
{
    private const string InterceptionName = "Interception";

    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create(InterceptionName)
        .SetGuiPresentation(Category.FightingStyle, DatabaseHelper.FightingStyleDefinitions.Defense)
        .SetFeatures(
            FeatureDefinitionPowerBuilder
                .Create("PowerInterception")
                .SetGuiPresentation(InterceptionName, Category.FightingStyle)
                .SetUsesFixed(ActivationTime.Reaction)
                .SetReactionContext(ExtraReactionContext.Custom)
                .AddCustomSubFeatures(new AttackBeforeHitPossibleOnMeOrAllyInterception(
                    ConditionDefinitionBuilder
                        .Create("ConditionInterception")
                        .SetGuiPresentationNoContent(true)
                        .SetSilent(Silent.WhenAddedOrRemoved)
                        .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                        .SetSpecialInterruptions(ExtraConditionInterruption.AfterWasAttacked)
                        .SetAmountOrigin(ConditionDefinition.OriginOfAmount.Fixed)
                        .AddFeatures(
                            FeatureDefinitionReduceDamageBuilder
                                .Create("ReduceDamageInterception")
                                .SetGuiPresentation(InterceptionName, Category.FightingStyle)
                                .SetAlwaysActiveReducedDamage(
                                    (_, defender) => defender.RulesetCharacter.AllConditions.FirstOrDefault(
                                        x => x.ConditionDefinition.Name == "ConditionInterception")!.Amount)
                                .AddToDB())
                        .AddToDB()))
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => new()
    {
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin, FightingStyleRanger
    };

    private sealed class AttackBeforeHitPossibleOnMeOrAllyInterception : IAttackBeforeHitPossibleOnMeOrAlly
    {
        private readonly ConditionDefinition _conditionDefinition;

        public AttackBeforeHitPossibleOnMeOrAllyInterception(ConditionDefinition conditionDefinition)
        {
            _conditionDefinition = conditionDefinition;
        }

        public IEnumerator OnAttackBeforeHitPossibleOnMeOrAlly(
            GameLocationBattleManager battleManager,
            GameLocationCharacter featureOwner,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect,
            ActionModifier attackModifier,
            int attackRoll)
        {
            if (featureOwner == defender)
            {
                yield break;
            }

            if (!battleManager.IsWithin1Cell(featureOwner, defender))
            {
                yield break;
            }

            if (!featureOwner.CanReact())
            {
                yield break;
            }

            var unitCharacter = featureOwner.RulesetCharacter;

            if (ValidatorsWeapon.IsUnarmed(unitCharacter.GetMainWeapon()?.ItemDefinition, null)
                && ValidatorsWeapon.IsUnarmed(unitCharacter.GetOffhandWeapon()?.ItemDefinition, null))
            {
                yield break;
            }

            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (manager == null)
            {
                yield break;
            }

            var reactionParams =
                new CharacterActionParams(featureOwner, (ActionDefinitions.Id)ExtraActionId.DoNothingReaction)
                {
                    StringParameter = "CustomReactionInterceptionDescription"
                        .Formatted(Category.Reaction, defender.Name, attacker.Name)
                };
            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom($"{InterceptionName}", reactionParams);

            manager.AddInterruptRequest(reactionRequest);

            yield return battleManager.WaitForReactions(featureOwner, manager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            var roll = unitCharacter.RollDie(DieType.D10, RollContext.None, true, AdvantageType.None, out _, out _);
            var reducedDamage = roll + unitCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);

            defender.RulesetCharacter.InflictCondition(
                _conditionDefinition.Name,
                _conditionDefinition.DurationType,
                _conditionDefinition.DurationParameter,
                _conditionDefinition.TurnOccurence,
                AttributeDefinitions.TagCombat,
                unitCharacter.guid,
                unitCharacter.CurrentFaction.Name,
                1,
                null,
                reducedDamage,
                0,
                0);
        }
    }
}
