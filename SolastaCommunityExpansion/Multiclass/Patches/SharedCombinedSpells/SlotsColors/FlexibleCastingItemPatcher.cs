// keep this around if we need to support Warlock again
#if WARLOCK_PACT_MAGIC
using System;
using System.Collections.Generic;
using HarmonyLib;
using SolastaModApi.Infrastructure;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Multiclass.Patches.SharedCombinedSpells.SlotsColors
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

                for (var index = 0; index < slotStatusTable.childCount; ++index)
                {
                    var component = slotStatusTable.GetChild(index).GetComponent<SlotStatus>();
                    var heroWithSpellRepertoire = RulesetImplementationManagerPatcher.HeroWithSpellRepertoire;
                    var spellRepertoire = RulesetImplementationManagerPatcher.SpellRepertoire;
                    var shortRestSlotsCount = Models.SharedSpellsContext.GetWarlockMaxSlots(heroWithSpellRepertoire);
                    var longRestSlotsCount = maxSlots - shortRestSlotsCount;
                    var shortRestSlotsUsedCount = 0;
                    var warlockSpellRepertoire = Models.SharedSpellsContext.GetWarlockSpellRepertoire(heroWithSpellRepertoire);
                    var warlockSpellLevel = Models.SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);

                    if (warlockSpellRepertoire != null)
                    {
                        var usedSpellsSlots = warlockSpellRepertoire.GetField<RulesetSpellRepertoire, Dictionary<int, int>>("usedSpellsSlots");

                        usedSpellsSlots.TryGetValue(-1, out shortRestSlotsUsedCount);
                        shortRestSlotsUsedCount = Math.Min(shortRestSlotsUsedCount, shortRestSlotsCount);
                    }

                    if (warlockSpellRepertoire != null && slotLevel > Models.SharedSpellsContext.WARLOCK_MAX_PACT_MAGIC_SPELL_LEVEL)
                    {
                        shortRestSlotsCount = 0;
                        shortRestSlotsUsedCount = 0;
                        longRestSlotsCount = maxSlots;
                    }

                    var shortRestSlotsRemainingCount = shortRestSlotsCount - shortRestSlotsUsedCount;

                    if (Models.SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire) && Models.SharedSpellsContext.IsEnabled && Models.SharedSpellsContext.IsCombined && slotLevel <= warlockSpellLevel)
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

                    if (Models.SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire) && Models.SharedSpellsContext.IsEnabled)
                    {
                        if (Models.SharedSpellsContext.IsCombined)
                        {
                            if (index >= longRestSlotsCount && slotLevel <= warlockSpellLevel)
                            {
                                component.Available.GetComponent<Image>().color = new Color(0f, 1f, 0f, 1f);
                            }
                            else
                            {
                                component.Available.GetComponent<Image>().color = new Color(0f, 0.5f, 0f, 1f);
                            }
                        }
                        else
                        {
                            if (Models.SharedSpellsContext.IsWarlock(spellRepertoire?.SpellCastingClass))
                            {
                                component.Available.GetComponent<Image>().color = new Color(0f, 1f, 0f, 1f);
                            }
                            else
                            {
                                component.Available.GetComponent<Image>().color = new Color(0f, 0.5f, 0f, 1f);
                            }
                        }
                    }
                    else
                    {
                        component.Available.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                    }
                }
            }
        }
    }
}
#endif
