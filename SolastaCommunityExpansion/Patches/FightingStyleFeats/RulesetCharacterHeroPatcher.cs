using HarmonyLib;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.Patches.FightingStyleFeats
{
    [HarmonyPatch(typeof(RulesetCharacterHero), "TrainFeats")]
    internal static class RulesetCharacterHero_TrainFeats
    {
        internal static void Postfix(RulesetCharacterHero __instance, List<FeatDefinition> feats)
        {
            foreach (FeatDefinition feat in feats)
            {
                foreach (FeatureDefinition featureDefinition in feat.Features)
                {
                    if (featureDefinition is FeatureDefinitionProficiency)
                    {
                        FeatureDefinitionProficiency featureDefinitionProficiency = featureDefinition as FeatureDefinitionProficiency;
                        if (featureDefinitionProficiency.ProficiencyType != RuleDefinitions.ProficiencyType.FightingStyle)
                        {
                            continue;
                        }
                        using (List<string>.Enumerator enumerator3 = featureDefinitionProficiency.Proficiencies.GetEnumerator())
                        {
                            while (enumerator3.MoveNext())
                            {
                                string key = enumerator3.Current;
                                FightingStyleDefinition element = DatabaseRepository.GetDatabase<FightingStyleDefinition>().GetElement(key, false);
                                __instance.TrainedFightingStyles.Add(element);
                            }
                            continue;
                        }
                    }
                }
            }

        }
    }
}
