using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Api.Helpers;

internal static class TranspileHelper
{
    public static IEnumerable<CodeInstruction> ReplaceCall(
        this IEnumerable<CodeInstruction> instructions,
        MethodInfo methodInfo,
        params CodeInstruction[] codeInstructions)
    {
        var code = instructions.ToList();
        var bindIndex = code.FindIndex(x => x.Calls(methodInfo));

        if (bindIndex <= 0)
        {
            throw new Exception();
        }

        for (var i = 0; i < codeInstructions.Length; i++)
        {
            code.Insert(bindIndex + i, codeInstructions[i]);
        }

        code.RemoveAt(bindIndex + codeInstructions.Length);

        return code;
    }

    internal static IEnumerable<CodeInstruction> RemoveBoolAsserts(this IEnumerable<CodeInstruction> instructions)
    {
        int assertIndex;
        var noAssert = new Action<bool>(NoAssert).Method;
        var code = instructions.ToList();

        while ((assertIndex = GetNextAssert(code, typeof(bool))) >= 0)
        {
            code[assertIndex] = new CodeInstruction(OpCodes.Call, noAssert);
        }

        return code;
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
