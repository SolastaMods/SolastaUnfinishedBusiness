using HarmonyLib;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SolastaCommunityExpansion.Patches.BookDisplayFixes
{
    [HarmonyPatch(typeof(TextBreaker), "BreakdownFragments")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class TextBreaker_BreakdownFragments
    {
        internal static bool Prefix(TextBreaker __instance, string textLine, TextFragmentStyleDescription style)
        {
            string str = textLine;
            __instance.Fragments.Clear();

            // begin patch
            str = str.Replace($"\r\n", $"\r");
            str = str.Replace($"\r", $"\n");
            // end patch

            char[] chArray = new char[1] { ' ' };
            foreach (string contentValue in str.Split(chArray))
                __instance.Fragments.Add(new TextBreaker.FragmentInfo(contentValue, style));
            for (int index = 0; index < __instance.Fragments.Count; ++index)
            {
                TextBreaker.FragmentInfo fragment1 = __instance.Fragments[index];
                int num1 = 0;
                while (fragment1.contentValue.StartsWith("\n") && fragment1.contentValue.Length > 1 && num1++ < 1000) // Environment.NewLine.Length
                {
                    fragment1.contentValue = fragment1.contentValue.Substring(fragment1.contentValue.IndexOf('\n') + 1);
                    fragment1.newLine = true;
                }
                int num2 = 0;
                while (fragment1.contentValue.EndsWith("\n") && num2++ < 1000)
                {
                    fragment1.contentValue = fragment1.contentValue.Substring(fragment1.contentValue.LastIndexOf('\n'));
                    if (index < __instance.Fragments.Count - 1)
                    {
                        TextBreaker.FragmentInfo fragment2 = __instance.Fragments[index + 1];
                        fragment2.newLine = true;
                        __instance.Fragments[index + 1] = fragment2;
                    }
                }
                int length = fragment1.contentValue.IndexOf('\n');
                if (length > 0)
                {
                    TextBreaker.FragmentInfo fragmentInfo = new TextBreaker.FragmentInfo(fragment1.contentValue.Substring(length + 1), fragment1.style);
                    fragmentInfo.newLine = true;
                    if (index < __instance.Fragments.Count - 1)
                        __instance.Fragments.Insert(index + 1, fragmentInfo);
                    else
                        __instance.Fragments.Add(fragmentInfo);
                    fragment1.contentValue = fragment1.contentValue.Substring(0, length);
                }
                __instance.Fragments[index] = fragment1;
            }

            return false;
        }
    }
}
