using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Subclasses;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionCastSpells;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPointPools;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellListDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.FeatureDefinitionCastSpellBuilder;

namespace SolastaUnfinishedBusiness.Models;

internal static class Level20Context
{
    internal const string PowerWarlockEldritchMasterName = "PowerWarlockEldritchMaster";
    internal const int ModMaxLevel = 20;
    internal const int ModMaxExperience = 355000;
    internal const int GameMaxLevel = 16;

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

        Level20SubclassesContext.Load();

        InitExperienceThresholdsTable();
    }

    internal static void LateLoad()
    {
        const BindingFlags PrivateBinding = BindingFlags.Instance | BindingFlags.NonPublic;

        var harmony = new Harmony("SolastaUnfinishedBusiness");
        var transpiler = new Func<IEnumerable<CodeInstruction>, IEnumerable<CodeInstruction>>(Level20Transpiler).Method;

        // these are currently the hard-coded levels on below methods
        var methods = new[]
        {
            typeof(ArchetypesPreviewModal).GetMethod("Refresh", PrivateBinding),
            typeof(CharactersPanel).GetMethod("Refresh", PrivateBinding),
            typeof(FeatureDefinitionCastSpell).GetMethod("EnsureConsistency"),
            typeof(HigherLevelFeaturesModal).GetMethod("Bind"), typeof(InvocationSubPanel).GetMethod("SetState"),
            typeof(RulesetCharacterHero).GetMethod("RegisterAttributes"),
            typeof(RulesetCharacterHero).GetMethod("SerializeElements"),
            typeof(RulesetEntity).GetMethod("SerializeElements"),
            typeof(UserCampaignEditorScreen).GetMethod("OnMaxLevelEndEdit"),
            typeof(UserCampaignEditorScreen).GetMethod("OnMinLevelEndEdit"),
            typeof(UserLocationSettingsModal).GetMethod("OnMaxLevelEndEdit"),
            typeof(UserLocationSettingsModal).GetMethod("OnMinLevelEndEdit")
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

    [NotNull]
    // ReSharper disable once SuggestBaseTypeForParameter
    private static List<CodeInstruction> Level20Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
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

    private static void BarbarianLoad()
    {
        var changeAbilityCheckBarbarianIndomitableMight = FeatureDefinitionBuilder
            .Create("ChangeAbilityCheckBarbarianIndomitableMight")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new ModifyAbilityCheckBarbarianIndomitableMight())
            .AddToDB();

        var customCodeBarbarianPrimalChampion = FeatureDefinitionBuilder
            .Create("CustomCodeBarbarianPrimalChampion")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new CustomLevelUpLogicBarbarianPrimalChampion())
            .AddToDB();

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

        var featureBardSuperiorInspiration = FeatureDefinitionBuilder
            .Create("FeatureBardSuperiorInspiration")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureBardSuperiorInspiration.AddCustomSubFeatures(
            new BattleStartedListenerBardSuperiorInspiration(featureBardSuperiorInspiration));

        Bard.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(pointPoolBardMagicalSecrets18, 18),
            new(FeatureSetAbilityScoreChoice, 19),
            new(featureBardSuperiorInspiration, 20)
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
            new(powerClericTurnUndead17, 17),
            new(AttributeModifierClericChannelDivinityAdd, 18),
            new(FeatureSetAbilityScoreChoice, 19)
        });

        EnumerateSlotsPerLevel(
            CasterProgression.Full,
            CastSpellCleric.SlotsPerLevels);

        SpellListCleric.maxSpellLevel = 9;
    }

    private static void DruidLoad()
    {
        // only a placeholder to display the feature name as this is solved on CanCastSpells patch
        var featureDruidBeastSpells = FeatureDefinitionBuilder
            .Create("FeatureDruidBeastSpells")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var magicAffinityArchDruid = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinityArchDruid")
            .SetGuiPresentation(Category.Feature)
            .SetHandsFullCastingModifiers(true, true, true)
            .AddToDB();

        magicAffinityArchDruid.AddCustomSubFeatures(new ActionFinishedByMeArchDruid(magicAffinityArchDruid));

        Druid.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(featureDruidBeastSpells, 18), new(FeatureSetAbilityScoreChoice, 19), new(magicAffinityArchDruid, 20)
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
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest, 1, 2)
            .SetOverriddenPower(PowerFighterActionSurge)
            .AddToDB();

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
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints, 4, 4)
            .SetEffectDescription(
                EffectDescriptionBuilder
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
                            .SetConditionForm(
                                ConditionDefinitionBuilder
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
            .AddToDB();

        battleStartedListenerMonkPerfectSelf.AddCustomSubFeatures(
            new BattleStartedListenerMonkPerfectSelf(battleStartedListenerMonkPerfectSelf));

        Monk.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(powerMonkEmptyBody, 18),
            new(FeatureSetAbilityScoreChoice, 19),
            new(battleStartedListenerMonkPerfectSelf, 20)
        });
    }

    private static void PaladinLoad()
    {
        var powerPaladinAuraOfCourage18 = FeatureDefinitionPowerBuilder
            .Create(PowerPaladinAuraOfCourage, "PowerPaladinAuraOfCourage18")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetOverriddenPower(PowerPaladinAuraOfCourage)
            .AddToDB();

        powerPaladinAuraOfCourage18.EffectDescription.targetParameter = 13;

        var powerPaladinAuraOfProtection18 = FeatureDefinitionPowerBuilder
            .Create(PowerPaladinAuraOfProtection, "PowerPaladinAuraOfProtection18")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetOverriddenPower(PowerPaladinAuraOfProtection)
            .AddToDB();

        powerPaladinAuraOfProtection18.EffectDescription.targetParameter = 13;

        Paladin.FeatureUnlocks.AddRange(
            new FeatureUnlockByLevel(powerPaladinAuraOfCourage18, 18),
            new FeatureUnlockByLevel(powerPaladinAuraOfProtection18, 18),
            new FeatureUnlockByLevel(FeatureSetAbilityScoreChoice, 19)
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

        var featureFoeSlayer = FeatureDefinitionBuilder
            .Create("FeatureRangerFoeSlayer")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureFoeSlayer.AddCustomSubFeatures(new ModifyWeaponAttackModeRangerFoeSlayer(featureFoeSlayer));

        Ranger.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(senseRangerFeralSenses, 18), new(FeatureSetAbilityScoreChoice, 19), new(featureFoeSlayer, 20)
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
        var featureRogueElusive = FeatureDefinitionBuilder
            .Create("FeatureRogueElusive")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new PhysicalAttackInitiatedOnMeRogueElusive())
            .AddToDB();

        var powerRogueStrokeOfLuck = FeatureDefinitionPowerBuilder
            .Create("PowerRogueStrokeOfLuck")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.ShortRest)
            .SetReactionContext(ExtraReactionContext.Custom)
            .AddToDB();

        powerRogueStrokeOfLuck.AddCustomSubFeatures(
            new TryAlterOutcomePhysicalAttackByMeRogueStrokeOfLuck(powerRogueStrokeOfLuck));

        Rogue.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(featureRogueElusive, 18), new(FeatureSetAbilityScoreChoice, 19), new(powerRogueStrokeOfLuck, 20)
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
            .SetEffectDescription(
                EffectDescriptionBuilder
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
            .Create("PointPoolWarlockMysticArcanum9")
            .SetGuiPresentation(Category.Feature, "Feature/&PointPoolWarlockMysticArcanumDescription")
            .SetSpellOrCantripPool(HeroDefinitions.PointsPoolType.Spell, 1, null, "MysticArcanum", 9, 9)
            .AddToDB();

        pointPoolWarlockMysticArcanum9.minSpellLevel = 9;
        pointPoolWarlockMysticArcanum9.maxSpellLevel = 9;

        var pointPoolWarlockInvocation18 = FeatureDefinitionPointPoolBuilder
            .Create("PointPoolWarlockInvocation18")
            .SetGuiPresentation("PointPoolWarlockInvocationAdditional", Category.Feature)
            .SetPool(HeroDefinitions.PointsPoolType.Invocation, 1)
            .AddToDB();

        var powerWarlockEldritchMaster = FeatureDefinitionPowerBuilder
            .Create(PowerWizardArcaneRecovery, PowerWarlockEldritchMasterName)
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Minute1, RechargeRate.LongRest)
            .AddToDB();

        Warlock.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(pointPoolWarlockMysticArcanum9, 17),
            new(pointPoolWarlockInvocation18, 18),
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
        var spellMastery = BuildWizardSpellMastery();
        var signatureSpells = BuildWizardSignatureSpells();

        Wizard.FeatureUnlocks.AddRange(new List<FeatureUnlockByLevel>
        {
            new(spellMastery, 18), new(FeatureSetAbilityScoreChoice, 19), new(signatureSpells, 20)
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

    private static void InitExperienceThresholdsTable()
    {
        var len = ExperienceThresholds.Length;
        var experience = new int[len + 1];

        Array.Copy(ExperienceThresholds, experience, len);
        experience[len] = experience[len - 1];

        ExperienceThresholds = experience;
    }

    //
    // HELPERS
    //

    private static FeatureDefinitionGrantInvocations BuildWizardSpellMastery()
    {
        const string SPELL_MASTERY = "SpellMastery";

        // any non reaction spell of 1st or 2nd level
        var allPossibleSpells = SpellListAllSpells.SpellsByLevel
            .Where(x => x.level is 1 or 2)
            .SelectMany(x => x.Spells)
            .Where(x => x.ActivationTime != ActivationTime.Reaction);

        var invocations = allPossibleSpells
            .Select(spell =>
                CustomInvocationDefinitionBuilder
                    .Create($"CustomInvocation{SPELL_MASTERY}{spell.Name}")
                    .SetGuiPresentation(spell.GuiPresentation)
                    .SetPoolType(InvocationPoolTypeCustom.Pools.SpellMastery)
                    .SetGrantedSpell(spell)
                    .SetRequirements(3)
                    .AddCustomSubFeatures(
                        ValidateRepertoireForAutoprepared.HasSpellCastingFeature(CastSpellWizard.Name),
                        IsValid())
                    .AddToDB());

        var grantInvocationsSpellMastery = FeatureDefinitionGrantInvocationsBuilder
            .Create($"GrantInvocations{SPELL_MASTERY}")
            .SetGuiPresentation(Category.Feature)
            .SetInvocations(invocations)
            .AddToDB();

        return grantInvocationsSpellMastery;

        static IsInvocationValidHandler IsValid()
        {
            return (character, invocation) =>
            {
                var spellRepertoire = character.GetClassSpellRepertoire(Wizard);

                if (spellRepertoire == null)
                {
                    return false;
                }

                // get the first 2 non reaction prepared spells of 1st or 2nd level
                var preparedSpells = spellRepertoire.PreparedSpells
                    .Where(x => x.SpellLevel is 1 or 2 && x.ActivationTime != ActivationTime.Reaction)
                    .Take(2);

                return preparedSpells.Contains(invocation.GrantedSpell);
            };
        }
    }

    private static FeatureDefinitionCustomInvocationPool BuildWizardSignatureSpells()
    {
        const string SIGNATURE_SPELLS = "SignatureSpells";

        // any non reaction spell of 3rd level
        var allPossibleSpells = SpellListAllSpells.SpellsByLevel
            .Where(x => x.level is 3)
            .SelectMany(x => x.Spells)
            .Where(x => x.ActivationTime != ActivationTime.Reaction)
            .ToList();

        allPossibleSpells
            .ForEach(spell =>
                CustomInvocationDefinitionBuilder
                    .Create($"CustomInvocation{SIGNATURE_SPELLS}{spell.name}")
                    .SetGuiPresentation(spell.GuiPresentation)
                    .SetPoolType(InvocationPoolTypeCustom.Pools.SignatureSpells)
                    .SetGrantedSpell(spell)
                    .AddCustomSubFeatures(
                        RechargeInvocationOnShortRest.Marker,
                        ValidateRepertoireForAutoprepared.HasSpellCastingFeature(CastSpellWizard.Name))
                    .AddToDB());

        var invocationPoolWizardSignatureSpells = CustomInvocationPoolDefinitionBuilder
            .Create("InvocationPoolWizardSignatureSpells")
            .SetGuiPresentation(Category.Feature)
            .Setup(InvocationPoolTypeCustom.Pools.SignatureSpells, 2)
            .AddToDB();

        return invocationPoolWizardSignatureSpells;
    }

    private sealed class ActionFinishedByMeArchDruid(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition featureDefinition) : IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            if (action is not CharacterActionUsePower actionUsePower)
            {
                yield break;
            }

            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var power = actionUsePower.activePower.PowerDefinition == PowerDruidWildShape
                ? PowerDruidWildShape
                : actionUsePower.activePower.PowerDefinition == CircleOfTheNight.PowerCircleOfTheNightWildShapeCombat
                    ? CircleOfTheNight.PowerCircleOfTheNightWildShapeCombat
                    : null;

            if (power == null)
            {
                yield break;
            }

            var usablePower = PowerProvider.Get(power, rulesetCharacter);

            usablePower.Recharge();
            rulesetCharacter.LogCharacterUsedFeature(featureDefinition);
        }
    }

    private sealed class ModifyAbilityCheckBarbarianIndomitableMight : IModifyAbilityCheck
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

            return character.TryGetAttributeValue(AttributeDefinitions.Strength);
        }
    }

    private sealed class CustomLevelUpLogicBarbarianPrimalChampion : ICustomLevelUpLogic
    {
        public void ApplyFeature([NotNull] RulesetCharacterHero hero, string tag)
        {
            ModifyAttributeAndMax(hero, AttributeDefinitions.Strength, 4);
            ModifyAttributeAndMax(hero, AttributeDefinitions.Constitution, 4);

            hero.RefreshAll();
        }

        public void RemoveFeature([NotNull] RulesetCharacterHero hero, string tag)
        {
            ModifyAttributeAndMax(hero, AttributeDefinitions.Strength, -4);
            ModifyAttributeAndMax(hero, AttributeDefinitions.Constitution, -4);

            hero.RefreshAll();
        }

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

    private sealed class BattleStartedListenerMonkPerfectSelf(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition featureDefinition)
        : ICharacterBattleStartedListener
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
            character.LogCharacterUsedFeature(featureDefinition);
        }
    }

    private sealed class PhysicalAttackInitiatedOnMeRogueElusive : IPhysicalAttackInitiatedOnMe
    {
        public IEnumerator OnPhysicalAttackInitiatedOnMe(
            GameLocationBattleManager __instance,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackerAttackMode)
        {
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (rulesetDefender.HasAnyConditionOfTypeOrSubType(ConditionIncapacitated))
            {
                yield break;
            }

            attackModifier.attackAdvantageTrends.Clear();
            attackModifier.ignoreAdvantage = true;
        }
    }

    private sealed class ModifyWeaponAttackModeRangerFoeSlayer(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition featureDefinition)
        : IModifyWeaponAttackMode
    {
        public void ModifyAttackMode(RulesetCharacter character, [CanBeNull] RulesetAttackMode attackMode)
        {
            var damage = attackMode?.EffectDescription.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            var wisdom = character.TryGetAttributeValue(AttributeDefinitions.Wisdom);
            var wisdomModifier = AttributeDefinitions.ComputeAbilityScoreModifier(wisdom);

            damage.BonusDamage += wisdomModifier;
            damage.DamageBonusTrends.Add(new TrendInfo(wisdomModifier, FeatureSourceType.CharacterFeature,
                featureDefinition.Name,
                featureDefinition));
        }
    }

    private class TryAlterOutcomePhysicalAttackByMeRogueStrokeOfLuck(FeatureDefinitionPower power)
        : ITryAlterOutcomeAttack
    {
        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager battle,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier attackModifier)
        {
            var gameLocationActionManager =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (gameLocationActionManager == null)
            {
                yield break;
            }

            if (action.AttackRollOutcome is not (RollOutcome.Failure or RollOutcome.CriticalFailure))
            {
                yield break;
            }

            if (attacker != helper)
            {
                yield break;
            }

            var rulesetCharacter = attacker.RulesetCharacter;

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false } ||
                !rulesetCharacter.CanUsePower(power) ||
                !attacker.CanPerceiveTarget(defender))
            {
                yield break;
            }

            var usablePower = PowerProvider.Get(power, rulesetCharacter);
            var reactionParams = new CharacterActionParams(attacker, (ActionDefinitions.Id)ExtraActionId.DoNothingFree)
            {
                StringParameter = "Reaction/&CustomReactionRogueStrokeOfLuckReactDescription",
                UsablePower = usablePower
            };
            var previousReactionCount = gameLocationActionManager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("RogueStrokeOfLuck", reactionParams);

            gameLocationActionManager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(attacker, gameLocationActionManager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            rulesetCharacter.UpdateUsageForPower(usablePower, usablePower.PowerDefinition.CostPerUse);

            var delta = -action.AttackSuccessDelta;

            action.AttackRollOutcome = RollOutcome.Success;
            action.AttackSuccessDelta = 0;
            action.AttackRoll += delta;
            attackModifier.ignoreAdvantage = false;
            attackModifier.AttackRollModifier += delta;
            attackModifier.AttacktoHitTrends.Add(new TrendInfo(delta, FeatureSourceType.Power, power.Name, power));
        }
    }

    private sealed class BattleStartedListenerBardSuperiorInspiration(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition featureDefinition)
        : ICharacterBattleStartedListener
    {
        public void OnCharacterBattleStarted(GameLocationCharacter locationCharacter, bool surprise)
        {
            var character = locationCharacter.RulesetCharacter;

            if (character == null)
            {
                return;
            }

            if (character.RemainingBardicInspirations != 0)
            {
                return;
            }

            character.usedBardicInspiration--;
            character.BardicInspirationAltered?.Invoke(character, character.RemainingBardicInspirations);
            character.LogCharacterUsedFeature(featureDefinition);
        }
    }
}
