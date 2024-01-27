// ReSharper disable once CheckNamespace

internal class FeatureDefinitionCustomInvocationPool : FeatureDefinition
{
    internal InvocationPoolTypeCustom PoolType { get; set; }

    internal int Points { get; set; }
    internal bool IsUnlearn { get; set; }
}
