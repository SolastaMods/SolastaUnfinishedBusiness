using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Classes.Warlock;
using SolastaCommunityExpansion.Models;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.PactMagic;

internal static class RulesetSpellRepertoirePatcher
{
    private static bool ShouldNotRun(RulesetSpellRepertoire __instance)
    {
        var heroWithSpellRepertoire = SharedSpellsContext.GetHero(__instance.CharacterName);

        return heroWithSpellRepertoire == null
               || SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire)
               || !SharedSpellsContext.IsWarlock(__instance.SpellCastingClass);
    }

    // ensures MC Warlocks are treated before SC ones
    [HarmonyPatch(typeof(RulesetSpellRepertoire), "GetMaxSlotsNumberOfAllLevels")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetSpellRepertoire_GetMaxSlotsNumberOfAllLevels
    {
        internal static bool Prefix(
            RulesetSpellRepertoire __instance,
            ref int __result)
        {
            if (ShouldNotRun(__instance))
            {
                return true;
            }

            // handles SC Warlock
            __instance.spellsSlotCapacities.TryGetValue(1, out __result);

            return false;
        }
    }

    // ensures MC Warlocks get a proper list of upcast slots under a MC scenario
    [HarmonyPatch(typeof(RulesetSpellRepertoire), "CanUpcastSpell")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetSpellRepertoire_CanUpcastSpell
    {
        public static int MySpellLevel(SpellDefinition spellDefinition,
            RulesetSpellRepertoire rulesetSpellRepertoire)
        {
            var isWarlockSpell = SharedSpellsContext.IsWarlock(rulesetSpellRepertoire.SpellCastingClass);

            if (!isWarlockSpell || spellDefinition.SpellLevel <= 0)
            {
                return spellDefinition.SpellLevel;
            }

            var hero = SharedSpellsContext.GetHero(rulesetSpellRepertoire.CharacterName);
            var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(hero);

            return warlockSpellLevel;
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var spellLevelMethod = typeof(SpellDefinition).GetMethod("get_SpellLevel");
            var mySpellLevelMethod = typeof(RulesetSpellRepertoire_CanUpcastSpell).GetMethod("MySpellLevel");

            foreach (var instruction in instructions)
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
            ref int __result)
        {
            if (ShouldNotRun(__instance))
            {
                return true;
            }

            // handles SC Warlock
            __instance.spellsSlotCapacities.TryGetValue(1, out var max);
            __instance.usedSpellsSlots.TryGetValue(1, out var used);
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
                __instance.spellsSlotCapacities.TryGetValue(1, out max);
                __instance.usedSpellsSlots.TryGetValue(1, out var used);
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
                if (!SharedSpellsContext.IsWarlock(__instance.SpellCastingClass))
                {
                    return true;
                }

                SpendWarlockSlots(__instance, heroWithSpellRepertoire);

                return false;

                // handles SC non-Warlock
            }

            var warlockSpellRepertoire = SharedSpellsContext.GetWarlockSpellRepertoire(heroWithSpellRepertoire);

            // handles MC non-Warlock
            if (warlockSpellRepertoire == null)
            {
                foreach (var spellRepertoire in heroWithSpellRepertoire.SpellRepertoires
                             .Where(x => x.SpellCastingRace == null))
                {
                    var usedSpellsSlots =
                        spellRepertoire.usedSpellsSlots;

                    usedSpellsSlots.TryAdd(slotLevel, 0);
                    usedSpellsSlots[slotLevel]++;
                    spellRepertoire.RepertoireRefreshed?.Invoke(spellRepertoire);
                }
            }

            // handles MC Warlock
            else
            {
                SpendMulticasterWarlockSlots(__instance, warlockSpellRepertoire, heroWithSpellRepertoire,
                    slotLevel);
            }

            return false;
        }

        private static void SpendWarlockSlots(RulesetSpellRepertoire rulesetSpellRepertoire,
            RulesetCharacterHero heroWithSpellRepertoire)
        {
            var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);
            var usedSpellsSlots =
                rulesetSpellRepertoire.usedSpellsSlots;

            for (var i = WarlockSpells.PactMagicSlotTabIndex; i <= warlockSpellLevel; i++)
            {
                // don't mess with cantrips
                if (i == 0)
                {
                    continue;
                }

                usedSpellsSlots.TryAdd(i, 0);
                usedSpellsSlots[i]++;
            }

            rulesetSpellRepertoire.RepertoireRefreshed?.Invoke(rulesetSpellRepertoire);
        }

        private static void SpendMulticasterWarlockSlots(
            RulesetSpellRepertoire __instance,
            RulesetSpellRepertoire warlockSpellRepertoire,
            RulesetCharacterHero heroWithSpellRepertoire,
            int slotLevel)
        {
            var sharedSpellLevel = SharedSpellsContext.GetSharedSpellLevel(heroWithSpellRepertoire);
            var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);

            var pactMaxSlots = SharedSpellsContext.GetWarlockMaxSlots(heroWithSpellRepertoire);
            var usedPactSlots = SharedSpellsContext.GetWarlockUsedSlots(heroWithSpellRepertoire);
            var pactRemainingSlots = pactMaxSlots - usedPactSlots;

            warlockSpellRepertoire.GetSlotsNumber(slotLevel, out var sharedRemainingSlots, out var sharedMaxSlots);

            sharedMaxSlots -= pactMaxSlots;
            sharedRemainingSlots -= pactRemainingSlots;

            var isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            var canConsumePactSlot = pactRemainingSlots > 0 && slotLevel <= warlockSpellLevel;
            var canConsumeSpellSlot = sharedRemainingSlots > 0 && slotLevel <= sharedSpellLevel;

            var forcePactSlot = __instance.SpellCastingClass == IntegrationContext.WarlockClass;
            var forceSpellSlot = canConsumeSpellSlot &&
                                 (isShiftPressed || (!forcePactSlot && sharedSpellLevel < warlockSpellLevel));

            // uses short rest slots across all repertoires
            if (canConsumePactSlot && !forceSpellSlot)
            {
                foreach (var spellRepertoire in heroWithSpellRepertoire.SpellRepertoires
                             .Where(spellRepertoire => spellRepertoire.SpellCastingFeature.SpellCastingOrigin !=
                                                       FeatureDefinitionCastSpell.CastingOrigin.Race))
                {
                    SpendWarlockSlots(spellRepertoire, heroWithSpellRepertoire);
                }
            }

            // otherwise uses long rest slots across all non race repertoires
            else
            {
                foreach (var spellRepertoire in heroWithSpellRepertoire.SpellRepertoires
                             .Where(x => x.SpellCastingFeature.SpellCastingOrigin !=
                                         FeatureDefinitionCastSpell.CastingOrigin.Race))
                {
                    var usedSpellsSlots =
                        spellRepertoire.usedSpellsSlots;

                    usedSpellsSlots.TryAdd(slotLevel, 0);
                    usedSpellsSlots[slotLevel]++;
                    spellRepertoire.RepertoireRefreshed?.Invoke(spellRepertoire);
                }
            }
        }
    }
}
