using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class HealthGaugeGroupPatcher
{
    [HarmonyPatch(typeof(HealthGaugeGroup), nameof(HealthGaugeGroup.Refresh))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Refresh_Patch
    {
        /// <summary>
        ///     This mods the horizontal gauge in the monster tooltip.
        ///     The gauge now shows health in steps instead of a continuous value.
        /// </summary>
        [UsedImplicitly]
        public static void Postfix(HealthGaugeGroup __instance)
        {
            //PATCH: HideMonsterHitPoints
            if (!Main.Settings.HideMonsterHitPoints)
            {
                return;
            }

            if (__instance.GuiCharacter.RulesetCharacterMonster is not { Side: RuleDefinitions.Side.Enemy })
            {
                return;
            }

            var ratio = Mathf.Clamp(
                __instance.GuiCharacter.CurrentHitPoints / (float)__instance.GuiCharacter.HitPoints, 0.0f, 1f);

            ratio = GameUiContext.GetSteppedHealthRatio(ratio);

            __instance.gaugeRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                __instance.gaugeMaxWidth * ratio);
        }
    }
}
