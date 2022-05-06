using HarmonyLib;
using SolastaModApi.Infrastructure;
using SolastaCommunityExpansion.Models;
using UnityEngine;

namespace SolastaMulticlass.Patches.SlotsColors
{
    internal static class FlexibleCastingItemPatcher
    {
        // creates different slots colors and pop up messages depending on slot types
        [HarmonyPatch(typeof(FlexibleCastingItem), "Bind")]
        internal static class FlexibleCastingItemBind
        {
            internal static void Postfix(
                FlexibleCastingItem __instance,
                int slotLevel,
                int remainingSlots,
                int maxSlots,
                bool createSlot,
                RectTransform ___slotStatusTable)
            {
                var flexibleCastingModal = __instance.GetComponentInParent<FlexibleCastingModal>();
                var caster = flexibleCastingModal.GetField<FlexibleCastingModal, RulesetCharacter>("caster") as RulesetCharacterHero;

                if (!SharedSpellsContext.IsMulticaster(caster))
                {
                    return;
                }

                MulticlassGameUiContext.PaintPactSlots(
                    caster, maxSlots, remainingSlots, slotLevel, ___slotStatusTable, true);
            }
        }

        // ensures slot colors are white before getting back to pool
        [HarmonyPatch(typeof(FlexibleCastingItem), "Unbind")]
        internal static class FlexibleCastingItemUnbind
        {
            internal static void Prefix(RectTransform ___slotStatusTable)
            {
                MulticlassGameUiContext.PaintSlotsWhite(___slotStatusTable);
            }
        }
    }
}
