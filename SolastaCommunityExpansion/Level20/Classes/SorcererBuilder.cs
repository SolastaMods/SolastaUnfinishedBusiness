using System.Collections.Generic;
using SolastaCommunityExpansion.Level20.Features;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionCastSpells;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPointPools;

namespace SolastaCommunityExpansion.Level20.Classes
{
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

            CastSpellSorcerer.SetSpellCastingLevel(9);

            CastSpellSorcerer.SlotsPerLevels.SetRange(SpellsHelper.FullCastingSlots);
            CastSpellSorcerer.ReplacedSpells.SetRange(SpellsHelper.FullCasterReplacedSpells);
            CastSpellSorcerer.KnownSpells.SetRange(SpellsHelper.SorcererKnownSpells);
        }
    }
}
