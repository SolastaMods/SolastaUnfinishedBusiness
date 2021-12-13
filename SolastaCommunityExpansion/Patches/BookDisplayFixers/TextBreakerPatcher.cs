using HarmonyLib;
using System.Diagnostics.CodeAnalysis;

namespace SolastaCommunityExpansion.Patches.BookDisplayFixes
{
    [HarmonyPatch(typeof(TextBreaker), "BreakdownFragments")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class TextBreaker_BreakdownFragments
    {
        internal static void Prefix(ref string textLine)
        {
            textLine = textLine.Replace($"\r", "");
        }
    }
}
