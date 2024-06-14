using System;

namespace SolastaUnfinishedBusiness.Builders;

internal class DieTypeDefinitionBuilder : DefinitionBuilder<DieTypeDefinition, DieTypeDefinitionBuilder>
{
    protected DieTypeDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected DieTypeDefinitionBuilder(DieTypeDefinition original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
        Definition.rollingMeshReference = original.rollingMeshReference;
    }

    internal DieTypeDefinitionBuilder SetDieType(RuleDefinitions.DieType dieType)
    {
        Definition.dieType = dieType;
        return this;
    }
}
