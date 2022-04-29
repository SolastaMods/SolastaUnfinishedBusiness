using System;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public class FeatureDefinitionFeatureSetDynamic : FeatureDefinitionFeatureSet
    {
        public Func<FeatureDefinitionFeatureSet, List<FeatureDefinition>> DynamicFeatureSet { get; set; } =
            (x) => new();
    }
}
