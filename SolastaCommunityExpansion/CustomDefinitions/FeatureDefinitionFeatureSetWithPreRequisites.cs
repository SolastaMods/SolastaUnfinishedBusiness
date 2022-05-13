using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public class FeatureDefinitionFeatureSetWithPreRequisites : FeatureDefinitionFeatureSet, IFeatureDefinitionWithPrerequisites
    {
        public List<IFeatureDefinitionWithPrerequisites.Validate> Validators { get; set; }
    }
}
