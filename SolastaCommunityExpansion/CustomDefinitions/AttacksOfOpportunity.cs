using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public static readonly ICanIgnoreAoOImmunity CanIgnoreDisengage = new CanIgnoreDisengage();
    public static readonly object SentinelFeatMarker = new SentinelFeatMarker();

    public static IEnumerator ProcessAoOOnCharacterAttackFinished(
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

        var units = battle.AllContenders
            .Where(u => !u.RulesetCharacter.IsDeadOrDyingOrUnconscious)
            .ToArray();

        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var count = actionService.PendingReactionRequestGroups.Count;
        foreach (var unit in units)
        {
            if (attacker != unit
                && defender != unit
                && attacker.IsOppositeSide(unit.Side)
                && defender.Side == unit.Side
                && unit.RulesetCharacter.HasSubFeatureOfType<SentinelFeatMarker>()
                && CanMakeAoO(unit, attacker, out var opportunityAttackMode, out var actionModifier, battleManager)
               )
            {
                var reactionParams = new CharacterActionParams(unit, ActionDefinitions.Id.AttackOpportunity,
                    opportunityAttackMode, attacker, actionModifier);
                actionService.ReactForOpportunityAttack(reactionParams);
            }
        }

        yield return battleManager.InvokeMethod("WaitForReactions", attacker, actionService, count);
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