using System.Collections;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface ITryAlterOutcomeAttributeCheck
{
    IEnumerator OnTryAlterAttributeCheck(
        GameLocationBattleManager battleManager,
        CharacterAction action,
        GameLocationCharacter defender,
        GameLocationCharacter helper,
        ActionModifier abilityCheckModifier);
}

internal static class TryAlterOutcomeAttributeCheck
{
    internal static IEnumerable Handler(
        GameLocationBattleManager battleManager,
        CharacterAction action,
        GameLocationCharacter defender,
        ActionModifier abilityCheckModifier)
    {
        var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
        var contenders =
            (Gui.Battle?.AllContenders ??
             locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters))
            .ToList();

        foreach (var unit in contenders
                     .Where(u => u.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false }))
        {
            foreach (var feature in unit.RulesetCharacter
                         .GetSubFeaturesByType<ITryAlterOutcomeAttributeCheck>())
            {
                yield return feature
                    .OnTryAlterAttributeCheck(battleManager, action, defender, unit, abilityCheckModifier);
            }
        }
    }
}
