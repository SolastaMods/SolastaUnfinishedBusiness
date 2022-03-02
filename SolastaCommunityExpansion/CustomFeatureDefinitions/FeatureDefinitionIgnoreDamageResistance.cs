using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomFeatureDefinitions
{
    public class FeatureDefinitionIgnoreDamageResistance : FeatureDefinitionDamageAffinity
    {
        private readonly List<string> DamageTypes = new();

        public FeatureDefinitionIgnoreDamageResistance(params string[] damageTypes)
        {
            DamageTypes.AddRange(damageTypes);
        }

        public new float ModulateSustainedDamage(
          string damageType,
          float multiplier,
          List<string> sourceTags,
          string ancestryDamageType)
        {
            if (DamageAffinityType == RuleDefinitions.DamageAffinityType.Resistance && DamageTypes.Contains(damageType))
            {
                return multiplier;
            }

            return base.ModulateSustainedDamage(damageType, multiplier, sourceTags, ancestryDamageType);
        }
    }
}
