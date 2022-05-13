using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Classes.Warlock;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.PactMagic
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
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class RulesetSpellRepertoire_GetMaxSlotsNumberOfAllLevels
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

        // ensures MC Warlocks get a proper list of upcast slots under a MC scenario
        [HarmonyPatch(typeof(RulesetSpellRepertoire), "CanUpcastSpell")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class RulesetSpellRepertoire_CanUpcastSpell
        {
            public static int MySpellLevel(SpellDefinition spellDefinition, RulesetSpellRepertoire rulesetSpellRepertoire)
            {
                var isWarlockSpell = SharedSpellsContext.IsWarlock(rulesetSpellRepertoire.SpellCastingClass);

                if (isWarlockSpell && spellDefinition.SpellLevel > 0)
                {
                    var hero = SharedSpellsContext.GetHero(rulesetSpellRepertoire.CharacterName);
                    var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(hero);

                    return warlockSpellLevel;
                }

                return spellDefinition.SpellLevel;
            }

            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var spellLevelMethod = typeof(SpellDefinition).GetMethod("get_SpellLevel");
                var mySpellLevelMethod = typeof(RulesetSpellRepertoire_CanUpcastSpell).GetMethod("MySpellLevel");

                foreach (CodeInstruction instruction in instructions)
                {
                    if (instruction.Calls(spellLevelMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0); // spellRepertoire
                        yield return new CodeInstruction(OpCodes.Call, mySpellLevelMethod);
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }
        }

        // ensures MC Warlocks are treated before SC ones
        [HarmonyPatch(typeof(RulesetSpellRepertoire), "GetRemainingSlotsNumberOfAllLevels")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class RulesetSpellRepertoire_GetRemainingSlotsNumberOfAllLevels
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
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class RulesetSpellRepertoire_GetSlotsNumber
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
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class RulesetSpellRepertoire_SpendSpellSlot
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
                    SpendMulticasterWarlockSlots(__instance, warlockSpellRepertoire, heroWithSpellRepertoire, slotLevel);
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

            private static void SpendMulticasterWarlockSlots(RulesetSpellRepertoire __instance, RulesetSpellRepertoire warlockSpellRepertoire, RulesetCharacterHero heroWithSpellRepertoire, int slotLevel)
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

                var isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                var canConsumePactSlot = warlockUsedSlots < warlockMaxSlots && slotLevel <= warlockSpellLevel;
                var canConsumeSpellSlot = sharedUsedSlots < sharedMaxSlots && slotLevel <= sharedSpellLevel;
                var forcePactSlot = __instance.SpellCastingClass == IntegrationContext.WarlockClass;
                var forceSpellSlot = canConsumeSpellSlot && (isShiftPressed || (!forcePactSlot && sharedSpellLevel < warlockSpellLevel));

                // uses short rest slots across all repertoires
                if (canConsumePactSlot && !forceSpellSlot)
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
    }
}
