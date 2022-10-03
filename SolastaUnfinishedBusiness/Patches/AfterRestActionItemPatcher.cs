using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class AfterRestActionItemPatcher
{
    [HarmonyPatch(typeof(AfterRestActionItem), "OnExecuteCb")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class OnExecuteCb_Patch
    {
        public static bool Prefix(AfterRestActionItem __instance)
        {
            //PATCH: replaces callback execution for bundled powers to show sub-power selection
            return PowersBundleContext.ExecuteAfterRestCb(__instance);
        }
    }
}
