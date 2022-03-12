// keep this around if we need to support Warlock again
#if false
using System;
using System.Collections.Generic;
using HarmonyLib;
using SolastaModApi.Infrastructure;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Multiclass.Patches.SharedCombinedSpells.SlotsColors
{
    internal static class CharacterReactionSubitemPatcher
    {
        // creates different slots colors and pop up messages depending on slot types
        [HarmonyPatch(typeof(CharacterReactionSubitem), "Bind")]
        internal static class CharacterReactionSubitemBind
        {
            public static void Postfix(CharacterReactionSubitem __instance, RulesetSpellRepertoire spellRepertoire, int slotLevel)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                if (spellRepertoire == null)
                {
                    return;
                }

                spellRepertoire.GetSlotsNumber(slotLevel, out var totalSlotsRemainingCount, out var totalSlotsCount);

                var slotStatusTable = __instance.GetField<CharacterReactionSubitem, RectTransform>("slotStatusTable");

                for (var index = 0; index < slotStatusTable.childCount; ++index)
                {
                    var child = slotStatusTable.GetChild(index);
                    var component = child.GetComponent<SlotStatus>();

                    child.gameObject.SetActive(true);
                    component.Used.gameObject.SetActive(index >= totalSlotsRemainingCount);
                    component.Available.gameObject.SetActive(index < totalSlotsRemainingCount);

                    if (spellRepertoire?.CharacterName == null)
                    {
                        continue;
                    }

                    var heroWithSpellRepertoire = Models.SharedSpellsContext.GetHero(spellRepertoire?.CharacterName);
                    var shortRestSlotsCount = Models.SharedSpellsContext.GetWarlockMaxSlots(heroWithSpellRepertoire);
                    var longRestSlotsCount = totalSlotsCount - shortRestSlotsCount;
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
                        longRestSlotsCount = totalSlotsCount;
                    }

                    var shortRestSlotsRemainingCount = shortRestSlotsCount - shortRestSlotsUsedCount;

                    if (Models.SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire) && Models.SharedSpellsContext.IsEnabled && Models.SharedSpellsContext.IsCombined && slotLevel <= warlockSpellLevel)
                    {
                        if (index < longRestSlotsCount)
                        {
                            component.Used.gameObject.SetActive(index >= totalSlotsRemainingCount - shortRestSlotsRemainingCount);
                            component.Available.gameObject.SetActive(index < totalSlotsRemainingCount - shortRestSlotsRemainingCount);
                        }
                        else
                        {
                            component.Used.gameObject.SetActive(index >= longRestSlotsCount + shortRestSlotsRemainingCount);
                            component.Available.gameObject.SetActive(index < longRestSlotsCount + shortRestSlotsRemainingCount);
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
