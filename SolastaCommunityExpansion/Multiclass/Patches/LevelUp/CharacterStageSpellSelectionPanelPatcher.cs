using HarmonyLib;
using SolastaModApi.Infrastructure;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Multiclass.Patches.LevelUp
{
    internal static class CharacterStageSpellSelectionPanelPatcher
    {
        [HarmonyPatch(typeof(CharacterStageSpellSelectionPanel), "Refresh")]
        internal static class CharacterStageSpellSelectionPanelRefresh
        {
            internal static void Postfix(CharacterStageSpellSelectionPanel __instance, RulesetCharacterHero ___currentHero)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                if (!Models.LevelUpContext.IsLevelingUp(___currentHero))
                {
                    return;
                }

                var spellCastingClass = Models.LevelUpContext.GetSelectedClass(___currentHero);
                var spellCastingSubclass = Models.LevelUpContext.GetSelectedSubclass(___currentHero); ;

                // determines the display context
                int slotLevel;
                var classSpellLevel = Models.SharedSpellsContext.GetClassSpellLevel(___currentHero, spellCastingClass, spellCastingSubclass);

                if (Models.SharedSpellsContext.IsEnabled)
                {
                    if (Models.SharedSpellsContext.IsCombined)
                    {
                        slotLevel = Models.SharedSpellsContext.GetCombinedSpellLevel(___currentHero);
                    }
                    else if (Models.SharedSpellsContext.IsWarlock(spellCastingClass))
                    {
                        slotLevel = Models.SharedSpellsContext.GetWarlockSpellLevel(___currentHero);
                    }
                    else
                    {
                        slotLevel = Models.SharedSpellsContext.GetSharedSpellLevel(___currentHero);
                    }
                }
                else
                {
                    slotLevel = classSpellLevel;
                }

                var levelButtonsTable = __instance.GetField<CharacterStageSpellSelectionPanel, RectTransform>("levelButtonsTable");
                var spellsByLevelTable = __instance.GetField<CharacterStageSpellSelectionPanel, RectTransform>("spellsByLevelTable");

                // patches the spell level buttons to be hidden if no spells available at that level
                for (var i = 0; i < levelButtonsTable.childCount; i++)
                {
                    var child = levelButtonsTable.GetChild(i);

                    child.gameObject.SetActive(i <= classSpellLevel);
                }

                // patches the panel to display higher level spell slots from shared slots table but hide the spell panels if class level not there yet
                for (var i = 0; i < spellsByLevelTable.childCount; i++)
                {
                    var spellsByLevel = spellsByLevelTable.GetChild(i);

                    for (var j = 0; j < spellsByLevel.childCount; j++)
                    {
                        var transform = spellsByLevel.GetChild(j);

                        if (transform.TryGetComponent(typeof(SlotStatusTable), out var _))
                        {
                            transform.gameObject.SetActive(i <= slotLevel);
                        }
                        else
                        {
                            transform.gameObject.SetActive(i <= classSpellLevel);
                        }
                    }
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(spellsByLevelTable);
            }
        }
    }
}
