using System.Collections.Generic;
using SolastaModApi.Extensions;
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
            Sorcerer.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel> {
                new FeatureUnlockByLevel(FeatureSetAbilityScoreChoice, 16),
                new FeatureUnlockByLevel(PointPoolSorcererAdditionalMetamagic, 17),
                new FeatureUnlockByLevel(FeatureSetAbilityScoreChoice, 19),
                // TODO: Sorcerous Restoration
            });

            CastSpellSorcerer.SetSpellCastingLevel(9);

            CastSpellSorcerer.SlotsPerLevels.Clear();
            CastSpellSorcerer.SlotsPerLevels.AddRange(SpellsHelper.FullCastingSlots);

            CastSpellSorcerer.ReplacedSpells.Clear();
            CastSpellSorcerer.ReplacedSpells.AddRange(SpellsHelper.FullCasterReplacedSpells);

            CastSpellSorcerer.KnownSpells.Clear();
            CastSpellSorcerer.KnownSpells.AddRange(SpellsHelper.SorcererKnownSpells);
        }
    }
}