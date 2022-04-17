using HarmonyLib;
using SolastaMulticlass.Models;
using UnityEngine;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaMulticlass.Patches.LevelUp
{
    internal static class CharacterStageSpellSelectionPanelPatcher
    {
        [HarmonyPatch(typeof(CharacterStageSpellSelectionPanel), "Refresh")]
        internal static class CharacterStageSpellSelectionPanelRefresh
        {
            internal static void Postfix(CharacterStageSpellSelectionPanel __instance, RulesetCharacterHero ___currentHero, GameObject ___levelButtonPrefab, RectTransform ___levelButtonsTable, RectTransform ___spellsByLevelTable)
            {
                if (!LevelUpContext.IsLevelingUp(___currentHero))
                {
                    return;
                }

                // determines the display context
                var spellCastingClass = LevelUpContext.GetSelectedClass(___currentHero);
                var spellCastingSubclass = LevelUpContext.GetSelectedSubclass(___currentHero);
                var classSpellLevel = SharedSpellsContext.GetClassSpellLevel(___currentHero, spellCastingClass, spellCastingSubclass);

                MulticlassGameUiContext.RebuildSlotsTable(
                    ___levelButtonPrefab,
                    ___levelButtonsTable,
                    ___spellsByLevelTable,
                    __instance.LevelSelected,
                    (spellCastingClass != Ranger && spellCastingClass != Paladin) ? 1 : 0,
                    classSpellLevel,
                    classSpellLevel);
            }
        }
    }
}
