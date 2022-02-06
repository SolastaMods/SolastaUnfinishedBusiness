using System;
using HarmonyLib;
using SolastaModApi.Infrastructure;
using UnityEngine;
using UnityEngine.UI;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaCommunityExpansion.Multiclass.Patches.HeroInspection
{
    internal static class SpellRepertoirePanelPatcher
    {
        // filters how spells and slots are displayed on inspection
        [HarmonyPatch(typeof(SpellRepertoirePanel), "Bind")]
        internal static class SpellRepertoirePanelBind
        {
            internal static void Postfix(SpellRepertoirePanel __instance)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                var heroWithSpellRepertoire = __instance.GuiCharacter.RulesetCharacterHero;
                var spellRepertoire = __instance.SpellRepertoire;
                var spellCastingClass = spellRepertoire.SpellCastingClass;
                var spellCastingSubclass = spellRepertoire.SpellCastingSubclass;
                var spellCastingRace = spellRepertoire.SpellCastingRace;
                var warlockLevel = Models.SharedSpellsContext.GetWarlockLevel(heroWithSpellRepertoire);
                var accountForCantrips = (spellRepertoire?.KnownCantrips.Count > 0 || Models.IntegrationContext.IsExtraContentInstalled) ? 1 : 0;

                // determines the display context
                int slotLevel;
                var classSpellLevel = Models.SharedSpellsContext.GetClassSpellLevel(heroWithSpellRepertoire, spellCastingClass, spellCastingSubclass);

                if (Models.SharedSpellsContext.IsEnabled)
                {
                    if (Models.SharedSpellsContext.IsCombined)
                    {
                        slotLevel = Models.SharedSpellsContext.GetCombinedSpellLevel(heroWithSpellRepertoire);
                    }
                    else if (Models.SharedSpellsContext.IsWarlock(spellCastingClass))
                    {
                        slotLevel = Models.SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);
                    }
                    else
                    {
                        slotLevel = Models.SharedSpellsContext.GetSharedSpellLevel(heroWithSpellRepertoire);
                    }
                }
                else
                {
                    slotLevel = classSpellLevel;
                }

                // custom logic to ensure level 6 labels aren't displayed on other casters if Warlock is at Arcanum level
                if (warlockLevel >= Models.SharedSpellsContext.WARLOCK_MYSTIC_ARCANUM_START_LEVEL && spellCastingClass != Models.IntegrationContext.WarlockClass)
                {
                    slotLevel = Math.Min(Models.SharedSpellsContext.WARLOCK_MAX_PACT_MAGIC_SPELL_LEVEL, slotLevel);
                }

                var levelButtonsTable = __instance.GetField<SpellRepertoirePanel, RectTransform>("levelButtonsTable");
                var spellsByLevelTable = __instance.GetField<SpellRepertoirePanel, RectTransform>("spellsByLevelTable");

                // patches the spell level buttons to be hidden if no spells available at that level
                for (var i = 0; i < levelButtonsTable.childCount; i++)
                {
                    var child = levelButtonsTable.GetChild(i);

                    child.gameObject.SetActive(i <= classSpellLevel + accountForCantrips - 1 && (spellCastingRace == null || i == 0));
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
                            transform.gameObject.SetActive(i <= slotLevel && (spellCastingRace == null || i == 0));
                        }
                        else
                        {
                            transform.gameObject.SetActive(i <= classSpellLevel + accountForCantrips - 1 && (spellCastingRace == null || i == 0));
                        }
                    }
                }

                // determines if the sorcery points UI needs to be hidden
                var hasSorceryPoints = spellCastingClass == Sorcerer && heroWithSpellRepertoire.ClassesAndLevels[Sorcerer] >= 2;
                var sorceryPointsBox = __instance.GetField<SpellRepertoirePanel, RectTransform>("sorceryPointsBox");
                var sorceryPointsLabel = __instance.GetField<SpellRepertoirePanel, GuiLabel>("sorceryPointsLabel");

                sorceryPointsBox.gameObject.SetActive(hasSorceryPoints);
                sorceryPointsLabel.gameObject.SetActive(hasSorceryPoints);
                LayoutRebuilder.ForceRebuildLayoutImmediate(spellsByLevelTable);
            }
        }
    }
}
