using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaCommunityExpansion.CustomUI;

public static class ExtraAttacksOnActionPanel
{
    //TODO: move other patch implementations for extra attacks here
    
    // Uses attack mode from cursor's ActionParams, instead of first one matching action type
    // Without this when you click on any attack in actions panel targeting would work as if you clicked on the 1st
    public static void ApplyCursorLocationSelectTargetTranspile(List<CodeInstruction> instructions)
    {
        var insertionIndex = instructions.FindIndex(x => x.opcode == OpCodes.Ldloc_2);

        if (insertionIndex > 0)
        {
            var method = new Func<RulesetAttackMode, CursorLocationSelectTarget, RulesetAttackMode>(
                GetAttackModeFromCursorActionParams).Method;

            instructions.InsertRange(insertionIndex + 1, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, method),
                new CodeInstruction(OpCodes.Stloc_2),
                new CodeInstruction(OpCodes.Ldloc_2),
            });
        }
    }

    private static RulesetAttackMode GetAttackModeFromCursorActionParams(RulesetAttackMode def,
        CursorLocationSelectTarget cursor)
    {
        if (cursor == null)
        {
            return def;
        }

        return cursor.ActionParams?.AttackMode ?? def;
    }
}