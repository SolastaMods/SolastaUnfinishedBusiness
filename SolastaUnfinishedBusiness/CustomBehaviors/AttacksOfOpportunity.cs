using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.FightingStyles;
using SolastaUnfinishedBusiness.Subclasses;
using TA;
using static ActionDefinitions;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal static class AttacksOfOpportunity
{
    internal const string NotAoOTag = "NotAoO"; //Used to distinguish reaction attacks from AoO
    internal static readonly ICanIgnoreAoOImmunity CanIgnoreDisengage = new CanIgnoreDisengage();
    internal static readonly object SentinelFeatMarker = new SentinelFeatMarker();
    private static readonly Dictionary<ulong, (int3, int3)> MovingCharactersCache = new();

    internal static IEnumerator ProcessOnCharacterAttackFinished(
        GameLocationBattleManager battleManager,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RulesetAttackMode attackerAttackMode)
    {
        if (battleManager == null)
        {
            yield break;
        }

        // this happens during Aksha fight when she uses second veil a 2nd time
        if (defender == null)
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

    private static IEnumerator ProcessSentinel(
        [NotNull] GameLocationCharacter unit,
        [NotNull] GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationBattleManager battleManager)
    {
        if (!attacker.IsOppositeSide(unit.Side) || defender.Side != unit.Side || unit == defender
            || !(unit.RulesetCharacter?.HasSubFeatureOfType<SentinelFeatMarker>() ?? false)
            || !CanMakeAoO(unit, attacker, out var opportunityAttackMode, out var actionModifier, battleManager))
        {
            yield break;
        }

        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var count = actionService.PendingReactionRequestGroups.Count;

        RequestReactionAttack(Sentinel.SentinelName, new CharacterActionParams(
            unit,
            Id.AttackOpportunity,
            opportunityAttackMode,
            attacker,
            actionModifier)
        );

        yield return battleManager.WaitForReactions(unit, actionService, count);
    }

    internal static void ProcessOnCharacterMoveStart([NotNull] GameLocationCharacter mover, int3 destination)
    {
        MovingCharactersCache.AddOrReplace(mover.Guid, (mover.locationPosition, destination));
    }

    internal static IEnumerator ProcessOnCharacterMoveEnd(GameLocationBattleManager battleManager,
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
            if (mover == unit
                || !mover.IsOppositeSide(unit.Side)
                || !MovingCharactersCache.TryGetValue(mover.Guid, out var movement))
            {
                continue;
            }

            if (unit.GetActionTypeStatus(ActionType.Reaction) == ActionStatus.Available)
            {
                yield return ProcessPolearmExpert(unit, mover, movement, battleManager);
            }

            foreach (var brace in unit.RulesetActor.GetSubFeaturesByType<MartialTactician.Brace>()
                         .Where(brace => MartialTactician.Brace.CanReact(unit)))
            {
                yield return brace.Process(unit, mover, movement, battleManager);
            }
        }
    }

    internal static void CleanMovingCache()
    {
        MovingCharactersCache.Clear();
    }

    private static IEnumerator ProcessPolearmExpert([NotNull] GameLocationCharacter attacker,
        [NotNull] GameLocationCharacter mover,
        (int3, int3) movement,
        GameLocationBattleManager battleManager)
    {
        if (!CanMakeAoOOnEnemyEnterReach(attacker.RulesetCharacter) ||
            !battleManager.CanPerformOpportunityAttackOnCharacter(attacker, mover, movement.Item2, movement.Item1,
                out var attackMode))
        {
            yield break;
        }

        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var count = actionService.PendingReactionRequestGroups.Count;
        var reactionParams = new CharacterActionParams(
            attacker,
            Id.AttackOpportunity,
            attackMode,
            mover,
            new ActionModifier());

        var manager = actionService as GameLocationActionManager;
        if (manager == null)
        {
            //shouldn't happen, but just in case use regular AoO reaction
            actionService.ReactForOpportunityAttack(reactionParams);
        }
        else
        {
            manager.AddInterruptRequest(new ReactionRequestReactionAttack("AoOEnter", reactionParams));
        }


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

        attackMode = attacker.FindActionAttackMode(Id.AttackOpportunity);

        if (attackMode == null)
        {
            return false;
        }

        var evaluationParams = new BattleDefinitions.AttackEvaluationParams();

        evaluationParams.FillForPhysicalReachAttack(attacker, attacker.LocationPosition, attackMode, defender,
            defender.LocationPosition, actionModifier);

        return battleManager.CanAttack(evaluationParams);
    }

    internal static bool IsSubjectToAttackOfOpportunity(RulesetCharacter character, RulesetCharacter attacker,
        bool def, float distance)
    {
        if (attacker.GetSubFeaturesByType<ICanIgnoreAoOImmunity>()
            .Any(f => f.CanIgnoreAoOImmunity(character, attacker, distance)))
        {
            return true;
        }

        if (character.HasSubFeatureOfType<IImmuneToAooOfRecentAttackedTarget>() &&
            character.proximityByAttackedCreature.TryGetValue(attacker.Guid, out var value) &&
            value == (int)RuleDefinitions.AttackProximity.Melee)
        {
            return false;
        }

        return def;
    }
}

internal sealed class CanIgnoreDisengage : ICanIgnoreAoOImmunity
{
    public bool CanIgnoreAoOImmunity([NotNull] RulesetCharacter character, RulesetCharacter attacker, float distance)
    {
        var featuresToBrowse = new List<FeatureDefinition>();

        character.EnumerateFeaturesToBrowse<ICombatAffinityProvider>(featuresToBrowse);

        var service = ServiceRepository.GetService<IRulesetImplementationService>();
        var disengaging = DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityDisengaging;

        foreach (var feature in featuresToBrowse)
        {
            if (feature != disengaging
                && feature is ICombatAffinityProvider affinityProvider
                && service.IsSituationalContextValid(
                    new RulesetImplementationDefinitions.SituationalContextParams(
                        affinityProvider.SituationalContext, attacker,
                        character, service.FindSourceIdOfFeature(character, feature),
                        affinityProvider.RequiredCondition, false, null))
                && affinityProvider.IsImmuneToOpportunityAttack(character, attacker, distance))
            {
                return false;
            }
        }

        return true;
    }
}

internal sealed class SentinelFeatMarker
{
}

internal sealed class CanMakeAoOOnReachEntered
{
    private readonly IsCharacterValidHandler[] validators;

    internal CanMakeAoOOnReachEntered(params IsCharacterValidHandler[] validators)
    {
        this.validators = validators;
    }

    internal bool IsValid([CanBeNull] RulesetCharacter character)
    {
        return character != null && character.IsValid(validators);
    }
}
