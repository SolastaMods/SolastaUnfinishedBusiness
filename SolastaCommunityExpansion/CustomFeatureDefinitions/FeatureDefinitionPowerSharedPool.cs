namespace SolastaCommunityExpansion.CustomFeatureDefinitions
{
    /**
     * Features using a shared pool should have UsesDetermination == Fixed.
     */
    public class FeatureDefinitionPowerSharedPool : FeatureDefinitionPower, IPowerSharedPool
    {
        public FeatureDefinitionPower SharedPool { get; internal set; }

        public FeatureDefinitionPower GetUsagePoolPower()
        {
            return SharedPool;
        }
    }

    public class FeatureDefinitionPowerPoolModifier : FeatureDefinitionPower, IPowerPoolModifier
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
}
