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
        var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
        var contenders =
            (Gui.Battle?.AllContenders ??
             locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters))
            .ToList();

        foreach (var unit in contenders
                     .Where(u => u.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
                     .ToList())
        {
            foreach (var feature in unit.RulesetCharacter
                         .GetSubFeaturesByType<ITryAlterOutcomeAttack>())
            {
                yield return feature.OnTryAlterOutcomeAttack(
                    battleManager, action, attacker, defender, unit, actionModifier);
            }

            // supports metamagic use cases
            var hero = unit.RulesetCharacter.GetOriginalHero();

            if (hero == null)
            {
                continue;
            }

            foreach (var feature in hero.TrainedMetamagicOptions
                         .SelectMany(metamagic => metamagic.GetAllSubFeaturesOfType<ITryAlterOutcomeAttack>()))
            {
                yield return feature.OnTryAlterOutcomeAttack(
                    battleManager, action, attacker, defender, unit, actionModifier);
            }
        }
    }
}
