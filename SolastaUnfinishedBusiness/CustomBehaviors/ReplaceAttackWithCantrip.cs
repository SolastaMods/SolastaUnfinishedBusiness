using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using UnityEngine;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal static class ReplaceAttackWithCantrip
{
    internal static void AllowCastDuringMainAttack(
        GameLocationCharacter character,
        ActionDefinitions.Id actionId,
        ActionDefinitions.ActionScope scope,
        ref ActionDefinitions.ActionStatus result)
    {
        if (scope != ActionDefinitions.ActionScope.Battle)
        {
            return;
        }

        if (actionId != ActionDefinitions.Id.CastMain)
        {
            return;
        }

        if (!character.RulesetCharacter.HasSubFeatureOfType<IAttackReplaceWithCantrip>())
        {
            return;
        }

        if (character.usedMainAttacks == 0)
        {
            return;
        }

        if (character.usedMainCantrip)
        {
            return;
        }

        result = ActionDefinitions.ActionStatus.Available;
    }

    internal static void AllowAttacksAfterCantrip(GameLocationCharacter __instance, CharacterActionParams actionParams,
        ActionDefinitions.ActionScope scope)
    {
        if (scope != ActionDefinitions.ActionScope.Battle)
        {
            return;
        }

        var rulesetCharacter = actionParams.actingCharacter.RulesetCharacter;

        if (!rulesetCharacter.HasSubFeatureOfType<IAttackReplaceWithCantrip>())
        {
            return;
        }

        if (actionParams.actionDefinition.Id != ActionDefinitions.Id.CastMain)
        {
            return;
        }

        if (actionParams.RulesetEffect is not RulesetEffectSpell spellEffect ||
            spellEffect.spellDefinition.spellLevel > 0)
        {
            return;
        }

        var num = actionParams.ActingCharacter.RulesetCharacter.AttackModes
            .Where(attackMode => attackMode.ActionType == ActionDefinitions.ActionType.Main)
            .Aggregate(0, (current, attackMode) => Mathf.Max(current, attackMode.AttacksNumber));

        //increment used attacks to count cantrip as attack
        __instance.usedMainAttacks++;

        //if still attacks left - refund main action
        if (__instance.usedMainAttacks < num)
        {
            __instance.currentActionRankByType[ActionDefinitions.ActionType.Main]--;
        }

        // reset __instance.usedMainAttacks so doesn't block haste or action surge
        if (__instance.usedMainAttacks >= num)
        {
            __instance.usedMainAttacks = 0;
        }
    }

    internal static void MightRefundOneAttackOfMainAction(
        GameLocationCharacter __instance,
        CharacterActionParams actionParams,
        ActionDefinitions.ActionScope scope)
    {
        // should be in battle only
        if (scope != ActionDefinitions.ActionScope.Battle)
        {
            return;
        }

        // if main is still available then don't refund
        if (__instance.currentActionRankByType[ActionDefinitions.ActionType.Main] <= 0)
        {
            return;
        }

        var features = actionParams.actingCharacter.RulesetCharacter
            .GetSubFeaturesByType<IMightRefundOneAttackOfMainAction>();
        var refund = features.Aggregate(false,
            (current, f) => current | f.MightRefundOneAttackOfMainAction(__instance, actionParams));

        if (!refund)
        {
            return;
        }

        __instance.currentActionRankByType[ActionDefinitions.ActionType.Main]--;
    }
}
