namespace SolastaCommunityExpansion.CustomDefinitions
{
    public interface IPowerPoolModifier
    {
        FeatureDefinitionPower GetUsagePoolPower();
        int PoolChangeAmount();
    }
}
