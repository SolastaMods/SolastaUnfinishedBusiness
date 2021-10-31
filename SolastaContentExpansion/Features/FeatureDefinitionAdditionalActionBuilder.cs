using SolastaModApi;
using SolastaModApi.Extensions;
using System.Collections.Generic;

namespace SolastaContentExpansion.Features
{
    public class FeatureDefinitionAdditionalActionBuilder : BaseDefinitionBuilder<FeatureDefinitionAdditionalAction>
    {
        public FeatureDefinitionAdditionalActionBuilder(string name, string guid, ActionDefinitions.ActionType actionType,
            List<ActionDefinitions.Id> forbiddenActions, List<ActionDefinitions.Id> authorizedActions,
            List<ActionDefinitions.Id> restrictedActions, int maxAttacksNumber, RuleDefinitions.AdditionalActionTriggerCondition triggerCondition,
            GuiPresentation guiPresentation) : base(name, guid)
        {
            Definition.SetActionType(actionType);
            Definition.ForbiddenActions.AddRange(forbiddenActions);
            Definition.AuthorizedActions.AddRange(authorizedActions);
            Definition.RestrictedActions.AddRange(restrictedActions);
            Definition.SetMaxAttacksNumber(maxAttacksNumber);
            Definition.SetTriggerCondition(triggerCondition);
            Definition.SetGuiPresentation(guiPresentation);
        }
    }
}
