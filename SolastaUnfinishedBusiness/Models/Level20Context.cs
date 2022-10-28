using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPointPools;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionProficiencys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionCastSpells;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellListDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.FeatureDefinitionCastSpellBuilder;

namespace SolastaUnfinishedBusiness.Models;

internal static class Level20Context
{
    internal const string PowerWarlockEldritchMasterName = "PowerWarlockEldritchMaster";

    internal const int ModMaxLevel = 20;
    internal const int GameMaxLevel = 12;
    internal const int GameFinalMaxLevel = 16;

    internal const int ModMaxExperience = 355000;
    internal const int GameMaxExperience = 100000;

    internal static void Load()
    {
        BarbarianLoad();
        BardLoad();
        ClericLoad();
        DruidLoad();
        FighterLoad();
        MonkLoad();
        PaladinLoad();
        RangerLoad();
        RogueLoad();
        SorcererLoad();
        WarlockLoad();
        WizardLoad();
        MartialSpellBladeLoad();
        RoguishShadowcasterLoad();

        //
        // required to avoid issues on how game calculates caster / spell levels and some trace error messages
        // that might affect multiplayer sessions and prevent level up from 19 to 20
        //

        var classesFeatures = DatabaseRepository.GetDatabase<CharacterClassDefinition>()
            .SelectMany(a => a.FeatureUnlocks)
            .Select(b => b.FeatureDefinition);

        var subclassesFeatures = DatabaseRepository.GetDatabase<CharacterSubclassDefinition>()
            .SelectMany(a => a.FeatureUnlocks)
            .Select(b => b.FeatureDefinition);

        var allFeatures = classesFeatures.Concat(subclassesFeatures).ToList();
        var castSpellDefinitions = allFeatures.OfType<FeatureDefinitionCastSpell>();
        var magicAffinityDefinitions = allFeatures.OfType<FeatureDefinitionMagicAffinity>();

        foreach (var magicAffinityDefinition in magicAffinityDefinitions)
        {
            var spellListDefinition = magicAffinityDefinition.ExtendedSpellList;

            if (spellListDefinition == null)
            {
                continue;
            }

            var spellsByLevel = spellListDefinition.SpellsByLevel;

            while (spellsByLevel.Count < spellListDefinition.MaxSpellLevel + (spellListDefinition.HasCantrips ? 1 : 0))
            {
                spellsByLevel.Add(new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level = spellsByLevel.Count, Spells = new List<SpellDefinition>()
                });
            }
        }

        foreach (var castSpellDefinition in castSpellDefinitions)
        {
            while (castSpellDefinition.KnownCantrips.Count < ModMaxLevel + 1)
            {
                castSpellDefinition.KnownCantrips.Add(0);
            }

            while (castSpellDefinition.KnownSpells.Count < ModMaxLevel + 1)
            {
                castSpellDefinition.KnownSpells.Add(0);
            }

            while (castSpellDefinition.ReplacedSpells.Count < ModMaxLevel + 1)
            {
                castSpellDefinition.ReplacedSpells.Add(0);
            }

            while (castSpellDefinition.ScribedSpells.Count < ModMaxLevel + 1)
            {
                castSpellDefinition.ScribedSpells.Add(0);
            }

            var spellListDefinition = castSpellDefinition.SpellListDefinition;

            if (spellListDefinition == null)
            {
                continue;
            }

            var spellsByLevel = spellListDefinition.SpellsByLevel;

            while (spellsByLevel.Count < spellListDefinition.MaxSpellLevel + (spellListDefinition.HasCantrips ? 1 : 0))
            {
                spellsByLevel.Add(new SpellListDefinition.SpellsByLevelDuplet
                {
                    Level = spellsByLevel.Count, Spells = new List<SpellDefinition>()
                });
            }
        }
    }

    internal static void LateLoad()
    {
        const BindingFlags PrivateBinding = BindingFlags.Instance | BindingFlags.NonPublic;

        var harmony = new Harmony("SolastaUnfinishedBusiness");
        var transpiler = new Func<IEnumerable<CodeInstruction>, IEnumerable<CodeInstruction>>(Level20Transpiler).Method;

        // these are currently the hard-coded levels on below methods
        var methods = new[]
        {
            typeof(ArchetypesPreviewModal).GetMethod("Refresh", PrivateBinding), // 12
            typeof(CharacterBuildingManager).GetMethod("CreateCharacterFromTemplate"), // 16
            typeof(CharactersPanel).GetMethod("Refresh", PrivateBinding), // 12
            typeof(FeatureDefinitionCastSpell).GetMethod("EnsureConsistency"), // 16
            typeof(HigherLevelFeaturesModal).GetMethod("Bind"), // 12
            typeof(InvocationSubPanel).GetMethod("SetState"), // 12
            typeof(RulesetCharacterHero).GetMethod("RegisterAttributes"), // 16
            typeof(RulesetCharacterHero).GetMethod("SerializeElements"), // 12, 16
            typeof(RulesetEntity).GetMethod("SerializeElements") // 12, 16
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
            new(AttributeModifierBarbarianBrutalCriticalAdd, 13),
            new(PowerBarbarianPersistentRageStart, 15),
            new(AttributeModifierBarbarianRageDamageAdd, 16),
            new(AttributeModifierBarbarianBrutalCriticalAdd, 17),
            new(AttributeModifierBarbarianRagePointsAdd, 17),
            // TODO 18: Barbarian Indomitable Might
            new(FeatureSetAbilityScoreChoice, 19)
            // TODO 20: Barbarian Primal Champion
        });
    }

    private static void BardLoad()
    {
        var pointPoolBardMagicalSecrets18 = FeatureDefinitionPointPoolBuilder
            .Create(PointPoolBardMagicalSecrets14, "PointPoolBardMagicalSecrets18")
            .AddToDB();

        Bard.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(PointPoolBardMagicalSecrets14, 14),
            new(AttributeModifierBardicInspirationDieD12, 15),
            new(FeatureSetAbilityScoreChoice, 16),
            new(pointPoolBardMagicalSecrets18, 18),
            new(FeatureSetAbilityScoreChoice, 19)
            // TODO 20: Bard Superior Inspiration
        });

        EnumerateSlotsPerLevel(
            CasterProgression.Full,
            CastSpellBard.SlotsPerLevels);

        EnumerateKnownSpells(
            4,
            CasterProgression.Full,
            CastSpellBard.KnownSpells);

        EnumerateReplacedSpells(
            2, 1, CastSpellBard.ReplacedSpells);

        SpellListBard.maxSpellLevel = 9;
    }

    private static void ClericLoad()
    {
        var effectPowerClericTurnUndead17 = new EffectDescription();

        effectPowerClericTurnUndead17.Copy(PowerClericTurnUndead14.EffectDescription);
        effectPowerClericTurnUndead17.EffectForms[0].KillForm.challengeRating = 4;

        var powerClericTurnUndead17 = FeatureDefinitionPowerBuilder
            .Create(PowerClericTurnUndead14, "PowerClericTurnUndead17")
            .SetEffectDescription(effectPowerClericTurnUndead17)
            .AddToDB();

        Cleric.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(PowerClericTurnUndead14, 14),
            new(FeatureSetAbilityScoreChoice, 16),
            new(powerClericTurnUndead17, 17),
            new(AttributeModifierClericChannelDivinityAdd, 18),
            new(FeatureSetAbilityScoreChoice, 19)
            // TODO 20: Divine Intervention
        });

        EnumerateSlotsPerLevel(
            CasterProgression.Full,
            CastSpellCleric.SlotsPerLevels);

        SpellListCleric.maxSpellLevel = 9;

        // var powerClericDivineInterventionImprovementCleric = FeatureDefinitionPowerBuilder
        //     .Create(
        //         PowerClericDivineInterventionCleric,
        //         "PowerClericDivineInterventionImprovementCleric")
        //     .SetHasCastingFailure(false)
        //     .SetOverriddenPower(PowerClericDivineInterventionCleric)
        //     .AddToDB();
        //
        // var powerClericDivineInterventionImprovementPaladin = FeatureDefinitionPowerBuilder
        //     .Create(
        //         PowerClericDivineInterventionPaladin,
        //         "PowerClericDivineInterventionImprovementPaladin")
        //     .SetHasCastingFailure(false)
        //     .SetOverriddenPower(PowerClericDivineInterventionPaladin)
        //     .AddToDB();
        //
        // var powerClericDivineInterventionImprovementWizard = FeatureDefinitionPowerBuilder
        //     .Create(
        //         PowerClericDivineInterventionWizard,
        //         "PowerClericDivineInterventionImprovementWizard")
        //     .SetHasCastingFailure(false)
        //     .SetOverriddenPower(PowerClericDivineInterventionWizard)
        //     .AddToDB();
        //
        // DomainBattle.FeatureUnlocks.Add(
        //     new FeatureUnlockByLevel(PowerClericDivineInterventionPaladin,
        //         20));
        // DomainElementalCold.FeatureUnlocks.Add(
        //     new FeatureUnlockByLevel(powerClericDivineInterventionImprovementWizard, 20));
        // DomainElementalFire.FeatureUnlocks.Add(
        //     new FeatureUnlockByLevel(powerClericDivineInterventionImprovementWizard, 20));
        // DomainElementalLighting.FeatureUnlocks.Add(
        //     new FeatureUnlockByLevel(powerClericDivineInterventionImprovementWizard, 20));
        // DomainInsight.FeatureUnlocks.Add(
        //     new FeatureUnlockByLevel(powerClericDivineInterventionImprovementCleric, 20));
        // DomainLaw.FeatureUnlocks.Add(
        //     new FeatureUnlockByLevel(powerClericDivineInterventionImprovementPaladin, 20));
        // DomainLife.FeatureUnlocks.Add(
        //     new FeatureUnlockByLevel(powerClericDivineInterventionImprovementCleric, 20));
        // DomainOblivion.FeatureUnlocks.Add(
        //     new FeatureUnlockByLevel(powerClericDivineInterventionImprovementCleric, 20));
        // DomainSun.FeatureUnlocks.Add(
        //     new FeatureUnlockByLevel(powerClericDivineInterventionImprovementWizard, 20));
    }

    private static void DruidLoad()
    {
        Druid.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(FeatureSetAbilityScoreChoice, 16),
            // TODO 18: Druid Beast Spells
            new(FeatureSetAbilityScoreChoice, 19)
            // TODO 20: Druid Arch Druid
        });

        EnumerateSlotsPerLevel(
            CasterProgression.Full,
            CastSpellDruid.SlotsPerLevels);

        SpellListDruid.maxSpellLevel = 9;
    }

    private static void FighterLoad()
    {
        var powerFighterActionSurge2 = FeatureDefinitionPowerBuilder
            .Create(PowerFighterActionSurge, "PowerFighterActionSurge2")
            .SetUsesFixed(RuleDefinitions.ActivationTime.NoCost, RuleDefinitions.RechargeRate.LongRest, 1, 2)
            .SetOverriddenPower(PowerFighterActionSurge)
            .AddToDB();

        Fighter.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(AttributeModifierFighterIndomitableAdd1, 13),
            new(FeatureSetAbilityScoreChoice, 14),
            new(FeatureSetAbilityScoreChoice, 16),
            new(powerFighterActionSurge2, 17),
            new(AttributeModifierFighterIndomitableAdd1, 17),
            new(FeatureSetAbilityScoreChoice, 19),
            new(AttributeModifierFighterExtraAttack, 20)
        });
    }

    private static void MonkLoad()
    {
        Monk.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(FeatureSetMonkTongueSunMoon, 13),
            new(FeatureSetMonkDiamondSoul, 14),
            new(FeatureSetMonkTimelessBody, 15),
            new(FeatureSetAbilityScoreChoice, 16),
            // TODO 17: Monastic Tradition Feature
            // TODO 18: Empty Body
            new(FeatureSetAbilityScoreChoice, 19)
            // TODO 20: Perfect Self
        });
    }

    private static void PaladinLoad()
    {
        var effectPowerPaladinAuraOfCourage18 = new EffectDescription();

        effectPowerPaladinAuraOfCourage18.Copy(PowerPaladinAuraOfCourage.EffectDescription);
        effectPowerPaladinAuraOfCourage18.targetParameter = 6;
        effectPowerPaladinAuraOfCourage18.rangeParameter = 0;
        effectPowerPaladinAuraOfCourage18.requiresTargetProximity = false;

        var powerPaladinAuraOfCourage18 = FeatureDefinitionPowerBuilder
            .Create(PowerPaladinAuraOfCourage, "PowerPaladinAuraOfCourage18")
            .SetGuiPresentation(Category.Feature)
            .SetEffectDescription(effectPowerPaladinAuraOfCourage18)
            .SetOverriddenPower(PowerPaladinAuraOfCourage)
            .AddToDB();

        var effectPowerPaladinAuraOfProtection18 = new EffectDescription();

        effectPowerPaladinAuraOfProtection18.Copy(PowerPaladinAuraOfProtection.EffectDescription);
        effectPowerPaladinAuraOfProtection18.targetParameter = 6;
        effectPowerPaladinAuraOfProtection18.rangeParameter = 0;
        effectPowerPaladinAuraOfProtection18.requiresTargetProximity = false;

        var powerPaladinAuraOfProtection18 = FeatureDefinitionPowerBuilder
            .Create(PowerPaladinAuraOfProtection, "PowerPaladinAuraOfProtection18")
            .SetGuiPresentation(Category.Feature)
            .SetEffectDescription(effectPowerPaladinAuraOfProtection18)
            .SetOverriddenPower(PowerPaladinAuraOfCourage)
            .AddToDB();

        Paladin.FeatureUnlocks.AddRange(
            new FeatureUnlockByLevel(PowerPaladinCleansingTouch, 14),
            new FeatureUnlockByLevel(FeatureSetAbilityScoreChoice, 16),
            new FeatureUnlockByLevel(powerPaladinAuraOfCourage18, 18),
            new FeatureUnlockByLevel(powerPaladinAuraOfProtection18, 18),
            new FeatureUnlockByLevel(FeatureSetAbilityScoreChoice, 19)
            // TODO 20: Sacred Oath Feature
        );

        // AutoPreparedSpellsOathOfDevotion.AutoPreparedSpellsGroups.Add(
        //     new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup
        //     {
        //         ClassLevel = 17,
        //         SpellsList = new List<SpellDefinition>
        //         {
        //             // Commune,
        //             FlameStrike
        //         }
        //     });
        //
        // AutoPreparedSpellsOathOfMotherland.AutoPreparedSpellsGroups.Add(
        //     new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup
        //     {
        //         ClassLevel = 17, SpellsList = new List<SpellDefinition> { FlameStrike }
        //     });
        //
        // AutoPreparedSpellsOathOfTirmar.AutoPreparedSpellsGroups.Add(
        //     new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup
        //     {
        //         ClassLevel = 17, SpellsList = new List<SpellDefinition> { WallOfForce, HoldMonster }
        //     });

        EnumerateSlotsPerLevel(
            CasterProgression.Half,
            CastSpellPaladin.SlotsPerLevels);

        SpellListPaladin.maxSpellLevel = 5;
    }

    private static void RangerLoad()
    {
        var senseRangerFeralSenses = FeatureDefinitionSenseBuilder
            .Create(SenseSeeInvisible12, "SenseRangerFeralSenses")
            .SetGuiPresentation(Category.Feature)
            .SetSense(SenseMode.Type.DetectInvisibility, 6)
            .AddToDB();

        Ranger.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(AdditionalDamageRangerFavoredEnemyChoice, 14),
            new(ActionAffinityRangerVanish, 14),
            new(FeatureSetAbilityScoreChoice, 16),
            new(senseRangerFeralSenses, 18),
            new(FeatureSetAbilityScoreChoice, 19)
            // TODO 20: Ranger Foe Slayer
        });

        EnumerateSlotsPerLevel(
            CasterProgression.Half,
            CastSpellRanger.SlotsPerLevels);

        EnumerateKnownSpells(
            2,
            CasterProgression.Half,
            CastSpellRanger.KnownSpells);

        EnumerateReplacedSpells(
            3, 1, CastSpellRanger.ReplacedSpells);

        SpellListRanger.maxSpellLevel = 5;
    }

    private static void RogueLoad()
    {
        Rogue.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(SenseRogueBlindsense, 14),
            new(ProficiencyRogueSlipperyMind, 15),
            new(FeatureSetAbilityScoreChoice, 16),
            // TODO 18: Rogue Elusive
            new(FeatureSetAbilityScoreChoice, 19)
            // TODO 20: Rogue Stroke of Luck
        });
    }

    private static void SorcererLoad()
    {
        const string PowerSorcerousRestorationName = "PowerSorcerousRestoration";

        _ = RestActivityDefinitionBuilder
            .Create("SorcererSorcerousRestoration")
            .SetRestData(
                RestDefinitions.RestStage.AfterRest,
                RuleDefinitions.RestType.ShortRest,
                RestActivityDefinition.ActivityCondition.CanUsePower,
                FunctorDefinitions.FunctorUsePower,
                PowerSorcerousRestorationName)
            .SetGuiPresentation(PowerSorcerousRestorationName, Category.Feature)
            .AddToDB();

        var effectFormRestoration = EffectFormBuilder
            .Create()
            .CreatedByCharacter()
            .SetSpellForm(9)
            .Build();

        effectFormRestoration.SpellSlotsForm.type = SpellSlotsForm.EffectType.GainSorceryPoints;
        effectFormRestoration.SpellSlotsForm.sorceryPointsGain = 4;

        var powerSorcerousRestoration = FeatureDefinitionPowerBuilder
            .Create(PowerSorcerousRestorationName)
            .SetGuiPresentation("PowerSorcerousRestoration", Category.Feature)
            .SetUsesFixed(RuleDefinitions.ActivationTime.Rest)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetEffectForms(effectFormRestoration)
                .SetTargetingData(
                    RuleDefinitions.Side.Ally,
                    RuleDefinitions.RangeType.Self,
                    1,
                    RuleDefinitions.TargetType.Self)
                .SetParticleEffectParameters(PowerWizardArcaneRecovery.EffectDescription
                    .EffectParticleParameters)
                .Build())
            .AddToDB();

        Sorcerer.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(FeatureSetAbilityScoreChoice, 16),
            new(PointPoolSorcererAdditionalMetamagic, 17),
            new(FeatureSetAbilityScoreChoice, 19),
            new(powerSorcerousRestoration, 20)
        });

        EnumerateSlotsPerLevel(
            CasterProgression.Full,
            CastSpellSorcerer.SlotsPerLevels);

        EnumerateKnownSpells(
            2,
            CasterProgression.Full,
            CastSpellSorcerer.KnownSpells);

        EnumerateReplacedSpells(
            2, 1, CastSpellSorcerer.ReplacedSpells);

        SpellListSorcerer.maxSpellLevel = 9;
    }

    private static void WarlockLoad()
    {
        var pointPoolWarlockMysticArcanum9 = FeatureDefinitionPointPoolBuilder
            .Create(PointPoolWarlockMysticArcanum7, "PointPoolWarlockMysticArcanum9")
            .SetGuiPresentation(
                "Feature/&PointPoolWarlockMysticArcanum9Title",
                "Feature/&PointPoolWarlockMysticArcanumDescription")
            .AddToDB();

        var powerWarlockEldritchMaster = FeatureDefinitionPowerBuilder
            .Create(PowerWizardArcaneRecovery, PowerWarlockEldritchMasterName)
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(RuleDefinitions.ActivationTime.Minute1, RuleDefinitions.RechargeRate.LongRest)
            .AddToDB();

        Warlock.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(PointPoolWarlockMysticArcanum7, 13),
            new(PointPoolWarlockInvocation15, 15),
            new(PointPoolWarlockMysticArcanum8, 15),
            new(FeatureSetAbilityScoreChoice, 16),
            new(pointPoolWarlockMysticArcanum9, 18),
            new(FeatureSetAbilityScoreChoice, 19),
            new(powerWarlockEldritchMaster, 20)
        });

        CastSpellWarlock.KnownSpells.SetRange(SharedSpellsContext.WarlockKnownSpells);

        EnumerateReplacedSpells(
            2, 1, CastSpellWarlock.ReplacedSpells);

        SpellListWarlock.maxSpellLevel = 9;
    }

    private static void WizardLoad()
    {
        Wizard.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(FeatureSetAbilityScoreChoice, 16),
            // TODO 18: Spell Mastery
            new(FeatureSetAbilityScoreChoice, 19)
            // TODO 20: Signature Spells
        });

        EnumerateSlotsPerLevel(
            CasterProgression.Full,
            CastSpellWizard.SlotsPerLevels);

        EnumerateKnownSpells(
            6,
            CasterProgression.Full,
            CastSpellWizard.KnownSpells);

        SpellListWizard.maxSpellLevel = 9;
    }

    private static void MartialSpellBladeLoad()
    {
        EnumerateSlotsPerLevel(
            CasterProgression.OneThird,
            CastSpellMartialSpellBlade.SlotsPerLevels);

        EnumerateKnownSpells(
            3,
            CasterProgression.OneThird,
            CastSpellMartialSpellBlade.KnownSpells);

        EnumerateReplacedSpells(
            4, 1, CastSpellMartialSpellBlade.ReplacedSpells);
    }

    private static void RoguishShadowcasterLoad()
    {
        EnumerateSlotsPerLevel(
            CasterProgression.OneThird,
            CastSpellShadowcaster.SlotsPerLevels);

        EnumerateKnownSpells(
            3,
            CasterProgression.OneThird,
            CastSpellShadowcaster.KnownSpells);

        EnumerateReplacedSpells(
            4, 1, CastSpellShadowcaster.ReplacedSpells);
    }

    [NotNull]
    private static IEnumerable<CodeInstruction> Level20Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
    {
        var code = new List<CodeInstruction>(instructions);

        if (!Main.Settings.EnableLevel20)
        {
            return code;
        }

        code
            .FindAll(x => x.opcode.Name == "ldc.i4.s"
                          && (Convert.ToInt32(x.operand) == GameFinalMaxLevel ||
                              Convert.ToInt32(x.operand) == GameMaxLevel))
            .ForEach(x => x.operand = ModMaxLevel);

        return code;
    }
}
