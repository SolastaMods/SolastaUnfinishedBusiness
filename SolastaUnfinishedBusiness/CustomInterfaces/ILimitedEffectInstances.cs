namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface ILimitedEffectInstances
{
    public string Name { get; }
    public int GetLimit(RulesetCharacter character);
}
