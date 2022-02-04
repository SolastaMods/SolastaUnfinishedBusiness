namespace SolastaCommunityExpansion.CustomFeatureDefinitions
{
    public interface IPowerPoolModifier
    {
        FeatureDefinitionPower GetUsagePoolPower();
        int PoolChangeAmount();
    }
}
