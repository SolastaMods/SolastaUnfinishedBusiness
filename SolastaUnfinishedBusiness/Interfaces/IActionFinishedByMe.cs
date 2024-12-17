using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IActionFinishedByMe
{
    [UsedImplicitly]
    public IEnumerator OnActionFinishedByMe(CharacterAction action);
}

public sealed class ActionFinishedByMeCheckBonusOrMainOrMove : IActionFinishedByMe
{
    private static readonly ConditionDefinition ConditionNoBonus = ConditionDefinitionBuilder
        .Create("ConditionNoBonus")
        .SetGuiPresentationNoContent(true)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .SetFeatures(
            FeatureDefinitionActionAffinityBuilder
                .Create("ActionAffinityNoBonus")
                .SetGuiPresentationNoContent(true)
                .SetAllowedActionTypes(bonus: false)
                .AddToDB())
        .AddToDB();

    private static readonly ConditionDefinition ConditionNoMove = ConditionDefinitionBuilder
        .Create("ConditionNoMove")
        .SetGuiPresentationNoContent(true)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .SetFeatures(
            FeatureDefinitionActionAffinityBuilder
                .Create("ActionAffinityNoMove")
                .SetGuiPresentationNoContent(true)
                .SetAllowedActionTypes(move: false)
                .AddToDB())
        .AddToDB();

    private static readonly ConditionDefinition ConditionNoMain = ConditionDefinitionBuilder
        .Create("ConditionNoMain")
        .SetGuiPresentationNoContent(true)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .SetFeatures(
            FeatureDefinitionActionAffinityBuilder
                .Create("ActionAffinityNoMain")
                .SetGuiPresentationNoContent(true)
                .SetAllowedActionTypes(false)
                .AddToDB())
        .AddToDB();

    public IEnumerator OnActionFinishedByMe(CharacterAction action)
    {
        var actionType = action.ActionType;
        var actingCharacter = action.ActingCharacter;

        if (actionType == ActionDefinitions.ActionType.Move &&
            (action is not CharacterActionMoveStepBase || actingCharacter.MovingToDestination))
        {
            yield break;
        }

        var rulesetCharacter = actingCharacter.RulesetCharacter;
        var conditions = actionType switch
        {
            ActionDefinitions.ActionType.Main => [ConditionNoMove, ConditionNoBonus],
            ActionDefinitions.ActionType.Bonus => [ConditionNoMain, ConditionNoMove],
            ActionDefinitions.ActionType.Move => [ConditionNoBonus, ConditionNoMain],
            _ => new List<ConditionDefinition>()
        };

        foreach (var condition in conditions)
        {
            rulesetCharacter.InflictCondition(
                condition.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                condition.Name,
                0,
                0,
                0);
        }
    }
}
