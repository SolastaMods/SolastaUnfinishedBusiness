using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.Classes.Warlock;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaMulticlass.Patches.SlotsSpells
{
    internal static class RulesetSpellRepertoirePatcher
    {
        private static bool ShouldNotRun(RulesetSpellRepertoire __instance)
        {
            var heroWithSpellRepertoire = SharedSpellsContext.GetHero(__instance.CharacterName);

            return heroWithSpellRepertoire == null
                || SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire)
                || !SharedSpellsContext.IsWarlock(__instance.SpellCastingClass);
        }

        //
        // the following 4 patches also exist in CE. The ones in CE get disabled in favor of these
        //

        // ensures MC Warlocks are treated before SC ones
        [HarmonyPatch(typeof(RulesetSpellRepertoire), "GetMaxSlotsNumberOfAllLevels")]
        internal static class RulesetSpellRepertoireGetMaxSlotsNumberOfAllLevels
        {
            internal static bool Prefix(
                RulesetSpellRepertoire __instance,
                ref int __result,
                Dictionary<int, int> ___spellsSlotCapacities)
            {
                if (ShouldNotRun(__instance))
                {
                    return true;
                }

                // handles SC Warlock
                ___spellsSlotCapacities.TryGetValue(WarlockSpells.PACT_MAGIC_SLOT_TAB_INDEX, out __result);

                return false;
            }
        }

        // ensures MC Warlocks are treated before SC ones
        [HarmonyPatch(typeof(RulesetSpellRepertoire), "GetRemainingSlotsNumberOfAllLevels")]
        internal static class RulesetSpellRepertoireGetRemainingSlotsNumberOfAllLevels
        {
            internal static bool Prefix(
                RulesetSpellRepertoire __instance,
                ref int __result,
                Dictionary<int, int> ___usedSpellsSlots,
                Dictionary<int, int> ___spellsSlotCapacities)
            {
                if (ShouldNotRun(__instance))
                {
                    return true;
                }

                // handles SC Warlock
                ___spellsSlotCapacities.TryGetValue(WarlockSpells.PACT_MAGIC_SLOT_TAB_INDEX, out var max);
                ___usedSpellsSlots.TryGetValue(WarlockSpells.PACT_MAGIC_SLOT_TAB_INDEX, out var used);
                __result = max - used;

                return false;
            }
        }

        // handles all different scenarios to determine slots numbers
        [HarmonyPatch(typeof(RulesetSpellRepertoire), "GetSlotsNumber")]
        internal static class RulesetSpellRepertoireGetSlotsNumber
        {
            internal static bool Prefix(
                RulesetSpellRepertoire __instance,
                Dictionary<int, int> ___usedSpellsSlots,
                Dictionary<int, int> ___spellsSlotCapacities,
                int spellLevel,
                ref int remaining,
                ref int max)
            {
                if (ShouldNotRun(__instance))
                {
                    return true;
                }

                // handles SC Warlock
                max = 0;
                remaining = 0;

                if (spellLevel <= __instance.MaxSpellLevelOfSpellCastingLevel)
                {
                    ___spellsSlotCapacities.TryGetValue(WarlockSpells.PACT_MAGIC_SLOT_TAB_INDEX, out max);
                    ___usedSpellsSlots.TryGetValue(WarlockSpells.PACT_MAGIC_SLOT_TAB_INDEX, out var used);
                    remaining = max - used;
                }

                return false;
            }
        }

        // handles all different scenarios of spell slots consumption (casts, smites, point buys)
        [HarmonyPatch(typeof(RulesetSpellRepertoire), "SpendSpellSlot")]
        internal static class RulesetSpellRepertoireSpendSpellSlot
        {
            internal static bool Prefix(RulesetSpellRepertoire __instance, int slotLevel)
            {
                if (slotLevel == 0)
                {
                    return true;
                }

                var heroWithSpellRepertoire = SharedSpellsContext.GetHero(__instance.CharacterName);

                if (heroWithSpellRepertoire == null)
                {
                    return true;
                }

                if (!SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire))
                {
                    // handles SC Warlock
                    if (SharedSpellsContext.IsWarlock(__instance.SpellCastingClass))
                    {
                        SpendWarlockSlots(__instance, heroWithSpellRepertoire);

                        return false;
                    }

                    // handles SC non-Warlock
                    return true;
                }

                var warlockSpellRepertoire = SharedSpellsContext.GetWarlockSpellRepertoire(heroWithSpellRepertoire);

                // handles MC non-Warlock
                if (warlockSpellRepertoire == null)
                {
                    foreach (var spellRepertoire in heroWithSpellRepertoire.SpellRepertoires
                        .Where(x => x.SpellCastingRace == null))
                    {
                        var usedSpellsSlots = spellRepertoire.GetField<RulesetSpellRepertoire, Dictionary<int, int>>("usedSpellsSlots");

                        usedSpellsSlots.TryAdd(slotLevel, 0);
                        usedSpellsSlots[slotLevel]++;
                        spellRepertoire.RepertoireRefreshed?.Invoke(spellRepertoire);
                    }
                }

                // handles MC Warlock
                else
                {
                    SpendMulticasterWarlockSlots(warlockSpellRepertoire, heroWithSpellRepertoire, slotLevel);
                }

                return false;
            }

            private static void SpendWarlockSlots(RulesetSpellRepertoire rulesetSpellRepertoire, RulesetCharacterHero heroWithSpellRepertoire)
            {
                var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);
                var usedSpellsSlots = rulesetSpellRepertoire.GetField<RulesetSpellRepertoire, Dictionary<int, int>>("usedSpellsSlots");

                for (var i = SharedSpellsContext.MC_PACT_MAGIC_SLOT_TAB_INDEX; i <= warlockSpellLevel; i++)
                {
                    if (i == 0)
                    {
                        continue;
                    }

                    usedSpellsSlots.TryAdd(i, 0);
                    usedSpellsSlots[i]++;
                }

                rulesetSpellRepertoire.RepertoireRefreshed?.Invoke(rulesetSpellRepertoire);
            }

            private static void SpendMulticasterWarlockSlots(RulesetSpellRepertoire warlockSpellRepertoire, RulesetCharacterHero heroWithSpellRepertoire, int slotLevel)
            {
                var sharedSpellLevel = SharedSpellsContext.GetSharedSpellLevel(heroWithSpellRepertoire);
                var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);
                var warlockMaxSlots = SharedSpellsContext.GetWarlockMaxSlots(heroWithSpellRepertoire);
                var usedSpellsSlotsWarlock = warlockSpellRepertoire.GetField<RulesetSpellRepertoire, Dictionary<int, int>>("usedSpellsSlots");

                usedSpellsSlotsWarlock.TryGetValue(SharedSpellsContext.MC_PACT_MAGIC_SLOT_TAB_INDEX, out var warlockUsedSlots);
                warlockSpellRepertoire.GetSlotsNumber(slotLevel, out var sharedRemainingSlots, out var sharedMaxSlots);

                var sharedUsedSlots = sharedMaxSlots - sharedRemainingSlots;

                sharedMaxSlots -= warlockMaxSlots;
                sharedUsedSlots -= warlockUsedSlots;

                var canConsumeShortRestSlot = warlockUsedSlots < warlockMaxSlots && slotLevel <= warlockSpellLevel;
                var canConsumeLongRestSlot = sharedUsedSlots < sharedMaxSlots && slotLevel <= sharedSpellLevel;
                var forceLongRestSlot = canConsumeLongRestSlot && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));

                // uses short rest slots across all repertoires
                if (canConsumeShortRestSlot && !forceLongRestSlot)
                {
                    foreach (var spellRepertoire in heroWithSpellRepertoire.SpellRepertoires)
                    {
                        if (spellRepertoire.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Race)
                        {
                            continue;
                        }

                        SpendWarlockSlots(spellRepertoire, heroWithSpellRepertoire);
                    }
                }

                // otherwise uses long rest slots across all non race repertoires
                else
                {
                    foreach (var spellRepertoire in heroWithSpellRepertoire.SpellRepertoires
                        .Where(x => x.SpellCastingFeature.SpellCastingOrigin != FeatureDefinitionCastSpell.CastingOrigin.Race))
                    {
                        var usedSpellsSlots = spellRepertoire.GetField<RulesetSpellRepertoire, Dictionary<int, int>>("usedSpellsSlots");

                        usedSpellsSlots.TryAdd(slotLevel, 0);
                        usedSpellsSlots[slotLevel]++;
                        spellRepertoire.RepertoireRefreshed?.Invoke(spellRepertoire);
                    }
                }
            }
        }

        //
        // patches exclusive to MC
        //

        // handles all different scenarios to determine max spell level
        [HarmonyPatch(typeof(RulesetSpellRepertoire), "MaxSpellLevelOfSpellCastingLevel", MethodType.Getter)]
        internal static class RulesetSpellRepertoireMaxSpellLevelOfSpellCastingLevelGetter
        {
            internal static void Postfix(RulesetSpellRepertoire __instance, ref int __result)
            {
                // required to ensure we don't learn auto prepared spells from higher levels
                if (SharedSpellsContext.DisableMaxSpellLevelOfSpellCastingLevelPatch)
                {
                    return;
                }

                var heroWithSpellRepertoire = SharedSpellsContext.GetHero(__instance.CharacterName);

                if (heroWithSpellRepertoire == null)
                {
                    return;
                }

                if (SharedSpellsContext.IsSharedcaster(heroWithSpellRepertoire))
                {
                    __result = SharedSpellsContext.GetSharedSpellLevel(heroWithSpellRepertoire);
                }
                else if (SharedSpellsContext.IsWarlock(__instance.SpellCastingClass))
                {
                    __result = Math.Max(
                        SharedSpellsContext.GetSharedSpellLevel(heroWithSpellRepertoire),
                        SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire));
                }
            }
        }

        // handles Arcane Recovery granted spells on short rests
        [HarmonyPatch(typeof(RulesetSpellRepertoire), "RecoverMissingSlots")]
        internal static class RulesetSpellRepertoireRecoverMissingSlots
        {
            internal static bool Prefix(RulesetSpellRepertoire __instance, Dictionary<int, int> recoveredSlots)
            {
                var heroWithSpellRepertoire = SharedSpellsContext.GetHero(__instance.CharacterName);

                if (heroWithSpellRepertoire == null)
                {
                    return true;
                }

                if (!SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire))
                {
                    return true;
                }

                foreach (var spellRepertoire in heroWithSpellRepertoire.SpellRepertoires)
                {
                    var usedSpellsSlots = spellRepertoire.GetField<RulesetSpellRepertoire, Dictionary<int, int>>("usedSpellsSlots");

                    foreach (var recoveredSlot in recoveredSlots)
                    {
                        var key = recoveredSlot.Key;

                        if (usedSpellsSlots.TryGetValue(key, out var used) && used > 0)
                        {
                            usedSpellsSlots[key] = Mathf.Max(0, used - recoveredSlot.Value);
                        }
                    }

                    spellRepertoire.RepertoireRefreshed?.Invoke(__instance);
                }

                return false;
            }
        }
    }
}
