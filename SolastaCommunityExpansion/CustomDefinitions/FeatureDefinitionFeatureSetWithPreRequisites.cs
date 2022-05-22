using System.Collections.Generic;
using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public class FeatureDefinitionFeatureSetWithPreRequisites : FeatureDefinitionFeatureSet, IFeatureDefinitionWithPrerequisites
    {
        public List<IFeatureDefinitionWithPrerequisites.Validate> Validators { get; set; }
    }
}
