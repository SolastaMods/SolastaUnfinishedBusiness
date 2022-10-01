namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IPowerPoolModifier
{
    FeatureDefinitionPower GetUsagePoolPower();
    int PoolChangeAmount();
}
