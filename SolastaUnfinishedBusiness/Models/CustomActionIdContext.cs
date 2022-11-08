using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;

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
                .SetActionType(ActionDefinitions.ActionType.Bonus)
                .AddToDB();
        }
    }
}


