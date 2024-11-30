using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class RulesetSpellRepertoirePatcher
{
    private static bool FormatTitle(RulesetSpellRepertoire __instance, ref string __result)
    {
        if (__instance.SpellCastingClass
            || __instance.SpellCastingSubclass
            || __instance.SpellCastingRace)
        {
            return true;
        }

        __result = __instance.SpellCastingFeature.FormatTitle();

        return false;
    }

    //PATCH: Supports Wizard Mastery and Signature spell features
    [HarmonyPatch(typeof(RulesetSpellRepertoire), nameof(RulesetSpellRepertoire.MaxPreparedSpell), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class MaxPreparedSpell_Getter_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(RulesetSpellRepertoire __instance, ref int __result)
        {
            var character = __instance.GetCaster();

            if (character == null)
            {
                return true;
            }

            if (Tabletop2024Context.IsMemorizeSpellPreparation(character, out var maxMemorizeSpell))
            {
                __result = maxMemorizeSpell;

                return false;
            }

            if (Level20Context.WizardSpellMastery.IsPreparation(character, out var maxSpellMastery))
            {
                __result = maxSpellMastery;

                return false;
            }

            // ReSharper disable once InvertIf
            if (Level20Context.WizardSignatureSpells.IsPreparation(character, out var maxSignatureSpells))
            {
                __result = maxSignatureSpells;

                return false;
            }

            return true;
        }
    }

    //PATCH: handles all different scenarios of spell slots consumption (casts, smites, point buys)
    [HarmonyPatch(typeof(RulesetSpellRepertoire), nameof(RulesetSpellRepertoire.SpendSpellSlot))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SpendSpellSlot_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(RulesetSpellRepertoire __instance, int slotLevel)
        {
            SpendSpellSlot(__instance, slotLevel);

            return false;
        }

        private static void ConsumeSlot(RulesetSpellRepertoire repertoire, int slotLevel)
        {
            var usedSpellsSlots = repertoire.usedSpellsSlots;

            usedSpellsSlots.TryAdd(slotLevel, 0);
            usedSpellsSlots[slotLevel]++;
            repertoire.RepertoireRefreshed?.Invoke(repertoire);
        }

        private static void SpendSpellSlot(RulesetSpellRepertoire __instance, int slotLevel)
        {
            // cantrips don't have usage
            if (slotLevel == 0)
            {
                return;
            }

            var character = __instance.GetCaster();

            // vanilla behavior if a race or monster origin
            if (__instance.SpellCastingFeature.SpellCastingOrigin
                is FeatureDefinitionCastSpell.CastingOrigin.Race
                or FeatureDefinitionCastSpell.CastingOrigin.Monster)
            {
                ConsumeSlot(__instance, slotLevel);

                return;
            }

            var hero = character as RulesetCharacterHero;

            RulesetSpellRepertoire warlockSpellRepertoire = null;

            if (hero != null)
            {
                warlockSpellRepertoire = SharedSpellsContext.GetWarlockSpellRepertoire(hero);
            }

            // handle single caster scenarios both alternate system and vanilla
            if (!SharedSpellsContext.IsMulticaster(hero))
            {
                if (Main.Settings.UseAlternateSpellPointsSystem &&
                    warlockSpellRepertoire == null)
                {
                    SpellPointsContext.ConsumeSlotsAtLevelsPointsCannotCastAnymore(hero, __instance, slotLevel);
                }
                else
                {
                    ConsumeSlot(__instance, slotLevel);
                }

                return;
            }

            // handles MC non-Warlock
            if (warlockSpellRepertoire == null)
            {
                var consume = true;

                foreach (var spellRepertoire in character.SpellRepertoires
                             .Where(x => x.SpellCastingFeature.SpellCastingOrigin !=
                                         FeatureDefinitionCastSpell.CastingOrigin.Race))
                {
                    if (Main.Settings.UseAlternateSpellPointsSystem)
                    {
                        SpellPointsContext.ConsumeSlotsAtLevelsPointsCannotCastAnymore(
                            character, spellRepertoire, slotLevel, consume, true);

                        consume = false;
                    }
                    else
                    {
                        ConsumeSlot(spellRepertoire, slotLevel);
                    }
                }
            }
            // handles MC Warlock
            else
            {
                SpendMulticasterWarlockSlots(__instance, hero, slotLevel);
            }
        }

        private static void SpendWarlockSlots(
            RulesetSpellRepertoire rulesetSpellRepertoire, RulesetCharacterHero heroWithSpellRepertoire)
        {
            var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);
            var usedSpellsSlots = rulesetSpellRepertoire.usedSpellsSlots;

            for (var i = SharedSpellsContext.PactMagicSlotsTab; i <= warlockSpellLevel; i++)
            {
                // don't mess with cantrips
                if (i == 0)
                {
                    continue;
                }

                usedSpellsSlots.TryAdd(i, 0);
                usedSpellsSlots[i]++;
            }

            rulesetSpellRepertoire.RepertoireRefreshed?.Invoke(rulesetSpellRepertoire);
        }

        private static void SpendMulticasterWarlockSlots(
            RulesetSpellRepertoire __instance, RulesetCharacterHero hero, int slotLevel)
        {
            var pactMaxSlots = SharedSpellsContext.GetWarlockMaxSlots(hero);
            var pactUsedSlots = SharedSpellsContext.GetWarlockUsedSlots(hero);

            var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(hero);
            var canConsumePactSlot = pactMaxSlots - pactUsedSlots > 0 && slotLevel <= warlockSpellLevel;

            __instance.GetSlotsNumber(slotLevel, out var totalRemainingSlots, out var totalMaxSlots);

            var totalUsedSlots = totalMaxSlots - totalRemainingSlots;
            var sharedMaxSlots = totalMaxSlots - pactMaxSlots;
            var sharedUsedSlots = totalUsedSlots - pactUsedSlots;

            // collect shift key state registered on spell activation box, reaction, and flexible casting modals
            var glc = GameLocationCharacter.GetFromActor(hero);
            var wasShiftPressed = glc.GetAndClearShiftState();

            // determine if a pact slot should be forced
            var forceConsumePactSlot = sharedUsedSlots == sharedMaxSlots ||
                                       (__instance.SpellCastingClass !=
                                           DatabaseHelper.CharacterClassDefinitions.Warlock && wasShiftPressed) ||
                                       (__instance.SpellCastingClass ==
                                           DatabaseHelper.CharacterClassDefinitions.Warlock && !wasShiftPressed);

            // uses short rest slots across all non race repertoires
            if (canConsumePactSlot && forceConsumePactSlot)
            {
                foreach (var spellRepertoire in hero.SpellRepertoires
                             .Where(x => x.SpellCastingFeature.SpellCastingOrigin !=
                                         FeatureDefinitionCastSpell.CastingOrigin.Race))
                {
                    SpendWarlockSlots(spellRepertoire, hero);
                }
            }

            // otherwise uses long rest slots across all non-race repertoires
            else
            {
                var consume = true;

                foreach (var spellRepertoire in hero.SpellRepertoires
                             .Where(x => x.SpellCastingFeature.SpellCastingOrigin !=
                                         FeatureDefinitionCastSpell.CastingOrigin.Race))
                {
                    if (Main.Settings.UseAlternateSpellPointsSystem)
                    {
                        SpellPointsContext.ConsumeSlotsAtLevelsPointsCannotCastAnymore(
                            hero, spellRepertoire, slotLevel, consume, true);

                        consume = false;
                    }
                    else
                    {
                        ConsumeSlot(spellRepertoire, slotLevel);
                    }
                }
            }
        }
    }

    //PATCH: handles all different scenarios to determine max spell level
    [HarmonyPatch(typeof(RulesetSpellRepertoire), nameof(RulesetSpellRepertoire.MaxSpellLevelOfSpellCastingLevel),
        MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class MaxSpellLevelOfSpellCastingLevel_Getter_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetSpellRepertoire __instance, ref int __result)
        {
            if (!__instance.SpellCastingFeature)
            {
                return;
            }

            if (__instance.SpellCastingFeature.SpellCastingOrigin is FeatureDefinitionCastSpell.CastingOrigin.Race
                or FeatureDefinitionCastSpell.CastingOrigin.Monster)
            {
                return;
            }

            if (SharedSpellsContext.UseMaxSpellLevelOfSpellCastingLevelDefaultBehavior)
            {
                return;
            }

            var heroWithSpellRepertoire = __instance.GetCaster() as RulesetCharacterHero;

            if (!SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire))
            {
                return;
            }

            var sharedSpellLevel = SharedSpellsContext.GetSharedSpellLevel(heroWithSpellRepertoire);
            var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);

            __result = Math.Max(sharedSpellLevel, warlockSpellLevel);
        }
    }

    //PATCH: handles Arcane Recovery granted spells on short rests
    [HarmonyPatch(typeof(RulesetSpellRepertoire), nameof(RulesetSpellRepertoire.RecoverMissingSlots))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RecoverMissingSlots_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(RulesetSpellRepertoire __instance, Dictionary<int, int> recoveredSlots)
        {
            if (__instance.SpellCastingFeature.SpellCastingOrigin is FeatureDefinitionCastSpell.CastingOrigin.Race
                or FeatureDefinitionCastSpell.CastingOrigin.Monster)
            {
                return true;
            }

            if (__instance.GetCaster() is not RulesetCharacterHero hero)
            {
                return true;
            }

            if (!SharedSpellsContext.IsMulticaster(hero))
            {
                return true;
            }

            foreach (var spellRepertoire in hero.SpellRepertoires)
            {
                var usedSpellsSlots = spellRepertoire.usedSpellsSlots;

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

    //PATCH: only offers upcast Warlock pact at their correct slot level
    [HarmonyPatch(typeof(RulesetSpellRepertoire), nameof(RulesetSpellRepertoire.CanUpcastSpell))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CanUpcastSpell_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
            RulesetSpellRepertoire __instance,
            List<int> availableSlotLevels)
        {
            if (__instance.SpellCastingFeature.SpellCastingOrigin is FeatureDefinitionCastSpell.CastingOrigin.Race
                or FeatureDefinitionCastSpell.CastingOrigin.Monster)
            {
                return;
            }

            if (__instance.SpellCastingClass == DatabaseHelper.CharacterClassDefinitions.Warlock)
            {
                return;
            }

            var heroWithSpellRepertoire = __instance.GetCaster() as RulesetCharacterHero;

            if (!SharedSpellsContext.IsMulticaster(heroWithSpellRepertoire))
            {
                return;
            }

            var sharedSpellLevel = SharedSpellsContext.GetSharedSpellLevel(heroWithSpellRepertoire);
            var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);

            for (var i = sharedSpellLevel + 1; i < warlockSpellLevel; i++)
            {
                availableSlotLevels.Remove(i);
            }
        }
    }

    [HarmonyPatch(typeof(RulesetSpellRepertoire), nameof(RulesetSpellRepertoire.FormatHeader))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class FormatHeader_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(RulesetSpellRepertoire __instance, ref string __result)
        {
            //PATCH: prevent null pointer crashes if all origin sources are null
            return FormatTitle(__instance, ref __result);
        }
    }

    [HarmonyPatch(typeof(RulesetSpellRepertoire), nameof(RulesetSpellRepertoire.FormatShortHeader))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class FormatShortHeader_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(RulesetSpellRepertoire __instance, ref string __result)
        {
            //PATCH: prevent null pointer crashes if all origin sources are null
            return FormatTitle(__instance, ref __result);
        }
    }

    [HarmonyPatch(typeof(RulesetSpellRepertoire), nameof(RulesetSpellRepertoire.GetLowestAvailableSlotLevel))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class GetLowestAvailableSlotLevel_Patch
    {
        //PATCH: ensures MC Warlock will cast spells using a correct slot level (MULTICLASS)
        [UsedImplicitly]
        public static bool Prefix(RulesetSpellRepertoire __instance, ref int __result)
        {
            // get off here if not multicaster
            if (__instance.GetCaster() is not RulesetCharacterHero hero ||
                !SharedSpellsContext.IsMulticaster(hero))
            {
                return true;
            }

            var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(hero);

            // get off here if it doesn't have any Warlock level
            if (warlockSpellLevel == 0)
            {
                return true;
            }

            var pactMaxSlots = SharedSpellsContext.GetWarlockMaxSlots(hero);
            var pactUsedSlots = SharedSpellsContext.GetWarlockUsedSlots(hero);
            var pactAvailableSlots = pactMaxSlots - pactUsedSlots;

            __result = pactAvailableSlots == 0 ? 0 : warlockSpellLevel;

            return __result == 0;
        }
    }

    [HarmonyPatch(typeof(RulesetSpellRepertoire), nameof(RulesetSpellRepertoire.HasKnowledgeOfSpell))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class HasKnowledgeOfSpell_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetSpellRepertoire __instance, ref bool __result,
            SpellDefinition consideredSpellDefinition)
        {
            if (__result)
            {
                return;
            }

            var castingFeature = __instance.spellCastingFeature;

            //PATCH: fix case when whole list prepared casters learn spell that's not in their spell list because it was enabled for them through mod options, but then that option is disabled 
            if (castingFeature.SpellKnowledge == SpellKnowledge.WholeList
                && castingFeature.spellReadyness == SpellReadyness.Prepared)
            {
                __result = __instance.PreparedSpells.Contains(consideredSpellDefinition);
            }
        }
    }
}
