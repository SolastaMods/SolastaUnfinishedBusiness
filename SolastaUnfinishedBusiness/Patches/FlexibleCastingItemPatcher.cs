using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

internal static class FlexibleCastingItemPatcher
{
    [HarmonyPatch(typeof(FlexibleCastingItem), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Bind_Patch
    {
        internal static void Postfix(
            FlexibleCastingItem __instance,
            int slotLevel,
            int remainingSlots,
            int maxSlots)
        {
            //PATCH: creates different slots colors and pop up messages depending on slot types (MULTICLASS)
            var flexibleCastingModal = __instance.GetComponentInParent<FlexibleCastingModal>();

            if (flexibleCastingModal.caster is not RulesetCharacterHero caster)
            {
                return;
            }

            if (!SharedSpellsContext.IsMulticaster(caster))
            {
                return;
            }

            MulticlassGameUiContext.PaintPactSlotsAlternate(
                caster, maxSlots, remainingSlots, slotLevel, __instance.slotStatusTable);
        }
    }

    [HarmonyPatch(typeof(FlexibleCastingItem), "Unbind")]
    internal static class Unbind_Patch
    {
        internal static void Prefix(FlexibleCastingItem __instance)
        {
            //PATCH: ensures slot colors are white before getting back to pool (MULTICLASS)
            MulticlassGameUiContext.PaintSlotsWhite(__instance.slotStatusTable);
        }
    }
}
