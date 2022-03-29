using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using static SolastaCommunityExpansion.Classes.Warlock.Warlock;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.PactMagic
{
    //
    // this is a Warlock only patch. Multiclass doesn't handle this case. Ensures we don't offer Mystic Arcanum slots on upcasting
    //
    [HarmonyPatch(typeof(RulesetSpellRepertoire), "CanUpcastSpell")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetSpellRepertoire_CanUpcastSpell_Patch
    {
        internal static void Postfix(
            RulesetSpellRepertoire __instance,
            ref bool __result,
            SpellDefinition spellDefinition, 
            List<int> availableSlotLevels)
        {
            if (__instance.SpellCastingClass != ClassWarlock || !__result)
            {
                return;
            }

            availableSlotLevels?.RemoveAll(s => s >= MYSTIC_ARCANUM_SPELL_LEVEL);

            if (spellDefinition.SpellLevel + 1 < MYSTIC_ARCANUM_SPELL_LEVEL)
            {
                __instance.GetSlotsNumber(1, out var remaining, out var max);
                __result = remaining > 0;
            }
            else
            {
                __result = false;
            }
        }
    }

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
                // PACT MAGIC
                if (spellLevel < MYSTIC_ARCANUM_SPELL_LEVEL)
                {
                    ___spellsSlotCapacities.TryGetValue(1, out max);
                    ___usedSpellsSlots.TryGetValue(-1, out var used);
                    remaining = max - used;
                }
                // MYSTIC ARCANUM
                else
                {
                    max = 1;
                    ___usedSpellsSlots.TryGetValue(spellLevel, out var used);
                    remaining = max - used;
                }
            }

            return false;
        }
    }

    // ensure all slot levels are consumed on Warlock
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

            // PACT MAGIC: consumes a slot from each level 1 to 5
            if (slotLevel < MYSTIC_ARCANUM_SPELL_LEVEL)
            {
                var limit = Math.Min(MYSTIC_ARCANUM_SPELL_LEVEL - 1, warlockSpellLevel);

                for (var i = 1; i <= limit; i++)
                {
                    if (!___usedSpellsSlots.ContainsKey(i))
                    {
                        ___usedSpellsSlots.Add(i, 0);
                    }

                    ___usedSpellsSlots[i]++;
                }
            }
            // MYSTIC ARCANUM: consumes a slot from each level 6 to 9
            else
            {
                for (var i = MYSTIC_ARCANUM_SPELL_LEVEL; i <= warlockSpellLevel; i++)
                {
                    if (!___usedSpellsSlots.ContainsKey(i))
                    {
                        ___usedSpellsSlots.Add(i, 0);
                    }

                    ___usedSpellsSlots[i]++;
                }
            }

            return false;
        }
    }
}
