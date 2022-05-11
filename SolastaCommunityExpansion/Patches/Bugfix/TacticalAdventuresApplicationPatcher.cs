using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.BugFix
{
    [HarmonyPatch(typeof(TacticalAdventuresApplication), "IsGameModded")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class TacticalAdventuresApplication_IsGameModded
    {
        public static bool Prefix(
            ref bool __result,
            out bool ummExists,
            out int ummActiveModsCount,
            out bool solastaModApiExists)
        {
            __result = false;
            ummExists = false;
            ummActiveModsCount = 0;
            solastaModApiExists = false;

            return false;
        }
    }
}
