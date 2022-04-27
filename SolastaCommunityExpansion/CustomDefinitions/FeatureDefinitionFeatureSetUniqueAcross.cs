using System;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public class FeatureDefinitionFeatureSetUniqueAcross : FeatureDefinitionFeatureSet, IFeatureDefinitionFeatureSetDynamic
    {
        internal const string REMOVE_BEHAVIOR = "remove";

        public List<string> BehaviorTags { get; } = new();

        public Func<FeatureDefinitionFeatureSet, Dictionary<FeatureDefinition, string>> DynamicFeatureSet { get; set; } =
            (x) => new();
    }
}
