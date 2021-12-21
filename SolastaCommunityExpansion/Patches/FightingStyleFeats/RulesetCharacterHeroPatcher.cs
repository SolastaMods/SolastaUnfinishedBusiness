using HarmonyLib;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SolastaCommunityExpansion.Patches.FightingStyleFeats
{
    [HarmonyPatch(typeof(RulesetCharacterHero), "TrainFeats")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacterHero_TrainFeats
    {
        internal static void Postfix(RulesetCharacterHero __instance, List<FeatDefinition> feats)
        {
            foreach (FeatDefinition feat in feats)
            {
                foreach (FeatureDefinition featureDefinition in feat.Features)
                {
                    if (!(featureDefinition is FeatureDefinitionProficiency featureDefinitionProficiency))
                    {
                        continue;
                    }

                    if (featureDefinitionProficiency.ProficiencyType != RuleDefinitions.ProficiencyType.FightingStyle)
                    {
                        continue;
                    }

                    using (var enumerator = featureDefinitionProficiency.Proficiencies.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            string key = enumerator.Current;
                            var element = DatabaseRepository.GetDatabase<FightingStyleDefinition>().GetElement(key, false);

                            __instance.TrainedFightingStyles.Add(element);
                        }
                    }
                }
            }

        }
    }
}
