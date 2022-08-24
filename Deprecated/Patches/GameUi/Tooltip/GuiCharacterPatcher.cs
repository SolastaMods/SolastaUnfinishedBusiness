using System.Diagnostics.CodeAnalysis;
using System.Text;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUi.Tooltip;

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
