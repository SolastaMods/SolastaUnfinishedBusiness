using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.ModUi
{
    [HarmonyPatch(typeof(GameLocationBaseScreen), "HandleInput")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBaseScreen_HandleInput
    {
        internal static bool Prefix(ref bool __result)
        {
            if (ModManagerUI.IsOpen)
            {
                __result = true;
                return false;
            }

            return true;
        }
    }
}
