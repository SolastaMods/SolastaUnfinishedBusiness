namespace SolastaUnfinishedBusiness.CustomDefinitions;

internal class FeatureDefinitionCustomInvocationPool : FeatureDefinition
{
    internal CustomInvocationPoolType PoolType { get; set; }

    internal int Points { get; set; }
    internal bool IsUnlearn { get; set; }
}
