using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
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
            var hero = flexibleCastingModal.caster.GetOriginalHero();

            if (hero == null)
            {
                return;
            }

            if (!SharedSpellsContext.IsMulticaster(hero))
            {
                // no way a Warlock would get here so no need to check for Warlock
                if (!Main.Settings.UseAlternateSpellPointsSystem)
                {
                    return;
                }

                //PATCH: support alternate spell system to avoid displaying spell slots on selection (SPELL_POINTS)
                for (var index = 0; index < __instance.slotStatusTable.childCount; ++index)
                {
                    var component = __instance.slotStatusTable.GetChild(index).GetComponent<SlotStatus>();

                    component.Used.gameObject.SetActive(false);
                    component.Available.gameObject.SetActive(false);
                }

                return;
            }

            MulticlassGameUiContext.PaintPactSlotsAlternate(
                hero, maxSlots, remainingSlots, slotLevel, __instance.slotStatusTable);
        }
    }

    [HarmonyPatch(typeof(FlexibleCastingItem), nameof(FlexibleCastingItem.Unbind))]
    [UsedImplicitly]
    public static class Unbind_Patch
    {
        [UsedImplicitly]
        public static void Prefix(FlexibleCastingItem __instance)
        {
            //PATCH: support alternate spell system to ensure points display is refreshed (SPELL_POINTS)
            if (Main.Settings.UseAlternateSpellPointsSystem)
            {
                SpellPointsContext.RefreshActionPanel();
            }

            //PATCH: ensures slot colors are white before getting back to pool (MULTICLASS)
            MulticlassGameUiContext.PaintSlotsWhite(__instance.slotStatusTable);
        }
    }
}
