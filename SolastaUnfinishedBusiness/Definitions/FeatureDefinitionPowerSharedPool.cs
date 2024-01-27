// ReSharper disable once CheckNamespace


// Features using a shared pool should have UsesDetermination == Fixed.

internal sealed class FeatureDefinitionPowerSharedPool : FeatureDefinitionPower
{
    internal BaseDefinition SourceDefinition { get; set; }
    internal FeatureDefinitionPower SharedPool { get; set; }

    public FeatureDefinitionPower GetUsagePoolPower()
    {
        return SharedPool;
    }
}
