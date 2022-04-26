using System;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public class FeatureDefinitionFeatureSetUniqueAcross : FeatureDefinitionFeatureSet, IFeatureDefinitionFeatureSetDynamic
    {
        public Func<FeatureDefinitionFeatureSet, List<FeatureDefinition>> DynamicFeatureSet { get; set; } =
            (x) => new List<FeatureDefinition>();
    }
}
