using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.Powers;

//
// Dynamic Powers
//
[HarmonyPatch(typeof(RulesetCharacterHero), "RefreshAll")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetCharacterHero_RefreshAll
{
    internal static void Postfix(RulesetCharacterHero __instance)
    {
        // Anything that grants powers dynamically will stop working if this is turned off.

        RefreshPowers(__instance);
    }

    private static void RefreshPowers(RulesetCharacterHero hero)
    {
        // Grant powers when we do a refresh all. This allows powers from things like fighting styles and conditions.
        // This is similar to grant powers, but doesn't use grant powers because grant powers also resets all powers
        // to max available uses.
        var curPowers = new List<RulesetUsablePower>();
        var newPower = false;
        hero.EnumerateFeaturesToBrowse<FeatureDefinitionPower>(hero.FeaturesToBrowse);

        foreach (var featureDefinitionPower in hero.FeaturesToBrowse.Cast<FeatureDefinitionPower>())
        {
            if ((featureDefinitionPower as IConditionalPower)?.IsActive(hero) == false)
            {
                continue;
            }

            var rulesetUsablePower = hero.UsablePowers.Find(up => up.PowerDefinition == featureDefinitionPower);

            if (rulesetUsablePower != null)
            {
                // If we found a power that was already on the character, re-add the same instance.
                curPowers.Add(rulesetUsablePower);
            }
            else
            {
                // If the character didn't already have the power, create the RulesetUsablePower and add it.
                var power = BuildUsablePower(hero, featureDefinitionPower);
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
        hero.usablePowers = curPowers;
        Traverse.Create(hero).Method("RebindUsablePowers", hero, hero.UsablePowers);
        hero.RefreshPowers();
    }

    private static RulesetUsablePower BuildUsablePower(RulesetCharacterHero hero,
        FeatureDefinitionPower featureDefinitionPower)
    {
        CharacterRaceDefinition originRace;
        CharacterClassDefinition originClass;
        (originRace, originClass, _) = LookForFeatureOrigin(hero, featureDefinitionPower);
        var rulesetUsablePower = new RulesetUsablePower(featureDefinitionPower, originRace, originClass);

        switch (featureDefinitionPower.RechargeRate)
        {
            case RuleDefinitions.RechargeRate.ChannelDivinity:
                rulesetUsablePower.UsesAttribute = hero.GetAttribute(AttributeDefinitions.ChannelDivinityNumber);
                break;
            case RuleDefinitions.RechargeRate.HealingPool:
                rulesetUsablePower.UsesAttribute = hero.GetAttribute(AttributeDefinitions.HealingPool);
                break;
            case RuleDefinitions.RechargeRate.SorceryPoints:
                rulesetUsablePower.UsesAttribute = hero.GetAttribute(AttributeDefinitions.SorceryPoints);
                break;
            default:
                if (featureDefinitionPower.UsesDetermination ==
                    RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed)
                {
                    rulesetUsablePower.UsesAttribute = hero.GetAttribute(featureDefinitionPower.UsesAbilityScoreName);
                }
                else if (featureDefinitionPower.UsesDetermination == RuleDefinitions.UsesDetermination.ProficiencyBonus)
                {
                    rulesetUsablePower.UsesAttribute = hero.GetAttribute(AttributeDefinitions.ProficiencyBonus);
                }

                break;
        }

        rulesetUsablePower.Recharge();
        return rulesetUsablePower;
    }

    private static (CharacterRaceDefinition raceDefinition, CharacterClassDefinition classDefinition, FeatDefinition
        featDefinition) LookForFeatureOrigin(RulesetCharacterHero hero, FeatureDefinition featureDefinition)
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
                                                           && hero.ClassesAndSubclasses[key].FeatureUnlocks
                                                               .Any(unlock =>
                                                                   unlock.FeatureDefinition == featureDefinition))
            {
                return (null, key, null);
            }
        }

        return (null, null, hero.TrainedFeats.Find(tf => tf.Features.Contains(featureDefinition)));
    }
}
