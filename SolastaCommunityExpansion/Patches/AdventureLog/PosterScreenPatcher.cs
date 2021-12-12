using HarmonyLib;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SolastaCommunityExpansion.Patches
{
    [HarmonyPatch(typeof(PosterScreen), "Show")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class PosterScreen_Show
    {
        internal static void Postfix(List<string> captions)
        {
            if (Main.Settings.EnableAdventureLogLore)
            {
                Models.AdventureLogContext.LogEntry("Lore", captions);
            }
        }
    }
}
