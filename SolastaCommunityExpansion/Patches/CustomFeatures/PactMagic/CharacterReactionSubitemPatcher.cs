using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.PactMagic
{

    // creates different slots colors and pop up messages depending on slot types
    [HarmonyPatch(typeof(CharacterReactionSubitem), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterReactionSubitem_Bind
    {
        public static void Postfix(
            RulesetSpellRepertoire spellRepertoire,
            int slotLevel,
            RectTransform ___slotStatusTable,
            CanvasGroup ___canvasGroup)
        {
            var heroWithSpellRepertoire = SharedSpellsContext.GetHero(spellRepertoire.CharacterName);

            spellRepertoire.GetSlotsNumber(slotLevel, out var totalSlotsRemainingCount, out var totalSlotsCount);

            MulticlassGameUiContext.PaintPactSlots(
                heroWithSpellRepertoire, totalSlotsCount, totalSlotsRemainingCount, slotLevel, ___slotStatusTable);

            var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);

            if (!SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire) && warlockSpellLevel != slotLevel)
            {
                ___canvasGroup.gameObject.SetActive(false);
            }
        }
    }

    // ensures slot colors are white before getting back to pool
    [HarmonyPatch(typeof(CharacterReactionSubitem), "Unbind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterReactionSubitem_Unbind
    {
        public static void Prefix(RectTransform ___slotStatusTable)
        {
            MulticlassGameUiContext.PaintSlotsWhite(___slotStatusTable);
        }
    }
}
