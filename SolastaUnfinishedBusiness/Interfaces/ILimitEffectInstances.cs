namespace SolastaUnfinishedBusiness.Interfaces;

public interface ILimitEffectInstances
{
    public string Name { get; }
    public int GetLimit(RulesetCharacter character);
}
