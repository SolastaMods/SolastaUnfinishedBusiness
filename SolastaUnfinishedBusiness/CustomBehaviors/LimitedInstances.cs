using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal class LimitedEffectInstances : ILimitedEffectInstances
{
    private readonly GetEffectLimit _handler;

    internal LimitedEffectInstances(string name, GetEffectLimit handler)
    {
        _handler = handler;
        Name = name;
    }

    public string Name { get; }

    public int GetLimit(RulesetCharacter character)
    {
        return _handler(character);
    }

    internal delegate int GetEffectLimit(RulesetCharacter character);
}
