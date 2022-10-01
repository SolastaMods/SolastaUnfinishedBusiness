using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

public class LimitedEffectInstances : ILimitedEffectInstances
{
    public delegate int GetEffectLimit(RulesetCharacter character);

    private readonly GetEffectLimit _handler;

    public LimitedEffectInstances(string name, GetEffectLimit handler)
    {
        _handler = handler;
        Name = name;
    }

    public string Name { get; }

    public int GetLimit(RulesetCharacter character)
    {
        return _handler(character);
    }
}
