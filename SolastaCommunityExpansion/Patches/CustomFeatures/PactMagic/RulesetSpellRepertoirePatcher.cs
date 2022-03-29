using System.Collections.Generic;
using HarmonyLib;
using static SolastaCommunityExpansion.Classes.Warlock.Warlock;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.PactMagic
{
    public static class RulesetSpellRepertoirePatcher
    {
        //
        // TODO: move this over to a proper place once merge is done (otherwise it'll be a pain to merge settings or main...)
        //
        public static bool IsMulticlassInstalled { get; set; }

        // use slot 1 to keep a tab on Warlock slots
        [HarmonyPatch(typeof(RulesetSpellRepertoire), "GetMaxSlotsNumberOfAllLevels")]
        internal static class RulesetSpellRepertoireGetMaxSlotsNumberOfAllLevels
        {
            internal static bool Prefix(
                RulesetSpellRepertoire __instance,
                ref int __result,
                Dictionary<int, int> ___spellsSlotCapacities)
            {
                if (IsMulticlassInstalled || __instance.SpellCastingClass != ClassWarlock)
                {
                    return true;
                }

                ___spellsSlotCapacities.TryGetValue(1, out __result);

                return false;
            }
        }

        // use slot 1 to keep a tab on Warlock slots
        [HarmonyPatch(typeof(RulesetSpellRepertoire), "GetRemainingSlotsNumberOfAllLevels")]
        internal static class RulesetSpellRepertoireGetRemainingSlotsNumberOfAllLevels
        {
            internal static bool Prefix(
                RulesetSpellRepertoire __instance, 
                ref int __result, 
                Dictionary<int, int> ___usedSpellsSlots,
                Dictionary<int, int> ___spellsSlotCapacities)
            {
                if (IsMulticlassInstalled || __instance.SpellCastingClass != ClassWarlock)
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
                if (IsMulticlassInstalled || __instance.SpellCastingClass != ClassWarlock)
                {
                    return true;
                }

                max = 0;
                remaining = 0;

                if (spellLevel <= __instance.MaxSpellLevelOfSpellCastingLevel)
                {
                    ___spellsSlotCapacities.TryGetValue(1, out max);
                    ___usedSpellsSlots.TryGetValue(1, out var used);
                    remaining = max - used;
                }

                return false;
            }
        }
 
        // ensure all slot levels are consumed on Warlock
        [HarmonyPatch(typeof(RulesetSpellRepertoire), "SpendSpellSlot")]
        internal static class RulesetSpellRepertoireSpendSpellSlot
        {
            internal static bool Prefix(RulesetSpellRepertoire __instance, Dictionary<int, int> ___usedSpellsSlots, int slotLevel)
            {
                if (IsMulticlassInstalled || slotLevel == 0 || __instance.SpellCastingClass != ClassWarlock)
                {
                    return true;
                }

                var maxSpellLevel = __instance.MaxSpellLevelOfSpellCastingLevel;

                for (var i = 1; i <= maxSpellLevel; i++)
                {
                    if (i == 0)
                    {
                        continue;
                    }

                    if (!___usedSpellsSlots.ContainsKey(i))
                    {
                    ___usedSpellsSlots.Add(i, 0);
                    }

                    ___usedSpellsSlots[i]++;
                }
                
                __instance.RepertoireRefreshed?.Invoke(__instance);

                return false;
            }
        }
    }
}
