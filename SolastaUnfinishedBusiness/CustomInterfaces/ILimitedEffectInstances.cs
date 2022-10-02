namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface ILimitedEffectInstances
{
    public string Name { get; }
    public int GetLimit(RulesetCharacter character);
}
