using System.Collections.Generic;
using SolastaModApi.Extensions;
using static SolastaCommunityExpansion.Level20.Features.PowerClericDivineInterventionImprovementBuilder;
using static SolastaCommunityExpansion.Level20.Features.PowerClericTurnUndeadBuilder;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;
using static SolastaModApi.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionCastSpells;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionFeatureSets;

namespace SolastaCommunityExpansion.Level20.Classes
{
    internal static class ClericBuilder
    {
        internal static void Load()
        {
            Cleric.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel> {
                new FeatureUnlockByLevel(PowerClericTurnUndead14, 14),
                new FeatureUnlockByLevel(FeatureSetAbilityScoreChoice, 16),
                new FeatureUnlockByLevel(PowerClericTurnUndead17, 17),
                new FeatureUnlockByLevel(AttributeModifierClericChannelDivinityAdd, 18),
                new FeatureUnlockByLevel(FeatureSetAbilityScoreChoice, 19)
                // Solasta handles divine intervention on the subclasses, added below.
            });

            CastSpellCleric.SetSpellCastingLevel(9);

            CastSpellCleric.SlotsPerLevels.Clear();
            CastSpellCleric.SlotsPerLevels.AddRange(SpellsHelper.FullCastingSlots);

            CastSpellCleric.ReplacedSpells.Clear();
            CastSpellCleric.ReplacedSpells.AddRange(SpellsHelper.EmptyReplacedSpells);

            DomainBattle.FeatureUnlocks.Add(new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementPaladin, 20));
            DomainElementalCold.FeatureUnlocks.Add(new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementWizard, 20));
            DomainElementalFire.FeatureUnlocks.Add(new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementWizard, 20));
            DomainElementalLighting.FeatureUnlocks.Add(new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementWizard, 20));
            DomainInsight.FeatureUnlocks.Add(new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementCleric, 20));
            DomainLaw.FeatureUnlocks.Add(new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementPaladin, 20));
            DomainLife.FeatureUnlocks.Add(new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementCleric, 20));
            DomainOblivion.FeatureUnlocks.Add(new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementCleric, 20));
            DomainSun.FeatureUnlocks.Add(new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementWizard, 20));
        }
    }
}
