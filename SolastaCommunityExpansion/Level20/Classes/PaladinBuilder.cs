using System.Collections.Generic;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Level20.Features;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionAutoPreparedSpellss;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionCastSpells;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaCommunityExpansion.Level20.Classes;

internal static class PaladinBuilder
{
    internal static void Load()
    {
        Paladin.FeatureUnlocks.AddRange(
            new FeatureUnlockByLevel(PowerPaladinAuraOfCourage18Builder.Instance, 18),
            new FeatureUnlockByLevel(PowerPaladinAuraOfProtection18Builder.Instance, 18),
            new FeatureUnlockByLevel(FeatureSetAbilityScoreChoice, 19)
        );

        //// Oath of Devotion
        AutoPreparedSpellsOathOfDevotion.AutoPreparedSpellsGroups.Add(
            new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup
            {
                ClassLevel = 13, SpellsList = new List<SpellDefinition> {FreedomOfMovement, GuardianOfFaith}
            });

        AutoPreparedSpellsOathOfDevotion.AutoPreparedSpellsGroups.Add(
            new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup
            {
                ClassLevel = 17,
                SpellsList = new List<SpellDefinition>
                {
                    // Commune,
                    FlameStrike
                }
            });

        //// Oath of Motherlands
        AutoPreparedSpellsOathOfMotherland.AutoPreparedSpellsGroups.Add(
            new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup
            {
                ClassLevel = 13, SpellsList = new List<SpellDefinition> {WallOfFire, Stoneskin}
            });

        AutoPreparedSpellsOathOfMotherland.AutoPreparedSpellsGroups.Add(
            new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup
            {
                ClassLevel = 17, SpellsList = new List<SpellDefinition> {FlameStrike}
            });

        //// Oath of Tirmar
        AutoPreparedSpellsOathOfTirmar.AutoPreparedSpellsGroups.Add(
            new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup
            {
                ClassLevel = 13,
                SpellsList = new List<SpellDefinition>
                {
                    Banishment
                    // Compulsion
                }
            });

        AutoPreparedSpellsOathOfTirmar.AutoPreparedSpellsGroups.Add(
            new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup
            {
                ClassLevel = 17, SpellsList = new List<SpellDefinition> {WallOfForce, HoldMonster}
            });

        CastSpellPaladin.spellCastingLevel = 5;

        CastSpellPaladin.SlotsPerLevels.SetRange(SpellsHelper.HalfCastingSlots);
        CastSpellPaladin.ReplacedSpells.SetRange(SpellsHelper.EmptyReplacedSpells);
    }
}
