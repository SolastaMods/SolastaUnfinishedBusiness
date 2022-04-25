using System;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public class FeatureDefinitionFeatureSetWithPreRequisites : FeatureDefinitionFeatureSet, IFeatureDefinitionWithPrerequisites
    {
        public Func<bool> Validator { get; set; }
    }
}
