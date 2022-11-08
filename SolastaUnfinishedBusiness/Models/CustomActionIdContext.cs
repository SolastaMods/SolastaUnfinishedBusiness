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
        if (DatabaseHelper.TryGetDefinition<ActionDefinition>("CastInvocation", out var baseAction))
        {
            ActionDefinitionBuilder
                .Create(baseAction, "CastInvocationBonus")
                .SetActionId(ExtraActionId.CastInvocationBonus)
                .SetActionType(ActionType.Bonus)
                .SetActionScope(ActionScope.Battle)
                .AddToDB();

            ActionDefinitionBuilder
                .Create(baseAction, "CastPlaneMagicMain")
                .SetGuiPresentation("CastPlaneMagic", Category.Action, Sprites.ActionPlaneMagic, sortOrder: 10)
                .SetActionId(ExtraActionId.CastPlaneMagicMain)
                .SetActionType(ActionType.Main)
                .SetActionScope(ActionScope.All)
                .AddToDB();

            ActionDefinitionBuilder
                .Create(baseAction, "CastPlaneMagicBonus")
                .SetGuiPresentation("CastPlaneMagic", Category.Action, Sprites.ActionPlaneMagic, sortOrder: 41)
                .SetActionId(ExtraActionId.CastPlaneMagicBonus)
                .SetActionType(ActionType.Bonus)
                .SetActionScope(ActionScope.Battle)
                .AddToDB();
        }
    }

    internal static bool IsInvocationActionId(Id id)
    {
        var extra = (ExtraActionId)id;
        return id is Id.CastInvocation
               || extra is ExtraActionId.CastInvocationBonus
                   or ExtraActionId.CastPlaneMagicMain
                   or ExtraActionId.CastPlaneMagicBonus;
    }
}
