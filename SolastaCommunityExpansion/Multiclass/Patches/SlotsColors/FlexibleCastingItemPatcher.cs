using System;
using System.Collections.Generic;
using HarmonyLib;
using SolastaCommunityExpansion;
using SolastaModApi.Infrastructure;
using SolastaMulticlass.Models;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaMulticlass.Patches.SlotsColors
{
    internal static class FlexibleCastingItemPatcher
    {
        // creates different slots colors and pop up messages depending on slot types
        [HarmonyPatch(typeof(FlexibleCastingItem), "Bind")]
        internal static class FlexibleCastingItemBind
        {
            internal static void Postfix(FlexibleCastingItem __instance, int slotLevel, int remainingSlots, int maxSlots)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                var slotStatusTable = __instance.GetField<FlexibleCastingItem, RectTransform>("slotStatusTable");

                var flexibleCastingModal = __instance.GetComponentInParent<FlexibleCastingModal>();
                var heroWithSpellRepertoire = flexibleCastingModal.GetField<FlexibleCastingModal, RulesetCharacterHero>("caster");

                var shortRestSlotsCount = SharedSpellsContext.GetWarlockMaxSlots(heroWithSpellRepertoire);
                var longRestSlotsCount = maxSlots - shortRestSlotsCount;
                var shortRestSlotsUsedCount = 0;

                var warlockSpellRepertoire = SharedSpellsContext.GetWarlockSpellRepertoire(heroWithSpellRepertoire);
                var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);

                if (warlockSpellRepertoire != null)
                {
                    var usedSpellsSlots = warlockSpellRepertoire.GetField<RulesetSpellRepertoire, Dictionary<int, int>>("usedSpellsSlots");

                    usedSpellsSlots.TryGetValue(-1, out shortRestSlotsUsedCount);
                    shortRestSlotsUsedCount = Math.Min(shortRestSlotsUsedCount, shortRestSlotsCount);
                }

                var shortRestSlotsRemainingCount = shortRestSlotsCount - shortRestSlotsUsedCount;

                for (var index = 0; index < slotStatusTable.childCount; ++index)
                {
                    var component = slotStatusTable.GetChild(index).GetComponent<SlotStatus>();

                    if (SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire) && slotLevel <= warlockSpellLevel)
                    {
                        if (index < longRestSlotsCount)
                        {
                            component.Used.gameObject.SetActive(index >= remainingSlots - shortRestSlotsRemainingCount);
                            component.Available.gameObject.SetActive(index < remainingSlots - shortRestSlotsRemainingCount);
                        }
                        else
                        {
                            component.Used.gameObject.SetActive(index - longRestSlotsCount >= shortRestSlotsRemainingCount);
                            component.Available.gameObject.SetActive(index - longRestSlotsCount < shortRestSlotsRemainingCount);
                        }
                    }

                    if (SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire))
                    {
                        if (index >= longRestSlotsCount && slotLevel <= warlockSpellLevel)
                        {
                            component.Available.GetComponent<Image>().color = SharedSpellsContext.LightGreenSlot;
                        }
                        else
                        {
                            component.Available.GetComponent<Image>().color = SharedSpellsContext.DarkGreenSlot;
                        }
                    }
                    else
                    {
                        component.Available.GetComponent<Image>().color = SharedSpellsContext.WhiteSlot;
                    }
                }
            }
        }

        // ensures slot colors are white before getting back to pool
        [HarmonyPatch(typeof(FlexibleCastingItem), "Unbind")]
        internal static class FlexibleCastingItemUnbind
        {
            internal static void Prefix(FlexibleCastingItem __instance)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                var slotStatusTable = __instance.GetField<FlexibleCastingItem, RectTransform>("slotStatusTable");

                for (var index = 0; index < slotStatusTable.childCount; ++index)
                {
                    var child = slotStatusTable.GetChild(index);
                    var component = child.GetComponent<SlotStatus>();

                    component.Available.GetComponent<Image>().color = SharedSpellsContext.WhiteSlot;
                }
            }
        }
    }
}
