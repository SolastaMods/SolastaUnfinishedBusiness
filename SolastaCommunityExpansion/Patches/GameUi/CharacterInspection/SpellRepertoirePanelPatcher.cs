using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterInspection;

// allows prepared spell casters to take metamagic feats and have a working UI [otherwise sorcery points get off screen]
[HarmonyPatch(typeof(SpellRepertoirePanel), "OnBeginShow")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class SpellRepertoirePanel_OnBeginShow
{
    internal static void Postfix(SpellRepertoirePanel __instance)
    {
        if (!Main.Settings.EnableMoveSorceryPointsBox)
        {
            return;
        }

        var rectTransform = __instance.sorceryPointsBox.GetComponent<RectTransform>();

        rectTransform.sizeDelta = new Vector2(275, 32);
        __instance.sorceryPointsBox.localPosition = new Vector3(-920, 38, 0);

        __instance.RefreshNow();
    }
}

// filters how spells and slots are displayed on inspection
[HarmonyPatch(typeof(SpellRepertoirePanel), "Bind")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class SpellRepertoirePanel_Bind
{
    private static void RebuildSlotsTable(
        SpellRepertoirePanel __instance,
        int accountForCantrips,
        int classSpellLevel,
        int slotLevel)
    {
        while (__instance.levelButtonsTable.childCount < classSpellLevel + accountForCantrips)
        {
            Gui.GetPrefabFromPool(__instance.levelButtonPrefab, __instance.levelButtonsTable);

            var index = __instance.levelButtonsTable.childCount - 1;
            var child = __instance.levelButtonsTable.GetChild(index);

            child.GetComponent<SpellLevelButton>().Bind(index, __instance.LevelSelected);
        }

        while (__instance.levelButtonsTable.childCount > classSpellLevel + accountForCantrips)
        {
            Gui.ReleaseInstanceToPool(__instance.levelButtonsTable
                .GetChild(__instance.levelButtonsTable.childCount - 1)
                .gameObject);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(__instance.levelButtonsTable);

        // patches the panel to display higher level spell slots from shared slots table but hide the spell panels if class level not there yet
        for (var i = 0; i < __instance.spellsByLevelTable.childCount; i++)
        {
            var spellsByLevel = __instance.spellsByLevelTable.GetChild(i);

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

        LayoutRebuilder.ForceRebuildLayoutImmediate(__instance.spellsByLevelTable);
    }

    internal static void Postfix(SpellRepertoirePanel __instance)
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
            var isSharedcaster = SharedSpellsContext.IsSharedcaster(heroWithSpellRepertoire);
            var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);
            var sharedSpellLevel = SharedSpellsContext.GetSharedSpellLevel(heroWithSpellRepertoire);

            classSpellLevel = SharedSpellsContext.GetClassSpellLevel(spellRepertoire);
            slotLevel = Math.Max(isSharedcaster ? sharedSpellLevel : classSpellLevel, warlockSpellLevel);
        }

        RebuildSlotsTable(
            __instance,
            spellRepertoire.KnownCantrips.Count > 0 ? 1 : 0,
            classSpellLevel,
            slotLevel);
    }
}
