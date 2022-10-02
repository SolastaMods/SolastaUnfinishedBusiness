namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface ILimitedEffectInstances
{
    string Name { get; }
    int GetLimit(RulesetCharacter character);
}
