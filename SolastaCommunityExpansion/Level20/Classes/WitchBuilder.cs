/*using SolastaCommunityExpansion.Level20.Features;
using SolastaModApi.Extensions;
using System.Collections.Generic;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionCastSpells;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPointPools;

namespace SolastaCommunityExpansion.Level20.Classes
{
    internal static class WitchBuilder
    {
        internal static void Load()
        {

            DatabaseRepository.GetDatabase<CharacterClassDefinition>().TryGetElement("Witch", out CharacterClassDefinition characterClassDefinition);
            DatabaseRepository.GetDatabase<FeatureDefinitionCastSpell>().TryGetElement("CastSpellClassWitch", out FeatureDefinitionCastSpell featureDefinitionCastSpell);

            if (characterClassDefinition != null && featureDefinitionCastSpell != null)
            {

                // add missing progression
                characterClassDefinition.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel> {
                    new FeatureUnlockByLevel(FeatureSetAbilityScoreChoice, 16),
                    new FeatureUnlockByLevel(FeatureSetAbilityScoreChoice, 19)
                });

                featureDefinitionCastSpell.SetSpellCastingLevel(9);

                featureDefinitionCastSpell.SlotsPerLevels.Clear();
                featureDefinitionCastSpell.SlotsPerLevels.AddRange(SpellsHelper.FullCastingSlots);

                featureDefinitionCastSpell.ReplacedSpells.Clear();
                featureDefinitionCastSpell.ReplacedSpells.AddRange(SpellsHelper.FullCasterReplacedSpells);

                featureDefinitionCastSpell.KnownSpells.Clear();
                featureDefinitionCastSpell.KnownSpells.AddRange(SpellsHelper.SorcererKnownSpells);
            }

        }
    }
}*/