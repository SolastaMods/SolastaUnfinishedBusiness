using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.CustomDefinitions;

/**
     * Features using a shared pool should have UsesDetermination == Fixed.
     */
public sealed class FeatureDefinitionPowerSharedPool : FeatureDefinitionPower, IPowerSharedPool
{
    public FeatureDefinitionPower SharedPool { get; internal set; }

    public FeatureDefinitionPower GetUsagePoolPower()
    {
        return SharedPool;
    }
}

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
