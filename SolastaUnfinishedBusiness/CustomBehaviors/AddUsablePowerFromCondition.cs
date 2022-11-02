using System.Linq;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

public class AddUsablePowerFromCondition : ICustomConditionFeature
{
    private readonly FeatureDefinitionPower power;

    public AddUsablePowerFromCondition(FeatureDefinitionPower power)
    {
        this.power = power;
    }

    public void ApplyFeature(RulesetCharacter hero)
    {
        if (hero.UsablePowers.Any(u => u.PowerDefinition == power))
        {
            return;
        }

        var usablePower = new RulesetUsablePower(power, null, null);
        usablePower.Recharge();
        hero.UsablePowers.Add(usablePower);
        hero.RefreshUsablePower(usablePower);
    }

    public void RemoveFeature(RulesetCharacter hero)
    {
        hero.UsablePowers.RemoveAll(usablePower => usablePower.PowerDefinition == power);
    }
}
