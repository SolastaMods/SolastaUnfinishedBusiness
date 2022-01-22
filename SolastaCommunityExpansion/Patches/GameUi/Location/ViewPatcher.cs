using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUi.Location
{
    // this patch prevents game from receive input if Mod UI is open
    [HarmonyPatch(typeof(View), "HandleInputs")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class View_HandleInputs
    {
        internal static bool Prefix()
        {
            if (ModManagerUI.IsOpen)
            {
                return false;
            }

            return true;
        }
    }
}
