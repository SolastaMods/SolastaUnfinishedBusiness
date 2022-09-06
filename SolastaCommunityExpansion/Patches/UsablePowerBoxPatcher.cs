using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.PatchCode.CustomUI;

namespace SolastaCommunityExpansion.Patches;

internal static class UsablePowerBoxPatcher
{
    [HarmonyPatch(typeof(UsablePowerBox), "OnActivateCb")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class OnActivateCb_Patch
    {
        internal static bool Prefix(UsablePowerBox __instance)
        {
            //PATCH: used by Power Bundles feature
            //If the activated power is a bundle, this tries to replace activation with sub-spell selector and
            //then activates bundled power according to selected subspell.
            //returns false and skips base method if it does
            return PowerBundleContext.PowerBoxActivated(__instance);
        }
    }

    [HarmonyPatch(typeof(UsablePowerBox), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class UsablePowerBox_Bind
    {
        internal static void Postfix(UsablePowerBox __instance)
        {
            //PATCH: sets current character as context for power tooltip, so it may update its properties based on user
            Tooltips.AddContextToPowerBoxTooltip(__instance);
        }
    }
}