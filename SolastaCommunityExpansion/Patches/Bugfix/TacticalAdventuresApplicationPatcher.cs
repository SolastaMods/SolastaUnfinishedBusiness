using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.BugFix
{
    [HarmonyPatch(typeof(TacticalAdventuresApplication), "IsModded", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class TacticalAdventuresApplication_IsModded_Getter
    {
        public static bool Prefix(ref bool __result)
        {
            __result = false;

            return false;
        }
    }
}
