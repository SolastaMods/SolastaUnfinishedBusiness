using System;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public interface IFeatureDefinitionFeatureSetDynamic
    {
        public Func<FeatureDefinitionFeatureSet, List<FeatureDefinition>> DynamicFeatureSet { get; set; }
    }
}
