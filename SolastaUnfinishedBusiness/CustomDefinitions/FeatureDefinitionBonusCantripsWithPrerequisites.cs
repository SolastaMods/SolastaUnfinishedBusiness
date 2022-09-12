using System.Collections.Generic;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

public sealed class FeatureDefinitionBonusCantripsWithPrerequisites : FeatureDefinitionBonusCantrips,
    IFeatureDefinitionWithPrerequisites
{
    public List<IFeatureDefinitionWithPrerequisites.Validate> Validators { get; set; }
}
