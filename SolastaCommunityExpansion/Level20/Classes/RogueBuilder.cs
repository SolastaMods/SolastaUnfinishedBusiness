using System.Collections.Generic;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaCommunityExpansion.Level20.Features.ProficiencyRogueBlindSenseBuilder;
using static SolastaCommunityExpansion.Level20.Features.ProficiencyRogueSlipperyMindBuilder;

namespace SolastaCommunityExpansion.Level20.Classes
{
    internal static class RogueBuilder
    {
        internal static void Load()
        {
            Rogue.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel> {
                new FeatureUnlockByLevel(ProficiencyRogueBlindSense, 14),
                new FeatureUnlockByLevel(ProficiencyRogueSlipperyMind, 15),
                new FeatureUnlockByLevel(FeatureSetAbilityScoreChoice, 16),
                // TODO 18: Elusive
                new FeatureUnlockByLevel(FeatureSetAbilityScoreChoice, 19)
                // TODO 20: Stroke of Luck
            });
        }
    }
}