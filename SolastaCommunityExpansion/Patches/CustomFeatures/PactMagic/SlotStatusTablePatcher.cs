using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.PactMagic
{
    // creates different slots colors and pop up messages depending on slot types
    [HarmonyPatch(typeof(SlotStatusTable), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SlotStatusTable_Bind
    {
        public static void Postfix(
            RulesetSpellRepertoire spellRepertoire,
            int spellLevel,
            RectTransform ___table)
        {
            // spellRepertoire is null during level up...
            if (spellRepertoire == null || spellLevel == 0)
            {
                return;
            }

            var heroWithSpellRepertoire = SharedSpellsContext.GetHero(spellRepertoire.CharacterName);

            spellRepertoire.GetSlotsNumber(spellLevel, out var totalSlotsRemainingCount, out var totalSlotsCount);

            MulticlassGameUiContext.PaintPactSlots(
                heroWithSpellRepertoire, totalSlotsCount, totalSlotsRemainingCount, spellLevel, ___table, hasTooltip: true);
        }
    }

    // ensures slot colors are white before getting back to pool
    [HarmonyPatch(typeof(SlotStatusTable), "Unbind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SlotStatusTable_Unbind
    {
        public static void Prefix(RectTransform ___table)
        {
            MulticlassGameUiContext.PaintSlotsWhite(___table);
        }
    }
}

