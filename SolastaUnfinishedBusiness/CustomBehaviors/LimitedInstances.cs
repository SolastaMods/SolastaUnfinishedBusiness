namespace SolastaUnfinishedBusiness.CustomBehaviors;

public interface ILimitedEffectInstances
{
    string Name { get; }
    int GetLimit(RulesetCharacter character);
}

public class LimitedEffectInstances : ILimitedEffectInstances
{
    public delegate int GetEffectLimit(RulesetCharacter character);

    private readonly GetEffectLimit _handler;
    public string Name { get; }

    public LimitedEffectInstances(string name, GetEffectLimit handler)
    {
        _handler = handler;
        Name = name;
    }

    public int GetLimit(RulesetCharacter character)
    {
        return _handler(character);
    }
}
