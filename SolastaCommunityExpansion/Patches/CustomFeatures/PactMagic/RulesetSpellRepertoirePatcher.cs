using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using static SolastaCommunityExpansion.Classes.Warlock.Warlock;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.PactMagic
{
    // use slot 1 to keep a tab on Warlock slots
    [HarmonyPatch(typeof(RulesetSpellRepertoire), "GetMaxSlotsNumberOfAllLevels")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetSpellRepertoire_GetMaxSlotsNumberOfAllLevels
    {
        internal static bool Prefix(
            RulesetSpellRepertoire __instance,
            ref int __result,
            Dictionary<int, int> ___spellsSlotCapacities)
        {
            if (Main.IsMulticlassInstalled || __instance.SpellCastingClass != ClassWarlock)
            {
                return true;
            }

            ___spellsSlotCapacities.TryGetValue(1, out __result);

            return false;
        }
    }

    // use slot 1 to keep a tab on Warlock slots
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
            if (Main.IsMulticlassInstalled || __instance.SpellCastingClass != ClassWarlock)
            {
                return true;
            }

            ___spellsSlotCapacities.TryGetValue(1, out var max);
            ___usedSpellsSlots.TryGetValue(1, out var used);
            __result = max - used;

            return false;
        }
    }

    // use slot 1 to keep a tab on Warlock slots
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
            if (Main.IsMulticlassInstalled || __instance.SpellCastingClass != ClassWarlock)
            {
                return true;
            }

            max = 0;
            remaining = 0;

            if (spellLevel <= __instance.MaxSpellLevelOfSpellCastingLevel)
            {
                ___spellsSlotCapacities.TryGetValue(1, out max);
                ___usedSpellsSlots.TryGetValue(-1, out var used);
                remaining = max - used;
            }

            return false;
        }
    }

    // ensures all slot levels are consumed on Warlock Pact Magic / Mystic Arcanum scenarios
    [HarmonyPatch(typeof(RulesetSpellRepertoire), "SpendSpellSlot")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetSpellRepertoire_SpendSpellSlot
    {
        internal static bool Prefix(RulesetSpellRepertoire __instance, Dictionary<int, int> ___usedSpellsSlots, int slotLevel)
        {
            if (Main.IsMulticlassInstalled || slotLevel == 0 || __instance.SpellCastingClass != ClassWarlock)
            {
                return true;
            }

            var warlockSpellLevel = __instance.MaxSpellLevelOfSpellCastingLevel;

            for (var i = 1; i <= warlockSpellLevel; i++)
            {
                if (!___usedSpellsSlots.ContainsKey(i))
                {
                    ___usedSpellsSlots.Add(i, 0);
                }

                ___usedSpellsSlots[i]++;
            }

            return false;
        }
    }
}
