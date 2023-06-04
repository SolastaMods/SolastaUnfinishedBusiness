using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class GameDocumentTablePatcher
{
    private static void MakeTranspiler(List<CodeInstruction> codes, int startCodeIndex, int jumpToCodeIndex,
        int localIndex, ILGenerator il)
    {
        if (startCodeIndex == -1)
        {
            Main.Error("Cannot find the start index of the patch : GameDocumentTablePatcher");
            return;
        }

        var endCode = codes[jumpToCodeIndex];

        // find the local variable of i
        var localBuilder = codes.First(x =>
            x.opcode == OpCodes.Ldloc_S &&
            x.operand is LocalBuilder builder &&
            builder.LocalIndex == localIndex).operand as LocalBuilder;
        var endLabel = il.DefineLabel();

        endCode.labels.Add(endLabel);

        // C# Code:
        // if (!HasCJKCharQuick(this.textBreaker.Fragments[i].contentValue))
        //      num5 += num4;
        var newCodes = new List<CodeInstruction>
        {
            // this
            new(OpCodes.Ldarg_0),
            // this.textBreaker
            new(OpCodes.Ldfld, AccessTools.Field(typeof(GameDocumentTable), "textBreaker")),
            // this.textBreaker.Fragments
            new(OpCodes.Callvirt, AccessTools.Property(typeof(TextBreaker), "Fragments").GetGetMethod()),
            // i
            new(OpCodes.Ldloc_S, localBuilder),
            // this.textBreaker.Fragments[i]
            new(OpCodes.Callvirt, AccessTools.Method(typeof(List<TextBreaker.FragmentInfo>), "get_Item")),
            // this.textBreaker.Fragments[i].contentValue
            new(OpCodes.Ldfld, AccessTools.Field(typeof(TextBreaker.FragmentInfo), "contentValue")),
            // HasCJKCharQuick(this.textBreaker.Fragments[i].contentValue)
            new(OpCodes.Call,
                AccessTools.Method(typeof(TranslatorContext), nameof(TranslatorContext.HasCJKCharQuick))),
            // if (!HasCJKCharQuick(this.textBreaker.Fragments[i].contentValue))
            new(OpCodes.Brtrue_S, endLabel)
        };

        codes.InsertRange(startCodeIndex, newCodes);
    }

    //PATCH: supports FixAsianLanguagesTextWrap
    [HarmonyPatch(typeof(GameDocumentTable), MethodType.Constructor,
        typeof(DocumentTableDefinition), typeof(DocumentDescription), typeof(ITextComputer))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Constructor1_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions,
            ILGenerator il)
        {
            if (!Main.Settings.FixAsianLanguagesTextWrap)
            {
                return instructions;
            }

            var codes = instructions.ToList();
            var startIndex = -1;
            var endIndex = -1;

            for (var index = 0; index < codes.Count; index++)
            {
                var code = codes[index];

                // find num += wordSpacing;
                if (code.opcode != OpCodes.Ldloc_S ||
                    code.operand is not LocalBuilder { LocalIndex: 14 } ||
                    index >= codes.Count - 1)
                {
                    continue;
                }

                var nextCode = codes[index + 1];

                if (nextCode.opcode != OpCodes.Ldloc_S ||
                    nextCode.operand is not LocalBuilder { LocalIndex: 10 })
                {
                    continue;
                }

                startIndex = index;
                endIndex = index + 4;

                break;
            }

            MakeTranspiler(codes, startIndex, endIndex, 15, il);

            return codes;
        }
    }

    //PATCH: supports FixAsianLanguagesTextWrap
    [HarmonyPatch(typeof(GameDocumentTable), MethodType.Constructor,
        typeof(DocumentTableDefinition), typeof(string), typeof(string), typeof(ITextComputer))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Constructor2_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions,
            ILGenerator il)
        {
            if (!Main.Settings.FixAsianLanguagesTextWrap)
            {
                return instructions;
            }

            var codes = instructions.ToList();
            var startIndex = -1;
            var endIndex = -1;

            for (var index = 0; index < codes.Count; index++)
            {
                var code = codes[index];

                // find num += wordSpacing;
                if (code.opcode != OpCodes.Ldloc_S ||
                    code.operand is not LocalBuilder { LocalIndex: 5 } ||
                    index >= codes.Count - 1)
                {
                    continue;
                }

                var nextCode = codes[index + 1];

                if (nextCode.opcode != OpCodes.Ldloc_3)
                {
                    continue;
                }

                startIndex = index;
                endIndex = index + 4;

                break;
            }

            MakeTranspiler(codes, startIndex, endIndex, 6, il);

            return codes;
        }
    }
}
