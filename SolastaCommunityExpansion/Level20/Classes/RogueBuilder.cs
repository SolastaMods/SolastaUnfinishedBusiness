using System.Collections.Generic;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionFeatureSets;

namespace SolastaCommunityExpansion.Level20.Classes;

internal static class RogueBuilder
{
    internal static void Load()
    {
        Rogue.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            // TODO 18: Elusive
            new(FeatureSetAbilityScoreChoice, 19)
            // TODO 20: Stroke of Luck
        });
    }
}
