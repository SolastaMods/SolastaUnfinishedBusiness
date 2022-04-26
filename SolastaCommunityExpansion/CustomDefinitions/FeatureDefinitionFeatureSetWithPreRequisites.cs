using System;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public class FeatureDefinitionFeatureSetWithPreRequisites : FeatureDefinitionFeatureSet, IFeatureDefinitionWithPrerequisites
    {
        public List<Func<bool>> Validators { get; set; } = null;
    }
}
