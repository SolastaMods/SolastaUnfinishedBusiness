using System.Collections.Generic;
using HarmonyLib;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Multiclass.Patches.SharedCombinedSpells
{
    internal static class RulesetSpellRepertoirePatcher
    {
        // handles all different scenarios to determine max slots numbers
        [HarmonyPatch(typeof(RulesetSpellRepertoire), "GetMaxSlotsNumberOfAllLevels")]
        internal static class RulesetSpellRepertoireGetMaxSlotsNumberOfAllLevels
        {
            internal static bool Prefix(RulesetSpellRepertoire __instance, ref int __result)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                var heroWithSpellRepertoire = Models.SharedSpellsContext.GetHero(__instance.CharacterName);

                if (heroWithSpellRepertoire == null)
                {
                    return true;
                }

                if (Models.SharedSpellsContext.IsWarlock(__instance.SpellCastingClass))
                {
                    if (Models.SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire))
                    {
                        if (Models.SharedSpellsContext.IsCombined)
                        {
                            // handles MC Warlock with combined system
                            return true;
                        }
                        else
                        {
                            // handles MC Warlock without combined system
                            __result = Models.SharedSpellsContext.GetWarlockMaxSlots(heroWithSpellRepertoire);

                            return false;
                        }
                    }
                    else
                    {
                        // handles SC Warlock
                        __result = Models.SharedSpellsContext.GetWarlockMaxSlots(heroWithSpellRepertoire);

                        return false;
                    }
                }
                else
                {
                    // handles SC non Warlock and MC non Warlock
                    return true;
                }
            }
        }

        // handles all different scenarios to determine remaining slots numbers
        [HarmonyPatch(typeof(RulesetSpellRepertoire), "GetRemainingSlotsNumberOfAllLevels")]
        internal static class RulesetSpellRepertoireGetRemainingSlotsNumberOfAllLevels
        {
            internal static bool Prefix(RulesetSpellRepertoire __instance, ref int __result)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                var heroWithSpellRepertoire = Models.SharedSpellsContext.GetHero(__instance.CharacterName);

                if (heroWithSpellRepertoire == null)
                {
                    return true;
                }

                var usedSpellsSlots = __instance.GetField<RulesetSpellRepertoire, Dictionary<int, int>>("usedSpellsSlots");

                if (Models.SharedSpellsContext.IsWarlock(__instance.SpellCastingClass))
                {
                    if (Models.SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire))
                    {
                        if (Models.SharedSpellsContext.IsCombined)
                        {
                            // handles MC Warlock with combined system
                            return true;
                        }
                        else
                        {
                            // handles MC Warlock without combined system
                            var max = Models.SharedSpellsContext.GetWarlockMaxSlots(heroWithSpellRepertoire);

                            usedSpellsSlots.TryGetValue(-1, out var used);
                            __result = max - used;

                            return false;
                        }
                    }
                    else
                    {
                        // handles SC Warlock
                        var max = Models.SharedSpellsContext.GetWarlockMaxSlots(heroWithSpellRepertoire);

                        usedSpellsSlots.TryGetValue(-1, out var used);
                        __result = max - used;

                        return false;
                    }
                }
                else
                {
                    // handles SC non Warlock and MC non Warlock
                    return true;
                }
            }
        }

        // handles all different scenarios to determine slots numbers
        [HarmonyPatch(typeof(RulesetSpellRepertoire), "GetSlotsNumber")]
        internal static class RulesetSpellRepertoireGetSlotsNumber
        {
            internal static bool Prefix(RulesetSpellRepertoire __instance, int spellLevel, ref int remaining, ref int max)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                var heroWithSpellRepertoire = Models.SharedSpellsContext.GetHero(__instance.CharacterName);

                if (heroWithSpellRepertoire == null)
                {
                    return true;
                }

                max = 0;
                remaining = 0;

                var usedSpellsSlots = __instance.GetField<RulesetSpellRepertoire, Dictionary<int, int>>("usedSpellsSlots");
                var spellsSlotCapacities = __instance.GetField<RulesetSpellRepertoire, Dictionary<int, int>>("spellsSlotCapacities");

                if (Models.SharedSpellsContext.IsWarlock(__instance.SpellCastingClass))
                {
                    if (Models.SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire))
                    {
                        if (Models.SharedSpellsContext.IsCombined)
                        {
                            // handles MC Warlock with combined system
                            spellsSlotCapacities.TryGetValue(spellLevel, out max);
                            usedSpellsSlots.TryGetValue(spellLevel, out var used);
                            remaining = max - used;
                        }
                        else
                        {
                            // handles MC Warlock without combined system
                            if (spellLevel <= __instance.MaxSpellLevelOfSpellCastingLevel)
                            {
                                if (spellLevel <= Models.SharedSpellsContext.WARLOCK_MAX_PACT_MAGIC_SPELL_LEVEL)
                                {
                                    max = Models.SharedSpellsContext.GetWarlockMaxSlots(heroWithSpellRepertoire);
                                    usedSpellsSlots.TryGetValue(-1, out var used);
                                    remaining = max - used;
                                }
                                else
                                {
                                    max = 1;
                                    usedSpellsSlots.TryGetValue(spellLevel, out var used);
                                    remaining = max - used;
                                }
                            }
                        }
                    }
                    else
                    {
                        // handles SC Warlock
                        if (spellLevel <= __instance.MaxSpellLevelOfSpellCastingLevel)
                        {
                            if (spellLevel <= Models.SharedSpellsContext.WARLOCK_MAX_PACT_MAGIC_SPELL_LEVEL)
                            {
                                max = Models.SharedSpellsContext.GetWarlockMaxSlots(heroWithSpellRepertoire);
                                usedSpellsSlots.TryGetValue(-1, out var used);
                                remaining = max - used;
                            }
                            else
                            {
                                max = 1;
                                usedSpellsSlots.TryGetValue(spellLevel, out var used);
                                remaining = max - used;
                            }
                        }
                    }
                }
                else
                {
                    // handles SC non Warlock and MC non Warlock
                    spellsSlotCapacities.TryGetValue(spellLevel, out max);
                    usedSpellsSlots.TryGetValue(spellLevel, out var used);
                    remaining = max - used;
                }

                return false;
            }
        }

        // handles all different scenarios to determine max spell level (must be a postfix)
        [HarmonyPatch(typeof(RulesetSpellRepertoire), "MaxSpellLevelOfSpellCastingLevel", MethodType.Getter)]
        internal static class RulesetSpellRepertoireMaxSpellLevelOfSpellCastingLevelGetter
        {
            internal static void Postfix(RulesetSpellRepertoire __instance, ref int __result)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    if (Main.Settings.EnableLevel20 && __instance.SpellCastingFeature != null && __instance.SpellCastingLevel > 0)
                    {
                        var slotsPerLevel = __instance.SpellCastingFeature.SlotsPerLevels[__instance.SpellCastingLevel - 1];

                        __result = slotsPerLevel.Slots.IndexOf(0);
                    }

                    return;
                }

                var heroWithSpellRepertoire = Models.SharedSpellsContext.GetHero(__instance.CharacterName);

                if (heroWithSpellRepertoire == null)
                {
                    return;
                }

                if (Models.SharedSpellsContext.IsCombined)
                {
                    __result = Models.SharedSpellsContext.GetCombinedSpellLevel(heroWithSpellRepertoire);
                }
                else
                {
                    if (Models.SharedSpellsContext.IsWarlock(__instance.SpellCastingClass))
                    {
                        __result = Models.SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);
                    }
                    else if (Models.SharedSpellsContext.IsEnabled)
                    {
                        __result = Models.SharedSpellsContext.GetSharedSpellLevel(heroWithSpellRepertoire);
                    }
                }
            }
        }

        // handles Arcane Recovery granted spells on short rests
        [HarmonyPatch(typeof(RulesetSpellRepertoire), "RecoverMissingSlots")]
        internal static class RulesetSpellRepertoireRecoverMissingSlots
        {
            internal static bool Prefix(RulesetSpellRepertoire __instance, Dictionary<int, int> recoveredSlots)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                var heroWithSpellRepertoire = Models.SharedSpellsContext.GetHero(__instance.CharacterName);

                if (heroWithSpellRepertoire == null)
                {
                    return true;
                }

                foreach (var spellRepertoire in heroWithSpellRepertoire.SpellRepertoires)
                {
                    if (!Models.SharedSpellsContext.IsEnabled && spellRepertoire != __instance)
                    {
                        continue;
                    }

                    if (!Models.SharedSpellsContext.IsCombined && spellRepertoire.SpellCastingClass == Models.IntegrationContext.WarlockClass)
                    {
                        continue;
                    }

                    foreach (var recoveredSlot in recoveredSlots)
                    {
                        var key = recoveredSlot.Key;
                        var usedSpellsSlots = spellRepertoire.GetField<RulesetSpellRepertoire, Dictionary<int, int>>("usedSpellsSlots");
                        var spellsSlotCapacities = spellRepertoire.GetField<RulesetSpellRepertoire, Dictionary<int, int>>("spellsSlotCapacities");

                        if (usedSpellsSlots.ContainsKey(key)
                            && spellsSlotCapacities.ContainsKey(key)
                            && usedSpellsSlots[key] > 0)
                        {
                            usedSpellsSlots[key] = UnityEngine.Mathf.Max(0, usedSpellsSlots[key] - recoveredSlot.Value);
                        }

                        spellRepertoire.RepertoireRefreshed?.Invoke(__instance);
                    }
                }

                return false;
            }
        }

        // handles Warlock short rest spells recovery
        [HarmonyPatch(typeof(RulesetSpellRepertoire), "RestoreAllSpellSlots")]
        internal static class RulesetSpellRepertoireRestoreAllSpellSlots
        {
            internal static bool Prefix(RulesetSpellRepertoire __instance)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                var heroWithSpellRepertoire = Models.SharedSpellsContext.GetHero(__instance.CharacterName);

                if (heroWithSpellRepertoire == null)
                {
                    return true;
                }

                if (Models.SharedSpellsContext.RestType == RuleDefinitions.RestType.LongRest)
                {
                    return true;
                }

                var warlockSpellLevel = Models.SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);

                __instance.GetField<RulesetSpellRepertoire, Dictionary<int, int>>("usedSpellsSlots").TryGetValue(-1, out var slotsToRestore);

                foreach (var spellRepertoire in heroWithSpellRepertoire.SpellRepertoires)
                {
                    if (spellRepertoire.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Race)
                    {
                        continue;
                    }

                    if (!Models.SharedSpellsContext.IsCombined && spellRepertoire.SpellCastingClass != Models.IntegrationContext.WarlockClass)
                    {
                        continue;
                    }

                    var usedSpellsSlots = spellRepertoire.GetField<RulesetSpellRepertoire, Dictionary<int, int>>("usedSpellsSlots");

                    // uses index -1 to keep a tab on short rest slots usage
                    var limit = System.Math.Min(Models.SharedSpellsContext.WARLOCK_MAX_PACT_MAGIC_SPELL_LEVEL, warlockSpellLevel);

                    for (var i = -1; i <= limit; i++)
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

                return false;
            }
        }

        // handles all different scenarios of spell slots consumption (casts, smites, point buys)
        [HarmonyPatch(typeof(RulesetSpellRepertoire), "SpendSpellSlot")]
        internal static class RulesetSpellRepertoireSpendSpellSlot
        {
            internal static bool Prefix(RulesetSpellRepertoire __instance, int slotLevel)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                if (slotLevel == 0)
                {
                    return true;
                }

                var heroWithSpellRepertoire = Models.SharedSpellsContext.GetHero(__instance.CharacterName);

                if (heroWithSpellRepertoire == null)
                {
                    return true;
                }

                // handles SC Warlock or MC Warlock without combined system
                if (Models.SharedSpellsContext.IsWarlock(__instance.SpellCastingClass) && (!Models.SharedSpellsContext.IsCombined || !Models.SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire)))
                {
                    SpendWarlockSlots(__instance, heroWithSpellRepertoire, slotLevel);
                    return false;
                }

                if (!Models.SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire))
                {
                    return true;
                }

                if (!Models.SharedSpellsContext.IsEnabled)
                {
                    return true;
                }

                var warlockSpellRepertoire = Models.SharedSpellsContext.GetWarlockSpellRepertoire(heroWithSpellRepertoire);

                // handles MC non-Warlock or MC without combined system
                if (warlockSpellRepertoire == null || !Models.SharedSpellsContext.IsCombined)
                {
                    foreach (var spellRepertoire in heroWithSpellRepertoire.SpellRepertoires)
                    {
                        if (spellRepertoire.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Race)
                        {
                            continue;
                        }

                        if (spellRepertoire.SpellCastingFeature.SlotsRecharge == RuleDefinitions.RechargeRate.LongRest)
                        {
                            var usedSpellsSlots = spellRepertoire.GetField<RulesetSpellRepertoire, Dictionary<int, int>>("usedSpellsSlots");

                            if (!usedSpellsSlots.ContainsKey(slotLevel))
                            {
                                usedSpellsSlots.Add(slotLevel, 0);
                            }

                            usedSpellsSlots[slotLevel]++;
                            spellRepertoire.RepertoireRefreshed?.Invoke(spellRepertoire);
                        }
                    }
                }

                // handles MC Warlock with combined system 
                else
                {
                    var sharedSpellLevel = Models.SharedSpellsContext.GetSharedSpellLevel(heroWithSpellRepertoire);
                    var warlockSpellLevel = Models.SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);
                    var warlockMaxSlots = Models.SharedSpellsContext.GetWarlockMaxSlots(heroWithSpellRepertoire);
                    var usedSpellsSlotsWarlock = warlockSpellRepertoire.GetField<RulesetSpellRepertoire, Dictionary<int, int>>("usedSpellsSlots");

                    // index -1 keeps a tab of short rest slots used
                    usedSpellsSlotsWarlock.TryGetValue(-1, out var warlockUsedSlots);
                    __instance.GetSlotsNumber(slotLevel, out var sharedRemainingSlots, out var sharedMaxSlots);

                    var sharedUsedSlots = sharedMaxSlots - sharedRemainingSlots;

                    sharedMaxSlots -= warlockMaxSlots;
                    sharedUsedSlots -= warlockUsedSlots;

                    var canConsumeShortRestSlot = warlockUsedSlots < warlockMaxSlots && slotLevel <= warlockSpellLevel;
                    var canConsumeLongRestSlot = sharedUsedSlots < sharedMaxSlots && slotLevel <= sharedSpellLevel;
                    var forceLongRestSlotUI = canConsumeLongRestSlot && Models.SharedSpellsContext.ForceLongRestSlot;

                    // uses short rest slots across all repertoires
                    if (canConsumeShortRestSlot && !forceLongRestSlotUI)
                    {
                        foreach (var spellRepertoire in heroWithSpellRepertoire.SpellRepertoires)
                        {
                            if (spellRepertoire.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Race)
                            {
                                continue;
                            }

                            SpendWarlockSlots(spellRepertoire, heroWithSpellRepertoire, slotLevel);
                        }
                    }

                    // otherwise uses long rest slots across all repertoires
                    else
                    {
                        foreach (var spellRepertoire in heroWithSpellRepertoire.SpellRepertoires)
                        {
                            if (spellRepertoire.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Race)
                            {
                                continue;
                            }

                            var usedSpellsSlots = spellRepertoire.GetField<RulesetSpellRepertoire, Dictionary<int, int>>("usedSpellsSlots");

                            if (!usedSpellsSlots.ContainsKey(slotLevel))
                            {
                                usedSpellsSlots.Add(slotLevel, 0);
                            }

                            usedSpellsSlots[slotLevel]++;
                            spellRepertoire.RepertoireRefreshed?.Invoke(spellRepertoire);
                        }
                    }
                }

                return false;
            }

            private static void SpendWarlockSlots(RulesetSpellRepertoire rulesetSpellRepertoire, RulesetCharacterHero heroWithSpellRepertoire, int slotLevel)
            {
                var warlockSpellLevel = Models.SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);
                var usedSpellsSlots = rulesetSpellRepertoire.GetField<RulesetSpellRepertoire, Dictionary<int, int>>("usedSpellsSlots");

                // uses index -1 to keep a tab on short rest slots usage
                if (slotLevel <= Models.SharedSpellsContext.WARLOCK_MAX_PACT_MAGIC_SPELL_LEVEL)
                {
                    var limit = System.Math.Min(5, warlockSpellLevel);

                    for (var i = -1; i <= limit; i++)
                    {
                        if (i == 0)
                        {
                            continue;
                        }

                        if (!usedSpellsSlots.ContainsKey(i))
                        {
                            usedSpellsSlots.Add(i, 0);
                        }

                        usedSpellsSlots[i]++;
                    }
                }
                else
                {
                    for (var i = Models.SharedSpellsContext.WARLOCK_MAX_PACT_MAGIC_SPELL_LEVEL; i <= warlockSpellLevel; i++)
                    {
                        if (!usedSpellsSlots.ContainsKey(i))
                        {
                            usedSpellsSlots.Add(i, 0);
                        }

                        usedSpellsSlots[i]++;
                    }
                }

                rulesetSpellRepertoire.RepertoireRefreshed?.Invoke(rulesetSpellRepertoire);
            }
        }
    }
}
