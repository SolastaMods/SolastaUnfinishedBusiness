using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class FlexibleCastingItemPatcher
{
    [HarmonyPatch(typeof(FlexibleCastingItem), nameof(FlexibleCastingItem.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
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

    [HarmonyPatch(typeof(FlexibleCastingItem), nameof(FlexibleCastingItem.Unbind))]
    [UsedImplicitly]
    public static class Unbind_Patch
    {
        [UsedImplicitly]
        public static void Prefix(FlexibleCastingItem __instance)
        {
            //PATCH: ensures slot colors are white before getting back to pool (MULTICLASS)
            MulticlassGameUiContext.PaintSlotsWhite(__instance.slotStatusTable);
        }
    }
}
