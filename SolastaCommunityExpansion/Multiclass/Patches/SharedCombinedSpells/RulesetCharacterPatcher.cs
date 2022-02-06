using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaModApi.Infrastructure;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaCommunityExpansion.Multiclass.Patches.SharedCombinedSpells
{
    internal static class RulesetCharacterPatcher
    {
        //
        // had to use a bool Prefix here to bypass ModHelpers transpiler on this method... All I really need is set my RestType state
        //
        [HarmonyPatch(typeof(RulesetCharacter), "ApplyRest")]
        internal static class RulesetCharacterApplyRest
        {
            internal static bool Prefix(
                RulesetCharacter __instance,
                RuleDefinitions.RestType restType,
                bool simulate,
                TimeInfo restStartTime)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                Models.SharedSpellsContext.RestType = restType;

                __instance.RecoveredFeatures.Clear();

                foreach (var usablePower in __instance.UsablePowers)
                {
                    if ((usablePower.PowerDefinition.RechargeRate == RuleDefinitions.RechargeRate.ShortRest && (restType == RuleDefinitions.RestType.ShortRest || restType == RuleDefinitions.RestType.LongRest) || usablePower.PowerDefinition.RechargeRate == RuleDefinitions.RechargeRate.LongRest && restType == RuleDefinitions.RestType.LongRest) && usablePower.RemainingUses < usablePower.MaxUses)
                    {
                        if (!simulate)
                        {
                            usablePower.Recharge();
                        }

                        __instance.RecoveredFeatures.Add(usablePower.PowerDefinition);
                    }
                }

                if (__instance.TryGetAttributeValue("ChannelDivinityNumber") > 0 && __instance.UsedChannelDivinity > 0)
                {
                    if (!simulate)
                    {
                        __instance.SetField("usedChannelDivinity", 0);
                    }

                    __instance.EnumerateFeaturesToBrowse<FeatureDefinitionAttributeModifier>(__instance.FeaturesToBrowse);

                    foreach (var featureDefinition in __instance.FeaturesToBrowse)
                    {
                        var attributeModifier = featureDefinition as FeatureDefinitionAttributeModifier;

                        if (attributeModifier.ModifiedAttribute == "ChannelDivinityNumber")
                        {
                            __instance.RecoveredFeatures.Add(attributeModifier);
                            break;
                        }
                    }
                }

                if (restType == RuleDefinitions.RestType.LongRest && __instance.TryGetAttributeValue("HealingPool") > 0 && (__instance.UsedHealingPool > 0 && !simulate))
                {
                    __instance.SetField("usedHealingPool", 0);
                }

                if (restType == RuleDefinitions.RestType.LongRest && __instance.TryGetAttributeValue("SorceryPoints") > 0 && (__instance.UsedSorceryPoints > 0 && !simulate))
                {
                    __instance.SetField("usedSorceryPoints", 0);
                }

                if (restType == RuleDefinitions.RestType.LongRest && __instance.TryGetAttributeValue("RagePoints") > 0 && (__instance.UsedRagePoints > 0 && !simulate))
                {
                    __instance.SetField("usedRagePoints", 0);
                }

                if (restType == RuleDefinitions.RestType.LongRest && __instance.TryGetAttributeValue("IndomitableResistances") > 0 && (__instance.UsedIndomitableResistances > 0 && !simulate))
                {
                    __instance.SetField("usedIndomitableResistances", 0);
                }

#pragma warning disable S1066 // Collapsible "if" statements should be merged
                if (!simulate)
                {
                    if (__instance.TryGetAttribute("RelentlessRageDC", out var rulesetAttribute))
                    {
                        rulesetAttribute.RemoveModifiersByTags("09Health");
                        rulesetAttribute.Refresh();
                    }
                }

                if (!simulate)
                {
                    if (__instance.TryGetAttribute("FrenzyExhaustionDC", out var rulesetAttribute))
                    {
                        rulesetAttribute.RemoveModifiersByTags("09Health");
                        rulesetAttribute.Refresh();
                    }
                }
#pragma warning restore S1066 // Collapsible "if" statements should be merged

                foreach (RulesetSpellRepertoire spellRepertoire in __instance.SpellRepertoires)
                {
                    if (spellRepertoire.SpellCastingFeature.SlotsRecharge == RuleDefinitions.RechargeRate.ShortRest && (restType == RuleDefinitions.RestType.ShortRest || restType == RuleDefinitions.RestType.LongRest) || spellRepertoire.SpellCastingFeature.SlotsRecharge == RuleDefinitions.RechargeRate.LongRest && restType == RuleDefinitions.RestType.LongRest)
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
            }
        }

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

                if (!(__instance is RulesetCharacterHero heroWithSpellRepertoire))
                {
                    return;
                }

                if (!Models.SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire))
                {
                    return;
                }

                if (spellRepertoire?.SpellCastingFeature?.SpellCastingOrigin != FeatureDefinitionCastSpell.CastingOrigin.Race && spellRepertoire?.SpellCastingClass != null)
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
            private static readonly Dictionary<int, int> affinityProviderAdditionalSlots = new Dictionary<int, int>();

            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var computeSpellSlotsMethod = typeof(RulesetSpellRepertoire).GetMethod("ComputeSpellSlots");
                var myComputeSpellSlotsMethod = typeof(RulesetCharacterRefreshSpellRepertoires).GetMethod("MyComputeSpellSlots");
                var finishRepertoiresRefreshMethod = typeof(RulesetCharacterRefreshSpellRepertoires).GetMethod("FinishRepertoiresRefresh");

                foreach (var instruction in instructions)
                {
                    if (!Main.Settings.EnableMulticlass)
                    {
                        yield return instruction;
                    }
                    else if (instruction.Calls(computeSpellSlotsMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
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
            public static void MyComputeSpellSlots(RulesetSpellRepertoire spellRepertoire, List<FeatureDefinition> spellCastingAffinities, RulesetCharacter heroWithSpellRepertoire)
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

                if (!(rulesetCharacter is RulesetCharacterHero heroWithSpellRepertoire))
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
