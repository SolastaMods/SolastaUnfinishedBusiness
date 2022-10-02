using System.Linq;
using SolastaUnfinishedBusiness.Api.Extensions;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

/// <summary>
///     Implement on a ConditionDefinition to make it be removed when its source's turn starts.
/// </summary>
public interface IConditionRemovedOnSourceTurnStart
{
}

public static class ConditionRemovedOnSourceTurnStartPatch
{
    public static void RemoveConditionIfNeeded(RulesetActor __instance,
        RuleDefinitions.TurnOccurenceType occurenceType)
    {
        if (occurenceType != RuleDefinitions.TurnOccurenceType.StartOfTurn)
        {
            return;
        }

        var battleService = ServiceRepository.GetService<IGameLocationBattleService>();

        if (battleService?.Battle == null)
        {
            return;
        }

        foreach (var contender in battleService.Battle.AllContenders
                     .Where(x => x is { destroying: false, destroyedBody: false, RulesetActor: { } }))
        {
            var conditionsToRemove = (from keyValuePair in contender.RulesetActor.ConditionsByCategory
                from rulesetCondition in keyValuePair.Value
                where rulesetCondition.SourceGuid == __instance.Guid &&
                      rulesetCondition.ConditionDefinition.HasSubFeatureOfType<IConditionRemovedOnSourceTurnStart>()
                select rulesetCondition).ToList();

            foreach (var conditionToRemove in conditionsToRemove)
            {
                contender.RulesetActor.RemoveCondition(conditionToRemove);
            }
        }
    }
}
