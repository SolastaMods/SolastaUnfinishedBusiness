using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.PatchCode.CustomFeatures;

internal static class PugilistFightingStyle
{
    // Removes check that makes `ShoveBonus` action unavailable if character has no shield
    // Replaces call to RulesetActor.IsWearingShield with custom method that always returns true
    public static void RemoveShieldRequiredForBonusPush(List<CodeInstruction> codes)
    {
        var customMethod = new Func<RulesetActor, bool>(CustomMethod).Method;

        var bindIndex = codes.FindIndex(x =>
        {
            if (x.operand == null)
            {
                return false;
            }

            var operand = x.operand.ToString();
            return operand.Contains("IsWearingShield");
        });

        if (bindIndex > 0)
        {
            codes[bindIndex] = new CodeInstruction(OpCodes.Call, customMethod);
        }
    }

    private static bool CustomMethod(RulesetActor actor)
    {
        return true;
    }
}
