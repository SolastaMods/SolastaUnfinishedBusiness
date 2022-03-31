using HarmonyLib;
using SolastaCommunityExpansion;
using SolastaModApi.Infrastructure;
using SolastaMulticlass.Models;
using UnityEngine;
using UnityEngine.UI;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;

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
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                // determines the display context
                int accountForCantrips;
                int classSpellLevel;
                int slotLevel;

                var heroWithSpellRepertoire = __instance.GuiCharacter.RulesetCharacterHero;
                var spellRepertoire = __instance.SpellRepertoire;
                var spellCastingClass = spellRepertoire.SpellCastingClass;
                var spellCastingSubclass = spellRepertoire.SpellCastingSubclass;
                var spellCastingRace = spellRepertoire.SpellCastingRace;

                if (spellCastingRace != null)
                {
                    accountForCantrips = 1;
                    classSpellLevel = 0;
                    slotLevel = 0;
                }
                else
                {
                    accountForCantrips = spellRepertoire.KnownCantrips.Count > 0 ? 1 : 0;
                    classSpellLevel = SharedSpellsContext.GetClassSpellLevel(heroWithSpellRepertoire, spellCastingClass, spellCastingSubclass);
                    slotLevel = SharedSpellsContext.GetCombinedSpellLevel(heroWithSpellRepertoire);
                }

                // patches the spell level buttons (using slotLevel here so we have buttons for all slots...)
                while(___levelButtonsTable.childCount < slotLevel + accountForCantrips)
                {
                    Gui.GetPrefabFromPool(___levelButtonPrefab, ___levelButtonsTable);

                    var index = ___levelButtonsTable.childCount - 1;
                    var child = ___levelButtonsTable.GetChild(index);

                    child.GetComponent<SpellLevelButton>().Bind(index, new SpellLevelButton.LevelSelectedHandler(__instance.LevelSelected));
                }

                while (___levelButtonsTable.childCount > slotLevel + accountForCantrips)
                {
                    Gui.ReleaseInstanceToPool(___levelButtonsTable.GetChild(___levelButtonsTable.childCount - 1).gameObject);
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(___levelButtonsTable);

                // patches the panel to display higher level spell slots from shared slots table but hide the spell panels if class level not there yet
                for (var i = 0; i < ___spellsByLevelTable.childCount; i++)
                {
                    var spellsByLevel = ___spellsByLevelTable.GetChild(i);

                    for (var j = 0; j < spellsByLevel.childCount; j++)
                    {
                        var transform = spellsByLevel.GetChild(j);

                        if (transform.TryGetComponent(typeof(SlotStatusTable), out var _))
                        {
                            transform.gameObject.SetActive(i < slotLevel + accountForCantrips); // table header (with slots)
                        }
                        else
                        {
                            transform.gameObject.SetActive(i < classSpellLevel + accountForCantrips); // table content
                        }
                    }
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(___spellsByLevelTable);

                // determines if the sorcery points UI needs to be hidden
                var hasSorceryPoints = spellCastingClass == Sorcerer && heroWithSpellRepertoire.ClassesAndLevels[Sorcerer] >= 2;
                var sorceryPointsBox = __instance.GetField<SpellRepertoirePanel, RectTransform>("sorceryPointsBox");
                var sorceryPointsLabel = __instance.GetField<SpellRepertoirePanel, GuiLabel>("sorceryPointsLabel");

                sorceryPointsBox.gameObject.SetActive(hasSorceryPoints);
                sorceryPointsLabel.gameObject.SetActive(hasSorceryPoints);
            }
        }
    }
}
