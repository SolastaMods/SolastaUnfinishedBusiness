using System;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public class FeatureDefinitionFeatureSetRemoval : FeatureDefinitionFeatureSet, IFeatureDefinitionFeatureSetDynamic, IFeatureDefinitionCustomCode
    {
        public FeatureDefinition SelectedFeatureDefinition { get; set; }

        public Func<FeatureDefinitionFeatureSet, List<FeatureDefinition>> DynamicFeatureSet { get; set; } =
            (x) => new List<FeatureDefinition>();

        public void ApplyFeature(RulesetCharacterHero hero)
        {
            foreach (var kvp in hero.ActiveFeatures)
            {
                var features = kvp.Value.ToArray();

                foreach (var feature in features)
                {
                    if (feature == SelectedFeatureDefinition)
                    {
                        kvp.Value.Remove(feature);
                    }
                }
            }
        }

        public void RemoveFeature(RulesetCharacterHero hero)
        {
            // Add back feature to hero
        }
    }
}
