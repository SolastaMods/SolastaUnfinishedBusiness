using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Level20;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAutoPreparedSpellss;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionCastSpells;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Level20.PowerClericDivineInterventionImprovementBuilder;
using static SolastaUnfinishedBusiness.Level20.PowerClericTurnUndeadBuilder;
using static SolastaUnfinishedBusiness.Level20.PowerFighterActionSurge2Builder;
using static SolastaUnfinishedBusiness.Level20.SenseRangerFeralSensesBuilder;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPointPools;

namespace SolastaUnfinishedBusiness.Models;

internal static class Level20Context
{
    public const int MaxSpellLevel = 9;

    public const int ModMaxLevel = 20;
    public const int GameMaxLevel = 12;

    public const int ModMaxExperience = 355000;
    public const int GameMaxExperience = 100000;

    [NotNull]
    // ReSharper disable once UnusedMember.Global
    public static IEnumerable<CodeInstruction> Level20Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
    {
        var code = new List<CodeInstruction>(instructions);

        if (Main.Settings.EnableLevel20)
        {
            code
                .FindAll(x => x.opcode.Name == "ldc.i4.s" && Convert.ToInt32(x.operand) == GameMaxLevel)
                .ForEach(x => x.operand = ModMaxLevel);
        }

        return code;
    }

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
        RoguishShadowcasterLoad();
        TraditionLightLoad();

        //CastSpellWarlock.uniqueLevelSlots = false;
    }

    internal static void LateLoad()
    {
        const BindingFlags PrivateBinding = BindingFlags.Instance | BindingFlags.NonPublic;

        var harmony = new Harmony("SolastaUnfinishedBusiness");
        var transpiler = typeof(Level20Context).GetMethod("Level20Transpiler");
        var methods = new[]
        {
            typeof(ArchetypesPreviewModal).GetMethod("Refresh", PrivateBinding),
            typeof(CharacterBuildingManager).GetMethod("CreateCharacterFromTemplate"),
            typeof(CharactersPanel).GetMethod("Refresh", PrivateBinding),
            typeof(FeatureDefinitionCastSpell).GetMethod("EnsureConsistency"),
            typeof(HigherLevelFeaturesModal).GetMethod("Bind"),
            typeof(RulesetCharacterHero).GetMethod("RegisterAttributes"),
            typeof(RulesetCharacterHero).GetMethod("SerializeAttributes"),
            typeof(RulesetCharacterHero).GetMethod("SerializeElements"),
            typeof(RulesetEntity).GetMethod("SerializeElements")
        };

        foreach (var method in methods)
        {
            try
            {
                harmony.Patch(method, transpiler: new HarmonyMethod(transpiler));
            }
            catch
            {
                Main.Error("cannot fully patch Level 20");
            }
        }
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

        CastSpellCleric.SlotsPerLevels.SetRange(SpellsSlotsContext.FullCastingSlots);
        CastSpellCleric.ReplacedSpells.SetRange(SpellsSlotsContext.EmptyReplacedSpells);

        DomainBattle.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementPaladin, 20));
        DomainElementalCold.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementWizard, 20));
        DomainElementalFire.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementWizard, 20));
        DomainElementalLighting.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementWizard, 20));
        DomainInsight.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementCleric, 20));
        DomainLaw.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementPaladin, 20));
        DomainLife.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementCleric, 20));
        DomainOblivion.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementCleric, 20));
        DomainSun.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementWizard, 20));
    }

    private static void DruidLoad()
    {
        Druid.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            // TODO 18: BEAST SPELLS
            new(FeatureSetAbilityScoreChoice, 19)
            // TODO 20: ARCH DRUID
        });

        CastSpellDruid.SlotsPerLevels.SetRange(SpellsSlotsContext.FullCastingSlots);
        CastSpellDruid.ReplacedSpells.SetRange(SpellsSlotsContext.EmptyReplacedSpells);
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

        AutoPreparedSpellsOathOfMotherland.AutoPreparedSpellsGroups.Add(
            new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup
            {
                ClassLevel = 17, SpellsList = new List<SpellDefinition> { FlameStrike }
            });

        AutoPreparedSpellsOathOfTirmar.AutoPreparedSpellsGroups.Add(
            new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup
            {
                ClassLevel = 17, SpellsList = new List<SpellDefinition> { WallOfForce, HoldMonster }
            });

        CastSpellPaladin.SlotsPerLevels.SetRange(SpellsSlotsContext.HalfCastingSlots);
        CastSpellPaladin.ReplacedSpells.SetRange(SpellsSlotsContext.EmptyReplacedSpells);
    }

    private static void RangerLoad()
    {
        Ranger.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(SenseRangerFeralSenses, 18), new(FeatureSetAbilityScoreChoice, 19)
        });

        CastSpellRanger.SlotsPerLevels.SetRange(SpellsSlotsContext.HalfCastingSlots);
        CastSpellRanger.ReplacedSpells.SetRange(SpellsSlotsContext.HalfCasterReplacedSpells);
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
        Sorcerer.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(PointPoolSorcererAdditionalMetamagic, 17),
            new(FeatureSetAbilityScoreChoice, 19),
            new(SorcerousRestorationBuilder.SorcerousRestoration, 20)
        });

        CastSpellSorcerer.SlotsPerLevels.SetRange(SpellsSlotsContext.FullCastingSlots);
        CastSpellSorcerer.ReplacedSpells.SetRange(SpellsSlotsContext.FullCasterReplacedSpells);
        CastSpellSorcerer.KnownSpells.SetRange(SpellsSlotsContext.SorcererKnownSpells);
    }

    private static void WizardLoad()
    {
        Wizard.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            // TODO 18: Spell Mastery
            new(FeatureSetAbilityScoreChoice, 19)
            // TODO 20: Signature Spells
        });

        CastSpellWizard.SlotsPerLevels.SetRange(SpellsSlotsContext.FullCastingSlots);
        CastSpellWizard.ReplacedSpells.SetRange(SpellsSlotsContext.EmptyReplacedSpells);
    }

    private static void MartialSpellBladeLoad()
    {
        CastSpellMartialSpellBlade.SlotsPerLevels.SetRange(SpellsSlotsContext.OneThirdCastingSlots);
        CastSpellMartialSpellBlade.ReplacedSpells.SetRange(SpellsSlotsContext.OneThirdCasterReplacedSpells);
    }

    private static void RoguishShadowcasterLoad()
    {
        CastSpellShadowcaster.SlotsPerLevels.SetRange(SpellsSlotsContext.OneThirdCastingSlots);
        CastSpellShadowcaster.ReplacedSpells.SetRange(SpellsSlotsContext.OneThirdCasterReplacedSpells);
    }

    private static void TraditionLightLoad()
    {
        CastSpellTraditionLight.SlotsPerLevels.SetRange(SpellsSlotsContext.OneThirdCastingSlots);
        CastSpellTraditionLight.ReplacedSpells.SetRange(SpellsSlotsContext.OneThirdCasterReplacedSpells);
    }
}
