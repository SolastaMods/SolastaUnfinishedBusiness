using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface ITryAlterOutcomeAttack
{
    // using these priorities across the mod
    // -50 = Way of the Silhouette Shadowy Sanctuary
    // -10 = anything that changes attack rolls
    // non-negative priorities will only trigger if attack is success or critical success
    //   0 = Roguish Acrobat Heroic Uncanny Dodge
    //  10 = anything that adds resistance to damage
    //  20 = anything that reduces damage
    //  30 = anything that buff defender, debuff attacker or damages attacker
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
    private static readonly List<(ITryAlterOutcomeAttack, GameLocationCharacter)> Handlers = [];

    private static void CollectHandlers()
    {
        var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
        var contenders =
            (Gui.Battle?.AllContenders ??
             locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters))
            .ToList();

        Handlers.Clear();

        foreach (var unit in contenders
                     .Where(u => u.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
                     .ToList())
        {
            Handlers.AddRange(unit.RulesetCharacter.GetSubFeaturesByType<ITryAlterOutcomeAttack>()
                .Select(handler => (handler, unit)));

            // supports metamagic use cases
            var hero = unit.RulesetCharacter.GetOriginalHero();

            if (hero != null)
            {
                Handlers.AddRange(hero.TrainedMetamagicOptions
                    .SelectMany(metamagic => metamagic.GetAllSubFeaturesOfType<ITryAlterOutcomeAttack>())
                    .Select(handler => (handler, unit)));
            }
        }
    }

    internal static IEnumerable HandlerNegativePriority(
        GameLocationBattleManager battleManager,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        ActionModifier actionModifier,
        RulesetAttackMode attackMode,
        RulesetEffect rulesetEffect)
    {
        CollectHandlers();

        foreach (var (handler, unit) in Handlers
                     .Where(x => x.Item1.HandlerPriority < 0)
                     .OrderBy(x => x.Item1.HandlerPriority))
        {
            yield return handler.OnTryAlterOutcomeAttack(
                battleManager, action, attacker, defender, unit, actionModifier, attackMode, rulesetEffect);
        }
    }

    internal static IEnumerable HandlerNonNegativePriority(
        GameLocationBattleManager battleManager,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        ActionModifier actionModifier,
        RulesetAttackMode attackMode,
        RulesetEffect rulesetEffect)
    {
        foreach (var (handler, unit) in Handlers
                     .Where(x => x.Item1.HandlerPriority >= 0)
                     .OrderBy(x => x.Item1.HandlerPriority))
        {
            yield return handler.OnTryAlterOutcomeAttack(
                battleManager, action, attacker, defender, unit, actionModifier, attackMode, rulesetEffect);
        }
    }
}
