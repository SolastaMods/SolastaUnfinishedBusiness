using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class HeroStatsColumnPatcher
{
    [HarmonyPatch(typeof(HeroStatsColumn), nameof(HeroStatsColumn.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        //PATCH: allow heroes not in pool (RESPEC ed) to be exported
        [UsedImplicitly]
        public static void Postfix(HeroStatsColumn __instance, RectTransform tooltipAnchor)
        {
            if (__instance.BuiltIn ||
                __instance.updateFileButton.interactable)
            {
                return;
            }

            __instance.updateFileButton.gameObject.SetActive(true);
            __instance.updateFileButtonTooltip.Anchor = tooltipAnchor;
            __instance.updateFileButtonTooltip.AnchorMode = TooltipDefinitions.AnchorMode.LEFT_FREE;
            __instance.updateFileButton.interactable = true;
        }
    }
}
