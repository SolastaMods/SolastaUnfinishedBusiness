using HarmonyLib;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaCommunityExpansion.Patches.PowerSharedPool;
using SolastaModApi.Infrastructure;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SolastaCommunityExpansion.Patches.ConditionalPowers
{
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
                if (featureDefinitionPower is IConditionalPower &&
                    !(featureDefinitionPower as IConditionalPower).IsActive(hero))
                {
                    continue;
                }
                RulesetUsablePower rulesetUsablePower = hero.UsablePowers.FirstOrDefault(up => up.PowerDefinition == featureDefinitionPower);
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

            foreach (KeyValuePair<CharacterClassDefinition, int> keyValuePair in hero.ClassesAndLevels)
            {
                if (keyValuePair.Key.FeatureUnlocks.Any(unlock => unlock.FeatureDefinition == featureDefinition))
                {
                    return (null, keyValuePair.Key, null);
                }

                if (hero.ClassesAndSubclasses.ContainsKey(keyValuePair.Key) && hero.ClassesAndSubclasses[keyValuePair.Key] != null)
                {
                    if (hero.ClassesAndSubclasses[keyValuePair.Key].FeatureUnlocks.Any(unlock => unlock.FeatureDefinition == featureDefinition))
                    {
                        return (null, keyValuePair.Key, null);
                    }
                }
            }

            return (null, null, hero.TrainedFeats.FirstOrDefault(tf => tf.Features.Contains(featureDefinition)));
        }
    }
}
