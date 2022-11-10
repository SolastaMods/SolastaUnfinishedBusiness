using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomDefinitions;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class CustomInvocationDefinitionBuilder
    : InvocationDefinitionBuilder<InvocationDefinitionCustom, CustomInvocationDefinitionBuilder>
{
    internal CustomInvocationDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    internal CustomInvocationDefinitionBuilder(InvocationDefinitionCustom original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    internal CustomInvocationDefinitionBuilder SetPoolType(InvocationPoolTypeCustom poolType)
    {
        Definition.PoolType = poolType;
        return this;
    }
}
