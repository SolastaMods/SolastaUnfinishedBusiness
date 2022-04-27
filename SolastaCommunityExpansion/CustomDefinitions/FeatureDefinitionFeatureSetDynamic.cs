using System;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public class FeatureDefinitionFeatureSetDynamic : FeatureDefinitionFeatureSet, IFeatureDefinitionFeatureSetDynamic
    {
        internal const string REMOVE_BEHAVIOR = "remove";

        public Func<FeatureDefinitionFeatureSet, Dictionary<FeatureDefinition, string>> DynamicFeatureSet { get; set; } =
            (x) => new();
    }
}
