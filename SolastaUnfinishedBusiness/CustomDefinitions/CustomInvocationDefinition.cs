using System;
using System.Collections.Generic;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

public class CustomInvocationDefinition : InvocationDefinition, IFeatureDefinitionWithPrerequisites
{
    public string PoolType { get; set; }

    //TODO: add validator setter
    public List<IFeatureDefinitionWithPrerequisites.Validate> Validators { get; }
}

public class CustomInvocationDefinitionBuilder : InvocationDefinitionBuilder<CustomInvocationDefinition,
    CustomInvocationDefinitionBuilder>
{
    internal CustomInvocationDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    internal CustomInvocationDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    internal CustomInvocationDefinitionBuilder(CustomInvocationDefinition original, string name, Guid namespaceGuid) :
        base(
            original, name, namespaceGuid)
    {
    }

    internal CustomInvocationDefinitionBuilder(CustomInvocationDefinition original, string name, string definitionGuid)
        :
        base(original, name, definitionGuid)
    {
    }

    public CustomInvocationDefinitionBuilder SetPoolType(CustomInvocationPoolType poolType)
    {
        Definition.PoolType = poolType.Name;
        return this;
    }
}
