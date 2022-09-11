using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Models;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches;

internal static class RulesetSpellRepertoirePatcher
{
    //
    // TODO: Check if still need this one...
    //
    // ensures MC Warlocks get a proper list of upcast slots under a MC scenario
    //[HarmonyPatch(typeof(RulesetSpellRepertoire), "CanUpcastSpell")]
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

    //PATCH: handles all different scenarios of spell slots consumption (casts, smites, point buys)
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
                return true;
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

            for (var i = -1; i <= warlockSpellLevel; i++)
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

            warlockSpellRepertoire.GetSlotsNumber(slotLevel, out var sharedRemainingSlots, out _);

            sharedRemainingSlots -= pactRemainingSlots;

            var isShiftPressed = !Global.IsMultiplayer &&
                                 (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));

            var canConsumePactSlot = pactRemainingSlots > 0 && slotLevel <= warlockSpellLevel;
            var canConsumeSpellSlot = sharedRemainingSlots > 0 && slotLevel <= sharedSpellLevel;

            var forcePactSlot = __instance.SpellCastingClass == DatabaseHelper.CharacterClassDefinitions.Warlock;
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

    //PATCH: handles all different scenarios to determine max spell level
    [HarmonyPatch(typeof(RulesetSpellRepertoire), "MaxSpellLevelOfSpellCastingLevel", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetSpellRepertoire_MaxSpellLevelOfSpellCastingLevel_Getter
    {
        internal static void Postfix(RulesetSpellRepertoire __instance, ref int __result)
        {
            var heroWithSpellRepertoire = SharedSpellsContext.GetHero(__instance.CharacterName);

            if (heroWithSpellRepertoire == null)
            {
                return;
            }

            if (LevelUpContext.IsLevelingUp(heroWithSpellRepertoire))
            {
                __result = SharedSpellsContext.GetClassSpellLevel(__instance);
            }
            else
            {
                __result = Math.Max(
                    SharedSpellsContext.GetSharedSpellLevel(heroWithSpellRepertoire),
                    SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire));
            }
        }
    }

    //PATCH: handles Arcane Recovery granted spells on short rests
    [HarmonyPatch(typeof(RulesetSpellRepertoire), "RecoverMissingSlots")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetSpellRepertoire_RecoverMissingSlots
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
                var usedSpellsSlots =
                    spellRepertoire.usedSpellsSlots;

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
