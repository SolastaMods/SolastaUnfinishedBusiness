using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface ITryAlterOutcomeAttack
{
    public int HandlerPriority { get; }

    public IEnumerator OnTryAlterOutcomeAttack(
        GameLocationBattleManager instance,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationCharacter helper,
        ActionModifier actionModifier,
        RulesetAttackMode attackMode,
        RulesetEffect rulesetEffect);
}

internal static class TryAlterOutcomeAttack
{
    internal static IEnumerable Handler(
        GameLocationBattleManager battleManager,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        ActionModifier actionModifier,
        RulesetAttackMode attackMode,
        RulesetEffect rulesetEffect)
    {
        var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
        var contenders =
            (Gui.Battle?.AllContenders ??
             locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters))
            .ToList();

        var handlers = new List<(ITryAlterOutcomeAttack, GameLocationCharacter)>();

        foreach (var unit in contenders
                     .Where(u => u.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
                     .ToList())
        {
            handlers.AddRange(unit.RulesetCharacter.GetSubFeaturesByType<ITryAlterOutcomeAttack>()
                .Select(handler => (handler, unit)));

            // supports metamagic use cases
            var hero = unit.RulesetCharacter.GetOriginalHero();

            if (hero != null)
            {
                handlers.AddRange(hero.TrainedMetamagicOptions
                    .SelectMany(metamagic => metamagic.GetAllSubFeaturesOfType<ITryAlterOutcomeAttack>())
                    .Select(handler => (handler, unit)));
            }
        }

        foreach (var (handler, unit) in handlers.OrderBy(x => x.Item1.HandlerPriority))
        {
            yield return handler.OnTryAlterOutcomeAttack(
                battleManager, action, attacker, defender, unit, actionModifier, attackMode, rulesetEffect);
        }
    }
}
