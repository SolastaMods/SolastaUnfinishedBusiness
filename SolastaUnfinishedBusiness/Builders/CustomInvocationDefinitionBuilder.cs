using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomDefinitions;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class CustomInvocationDefinitionBuilder
    : InvocationDefinitionBuilder<CustomInvocationDefinition, CustomInvocationDefinitionBuilder>
{
    internal CustomInvocationDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    internal CustomInvocationDefinitionBuilder(CustomInvocationDefinition original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    internal CustomInvocationDefinitionBuilder SetPoolType(CustomInvocationPoolType poolType)
    {
        Definition.PoolType = poolType;
        return this;
    }
}
