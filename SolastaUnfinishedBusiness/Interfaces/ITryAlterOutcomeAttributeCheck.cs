using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface ITryAlterOutcomeAttributeCheck
{
    [UsedImplicitly]
    IEnumerator OnTryAlterAttributeCheck(
        GameLocationBattleManager battleManager,
        AbilityCheckData abilityCheckData,
        GameLocationCharacter defender,
        GameLocationCharacter helper,
        ActionModifier abilityCheckModifier);
}

public sealed class AbilityCheckData
{
    public int AbilityCheckRoll { get; set; }
    public RuleDefinitions.RollOutcome AbilityCheckRollOutcome { get; set; }
    public int AbilityCheckSuccessDelta { get; set; }
}

internal static class TryAlterOutcomeAttributeCheck
{
    internal static IEnumerator HandleITryAlterOutcomeAttributeCheck(
        GameLocationCharacter actingCharacter,
        AbilityCheckData abilityCheckData,
        ActionModifier actionModifier)
    {
        yield return HandleBardicRollOnFailure(actingCharacter, abilityCheckData);

        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var battleManager = ServiceRepository.GetService<IGameLocationBattleService>()
            as GameLocationBattleManager;
        var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
        var contenders =
            Gui.Battle?.AllContenders ??
            locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters);

        foreach (var unit in contenders
                     .Where(u => u.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
                     .ToList())
        {
            var hasUnit =
                actionService.PendingReactionRequestGroups.Count > 0 &&
                actionService.PendingReactionRequestGroups.Peek().Requests
                    .Any(x => x.Character == unit);

            if (hasUnit)
            {
                continue;
            }

            foreach (var tryAlterOutcomeAttributeCheck in unit.RulesetCharacter
                         .GetSubFeaturesByType<ITryAlterOutcomeAttributeCheck>())
            {
                yield return tryAlterOutcomeAttributeCheck.OnTryAlterAttributeCheck(
                    battleManager, abilityCheckData, actingCharacter, unit, actionModifier);
            }
        }
    }

    private static IEnumerator HandleBardicRollOnFailure(
        GameLocationCharacter actingCharacter, AbilityCheckData abilityCheckData)
    {
        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var battleManager = ServiceRepository.GetService<IGameLocationBattleService>()
            as GameLocationBattleManager;

        if (abilityCheckData.AbilityCheckRollOutcome != RuleDefinitions.RollOutcome.Failure)
        {
            yield break;
        }

        battleManager!.GetBestParametersForBardicDieRoll(
            actingCharacter,
            out var bestDie,
            out _,
            out var sourceCondition,
            out var forceMaxRoll,
            out var advantage);

        if (bestDie <= RuleDefinitions.DieType.D1 ||
            actingCharacter.RulesetCharacter == null)
        {
            yield break;
        }

        // Is the die enough to overcome the failure?
        if (RuleDefinitions.DiceMaxValue[(int)bestDie] < Mathf.Abs(abilityCheckData.AbilityCheckSuccessDelta))
        {
            yield break;
        }

        var reactionParams =
            new CharacterActionParams(actingCharacter, ActionDefinitions.Id.UseBardicInspiration)
            {
                IntParameter = (int)bestDie,
                IntParameter2 = (int)RuleDefinitions.BardicInspirationUsageType.AbilityCheck
            };
        var previousReactionCount = actionService.PendingReactionRequestGroups.Count;

        actionService.ReactToUseBardicInspiration(reactionParams);

        yield return battleManager.WaitForReactions(actingCharacter, actionService, previousReactionCount);

        if (!reactionParams.ReactionValidated)
        {
            yield break;
        }

        // Now we have a shot at succeeding on the ability check
        var roll = actingCharacter.RulesetCharacter.RollBardicInspirationDie(
            sourceCondition, abilityCheckData.AbilityCheckSuccessDelta, forceMaxRoll, advantage);

        if (roll < Mathf.Abs(abilityCheckData.AbilityCheckSuccessDelta))
        {
            yield break;
        }

        // The roll is now a success!
        abilityCheckData.AbilityCheckSuccessDelta += roll;
        abilityCheckData.AbilityCheckRollOutcome = RuleDefinitions.RollOutcome.Success;
    }
}
