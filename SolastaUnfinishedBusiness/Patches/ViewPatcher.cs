using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches;

internal static class ViewPatcher
{
    [HarmonyPatch(typeof(View), "HandleInputs")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class HandleInputs_Patch
    {
        internal static bool Prefix()
        {
            //PATCH: prevents game from receiving input if Mod UI is open
            return !UnityModManagerUIPatcher.ModManagerUI.IsOpen;
        }
    }
}
