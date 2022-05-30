using System;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders;

public class ActionDefinitionBuilder : DefinitionBuilder<ActionDefinition, ActionDefinitionBuilder>
{
    public ActionDefinitionBuilder SetId(ActionDefinitions.Id value)
    {
        Definition.SetId(value);
        return this;
    }

    #region Constructors

    protected ActionDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected ActionDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected ActionDefinitionBuilder(ActionDefinition original, string name, Guid namespaceGuid) : base(original,
        name, namespaceGuid)
    {
    }

    protected ActionDefinitionBuilder(ActionDefinition original, string name, string definitionGuid) : base(
        original, name, definitionGuid)
    {
    }

    #endregion
}
