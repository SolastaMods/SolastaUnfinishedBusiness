using System.Linq;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

/**Adds power to character's usable powers when condition is applied and removes it when condition is removed*/
public class AddUsablePowerFromCondition : IOnConditionAddedOrRemoved
{
    private readonly FeatureDefinitionPower _power;

    public AddUsablePowerFromCondition(FeatureDefinitionPower power)
    {
        _power = power;
    }

    public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
    {
        if (target.UsablePowers.Any(u => u.PowerDefinition == _power))
        {
            return;
        }

        var usablePower = new RulesetUsablePower(_power, null, null);

        usablePower.Recharge();
        target.UsablePowers.Add(usablePower);
        target.RefreshUsablePower(usablePower);
    }

    public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
    {
        target.UsablePowers.RemoveAll(usablePower => usablePower.PowerDefinition == _power);
    }
}
