using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Patches;

//PATCH: EnableEnhancedCharacterInspection
[HarmonyPatch(typeof(GuiCharacter), "MainClassDefinition", MethodType.Getter)]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GuiCharacter_MainClassDefinition_Getter
{
    internal static void Postfix(ref CharacterClassDefinition __result)
    {
        if (!Main.Settings.EnableEnhancedCharacterInspection)
        {
            return;
        }

        // NOTE: don't use SelectedClass??. which bypasses Unity object lifetime check
        var selectedClass = InspectionPanelContext.SelectedClass;

        if (selectedClass)
        {
            __result = selectedClass;
        }
    }
}

//PATCH: Changes Game UI to support Multiclass
[HarmonyPatch(typeof(GuiCharacter), "LevelAndClassAndSubclass", MethodType.Getter)]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GuiCharacter_LevelAndClassAndSubclass_Getter
{
    internal static void Postfix(GuiCharacter __instance, ref string __result)
    {
        __result = MulticlassGameUiContext.GetAllClassesLabel(__instance, ' ') ?? __result;
    }
}

//PATCH: Changes Game UI to support Multiclass
[HarmonyPatch(typeof(GuiCharacter), "ClassAndLevel", MethodType.Getter)]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GuiCharacter_ClassAndLevel_Getter
{
    internal static void Postfix(GuiCharacter __instance, ref string __result)
    {
        __result = MulticlassGameUiContext.GetAllClassesLabel(__instance, ' ') ?? __result;
    }
}

//PATCH: Changes Game UI to support Multiclass
[HarmonyPatch(typeof(GuiCharacter), "LevelAndExperienceTooltip", MethodType.Getter)]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GuiCharacter_LevelAndExperienceTooltip_Getter
{
    internal static void Postfix(GuiCharacter __instance, ref string __result)
    {
        __result = MulticlassGameUiContext.GetLevelAndExperienceTooltip(__instance) ?? __result;
    }
}

//PATCH: Enables additional background display on inspection panel
[HarmonyPatch(typeof(GuiCharacter), "BackgroundDescription", MethodType.Getter)]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GuiCharacter_BackgroundDescription_Getter
{
    internal static void Postfix(GuiCharacter __instance, ref string __result)
    {
        if (!Main.Settings.EnableAdditionalBackstoryDisplay)
        {
            return;
        }

        var hero = __instance.RulesetCharacterHero;

        if (hero == null)
        {
            return;
        }

        var additionalBackstory = hero.AdditionalBackstory;

        if (string.IsNullOrEmpty(additionalBackstory))
        {
            return;
        }

        var builder = new StringBuilder();

        builder.Append(__result);
        builder.Append("\n\n<B>");
        builder.Append(Gui.Format("Stage/&IdentityAdditionalBackstoryHeader"));
        builder.Append("</B>\n\n");
        builder.Append(additionalBackstory);

        __result = builder.ToString();
    }
}

//PATCH: HideMonsterHitPoints
[HarmonyPatch(typeof(GuiCharacter), "FormatHealthGauge")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GuiCharacter_FormatHealthGauge
{
    /// <summary>
    ///     This mods the vertical gauge in the monster portrait.
    ///     The gauge now shows health in steps instead of a continuous value.
    /// </summary>
    internal static void Prefix(GuiCharacter __instance, out bool __state)
    {
        if (!Main.Settings.HideMonsterHitPoints)
        {
            __state = false;
            return;
        }

        var dirty = __instance.UpdateHealthStatus();

        // capture current state of dirty flag for use in Postfix
        __state = __instance.healthGaugeDirty || dirty;
    }

    internal static void Postfix(GuiCharacter __instance, Image healthGauge, float parentHeight, bool __state)
    {
        if (!Main.Settings.HideMonsterHitPoints)
        {
            return;
        }

        if (!__state)
        {
            return; // health wasn't dirty so healthGauge hasn't been updated
        }

        if (__instance.RulesetCharacterMonster is not { Side: RuleDefinitions.Side.Enemy })
        {
            return;
        }

        var ratio = Mathf.Clamp(__instance.CurrentHitPoints / (float)__instance.HitPoints, 0.0f, 1f);

        ratio = HideMonsterHitPointsContext.GetSteppedHealthRatio(ratio);

        healthGauge.rectTransform.offsetMax = new Vector2(healthGauge.rectTransform.offsetMax.x,
            (float)(-parentHeight * (1.0 - ratio)));
    }
}

//PATCH: HideMonsterHitPoints
[HarmonyPatch(typeof(GuiCharacter), "FormatHealthLabel")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GuiCharacter_FormatHealthLabel
{
    private static readonly Regex HitPointRegex = new(@"^<#.{6}>(?<current_hp>\d{1,4})</color>/(?<max_hp>\d{1,4})",
        RegexOptions.Compiled | RegexOptions.Singleline);

    /// <summary>
    ///     Mods the monster health label (current/max) hit points to hide current hit points.
    /// </summary>
    internal static void Prefix(GuiCharacter __instance, out bool __state)
    {
        if (!Main.Settings.HideMonsterHitPoints)
        {
            __state = false;
            return;
        }

        var dirty = __instance.UpdateHealthStatus();

        // capture current state of dirty flag for use in Postfix
        __state = __instance.healthLabelDirty || dirty;
    }

    internal static void Postfix(GuiCharacter __instance, GuiLabel healthLabel, bool __state)
    {
        if (!Main.Settings.HideMonsterHitPoints)
        {
            return;
        }

        if (!__state)
        {
            return; // health wasn't dirty so healthLabel hasn't been updated
        }

        // A monster has __instance.RulesetCharacterMonster != null and __instance.RulesetCharacter != null
        // A hero has __instance.RulesetCharacterHero != null and __instance.RulesetCharacter != null
        // A hero with wildshape has __instance.RulesetCharacterMonster != null, __instance.RulesetCharacter != null and __instance.RulesetCharacter.IsSubstitute == true

        if (__instance.HasHitPointsKnowledge && IsMonster())
        {
            // Our heros now have enough bestiary knowledge to display the monster's hit points
            // which makes picking off damaged monsters easier than it might be.

            // Make the following changes:
            // 1) Full hit points are still displayed, e.g. 28/28
            // 2) Less than full hit points are hidden, but the number of digits is shown, so **/28 or */28.
            // Standard health colours will still be in effect.  Green (50%-100%), Orange (25%-50%), Red (0-25%).

            // Normal text formatting runs before the patch so the healthLabel text at this point is
            // "?? / ??" (if HasHitPointsKnowledge=false), or <#xxxxxx>current_hp</color>/max_hp

            var text = healthLabel.Text;

            // extract current and max hp
            var match = HitPointRegex.Match(text);

            if (match.Success && match.Groups["current_hp"].Value != match.Groups["max_hp"].Value)
            {
                var hp = match.Groups["current_hp"].Value;
                var hpLen = hp.Length;
                var stars = new string('*', hpLen);

                // replace with asterisks
                healthLabel.Text = text.Replace($">{hp}<", $">{stars}<");
            }
        }

        bool IsMonster()
        {
            if (__instance.RulesetCharacterMonster == null)
            {
                // definitely not a monster
                return false;
            }

            if (__instance.RulesetCharacter.IsSubstitute)
            {
                // It's a hero wildshaping (probably).
                return false;
            }

            return __instance.RulesetCharacter.Side == RuleDefinitions.Side.Enemy;
        }
    }
}

[HarmonyPatch(typeof(CharacterPlateGame), "OnPointerEnter")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterPlateGame_OnPointerEnter
{
    internal static void Prefix(CharacterPlateGame __instance)
    {
        var hero = __instance.GuiCharacter?.RulesetCharacterHero;
        var tooltip = __instance.GuiTooltip;

        if (tooltip == null)
        {
            return;
        }

        if (hero == null || !Main.Settings.EnableStatsOnHeroTooltip)
        {
            tooltip.TooltipClass = "HeroDescription";

            return;
        }

        var sb = new StringBuilder();
        var totalAttacks = (float)hero.successfulAttacks + hero.failedAttacks;
        var hitAccuracy = totalAttacks > 0 ? hero.successfulAttacks / totalAttacks : 0;
        var feet = hero.travelledCells / 5f;
        var meter = feet * 0.3048;

        sb.AppendLine(hero.Name);
        sb.AppendLine();
        sb.AppendLine($"<b>{Gui.Localize("Modal/&StatHitAccuracyTitle")}</b> {hitAccuracy:P2}");
        sb.AppendLine($"<b>{Gui.Localize("Modal/&StatCriticalHitsTitle")}</b> {hero.criticalHits:N0}");
        sb.AppendLine($"<b>{Gui.Localize("Modal/&StatCriticalFailuresTitle")}</b> {hero.criticalFailures:N0}");
        sb.AppendLine($"<b>{Gui.Localize("Modal/&StatInflictedDamageTitle")}</b> {hero.inflictedDamage:N0}");
        sb.AppendLine($"<b>{Gui.Localize("Modal/&StatSlainEnemiesTitle")}</b> {hero.slainEnemies:N0}");
        sb.AppendLine($"<b>{Gui.Localize("Modal/&StatSustainedInjuriesTitle")}</b> {hero.sustainedInjuries:N0}");
        sb.AppendLine($"<b>{Gui.Localize("Modal/&StatRestoredHealthTitle")}</b> {hero.restoredHealth:N0}");
        sb.AppendLine($"<b>{Gui.Localize("Modal/&StatUsedMagicAndPowersTitle")}</b> {hero.usedMagicAndPowers:N0}");
        sb.AppendLine($"<b>{Gui.Localize("Modal/&StatKnockOutsTitle")}</b> {hero.knockOuts:N0}");
        sb.AppendLine($"<b>{Gui.Localize("Modal/&StatTravelledCellsTitle")}</b> {feet:N0}ft / {meter:N0}m");

        tooltip.Content = sb.ToString();
        tooltip.TooltipClass = string.Empty;
    }
}
