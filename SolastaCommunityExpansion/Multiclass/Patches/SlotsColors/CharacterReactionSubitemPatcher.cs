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

                spellRepertoire.GetSlotsNumber(slotLevel, out var totalSlotsRemainingCount, out var totalSlotsCount);

                var slotStatusTable = __instance.GetField<CharacterReactionSubitem, RectTransform>("slotStatusTable");

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
                            component.Used.gameObject.SetActive(index >= totalSlotsRemainingCount - shortRestSlotsRemainingCount);
                            component.Available.gameObject.SetActive(index < totalSlotsRemainingCount - shortRestSlotsRemainingCount);
                        }
                        else
                        {
                            component.Used.gameObject.SetActive(index >= longRestSlotsCount + shortRestSlotsRemainingCount);
                            component.Available.gameObject.SetActive(index < longRestSlotsCount + shortRestSlotsRemainingCount);
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
        [HarmonyPatch(typeof(CharacterReactionSubitem), "Unbind")]
        internal static class CharacterReactionSubitemUnbind
        {
            public static void Prefix(CharacterReactionSubitem __instance)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                var slotStatusTable = __instance.GetField<CharacterReactionSubitem, RectTransform>("slotStatusTable");

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
