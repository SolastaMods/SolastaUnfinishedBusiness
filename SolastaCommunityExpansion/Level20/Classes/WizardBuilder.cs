using System.Collections.Generic;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionCastSpells;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionFeatureSets;

namespace SolastaCommunityExpansion.Level20.Classes
{
    internal static class WizardBuilder
    {
        internal static void Load()
        {
            // add missing progression
            Wizard.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel> {
                // TODO 14: Overchannel
                new FeatureUnlockByLevel(FeatureSetAbilityScoreChoice, 16),
                // TODO 18: Spell Mastery
                new FeatureUnlockByLevel(FeatureSetAbilityScoreChoice, 19)
                // TODO 20: Signature Spells
            });

            CastSpellWizard.SetSpellCastingLevel(9);

            CastSpellWizard.KnownCantrips.Add(0); // need this to avoid a trace error message when leveling up to 20

            CastSpellWizard.SlotsPerLevels.SetRange(SpellsHelper.FullCastingSlots);

            CastSpellWizard.ReplacedSpells.SetRange(SpellsHelper.EmptyReplacedSpells);
        }
    }
}
