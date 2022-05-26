using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.Multiclass.SlotsSpells
{
    // creates different slots colors and pop up messages depending on slot types
    [HarmonyPatch(typeof(FlexibleCastingItem), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FlexibleCastingItem_Bind
    {
        internal static void Postfix(
            FlexibleCastingItem __instance,
            int slotLevel,
            int remainingSlots,
            int maxSlots,
            RectTransform ___slotStatusTable)
        {
            var flexibleCastingModal = __instance.GetComponentInParent<FlexibleCastingModal>();
            var caster =
                flexibleCastingModal.GetField<FlexibleCastingModal, RulesetCharacter>("caster") as RulesetCharacterHero;

            if (caster is null)
            {
                return;
            }

            if (!SharedSpellsContext.IsMulticaster(caster))
            {
                return;
            }

            MulticlassGameUiContext.PaintPactSlots(
                caster, maxSlots, remainingSlots, slotLevel, ___slotStatusTable);
        }
    }

    // ensures slot colors are white before getting back to pool
    [HarmonyPatch(typeof(FlexibleCastingItem), "Unbind")]
    internal static class FlexibleCastingItem_Unbind
    {
        internal static void Prefix(RectTransform ___slotStatusTable)
        {
            MulticlassGameUiContext.PaintSlotsWhite(___slotStatusTable);
        }
    }
}
