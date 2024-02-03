using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface ITryAlterOutcomeSavingThrowFromAllyOrEnemy
{
    IEnumerator OnSavingThrowTryAlterOutcomeFromAllyOrEnemy(
        GameLocationBattleManager battleManager,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationCharacter featureOwner,
        ActionModifier saveModifier,
        bool hasHitVisual,
        [UsedImplicitly] bool hasBorrowedLuck);
}

internal static class TryAlterOutcomeSavingThrowFromAllyOrEnemy
{
    internal static IEnumerable Handler(
        GameLocationBattleManager battleManager,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        ActionModifier actionModifier,
        bool hasBorrowedLuck)
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
                         .GetSubFeaturesByType<ITryAlterOutcomeSavingThrowFromAllyOrEnemy>())
            {
                yield return feature.OnSavingThrowTryAlterOutcomeFromAllyOrEnemy(
                    battleManager, action, attacker, defender, unit, actionModifier, false, hasBorrowedLuck);
            }
        }
    }
}
