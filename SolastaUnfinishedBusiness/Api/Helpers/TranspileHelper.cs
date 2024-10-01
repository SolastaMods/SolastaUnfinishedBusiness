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
        return instructions.ReplaceCodeImpl(i => i.Calls(methodInfo),
            -1, 0, patchContext, codeInstructions);
    }

    // 11 replace call
    public static IEnumerable<CodeInstruction> ReplaceCall(
        this IEnumerable<CodeInstruction> instructions,
        MethodInfo methodInfo,
        int occurrence,
        string patchContext,
        params CodeInstruction[] codeInstructions)
    {
        return instructions.ReplaceCodeImpl(i => i.Calls(methodInfo),
            occurrence, 0, patchContext, codeInstructions);
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
        return instructions.ReplaceCodeImpl(i => i.Calls(methodInfo),
            occurrence, bypass, patchContext, codeInstructions);
    }

    // 2 replace call generic code
    public static IEnumerable<CodeInstruction> ReplaceCall(
        this IEnumerable<CodeInstruction> instructions,
        string contains,
        int occurrence,
        string patchContext,
        params CodeInstruction[] codeInstructions)
    {
        return instructions.ReplaceCodeImpl(i => i.opcode == OpCodes.Callvirt &&
                                                 i.operand.ToString().Contains(contains),
            occurrence, 0, patchContext, codeInstructions);
    }

    // 4 replace call EnumerateFeaturesToBrowse
    public static IEnumerable<CodeInstruction> ReplaceEnumerateFeaturesToBrowse<T>(
        this IEnumerable<CodeInstruction> instructions,
        int occurrence,
        string patchContext,
        params CodeInstruction[] codeInstructions)
    {
        return instructions.ReplaceCodeImpl(i => i.opcode == OpCodes.Callvirt
                                                 && i.operand is MethodInfo
                                                 {
                                                     Name: nameof(RulesetActor.EnumerateFeaturesToBrowse),
                                                     IsGenericMethod: true
                                                 } method
                                                 && method.GetGenericArguments().Contains(typeof(T)),
            occurrence, 0, patchContext,
            codeInstructions);
    }

    public static IEnumerable<CodeInstruction> ReplaceEnumerateFeaturesToBrowse<T>(
        this IEnumerable<CodeInstruction> instructions,
        string patchContext,
        Action<RulesetCharacter, List<FeatureDefinition>, Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin>>
            handler) where T : class
    {
        return instructions.ReplaceEnumerateFeaturesToBrowse<T>(-1, patchContext,
            new CodeInstruction(OpCodes.Call, handler.GetMethodInfo()));
    }

    public static IEnumerable<CodeInstruction> ReplaceAdd(
        this IEnumerable<CodeInstruction> instructions,
        string match,
        int occurrence,
        string patchContext,
        params CodeInstruction[] codeInstructions)
    {
        return instructions.ReplaceCodeImpl(i =>
                i.opcode == OpCodes.Callvirt &&
                i.operand.ToString().Contains("Add") &&
                i.operand.ToString().Contains(match),
            occurrence, 0, patchContext, codeInstructions);
    }

    public static IEnumerable<CodeInstruction> ReplaceRemoveAt(
        this IEnumerable<CodeInstruction> instructions,
        int occurrence,
        string patchContext,
        params CodeInstruction[] codeInstructions)
    {
        return instructions.ReplaceCodeImpl(i =>
                i.opcode == OpCodes.Callvirt &&
                i.operand.ToString().Contains("RemoveAt"),
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
        return instructions.ReplaceCodeImpl(i => i.LoadsField(fieldInfo),
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

    private static void NoAssert(bool condition)
    {
    }

    private static void NoAssert(bool condition, string message, params object[] args)
    {
    }

    public static IEnumerable<CodeInstruction> RemoveBoolAsserts(this IEnumerable<CodeInstruction> instructions)
    {
        var code = instructions.ToList();

        var assert = typeof(Trace).GetMethod("Assert", [typeof(bool)]);
        var noAssert = new Action<bool>(NoAssert).Method;

        code.FindAll(x => x.Calls(assert)).ForEach(x => x.operand = noAssert);

        assert = typeof(Trace).GetMethod("Assert", [typeof(bool), typeof(string), typeof(object[])]);
        noAssert = new Action<bool, string, object[]>(NoAssert).Method;

        code.FindAll(x => x.Calls(assert)).ForEach(x => x.operand = noAssert);

        return code;
    }

    public static bool Calls(this CodeInstruction instruction, string method)
    {
        return (instruction.opcode == OpCodes.Callvirt || instruction.opcode == OpCodes.Call)
               && instruction.operand.ToString().Contains(method);
    }

    // ReSharper disable once ReturnTypeCanBeEnumerable.Local
    private static List<CodeInstruction> ReplaceCodeImpl(
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
}
