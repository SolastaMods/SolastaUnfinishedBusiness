using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
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
        GameLocationCharacter defender)
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
        var actionManager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

        var units = battle.AllContenders
            .Where(u => u.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
            .ToArray();

        //Process other participants of the battle
        foreach (var unit in units)
        {
            if (attacker != unit && defender != unit)
            {
                yield return ProcessSentinel(unit, attacker, defender, battleManager, actionManager);
            }
        }
    }

    private static IEnumerator ProcessSentinel(
        [NotNull] GameLocationCharacter unit,
        [NotNull] GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationBattleManager battleManager,
        GameLocationActionManager actionManager)
    {
        if (!attacker.IsOppositeSide(unit.Side) || defender.Side != unit.Side || unit == defender)
        {
            yield break;
        }

        foreach (var reaction in unit.RulesetActor.GetSubFeaturesByType<SentinelFeatMarker>()
                     .Where(feature => feature.IsValid(unit, attacker)))
        {
            yield return reaction.Process(unit, attacker, null, battleManager, actionManager, false);
        }
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

        var actionManager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

        var battle = battleManager.Battle;

        if (battle == null)
        {
            yield break;
        }

        var units = battle.AllContenders
            .Where(u => u.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
            .ToList();

        //Process other participants of the battle
        foreach (var unit in units)
        {
            if (mover == unit
                || !mover.IsOppositeSide(unit.Side)
                || !MovingCharactersCache.TryGetValue(mover.Guid, out var movement))
            {
                continue;
            }

            foreach (var brace in unit.RulesetActor.GetSubFeaturesByType<CanMakeAoOOnReachEntered>()
                         .Where(feature => feature.IsValid(unit, mover)))
            {
                yield return brace.Process(unit, mover, movement, battleManager, actionManager, brace.AllowRange);
            }
        }
    }

    internal static void CleanMovingCache()
    {
        MovingCharactersCache.Clear();
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

internal sealed class SentinelFeatMarker : CustomReactionAttack
{
    public SentinelFeatMarker()
    {
        Name = "ReactionAttackSentinel";
    }
}

internal class CanMakeAoOOnReachEntered : CustomReactionAttack
{
    public CanMakeAoOOnReachEntered()
    {
        Name = "ReactionAttackAoOEnter";
    }

    public bool AllowRange { get; set; }
}

internal class CustomReactionAttack
{
    public delegate IEnumerator Handler([NotNull] GameLocationCharacter attacker,
        [NotNull] GameLocationCharacter defender, GameLocationBattleManager battleManager,
        GameLocationActionManager actionManager, ReactionRequest request);

    protected string Name { get; set; }
    protected IsCharacterValidHandler ValidateAttacker { get; set; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    private IsCharacterValidHandler ValidateMover { get; set; }
    public IsWeaponValidHandler WeaponValidator { get; set; }
    public Handler BeforeReaction { get; set; }
    public Handler AfterReaction { get; set; }
    [UsedImplicitly] public bool IgnoreReactionUses { get; set; }
    public bool AccountAoOImmunity { get; set; }

    internal bool IsValid(GameLocationCharacter attacker, GameLocationCharacter mover)
    {
        var rulesetAttacker = attacker.RulesetCharacter;
        var rulesetMover = mover.RulesetCharacter;

        return rulesetAttacker != null
               && rulesetMover != null
               && (ValidateAttacker?.Invoke(rulesetAttacker) ?? true)
               && attacker.CanReact(IgnoreReactionUses)
               && (ValidateMover?.Invoke(rulesetMover) ?? true);
    }

    public IEnumerator Process(
        [NotNull] GameLocationCharacter attacker,
        [NotNull] GameLocationCharacter mover,
        (int3 from, int3 to)? movement,
        GameLocationBattleManager battleManager,
        GameLocationActionManager actionManager,
        bool allowRange)
    {
        if (!attacker.CanPerformOpportunityAttackOnCharacter(mover, movement?.to, movement?.from,
                out var mode, out var attackModifier, allowRange, battleManager, AccountAoOImmunity, WeaponValidator))
        {
            yield break;
        }

        var attackMode = RulesetAttackMode.AttackModesPool.Get();
        attackMode.Copy(mode);
        attackMode.actionType = ActionType.Reaction;

        var reactionRequest = MakeReactionRequest(attacker, mover, attackMode, attackModifier);

        if (BeforeReaction != null)
        {
            yield return BeforeReaction(attacker, mover, battleManager, actionManager, reactionRequest);
        }

        var previousReactionCount = actionManager.PendingReactionRequestGroups.Count;

        actionManager.AddInterruptRequest(reactionRequest);

        yield return battleManager.WaitForReactions(attacker, actionManager, previousReactionCount);

        if (AfterReaction != null)
        {
            yield return AfterReaction(attacker, mover, battleManager, actionManager, reactionRequest);
        }

        RulesetAttackMode.AttackModesPool.Return(attackMode);
    }

    protected virtual ReactionRequest MakeReactionRequest(GameLocationCharacter attacker,
        GameLocationCharacter defender, RulesetAttackMode attackMode, ActionModifier attackModifier)
    {
        var reactionParams = new CharacterActionParams(
            attacker,
            Id.AttackOpportunity,
            attackMode,
            defender,
            attackModifier) { StringParameter2 = Name };

        return new ReactionRequestWarcaster(Name, reactionParams);
    }
}
