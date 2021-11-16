using HarmonyLib;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaCommunityExpansion.Patches.PowerSharedPool;
using SolastaModApi.Infrastructure;
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
			foreach (FeatureDefinition featureDefinition in hero.FeaturesToBrowse)
			{
				FeatureDefinitionPower featureDefinitionPower = (FeatureDefinitionPower)featureDefinition;
				if (featureDefinitionPower is IConditionalPower)
				{
					// If this is a conditional power, then check if it is active.
					if (!(featureDefinitionPower as IConditionalPower).IsActive(hero))
					{
						continue;
					}
				}
				RulesetUsablePower rulesetUsablePower = null;
				foreach (RulesetUsablePower rulesetUsablePower2 in hero.UsablePowers)
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
					curPowers.Add(rulesetUsablePower);
				}
				else
				{
					// If the character didn't already have the power, create the RulesetUsablePower and add it.
					RulesetUsablePower power = BuildUsablePower(hero, featureDefinitionPower);
					// If this new power is part of a shared pool, get it properly initialized for usage.
					if (featureDefinitionPower is IPowerSharedPool)
                    {
						RulesetCharacterPatch.UpdateUsageForPowerPool(hero, power, 0);

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
