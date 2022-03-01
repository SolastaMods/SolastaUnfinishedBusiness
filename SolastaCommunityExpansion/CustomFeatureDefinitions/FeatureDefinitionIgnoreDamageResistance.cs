using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomFeatureDefinitions
{
    public interface IIgnoreDamageAffinity
    {
        bool CanIgnoreDamageAffinity(IDamageAffinityProvider provider, string damageType);
    }

    public class FeatureDefinitionIgnoreDamageResistance : FeatureDefinition, IIgnoreDamageAffinity
    {
        public List<string> DamageTypes = new();

        public bool CanIgnoreDamageAffinity(IDamageAffinityProvider provider, string damageType)
        {
            if (provider.DamageAffinityType != RuleDefinitions.DamageAffinityType.Resistance)
            {
                return false;
            }

            return DamageTypes.Contains(damageType);
        }
    }
}
