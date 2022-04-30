using HarmonyLib;
using SolastaMulticlass.Models;
using UnityEngine;

namespace SolastaMulticlass.Patches.SlotsColors
{
    internal static class CharacterReactionSubitemPatcher
    {
        // creates different slots colors and pop up messages depending on slot types
        [HarmonyPatch(typeof(CharacterReactionSubitem), "Bind")]
        internal static class CharacterReactionSubitemBind
        {
            public static void Postfix(
                RulesetSpellRepertoire spellRepertoire,
                int slotLevel,
                RectTransform ___slotStatusTable)
            {
                var heroWithSpellRepertoire = SharedSpellsContext.GetHero(spellRepertoire.CharacterName);

                if (!SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire))
                {
                    return;
                }

                spellRepertoire.GetSlotsNumber(slotLevel, out var totalSlotsRemainingCount, out var totalSlotsCount);

                MulticlassGameUiContext.PaintSlotsLightOrDarkGreen(
                    heroWithSpellRepertoire, totalSlotsCount, totalSlotsRemainingCount, slotLevel, ___slotStatusTable);
            }
        }

        // ensures slot colors are white before getting back to pool
        [HarmonyPatch(typeof(CharacterReactionSubitem), "Unbind")]
        internal static class CharacterReactionSubitemUnbind
        {
            public static void Prefix(RectTransform ___slotStatusTable)
            {
                MulticlassGameUiContext.PaintSlotsWhite(___slotStatusTable);
            }
        }
    }
}
