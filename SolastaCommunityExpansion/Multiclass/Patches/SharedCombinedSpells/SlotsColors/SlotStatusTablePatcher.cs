using System.Collections.Generic;
using HarmonyLib;
using SolastaModApi.Infrastructure;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Multiclass.Patches.SharedCombinedSpells.SlotsColors
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

                if (spellRepertoire == null || spellLevel == 0)
                {
                    return;
                }

                var str = string.Empty;
                var table = __instance.GetField<SlotStatusTable, RectTransform>("table");

                spellRepertoire.GetSlotsNumber(spellLevel, out var totalSlotsRemainingCount, out var totalSlotsCount);

                if (spells?.Count > 1 || totalSlotsCount == 1 || !compactIfNeeded)
                {
                    for (var index = 0; index < table.childCount; ++index)
                    {
                        var child = table.GetChild(index);
                        var component = child.GetComponent<SlotStatus>();

                        if (spellRepertoire?.CharacterName == null)
                        {
                            continue;
                        }

                        var heroWithSpellRepertoire = Models.SharedSpellsContext.GetHero(spellRepertoire.CharacterName);
                        var shortRestSlotsCount = Models.SharedSpellsContext.GetWarlockMaxSlots(heroWithSpellRepertoire);
                        var longRestSlotsCount = totalSlotsCount - shortRestSlotsCount;
                        var shortRestSlotsUsedCount = 0;
                        var warlockSpellRepertoire = Models.SharedSpellsContext.GetWarlockSpellRepertoire(heroWithSpellRepertoire);
                        var warlockSpellLevel = Models.SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);

                        if (warlockSpellRepertoire != null)
                        {
                            var usedSpellsSlots = warlockSpellRepertoire.GetField<RulesetSpellRepertoire, Dictionary<int,int>>("usedSpellsSlots");

                            usedSpellsSlots.TryGetValue(-1, out shortRestSlotsUsedCount);
                        }

                        if (warlockSpellRepertoire != null && spellLevel > Models.SharedSpellsContext.WARLOCK_MAX_PACT_MAGIC_SPELL_LEVEL)
                        {
                            shortRestSlotsCount = 0;
                            shortRestSlotsUsedCount = 0;
                            longRestSlotsCount = totalSlotsCount;
                        }

                        var shortRestSlotsRemainingCount = shortRestSlotsCount - shortRestSlotsUsedCount;
                        var longRestSlotsRemainingCount = totalSlotsRemainingCount - shortRestSlotsRemainingCount;
                        var longRestSlotsUsedCount = longRestSlotsCount - longRestSlotsRemainingCount;

                        if (Models.SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire) && Models.SharedSpellsContext.IsEnabled && Models.SharedSpellsContext.IsCombined)
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

                        if (Models.SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire) && Models.SharedSpellsContext.IsEnabled)
                        {
                            if (Models.SharedSpellsContext.IsCombined)
                            {
                                if (index >= longRestSlotsCount && spellLevel <= warlockSpellLevel)
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

                    table.GetComponent<GuiTooltip>().Content = str;
                }
            }
        }
    }
}
