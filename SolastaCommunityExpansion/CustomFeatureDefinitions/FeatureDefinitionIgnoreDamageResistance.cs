using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Patches.CustomFeatures;

namespace SolastaCommunityExpansion.CustomFeatureDefinitions
{
    public class FeatureDefinitionIgnoreDamageResistance : FeatureDefinitionDamageAffinity
    {
        private readonly List<string> DamageTypes = new();

        private static IEnumerable<T> ExtractFeatures<T>(RulesetActor rulesetActor) where T : class
        {
            var featureDefinitions = new List<FeatureDefinition>();

            rulesetActor.EnumerateFeaturesToBrowse<T>(featureDefinitions, null);

            return featureDefinitions.Select(x => x as T);
        }

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
            var attacker = GameLocationBattleManager_HandleCharacterMagicalAttackDamage.Attacker;
            var features = ExtractFeatures<FeatureDefinitionIgnoreDamageResistance>(attacker.RulesetActor);

            foreach (var feature in features)
            {
                if (DamageAffinityType == RuleDefinitions.DamageAffinityType.Resistance && feature.DamageTypes.Contains(damageType))
                {
                    return multiplier;
                }
            }

            return base.ModulateSustainedDamage(damageType, multiplier, sourceTags, ancestryDamageType);
        }
    }
}
