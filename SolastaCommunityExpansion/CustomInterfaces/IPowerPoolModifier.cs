namespace SolastaCommunityExpansion.CustomInterfaces
{
    public interface IPowerPoolModifier
    {
        FeatureDefinitionPower GetUsagePoolPower();
        int PoolChangeAmount();
    }
}
