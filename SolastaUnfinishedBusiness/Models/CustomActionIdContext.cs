using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomUI;
using static ActionDefinitions;

namespace SolastaUnfinishedBusiness.Models;

public static class CustomActionIdContext
{
    internal static void Load()
    {
        BuildCustomInvocationActions();
    }

    private static void BuildCustomInvocationActions()
    {
        if (!DatabaseHelper.TryGetDefinition<ActionDefinition>("CastInvocation", out var baseAction))
        {
            return;
        }

        ActionDefinitionBuilder
            .Create(baseAction, "CastInvocationBonus")
            .SetActionId(ExtraActionId.CastInvocationBonus)
            .SetActionType(ActionType.Bonus)
            .SetActionScope(ActionScope.Battle)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(baseAction, "CastInvocationNoCost")
            .SetActionId(ExtraActionId.CastInvocationNoCost)
            .SetActionType(ActionType.NoCost)
            .SetActionScope(ActionScope.Battle)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(baseAction, "CastPlaneMagicMain")
            .SetGuiPresentation("CastPlaneMagic", Category.Action, Sprites.ActionPlaneMagic, 10)
            .SetActionId(ExtraActionId.CastPlaneMagicMain)
            .SetActionType(ActionType.Main)
            .SetActionScope(ActionScope.All)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(baseAction, "CastPlaneMagicBonus")
            .SetGuiPresentation("CastPlaneMagic", Category.Action, Sprites.ActionPlaneMagic, 41)
            .SetActionId(ExtraActionId.CastPlaneMagicBonus)
            .SetActionType(ActionType.Bonus)
            .SetActionScope(ActionScope.Battle)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(baseAction, "InventorInfusion")
            .SetGuiPresentation(Category.Action, Sprites.ActionInfuse, 20)
            .SetActionId(ExtraActionId.InventorInfusion)
            .SetActionType(ActionType.Main)
            .SetActionScope(ActionScope.Exploration)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(baseAction, "TacticianGambitMain")
            .SetGuiPresentation("TacticianGambit", Category.Action, Sprites.ActionGambit, 20)
            .SetActionId(ExtraActionId.TacticianGambitMain)
            .SetActionType(ActionType.Main)
            .SetActionScope(ActionScope.All)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(baseAction, "TacticianGambitBonus")
            .SetGuiPresentation("TacticianGambit", Category.Action, Sprites.ActionGambit, 20)
            .SetActionId(ExtraActionId.TacticianGambitBonus)
            .SetActionType(ActionType.Bonus)
            .SetActionScope(ActionScope.Battle)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(baseAction, "TacticianGambitNoCost")
            .SetGuiPresentation("TacticianGambit", Category.Action, Sprites.ActionGambit, 20)
            .SetActionId(ExtraActionId.TacticianGambitNoCost)
            .SetActionType(ActionType.NoCost)
            .SetActionScope(ActionScope.Battle)
            .AddToDB();
    }

    public static void ProcessCustomActionIds(
        GameLocationCharacter locationCharacter,
        ref ActionStatus result,
        Id actionId,
        ActionScope scope,
        ActionStatus actionTypeStatus,
        RulesetAttackMode optionalAttackMode,
        bool ignoreMovePoints,
        bool allowUsingDelegatedPowersAsPowers)
    {
        var isInvocationAction = IsInvocationActionId(actionId);

        if (!isInvocationAction)
        {
            return;
        }

        var action = ServiceRepository.GetService<IGameLocationActionService>().AllActionDefinitions[actionId];
        var actionType = action.actionType;
        var character = locationCharacter.RulesetCharacter;

        if (actionTypeStatus == ActionStatus.Irrelevant)
        {
            actionTypeStatus = locationCharacter.GetActionTypeStatus(action.ActionType, scope, ignoreMovePoints);
        }

        if (actionTypeStatus != ActionStatus.Available)
        {
            result = actionTypeStatus == ActionStatus.Spent ? ActionStatus.Unavailable : actionTypeStatus;
            return;
        }

        if (action.ActionScope != ActionScope.All && action.ActionScope != scope)
        {
            result = ActionStatus.Unavailable;
            return;
        }

        if (action.UsesPerTurn > 0)
        {
            var name = action.Name;

            if (locationCharacter.UsedSpecialFeatures.ContainsKey(name)
                && locationCharacter.UsedSpecialFeatures[name] >= action.UsesPerTurn)
            {
                result = ActionStatus.Unavailable;
                return;
            }
        }

        var index = locationCharacter.currentActionRankByType[actionType];
        var actionPerformanceFilters = locationCharacter.actionPerformancesByType[actionType];

        if (action.RequiresAuthorization)
        {
            if (index >= actionPerformanceFilters.Count
                || !actionPerformanceFilters[index].AuthorizedActions.Contains(actionId))
            {
                result = ActionStatus.Unavailable;
                return;
            }
        }
        else if (index >= actionPerformanceFilters.Count)
        {
            result = ActionStatus.Unavailable;
            return;
        }

        var canCastSpells = character.CanCastSpells();
        var canOnlyUseCantrips = scope == ActionScope.Battle && locationCharacter.CanOnlyUseCantrips;

        if (isInvocationAction)
        {
            result = CanUseInvocationAction(actionId, scope, character, canCastSpells, canOnlyUseCantrips);
        }
    }

    private static ActionStatus CanUseInvocationAction(Id actionId, ActionScope scope,
        RulesetCharacter character, bool canCastSpells, bool canOnlyUseCantrips)
    {
        return character.CanCastAnyInvocationOfActionId(actionId, scope, canCastSpells, canOnlyUseCantrips)
            ? ActionStatus.Available
            : ActionStatus.Unavailable;
    }

    internal static bool IsInvocationActionId(Id id)
    {
        var extra = (ExtraActionId)id;

        //TODO: consider adding all invocation actions to a list and check it here
        return id is Id.CastInvocation
               || extra is ExtraActionId.CastInvocationBonus
                   or ExtraActionId.CastInvocationNoCost
                   or ExtraActionId.InventorInfusion
                   or ExtraActionId.TacticianGambitMain
                   or ExtraActionId.TacticianGambitBonus
                   or ExtraActionId.TacticianGambitNoCost
                   or ExtraActionId.CastPlaneMagicMain
                   or ExtraActionId.CastPlaneMagicBonus;
    }
}
