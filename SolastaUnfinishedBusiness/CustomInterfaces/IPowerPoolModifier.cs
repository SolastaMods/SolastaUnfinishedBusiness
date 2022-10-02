namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface IPowerPoolModifier
{
    FeatureDefinitionPower GetUsagePoolPower();
    int PoolChangeAmount();
}
