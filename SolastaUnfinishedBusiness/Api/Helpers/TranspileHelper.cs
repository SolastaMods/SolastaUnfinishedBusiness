using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Api.Helpers;

internal static class TranspileHelper
{
    // 42 replace calls
    public static IEnumerable<CodeInstruction> ReplaceCalls(
        this IEnumerable<CodeInstruction> instructions,
        MethodInfo methodInfo,
        string patchContext,
        params CodeInstruction[] codeInstructions)
    {
        return instructions.ReplaceCodeImpl(i => i.Calls(methodInfo), -1, 0, patchContext, codeInstructions);
    }

    // 10 replace call
    public static IEnumerable<CodeInstruction> ReplaceCall(
        this IEnumerable<CodeInstruction> instructions,
        MethodInfo methodInfo,
        int occurrence,
        string patchContext,
        params CodeInstruction[] codeInstructions)
    {
        return instructions.ReplaceCodeImpl(i => i.Calls(methodInfo), occurrence, 0, patchContext, codeInstructions);
    }

    // 3 bypass replace call
    public static IEnumerable<CodeInstruction> ReplaceCall(
        this IEnumerable<CodeInstruction> instructions,
        MethodInfo methodInfo,
        int occurrence,
        int bypass,
        string patchContext,
        params CodeInstruction[] codeInstructions)
    {
        return instructions.ReplaceCodeImpl(i => i.Calls(methodInfo), occurrence, bypass, patchContext,
            codeInstructions);
    }

    // 2 replace call generic code
    public static IEnumerable<CodeInstruction> ReplaceCallGeneric(
        this IEnumerable<CodeInstruction> instructions,
        string contains,
        int occurrence,
        string patchContext,
        params CodeInstruction[] codeInstructions)
    {
        return instructions.ReplaceCodeImpl(
            instruction =>
                instruction.opcode == OpCodes.Callvirt && instruction.operand.ToString().Contains(contains),
            occurrence, 0, patchContext, codeInstructions);
    }

    // 4 replace call EnumerateFeaturesToBrowse
    public static IEnumerable<CodeInstruction> ReplaceEnumerateFeaturesToBrowse(
        this IEnumerable<CodeInstruction> instructions,
        string featureToBrowse,
        int occurrence,
        string patchContext,
        params CodeInstruction[] codeInstructions)
    {
        return instructions.ReplaceCodeImpl(instruction =>
                instruction.operand?.ToString().Contains("EnumerateFeaturesToBrowse") == true &&
                instruction.operand?.ToString().Contains(featureToBrowse) == true,
            occurrence, 0, patchContext, codeInstructions);
    }

    // 2 bypass replace load field
    public static IEnumerable<CodeInstruction> ReplaceLoadField(
        this IEnumerable<CodeInstruction> instructions,
        FieldInfo fieldInfo,
        int occurrence,
        int bypass,
        string patchContext,
        params CodeInstruction[] codeInstructions)
    {
        return instructions.ReplaceCodeImpl(instruction => instruction.LoadsField(fieldInfo),
            occurrence, bypass, patchContext, codeInstructions);
    }

    // 6 generic replace code
    public static IEnumerable<CodeInstruction> ReplaceCode(
        this IEnumerable<CodeInstruction> instructions,
        Predicate<CodeInstruction> match,
        int occurrence,
        string patchContext,
        params CodeInstruction[] codeInstructions)
    {
        return instructions.ReplaceCodeImpl(match, occurrence, 0, patchContext, codeInstructions);
    }

    private static IEnumerable<CodeInstruction> ReplaceCodeImpl(
        this IEnumerable<CodeInstruction> instructions,
        Predicate<CodeInstruction> match,
        int occurrence,
        int bypass,
        string patchContext,
        params CodeInstruction[] codeInstructions)
    {
        var found = 0;
        var code = instructions.ToList();
        var bindIndex = code.FindIndex(match);

        if (bindIndex <= 0)
        {
            Main.Error($"Failed to apply transpiler patch [{patchContext}]!");
        }

        while (bindIndex >= 0)
        {
            if (occurrence < 0 || ++found == occurrence)
            {
                for (var i = 0; i < codeInstructions.Length; i++)
                {
                    code.Insert(bindIndex + i, codeInstructions[i]);
                }

                for (var i = 0; i <= bypass; i++)
                {
                    code.RemoveAt(bindIndex + codeInstructions.Length);
                }

                bindIndex = code.FindIndex(bindIndex + codeInstructions.Length, match);
            }
            else
            {
                bindIndex = code.FindIndex(bindIndex + 1, match);
            }
        }

        return code;
    }

    public static IEnumerable<CodeInstruction> RemoveBoolAsserts(this IEnumerable<CodeInstruction> instructions)
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
