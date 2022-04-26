using System;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public class FeatureDefinitionFeatureSetRemoval : FeatureDefinitionFeatureSet, IFeatureDefinitionFeatureSetDynamic, IFeatureDefinitionCustomCode
    {
        public FeatureDefinition RemovedFeature { get; set; }

        public Func<FeatureDefinitionFeatureSet, List<FeatureDefinition>> DynamicFeatureSet { get; set; } =
            (x) => new List<FeatureDefinition>();

        public void ApplyFeature(RulesetCharacterHero hero)
        {
            // Remove feature from hero
        }

        public void RemoveFeature(RulesetCharacterHero hero)
        {
            // Add back feature to hero
        }
    }
}
