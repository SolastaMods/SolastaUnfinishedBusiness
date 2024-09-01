using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface ITryAlterOutcomeSavingThrow
{
    IEnumerator OnTryAlterOutcomeSavingThrow(
        GameLocationBattleManager battleManager,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationCharacter helper,
        ActionModifier actionModifier,
        bool hasHitVisual,
        [UsedImplicitly] bool hasBorrowedLuck);
}

internal static class TryAlterOutcomeSavingThrow
{
    internal static IEnumerator Handler(
        GameLocationBattleManager battleManager,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        ActionModifier actionModifier,
        bool hasBorrowedLuck,
        EffectDescription effectDescription)
    {
        // Legendary Resistance or Indomitable?
        if (action.SaveOutcome == RuleDefinitions.RollOutcome.Failure)
        {
            yield return battleManager.HandleFailedSavingThrow(
                action, attacker, defender, actionModifier, false, hasBorrowedLuck);
        }

        //PATCH: support for `ITryAlterOutcomeSavingThrow`
        foreach (var tryAlterOutcomeSavingThrow in TryAlterOutcomeSavingThrowHandler(
                     battleManager, action, attacker, defender, actionModifier, false, hasBorrowedLuck))
        {
            yield return tryAlterOutcomeSavingThrow;
        }

        defender.RulesetActor.GrantConditionOnSavingThrowOutcome(effectDescription, action.SaveOutcome, true);
    }

    private static IEnumerable TryAlterOutcomeSavingThrowHandler(
        GameLocationBattleManager battleManager,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        ActionModifier actionModifier,
        bool hasHitVisual,
        bool hasBorrowedLuck)
    {
        var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
        var contenders =
            Gui.Battle?.AllContenders ??
            locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters);

        foreach (var unit in contenders
                     .Where(u => u.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
                     .ToList())
        {
            foreach (var feature in unit.RulesetCharacter
                         .GetSubFeaturesByType<ITryAlterOutcomeSavingThrow>())
            {
                yield return feature.OnTryAlterOutcomeSavingThrow(
                    battleManager, action, attacker, defender, unit, actionModifier, hasHitVisual, hasBorrowedLuck);
            }
        }
    }
}
