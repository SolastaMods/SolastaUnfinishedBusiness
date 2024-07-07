using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;

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
    internal static IEnumerable Handler(
        GameLocationBattleManager battleManager,
        CharacterAction action,
        GameLocationCharacter defender,
        ActionModifier abilityCheckModifier)
    {
        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
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

            foreach (var feature in unit.RulesetCharacter
                         .GetSubFeaturesByType<ITryAlterOutcomeAttributeCheck>())
            {
                var abilityCheckData = new AbilityCheckData
                {
                    AbilityCheckRoll = action.AbilityCheckRoll,
                    AbilityCheckRollOutcome = action.AbilityCheckRollOutcome,
                    AbilityCheckSuccessDelta = action.AbilityCheckSuccessDelta
                };

                yield return feature
                    .OnTryAlterAttributeCheck(battleManager, abilityCheckData, defender, unit, abilityCheckModifier);

                action.AbilityCheckRoll = abilityCheckData.AbilityCheckRoll;
                action.AbilityCheckRollOutcome = abilityCheckData.AbilityCheckRollOutcome;
                action.AbilityCheckSuccessDelta = abilityCheckData.AbilityCheckSuccessDelta;
            }
        }
    }
}
