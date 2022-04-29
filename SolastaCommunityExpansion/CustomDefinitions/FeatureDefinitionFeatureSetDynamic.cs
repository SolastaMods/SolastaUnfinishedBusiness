using System;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public class FeatureDefinitionFeatureSetDynamic : FeatureDefinitionFeatureSet
    {
        public Func<FeatureDefinitionFeatureSet, Queue<FeatureDefinition>> DynamicFeatureSet { get; set; } =
            (x) => new();
    }
}
