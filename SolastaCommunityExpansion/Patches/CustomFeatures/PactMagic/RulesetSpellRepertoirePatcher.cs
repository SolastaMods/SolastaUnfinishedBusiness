using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using static SolastaCommunityExpansion.Classes.Warlock.Warlock;
using static SolastaCommunityExpansion.Classes.Warlock.WarlockSpells;

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
            if (Main.Settings.EnableMulticlass || __instance.SpellCastingClass != ClassWarlock)
            {
                return true;
            }

            ___spellsSlotCapacities.TryGetValue(PACT_MAGIC_SLOT_TAB_INDEX, out __result);

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
            if (Main.Settings.EnableMulticlass || __instance.SpellCastingClass != ClassWarlock)
            {
                return true;
            }

            ___spellsSlotCapacities.TryGetValue(PACT_MAGIC_SLOT_TAB_INDEX, out var max);
            ___usedSpellsSlots.TryGetValue(PACT_MAGIC_SLOT_TAB_INDEX, out var used);
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
            if (Main.Settings.EnableMulticlass || __instance.SpellCastingClass != ClassWarlock)
            {
                return true;
            }

            max = 0;
            remaining = 0;

            if (spellLevel <= __instance.MaxSpellLevelOfSpellCastingLevel)
            {
                ___spellsSlotCapacities.TryGetValue(PACT_MAGIC_SLOT_TAB_INDEX, out max);
                ___usedSpellsSlots.TryGetValue(PACT_MAGIC_SLOT_TAB_INDEX, out var used);
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
            if (Main.Settings.EnableMulticlass || slotLevel == 0 || __instance.SpellCastingClass != ClassWarlock)
            {
                return true;
            }

            var warlockSpellLevel = __instance.MaxSpellLevelOfSpellCastingLevel;

            for (var i = PACT_MAGIC_SLOT_TAB_INDEX; i <= warlockSpellLevel; i++)
            {
                ___usedSpellsSlots.TryAdd(i, 0);
                ___usedSpellsSlots[i]++;
            }

            return false;
        }
    }
}
