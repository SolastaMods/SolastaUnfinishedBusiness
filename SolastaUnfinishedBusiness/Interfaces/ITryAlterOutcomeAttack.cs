using System.Collections;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface ITryAlterOutcomeAttack
{
    IEnumerator OnTryAlterOutcomeAttack(
        GameLocationBattleManager instance,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationCharacter helper,
        ActionModifier actionModifier);
}

internal static class TryAlterOutcomeAttack
{
    internal static IEnumerable Handler(
        GameLocationBattleManager battleManager,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        ActionModifier actionModifier)
    {
        if (Gui.Battle == null)
        {
            yield break;
        }

        foreach (var unit in Gui.Battle.AllContenders
                     .Where(u => u.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false }))
        {
            foreach (var feature in unit.RulesetCharacter
                         .GetSubFeaturesByType<ITryAlterOutcomeAttack>())
            {
                yield return feature.OnTryAlterOutcomeAttack(
                    battleManager, action, attacker, defender, unit, actionModifier);
            }
        }
    }
}
