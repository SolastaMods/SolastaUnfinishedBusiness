using System.Collections.Generic;
using static SolastaCommunityExpansion.Level20.Features.ProficiencyRogueBlindSenseBuilder;
using static SolastaCommunityExpansion.Level20.Features.ProficiencyRogueSlipperyMindBuilder;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionFeatureSets;

namespace SolastaCommunityExpansion.Level20.Classes;

internal static class RogueBuilder
{
    internal static void Load()
    {
        Rogue.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(ProficiencyRogueBlindSense, 14),
            new(ProficiencyRogueSlipperyMind, 15),
            new(FeatureSetAbilityScoreChoice, 16),
            // TODO 18: Elusive
            new(FeatureSetAbilityScoreChoice, 19)
            // TODO 20: Stroke of Luck
        });
    }
}
