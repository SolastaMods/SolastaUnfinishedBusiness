using System;

namespace SolastaUnfinishedBusiness.Builders.Features;

internal class FeatureDefinitionCampAffinityBuilder : FeatureDefinitionBuilder<FeatureDefinitionCampAffinity,
    FeatureDefinitionCampAffinityBuilder>

{
    internal FeatureDefinitionCampAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    internal FeatureDefinitionCampAffinityBuilder(FeatureDefinitionCampAffinity original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }
}
