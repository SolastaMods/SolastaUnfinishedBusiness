using System;
using System.Diagnostics.CodeAnalysis;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomDefinitions;

namespace SolastaUnfinishedBusiness.Builders;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
internal class CustomInvocationPoolDefinitionBuilder 
    : DefinitionBuilder<CustomInvocationPoolDefinition, CustomInvocationPoolDefinitionBuilder>
{
    protected CustomInvocationPoolDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected CustomInvocationPoolDefinitionBuilder(CustomInvocationPoolDefinition original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    internal CustomInvocationPoolDefinitionBuilder Setup(CustomInvocationPoolType poolType, int points = 1,
        bool isUnlearn = false)
    {
        Definition.PoolType = poolType;
        Definition.Points = points;
        Definition.IsUnlearn = isUnlearn;
        return this;
    }
}
