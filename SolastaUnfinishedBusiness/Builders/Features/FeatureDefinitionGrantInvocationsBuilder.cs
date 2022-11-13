using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.CustomDefinitions;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionGrantInvocationsBuilder
    : DefinitionBuilder<FeatureDefinitionGrantInvocations, FeatureDefinitionGrantInvocationsBuilder>
{
#if false
    internal FeatureDefinitionGrantInvocationsBuilder SetInvocations(
        params InvocationDefinition[] invocations)
    {
        Definition.Invocations.SetRange(invocations);
        return this;
    }
#endif

    internal FeatureDefinitionGrantInvocationsBuilder SetInvocations(
        IEnumerable<InvocationDefinition> invocations)
    {
        Definition.Invocations.SetRange(invocations);
        return this;
    }

    #region Constructors

    internal FeatureDefinitionGrantInvocationsBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    internal FeatureDefinitionGrantInvocationsBuilder(
        FeatureDefinitionGrantInvocations original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
