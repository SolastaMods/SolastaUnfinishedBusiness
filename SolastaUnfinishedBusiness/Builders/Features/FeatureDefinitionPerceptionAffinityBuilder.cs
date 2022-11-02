using System;

namespace SolastaUnfinishedBusiness.Builders.Features;

internal class FeatureDefinitionPerceptionAffinityBuilder :
    FeatureDefinitionBuilder<FeatureDefinitionPerceptionAffinity, FeatureDefinitionPerceptionAffinityBuilder>
{
    internal FeatureDefinitionPerceptionAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    internal FeatureDefinitionPerceptionAffinityBuilder(FeatureDefinitionPerceptionAffinity original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    internal FeatureDefinitionPerceptionAffinityBuilder CannotBeSurprised(bool value = true)
    {
        Definition.cannotBeSurprised = value;
        return this;
    }
}
