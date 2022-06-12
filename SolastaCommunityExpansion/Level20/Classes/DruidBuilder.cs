using System.Collections.Generic;
using SolastaCommunityExpansion.Api.Infrastructure;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionCastSpells;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionFeatureSets;

namespace SolastaCommunityExpansion.Level20.Classes;

internal static class DruidBuilder
{
    internal static void Load()
    {
        // add missing progression
        Druid.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            // TODO 18: BEAST SPELLS
            new(FeatureSetAbilityScoreChoice, 19)
            // TODO 20: ARCHDRUID
        });

        CastSpellDruid.spellCastingLevel = 9;

        CastSpellDruid.SlotsPerLevels.SetRange(SpellsHelper.FullCastingSlots);
        CastSpellDruid.ReplacedSpells.SetRange(SpellsHelper.EmptyReplacedSpells);
    }
}
