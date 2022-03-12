using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Multiclass.Patches.SharedCombinedSpells
{
    internal static class RulesetCharacterPatcher
    {

        // only need this patch in case we need to support Warlock Pact Magic
#if false
        [HarmonyPatch(typeof(RulesetCharacter), "ApplyRest")]
        internal static class RulesetCharacterApplyRest
        {
            internal static void Prefix(RuleDefinitions.RestType restType)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                Models.SharedSpellsContext.RestType = restType;

                // keep this around in case we ever need to support ModHelpers again
#if false
                //
                // default game code from here. had to use a bool Prefix here to bypass ModHelpers transpiler on this method...
                //

                __instance.RecoveredFeatures.Clear();

                foreach (var usablePower in __instance.UsablePowers)
                {
                    if (((usablePower.PowerDefinition.RechargeRate == RuleDefinitions.RechargeRate.ShortRest && (restType == RuleDefinitions.RestType.ShortRest || restType == RuleDefinitions.RestType.LongRest)) || (usablePower.PowerDefinition.RechargeRate == RuleDefinitions.RechargeRate.LongRest && restType == RuleDefinitions.RestType.LongRest)) && usablePower.RemainingUses < usablePower.MaxUses)
                    {
                        if (!simulate)
                        {
                            usablePower.Recharge();
                        }

                        __instance.RecoveredFeatures.Add(usablePower.PowerDefinition);
                    }
                }

                if (__instance.TryGetAttributeValue(AttributeDefinitions.ChannelDivinityNumber) > 0 && __instance.UsedChannelDivinity > 0)
                {
                    if (!simulate)
                    {
                        __instance.SetField("usedChannelDivinity", 0);
                    }

                    __instance.EnumerateFeaturesToBrowse<FeatureDefinitionAttributeModifier>(__instance.FeaturesToBrowse);

                    foreach (var featureDefinition in __instance.FeaturesToBrowse)
                    {
                        var attributeModifier = featureDefinition as FeatureDefinitionAttributeModifier;

                        if (attributeModifier.ModifiedAttribute == AttributeDefinitions.ChannelDivinityNumber)
                        {
                            __instance.RecoveredFeatures.Add(attributeModifier);
                            break;
                        }
                    }
                }

                if (restType == RuleDefinitions.RestType.LongRest && __instance.TryGetAttributeValue(AttributeDefinitions.HealingPool) > 0 && (__instance.UsedHealingPool > 0 && !simulate))
                {
                    __instance.SetField("usedHealingPool", 0);
                }

                if (restType == RuleDefinitions.RestType.LongRest && __instance.TryGetAttributeValue(AttributeDefinitions.SorceryPoints) > 0 && (__instance.UsedSorceryPoints > 0 && !simulate))
                {
                    __instance.SetField("usedSorceryPoints", 0);
                }

                if (restType == RuleDefinitions.RestType.LongRest && __instance.TryGetAttributeValue(AttributeDefinitions.RagePoints) > 0 && (__instance.UsedRagePoints > 0 && !simulate))
                {
                    __instance.SetField("usedRagePoints", 0);
                }

                if (restType == RuleDefinitions.RestType.LongRest && __instance.TryGetAttributeValue(AttributeDefinitions.IndomitableResistances) > 0 && (__instance.UsedIndomitableResistances > 0 && !simulate))
                {
                    __instance.SetField("usedIndomitableResistances", 0);
                }

                if (!simulate && __instance.TryGetAttribute(AttributeDefinitions.RelentlessRageDC, out var rulesetAttribute))
                {
                    rulesetAttribute.RemoveModifiersByTags(AttributeDefinitions.TagHealth);
                    rulesetAttribute.Refresh();
                }

                if (!simulate && __instance.TryGetAttribute(AttributeDefinitions.FrenzyExhaustionDC, out var rulesetAttribute1))
                {
                    rulesetAttribute1.RemoveModifiersByTags(AttributeDefinitions.TagHealth);
                    rulesetAttribute1.Refresh();
                }

                foreach (RulesetSpellRepertoire spellRepertoire in __instance.SpellRepertoires)
                {
                    if ((spellRepertoire.SpellCastingFeature.SlotsRecharge == RuleDefinitions.RechargeRate.ShortRest && (restType == RuleDefinitions.RestType.ShortRest || restType == RuleDefinitions.RestType.LongRest)) || (spellRepertoire.SpellCastingFeature.SlotsRecharge == RuleDefinitions.RechargeRate.LongRest && restType == RuleDefinitions.RestType.LongRest))
                    {
                        if (!simulate)
                        {
                            spellRepertoire.RestoreAllSpellSlots();
                        }

                        __instance.RecoveredFeatures.Add(spellRepertoire.SpellCastingFeature);
                    }
                }

                var rulesetCharacterType = typeof(RulesetCharacter);

                if (restType == RuleDefinitions.RestType.ShortRest)
                {
                    var applyShortRestMethod = rulesetCharacterType.GetMethod("ApplyShortRest", BindingFlags.NonPublic | BindingFlags.Instance);

                    applyShortRestMethod.Invoke(__instance, new object[] { simulate });
                }
                else if (restType == RuleDefinitions.RestType.LongRest)
                {
                    var applyLongRestMethod = rulesetCharacterType.GetMethod("ApplyLongRest", BindingFlags.NonPublic | BindingFlags.Instance);

                    applyLongRestMethod.Invoke(__instance, new object[] { simulate, restStartTime });
                }

                return false;
#endif
            }
        }
#endif

        // use caster level instead of character level on multiclassed heroes
        [HarmonyPatch(typeof(RulesetCharacter), "GetSpellcastingLevel")]
        internal static class RulesetCharacterGetSpellcastingLevel
        {
            internal static void Postfix(RulesetCharacter __instance, ref int __result, RulesetSpellRepertoire spellRepertoire)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                if (__instance is not RulesetCharacterHero heroWithSpellRepertoire)
                {
                    return;
                }

                if (!Models.SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire))
                {
                    return;
                }

                // NOTE: don't use SpellCastingFeature?. which bypasses Unity object lifetime check
                if (spellRepertoire != null
                    && spellRepertoire.SpellCastingFeature
                    && spellRepertoire.SpellCastingFeature.SpellCastingOrigin != FeatureDefinitionCastSpell.CastingOrigin.Race
                    && spellRepertoire.SpellCastingClass)
                {
                    __result = heroWithSpellRepertoire.ClassesAndLevels[spellRepertoire.SpellCastingClass];
                }
            }
        }

        // ensures ritual spells work correctly when MC
        [HarmonyPatch(typeof(RulesetCharacter), "CanCastAnyRitualSpell")]
        internal static class RulesetCharacterCanCastAnyRitualSpell
        {
            internal static bool Prefix(RulesetCharacter __instance, ref bool __result)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                var canCast = false;

                __instance.EnumerateFeaturesToBrowse<FeatureDefinitionMagicAffinity>(__instance.FeaturesToBrowse);

                foreach (var featureDefinition in __instance.FeaturesToBrowse)
                {
                    if (featureDefinition is FeatureDefinitionMagicAffinity featureDefinitionMagicAffinity && featureDefinitionMagicAffinity.RitualCasting != RuleDefinitions.RitualCasting.None)
                    {
                        var ritualType = featureDefinitionMagicAffinity.RitualCasting;
                        var usableSpells = __instance.GetField<RulesetCharacter, List<SpellDefinition>>("usableSpells");

                        __instance.EnumerateUsableRitualSpells(ritualType, usableSpells);

                        if (usableSpells.Count > 0)
                        {
                            canCast = true;
                            break;
                        }
                    }
                }

                __result = canCast;

                return false;
            }
        }

        // logic to correctly offer / calculate spell slots on all different scenarios
        [HarmonyPatch(typeof(RulesetCharacter), "RefreshSpellRepertoires")]
        internal static class RulesetCharacterRefreshSpellRepertoires
        {
            private static readonly Dictionary<int, int> affinityProviderAdditionalSlots = new();

            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    foreach (var instruction in instructions)
                    {
                        yield return instruction;
                    }

                    yield break;
                }

                var computeSpellSlotsMethod = typeof(RulesetSpellRepertoire).GetMethod("ComputeSpellSlots");
                var myComputeSpellSlotsMethod = typeof(RulesetCharacterRefreshSpellRepertoires).GetMethod("MyComputeSpellSlots");
                var finishRepertoiresRefreshMethod = typeof(RulesetCharacterRefreshSpellRepertoires).GetMethod("FinishRepertoiresRefresh");

                foreach (var instruction in instructions)
                {
                    if (instruction.Calls(computeSpellSlotsMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Call, myComputeSpellSlotsMethod);
                    }
                    else if (instruction.opcode == OpCodes.Brtrue_S)
                    {
                        yield return instruction;
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Call, finishRepertoiresRefreshMethod);
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }

            // custom implementation that keeps a tab on additional spell slots that are granted from ISpellCastingAffinityProvider
            public static void MyComputeSpellSlots(RulesetSpellRepertoire spellRepertoire, List<FeatureDefinition> spellCastingAffinities)
            {
                if (!Models.SharedSpellsContext.IsEnabled || spellCastingAffinities == null)
                {
                    spellRepertoire.ComputeSpellSlots(spellCastingAffinities);

                    return;
                }

                var mySpellCastingAffinities = new List<FeatureDefinition>();

                affinityProviderAdditionalSlots.Clear();
                mySpellCastingAffinities.AddRange(spellCastingAffinities);

                foreach (var spellCastingAffinityProvider in spellCastingAffinities.OfType<ISpellCastingAffinityProvider>())
                {
                    var foundSlots = false;

                    foreach (var additionalSlot in spellCastingAffinityProvider.AdditionalSlots)
                    {
                        var slotLevel = additionalSlot.SlotLevel;

                        foundSlots = true;

                        if (!affinityProviderAdditionalSlots.ContainsKey(slotLevel))
                        {
                            affinityProviderAdditionalSlots[slotLevel] = 0;
                        }

                        affinityProviderAdditionalSlots[slotLevel] += additionalSlot.SlotsNumber;
                    }

                    if (foundSlots)
                    {
                        mySpellCastingAffinities.Remove((FeatureDefinition)spellCastingAffinityProvider);
                    }
                }

                spellRepertoire.ComputeSpellSlots(mySpellCastingAffinities);
            }

            public static void FinishRepertoiresRefresh(RulesetCharacter rulesetCharacter)
            {
                if (!Models.SharedSpellsContext.IsEnabled)
                {
                    return;
                }

                if (rulesetCharacter is not RulesetCharacterHero heroWithSpellRepertoire)
                {
                    return;
                }

                var warlockRepertoire = Models.SharedSpellsContext.GetWarlockSpellRepertoire(heroWithSpellRepertoire);

                foreach (var spellRepertoire in heroWithSpellRepertoire.SpellRepertoires
                    .Where(x => x.SpellCastingFeature.SpellCastingOrigin != FeatureDefinitionCastSpell.CastingOrigin.Race && x != warlockRepertoire))
                {
                    var spellsSlotCapacities = spellRepertoire.GetField<RulesetSpellRepertoire, Dictionary<int, int>>("spellsSlotCapacities");

                    // replaces standard caster slots with shared slots system
                    if (Models.SharedSpellsContext.IsSharedcaster(heroWithSpellRepertoire))
                    {
                        var sharedCasterLevel = Models.SharedSpellsContext.GetSharedCasterLevel(heroWithSpellRepertoire);
                        var spellLevel = Models.SharedSpellsContext.GetSharedSpellLevel(heroWithSpellRepertoire);

                        spellsSlotCapacities.Clear();

                        // adds shared slots
                        for (var i = 1; i <= spellLevel; i++)
                        {
                            spellsSlotCapacities[i] = Models.SharedSpellsContext.FullCastingSlots[sharedCasterLevel - 1].Slots[i - 1];
                        }
                    }

                    // adds affinity provider slots collected in my custom compute spell slots
                    foreach (var slot in affinityProviderAdditionalSlots)
                    {
                        if (!spellsSlotCapacities.ContainsKey(slot.Key))
                        {
                            spellsSlotCapacities[slot.Key] = 0;
                        }

                        spellsSlotCapacities[slot.Key] += slot.Value;
                    }

                    spellRepertoire.RepertoireRefreshed?.Invoke(spellRepertoire);
                }

                // collects any repertoire different than Warlock as they are all the same from a slots count perspective on a Shared Spells scenario
                var anySharedRepertoire = heroWithSpellRepertoire.SpellRepertoires.Find(sr => !Models.SharedSpellsContext.IsWarlock(sr.SpellCastingClass) &&
                    (sr.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Class || sr.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Subclass));

                // combines the Shared Slot System and Warlock Pact Magic
                if (warlockRepertoire != null && anySharedRepertoire != null && Models.SharedSpellsContext.IsCombined)
                {
                    var warlockLevel = Models.SharedSpellsContext.GetWarlockLevel(heroWithSpellRepertoire);
                    var warlockSlotsCapacities = warlockRepertoire.GetField<RulesetSpellRepertoire, Dictionary<int, int>>("spellsSlotCapacities");
                    var anySharedSlotsCapacities = anySharedRepertoire.GetField<RulesetSpellRepertoire, Dictionary<int, int>>("spellsSlotCapacities");

                    for (var i = 1; i <= Math.Max(warlockSlotsCapacities.Count, anySharedSlotsCapacities.Count); i++)
                    {
                        if (!warlockSlotsCapacities.ContainsKey(i))
                        {
                            warlockSlotsCapacities[i] = 0;
                        }

                        if (anySharedSlotsCapacities.ContainsKey(i))
                        {
                            warlockSlotsCapacities[i] += anySharedSlotsCapacities[i];
                        }
                    }

                    foreach (var spellRepertoire in heroWithSpellRepertoire.SpellRepertoires
                        .Where(x => x.SpellCastingFeature.SpellCastingOrigin != FeatureDefinitionCastSpell.CastingOrigin.Race && x != warlockRepertoire))
                    {
                        var spellsSlotCapacities = spellRepertoire.GetField<RulesetSpellRepertoire, Dictionary<int, int>>("spellsSlotCapacities");

                        spellsSlotCapacities.Clear();

                        foreach (var warlockSlotCapacities in warlockSlotsCapacities
                            .Where(x => warlockLevel < Models.SharedSpellsContext.WARLOCK_MYSTIC_ARCANUM_START_LEVEL || x.Key <= Models.SharedSpellsContext.WARLOCK_MAX_PACT_MAGIC_SPELL_LEVEL))
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
