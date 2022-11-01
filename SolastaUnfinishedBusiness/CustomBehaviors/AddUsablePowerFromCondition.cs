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
        var usablePower = new RulesetUsablePower(power, null, null);
        hero.UsablePowers.TryAdd(usablePower);
        hero.RefreshUsablePower(usablePower);
    }

    public void RemoveFeature(RulesetCharacter hero)
    {
        hero.UsablePowers.RemoveAll(usablePower => usablePower.PowerDefinition == power);
    }
}
