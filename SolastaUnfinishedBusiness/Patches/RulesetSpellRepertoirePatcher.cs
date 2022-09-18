using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

internal static class RulesetSpellRepertoirePatcher
{
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

            // var isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            var isShiftPressed = Global.CurrentAction is CharacterActionCastSpell &&
                                 Global.CurrentAction.actionParams.BoolParameter5;
            var canConsumePactSlot = pactRemainingSlots > 0 && slotLevel <= warlockSpellLevel;
            var canConsumeSpellSlot = sharedRemainingSlots > 0 && slotLevel <= sharedSpellLevel;

            // var forcePactSlot = __instance.SpellCastingClass == DatabaseHelper.CharacterClassDefinitions.Warlock;
            // var forceSpellSlot = canConsumeSpellSlot &&
            //                      (isShiftPressed || (!forcePactSlot && sharedSpellLevel < warlockSpellLevel));

            // uses short rest slots across all non race repertoires
            if (canConsumePactSlot &&
                (!canConsumeSpellSlot
                 || (__instance.SpellCastingClass == DatabaseHelper.CharacterClassDefinitions.Warlock &&
                     !isShiftPressed)
                 || (__instance.SpellCastingClass != DatabaseHelper.CharacterClassDefinitions.Warlock &&
                     isShiftPressed)))
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
                return;
            }

            __result = Math.Max(
                SharedSpellsContext.GetSharedSpellLevel(heroWithSpellRepertoire),
                SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire));
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
