using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.Multiclass.SlotsSpells
{
    // handles all different scenarios to determine max spell level
    [HarmonyPatch(typeof(RulesetSpellRepertoire), "MaxSpellLevelOfSpellCastingLevel", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetSpellRepertoire_MaxSpellLevelOfSpellCastingLevel_Getter
    {
        internal static void Postfix(RulesetSpellRepertoire __instance, ref int __result)
        {
            // required to ensure we don't learn auto prepared spells from higher levels
            if (SharedSpellsContext.DisableMaxSpellLevelOfSpellCastingLevelPatch)
            {
                return;
            }

            //
            // required to support level up from 19 to 20 on SC caster classes
            //
            if (Main.Settings.EnableLevel20)
            {
                var slotsPerLevel = __instance.SpellCastingFeature.SlotsPerLevels[__instance.SpellCastingLevel - 1];

                __result = slotsPerLevel.Slots.IndexOf(0);
            }

            //
            // handles MC scenarios
            //
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
