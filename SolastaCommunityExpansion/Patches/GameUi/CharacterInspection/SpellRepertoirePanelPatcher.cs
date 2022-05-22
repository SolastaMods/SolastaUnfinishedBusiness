using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterInspection
{
    // allows prepared spell casters to take metamagic feats and have a working UI [otherwise sorcery points get off screen]
    [HarmonyPatch(typeof(SpellRepertoirePanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SpellRepertoirePanel_OnBeginShow
    {
        internal static void Postfix(SpellRepertoirePanel __instance, RectTransform ___sorceryPointsBox)
        {
            if (!Main.Settings.EnableMoveSorceryPointsBox)
            {
                return;
            }

            var rectTransform = ___sorceryPointsBox.GetComponent<RectTransform>();

            rectTransform.sizeDelta = new Vector2(275, 32);
            ___sorceryPointsBox.localPosition = new Vector3(-920, 38, 0);

            __instance.RefreshNow();
        }
    }

    // filters how spells and slots are displayed on inspection
    [HarmonyPatch(typeof(SpellRepertoirePanel), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SpellRepertoirePanel_Bind
    {
        private static void RebuildSlotsTable(
            GameObject ___levelButtonPrefab,
            RectTransform ___levelButtonsTable,
            RectTransform ___spellsByLevelTable,
            SpellLevelButton.LevelSelectedHandler levelSelected,
            int accountForCantrips,
            int classSpellLevel,
            int slotLevel)
        {
            while (___levelButtonsTable.childCount < classSpellLevel + accountForCantrips)
            {
                Gui.GetPrefabFromPool(___levelButtonPrefab, ___levelButtonsTable);

                var index = ___levelButtonsTable.childCount - 1;
                var child = ___levelButtonsTable.GetChild(index);

                child.GetComponent<SpellLevelButton>().Bind(index, new SpellLevelButton.LevelSelectedHandler(levelSelected));
            }

            while (___levelButtonsTable.childCount > classSpellLevel + accountForCantrips)
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
        }

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
                var isWarlockRepertoire = SharedSpellsContext.IsWarlock(spellRepertoire.SpellCastingClass);
                var isSharedcaster = SharedSpellsContext.IsSharedcaster(heroWithSpellRepertoire);
                var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);
                var sharedSpellLevel = SharedSpellsContext.GetSharedSpellLevel(heroWithSpellRepertoire);

                classSpellLevel = SharedSpellsContext.GetClassSpellLevel(spellRepertoire);
                slotLevel = Math.Max(isSharedcaster ? sharedSpellLevel : classSpellLevel, warlockSpellLevel);
            }

            RebuildSlotsTable(
                ___levelButtonPrefab,
                ___levelButtonsTable,
                ___spellsByLevelTable,
                __instance.LevelSelected,
                spellRepertoire.KnownCantrips.Count > 0 ? 1 : 0,
                classSpellLevel,
                slotLevel);
        }
    }
}
