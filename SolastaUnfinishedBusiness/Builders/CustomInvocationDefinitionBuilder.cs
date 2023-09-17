using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomDefinitions;

namespace SolastaUnfinishedBusiness.Builders;

internal delegate bool IsInvocationValidHandler(RulesetCharacter character, InvocationDefinition invocation);

[UsedImplicitly]
internal class CustomInvocationDefinitionBuilder
    : InvocationDefinitionBuilder<InvocationValidateDefinitionCustom, CustomInvocationDefinitionBuilder>
{
    internal CustomInvocationDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    internal CustomInvocationDefinitionBuilder(InvocationValidateDefinitionCustom original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    internal CustomInvocationDefinitionBuilder SetPoolType(InvocationPoolTypeCustom poolType)
    {
        Definition.PoolType = poolType;
        return this;
    }
}
