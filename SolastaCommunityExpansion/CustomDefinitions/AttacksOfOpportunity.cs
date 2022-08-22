using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomUI;
using SolastaCommunityExpansion.Feats;
using SolastaCommunityExpansion.Models;
using TA;

namespace SolastaCommunityExpansion.CustomDefinitions;

public interface ICanIgnoreAoOImmunity
{
    bool CanIgnoreAoOImmunity(RulesetCharacter character, RulesetCharacter attacker);
}

public static class AttacksOfOpportunity
{
    public const string NotAoOTag = "NotAoO"; //Used to distinguish reaction attacks from AoO
    public static readonly ICanIgnoreAoOImmunity CanIgnoreDisengage = new CanIgnoreDisengage();
    public static readonly object SentinelFeatMarker = new SentinelFeatMarker();
    public static readonly Dictionary<ulong, (int3, int3)> MovingCharactersCache = new();

    public static IEnumerator ProcessOnCharacterAttackFinished(
        GameLocationBattleManager battleManager,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RulesetAttackMode attackerAttackMode)
    {
        if (battleManager == null)
        {
            yield break;
        }

        var battle = battleManager.Battle;
        if (battle == null)
        {
            yield break;
        }

        //Process features on attacker or defender

        var units = battle.AllContenders
            .Where(u => !u.RulesetCharacter.IsDeadOrDyingOrUnconscious)
            .ToArray();

        //Process other participants of the battle
        foreach (var unit in units)
        {
            if (attacker != unit && defender != unit)
            {
                yield return ProcessSentinel(unit, attacker, defender, battleManager);
            }
        }
    }

    private static IEnumerator ProcessSentinel([NotNull] GameLocationCharacter unit,
        [NotNull] GameLocationCharacter attacker,
        GameLocationCharacter defender, GameLocationBattleManager battleManager)
    {
        if (!attacker.IsOppositeSide(unit.Side) || defender.Side != unit.Side ||
            !(unit.RulesetCharacter?.HasSubFeatureOfType<SentinelFeatMarker>() ?? false) ||
            (defender.RulesetCharacter?.HasSubFeatureOfType<SentinelFeatMarker>() ?? false) || !CanMakeAoO(unit,
                attacker, out var opportunityAttackMode, out var actionModifier,
                battleManager))
        {
            yield break;
        }

        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var count = actionService.PendingReactionRequestGroups.Count;

        RequestReactionAttack(EwFeats.SentinelFeat, new CharacterActionParams(
            unit,
            ActionDefinitions.Id.AttackOpportunity,
            opportunityAttackMode,
            attacker,
            actionModifier)
        );

        yield return battleManager.WaitForReactions(unit, actionService, count);
    }

    public static void ProcessOnCharacterMoveStart([NotNull] GameLocationCharacter mover, int3 destination)
    {
        MovingCharactersCache.AddOrReplace(mover.Guid, (mover.locationPosition, destination));
    }

    public static IEnumerator ProcessOnCharacterMoveEnd(GameLocationBattleManager battleManager,
        GameLocationCharacter mover)
    {
        if (battleManager == null)
        {
            yield break;
        }

        var battle = battleManager.Battle;
        if (battle == null)
        {
            yield break;
        }

        var units = battle.AllContenders
            .Where(u => !u.RulesetCharacter.IsDeadOrDyingOrUnconscious)
            .ToArray();

        //Process other participants of the battle
        foreach (var unit in units)
        {
            if (mover != unit)
            {
                yield return ProcessPolearmExpert(unit, mover, battleManager);
            }
        }
    }

    public static void CleanMovingCache()
    {
        MovingCharactersCache.Clear();
    }

    private static IEnumerator ProcessPolearmExpert([NotNull] GameLocationCharacter attacker,
        [NotNull] GameLocationCharacter mover,
        GameLocationBattleManager battleManager)
    {
        if (!attacker.IsOppositeSide(mover.Side) || !CanMakeAoOOnEnemyEnterReach(attacker.RulesetCharacter) ||
            !MovingCharactersCache.TryGetValue(mover.Guid, out var movement) ||
            !battleManager.CanPerformOpportunityAttackOnCharacter(attacker, mover, movement.Item2, movement.Item1,
                out var attackMode))
        {
            yield break;
        }

        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var count = actionService.PendingReactionRequestGroups.Count;

        actionService.ReactForOpportunityAttack(new CharacterActionParams(
            attacker,
            ActionDefinitions.Id.AttackOpportunity,
            attackMode,
            mover,
            new ActionModifier()));

        yield return battleManager.WaitForReactions(attacker, actionService, count);
    }

    private static bool CanMakeAoOOnEnemyEnterReach([CanBeNull] RulesetCharacter character)
    {
        return character != null &&
               character.GetSubFeaturesByType<CanMakeAoOOnReachEntered>()
                   .Any(f => f.IsValid(character));
    }

    private static void RequestReactionAttack(string type, CharacterActionParams actionParams)
    {
        var actionManager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
        if (actionManager == null)
        {
            return;
        }

        actionParams.AttackMode?.AddAttackTagAsNeeded(NotAoOTag);
        actionManager.AddInterruptRequest(new ReactionRequestReactionAttack(type, actionParams));
    }

    private static bool CanMakeAoO(GameLocationCharacter attacker, GameLocationCharacter defender,
        [CanBeNull] out RulesetAttackMode attackMode, [NotNull] out ActionModifier actionModifier,
        IGameLocationBattleService battleManager = null)
    {
        battleManager ??= ServiceRepository.GetService<IGameLocationBattleService>();

        actionModifier = new ActionModifier();
        attackMode = null;
        if (!battleManager.IsValidAttackerForOpportunityAttackOnCharacter(attacker, defender))
        {
            return false;
        }

        attackMode = attacker.FindActionAttackMode(ActionDefinitions.Id.AttackOpportunity);
        if (attackMode == null)
        {
            return false;
        }

        var evaluationParams = new BattleDefinitions.AttackEvaluationParams();
        evaluationParams.FillForPhysicalReachAttack(attacker, attacker.LocationPosition, attackMode, defender,
            defender.LocationPosition, actionModifier);
        return battleManager.CanAttack(evaluationParams);
    }

    public static bool IsSubjectToAttackOfOpportunity(RulesetCharacter character, RulesetCharacter attacker,
        bool def)
    {
        return def || attacker.GetSubFeaturesByType<ICanIgnoreAoOImmunity>()
            .Any(f => f.CanIgnoreAoOImmunity(character, attacker));
    }
}

internal sealed class CanIgnoreDisengage : ICanIgnoreAoOImmunity
{
    public bool CanIgnoreAoOImmunity([NotNull] RulesetCharacter character, RulesetCharacter attacker)
    {
        var featuresToBrowse = new List<FeatureDefinition>();
        character.EnumerateFeaturesToBrowse<ICombatAffinityProvider>(featuresToBrowse);
        var service = ServiceRepository.GetService<IRulesetImplementationService>();
        var disengaging = DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityDisengaging;
        // foreach (var feature in featuresToBrowse)
        // {
        //     if (feature != disengaging
        //         && feature is ICombatAffinityProvider affinityProvider
        //         && service.IsSituationalContextValid(
        //             new RulesetImplementationDefinitions.SituationalContextParams(
        //                 affinityProvider.SituationalContext, attacker,
        //                 character, service.FindSourceIdOfFeature(character, feature),
        //                 affinityProvider.RequiredCondition, null))
        //         && affinityProvider.IsImmuneToOpportunityAttack(character, attacker))
        //     {
        //         return false;
        //     }
        // }

        return true;
    }
}

internal sealed class SentinelFeatMarker
{
}

internal sealed class CanMakeAoOOnReachEntered
{
    private readonly CharacterValidator[] validators;

    public CanMakeAoOOnReachEntered(params CharacterValidator[] validators)
    {
        this.validators = validators;
    }

    public bool IsValid([CanBeNull] RulesetCharacter character)
    {
        return character != null && character.IsValid(validators);
    }
}

public interface IReactToAttackFinished
{
    IEnumerator HandleReactToAttackFinished(GameLocationCharacter character, GameLocationCharacter defender,
        RuleDefinitions.RollOutcome outcome, CharacterActionParams actionParams, RulesetAttackMode mode,
        ActionModifier modifier);
}

public delegate IEnumerator ReactToAttackFinishedHandler(GameLocationCharacter character,
    GameLocationCharacter defender, RuleDefinitions.RollOutcome outcome,
    CharacterActionParams actionParams, RulesetAttackMode mode, ActionModifier modifier);

public sealed class ReactToAttackFinished : IReactToAttackFinished
{
    private readonly ReactToAttackFinishedHandler handler;

    public ReactToAttackFinished(ReactToAttackFinishedHandler handler)
    {
        this.handler = handler;
    }

    public IEnumerator HandleReactToAttackFinished(GameLocationCharacter character, GameLocationCharacter defender,
        RuleDefinitions.RollOutcome outcome,
        CharacterActionParams actionParams, RulesetAttackMode mode, ActionModifier modifier)
    {
        yield return handler(character, defender, outcome, actionParams, mode, modifier);
    }
}
