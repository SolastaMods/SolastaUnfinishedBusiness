using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

public sealed class FeatureDefinitionPowerPoolModifier : FeatureDefinitionPower, IPowerPoolModifier
{
    public FeatureDefinitionPower PoolPower { get; set; }

    public FeatureDefinitionPower GetUsagePoolPower()
    {
        return PoolPower;
    }

    public int PoolChangeAmount()
    {
        return 0;
    }
}
