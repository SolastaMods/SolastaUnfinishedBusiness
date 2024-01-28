using SolastaUnfinishedBusiness.Interfaces;

namespace SolastaUnfinishedBusiness.Behaviors;

internal class LimitEffectInstances : ILimitEffectInstances
{
    private readonly GetEffectLimit _handler;

    internal LimitEffectInstances(string name, GetEffectLimit handler)
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
