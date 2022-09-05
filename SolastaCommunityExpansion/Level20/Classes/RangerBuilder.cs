using System.Collections.Generic;
using SolastaCommunityExpansion.Api.Infrastructure;
using static SolastaCommunityExpansion.Level20.Features.RangerFeralSensesBuilder;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionCastSpells;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionFeatureSets;

//using static SolastaCommunityExpansion.Models.Features.FeatureSetRangerFoeSlayerBuilder;

namespace SolastaCommunityExpansion.Level20.Classes;

internal static class RangerBuilder
{
    internal static void Load()
    {
        Ranger.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(RangerFeralSenses, 18), new(FeatureSetAbilityScoreChoice, 19)
            //new FeatureUnlockByLevel(FeatureSetRangerFoeSlayer, 20)
        });

        CastSpellRanger.spellCastingLevel = 5;

        CastSpellRanger.SlotsPerLevels.SetRange(SpellsHelper.HalfCastingSlots);
        CastSpellRanger.ReplacedSpells.SetRange(SpellsHelper.HalfCasterReplacedSpells);
    }
}
