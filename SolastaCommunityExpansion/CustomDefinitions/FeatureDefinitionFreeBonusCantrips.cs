using System.Collections.Generic;
using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public class FeatureDefinitionFreeBonusCantrips : FeatureDefinitionBonusCantrips, IPointPoolMaxBonus
    {
        public int MaxPointsBonus => BonusCantrips.Count;
        public HeroDefinitions.PointsPoolType PoolType => HeroDefinitions.PointsPoolType.Cantrip;
    }

    public class FeatureDefinitionFreeBonusCantripsWithPrerequisites : FeatureDefinitionFreeBonusCantrips,
        IFeatureDefinitionWithPrerequisites
    {
        public List<IFeatureDefinitionWithPrerequisites.Validate> Validators { get; set; }
    }
}
