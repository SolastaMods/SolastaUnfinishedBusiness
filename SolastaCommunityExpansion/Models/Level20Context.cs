using System.Collections.Generic;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Level20;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionAutoPreparedSpellss;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionCastSpells;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaCommunityExpansion.Api.DatabaseHelper.SpellDefinitions;
using static SolastaCommunityExpansion.Level20.PowerClericDivineInterventionImprovementBuilder;
using static SolastaCommunityExpansion.Level20.PowerClericTurnUndeadBuilder;
using static SolastaCommunityExpansion.Level20.PowerFighterActionSurge2Builder;
using static SolastaCommunityExpansion.Level20.SenseRangerFeralSensesBuilder;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionPointPools;

namespace SolastaCommunityExpansion.Models;

internal static class Level20Context
{
    public const int MaxSpellLevel = 9;

    public const int ModMaxLevel = 20;
    public const int GameMaxLevel = 12;

    public const int ModMaxExperience = 355000;
    public const int GameMaxExperience = 100000;

    internal static void Load()
    {
        BarbarianLoad();
        ClericLoad();
        DruidLoad();
        FighterLoad();
        PaladinLoad();
        RangerLoad();
        RogueLoad();
        SorcererLoad();
        WizardLoad();
        MartialSpellBladeLoad();
        ShadowcasterLoad();

        CastSpellWarlock.uniqueLevelSlots = false;
    }

    internal static void LateLoad()
    {
        Level20PatchingContext.Load();
    }

    private static void BarbarianLoad()
    {
        Barbarian.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(AttributeModifierBarbarianBrutalCriticalAdd, 17),
            new(FeatureDefinitionIndomitableMightBuilder.FeatureDefinitionIndomitableMight, 18),
            new(FeatureSetAbilityScoreChoice, 19),
            new(FeatureDefinitionPrimalChampionBuilder.FeatureDefinitionPrimalChampion, 20)
        });
    }

    private static void ClericLoad()
    {
        Cleric.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(PowerClericTurnUndead17, 17),
            new(AttributeModifierClericChannelDivinityAdd, 18),
            new(FeatureSetAbilityScoreChoice, 19)
            // Solasta handles divine intervention on the subclasses, added below.
        });

        CastSpellCleric.spellCastingLevel = 9;

        CastSpellCleric.SlotsPerLevels.SetRange(SpellsHelper.FullCastingSlots);
        CastSpellCleric.ReplacedSpells.SetRange(SpellsHelper.EmptyReplacedSpells);

        DomainBattle.FeatureUnlocks.Add(new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementPaladin,
            20));
        DomainElementalCold.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementWizard, 20));
        DomainElementalFire.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementWizard, 20));
        DomainElementalLighting.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementWizard, 20));
        DomainInsight.FeatureUnlocks.Add(new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementCleric,
            20));
        DomainLaw.FeatureUnlocks.Add(new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementPaladin, 20));
        DomainLife.FeatureUnlocks.Add(new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementCleric, 20));
        DomainOblivion.FeatureUnlocks.Add(new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementCleric,
            20));
        DomainSun.FeatureUnlocks.Add(new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementWizard, 20));
    }

    private static void DruidLoad()
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

    private static void FighterLoad()
    {
        Fighter.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(PowerFighterActionSurge2, 17),
            new(AttributeModifierFighterIndomitableAdd1, 17),
            new(FeatureSetAbilityScoreChoice, 19),
            new(AttributeModifierFighterExtraAttack, 20)
        });
    }

    private static void PaladinLoad()
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
                ClassLevel = 13, SpellsList = new List<SpellDefinition> { FreedomOfMovement, GuardianOfFaith }
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
                ClassLevel = 13, SpellsList = new List<SpellDefinition> { WallOfFire, Stoneskin }
            });

        AutoPreparedSpellsOathOfMotherland.AutoPreparedSpellsGroups.Add(
            new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup
            {
                ClassLevel = 17, SpellsList = new List<SpellDefinition> { FlameStrike }
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
                ClassLevel = 17, SpellsList = new List<SpellDefinition> { WallOfForce, HoldMonster }
            });

        CastSpellPaladin.spellCastingLevel = 5;

        CastSpellPaladin.SlotsPerLevels.SetRange(SpellsHelper.HalfCastingSlots);
        CastSpellPaladin.ReplacedSpells.SetRange(SpellsHelper.EmptyReplacedSpells);
    }

    private static void RangerLoad()
    {
        Ranger.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(SenseRangerFeralSenses, 18), new(FeatureSetAbilityScoreChoice, 19)
            //new FeatureUnlockByLevel(FeatureSetRangerFoeSlayer, 20)
        });

        CastSpellRanger.spellCastingLevel = 5;

        CastSpellRanger.SlotsPerLevels.SetRange(SpellsHelper.HalfCastingSlots);
        CastSpellRanger.ReplacedSpells.SetRange(SpellsHelper.HalfCasterReplacedSpells);
    }

    private static void RogueLoad()
    {
        Rogue.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            // TODO 18: Elusive
            new(FeatureSetAbilityScoreChoice, 19)
            // TODO 20: Stroke of Luck
        });
    }

    private static void SorcererLoad()
    {
        // add missing progression
        Sorcerer.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(PointPoolSorcererAdditionalMetamagic, 17),
            new(FeatureSetAbilityScoreChoice, 19),
            new(SorcerousRestorationBuilder.SorcerousRestoration, 20)
        });

        CastSpellSorcerer.spellCastingLevel = 9;

        CastSpellSorcerer.SlotsPerLevels.SetRange(SpellsHelper.FullCastingSlots);
        CastSpellSorcerer.ReplacedSpells.SetRange(SpellsHelper.FullCasterReplacedSpells);
        CastSpellSorcerer.KnownSpells.SetRange(SpellsHelper.SorcererKnownSpells);
    }

    private static void WizardLoad()
    {
        // add missing progression
        Wizard.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            // TODO 18: Spell Mastery
            new(FeatureSetAbilityScoreChoice, 19)
            // TODO 20: Signature Spells
        });

        CastSpellWizard.spellCastingLevel = 9;

        CastSpellWizard.SlotsPerLevels.SetRange(SpellsHelper.FullCastingSlots);
        CastSpellWizard.ReplacedSpells.SetRange(SpellsHelper.EmptyReplacedSpells);
    }

    private static void MartialSpellBladeLoad()
    {
        CastSpellMartialSpellBlade.spellCastingLevel = 4;

        CastSpellMartialSpellBlade.SlotsPerLevels.SetRange(SpellsHelper.OneThirdCastingSlots);

        CastSpellMartialSpellBlade.ReplacedSpells.SetRange(SpellsHelper.OneThirdCasterReplacedSpells);
    }

    private static void ShadowcasterLoad()
    {
        CastSpellShadowcaster.spellCastingLevel = 4;

        CastSpellShadowcaster.SlotsPerLevels.SetRange(SpellsHelper.OneThirdCastingSlots);

        CastSpellShadowcaster.ReplacedSpells.SetRange(SpellsHelper.OneThirdCasterReplacedSpells);
    }
}
