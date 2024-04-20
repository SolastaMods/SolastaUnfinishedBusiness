using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using UnityEngine.UI;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class UsablePowerBoxPatcher
{
    [HarmonyPatch(typeof(UsablePowerBox), nameof(UsablePowerBox.OnActivateCb))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnActivateCb_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(UsablePowerBox __instance)
        {
            //PATCH: used by Power Bundles feature
            //if the activated power is a bundle, this tries to replace activation with sub-spell selector and
            //then activates bundled power according to selected subspell.
            //returns false and skips base method if it does
            return PowerBundle.PowerBoxActivated(__instance);
        }
    }

    [HarmonyPatch(typeof(UsablePowerBox), nameof(UsablePowerBox.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(UsablePowerBox __instance)
        {
            //PATCH: sets current character as context for power tooltip, so it may update its properties based on user
            __instance.GuiTooltip.Context = __instance.activator;

            //PATCH: make reaction powers not active
            if (__instance.usablePower.PowerDefinition.activationTime == ActivationTime.Reaction)
            {
                __instance.canvasGroup.interactable = false;
                __instance.titleActiveLabel.gameObject.SetActive(false);
                __instance.titleInactiveLabel.gameObject.SetActive(true);
            }
            else
            {
                __instance.canvasGroup.interactable = true;
            }


            //PATCH: make power icons fit into box, instead of stretching
            var img = __instance.image;
            var aspect = img.GetComponent<AspectRatioFitter>();

            if (!aspect || !img || !img.sprite)
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
