using SolastaUnfinishedBusiness.CustomDefinitions;
using static ActionDefinitions;

namespace SolastaUnfinishedBusiness.Api.Extensions;

public static class InvocationDefinitionExtensions
{
    private static bool IsBonusAction(this InvocationDefinition invocation)
    {
        if (invocation.GrantedSpell != null)
        {
            return invocation.GrantedSpell.castingTime == RuleDefinitions.ActivationTime.BonusAction;
        }

        if (invocation.GrantedFeature is FeatureDefinitionPower power)
        {
            return power.ActivationTime == RuleDefinitions.ActivationTime.BonusAction;
        }

        return false;
    }

    internal static Id GetActionId(this InvocationDefinition invocation)
    {
        var isBonus = invocation.IsBonusAction();

        if (invocation is InvocationDefinitionCustom custom)
        {
            return isBonus ? custom.BonusActionId : custom.MainActionId;
        }

        return isBonus ? (Id)ExtraActionId.CastInvocationBonus : Id.CastInvocation;
    }

    internal static Id GetMainActionId(this InvocationDefinition invocation)
    {
        return invocation is InvocationDefinitionCustom custom
            ? custom.MainActionId
            : Id.CastInvocation;
    }
}
