using HarmonyLib;
using SolastaMulticlass.Models;
using UnityEngine;

namespace SolastaMulticlass.Patches.SlotsColors
{
    internal static class SlotStatusTablePatcher
    {
        // creates different slots colors and pop up messages depending on slot types
        [HarmonyPatch(typeof(SlotStatusTable), "Bind")]
        internal static class SlotStatusTableBind
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

                if (!SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire))
                {
                    return;
                }

                spellRepertoire.GetSlotsNumber(spellLevel, out var totalSlotsRemainingCount, out var totalSlotsCount);

                MulticlassGameUiContext.PaintSlotsLightOrDarkGreen(
                    heroWithSpellRepertoire, totalSlotsCount, totalSlotsRemainingCount, spellLevel, ___table);
            }
        }

        // ensures slot colors are white before getting back to pool
        [HarmonyPatch(typeof(SlotStatusTable), "Unbind")]
        internal static class SlotStatusTableUnbind
        {
            public static void Prefix(RectTransform ___table)
            {
                MulticlassGameUiContext.PaintSlotsWhite(___table);
            }
        }
    }
}
