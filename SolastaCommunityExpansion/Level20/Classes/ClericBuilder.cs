using System.Collections.Generic;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionCastSpells;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaCommunityExpansion.Level20.Features.PowerClericTurnUndeadBuilder;

namespace SolastaCommunityExpansion.Level20.Classes
{
    internal static class ClericBuilder
    {
        internal static void Load()
        {
            // add missing progression
            Cleric.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel> {
                new FeatureUnlockByLevel(PowerClericTurnUndead14, 14),
                new FeatureUnlockByLevel(FeatureSetAbilityScoreChoice, 16),
                new FeatureUnlockByLevel(PowerClericTurnUndead17, 17),
                new FeatureUnlockByLevel(AttributeModifierClericChannelDivinityAdd, 18),
                new FeatureUnlockByLevel(FeatureSetAbilityScoreChoice, 19)
                // TODO 20: Divine Intervention Improvement
            });

            CastSpellCleric.SetSpellCastingLevel(9);

            CastSpellCleric.SlotsPerLevels.Clear();
            CastSpellCleric.SlotsPerLevels.AddRange(SpellsHelper.FullCastingSlots);

            CastSpellCleric.ReplacedSpells.Clear();
            CastSpellCleric.ReplacedSpells.AddRange(SpellsHelper.EmptyReplacedSpells);
        }
    }
}