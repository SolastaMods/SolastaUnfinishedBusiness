using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomUI;
using static ActionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

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
        var unitCharacter = unit.RulesetCharacter;
        var defenderCharacter = defender.RulesetCharacter;

        if (defenderCharacter == null ||
            !attacker.IsOppositeSide(unit.Side) ||
            defender.Side != unit.Side ||
            unit == defender ||
            !unitCharacter.HasSubFeatureOfType<SpiritualShielding>())
        {
            yield break;
        }

        //Is this unit able to react (not paralyzed, prone etc.)?
        if (!unit.CanReact(true))
        {
            yield break;
        }

        //Can this unit see defender?
        if (!unit.PerceivedAllies.Contains(defender))
        {
            yield break;
        }

        //Does this unit has enough Channel Divinity uses left?
        var maxUses = unitCharacter.TryGetAttributeValue(AttributeDefinitions.ChannelDivinityNumber);
        if (unitCharacter.UsedChannelDivinity >= maxUses)
        {
            yield break;
        }

        //Is defender already shielded?
        if (defenderCharacter.HasConditionOfType(ConditionDefinitions.ConditionShielded))
        {
            yield break;
        }

        var totalAttack = attackRoll
            + attackMode?.ToHitBonus ?? rulesetEffect?.MagicAttackBonus ?? 0
            + attackModifier.AttackRollModifier;

        //Can shielding prevent hit?
        if (!defenderCharacter.CanMagicEffectPreventHit(SpellDefinitions.Shield, totalAttack))
        {
            yield break;
        }

        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var count = actionService.PendingReactionRequestGroups.Count;

        var temp = new CharacterActionParams(unit, (Id)ExtraActionId.DoNothingReaction)
        {
            StringParameter = "CustomReactionSpiritualShieldingDescription"
                .Formatted(Category.Reaction, defender.Name, attacker.Name)
        };

        RequestCustomReaction("SpiritualShielding", temp);

        yield return battleManager.WaitForReactions(unit, actionService, count);

        if (!temp.ReactionValidated)
        {
            yield break;
        }

        //Spend resources
        unitCharacter.usedChannelDivinity++;

        var rulesetCondition = RulesetCondition.CreateActiveCondition(
            defenderCharacter.Guid,
            ConditionDefinitions.ConditionShielded,
            RuleDefinitions.DurationType.Round,
            1,
            RuleDefinitions.TurnOccurenceType.StartOfTurn,
            unitCharacter.Guid,
            unitCharacter.CurrentFaction.Name);

        defenderCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
    }

    private static void RequestCustomReaction(string type, CharacterActionParams actionParams)
    {
        var actionManager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

        if (actionManager == null)
        {
            return;
        }

        var reactionRequest = new ReactionRequestCustom(type, actionParams)
        {
            Resource = ReactionResourceChannelDivinity.Instance
        };

        actionManager.AddInterruptRequest(reactionRequest);
    }

    private sealed class SpiritualShielding
    {
    }
}
