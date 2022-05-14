using HarmonyLib;
using SolastaModApi.Infrastructure;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomReactions
{
    internal static class CharacterReactionSubitemPatcher
    {
        [HarmonyPatch(typeof(CharacterReactionSubitem), "Unbind")]
        internal static class CharacterReactionSubitem_Unbind
        {
            internal static void Prefix(CharacterReactionSubitem __instance)
            {
                var toggle = __instance.GetField<Toggle>("toggle").GetComponent<RectTransform>();
                toggle.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 34);
                
                var background = toggle.FindChildRecursive("Background");
                if (background != null)
                {
                    if (background.TryGetComponent<GuiTooltip>(out var tooltip))
                    {
                        tooltip.Disabled = true;
                    }
                }
            }
        }
    }
}
