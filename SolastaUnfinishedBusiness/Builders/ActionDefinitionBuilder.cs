using System;
using SolastaUnfinishedBusiness.Api.Extensions;

namespace SolastaUnfinishedBusiness.Builders;

internal class ActionDefinitionBuilder : DefinitionBuilder<ActionDefinition, ActionDefinitionBuilder>
{
    internal ActionDefinitionBuilder SetActionId(ActionDefinitions.Id id)
    {
        Definition.id = id;
        return this;
    }

    internal ActionDefinitionBuilder SetActionId(ExtraActionId id)
    {
        return SetActionId((ActionDefinitions.Id)id);
    }

    internal ActionDefinitionBuilder SetActionType(ActionDefinitions.ActionType type)
    {
        Definition.actionType = type;
        return this;
    }

    internal ActionDefinitionBuilder SetActionScope(ActionDefinitions.ActionScope scope)
    {
        Definition.actionScope = scope;
        return this;
    }

    #region Constructors

    internal ActionDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    internal ActionDefinitionBuilder(ActionDefinition original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
