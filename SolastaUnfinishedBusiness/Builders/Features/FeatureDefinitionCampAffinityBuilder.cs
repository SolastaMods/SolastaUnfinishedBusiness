using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
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
