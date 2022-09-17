using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Subclasses;

namespace SolastaUnfinishedBusiness.Patches;

internal static class RulesetCharacterHeroPatcher
{
    [HarmonyPatch(typeof(RulesetCharacterHero), "FindClassHoldingFeature")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FindClassHoldingFeature_Patch
    {
        internal static void Postfix(
            RulesetCharacterHero __instance,
            FeatureDefinition featureDefinition,
            ref CharacterClassDefinition __result)
        {
            //PATCH: replaces feature holding class with one provided by custom interface
            //used for features that are not granted directly through class but need to scale with class levels
            var classHolder = featureDefinition.GetFirstSubFeatureOfType<IClassHoldingFeature>()?.Class;

            if (classHolder == null)
            {
                return;
            }

            // Only override if the character actually has levels in the class, to prevent errors
            if (__instance.ClassesAndLevels.TryGetValue(classHolder, out var levelsInClass) && levelsInClass > 0)
            {
                __result = classHolder;
            }
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), "RefreshAttackModes")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RefreshAttackModes_Patch
    {
        private static bool _callRefresh;

        internal static void Prefix(ref bool callRefresh)
        {
            //save refresh flag, so it can be used in postfix
            _callRefresh = callRefresh;
            //reset refresh flag, so default code won't do refresh before postfix
            callRefresh = false;
        }

        internal static void Postfix(RulesetCharacterHero __instance, bool callRefresh = false)
        {
            //PATCH: Allows adding extra attack modes
            __instance.GetSubFeaturesByType<IAddExtraAttack>()
                .ForEach(provider => provider.TryAddExtraAttack(__instance));

            //PATCH: Allows changing damage and other stats of an attack mode
            var modifiers = __instance.GetSubFeaturesByType<IModifyAttackModeForWeapon>();

            foreach (var attackMode in __instance.AttackModes)
            {
                foreach (var modifier in modifiers)
                {
                    modifier.ModifyAttackMode(__instance, attackMode);
                }
            }

            //refresh character if needed after postfix
            if (_callRefresh && __instance.CharacterRefreshed != null)
            {
                __instance.CharacterRefreshed(__instance);
            }
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), "RefreshAll")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RefreshAll_Patch
    {
        internal static void Prefix(RulesetCharacterHero __instance)
        {
            //PATCH: Support for `IHeroRefreshedListener`
            __instance.GetSubFeaturesByType<IHeroRefreshedListener>()
                .ForEach(listener => listener.OnHeroRefreshed(__instance));
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), "ItemEquiped")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ItemEquiped_Patch
    {
        internal static void Postfix(RulesetCharacterHero __instance, RulesetItem rulesetItem)
        {
            //PATCH: blade dancer only trigger certain features if certain item aren't wielded
            WizardBladeDancer.OnItemEquipped(__instance, rulesetItem);
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), "RefreshActiveFightingStyles")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RefreshActiveFightingStyles_Patch
    {
        internal static void Postfix(RulesetCharacterHero __instance)
        {
            //PATCH: enables some corner-case fighting styles (like archery for hand crossbows and dual wielding for shield expert)
            FightingStyleContext.RefreshFightingStylesPatch(__instance);
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), "AcknowledgeAttackUse")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class AcknowledgeAttackUse_Patch
    {
        // ReSharper disable once RedundantAssignment
        internal static void Prefix(RulesetCharacterHero __instance,
            RulesetAttackMode mode)
        {
            //PATCH: supports turning Produced Flame into a weapon
            //destroys Produced Flame after attacking with it
            CustomWeaponsContext.ProcessProducedFlameAttack(__instance, mode);
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), "ComputeCraftingDurationHours")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ComputeCraftingDurationHours_Patch
    {
        internal static void Postfix(ref int __result)
        {
            //PATCH: reduces the total crafting time by a given percentage
            __result = (int)((100f - Main.Settings.TotalCraftingTimeModifier) / 100 * __result);
        }
    }

    //PATCH: DisableAutoEquip
    [HarmonyPatch(typeof(RulesetCharacterHero), "GrantItem")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GrantItem_Patch
    {
        internal static void Prefix(RulesetCharacterHero __instance, ref bool tryToEquip)
        {
            if (!Main.Settings.DisableAutoEquip || !tryToEquip)
            {
                return;
            }

            tryToEquip = __instance.TryGetHeroBuildingData(out _);
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), "RefreshArmorClass")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RefreshArmorClass_Patch
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: implements exclusivity for some AC modifiers
            // Makes sure various unarmored defense features don't stack with themselves and Dragon Resilience
            // Replaces calls to `RulesetAttributeModifier.SortAttributeModifiersList` with custom method
            // that removes inactive exclusive modifiers, and then calls `RulesetAttributeModifier.SortAttributeModifiersList`
            return ArmorClassStacking.UnstackAcTranspile(instructions);
        }
    }

    //PATCH: ensures ritual spells from all spell repertoires are made available (Multiclass)
    [HarmonyPatch(typeof(RulesetCharacterHero), "EnumerateUsableRitualSpells")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class EnumerateUsableRitualSpells_Patch
    {
        internal static bool Prefix(
            RulesetCharacterHero __instance,
            List<SpellDefinition> ritualSpells)
        {
            if (!SharedSpellsContext.IsMulticaster(__instance))
            {
                return true;
            }

            var allRitualSpells = new List<SpellDefinition>();
            var magicAffinities = new List<FeatureDefinition>();
            ritualSpells.SetRange(allRitualSpells);

            __instance.EnumerateFeaturesToBrowse<FeatureDefinitionMagicAffinity>(magicAffinities);

            // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
            foreach (FeatureDefinitionMagicAffinity featureDefinitionMagicAffinity in magicAffinities)
            {
                if (featureDefinitionMagicAffinity.RitualCasting == RuleDefinitions.RitualCasting.None)
                {
                    continue;
                }

                foreach (var spellRepertoire in __instance.SpellRepertoires)
                {
                    // this is very similar to switch statement TA wrote but with spell loops outside
                    switch (featureDefinitionMagicAffinity.RitualCasting)
                    {
                        case RuleDefinitions.RitualCasting.PactTomeRitual:
                        {
                            foreach (var kvp in spellRepertoire.ExtraSpellsByTag)
                            {
                                if (!kvp.Key.Contains("PactTomeRitual"))
                                {
                                    continue;
                                }

                                foreach (var spellDefinition in kvp.Value)
                                {
                                    if (spellDefinition.Ritual
                                        && spellRepertoire.MaxSpellLevelOfSpellCastingLevel >=
                                        spellDefinition.SpellLevel)
                                    {
                                        allRitualSpells.Add(spellDefinition);
                                    }
                                }
                            }

                            break;
                        }

                        case RuleDefinitions.RitualCasting.Selection:
                        {
                            var spells = spellRepertoire.KnownSpells
                                .Where(knownSpell =>
                                    knownSpell.Ritual && spellRepertoire.MaxSpellLevelOfSpellCastingLevel >=
                                    knownSpell.SpellLevel);

                            allRitualSpells.AddRange(spells);

                            break;
                        }

                        case RuleDefinitions.RitualCasting.Prepared
                            when spellRepertoire.SpellCastingFeature.SpellReadyness ==
                                 RuleDefinitions.SpellReadyness.Prepared &&
                                 spellRepertoire.SpellCastingFeature.SpellKnowledge ==
                                 RuleDefinitions.SpellKnowledge.WholeList:
                        {
                            var maxSpellLevel = SharedSpellsContext.GetClassSpellLevel(spellRepertoire);
                            var spells = spellRepertoire.PreparedSpells
                                .Where(s => s.Ritual)
                                .Where(s => maxSpellLevel >= s.SpellLevel);

                            allRitualSpells.AddRange(spells);

                            break;
                        }
                        case RuleDefinitions.RitualCasting.Spellbook
                            when spellRepertoire.SpellCastingFeature.SpellKnowledge ==
                                 RuleDefinitions.SpellKnowledge.Spellbook:
                        {
                            __instance.CharacterInventory.EnumerateAllItems(__instance.Items);

                            var maxSpellLevel = SharedSpellsContext.GetClassSpellLevel(spellRepertoire);
                            var spells = __instance.Items
                                .OfType<RulesetItemSpellbook>()
                                .SelectMany(x => x.ScribedSpells)
                                .ToList();

                            spells = spells
                                .Where(s => s.Ritual)
                                .Where(s => maxSpellLevel >= s.SpellLevel)
                                .ToList();

                            __instance.Items.Clear();

                            allRitualSpells.AddRange(spells);

                            break;
                        }
                        // special case for Witch
                        case (RuleDefinitions.RitualCasting)ExtraRitualCasting.Known:
                        {
                            var maxSpellLevel = SharedSpellsContext.GetClassSpellLevel(spellRepertoire);
                            var spells = spellRepertoire.KnownSpells
                                .Where(s => s.Ritual)
                                .Where(s => maxSpellLevel >= s.SpellLevel);

                            allRitualSpells.AddRange(spells);

                            if (spellRepertoire.AutoPreparedSpells == null)
                            {
                                return true;
                            }

                            spells = spellRepertoire.AutoPreparedSpells
                                .Where(s => s.Ritual)
                                .Where(s => maxSpellLevel >= s.SpellLevel);

                            allRitualSpells.AddRange(spells);
                            break;
                        }
                    }
                }
            }

            ritualSpells.SetRange(allRitualSpells.Distinct());

            return false;
        }
    }
    
    [HarmonyPatch(typeof(RulesetCharacterHero), "GrantExperience")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GrantExperience_Patch
    {
        internal static void Prefix(ref int experiencePoints)
        {
            if (Main.Settings.MultiplyTheExperienceGainedBy is 100 or <= 0)
            {
                return;
            }

            var original = experiencePoints;

            experiencePoints =
                (int)Math.Round(experiencePoints * Main.Settings.MultiplyTheExperienceGainedBy / 100.0f,
                    MidpointRounding.AwayFromZero);

            Main.Log(
                $"GrantExperience: Multiplying experience gained by {Main.Settings.MultiplyTheExperienceGainedBy}%. Original={original}, modified={experiencePoints}.");
        }
    }

    //PATCH: enables the No Experience on Level up cheat (NoExperienceOnLevelUp)
    [HarmonyPatch(typeof(RulesetCharacterHero), "CanLevelUp", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CanLevelUp_Patch
    {
        internal static bool Prefix(RulesetCharacterHero __instance, ref bool __result)
        {
            if (Main.Settings.NoExperienceOnLevelUp)
            {
                var levelCap = Main.Settings.EnableLevel20
                    ? Level20Context.ModMaxLevel
                    : Level20Context.GameMaxLevel;

                __result = __instance.ClassesHistory.Count < levelCap;

                return false;
            }

            if (!Main.Settings.EnableLevel20)
            {
                return true;
            }

            {
                var levelCap = Main.Settings.EnableLevel20
                    ? Level20Context.ModMaxLevel
                    : Level20Context.GameMaxLevel;
                // If the game doesn't know how much XP to reach the next level it uses -1 to determine if the character can level up.
                // When a character is level 20, this ends up meaning the character can now level up forever unless we stop it here.
                if (__instance.ClassesHistory.Count < levelCap)
                {
                    return true;
                }

                __result = false;

                return false;
            }
        }
    }
}
