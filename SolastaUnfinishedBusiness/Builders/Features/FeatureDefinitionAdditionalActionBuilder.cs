using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionAdditionalActionBuilder : FeatureDefinitionBuilder<FeatureDefinitionAdditionalAction,
    FeatureDefinitionAdditionalActionBuilder>
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

    internal FeatureDefinitionAdditionalActionBuilder SetTriggerCondition(
        RuleDefinitions.AdditionalActionTriggerCondition triggerCondition)
    {
        Definition.triggerCondition = triggerCondition;
        return this;
    }

    internal FeatureDefinitionAdditionalActionBuilder SetRestrictedActions(
        params ActionDefinitions.Id[] restrictedActions)
    {
        Definition.RestrictedActions.SetRange(restrictedActions);
        Definition.RestrictedActions.Sort();
        return this;
    }

    #region Constructors

    protected FeatureDefinitionAdditionalActionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionAdditionalActionBuilder(string name, string definitionGuid) : base(name,
        definitionGuid)
    {
    }

    protected FeatureDefinitionAdditionalActionBuilder(FeatureDefinitionAdditionalAction original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionAdditionalActionBuilder(FeatureDefinitionAdditionalAction original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    #endregion
}
