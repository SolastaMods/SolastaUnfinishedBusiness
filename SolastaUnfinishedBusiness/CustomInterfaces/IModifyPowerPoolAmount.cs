namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IModifyPowerPoolAmount
{
    public FeatureDefinitionPower PowerPool { get; }
    public int PoolChangeAmount(RulesetCharacter character);
}
