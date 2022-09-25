using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionAdditionalActionBuilder : FeatureDefinitionBuilder<FeatureDefinitionAdditionalAction,
    FeatureDefinitionAdditionalActionBuilder>
{
    public FeatureDefinitionAdditionalActionBuilder SetActionType(ActionDefinitions.ActionType actionType)
    {
        Definition.actionType = actionType;
        return this;
    }

    public FeatureDefinitionAdditionalActionBuilder SetMaxAttacksNumber(int maxAttacksNumber)
    {
        Definition.maxAttacksNumber = maxAttacksNumber;
        return this;
    }

    public FeatureDefinitionAdditionalActionBuilder SetTriggerCondition(
        RuleDefinitions.AdditionalActionTriggerCondition triggerCondition)
    {
        Definition.triggerCondition = triggerCondition;
        return this;
    }

    public FeatureDefinitionAdditionalActionBuilder SetRestrictedActions(
        params ActionDefinitions.Id[] restrictedActions)
    {
        return SetRestrictedActions(restrictedActions.AsEnumerable());
    }

    public FeatureDefinitionAdditionalActionBuilder SetRestrictedActions(
        IEnumerable<ActionDefinitions.Id> restrictedActions)
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
