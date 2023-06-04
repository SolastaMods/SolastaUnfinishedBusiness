using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using HarmonyLib;
using JetBrains.Annotations;
using Mono.Cecil.Cil;
using UnityEngine;
using OpCodes = System.Reflection.Emit.OpCodes;

namespace SolastaUnfinishedBusiness.Patches;

/// <summary>
/// Fixed incorrect line breaks in CJK language
/// </summary>
[UsedImplicitly]
public static class TextBreakerPatcher
{
    public static Regex RegexHasCJK = new Regex(@"\p{IsCJKUnifiedIdeographs}", RegexOptions.Compiled);
    
    public static bool IsCJKChar(char c)
    {
        return c >= 0x4E00 && c <= 0x9FA5;
    }
    
    public static bool HasCJKChar(string s)
    {
        return s.Length > 0 && RegexHasCJK.IsMatch(s);
    }
    
    public static bool HasCJKCharQuick(string s)
    {
        return s.Length > 0 && IsCJKChar(s[0]);
    }

    [UsedImplicitly]
    [HarmonyPatch(typeof(TextBreaker), nameof(TextBreaker.BreakdownFragments))]
    public static class BreakdownFragments_Patch
    {
        [UsedImplicitly]
        [HarmonyPrefix]
        public static bool Prefix(TextBreaker __instance, string textLine,
            TextFragmentStyleDescription style, bool handleSprites = false)
        {
            if (!Main.Settings.FixCJKWrappingError)
            {
                return true;
            }
            
            BreakdownFragments_Fixed(__instance, textLine, style, handleSprites);
            return false;
        }

        // Copy from dnSpy
        public static void BreakdownFragments_Fixed(TextBreaker textBreaker, string textLine,
            TextFragmentStyleDescription style, bool handleSprites = false)
        {
            textBreaker.fragments.Clear();
            if (handleSprites)
            {
                textLine = textLine.Replace("sprite index", "sprite_index");
            }

            // Changed here
            var fragments = SplitText(textLine);
            // End of change

            foreach (string text in fragments)
            {
                TextBreaker.FragmentInfo fragmentInfo = new TextBreaker.FragmentInfo(
                    handleSprites ? text.Replace("sprite_index", "sprite index") : text, style, null, null);
                textBreaker.fragments.Add(fragmentInfo);
            }

            for (int j = 0; j < textBreaker.fragments.Count; j++)
            {
                TextBreaker.FragmentInfo fragmentInfo2 = textBreaker.fragments[j];
                int num = 0;
                while (fragmentInfo2.contentValue.StartsWith("\n") &&
                       fragmentInfo2.contentValue.Length > Environment.NewLine.Length && num++ < 1000)
                {
                    fragmentInfo2.contentValue =
                        fragmentInfo2.contentValue.Substring(fragmentInfo2.contentValue.IndexOf('\n') + 1);
                    fragmentInfo2.newLine = true;
                }

                num = 0;
                while (fragmentInfo2.contentValue.EndsWith("\n") && num++ < 1000)
                {
                    fragmentInfo2.contentValue =
                        fragmentInfo2.contentValue.Substring(fragmentInfo2.contentValue.LastIndexOf('\n'));
                    if (j < textBreaker.fragments.Count - 1)
                    {
                        TextBreaker.FragmentInfo fragmentInfo3 = textBreaker.fragments[j + 1];
                        fragmentInfo3.newLine = true;
                        textBreaker.fragments[j + 1] = fragmentInfo3;
                    }
                }

                int num2 = fragmentInfo2.contentValue.IndexOf('\n');
                if (num2 > 0)
                {
                    TextBreaker.FragmentInfo fragmentInfo4 =
                        new TextBreaker.FragmentInfo(fragmentInfo2.contentValue.Substring(num2 + 1),
                            fragmentInfo2.style, null, null);
                    fragmentInfo4.newLine = true;
                    if (j < textBreaker.fragments.Count - 1)
                    {
                        textBreaker.fragments.Insert(j + 1, fragmentInfo4);
                    }
                    else
                    {
                        textBreaker.fragments.Add(fragmentInfo4);
                    }

                    fragmentInfo2.contentValue = fragmentInfo2.contentValue.Substring(0, num2);
                }

                textBreaker.fragments[j] = fragmentInfo2;
            }
        }

        public static List<string> SplitText(string textLine)
        {
            var texts = textLine.Split(new char[] { ' ' });
            var fragments = new List<string>();
            var sb = new StringBuilder();

            foreach (var s in texts)
            {
                if (HasCJKChar(s))
                {
                    foreach (var c in s)
                    {
                        if (IsCJKChar(c))
                        {
                            fragments.Add(sb.ToString());
                            sb.Clear();
                            sb.Append(c);
                        }
                        else
                        {
                            sb.Append(c);
                        }
                    }

                    if (sb.Length > 0)
                    {
                        fragments.Add(sb.ToString());
                        sb.Clear();
                    }
                }
                else
                {
                    fragments.Add(s);
                }
            }

            return fragments;
        }
    }

    [UsedImplicitly]
    [HarmonyPatch(typeof(TextBreaker), nameof(TextBreaker.DispatchFragments))]
    public static class DispatchFragments_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(TextBreaker __instance, float areaWidth, float indentWidth, float lineHeight,
            float paragraphSpacing, float wordSpacing, bool indent, float currentY, ref float __result)
        {
            if (!Main.Settings.FixCJKWrappingError)
            {
                return true;
            }
            
            __result = DispatchFragments_Fixed(__instance, areaWidth, indentWidth, lineHeight, paragraphSpacing,
                wordSpacing,
                indent, currentY);
            return false;
        }


        // Copy from dnSpy
        public static float DispatchFragments_Fixed(TextBreaker textBreaker, float areaWidth, float indentWidth,
            float lineHeight, float paragraphSpacing, float wordSpacing, bool indent, float currentY)
        {
            float num = currentY;
            float num2 = (indent ? indentWidth : 0f);
            for (int i = 0; i < textBreaker.fragments.Count; i++)
            {
                TextBreaker.FragmentInfo fragmentInfo = textBreaker.fragments[i];
                if (fragmentInfo.newLine)
                {
                    num2 = 0f;
                    currentY += paragraphSpacing + lineHeight;
                }

                if (num2 + fragmentInfo.width > areaWidth)
                {
                    fragmentInfo.x = 0f;
                    num2 = fragmentInfo.width;
                    currentY += lineHeight;
                }
                else
                {
                    fragmentInfo.x = num2;
                    num2 += fragmentInfo.width;
                }

                fragmentInfo.y = -currentY;
                if (i < textBreaker.fragments.Count - 1 &&
                    textBreaker.fragments[i + 1].contentValue.IndexOf(',') != 0 &&
                    fragmentInfo.contentValue.IndexOf('+') != fragmentInfo.contentValue.Length - 1)
                {
                    // Changed here
                    if (!HasCJKCharQuick(textBreaker.fragments[i].contentValue))
                        num2 += wordSpacing;
                    // End of change

                    if (num2 > areaWidth)
                    {
                        num2 = 0f;
                        currentY += lineHeight;
                    }
                }

                textBreaker.fragments[i] = fragmentInfo;
            }

            currentY += lineHeight;
            return currentY - num;
        }
    }

    [UsedImplicitly]
    public static class GameDocumentTablePatcher
    {
        [HarmonyPatch(typeof(GameDocumentTable), ".ctor", MethodType.Constructor)]
        [UsedImplicitly]
        public static class Constructor_Patch
        {
            [HarmonyPatch(MethodType.Constructor)]
            [HarmonyPatch(new Type[] {typeof(DocumentTableDefinition), typeof(DocumentDescription), typeof(ITextComputer)})]
            [HarmonyTranspiler]
            [UsedImplicitly]
            [HarmonyDebug]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                if (!Main.Settings.FixCJKWrappingError)
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
                    if(code.opcode == OpCodes.Ldloc_S && code.operand is LocalBuilder { LocalIndex: 14 } && index < codes.Count - 1)
                    {
                        var nextCode = codes[index + 1];
                        if(nextCode.opcode == OpCodes.Ldloc_S && nextCode.operand is LocalBuilder { LocalIndex: 10 })
                        {
                            startIndex = index;
                            endIndex = index + 4;
                            break;
                        }
                    }
                }

                MakeTranspiler(codes, startIndex, endIndex, 15, il);

                return codes;
            }
            
            [HarmonyPatch(new Type[] {typeof(DocumentTableDefinition), typeof(string), typeof(string), typeof(ITextComputer)})]
            [HarmonyTranspiler]
            [UsedImplicitly]
            [HarmonyDebug]
            public static IEnumerable<CodeInstruction> Transpiler2(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                if (!Main.Settings.FixCJKWrappingError)
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
                    if(code.opcode == OpCodes.Ldloc_S && code.operand is LocalBuilder { LocalIndex: 5 } && index < codes.Count - 1)
                    {
                        var nextCode = codes[index + 1];
                        if(nextCode.opcode == OpCodes.Ldloc_3)
                        {
                            startIndex = index;
                            endIndex = index + 4;
                            break;
                        }
                    }
                }

                MakeTranspiler(codes, startIndex, endIndex, 6, il);

                return codes;
            }

            private static void MakeTranspiler(List<CodeInstruction> codes, int startCodeIndex, int jumpToCodeIndex, int localIndex, ILGenerator il)
            {
                if (startCodeIndex == -1)
                {
                    Main.Error("Cannot find the start index of the patch : GameDocumentTablePatcher");
                    return;
                }

                var endCode = codes[jumpToCodeIndex];
                // find the local variable of i
                var localBuilder = codes.First(x => x.opcode == OpCodes.Ldloc_S && x.operand is LocalBuilder builder 
                    && builder.LocalIndex == localIndex).operand as LocalBuilder;
                
                var endLabel = il.DefineLabel();
                endCode.labels.Add(endLabel);

                // C# Code:
                // if (!HasCJKCharQuick(this.textBreaker.Fragments[i].contentValue))
                //      num5 += num4;
                var newCodes = new List<CodeInstruction>()
                {
                    // this
                    new CodeInstruction(OpCodes.Ldarg_0),
                    // this.textBreaker
                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(GameDocumentTable), "textBreaker")),
                    // this.textBreaker.Fragments
                    new CodeInstruction(OpCodes.Callvirt, AccessTools.Property(typeof(TextBreaker), "Fragments").GetGetMethod()),
                    // i
                    new CodeInstruction(OpCodes.Ldloc_S, localBuilder),
                    // this.textBreaker.Fragments[i]
                    new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(List<TextBreaker.FragmentInfo>), "get_Item")),
                    // this.textBreaker.Fragments[i].contentValue
                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(TextBreaker.FragmentInfo), "contentValue")),
                    // HasCJKCharQuick(this.textBreaker.Fragments[i].contentValue)
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TextBreakerPatcher), nameof(HasCJKCharQuick))),
                    // if (!HasCJKCharQuick(this.textBreaker.Fragments[i].contentValue))
                    new CodeInstruction(OpCodes.Brtrue_S, endLabel),
                };

                codes.InsertRange(startCodeIndex, newCodes);
            }
        }
    }
}
