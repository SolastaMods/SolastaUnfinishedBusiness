using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionActionAffinityBuilder : FeatureDefinitionBuilder<FeatureDefinitionActionAffinity,
    FeatureDefinitionActionAffinityBuilder>
{
    internal FeatureDefinitionActionAffinityBuilder SetAuthorizedActions(params ActionDefinitions.Id[] actions)
    {
        Definition.AuthorizedActions.SetRange(actions);
        Definition.AuthorizedActions.Sort();
        return This();
    }

    internal FeatureDefinitionActionAffinityBuilder SetForbiddenActions(params ActionDefinitions.Id[] actions)
    {
        Definition.ForbiddenActions.SetRange(actions);
        Definition.ForbiddenActions.Sort();
        return This();
    }

#if false
    internal FeatureDefinitionActionAffinityBuilder SetRestrictedActions(params ActionDefinitions.Id[] actions)
    {
        Definition.RestrictedActions.SetRange(actions);
        Definition.RestrictedActions.Sort();
        return This();
    }
#endif

    internal FeatureDefinitionActionAffinityBuilder SetActionExecutionModifiers(
        params ActionDefinitions.ActionExecutionModifier[] modifiers)
    {
        Definition.ActionExecutionModifiers.SetRange(modifiers);
        return This();
    }

    internal FeatureDefinitionActionAffinityBuilder SetDefaultAllowedActonTypes()
    {
        Definition.AllowedActionTypes = new[] { true, true, true, true, true, true };
        return This();
    }

    #region Constructors

    protected FeatureDefinitionActionAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionActionAffinityBuilder(FeatureDefinitionActionAffinity original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
