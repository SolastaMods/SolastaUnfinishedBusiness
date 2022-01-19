using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.ModUi
{
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
