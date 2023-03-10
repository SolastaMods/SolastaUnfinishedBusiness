using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionCastSpells;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPointPools;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionProficiencys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellListDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Builders.Features.FeatureDefinitionCastSpellBuilder;

namespace SolastaUnfinishedBusiness.Models;

internal static class Level20Context
{
    internal const string PowerWarlockEldritchMasterName = "PowerWarlockEldritchMaster";

    internal const int ModMaxLevel = 20;

    internal const int ModMaxExperience = 355000;
    internal const int GameMaxExperience = 100000;
    internal static readonly int GameMaxLevel = Main.IsDebugBuild ? 16 : 12;

    internal static void Load()
    {
        InitExperienceThresholdsTable();

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
        // that might affect multiplayer sessions, prevent level up from 19 to 20 and prevent some MC scenarios
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

            while (spellsByLevel.Count < 10)
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

            while (spellsByLevel.Count < 10)
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
            // typeof(CharacterBuildingManager).GetMethod("CreateCharacterFromTemplate"), // 12
            typeof(CharactersPanel).GetMethod("Refresh", PrivateBinding), // 12
            typeof(FeatureDefinitionCastSpell).GetMethod("EnsureConsistency"), // 12
            typeof(HigherLevelFeaturesModal).GetMethod("Bind"), // 12
            typeof(InvocationSubPanel).GetMethod("SetState"), // 12
            typeof(RulesetCharacterHero).GetMethod("RegisterAttributes"), // 12
            typeof(RulesetCharacterHero).GetMethod("SerializeElements"), // 12
            typeof(RulesetEntity).GetMethod("SerializeElements"), // 12
            typeof(UserCampaignEditorScreen).GetMethod("OnMaxLevelEndEdit"), // 12
            typeof(UserCampaignEditorScreen).GetMethod("OnMinLevelEndEdit"), // 12
            typeof(UserLocationSettingsModal).GetMethod("OnMaxLevelEndEdit"), // 12
            typeof(UserLocationSettingsModal).GetMethod("OnMinLevelEndEdit") // 12
        };

        foreach (var method in methods)
        {
            try
            {
                harmony.Patch(method, transpiler: new HarmonyMethod(transpiler));
            }
            catch
            {
                Main.Error($"Failed to apply Level20Transpiler patch to {method.DeclaringType}.{method.Name}");
            }
        }
    }

    private static void InitExperienceThresholdsTable()
    {
        var len = ExperienceThresholds.Length;
        var experience = new int[len + 1];

        Array.Copy(ExperienceThresholds, experience, len);
        experience[len] = experience[len - 1];

        ExperienceThresholds = experience;
    }

    private static void BarbarianLoad()
    {
        var changeAbilityCheckBarbarianIndomitableMight = FeatureDefinitionBuilder
            .Create("ChangeAbilityCheckBarbarianIndomitableMight")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new ChangeAbilityCheckBarbarianIndomitableMight())
            .AddToDB();

        var customCodeBarbarianPrimalChampion = FeatureDefinitionBuilder
            .Create("CustomCodeBarbarianPrimalChampion")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new CustomCodeBarbarianPrimalChampion())
            .AddToDB();

        if (!Main.IsDebugBuild)
        {
            Barbarian.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
            {
                new(AttributeModifierBarbarianBrutalCriticalAdd, 13),
                new(PowerBarbarianPersistentRageStart, 15),
                new(AttributeModifierBarbarianRageDamageAdd, 16),
                new(FeatureSetAbilityScoreChoice, 16)
            });
        }

        Barbarian.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(AttributeModifierBarbarianBrutalCriticalAdd, 17),
            new(AttributeModifierBarbarianRagePointsAdd, 17),
            new(changeAbilityCheckBarbarianIndomitableMight, 18),
            new(FeatureSetAbilityScoreChoice, 19),
            new(customCodeBarbarianPrimalChampion, 20)
        });
    }

    private static void BardLoad()
    {
        var pointPoolBardMagicalSecrets18 = FeatureDefinitionPointPoolBuilder
            .Create(PointPoolBardMagicalSecrets14, "PointPoolBardMagicalSecrets18")
            .AddToDB();

        if (!Main.IsDebugBuild)
        {
            Bard.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
            {
                new(PointPoolBardMagicalSecrets14, 14),
                new(AttributeModifierBardicInspirationDieD12, 15),
                new(FeatureSetAbilityScoreChoice, 16)
            });
        }

        Bard.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(pointPoolBardMagicalSecrets18, 18), new(FeatureSetAbilityScoreChoice, 19)
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

        if (!Main.IsDebugBuild)
        {
            SpellListBard.SpellsByLevel.Add(new SpellListDefinition.SpellsByLevelDuplet
            {
                level = 7, Spells = new List<SpellDefinition> { Resurrection }
            });
        }
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

        if (!Main.IsDebugBuild)
        {
            Cleric.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
            {
                new(PowerClericTurnUndead14, 14), new(FeatureSetAbilityScoreChoice, 16)
            });
        }

        Cleric.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(powerClericTurnUndead17, 17),
            new(AttributeModifierClericChannelDivinityAdd, 18),
            new(FeatureSetAbilityScoreChoice, 19)
        });

        EnumerateSlotsPerLevel(
            CasterProgression.Full,
            CastSpellCleric.SlotsPerLevels);

        SpellListCleric.maxSpellLevel = 9;

        // Divine Intervention

        var powerClericDivineInterventionImprovementCleric = FeatureDefinitionPowerBuilder
            .Create(PowerClericDivineInterventionCleric, "PowerClericDivineInterventionImprovementCleric")
            .SetHasCastingFailure(false)
            .SetOverriddenPower(PowerClericDivineInterventionCleric)
            .AddToDB();

        var powerClericDivineInterventionImprovementPaladin = FeatureDefinitionPowerBuilder
            .Create(PowerClericDivineInterventionPaladin, "PowerClericDivineInterventionImprovementPaladin")
            .SetHasCastingFailure(false)
            .SetOverriddenPower(PowerClericDivineInterventionPaladin)
            .AddToDB();

        var powerClericDivineInterventionImprovementWizard = FeatureDefinitionPowerBuilder
            .Create(PowerClericDivineInterventionWizard, "PowerClericDivineInterventionImprovementWizard")
            .SetHasCastingFailure(false)
            .SetOverriddenPower(PowerClericDivineInterventionWizard)
            .AddToDB();

        DomainBattle.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementPaladin, 20));
        DomainElementalCold.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementWizard, 20));
        DomainElementalFire.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementWizard, 20));
        DomainElementalLighting.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementWizard, 20));
        DomainInsight.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementCleric, 20));
        DomainLaw.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementPaladin, 20));
        DomainLife.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementCleric, 20));
        DomainMischief.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementWizard, 20));
        DomainOblivion.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementCleric, 20));
        DomainSun.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementWizard, 20));

        if (!Main.IsDebugBuild)
        {
            SpellListCleric.SpellsByLevel.Add(new SpellListDefinition.SpellsByLevelDuplet
            {
                level = 7, Spells = new List<SpellDefinition> { Resurrection }
            });
        }
    }

    private static void DruidLoad()
    {
        // only a placeholder to display the feature name
        // this is solved on CanCastSpells patch
        var druidBeastSpells = FeatureDefinitionBuilder
            .Create("FeatureDruidBeastSpells")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        if (!Main.IsDebugBuild)
        {
            Druid.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel> { new(FeatureSetAbilityScoreChoice, 16) });
        }

        Druid.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(druidBeastSpells, 18), new(FeatureSetAbilityScoreChoice, 19)
            // TODO 20: Druid Arch Druid
        });

        EnumerateSlotsPerLevel(
            CasterProgression.Full,
            CastSpellDruid.SlotsPerLevels);

        SpellListDruid.maxSpellLevel = 9;

        if (!Main.IsDebugBuild)
        {
            SpellListDruid.SpellsByLevel.Add(new SpellListDefinition.SpellsByLevelDuplet
            {
                level = 7, Spells = new List<SpellDefinition> { Resurrection }
            });
        }
    }

    private static void FighterLoad()
    {
        var powerFighterActionSurge2 = FeatureDefinitionPowerBuilder
            .Create(PowerFighterActionSurge, "PowerFighterActionSurge2")
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest, 1, 2)
            .SetOverriddenPower(PowerFighterActionSurge)
            .AddToDB();

        if (!Main.IsDebugBuild)
        {
            Fighter.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
            {
                new(AttributeModifierFighterIndomitableAdd1, 13),
                new(FeatureSetAbilityScoreChoice, 14),
                new(FeatureSetAbilityScoreChoice, 16)
            });
        }

        Fighter.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(powerFighterActionSurge2, 17),
            new(AttributeModifierFighterIndomitableAdd1, 17),
            new(FeatureSetAbilityScoreChoice, 19),
            new(AttributeModifierFighterExtraAttack, 20)
        });
    }

    private static void MonkLoad()
    {
        var emptyBodySprite = Sprites.GetSprite("EmptyBody", Resources.EmptyBody, 128, 64);

        var powerMonkEmptyBody = FeatureDefinitionPowerBuilder
            .Create("PowerMonkEmptyBody")
            .SetGuiPresentation(Category.Feature, emptyBodySprite)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints, 4)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Minute, 1)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(DatabaseHelper.ConditionDefinitions.ConditionInvisibleGreater,
                            ConditionForm.ConditionOperation.Add)
                        .Build(),
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(ConditionDefinitionBuilder
                                .Create("ConditionMonkEmptyBody")
                                .SetGuiPresentation(
                                    Category.Condition,
                                    DatabaseHelper.ConditionDefinitions.ConditionShielded)
                                .AddFeatures(
                                    DamageAffinityAcidResistance,
                                    DamageAffinityColdResistance,
                                    DamageAffinityFireResistance,
                                    DamageAffinityLightningResistance,
                                    DamageAffinityNecroticResistance,
                                    DamageAffinityPoisonResistance,
                                    DamageAffinityPsychicResistance,
                                    DamageAffinityRadiantResistance,
                                    DamageAffinityThunderResistance,
                                    FeatureDefinitionDamageAffinityBuilder
                                        .Create("DamageAffinityMonkEmptyBodyBludgeoningResistance")
                                        .SetGuiPresentationNoContent(true)
                                        .SetDamageType(DamageTypeBludgeoning)
                                        .SetDamageAffinityType(DamageAffinityType.Resistance)
                                        .AddToDB(),
                                    FeatureDefinitionDamageAffinityBuilder
                                        .Create("DamageAffinityMonkEmptyBodyPiercingResistance")
                                        .SetGuiPresentationNoContent(true)
                                        .SetDamageType(DamageTypePiercing)
                                        .SetDamageAffinityType(DamageAffinityType.Resistance)
                                        .AddToDB(),
                                    FeatureDefinitionDamageAffinityBuilder
                                        .Create("DamageAffinityMonkEmptyBodySlashingResistance")
                                        .SetGuiPresentationNoContent(true)
                                        .SetDamageType(DamageTypeSlashing)
                                        .SetDamageAffinityType(DamageAffinityType.Resistance)
                                        .AddToDB())
                                .SetPossessive()
                                .AddToDB(),
                            ConditionForm.ConditionOperation.Add)
                        .Build())
                .Build())
            .AddToDB();

        var battleStartedListenerMonkPerfectSelf = FeatureDefinitionBuilder
            .Create("BattleStartedListenerMonkPerfectSelf")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new BattleStartedListenerMonkPerfectSelf())
            .AddToDB();

        if (!Main.IsDebugBuild)
        {
            Monk.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
            {
                new(FeatureSetMonkTongueSunMoon, 13),
                new(FeatureSetMonkDiamondSoul, 14),
                new(FeatureSetMonkTimelessBody, 15),
                new(FeatureSetAbilityScoreChoice, 16)
            });
        }

        Monk.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            // TODO 17: Monastic Tradition Feature
            new(powerMonkEmptyBody, 18),
            new(FeatureSetAbilityScoreChoice, 19),
            new(battleStartedListenerMonkPerfectSelf, 20)
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

        if (!Main.IsDebugBuild)
        {
            Paladin.FeatureUnlocks.AddRange(
                new FeatureUnlockByLevel(PowerPaladinCleansingTouch, 14),
                new FeatureUnlockByLevel(FeatureSetAbilityScoreChoice, 16)
            );

            DatabaseHelper.FeatureDefinitionAutoPreparedSpellss.AutoPreparedSpellsOathOfDevotion
                .AutoPreparedSpellsGroups.Add(
                    BuildSpellGroup(13, GuardianOfFaith, FreedomOfMovement));

            DatabaseHelper.FeatureDefinitionAutoPreparedSpellss.AutoPreparedSpellsOathOfJugement
                .AutoPreparedSpellsGroups.Add(
                    BuildSpellGroup(13, Banishment, Blight));

            DatabaseHelper.FeatureDefinitionAutoPreparedSpellss.AutoPreparedSpellsOathOfMotherland
                .AutoPreparedSpellsGroups.Add(
                    BuildSpellGroup(13, WallOfFire, FireShield));

            DatabaseHelper.FeatureDefinitionAutoPreparedSpellss.AutoPreparedSpellsOathOfTirmar.AutoPreparedSpellsGroups
                .Add(
                    BuildSpellGroup(13, DreadfulOmen, PhantasmalKiller));
        }

        Paladin.FeatureUnlocks.AddRange(
            new FeatureUnlockByLevel(powerPaladinAuraOfCourage18, 18),
            new FeatureUnlockByLevel(powerPaladinAuraOfProtection18, 18),
            new FeatureUnlockByLevel(FeatureSetAbilityScoreChoice, 19)
            // TODO 20: Sacred Oath Feature
        );

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

        if (!Main.IsDebugBuild)
        {
            Ranger.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
            {
                new(AdditionalDamageRangerFavoredEnemyChoice, 14),
                new(ActionAffinityRangerVanish, 14),
                new(FeatureSetAbilityScoreChoice, 16)
            });
        }

        Ranger.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(senseRangerFeralSenses, 18), new(FeatureSetAbilityScoreChoice, 19)
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
        if (!Main.IsDebugBuild)
        {
            Rogue.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
            {
                new(SenseRogueBlindsense, 14),
                new(ProficiencyRogueSlipperyMind, 15),
                new(FeatureSetAbilityScoreChoice, 16)
            });
        }

        Rogue.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            // TODO 18: Rogue Elusive
            new(FeatureSetAbilityScoreChoice, 19)
            // TODO 20: Rogue Stroke of Luck
        });
    }

    private static void SorcererLoad()
    {
        const string PowerSorcerousRestorationName = "PowerSorcererSorcerousRestoration";

        _ = RestActivityDefinitionBuilder
            .Create("RestActivitySorcererSorcerousRestoration")
            .SetGuiPresentation(PowerSorcerousRestorationName, Category.Feature)
            .SetRestData(
                RestDefinitions.RestStage.AfterRest,
                RestType.ShortRest,
                RestActivityDefinition.ActivityCondition.CanUsePower,
                FunctorDefinitions.FunctorUsePower,
                PowerSorcerousRestorationName)
            .AddToDB();

        var effectFormRestoration = EffectFormBuilder
            .Create()
            .SetSpellForm(9)
            .Build();

        effectFormRestoration.SpellSlotsForm.type = SpellSlotsForm.EffectType.GainSorceryPoints;
        effectFormRestoration.SpellSlotsForm.sorceryPointsGain = 4;

        var powerSorcerousRestoration = FeatureDefinitionPowerBuilder
            .Create(PowerSorcerousRestorationName)
            .SetGuiPresentation("PowerSorcererSorcerousRestoration", Category.Feature)
            .SetUsesFixed(ActivationTime.Rest)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetEffectForms(effectFormRestoration)
                .SetTargetingData(
                    Side.Ally,
                    RangeType.Self,
                    1,
                    TargetType.Self)
                .SetParticleEffectParameters(PowerWizardArcaneRecovery.EffectDescription
                    .EffectParticleParameters)
                .Build())
            .AddToDB();

        if (!Main.IsDebugBuild)
        {
            Sorcerer.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel> { new(FeatureSetAbilityScoreChoice, 16) });
        }

        Sorcerer.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
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
            .SetUsesFixed(ActivationTime.Minute1, RechargeRate.LongRest)
            .AddToDB();

        if (!Main.IsDebugBuild)
        {
            Warlock.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
            {
                new(PointPoolWarlockMysticArcanum7, 13),
                new(PointPoolWarlockInvocation15, 15),
                new(PointPoolWarlockMysticArcanum8, 15),
                new(FeatureSetAbilityScoreChoice, 16)
            });
        }

        Warlock.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
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

        var result = code
            .FindAll(x => x.opcode == OpCodes.Ldc_I4_S && Convert.ToInt32(x.operand) == GameMaxLevel);

        if (result.Count > 0)
        {
            result.ForEach(x => x.operand = ModMaxLevel);
        }
        else
        {
            Main.Error("Level20Transpiler");
        }

        return code;
    }

    private sealed class ChangeAbilityCheckBarbarianIndomitableMight : IChangeAbilityCheck
    {
        public int MinRoll(
            [CanBeNull] RulesetCharacter character,
            int baseBonus,
            int rollModifier,
            string abilityScoreName,
            string proficiencyName,
            List<TrendInfo> advantageTrends,
            List<TrendInfo> modifierTrends)
        {
            if (character == null || abilityScoreName != AttributeDefinitions.Strength)
            {
                return 1;
            }

            return character.GetAttribute(AttributeDefinitions.Strength).CurrentValue;
        }
    }

    private sealed class CustomCodeBarbarianPrimalChampion : IFeatureDefinitionCustomCode
    {
        public void ApplyFeature([NotNull] RulesetCharacterHero hero, string tag)
        {
            ModifyAttributeAndMax(hero, AttributeDefinitions.Strength, 4);
            ModifyAttributeAndMax(hero, AttributeDefinitions.Constitution, 4);

            hero.RefreshAll();
        }

#if false
        public void RemoveFeature([NotNull] RulesetCharacterHero hero, string tag)
        {
            ModifyAttributeAndMax(hero, AttributeDefinitions.Strength, -4);
            ModifyAttributeAndMax(hero, AttributeDefinitions.Constitution, -4);
        
            hero.RefreshAll();
        }
#endif

        private static void ModifyAttributeAndMax([NotNull] RulesetActor hero, string attributeName, int amount)
        {
            var attribute = hero.GetAttribute(attributeName);

            attribute.BaseValue += amount;
            attribute.MaxValue += amount;
            attribute.MaxEditableValue += amount;
            attribute.Refresh();

            hero.AbilityScoreIncreased?.Invoke(hero, attributeName, amount, amount);
        }
    }

    private sealed class BattleStartedListenerMonkPerfectSelf : ICharacterBattleStartedListener
    {
        public void OnCharacterBattleStarted(GameLocationCharacter locationCharacter, bool surprise)
        {
            var character = locationCharacter.RulesetCharacter;

            if (character == null)
            {
                return;
            }

            if (character.RemainingKiPoints != 0)
            {
                return;
            }

            character.ForceKiPointConsumption(-4);
            character.KiPointsAltered?.Invoke(character, character.RemainingKiPoints);
            GameConsoleHelper.LogCharacterActivatesAbility(character, "Feature/&MonkPerfectSelfTitle");
        }
    }
}
