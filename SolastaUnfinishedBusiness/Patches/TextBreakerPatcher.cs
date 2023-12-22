using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class TextBreakerPatcher
{
    //PATCH: supports FixAsianLanguagesTextWrap
    [HarmonyPatch(typeof(TextBreaker), nameof(TextBreaker.BreakdownFragments))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class BreakdownFragments_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            TextBreaker __instance,
            string textLine,
            TextFragmentStyleDescription style,
            bool handleSprites = false)
        {
            if (!Main.Settings.FixAsianLanguagesTextWrap)
            {
                return true;
            }

            BreakdownFragments_Fixed(__instance, textLine, style, handleSprites);

            return false;
        }

        // Copy from dnSpy
        private static void BreakdownFragments_Fixed(
            TextBreaker textBreaker,
            string textLine,
            TextFragmentStyleDescription style,
            bool handleSprites = false)
        {
            textBreaker.fragments.Clear();

            if (handleSprites)
            {
                textLine = textLine.Replace("sprite index", "sprite_index");
            }

            // BEGIN CHANGE
            var fragments = SplitText(textLine);
            // END CHANGE

            foreach (var fragmentInfo in fragments
                         .Select(text => new TextBreaker.FragmentInfo(
                             handleSprites ? text.Replace("sprite_index", "sprite index") : text, style)))
            {
                textBreaker.fragments.Add(fragmentInfo);
            }

            for (var j = 0; j < textBreaker.fragments.Count; j++)
            {
                var fragmentInfo2 = textBreaker.fragments[j];
                var num = 0;

                while (fragmentInfo2.contentValue.StartsWith("\n") &&
                       fragmentInfo2.contentValue.Length > Environment.NewLine.Length &&
                       num++ < 1000)
                {
                    fragmentInfo2.contentValue =
                        fragmentInfo2.contentValue.Substring(fragmentInfo2.contentValue.IndexOf('\n') + 1);
                    fragmentInfo2.newLine = true;
                }

                num = 0;

                while (fragmentInfo2.contentValue.EndsWith("\n") &&
                       num++ < 1000)
                {
                    fragmentInfo2.contentValue =
                        fragmentInfo2.contentValue.Substring(fragmentInfo2.contentValue.LastIndexOf('\n'));

                    if (j >= textBreaker.fragments.Count - 1)
                    {
                        continue;
                    }

                    var fragmentInfo3 = textBreaker.fragments[j + 1];

                    fragmentInfo3.newLine = true;
                    textBreaker.fragments[j + 1] = fragmentInfo3;
                }

                var num2 = fragmentInfo2.contentValue.IndexOf('\n');

                if (num2 > 0)
                {
                    var fragmentInfo4 =
                        new TextBreaker.FragmentInfo(fragmentInfo2.contentValue.Substring(num2 + 1),
                            fragmentInfo2.style) { newLine = true };

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

        // ReSharper disable once ReturnTypeCanBeEnumerable.Local
        private static List<string> SplitText(string textLine)
        {
            var texts = textLine.Split(' ');
            var fragments = new List<string>();
            var sb = new StringBuilder();

            foreach (var s in texts)
            {
                if (TranslatorContext.HasCJKChar(s))
                {
                    foreach (var c in s)
                    {
                        if (TranslatorContext.IsCJKChar(c))
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

                    if (sb.Length <= 0)
                    {
                        continue;
                    }

                    fragments.Add(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    fragments.Add(s);
                }
            }

            return fragments;
        }
    }

    //PATCH: supports FixAsianLanguagesTextWrap
    [HarmonyPatch(typeof(TextBreaker), nameof(TextBreaker.DispatchFragments))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class DispatchFragments_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            TextBreaker __instance,
            float areaWidth,
            float indentWidth,
            float lineHeight,
            float paragraphSpacing,
            float wordSpacing,
            bool indent,
            float currentY,
            ref float __result)
        {
            if (!Main.Settings.FixAsianLanguagesTextWrap)
            {
                return true;
            }

            __result = DispatchFragments_Fixed(
                __instance, areaWidth, indentWidth, lineHeight, paragraphSpacing, wordSpacing, indent, currentY);

            return false;
        }

        // Copy from dnSpy
        private static float DispatchFragments_Fixed(
            TextBreaker textBreaker,
            float areaWidth,
            float indentWidth,
            float lineHeight,
            float paragraphSpacing,
            float wordSpacing,
            bool indent,
            float currentY)
        {
            var num = currentY;
            var num2 = indent ? indentWidth : 0f;

            for (var i = 0; i < textBreaker.fragments.Count; i++)
            {
                var fragmentInfo = textBreaker.fragments[i];

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
                    // BEGIN CHANGE
                    if (!TranslatorContext.HasCJKCharQuick(textBreaker.fragments[i].contentValue))
                    {
                        num2 += wordSpacing;
                    }
                    // END CHANGE

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
}
