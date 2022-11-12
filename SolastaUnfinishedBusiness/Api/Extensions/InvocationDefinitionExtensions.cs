using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using static ActionDefinitions;

namespace SolastaUnfinishedBusiness.Api.Extensions;

public static class InvocationDefinitionExtensions
{
    internal static bool IsPermanent(this InvocationDefinition invocation)
    {
        return invocation.GrantedFeature != null && invocation.GetPower() == null;
    }

    internal static ActionType GetActionType(this InvocationDefinition invocation)
    {
        if (invocation.IsPermanent())
        {
            return ActionType.NoCost;
        }

        var time = RuleDefinitions.ActivationTime.Action;

        if (invocation.GrantedSpell != null)
        {
            time = invocation.GrantedSpell.ActivationTime;
        }

        if (invocation.GrantedFeature is FeatureDefinitionPower power)
        {
            time = power.ActivationTime;
        }

        switch (time)
        {
            case RuleDefinitions.ActivationTime.Action:
                return ActionType.Main;
            case RuleDefinitions.ActivationTime.BonusAction:
                return ActionType.Bonus;
            case RuleDefinitions.ActivationTime.NoCost:
                return ActionType.NoCost;
        }

        return ActionType.Main;
    }

    internal static Id GetActionId(this InvocationDefinition invocation)
    {
        if (invocation is InvocationDefinitionCustom custom)
        {
            return custom.BattleActionId;
        }

        var type = invocation.GetActionType();

        switch (type)
        {
            case ActionType.Main:
                return Id.CastInvocation;
            case ActionType.Bonus:
                return (Id)ExtraActionId.CastInvocationBonus;
            case ActionType.NoCost:
                return (Id)ExtraActionId.CastInvocationNoCost;
        }

        return Id.CastInvocation;
    }

    internal static Id GetMainActionId(this InvocationDefinition invocation)
    {
        return invocation is InvocationDefinitionCustom custom
            ? custom.MainActionId
            : Id.CastInvocation;
    }

    internal static FeatureDefinitionPower GetPower(this InvocationDefinition invocation)
    {
        if (invocation.GrantedFeature is FeatureDefinitionPower power)
        {
            return power.HasSubFeatureOfType<PowerFromInvocation>() ? power : null;
        }

        return null;
    }
}
