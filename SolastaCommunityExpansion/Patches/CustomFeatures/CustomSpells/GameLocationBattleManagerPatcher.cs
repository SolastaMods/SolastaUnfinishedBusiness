using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomSpells;

//Checks if attack cantrip is valid to be cast as readied action on a target
[HarmonyPatch(typeof(GameLocationBattleManager), "IsValidAttackForReadiedAction")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GameLocationBattleManager_IsValidAttackForReadiedAction
{
    internal static void Postfix(
        GameLocationBattleManager __instance,
        ref bool __result,
        BattleDefinitions.AttackEvaluationParams attackParams,
        bool forbidDisadvantage)
    {
        if (!DatabaseHelper.TryGetDefinition<SpellDefinition>(attackParams.effectName, null, out var cantrip))
        {
            return;
        }

        var attack = cantrip.GetFirstSubFeatureOfType<IPerformAttackAfterMagicEffectUse>();
        var canAttack = attack?.CanAttack;

        if (canAttack != null)
        {
            __result = canAttack(attackParams.attacker, attackParams.defender);
        }
    }
}

//Makes only preferred cantrip valid if it is selected and forced
[HarmonyPatch(typeof(GameLocationBattleManager), "CanPerformReadiedActionOnCharacter")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GameLocationBattleManager_CanPerformReadiedActionOnCharacter
{
    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();
        var customBindMethod =
            new Func<List<SpellDefinition>, SpellDefinition, bool>(CheckAndModifyCantrips).Method;

        var containsIndex = -1;
        //TODO: is there a better way to detect proper placament?
        for (var i = 0; i < codes.Count; i++)
        {
            if (i < 1) { continue; }

            var code = codes[i];
            if (code.opcode == OpCodes.Callvirt && code.operand.ToString().Contains("Contains"))
            {
                var prev = codes[i - 1];
                if (prev.opcode == OpCodes.Callvirt &&
                    prev.operand.ToString().Contains("PreferredReadyCantrip"))
                {
                    containsIndex = i;
                    break;
                }
            }
        }

        if (containsIndex > 0)
        {
            codes[containsIndex] = new CodeInstruction(OpCodes.Call, customBindMethod);
        }

        return codes.AsEnumerable();
    }

    private static bool CheckAndModifyCantrips(List<SpellDefinition> readied,
        SpellDefinition preferred)
    {
        if (CustomReactionsContext.ForcePreferredCantrip)
        {
            readied.RemoveAll(c => c != preferred);
            return !readied.Empty();
        }

        return readied.Contains(preferred);
    }
}
