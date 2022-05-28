using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.SharedSpells
{
    // handles all different scenarios to determine max spell level
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
