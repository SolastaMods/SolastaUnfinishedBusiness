using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Api.Helpers;

internal static class TranspileAssertHelper
{
    internal static void RemoveBoolAsserts(List<CodeInstruction> codes)
    {
        int assertIndex;
        var noAssert = new Action<bool>(NoAssert).Method;
        while ((assertIndex = GetNextAssert(codes, typeof(bool))) >= 0)
        {
            codes[assertIndex] = new CodeInstruction(OpCodes.Call, noAssert);
        }
    }

    private static int GetNextAssert(List<CodeInstruction> codes, params Type[] types)
    {
        var assert = typeof(Trace).GetMethod("Assert", types);
        return codes.FindIndex(x => x.Calls(assert));
    }

    private static void NoAssert(bool condition)
    {
    }
}
