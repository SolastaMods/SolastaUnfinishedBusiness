using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

// Features using a shared pool should have UsesDetermination == Fixed.
public sealed class FeatureDefinitionPowerSharedPool : FeatureDefinitionPower, IPowerSharedPool
{
    public FeatureDefinitionPower SharedPool { get; internal set; }

    public FeatureDefinitionPower GetUsagePoolPower()
    {
        return SharedPool;
    }
}
