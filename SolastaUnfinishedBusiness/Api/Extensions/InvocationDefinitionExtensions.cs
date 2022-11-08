namespace SolastaUnfinishedBusiness.Api.Extensions;

public static class InvocationDefinitionExtensions
{
    internal static bool IsBonusAction(this InvocationDefinition invocation)
    {
        if (invocation.GrantedSpell != null)
        {
            return invocation.GrantedSpell.castingTime == RuleDefinitions.ActivationTime.BonusAction;
        }
        else if (invocation.GrantedFeature is FeatureDefinitionPower power)
        {
            return power.ActivationTime == RuleDefinitions.ActivationTime.BonusAction;
        }

        return false;
    }
}
