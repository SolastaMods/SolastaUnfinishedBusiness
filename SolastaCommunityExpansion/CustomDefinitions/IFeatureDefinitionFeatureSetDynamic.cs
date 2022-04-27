using System;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public interface IFeatureDefinitionFeatureSetDynamic
    {
        public Func<FeatureDefinitionFeatureSet, Dictionary<FeatureDefinition, string>> DynamicFeatureSet { get; set; }
    }
}
