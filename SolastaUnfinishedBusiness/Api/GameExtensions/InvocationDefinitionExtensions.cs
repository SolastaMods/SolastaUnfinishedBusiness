using SolastaUnfinishedBusiness.Behaviors;
using static ActionDefinitions;

namespace SolastaUnfinishedBusiness.Api.GameExtensions;

public static class InvocationDefinitionExtensions
{
    internal static bool IsPermanent(this InvocationDefinition invocation)
    {
        return invocation.GrantedFeature && !invocation.GetPower();
    }

    internal static ActionType GetActionType(this InvocationDefinition invocation)
    {
        if (invocation.IsPermanent())
        {
            return ActionType.NoCost;
        }

        var time = RuleDefinitions.ActivationTime.Action;

        if (invocation.GrantedSpell)
        {
            time = invocation.GrantedSpell.ActivationTime;
        }

        if (invocation.GrantedFeature is FeatureDefinitionPower power)
        {
            time = power.ActivationTime;
        }

        return time switch
        {
            RuleDefinitions.ActivationTime.Action => ActionType.Main,
            RuleDefinitions.ActivationTime.BonusAction => ActionType.Bonus,
            RuleDefinitions.ActivationTime.NoCost => ActionType.NoCost,
            _ => ActionType.Main
        };
    }

    internal static Id GetActionId(this InvocationDefinition invocation)
    {
        if (invocation is InvocationDefinitionCustom custom)
        {
            return custom.BattleActionId;
        }

        var type = invocation.GetActionType();

        return type switch
        {
            ActionType.Main => Id.CastInvocation,
            ActionType.Bonus => (Id)ExtraActionId.CastInvocationBonus,
            ActionType.NoCost => (Id)ExtraActionId.CastInvocationNoCost,
            _ => Id.CastInvocation
        };
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
            return power.HasSubFeatureOfType<ModifyPowerFromInvocation>() ? power : null;
        }

        return null;
    }
}
