using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Subclasses;
using static ActionDefinitions;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

//Old Attack of Opportunity before it became too narrow to use
[UsedImplicitly]
internal class DefensiveStrikeAttack
{
    private const string NotAoOTag = "NotAoO"; //Used to distinguish reaction attacks from AoO
    internal static readonly object DefensiveStrikeMarker = new DefensiveStrikeMarker();

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

        var units = battle.AllContenders
            .Where(u => !u.RulesetCharacter.IsDeadOrDyingOrUnconscious)
            .ToArray();

        //Process other participants of the battle
        foreach (var unit in units)
        {
            if (attacker != unit && defender != unit)
            {
                yield return ActiveDefensiveStrike(unit, attacker, defender, battleManager);
            }
        }
    }

    private static IEnumerator ActiveDefensiveStrike(
        [NotNull] GameLocationCharacter unit,
        [NotNull] GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationBattleManager battleManager)
    {
        if (!attacker.IsOppositeSide(unit.Side) || defender.Side != unit.Side || unit == defender
            || !(unit.RulesetCharacter?.HasSubFeatureOfType<DefensiveStrikeMarker>() ?? false)
            || !CanMakeAoO(defender, attacker, out var opportunityAttackMode, out var actionModifier, battleManager))
        {
            yield break;
        }

        if (unit.RulesetCharacter.UsedChannelDivinity == 1)
        {
            yield break;
        }

        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var count = actionService.PendingReactionRequestGroups.Count;
        var temp = new CharacterActionParams(
            unit,
            (Id)ExtraActionId.DoNothingReaction,
            opportunityAttackMode,
            defender,
            new ActionModifier()
        );

        RequestCustomReaction(OathOfAltruism.Name2, temp);

        yield return battleManager.WaitForReactions(unit, actionService, count);

        if (!temp.ReactionValidated)
        {
            yield break;
        }

        actionService = ServiceRepository.GetService<IGameLocationActionService>();
        count = actionService.PendingReactionRequestGroups.Count;

        var damage = opportunityAttackMode.EffectDescription.FindFirstDamageForm();

        damage.bonusDamage +=
            AttributeDefinitions.ComputeAbilityScoreModifier(unit.RulesetCharacter
                .GetAttribute(AttributeDefinitions.Charisma).CurrentValue) + unit.RulesetCharacter
                .GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;

        opportunityAttackMode.toHitBonus +=
            AttributeDefinitions.ComputeAbilityScoreModifier(unit.RulesetCharacter
                .GetAttribute(AttributeDefinitions.Charisma).CurrentValue) + unit.RulesetCharacter
                .GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;

        var temp2 = new CharacterActionParams(
            defender,
            Id.AttackOpportunity,
            opportunityAttackMode,
            attacker,
            actionModifier);

        RequestReactionAttack(OathOfAltruism.Name3, temp2);

        yield return battleManager.WaitForReactions(defender, actionService, count);

        if (!temp2.reactionValidated)
        {
            yield break;
        }

        unit.RulesetCharacter.usedChannelDivinity++;
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

    private static void RequestCustomReaction(string type, CharacterActionParams actionParams)
    {
        var actionManager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

        if (actionManager == null)
        {
            return;
        }

        var reactionRequest = new ReactionRequestCustom(type, actionParams);

        actionManager.AddInterruptRequest(reactionRequest);
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
}

internal sealed class DefensiveStrikeMarker
{
}

#if false
internal sealed class CanMakeAoOOnReachEnteredDefensiveStrike
{
    private readonly IsCharacterValidHandler[] validators;

    internal CanMakeAoOOnReachEnteredDefensiveStrike(params IsCharacterValidHandler[] validators)
    {
        this.validators = validators;
    }

    internal bool IsValid([CanBeNull] RulesetCharacter character)
    {
        return character != null && character.IsValid(validators);
    }
}
#endif
