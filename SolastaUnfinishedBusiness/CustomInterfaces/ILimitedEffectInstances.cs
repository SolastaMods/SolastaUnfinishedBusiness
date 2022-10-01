namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface ILimitedEffectInstances
{
    string Name { get; }
    int GetLimit(RulesetCharacter character);
}
