using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

/// <summary>
///     Implement on a ConditionDefinition to make it be removed when its source's turn starts.
/// </summary>
public interface IConditionRemovedOnSourceTurnStart
{
}

//TODO: get rid of interface and add this as sub feature to conditions that implemented interface
internal class RemoveConditionOnSourceTurnStart : IConditionRemovedOnSourceTurnStart
{
    private RemoveConditionOnSourceTurnStart()
    {
    }

    public static IConditionRemovedOnSourceTurnStart Mark { get; } = new RemoveConditionOnSourceTurnStart();
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
            var conditionsToRemove = new List<RulesetCondition>();

            conditionsToRemove.AddRange(
                contender.RulesetActor.ConditionsByCategory
                    .SelectMany(x => x.Value)
                    .Where(x => x.SourceGuid == __instance.Guid)
                    .Where(x => x.ConditionDefinition
                        .HasSubFeatureOfType<IConditionRemovedOnSourceTurnStart>()));

            foreach (var conditionToRemove in conditionsToRemove)
            {
                contender.RulesetActor.RemoveCondition(conditionToRemove);
            }
        }
    }
}
