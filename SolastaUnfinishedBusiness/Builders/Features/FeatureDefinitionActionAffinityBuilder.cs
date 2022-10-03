using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionActionAffinityBuilder : FeatureDefinitionBuilder<FeatureDefinitionActionAffinity,
    FeatureDefinitionActionAffinityBuilder>
{
    internal FeatureDefinitionActionAffinityBuilder SetAuthorizedActions(
        params ActionDefinitions.Id[] actions)
    {
        Definition.AuthorizedActions.SetRange(actions);
        Definition.AuthorizedActions.Sort();
        return This();
    }

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

    protected FeatureDefinitionActionAffinityBuilder(string name, string definitionGuid) : base(name,
        definitionGuid)
    {
    }

    protected FeatureDefinitionActionAffinityBuilder(FeatureDefinitionActionAffinity original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionActionAffinityBuilder(FeatureDefinitionActionAffinity original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    #endregion
}
