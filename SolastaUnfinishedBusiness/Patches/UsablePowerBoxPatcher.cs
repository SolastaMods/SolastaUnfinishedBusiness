using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.Patches;

internal static class UsablePowerBoxPatcher
{
    [HarmonyPatch(typeof(UsablePowerBox), "OnActivateCb")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class OnActivateCb_Patch
    {
        internal static bool Prefix(UsablePowerBox __instance)
        {
            //PATCH: used by Power Bundles feature
            //if the activated power is a bundle, this tries to replace activation with sub-spell selector and
            //then activates bundled power according to selected subspell.
            //returns false and skips base method if it does
            return PowersBundleContext.PowerBoxActivated(__instance);
        }
    }

    [HarmonyPatch(typeof(UsablePowerBox), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Bind_Patch
    {
        internal static void Postfix(UsablePowerBox __instance)
        {
            //PATCH: sets current character as context for power tooltip, so it may update its properties based on user
            Tooltips.AddContextToPowerBoxTooltip(__instance);

            //PATCH: make power icons fit into box, instead of stretching
            var img = __instance.image;
            var aspect = img.GetComponent<AspectRatioFitter>();
            if (aspect == null)
            {
                return;
            }
            var rect = img.sprite.rect;
            //Set aspect ratio to natural for the sprite, to remove stretching
            aspect.aspectRatio = rect.width / rect.height;
            //Set mode that would fill parent
            aspect.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        }
    }
}
