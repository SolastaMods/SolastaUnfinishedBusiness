using System;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public interface IFeatureDefinitionFeatureSetDynamic
    {
        public Func<FeatureDefinitionFeatureSetDynamic, List<FeatureDefinition>> DynamicFeatureSet { get; set; }
    }
}
