using System.Collections.Generic;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using static SolastaCommunityExpansion.Level20.Features.RangerFeralSensesBuilder;
using static SolastaCommunityExpansion.Level20.Features.RangerVanishActionBuilder;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionCastSpells;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionFeatureSets;

//using static SolastaCommunityExpansion.Models.Features.FeatureSetRangerFoeSlayerBuilder;

namespace SolastaCommunityExpansion.Level20.Classes;

internal static class RangerBuilder
{
    internal static void Load()
    {
        Ranger.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(AdditionalDamageRangerFavoredEnemyChoice, 14),
            new(RangerVanishAction, 14),
            new(FeatureSetAbilityScoreChoice, 16),
            new(RangerFeralSenses, 18),
            new(FeatureSetAbilityScoreChoice, 19)
            //new FeatureUnlockByLevel(FeatureSetRangerFoeSlayer, 20)
        });

        CastSpellRanger.SetSpellCastingLevel(5);

        CastSpellRanger.SlotsPerLevels.SetRange(SpellsHelper.HalfCastingSlots);
        CastSpellRanger.ReplacedSpells.SetRange(SpellsHelper.HalfCasterReplacedSpells);
    }
}
