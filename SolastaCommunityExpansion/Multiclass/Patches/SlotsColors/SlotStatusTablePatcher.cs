using System.Collections.Generic;
using HarmonyLib;
using SolastaCommunityExpansion;
using SolastaModApi.Infrastructure;
using SolastaMulticlass.Models;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaMulticlass.Patches.SlotsColors
{
    internal static class SlotStatusTablePatcher
    {
        // creates different slots colors and pop up messages depending on slot types
        [HarmonyPatch(typeof(SlotStatusTable), "Bind")]
        internal static class SlotStatusTableBind
        {
            public static void Postfix(
                SlotStatusTable __instance,
                RulesetSpellRepertoire spellRepertoire,
                int spellLevel,
                List<SpellDefinition> spells,
                bool compactIfNeeded = true)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                // spellRepertoire is null during level up so slots don't get displayed
                if (spellRepertoire == null || spellLevel == 0)
                {
                    return;
                }

                var str = string.Empty;
                var table = __instance.GetField<SlotStatusTable, RectTransform>("table");

                spellRepertoire.GetSlotsNumber(spellLevel, out var totalSlotsRemainingCount, out var totalSlotsCount);

                var heroWithSpellRepertoire = SharedSpellsContext.GetHero(spellRepertoire.CharacterName);

                var shortRestSlotsCount = SharedSpellsContext.GetWarlockMaxSlots(heroWithSpellRepertoire);
                var longRestSlotsCount = totalSlotsCount - shortRestSlotsCount;
                var shortRestSlotsUsedCount = 0;

                var warlockSpellRepertoire = SharedSpellsContext.GetWarlockSpellRepertoire(heroWithSpellRepertoire);
                var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);

                if (warlockSpellRepertoire != null)
                {
                    var usedSpellsSlots = warlockSpellRepertoire.GetField<RulesetSpellRepertoire, Dictionary<int, int>>("usedSpellsSlots");

                    usedSpellsSlots.TryGetValue(-1, out shortRestSlotsUsedCount);
                }

                var shortRestSlotsRemainingCount = shortRestSlotsCount - shortRestSlotsUsedCount;
                var longRestSlotsRemainingCount = totalSlotsRemainingCount - shortRestSlotsRemainingCount;
                var longRestSlotsUsedCount = longRestSlotsCount - longRestSlotsRemainingCount;

                if (spells.Count > 1 || totalSlotsCount == 1 || !compactIfNeeded)
                {
                    for (var index = 0; index < table.childCount; ++index)
                    {
                        var component = table.GetChild(index).GetComponent<SlotStatus>();

                        if (SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire))
                        {
                            if (totalSlotsRemainingCount == 0)
                            {
                                str = "Screen/&SpellSlotsUsedAllDescription";
                            }
                            else if (totalSlotsRemainingCount == totalSlotsCount)
                            {
                                str = "Screen/&SpellSlotsUsedNoneDescription";
                            }
                            else if (shortRestSlotsRemainingCount == shortRestSlotsCount)
                            {
                                str = Gui.Format("Screen/&SpellSlotsUsedLongDescription", longRestSlotsUsedCount.ToString());
                            }
                            else if (longRestSlotsRemainingCount == longRestSlotsCount)
                            {
                                str = Gui.Format("Screen/&SpellSlotsUsedShortDescription", shortRestSlotsUsedCount.ToString());
                            }
                            else
                            {
                                str = Gui.Format("Screen/&SpellSlotsUsedShortLongDescription", shortRestSlotsUsedCount.ToString(), longRestSlotsUsedCount.ToString());
                            }

                            if (spellLevel <= warlockSpellLevel)
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
                        }

                        if (SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire))
                        {
                            if (index >= longRestSlotsCount && spellLevel <= warlockSpellLevel)
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

                    table.GetComponent<GuiTooltip>().Content = str;
                }
            }
        }

        // ensures slot colors are white before getting back to pool
        [HarmonyPatch(typeof(SlotStatusTable), "Unbind")]
        internal static class SlotStatusTableUnbind
        {
            public static void Prefix(SlotStatusTable __instance)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                var table = __instance.GetField<SlotStatusTable, RectTransform>("table");

                for (var index = 0; index < table.childCount; ++index)
                {
                    var child = table.GetChild(index);
                    var component = child.GetComponent<SlotStatus>();

                    component.Available.GetComponent<Image>().color = SharedSpellsContext.WhiteSlot;
                }
            }
        }
    }
}
