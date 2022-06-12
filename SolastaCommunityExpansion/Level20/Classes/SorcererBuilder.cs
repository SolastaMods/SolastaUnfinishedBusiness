using System.Collections.Generic;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Level20.Features;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionCastSpells;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionPointPools;

namespace SolastaCommunityExpansion.Level20.Classes;

internal static class SorcererBuilder
{
    internal static void Load()
    {
        // add missing progression
        Sorcerer.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(FeatureSetAbilityScoreChoice, 16),
            new(PointPoolSorcererAdditionalMetamagic, 17),
            new(FeatureSetAbilityScoreChoice, 19),
            new(SorcerousRestorationBuilder.SorcerousRestoration, 20)
        });

        CastSpellSorcerer.spellCastingLevel = 9;

        CastSpellSorcerer.SlotsPerLevels.SetRange(SpellsHelper.FullCastingSlots);
        CastSpellSorcerer.ReplacedSpells.SetRange(SpellsHelper.FullCasterReplacedSpells);
        CastSpellSorcerer.KnownSpells.SetRange(SpellsHelper.SorcererKnownSpells);
    }
}
