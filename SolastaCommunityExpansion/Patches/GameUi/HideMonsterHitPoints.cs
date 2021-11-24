using HarmonyLib;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Patches
{
    // TODO: split this with a proper model
    internal static class HideMonsterHitPoints
    {
        /// <summary>
        /// Call 'HasHealthUpdated' which returns true/false but as a side effect updates the health state and dirty flags.
        /// </summary>
        internal static bool UpdateHealthStatus(this GuiCharacter __instance)
        {
            // call badly named method
            var rb = typeof(GuiCharacter).GetMethod("HasHealthUpdated", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            bool retval = false;

            if (rb != null)
            {
                retval = (bool)rb.Invoke(__instance, null);
            }

            return retval;
        }

        /// <summary>
        /// Converts continuous ratio into series of stepped values.
        /// </summary>
        internal static float GetSteppedHealthRatio(float ratio)
        {
            // Green
            if (ratio >= 1f) return 1f;
            // Green
            if (ratio >= 0.5f) return 0.75f;
            // Orange
            if (ratio >= 0.25f) return 0.5f;
            // Red
            if (ratio > 0f) return 0.25f;
            return ratio;
        }
    }

    /// <summary>
    /// This mods the vertical gauge in the monster portrait.
    /// The gauge now shows health in steps instead of a continuous value.
    /// </summary>
    [HarmonyPatch(typeof(GuiCharacter), "FormatHealthGauge")]
    internal static class GuiCharacter_FormatHealthGauge
    {
        internal static void Prefix(GuiCharacter __instance, bool ___healthGaugeDirty, out bool __state)
        {
            if (!Main.Settings.HideMonsterHitPoints)
            {
                __state = false;
                return;
            }

            var dirty = __instance.UpdateHealthStatus();

            // capture current state of dirty flag for use in Postfix
            __state = ___healthGaugeDirty || dirty;
        }

        internal static void Postfix(GuiCharacter __instance, Image healthGauge, float parentHeight, bool __state)
        {
            if (!Main.Settings.HideMonsterHitPoints) return;

            if (!__state) return;  // health wasn't dirty so healthGauge hasn't been updated

            if (__instance.RulesetCharacterMonster != null) // only change for monsters
            {
                var ratio = Mathf.Clamp(__instance.CurrentHitPoints / (float)__instance.HitPoints, 0.0f, 1f);

                ratio = HideMonsterHitPoints.GetSteppedHealthRatio(ratio);

                healthGauge.rectTransform.offsetMax = new Vector2(healthGauge.rectTransform.offsetMax.x, (float)(-parentHeight * (1.0 - ratio)));
            }
        }
    }

    /// <summary>
    /// Mods the monster health label (current/max) hit points to hide current hit points.
    /// </summary>
    [HarmonyPatch(typeof(GuiCharacter), "FormatHealthLabel")]
    internal static class GuiCharacter_FormatHealthLabel
    {
        private static readonly Regex HitPointRegex = new Regex(@"^<#.{6}>(?<current_hp>\d{1,4})</color>/(?<max_hp>\d{1,4})", RegexOptions.Compiled | RegexOptions.Singleline);

        internal static void Prefix(GuiCharacter __instance, bool ___healthLabelDirty, out bool __state)
        {
            if (!Main.Settings.HideMonsterHitPoints)
            {
                __state = false;
                return;
            }

            bool dirty = __instance.UpdateHealthStatus();

            // capture current state of dirty flag for use in Postfix
            __state = ___healthLabelDirty || dirty;
        }

        internal static void Postfix(GuiCharacter __instance, GuiLabel healthLabel, bool __state)
        {
            if (!Main.Settings.HideMonsterHitPoints) return;

            if (!__state) return;  // health wasn't dirty so healthLabel hasn't been updated

            // A monster has __instance.RulesetCharacterMonster != null and __instance.RulesetCharacter != null
            // A hero has __instance.RulesetCharacterHero != null and __instance.RulesetCharacter != null

            if (__instance.HasHitPointsKnowledge && __instance.RulesetCharacterMonster != null)
            {
                // Our heros now have enough bestiary knowledge to display the monster hit points
                // which makes picking off damaged monsters easier that it might be.

                // We make the following changes:
                // 1) Full hit points is still displayed, e.g. 28/28
                // 2) Less than full hit points is hidden, but the number of digits is shown, so **/28 or */28.
                // Standard health colours will still be in effect.  Green (50%-100%), Orange (25%-50%), Red (0-25%).

                // Normal text formatting runs first so the healthLabel text at this point is
                // "?? / ??" (if HasHitPointsKnowledge=false), or <#xxxxxx>current_hp</color>/max_hp

                var text = healthLabel.Text;

                // extract current and max hp
                var match = HitPointRegex.Match(text);

                if (match.Success && (match.Groups["current_hp"].Value != match.Groups["max_hp"].Value))
                {
                    var hp = match.Groups["current_hp"].Value;
                    var hpLen = hp.Length;
                    var stars = new string('*', hpLen);

                    // replace with asterisks
                    healthLabel.Text = text.Replace($">{hp}<", $">{stars}<");
                }
            }
        }

        /// <summary>
        /// This mods the horizontal gauge in the monster tooltip.
        /// The gauge now shows health in steps instead of a continuous value.
        /// </summary>
        [HarmonyPatch(typeof(HealthGaugeGroup), "Refresh")]
        internal static class HealthGaugeGroup_Refresh
        {
            internal static void Postfix(HealthGaugeGroup __instance, RectTransform ___gaugeRect, float ___gaugeMaxWidth)
            {
                if (!Main.Settings.HideMonsterHitPoints) return;

                if (__instance.GuiCharacter.RulesetCharacterMonster != null) // Only change for monsters
                {
                    float ratio = Mathf.Clamp(__instance.GuiCharacter.CurrentHitPoints / (float)__instance.GuiCharacter.HitPoints, 0.0f, 1f);

                    ratio = HideMonsterHitPoints.GetSteppedHealthRatio(ratio);

                    ___gaugeRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ___gaugeMaxWidth * ratio);
                }
            }
        }
    }
}
