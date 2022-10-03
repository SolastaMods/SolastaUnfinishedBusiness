namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IPowerPoolModifier
{
    public FeatureDefinitionPower GetUsagePoolPower();
    public int PoolChangeAmount();
}
