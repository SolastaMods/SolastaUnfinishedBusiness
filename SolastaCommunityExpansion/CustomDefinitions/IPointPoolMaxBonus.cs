namespace SolastaCommunityExpansion.CustomDefinitions
{
    public interface IPointPoolMaxBonus
    {
        public int MaxPointsBonus { get; }
        public HeroDefinitions.PointsPoolType PoolType { get; }
    }
}
