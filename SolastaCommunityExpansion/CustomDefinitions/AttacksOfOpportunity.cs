using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.CustomUI;
using SolastaCommunityExpansion.Feats;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;

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

        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var count = actionService.PendingReactionRequestGroups.Count;

        //Process fetures on attacker or defender

        var units = battle.AllContenders
            .Where(u => !u.RulesetCharacter.IsDeadOrDyingOrUnconscious)
            .ToArray();

        //Process other participants of the battle
        foreach (var unit in units)
        {
            if (attacker != unit && defender != unit)
            {
                ProcessSentinel(unit, attacker, defender, battleManager);
            }
        }

        yield return battleManager.InvokeMethod("WaitForReactions", attacker, actionService, count);
    }

    private static void ProcessSentinel(GameLocationCharacter unit, GameLocationCharacter attacker,
        GameLocationCharacter defender, GameLocationBattleManager battleManager)
    {
        if (attacker.IsOppositeSide(unit.Side)
            && defender.Side == unit.Side
            && (unit.RulesetCharacter?.HasSubFeatureOfType<SentinelFeatMarker>() ?? false)
            && !(defender.RulesetCharacter?.HasSubFeatureOfType<SentinelFeatMarker>() ?? false)
            && CanMakeAoO(unit, attacker, out var opportunityAttackMode, out var actionModifier,
                battleManager))
        {
            RequestReactionAttack(EWFeats.SentinelFeat, new CharacterActionParams(
                unit,
                ActionDefinitions.Id.AttackOpportunity,
                opportunityAttackMode,
                attacker,
                actionModifier)
            );
        }
    }

    public static void RequestReactionAttack(string type, CharacterActionParams actionParams)
    {
        var actionMnager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
        if (actionMnager != null)
        {
            actionParams.AttackMode?.AddAttackTagAsNeeded(NotAoOTag);
            actionMnager.InvokeMethod("AddInterruptRequest", new ReactionRequestReactionAttack(type, actionParams));
        }
    }

    private static bool CanMakeAoO(GameLocationCharacter attacker, GameLocationCharacter defender,
        out RulesetAttackMode attackMode, out ActionModifier actionModifier,
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
        return battleManager.CanAttack(evaluationParams, false);
    }

    public static bool IsSubjectToAttackOfOpportunity(RulesetCharacter character, RulesetCharacter attacker,
        bool def)
    {
        return def || attacker.GetSubFeaturesByType<ICanIgnoreAoOImmunity>()
            .Any(f => f.CanIgnoreAoOImmunity(character, attacker));
    }
}

internal class CanIgnoreDisengage : ICanIgnoreAoOImmunity
{
    public bool CanIgnoreAoOImmunity(RulesetCharacter character, RulesetCharacter attacker)
    {
        var FeaturesToBrowse = new List<FeatureDefinition>();
        character.EnumerateFeaturesToBrowse<ICombatAffinityProvider>(FeaturesToBrowse);
        var service = ServiceRepository.GetService<IRulesetImplementationService>();
        var disengaging = DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityDisengaging;
        foreach (var feature in FeaturesToBrowse)
        {
            if (feature != disengaging
                && feature is ICombatAffinityProvider affinityProvider
                && service.IsSituationalContextValid(
                    new RulesetImplementationDefinitions.SituationalContextParams(
                        affinityProvider.SituationalContext, attacker,
                        character, service.FindSourceIdOfFeature(character, feature),
                        affinityProvider.RequiredCondition, null))
                && affinityProvider.IsImmuneToOpportunityAttack(character, attacker))
            {
                return false;
            }
        }

        return true;
    }
}

internal class SentinelFeatMarker
{
}