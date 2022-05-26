using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    /**
     * FeatureDefinitionPointPool but it will add bonus points to the pool, so next levels these selections won't count.
     * Mostly useful for bonus spell/cantrip selections that should not affect regular spell/cantrip progression
     */
    public class FeatureDefinitionPointPoolWithBonus : FeatureDefinitionPointPool, IPointPoolMaxBonus
    {
        public int MaxPointsBonus => PoolAmount;
    }
}
