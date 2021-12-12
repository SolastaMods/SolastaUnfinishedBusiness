using HarmonyLib;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SolastaCommunityExpansion.Patches
{
    [HarmonyPatch(typeof(GamePartyStatusScreen), "ShowBottomPopup")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GamePartyStatusScreen_ShowBottomPopup
    {
        internal static void Postfix(string text)
        {
            if (Main.Settings.EnableAdventureLogPopups)
            {
                Models.AdventureLogContext.LogEntry(string.Empty, text);
            }
        }
    }

    [HarmonyPatch(typeof(GamePartyStatusScreen), "ShowHeaderPopup")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GamePartyStatusScreen_ShowHeaderPopup
    {
        internal static void Postfix(string text)
        {
            if (Main.Settings.EnableAdventureLogPopups)
            {
                Models.AdventureLogContext.LogEntry(string.Empty, text);
            }
        }
    }
}
