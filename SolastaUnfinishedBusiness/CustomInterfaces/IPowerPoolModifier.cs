namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface IPowerPoolModifier
{
    public FeatureDefinitionPower GetUsagePoolPower();
    public int PoolChangeAmount();
}
