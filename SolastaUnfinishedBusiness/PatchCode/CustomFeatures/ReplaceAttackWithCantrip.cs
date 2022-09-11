using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using UnityEngine;

namespace SolastaUnfinishedBusiness.PatchCode.CustomFeatures;

internal static class ReplaceAttackWithCantrip
{
    public static void AllowCastDuringMainAttack(GameLocationCharacter character, ActionDefinitions.Id actionId,
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

        if (!character.RulesetCharacter.HasSubFeatureOfType<IReplaceAttackWithCantrip>())
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

    public static void AllowAttacksAfterCantrip(GameLocationCharacter __instance, CharacterActionParams actionParams,
        ActionDefinitions.ActionScope scope)
    {
        if (scope != ActionDefinitions.ActionScope.Battle)
        {
            return;
        }

        var rulesetCharacter = actionParams.actingCharacter.RulesetCharacter;

        if (!rulesetCharacter.HasSubFeatureOfType<IReplaceAttackWithCantrip>())
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

        var num = 0;
        foreach (var attackMode in actionParams.ActingCharacter.RulesetCharacter.AttackModes)
        {
            if (attackMode.ActionType == ActionDefinitions.ActionType.Main)
            {
                num = Mathf.Max(num, attackMode.AttacksNumber);
            }
        }

        //increment used attacks to count cantrip as attack
        __instance.usedMainAttacks++;

        //if still attacks left - refund main action
        if (__instance.usedMainAttacks < num)
        {
            __instance.currentActionRankByType[ActionDefinitions.ActionType.Main]--;
        }
        //This doesn't look right
        // else
        // {
        //     __instance.usedMainAttacks = 0;
        // }
    }
}
