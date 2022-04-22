using SolastaCommunityExpansion.CustomDefinitions;

namespace SolastaCommunityExpansion.Builders
{
    public class FeatureDefinitionFreeBonusCantrips : FeatureDefinitionBonusCantrips, IPointPoolMaxBonus
    {
        public int MaxPointsBonus { get => BonusCantrips.Count; }
        public HeroDefinitions.PointsPoolType PoolType { get => HeroDefinitions.PointsPoolType.Cantrip; }
    }
}
