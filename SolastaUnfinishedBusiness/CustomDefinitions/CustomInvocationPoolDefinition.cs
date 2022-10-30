namespace SolastaUnfinishedBusiness.CustomDefinitions;

internal class CustomInvocationPoolDefinition : FeatureDefinition
{
    internal CustomInvocationPoolType PoolType { get; set; }

    internal int Points { get; set; }
    internal bool IsUnlearn { get; set; }

    /**Are level requirements in character levels or class levels?*/
    internal bool RequireClassLevels { get; set; }
}
