using HarmonyLib;
using SolastaCommunityExpansion.Models;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.GameUiMonsters
{
    /// <summary>
    /// This mods the horizontal gauge in the monster tooltip.
    /// The gauge now shows health in steps instead of a continuous value.
    /// </summary>
    [HarmonyPatch(typeof(HealthGaugeGroup), "Refresh")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class HealthGaugeGroup_Refresh
    {
        internal static void Postfix(HealthGaugeGroup __instance, RectTransform ___gaugeRect, float ___gaugeMaxWidth)
        {
            if (!Main.Settings.HideMonsterHitPoints) 
            {
                return;
            }

            if (__instance.GuiCharacter.RulesetCharacterMonster != null) // Only change for monsters
            {
                float ratio = Mathf.Clamp(__instance.GuiCharacter.CurrentHitPoints / (float)__instance.GuiCharacter.HitPoints, 0.0f, 1f);

                ratio = HideMonsterHitPointsContext.GetSteppedHealthRatio(ratio);

                ___gaugeRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ___gaugeMaxWidth * ratio);
            }
        }
    }
}
