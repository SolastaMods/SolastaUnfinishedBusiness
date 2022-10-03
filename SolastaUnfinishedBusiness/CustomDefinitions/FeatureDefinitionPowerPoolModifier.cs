using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

internal sealed class FeatureDefinitionPowerPoolModifier : FeatureDefinitionPower, IPowerPoolModifier
{
    internal FeatureDefinitionPower PoolPower { get; set; }

    public FeatureDefinitionPower GetUsagePoolPower()
    {
        return PoolPower;
    }

    public int PoolChangeAmount()
    {
        return 0;
    }
}
