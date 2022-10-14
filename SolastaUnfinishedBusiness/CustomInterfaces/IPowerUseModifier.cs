namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface IPowerUseModifier
{
    public FeatureDefinitionPower PowerPool { get; }
    public int PoolChangeAmount(RulesetCharacter character);
}
