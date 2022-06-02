using System.Collections.Generic;
using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.CustomDefinitions;

public class FeatureDefinitionBonusCantripsWithPrerequisites : FeatureDefinitionBonusCantrips,
    IFeatureDefinitionWithPrerequisites
{
    public List<IFeatureDefinitionWithPrerequisites.Validate> Validators { get; set; }
}
