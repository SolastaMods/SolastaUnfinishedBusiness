using System;

namespace SolastaModApi.Extensions
{
    public static partial class FeatureDefinitionMagicAffinityExtensions
    {
        [Obsolete("Use SetConcentrationAffinity")]
        public static T SetConcentrationAdvantage<T>(this T entity, RuleDefinitions.AdvantageType value)
            where T : FeatureDefinitionMagicAffinity
        {
            switch (value)
            {
                case RuleDefinitions.AdvantageType.Advantage:
                    entity.SetConcentrationAffinity(RuleDefinitions.ConcentrationAffinity.Advantage);
                    break;
                case RuleDefinitions.AdvantageType.Disadvantage:
                    entity.SetConcentrationAffinity(RuleDefinitions.ConcentrationAffinity.Disadvantage);
                    break;
                case RuleDefinitions.AdvantageType.None:
                    entity.SetConcentrationAffinity(RuleDefinitions.ConcentrationAffinity.None);
                    break;
            }

            return entity;
        }
    }
}