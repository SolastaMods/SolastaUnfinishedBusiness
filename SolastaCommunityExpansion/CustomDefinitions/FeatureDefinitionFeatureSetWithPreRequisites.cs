using System.Collections.Generic;
using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.CustomDefinitions;

public sealed class FeatureDefinitionFeatureSetWithPreRequisites : FeatureDefinitionFeatureSet,
    IFeatureDefinitionWithPrerequisites
{
    public List<IFeatureDefinitionWithPrerequisites.Validate> Validators { get; set; }
}
