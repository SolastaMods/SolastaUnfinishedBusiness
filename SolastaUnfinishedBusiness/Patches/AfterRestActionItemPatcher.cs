using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

internal static class AfterRestActionItemPatcher
{
    [HarmonyPatch(typeof(AfterRestActionItem), "OnExecuteCb")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class OnExecuteCb_Patch
    {
        internal static bool Prefix(AfterRestActionItem __instance)
        {
            //PATCH: replaces callback execution for bundled powers to show sub-power selection
            return PowerBundleContext.ExecuteAfterRestCb(__instance);
        }
    }
}
