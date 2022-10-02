using System;
using System.Diagnostics.CodeAnalysis;
using SolastaUnfinishedBusiness.Builders.Features;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

internal class CustomInvocationPoolDefinition : FeatureDefinition
{
    internal CustomInvocationPoolType PoolType { get; set; }

    internal int Points { get; set; }
    internal bool IsUnlearn { get; set; }

    /**Are level requirements in character levels or class levels?*/
    internal bool RequireClassLevels { get; set; }
}

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
internal class CustomInvocationPoolDefinitionBuilder : FeatureDefinitionBuilder
    <CustomInvocationPoolDefinition, CustomInvocationPoolDefinitionBuilder>
{
    protected CustomInvocationPoolDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected CustomInvocationPoolDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected CustomInvocationPoolDefinitionBuilder(CustomInvocationPoolDefinition original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    protected CustomInvocationPoolDefinitionBuilder(CustomInvocationPoolDefinition original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
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
