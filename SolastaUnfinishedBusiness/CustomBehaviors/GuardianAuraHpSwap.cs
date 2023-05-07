using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using static ActionDefinitions;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal static class GuardianAuraHpSwap
{
    private static readonly FeatureDefinitionPower DummyAuraGuardianPower = FeatureDefinitionPowerBuilder
        .Create("GuardianAura")
        .SetGuiPresentation(DatabaseHelper.SpellDefinitions.ShieldOfFaith.guiPresentation)
        .AddToDB();

    internal static readonly object AuraGuardianConditionMarker = new GuardianAuraCondition();
    internal static readonly object AuraGuardianUserMarker = new GuardianAuraUser();

    internal static IEnumerator ProcessOnCharacterAttackHitFinished(
        GameLocationBattleManager battleManager,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RulesetAttackMode attackerAttackMode,
        RulesetEffect rulesetEffect,
        int damageAmount)
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
                yield return ActiveHealthSwap(
                    unit, attacker, defender, battleManager, attackerAttackMode, rulesetEffect, damageAmount);
            }
        }
    }

    private static IEnumerator ActiveHealthSwap(
        [NotNull] GameLocationCharacter unit,
        [NotNull] GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationBattleManager battleManager,
        RulesetAttackMode attackerAttackMode,
        RulesetEffect rulesetEffect,
        int damageAmount)
    {
        if (!attacker.IsOppositeSide(unit.Side) ||
            defender.Side != unit.Side ||
            unit == defender ||
            !(unit.RulesetCharacter?.HasSubFeatureOfType<GuardianAuraUser>() ?? false) ||
            !(defender.RulesetCharacter?.HasSubFeatureOfType<GuardianAuraCondition>() ?? false))
        {
            yield break;
        }

        if (!unit.CanReact(true))
        {
            yield break;
        }

        if (defender.RulesetCharacter.isDeadOrDyingOrUnconscious)
        {
            yield break;
        }

        if (damageAmount == 0)
        {
            yield break;
        }

        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var count = actionService.PendingReactionRequestGroups.Count;
        var actionParams = new CharacterActionParams(unit, (Id)ExtraActionId.DoNothingReaction)
        {
            StringParameter = "CustomReactionGuardianAuraDescription"
                .Formatted(Category.Reaction, defender.Name, damageAmount)
        };

        RequestCustomReaction("GuardianAura", actionParams);

        yield return battleManager.WaitForReactions(unit, actionService, count);

        if (!actionParams.ReactionValidated)
        {
            yield break;
        }

        DamageForm damage = null;

        if (attackerAttackMode != null)
        {
            damage = attackerAttackMode.EffectDescription.FindFirstDamageForm();
        }

        if (rulesetEffect != null)
        {
            damage = rulesetEffect.EffectDescription.FindFirstDamageForm();
        }

        defender.RulesetCharacter.HealingReceived(
            defender.RulesetCharacter, damageAmount, unit.Guid, RuleDefinitions.HealingCap.MaximumHitPoints, null);
        defender.RulesetCharacter.ForceSetHealth(damageAmount, true);

        if (damage != null)
        {
            RulesetActor.InflictDamage(
                damageAmount,
                damage,
                damage.DamageType,
                new RulesetImplementationDefinitions.ApplyFormsParams { targetCharacter = unit.RulesetCharacter },
                unit.RulesetCharacter,
                false,
                attacker.Guid,
                false,
                new List<string>(),
                new RollInfo(RuleDefinitions.DieType.D1, new List<int>(), damageAmount),
                true,
                out _);

            unit.RulesetCharacter.SustainDamage(
                damageAmount, damage.DamageType, false, attacker.Guid, null, out _);
        }

        GameConsoleHelper.LogCharacterUsedPower(unit.RulesetCharacter, DummyAuraGuardianPower,
            "Feedback/&GuardianAuraHeal");
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

    private sealed class GuardianAuraCondition
    {
    }

    private sealed class GuardianAuraUser
    {
    }
}
