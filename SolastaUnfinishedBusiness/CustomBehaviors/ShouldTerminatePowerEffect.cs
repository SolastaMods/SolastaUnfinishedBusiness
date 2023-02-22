using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal class ShouldTerminatePowerEffect : IShouldTerminateEffect
{
    private readonly string _powerName;

    public ShouldTerminatePowerEffect(string powerName)
    {
        _powerName = powerName;
    }

    public bool Validate(RulesetEffect rulesetEffect)
    {
        if (rulesetEffect is not RulesetEffectPower rulesetEffectPower ||
            rulesetEffectPower.PowerDefinition.Name != _powerName)
        {
            return false;
        }

        return !rulesetEffectPower.user.IsUnconscious;
    }
}
