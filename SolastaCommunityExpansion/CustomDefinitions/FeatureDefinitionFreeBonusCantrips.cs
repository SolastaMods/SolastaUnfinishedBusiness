using System;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public class FeatureDefinitionFreeBonusCantrips : FeatureDefinitionBonusCantrips, IPointPoolMaxBonus
    {
        public int MaxPointsBonus { get => BonusCantrips.Count; }
        public HeroDefinitions.PointsPoolType PoolType { get => HeroDefinitions.PointsPoolType.Cantrip; }
    }

    public class FeatureDefinitionFreeBonusCantripsWithPrerequisites : FeatureDefinitionFreeBonusCantrips, IFeatureDefinitionWithPrerequisites
    {
        public List<Func<bool>> Validators { get; set; }
    }
}
