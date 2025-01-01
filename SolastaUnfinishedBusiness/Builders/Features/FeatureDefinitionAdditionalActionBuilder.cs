using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionAdditionalActionBuilder
    : DefinitionBuilder<FeatureDefinitionAdditionalAction, FeatureDefinitionAdditionalActionBuilder>
{
    internal FeatureDefinitionAdditionalActionBuilder SetActionType(ActionDefinitions.ActionType actionType)
    {
        Definition.actionType = actionType;
        return this;
    }

    internal FeatureDefinitionAdditionalActionBuilder SetMaxAttacksNumber(int maxAttacksNumber)
    {
        Definition.maxAttacksNumber = maxAttacksNumber;
        return this;
    }

    internal FeatureDefinitionAdditionalActionBuilder SetRestrictedActions(
        params ActionDefinitions.Id[] restrictedActions)
    {
        Definition.RestrictedActions.SetRange(restrictedActions);
        Definition.RestrictedActions.Sort();
        return this;
    }

    internal FeatureDefinitionAdditionalActionBuilder SetForbiddenActions(
        params ActionDefinitions.Id[] forbiddenActions)
    {
        Definition.ForbiddenActions.SetRange(forbiddenActions);
        Definition.ForbiddenActions.Sort();
        return this;
    }

    #region Constructors

    protected FeatureDefinitionAdditionalActionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionAdditionalActionBuilder(FeatureDefinitionAdditionalAction original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
