using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomDefinitions;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionGrantCustomInvocationsBuilder : DefinitionBuilder<
    FeatureDefinitionGrantCustomInvocations,
    FeatureDefinitionGrantCustomInvocationsBuilder>
{
#if false
    internal FeatureDefinitionGrantCustomInvocationsBuilder SetInvocations(
        params CustomInvocationDefinition[] invocations)
    {
        Definition.Invocations.SetRange(invocations);
        return this;
    }
#endif

    #region Constructors

    internal FeatureDefinitionGrantCustomInvocationsBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    internal FeatureDefinitionGrantCustomInvocationsBuilder(
        FeatureDefinitionGrantCustomInvocations original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
