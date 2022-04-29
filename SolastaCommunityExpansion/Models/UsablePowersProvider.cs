using System.Collections.Generic;
using System.Linq;

namespace SolastaCommunityExpansion.Models
{

    public static class UsablePowersProvider
    {
        private static readonly Dictionary<FeatureDefinitionPower, RulesetUsablePower> UsablePowers = new();
        public static RulesetUsablePower Get(FeatureDefinitionPower power, RulesetCharacter actor = null)
        {
            RulesetUsablePower result = null;
            if (actor != null)
            {
                result = actor.UsablePowers.FirstOrDefault(u => u.PowerDefinition == power);
            }

            if (result == null)
            {
                if (UsablePowers.ContainsKey(power))
                {
                    result = UsablePowers[power];
                }
                else
                {
                    result = new RulesetUsablePower(power, null, null);
                    UsablePowers.Add(power, result);
                }
            }

            return result;
        }
    }
}
