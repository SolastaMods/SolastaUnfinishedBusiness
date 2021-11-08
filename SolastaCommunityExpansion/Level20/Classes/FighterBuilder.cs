using System.Collections.Generic;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaCommunityExpansion.Level20.Features.AttributeModifierFighterIndomitableBuilder;
using static SolastaCommunityExpansion.Level20.Features.PowerFighterActionSurge2Builder;

namespace SolastaCommunityExpansion.Level20.Classes
{
    internal static class FighterBuilder
    {
        internal static void Load()
        {
            Fighter.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel> {
                new FeatureUnlockByLevel(AttributeModifierFighterIndomitableAdd, 13),
                new FeatureUnlockByLevel(FeatureSetAbilityScoreChoice, 14),
                new FeatureUnlockByLevel(FeatureSetAbilityScoreChoice, 16),
                new FeatureUnlockByLevel(PowerFighterActionSurge2, 17),
                new FeatureUnlockByLevel(AttributeModifierFighterIndomitableAdd, 17),
                new FeatureUnlockByLevel(FeatureSetAbilityScoreChoice, 19),
                new FeatureUnlockByLevel(AttributeModifierFighterExtraAttack, 20)
            });
        }
    }
}