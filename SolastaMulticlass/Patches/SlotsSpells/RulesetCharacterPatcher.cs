using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaModApi.Infrastructure;
using SolastaMulticlass.Models;
using static SolastaCommunityExpansion.Level20.SpellsHelper;

namespace SolastaMulticlass.Patches.SlotsSpells
{
    internal static class RulesetCharacterPatcher
    {
        [HarmonyPatch(typeof(RulesetCharacter), "ApplyRest")]
        internal static class RulesetCharacterApplyRest
        {
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var restoreAllSpellSlotsMethod = typeof(RulesetSpellRepertoire).GetMethod("RestoreAllSpellSlots");
                var myRestoreAllSpellSlotsMethod = typeof(RulesetCharacterApplyRest).GetMethod("RestoreAllSpellSlots");

                var code = instructions.ToList();
                var index = code.FindIndex(x => x.Calls(restoreAllSpellSlotsMethod));

                // final sequence is ldarg_0, ldarg_1, call
                code[index] = new CodeInstruction(OpCodes.Call, myRestoreAllSpellSlotsMethod);
                code.Insert(index, new CodeInstruction(OpCodes.Ldarg_1)); // restType
                code.Insert(index, new CodeInstruction(OpCodes.Ldarg_0)); // rulesetCharacter

                return code;
            }

            public static void RestoreAllSpellSlots(RulesetSpellRepertoire __instance, RulesetCharacter rulesetCharacter, RuleDefinitions.RestType restType)
            {
                if (restType == RuleDefinitions.RestType.LongRest
                    || rulesetCharacter is not RulesetCharacterHero heroWithSpellRepertoire)
                {
                    rulesetCharacter.RestoreAllSpellSlots();

                    return;
                }

                var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);

                __instance.GetField<RulesetSpellRepertoire, Dictionary<int, int>>("usedSpellsSlots")
                    .TryGetValue(SharedSpellsContext.PACT_MAGIC_SLOT_TAB_INDEX, out var slotsToRestore);

                foreach (var spellRepertoire in heroWithSpellRepertoire.SpellRepertoires
                    .Where(x => x.SpellCastingRace == null))
                {
                    var usedSpellsSlots = spellRepertoire.GetField<RulesetSpellRepertoire, Dictionary<int, int>>("usedSpellsSlots");

                    for (var i = SharedSpellsContext.PACT_MAGIC_SLOT_TAB_INDEX; i <= warlockSpellLevel; i++)
                    {
                        if (i == 0)
                        {
                            continue;
                        }

                        if (usedSpellsSlots.ContainsKey(i))
                        {
                            usedSpellsSlots[i] -= slotsToRestore;
                        }
                    }

                    spellRepertoire.RepertoireRefreshed?.Invoke(spellRepertoire);
                }
            }
        }

        // logic to correctly offer / calculate spell slots on all different scenarios
        [HarmonyPatch(typeof(RulesetCharacter), "RefreshSpellRepertoires")]
        internal static class RulesetCharacterRefreshSpellRepertoires
        {
            private static readonly Dictionary<int, int> affinityProviderAdditionalSlots = new();

            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var computeSpellSlotsMethod = typeof(RulesetSpellRepertoire).GetMethod("ComputeSpellSlots");
                var myComputeSpellSlotsMethod = typeof(RulesetCharacterRefreshSpellRepertoires).GetMethod("ComputeSpellSlots");
                var finishRepertoiresRefreshMethod = typeof(RulesetCharacterRefreshSpellRepertoires).GetMethod("FinishRepertoiresRefresh");

                var code = instructions.ToList();
                var index = code.FindIndex(x => x.Calls(computeSpellSlotsMethod));

                // final sequence is pop, call
                code[index] = new CodeInstruction(OpCodes.Call, myComputeSpellSlotsMethod);
                code.Insert(index, new CodeInstruction(OpCodes.Pop));

                index = code.FindIndex(x => x.opcode == OpCodes.Brtrue_S);

                // final sequence is original, ldarg_0, call
                code.Insert(index, new CodeInstruction(OpCodes.Call, finishRepertoiresRefreshMethod));
                code.Insert(index, new CodeInstruction(OpCodes.Ldarg_0));

                return code;
            }

            public static void ComputeSpellSlots(RulesetSpellRepertoire spellRepertoire)
            {
                // will calculate additional slots from features later
                spellRepertoire.ComputeSpellSlots(null);
            }

            public static void FinishRepertoiresRefresh(RulesetCharacter rulesetCharacter)
            {
                if (rulesetCharacter is not RulesetCharacterHero heroWithSpellRepertoire)
                {
                    return;
                }

                // calculates additional slots from features
                affinityProviderAdditionalSlots.Clear();

                foreach (var spellCastingAffinityProvider in rulesetCharacter.FeaturesToBrowse.OfType<ISpellCastingAffinityProvider>())
                {
                    foreach (var additionalSlot in spellCastingAffinityProvider.AdditionalSlots)
                    {
                        var slotLevel = additionalSlot.SlotLevel;

                        affinityProviderAdditionalSlots.TryAdd(slotLevel, 0);
                        affinityProviderAdditionalSlots[slotLevel] += additionalSlot.SlotsNumber;
                    }
                }

                // calculates shared slots system across all repertoires except for Race and Warlock
                var isSharedCaster = SharedSpellsContext.IsSharedcaster(heroWithSpellRepertoire);

                foreach (var spellRepertoire in heroWithSpellRepertoire.SpellRepertoires
                    .Where(x => x.SpellCastingRace == null && x.SpellCastingClass != IntegrationContext.WarlockClass))
                {
                    var spellsSlotCapacities = spellRepertoire.GetField<RulesetSpellRepertoire, Dictionary<int, int>>("spellsSlotCapacities");

                    // replaces standard caster slots with shared slots system
                    if (isSharedCaster)
                    {
                        var sharedCasterLevel = SharedSpellsContext.GetSharedCasterLevel(heroWithSpellRepertoire);
                        var sharedSpellLevel = SharedSpellsContext.GetSharedSpellLevel(heroWithSpellRepertoire);

                        spellsSlotCapacities.Clear();

                        // adds shared slots
                        for (var i = 1; i <= sharedSpellLevel; i++)
                        {
                            spellsSlotCapacities[i] = FullCastingSlots[sharedCasterLevel - 1].Slots[i - 1];
                        }
                    }

                    // adds affinity provider slots collected in my custom compute spell slots
                    foreach (var slot in affinityProviderAdditionalSlots)
                    {
                        spellsSlotCapacities.TryAdd(slot.Key, 0);
                        spellsSlotCapacities[slot.Key] += slot.Value;
                    }

                    spellRepertoire.RepertoireRefreshed?.Invoke(spellRepertoire);
                }

                // collects warlock and non warlock repertoires for consolidation
                var warlockRepertoire = SharedSpellsContext.GetWarlockSpellRepertoire(heroWithSpellRepertoire);
                var anySharedRepertoire = heroWithSpellRepertoire.SpellRepertoires.Find(sr => !SharedSpellsContext.IsWarlock(sr.SpellCastingClass) &&
                    (sr.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Class || sr.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Subclass));

                // combines the Shared Slot System and Warlock Pact Magic
                if (warlockRepertoire != null && anySharedRepertoire != null)
                {
                    var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);
                    var warlockSlotsCapacities = warlockRepertoire.GetField<RulesetSpellRepertoire, Dictionary<int, int>>("spellsSlotCapacities");
                    var anySharedSlotsCapacities = anySharedRepertoire.GetField<RulesetSpellRepertoire, Dictionary<int, int>>("spellsSlotCapacities");

                    // first consolidates under Warlock repertoire
                    for (var i = 1; i <= Math.Max(warlockSlotsCapacities.Count, anySharedSlotsCapacities.Count); i++)
                    {
                        warlockSlotsCapacities.TryAdd(i, 0);

                        if (anySharedSlotsCapacities.ContainsKey(i))
                        {
                            warlockSlotsCapacities[i] += anySharedSlotsCapacities[i];
                        }
                    }

                    // then copy over Warlock repertoire to all others
                    foreach (var spellRepertoire in heroWithSpellRepertoire.SpellRepertoires
                        .Where(x => x.SpellCastingRace == null && x.SpellCastingClass != IntegrationContext.WarlockClass))
                    {
                        var spellsSlotCapacities = spellRepertoire.GetField<RulesetSpellRepertoire, Dictionary<int, int>>("spellsSlotCapacities");

                        spellsSlotCapacities.Clear();

                        foreach (var warlockSlotCapacities in warlockSlotsCapacities)
                        {
                            spellsSlotCapacities[warlockSlotCapacities.Key] = warlockSlotCapacities.Value;
                        }

                        spellRepertoire.RepertoireRefreshed?.Invoke(spellRepertoire);
                    }

                    warlockRepertoire?.RepertoireRefreshed?.Invoke(warlockRepertoire);
                }
            }
        }
    }
}
