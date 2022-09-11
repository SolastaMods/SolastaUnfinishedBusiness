using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

internal static class UsablePowerBoxPatcher
{
    //PATCH: used by Power Bundles feature
    [HarmonyPatch(typeof(UsablePowerBox), "OnActivateCb")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class OnActivateCb_Patch
    {
        internal static bool Prefix(UsablePowerBox __instance)
        {
            //if the activated power is a bundle, this tries to replace activation with sub-spell selector and
            //then activates bundled power according to selected subspell.
            //returns false and skips base method if it does
            return PowerBundleContext.PowerBoxActivated(__instance);
        }
    }

    //PATCH: sets current character as context for power tooltip, so it may update its properties based on user
    [HarmonyPatch(typeof(UsablePowerBox), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Bind_Patch
    {
        internal static void Postfix(UsablePowerBox __instance)
        {
            Tooltips.AddContextToPowerBoxTooltip(__instance);
        }
    }
}
