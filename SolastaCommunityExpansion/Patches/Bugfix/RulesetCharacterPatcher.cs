using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaCommunityExpansion.Patches.Bugfix;

// Fixes `Shield of Faith` being available to cast blocking reaction cast of `Shield`
// Removes all non-Reaction spells from consideration
[HarmonyPatch(typeof(RulesetCharacter), "CanCastAttackOutcomeAlterationSpell")]
internal static class RulesetCharacter_CanCastAttackOutcomeAlterationSpell
{
    private static void EnumerateReactionSpells([NotNull] RulesetCharacter caster)
    {
        caster.EnumerateUsableSpells();
        caster.UsableSpells.RemoveAll(s => s.ActivationTime != RuleDefinitions.ActivationTime.Reaction);
    }

    [NotNull]
    internal static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();
        var customMethod = new Action<RulesetCharacter>(EnumerateReactionSpells).Method;
        var enumerateIndex = codes.FindIndex(x =>
            x.opcode == OpCodes.Call && x.operand.ToString().Contains("EnumerateUsableSpells")
        );

        if (enumerateIndex > 0)
        {
            codes[enumerateIndex] = new CodeInstruction(OpCodes.Call, customMethod);
        }

        return codes;
    }
}
