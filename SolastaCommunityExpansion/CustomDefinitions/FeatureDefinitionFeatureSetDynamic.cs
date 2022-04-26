using System;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public class FeatureDefinitionFeatureSetDynamic : FeatureDefinitionFeatureSet, IFeatureDefinitionFeatureSetDynamic
    {
        public Func<FeatureDefinitionFeatureSetDynamic, List<FeatureDefinition>> DynamicFeatureSet { get; set; } =
            (x) => new List<FeatureDefinition>();
    }
}
