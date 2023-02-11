using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomUI;
using static ActionDefinitions;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal static class BlockAttacks
{
    internal static readonly object SpiritualShieldingMarker = new SpiritualShielding();

    internal static IEnumerator ProcessOnCharacterAttackHitConfirm(
        GameLocationBattleManager battleManager,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RulesetAttackMode attackMode,
        RulesetEffect rulesetEffect,
        ActionModifier attackModifier,
        int attackRoll)
    {
        if (battleManager == null)
        {
            yield break;
        }

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

        foreach (var unit in units)
        {
            if (attacker != unit && defender != unit)
            {
                yield return ActiveSpiritualShielding(
                    unit, attacker, defender, battleManager, attackMode, rulesetEffect, attackModifier, attackRoll);
            }
        }
    }

    private static IEnumerator ActiveSpiritualShielding(
        [NotNull] GameLocationCharacter unit,
        [NotNull] GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationBattleManager battleManager,
        RulesetAttackMode attackMode,
        RulesetEffect rulesetEffect,
        ActionModifier attackModifier,
        int attackRoll
    )
    {
        if (!attacker.IsOppositeSide(unit.Side) || defender.Side != unit.Side || unit == defender
            || !(unit.RulesetCharacter?.HasSubFeatureOfType<SpiritualShielding>() ?? false))
        {
            yield break;
        }

        if (unit.RulesetCharacter.UsedChannelDivinity == 1)
        {
            yield break;
        }

        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var count = actionService.PendingReactionRequestGroups.Count;

        if (defender.RulesetCharacter.GetAttribute(AttributeDefinitions.ArmorClass).CurrentValue + 5 <= attackRoll +
            (attackMode?.ToHitBonus ?? rulesetEffect.MagicAttackBonus) + attackModifier.AttackRollModifier)
        {
            yield break;
        }

        if (defender.RulesetCharacter.GetAttribute(AttributeDefinitions.ArmorClass).CurrentValue > attackRoll +
            (attackMode?.ToHitBonus ?? rulesetEffect.MagicAttackBonus) + attackModifier.AttackRollModifier)
        {
            yield break;
        }

        attackMode ??= defender.FindActionAttackMode(Id.AttackMain);

        var guiUnit = new GuiCharacter(unit);
        var guiDefender = new GuiCharacter(defender);

        var temp = new CharacterActionParams(
            unit,
            (Id)ExtraActionId.DoNothingReaction,
            attackMode,
            defender,
            new ActionModifier())
        {
            StringParameter = Gui.Format("Reaction/&CustomReactionSpiritualShieldingDescription", guiUnit.Name,
                guiDefender.Name)
        };

        RequestCustomReaction("SpiritualShielding", temp);

        yield return battleManager.WaitForReactions(unit, actionService, count);

        if (!temp.ReactionValidated)
        {
            yield break;
        }

        unit.RulesetCharacter.usedChannelDivinity++;

        var rulesetCondition = RulesetCondition.CreateActiveCondition(
            defender.RulesetCharacter.Guid,
            DatabaseHelper.ConditionDefinitions.ConditionShielded,
            RuleDefinitions.DurationType.Round,
            1,
            RuleDefinitions.TurnOccurenceType.StartOfTurn,
            unit.RulesetCharacter.Guid,
            unit.RulesetCharacter.CurrentFaction.Name);

        defender.RulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
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

    private sealed class SpiritualShielding
    {
    }
}
