using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches;

internal static class CharacterReactionSubitemPatcher
{
    [HarmonyPatch(typeof(CharacterReactionSubitem), "Unbind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Unbind_Patch
    {
        internal static void Prefix(CharacterReactionSubitem __instance)
        {
            //PATCH: disables tooltip on Unbind.
            //Default implementation doesn't use tooltips, so we are cleaning after custom warcaster and bundled power binds

            var toggle = __instance.toggle.GetComponent<RectTransform>();
            toggle.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 34);

            var background = toggle.FindChildRecursive("Background");

            if (background == null)
            {
                return;
            }

            if (background.TryGetComponent<GuiTooltip>(out var tooltip))
            {
                tooltip.Disabled = true;
            }
        }
    }
}
