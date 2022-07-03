using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomReactions;

[HarmonyPatch(typeof(CharacterReactionSubitem), "Unbind")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterReactionSubitem_Unbind
{
    internal static void Prefix(CharacterReactionSubitem __instance)
    {
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
