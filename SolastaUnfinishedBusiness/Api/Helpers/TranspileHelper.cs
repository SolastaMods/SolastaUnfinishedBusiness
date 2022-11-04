using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Api.Helpers;

internal static class TranspileHelper
{
    public static IEnumerable<CodeInstruction> ReplaceCodeCall(
        IEnumerable<CodeInstruction> instructions,
        MethodInfo calledMethod,
        params CodeInstruction[] codeInstructions)
    {
        var code = instructions.ToList();
        var bindIndex = code.FindIndex(x => x.Calls(calledMethod));

        if (bindIndex <= 0)
        {
            //TODO: improve this error message
            Main.Error("cannot find code to replace.");
            return code;
        }

        for (var i = codeInstructions.Length - 1; i >= 0; i--)
        {
            code.Insert(bindIndex, codeInstructions[i]);
        }

        code.RemoveAt(bindIndex);

        return code;
    }

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
