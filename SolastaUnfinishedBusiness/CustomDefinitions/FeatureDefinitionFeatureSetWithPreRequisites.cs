using System.Collections.Generic;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

public sealed class FeatureDefinitionFeatureSetWithPreRequisites : FeatureDefinitionFeatureSet,
    IFeatureDefinitionWithPrerequisites
{
    public List<IFeatureDefinitionWithPrerequisites.Validate> Validators { get; set; }
}
