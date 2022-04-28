using System.Collections.Generic;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public class IgnoreDynamicVisionImpairment : FeatureDefinition
    {
        public float maxRange;
        public List<FeatureDefinition> requiredFeatures = new();
        public List<FeatureDefinition> forbiddenFeatures = new();

        public bool CanIgnoreDynamicVisionImpairment(RulesetCharacter character, float range)
        {
            return range <= maxRange
                   && character.HasAllFeatures(requiredFeatures)
                   && !character.HasAnyFeature(forbiddenFeatures);
        }
    }
}
