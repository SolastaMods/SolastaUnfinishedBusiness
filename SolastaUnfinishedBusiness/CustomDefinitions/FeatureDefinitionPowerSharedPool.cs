using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

// Features using a shared pool should have UsesDetermination == Fixed.
internal sealed class FeatureDefinitionPowerSharedPool : FeatureDefinitionPower, IPowerSharedPool
{
    internal BaseDefinition SourceDefinition { get; set; }
    internal FeatureDefinitionPower SharedPool { get; set; }

    public FeatureDefinitionPower GetUsagePoolPower()
    {
        return SharedPool;
    }
}
