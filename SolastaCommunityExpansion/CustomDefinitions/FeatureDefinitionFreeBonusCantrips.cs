using System.Collections.Generic;
using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public class FeatureDefinitionFreeBonusCantrips : FeatureDefinitionBonusCantrips, IPointPoolMaxBonus
    {
        public int MaxPointsBonus { get => BonusCantrips.Count; }
        public HeroDefinitions.PointsPoolType PoolType { get => HeroDefinitions.PointsPoolType.Cantrip; }
    }

    public class FeatureDefinitionFreeBonusCantripsWithPrerequisites : FeatureDefinitionFreeBonusCantrips, IFeatureDefinitionWithPrerequisites
    {
        public List<IFeatureDefinitionWithPrerequisites.Validate> Validators { get; set; }
    }
}
