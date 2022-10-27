namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IPowerUseModifier
{
    public FeatureDefinitionPower PowerPool { get; }
    public int PoolChangeAmount(RulesetCharacter character);
}
