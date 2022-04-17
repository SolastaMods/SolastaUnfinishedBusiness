using HarmonyLib;
using SolastaMulticlass.Models;
using UnityEngine;

namespace SolastaMulticlass.Patches.HeroInspection
{
    internal static class SpellRepertoirePanelPatcher
    {
        // filters how spells and slots are displayed on inspection
        [HarmonyPatch(typeof(SpellRepertoirePanel), "Bind")]
        internal static class SpellRepertoirePanelBind
        {
            internal static void Postfix(
                SpellRepertoirePanel __instance,
                GameObject ___levelButtonPrefab,
                RectTransform ___levelButtonsTable,
                RectTransform ___spellsByLevelTable)
            {
                var spellRepertoire = __instance.SpellRepertoire;

                int classSpellLevel;
                int slotLevel;

                // determines the display context
                if (spellRepertoire.SpellCastingRace != null)
                {
                    classSpellLevel = 0;
                    slotLevel = 0;
                }
                else
                {
                    var heroWithSpellRepertoire = __instance.GuiCharacter.RulesetCharacterHero;
                    var spellCastingClass = spellRepertoire.SpellCastingClass;
                    var spellCastingSubclass = spellRepertoire.SpellCastingSubclass;

                    classSpellLevel = SharedSpellsContext.GetClassSpellLevel(heroWithSpellRepertoire, spellCastingClass, spellCastingSubclass);
                    slotLevel = SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire)
                        ? SharedSpellsContext.GetCombinedSpellLevel(heroWithSpellRepertoire)
                        : classSpellLevel;        
                }

                MulticlassGameUiContext.RebuildSlotsTable(
                    ___levelButtonPrefab,
                    ___levelButtonsTable,
                    ___spellsByLevelTable,
                    __instance.LevelSelected,
                    spellRepertoire.KnownCantrips.Count > 0 ? 1 : 0,
                    classSpellLevel,
                    slotLevel,
                    useClassSpellLevelOnButtons: false);
            }
        }
    }
}
