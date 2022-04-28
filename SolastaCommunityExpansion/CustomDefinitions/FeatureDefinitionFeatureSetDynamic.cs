using System;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public class FeatureDefinitionFeatureSetDynamic : FeatureDefinitionFeatureSet
    {
        public Func<FeatureDefinitionFeatureSet, Dictionary<FeatureDefinition, string>> DynamicFeatureSet { get; set; } =
            (x) => new();
    }
}
