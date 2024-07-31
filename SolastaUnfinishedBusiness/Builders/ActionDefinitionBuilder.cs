using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class ActionDefinitionBuilder : DefinitionBuilder<ActionDefinition, ActionDefinitionBuilder>
{
    internal ActionDefinitionBuilder RequiresAuthorization(bool value = true)
    {
        Definition.requiresAuthorization = value;
        return this;
    }

    private ActionDefinitionBuilder SetActionId(ActionDefinitions.Id id)
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

    internal ActionDefinitionBuilder SetStealthBreakerBehavior(
        ActionDefinitions.StealthBreakerBehavior stealthBreakerBehavior)
    {
        Definition.stealthBreakerBehavior = stealthBreakerBehavior;
        return this;
    }

    internal ActionDefinitionBuilder SetFormType(ActionDefinitions.ActionFormType form)
    {
        Definition.formType = form;
        return this;
    }

    public ActionDefinitionBuilder OverrideClassName(string name)
    {
        Definition.classNameOverride = name;
        return this;
    }

    internal ActionDefinitionBuilder SetActivatedPower(
        FeatureDefinitionPower power,
        ActionDefinitions.ActionParameter parameter = ActionDefinitions.ActionParameter.ActivatePower,
        bool usePowerTooltip = true)
    {
        Definition.activatedPower = power;
        Definition.displayPowerTooltip = usePowerTooltip;
        Definition.parameter = parameter;
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
