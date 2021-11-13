using HarmonyLib;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.Patches.ConditionalPowers
{
    [HarmonyPatch(typeof(RulesetCharacterHero), "RefreshAll")]
    internal static class RulesetCharacterHero_RefreshAll_Patch
    {
        internal static void Postfix(RulesetCharacterHero __instance)
        {
            // Anything that grants powers dynamically will stop working if this is turned off.
            // I'm making it a setting to allow it to be disabled if that becomes necesary, but 
            // this shouldn't get exposed in the UI.
            if (Main.Settings.AllowDynamicPowers)
            {
				// Grant powers when we do a refresh all. This allows powers from things like fighting styles and conditions.
				// This is similar to grant powers, but doesn't use grant powers because grant powers also resets all powers
				// to max available uses.
				List<RulesetUsablePower> curPowers = new List<RulesetUsablePower>();
				curPowers.AddRange(__instance.UsablePowers);
				__instance.UsablePowers.Clear();
				__instance.EnumerateFeaturesToBrowse<FeatureDefinitionPower>(__instance.FeaturesToBrowse, null);
				foreach (FeatureDefinition featureDefinition in __instance.FeaturesToBrowse)
				{
					FeatureDefinitionPower featureDefinitionPower = (FeatureDefinitionPower)featureDefinition;
					if (featureDefinitionPower is IConditionalPower)
                    {
						// If this is a conditional power, then check if it is active.
						if (!(featureDefinitionPower as IConditionalPower).IsActive(__instance))
                        {
							continue;
                        }
                    }
					RulesetUsablePower rulesetUsablePower = null;
					foreach (RulesetUsablePower rulesetUsablePower2 in curPowers)
					{
						if (rulesetUsablePower2.PowerDefinition == featureDefinitionPower)
						{
							rulesetUsablePower = rulesetUsablePower2;
							break;
						}
					}
					if (rulesetUsablePower != null)
                    {
						// If we found a power that was already on the character, re-add the same instance.
						__instance.UsablePowers.Add(rulesetUsablePower);
                    } else
                    {
						// If the character didn't already have the power, create the RulesetUsablePower and add it.
						__instance.UsablePowers.Add(BuildUsablePower(__instance, featureDefinitionPower));
					}
				}
				Traverse.Create(__instance).Method("RebindUsablePowers", __instance, __instance.UsablePowers);
				__instance.RefreshPowers();
            }
        }

		private static RulesetUsablePower BuildUsablePower(RulesetCharacterHero hero, FeatureDefinitionPower featureDefinitionPower)
        {
			CharacterRaceDefinition originRace = null;
			CharacterClassDefinition originClass = null;
			FeatDefinition featDefinition = null;
			LookForFeatureOrigin(hero, featureDefinitionPower, out originRace, out originClass, out featDefinition);
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

		private static void LookForFeatureOrigin(RulesetCharacterHero hero, FeatureDefinition featureDefinition, out CharacterRaceDefinition raceDefinition, out CharacterClassDefinition classDefinition, out FeatDefinition featDefinition)
		{
			raceDefinition = null;
			classDefinition = null;
			featDefinition = null;
			using (List<FeatureUnlockByLevel>.Enumerator enumerator = hero.RaceDefinition.FeatureUnlocks.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.FeatureDefinition == featureDefinition)
					{
						raceDefinition = hero.RaceDefinition;
						return;
					}
				}
			}
			if (hero.SubRaceDefinition != null)
			{
				using (List<FeatureUnlockByLevel>.Enumerator enumerator = hero.SubRaceDefinition.FeatureUnlocks.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.FeatureDefinition == featureDefinition)
						{
							raceDefinition = hero.SubRaceDefinition;
							return;
						}
					}
				}
			}
			foreach (KeyValuePair<CharacterClassDefinition, int> keyValuePair in hero.ClassesAndLevels)
			{
				using (List<FeatureUnlockByLevel>.Enumerator enumerator = keyValuePair.Key.FeatureUnlocks.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.FeatureDefinition == featureDefinition)
						{
							classDefinition = keyValuePair.Key;
							return;
						}
					}
				}
				if (hero.ClassesAndSubclasses.ContainsKey(keyValuePair.Key) && hero.ClassesAndSubclasses[keyValuePair.Key] != null)
				{
					using (List<FeatureUnlockByLevel>.Enumerator enumerator = hero.ClassesAndSubclasses[keyValuePair.Key].FeatureUnlocks.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current.FeatureDefinition == featureDefinition)
							{
								classDefinition = keyValuePair.Key;
								return;
							}
						}
					}
				}
			}
			foreach (FeatDefinition featDefinition2 in hero.TrainedFeats)
			{
				using (List<FeatureDefinition>.Enumerator enumerator4 = featDefinition2.Features.GetEnumerator())
				{
					while (enumerator4.MoveNext())
					{
						if (enumerator4.Current == featureDefinition)
						{
							featDefinition = featDefinition2;
							return;
						}
					}
				}
			}
		}
	}
}
