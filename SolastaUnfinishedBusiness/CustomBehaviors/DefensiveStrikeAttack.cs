using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Subclasses;
using static ActionDefinitions;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

//Old Attack of Opportunity before it became too narrow to use
internal static class DefensiveStrikeAttack
{
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
        var unitCharacter = unit.RulesetCharacter;
        if (!attacker.IsOppositeSide(unit.Side) || defender.Side != unit.Side || unit == defender
            || !unitCharacter.HasSubFeatureOfType<DefensiveStrikeMarker>())
        {
            yield break;
        }

        //Is this unit able to react (not paralyzed, prone etc.)?
        if (!unit.CanReact(true))
        {
            yield break;
        }

        //Is defender able to use reaction (has reaction available and not paralyzed, prone etc.)?
        if (!defender.CanReact())
        {
            yield break;
        }

        //Does unit has enough Channel Divinity uses left?
        var maxUses = unitCharacter.TryGetAttributeValue(AttributeDefinitions.ChannelDivinityNumber);
        if (unitCharacter.UsedChannelDivinity >= maxUses)
        {
            yield break;
        }

        //Find defender's attack mode
        var (opportunityAttackMode, actionModifier) = defender.GetFirstMeleeModeThatCanAttack(attacker, battleManager);
        if (opportunityAttackMode == null || actionModifier == null)
        {
            yield break;
        }

        //Calculate bonus
        var charisma = unitCharacter.TryGetAttributeValue(AttributeDefinitions.Charisma);
        var bonus = AttributeDefinitions.ComputeAbilityScoreModifier(charisma);

        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var count = actionService.PendingReactionRequestGroups.Count;
        var temp = new CharacterActionParams(
            unit,
            (Id)ExtraActionId.DoNothingFree,
            opportunityAttackMode,
            defender,
            new ActionModifier()
        )
        {
            StringParameter = Gui.Format($"Reaction/&CustomReaction{OathOfAltruism.Name2}Description",
                defender.Name, attacker.Name, bonus.ToString())
        };

        var actionManager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

        if (actionManager == null)
        {
            yield break;
        }

        var reactionRequest = new ReactionRequestCustom(OathOfAltruism.Name2, temp)
        {
            Resource = ReactionResourceChannelDivinity.Instance
        };

        actionManager.AddInterruptRequest(reactionRequest);

        yield return battleManager.WaitForReactions(unit, actionService, count);

        if (!temp.ReactionValidated)
        {
            yield break;
        }

        //spend resources
        unitCharacter.UsedChannelDivinity++;
        defender.SpendActionType(ActionType.Reaction);

        //create attack mode copy so we won't affect real one
        var tmp = RulesetAttackMode.AttackModesPool.Get();
        tmp.Copy(opportunityAttackMode);
        opportunityAttackMode = tmp;

        //Apply bonus to hit and damage of the attack mode
        opportunityAttackMode.EffectDescription.FindFirstDamageForm().bonusDamage += bonus;
        opportunityAttackMode.toHitBonus += bonus;

        opportunityAttackMode.ToHitBonusTrends.Add(new RuleDefinitions.TrendInfo(bonus,
            RuleDefinitions.FeatureSourceType.CharacterFeature, OathOfAltruism.Name2, unit));

        //Create and execute attack
        var enums = new CharacterActionAttack(new CharacterActionParams(
            defender,
            Id.AttackOpportunity,
            opportunityAttackMode,
            attacker,
            actionModifier)).Execute();

        while (enums.MoveNext())
        {
            yield return enums.Current;
        }

        //Rerturn our copied attack mode to the pool
        RulesetAttackMode.AttackModesPool.Return(opportunityAttackMode);
    }
}

internal sealed class DefensiveStrikeMarker
{
    private DefensiveStrikeMarker()
    {
    }

    public static DefensiveStrikeMarker Mark { get; } = new();
}
