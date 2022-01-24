using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUi.AdventureLog
{
    [HarmonyPatch(typeof(GamePartyStatusScreen), "ShowBottomPopup")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GamePartyStatusScreen_ShowBottomPopup
    {
        internal static void Postfix(string text)
        {
            if (Main.Settings.EnableAdventureLogPopups)
            {
                Models.AdventureLogContext.LogEntry("Popup", text);
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
                Models.AdventureLogContext.LogEntry("Popup", text);
            }
        }
    }
}
