using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using static ActionDefinitions;

namespace SolastaUnfinishedBusiness.Models;

public static class CustomActionIdContext
{
    internal static ActionDefinition CastInvocationBonus;

    internal static void Load()
    {
        if (DatabaseHelper.TryGetDefinition<ActionDefinition>("CastInvocation", out var baseAction))
        {
            CastInvocationBonus = ActionDefinitionBuilder
                .Create(baseAction, "CastInvocationBonus")
                .SetActionId(ExtraActionId.CastInvocationBonus)
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
