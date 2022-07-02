using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.GameUi.Monsters;

/// <summary>
///     This mods the horizontal gauge in the monster tooltip.
///     The gauge now shows health in steps instead of a continuous value.
/// </summary>
[HarmonyPatch(typeof(HealthGaugeGroup), "Refresh")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class HealthGaugeGroup_Refresh
{
    internal static void Postfix(HealthGaugeGroup __instance)
    {
        if (!Main.Settings.HideMonsterHitPoints)
        {
            return;
        }

        if (__instance.GuiCharacter.RulesetCharacterMonster == null ||
            __instance.GuiCharacter.RulesetCharacterMonster.Side != RuleDefinitions.Side.Enemy)
        {
            return;
        }

        var ratio = Mathf.Clamp(
            __instance.GuiCharacter.CurrentHitPoints / (float)__instance.GuiCharacter.HitPoints, 0.0f, 1f);

        ratio = HideMonsterHitPointsContext.GetSteppedHealthRatio(ratio);

        __instance.gaugeRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
            __instance.gaugeMaxWidth * ratio);
    }
}
