using HarmonyLib;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SolastaCommunityExpansion.Patches
{
    [HarmonyPatch(typeof(GameLocationBanterManager), "ForceBanterLine")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBanterManager_ForceBanterLine
    {
        internal static void Postfix(string line, GameLocationCharacter speaker)
        {
            if (Main.Settings.EnableAdventureLogBanterLines)
            {
                Models.AdventureLogContext.LogEntry("Conversation", new List<string> { line }, speaker.Name);
            }
        }
    }
}
