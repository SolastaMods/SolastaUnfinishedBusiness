namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface ILimitEffectInstances
{
    public string Name { get; }
    public int GetLimit(RulesetCharacter character);
}
