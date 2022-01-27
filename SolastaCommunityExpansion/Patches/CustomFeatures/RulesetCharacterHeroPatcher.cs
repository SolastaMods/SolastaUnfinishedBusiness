using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaCommunityExpansion.Helpers;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Patches.CustomFeatures
{
    [HarmonyPatch(typeof(RulesetCharacterHero), "EnumerateUsableRitualSpells")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RestModuleHitDice_EnumerateUsableRitualSpells_Patch
    {
        internal static void Postfix(RulesetCharacterHero __instance, RuleDefinitions.RitualCasting ritualType, List<SpellDefinition> ritualSpells)
        {
            if ((ExtraRitualCasting)ritualType != ExtraRitualCasting.Known) { return; }

            var spellRepertoire = __instance.SpellRepertoires
                .Where(r => r.SpellCastingFeature.SpellReadyness == RuleDefinitions.SpellReadyness.AllKnown)
                .FirstOrDefault(r => r.SpellCastingFeature.SpellKnowledge == RuleDefinitions.SpellKnowledge.Selection);

            if (spellRepertoire == null) { return; }

            ritualSpells.AddRange(spellRepertoire.KnownSpells
                .Where(s => s.Ritual)
                .Where(s => spellRepertoire.MaxSpellLevelOfSpellCastingLevel >= s.SpellLevel));

            if (spellRepertoire.AutoPreparedSpells == null) { return; }

            ritualSpells.AddRange(spellRepertoire.AutoPreparedSpells
                .Where(s => s.Ritual)
                .Where(s => spellRepertoire.MaxSpellLevelOfSpellCastingLevel >= s.SpellLevel));
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), "FindClassHoldingFeature")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacterHero_FindClassHoldingFeature
    {
        //
        // TODO @CHRIS: should we protect this patch? What is the purpose?
        //
        internal static void Postfix(
            RulesetCharacterHero __instance,
            FeatureDefinition featureDefinition,
            ref CharacterClassDefinition __result)
        {
            var overrideClassHoldingFeature = featureDefinition as IClassHoldingFeature;

            if (overrideClassHoldingFeature?.Class == null)
            {
                return;
            }

            // Only override if the character actually has levels in the class, to prevent errors
            if (__instance.ClassesAndLevels.TryGetValue(overrideClassHoldingFeature.Class, out int levelsInClass) && levelsInClass > 0)
            {
                __result = overrideClassHoldingFeature.Class;
            }
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), "RefreshActiveFightingStyles")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacterHero_RefreshActiveFightingStyles
    {
        internal static void Postfix(RulesetCharacterHero __instance)
        {
            foreach (FightingStyleDefinition fightingStyleDefinition in __instance.TrainedFightingStyles)
            {
                if (fightingStyleDefinition is not ICustomFightingStyle customFightingStyle)
                {
                    continue;
                }

                bool isActive = customFightingStyle.IsActive(__instance);
                // We don't know what normal fighting style condition was used or if it was met.
                // The simplest thing to do is just make sure the active state of this fighting style is handled properly.
                if (isActive)
                {
                    if (!__instance.ActiveFightingStyles.Contains(fightingStyleDefinition))
                    {
                        __instance.ActiveFightingStyles.Add(fightingStyleDefinition);
                    }
                }
                else
                {
                    if (__instance.ActiveFightingStyles.Contains(fightingStyleDefinition))
                    {
                        __instance.ActiveFightingStyles.Remove(fightingStyleDefinition);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), "TrainFeats")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacterHero_TrainFeats
    {
        internal static void Postfix(RulesetCharacterHero __instance, List<FeatDefinition> feats)
        {
            foreach (FeatDefinition feat in feats)
            {
                CustomFeaturesContext.RecursiveGrantCustomFeatures(__instance, feat.Features);
            }
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), "RefreshAll")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacterHero_RefreshAll
    {
        internal static void Postfix(RulesetCharacterHero __instance)
        {
            // Anything that grants powers dynamically will stop working if this is turned off.
            // I'm making it a setting to allow it to be disabled if that becomes necesary, but 
            // this shouldn't get exposed in the UI.
            if (Main.Settings.AllowDynamicPowers)
            {
                RefreshPowers(__instance);
            }
        }

        private static void RefreshPowers(RulesetCharacterHero hero)
        {
            // Grant powers when we do a refresh all. This allows powers from things like fighting styles and conditions.
            // This is similar to grant powers, but doesn't use grant powers because grant powers also resets all powers
            // to max available uses.
            List<RulesetUsablePower> curPowers = new List<RulesetUsablePower>();
            bool newPower = false;
            hero.EnumerateFeaturesToBrowse<FeatureDefinitionPower>(hero.FeaturesToBrowse, null);
            foreach (FeatureDefinitionPower featureDefinitionPower in hero.FeaturesToBrowse.Cast<FeatureDefinitionPower>())
            {
                if ((featureDefinitionPower as IConditionalPower)?.IsActive(hero) == false)
                {
                    continue;
                }
                RulesetUsablePower rulesetUsablePower = hero.UsablePowers.Find(up => up.PowerDefinition == featureDefinitionPower);
                if (rulesetUsablePower != null)
                {
                    // If we found a power that was already on the character, re-add the same instance.
                    curPowers.Add(rulesetUsablePower);
                }
                else
                {
                    // If the character didn't already have the power, create the RulesetUsablePower and add it.
                    RulesetUsablePower power = BuildUsablePower(hero, featureDefinitionPower);
                    // If this new power is part of a shared pool, get it properly initialized for usage.
                    if (featureDefinitionPower is IPowerSharedPool)

                    {
                        hero.UpdateUsageForPowerPool(power, 0);
                    }
                    curPowers.Add(power);
                    newPower = true;
                }
            }
            if (hero.UsablePowers.Count == curPowers.Count && !newPower)
            {
                // No change to powers, don't do the potentially expensive calls below.
                return;
            }
            // We only want to modify the UsablePowers list if needed. Because it is modified so rarely in the base game
            // there are some TA.coroutines that iterate over the list with yields in between. This iteration breaks if
            // UsablePowers is modified. We intentionally use SetField here rather than modify the UsablePowers list so
            // that anything currently iterating over the powers won't hang the game.
            hero.SetField("usablePowers", curPowers);
            Traverse.Create(hero).Method("RebindUsablePowers", hero, hero.UsablePowers);
            hero.RefreshPowers();
        }

        private static RulesetUsablePower BuildUsablePower(RulesetCharacterHero hero, FeatureDefinitionPower featureDefinitionPower)
        {
            CharacterRaceDefinition originRace;
            CharacterClassDefinition originClass;
            (originRace, originClass, _) = LookForFeatureOrigin(hero, featureDefinitionPower);
            RulesetUsablePower rulesetUsablePower = new RulesetUsablePower(featureDefinitionPower, originRace, originClass);

            if (featureDefinitionPower.RechargeRate == RuleDefinitions.RechargeRate.ChannelDivinity)
            {
                rulesetUsablePower.UsesAttribute = hero.GetAttribute("ChannelDivinityNumber", false);
            }
            else if (featureDefinitionPower.RechargeRate == RuleDefinitions.RechargeRate.HealingPool)
            {
                rulesetUsablePower.UsesAttribute = hero.GetAttribute("HealingPool", false);
            }
            else if (featureDefinitionPower.RechargeRate == RuleDefinitions.RechargeRate.SorceryPoints)
            {
                rulesetUsablePower.UsesAttribute = hero.GetAttribute("SorceryPoints", false);
            }
            else if (featureDefinitionPower.UsesDetermination == RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed)
            {
                rulesetUsablePower.UsesAttribute = hero.GetAttribute(featureDefinitionPower.UsesAbilityScoreName, false);
            }
            else if (featureDefinitionPower.UsesDetermination == RuleDefinitions.UsesDetermination.ProficiencyBonus)
            {
                rulesetUsablePower.UsesAttribute = hero.GetAttribute("ProficiencyBonus", false);
            }
            rulesetUsablePower.Recharge();
            return rulesetUsablePower;
        }

        private static (CharacterRaceDefinition raceDefinition, CharacterClassDefinition classDefinition, FeatDefinition featDefinition) LookForFeatureOrigin(RulesetCharacterHero hero, FeatureDefinition featureDefinition)
        {
            if (hero.RaceDefinition.FeatureUnlocks.Any(unlock => unlock.FeatureDefinition == featureDefinition))
            {
                return (hero.RaceDefinition, null, null);
            }

            if (hero.SubRaceDefinition != null &&
                hero.SubRaceDefinition.FeatureUnlocks.Any(unlock => unlock.FeatureDefinition == featureDefinition))
            {
                return (hero.SubRaceDefinition, null, null);
            }

            foreach (var key in hero.ClassesAndLevels.Select(kvp => kvp.Key))
            {
                if (key.FeatureUnlocks.Any(unlock => unlock.FeatureDefinition == featureDefinition))
                {
                    return (null, key, null);
                }

                if (hero.ClassesAndSubclasses.ContainsKey(key) && hero.ClassesAndSubclasses[key] != null
                    && hero.ClassesAndSubclasses[key].FeatureUnlocks.Any(unlock => unlock.FeatureDefinition == featureDefinition))
                {
                    return (null, key, null);
                }
            }

            return (null, null, hero.TrainedFeats.Find(tf => tf.Features.Contains(featureDefinition)));
        }
    }
}
