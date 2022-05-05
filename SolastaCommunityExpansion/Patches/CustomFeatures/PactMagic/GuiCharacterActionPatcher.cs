#if false
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaMulticlass.Models;
using UnityEngine;
using static SolastaModApi.DatabaseHelper.ActionDefinitions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.PactMagic
{
    [HarmonyPatch(typeof(GuiCharacterAction), "SetupUseSlots")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]

    internal static class GuiCharacterAction_SetupUseSlots
    {
        public static void Postfix(
            RectTransform useSlotsTable, 
            GameObject slotStatusPrefab,
            GameLocationCharacter ___actingCharacter,
            ActionDefinition ___actionDefinition)
        {
            if (___actionDefinition != CastBonus && ___actionDefinition != CastMain)
            {
                return;
            }

            var hero = ___actingCharacter.RulesetCharacter as RulesetCharacterHero;
            var warlockRepertoire = SharedSpellsContext.GetWarlockSpellRepertoire(hero);

            if (warlockRepertoire == null)
            {
                return;
            }

            //
            // TODO: fix this fetch logic for MC...
            //
            warlockRepertoire.GetSlotsNumber(Classes.Warlock.WarlockSpells.PACT_MAGIC_SLOT_TAB_INDEX, out var remaining, out var max);

            while (useSlotsTable.childCount < max)
            {
                Gui.GetPrefabFromPool(slotStatusPrefab, useSlotsTable);
            }

            for (int index = 0; index < useSlotsTable.childCount; ++index)
            {
                var child = useSlotsTable.GetChild(index);

                if (index < max)
                {
                    child.gameObject.SetActive(true);
                    SlotStatus component = child.GetComponent<SlotStatus>();
                    component.Used.gameObject.SetActive(index >= remaining);
                    component.Available.gameObject.SetActive(index < remaining);
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }

            useSlotsTable.gameObject.SetActive(true);
        }
    }
}
#endif
