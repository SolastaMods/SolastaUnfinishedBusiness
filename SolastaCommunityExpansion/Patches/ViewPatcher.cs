using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches;

internal static class ViewPatcher
{
    //PATCH: this patch prevents game from receive input if Mod UI is open
    [HarmonyPatch(typeof(View), "HandleInputs")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class HandleInputs_Patch
    {
        internal static bool Prefix()
        {
            return !UnityModManagerUIPatcher.ModManagerUI.IsOpen;
        }
    }
}
