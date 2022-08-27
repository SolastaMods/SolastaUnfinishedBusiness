using System.Collections.Generic;
using static SolastaCommunityExpansion.Level20.Features.FeatureDefinitionIndomitableMightBuilder;
using static SolastaCommunityExpansion.Level20.Features.FeatureDefinitionPrimalChampionBuilder;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionFeatureSets;

namespace SolastaCommunityExpansion.Level20.Classes;

internal static class BarbarianBuilder
{
    internal static void Load()
    {
        Barbarian.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(AttributeModifierBarbarianBrutalCriticalAdd, 17),
            new(FeatureDefinitionIndomitableMight, 18),
            new(FeatureSetAbilityScoreChoice, 19),
            new(FeatureDefinitionPrimalChampion, 20)
        });
    }
}
