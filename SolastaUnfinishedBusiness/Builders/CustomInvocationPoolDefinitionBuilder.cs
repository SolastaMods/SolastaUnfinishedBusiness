using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Definitions;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class CustomInvocationPoolDefinitionBuilder
    : DefinitionBuilder<FeatureDefinitionCustomInvocationPool, CustomInvocationPoolDefinitionBuilder>
{
    protected CustomInvocationPoolDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected CustomInvocationPoolDefinitionBuilder(FeatureDefinitionCustomInvocationPool original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    internal CustomInvocationPoolDefinitionBuilder Setup(InvocationPoolTypeCustom poolType, int points = 1,
        bool isUnlearn = false)
    {
        Definition.PoolType = poolType;
        Definition.Points = points;
        Definition.IsUnlearn = isUnlearn;
        return this;
    }
}
