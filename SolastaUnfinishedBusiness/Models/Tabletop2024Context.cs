using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
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
using SolastaUnfinishedBusiness.Subclasses.Builders;
using SolastaUnfinishedBusiness.Validators;
using TA;
using UnityEngine.AddressableAssets;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDieRollModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionRestHealingModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ActionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMovementAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPointPools;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionProficiencys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttackModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ItemDefinitions;

namespace SolastaUnfinishedBusiness.Models;

internal static class Tabletop2024Context
{
    private static readonly FeatureDefinitionActionAffinity ActionAffinityPotionBonusAction =
        FeatureDefinitionActionAffinityBuilder
            .Create("ActionAffinityPotionBonusAction")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(
                new ValidateDeviceFunctionUse((_, device, _) =>
                    device.UsableDeviceDescription.UsableDeviceTags.Contains("Potion") &&
                    (device.Name.Contains("Healing") ||
                     device.Name.Contains("Remedy") ||
                     device.Name.Contains("Antitoxin"))))
            .SetAuthorizedActions(Id.UseItemBonus)
            .AddToDB();

    private static readonly ItemPropertyDescription ItemPropertyPotionBonusAction =
        new(RingFeatherFalling.StaticProperties[0])
        {
            appliesOnItemOnly = false,
            type = ItemPropertyDescription.PropertyType.Feature,
            featureDefinition = ActionAffinityPotionBonusAction,
            conditionDefinition = null,
            knowledgeAffinity = EquipmentDefinitions.KnowledgeAffinity.ActiveAndHidden
        };

    private static readonly FeatureDefinitionCombatAffinity CombatAffinityConditionSurprised =
        FeatureDefinitionCombatAffinityBuilder
            .Create("CombatAffinityConditionSurprised")
            .SetGuiPresentationNoContent(true)
            .SetInitiativeAffinity(AdvantageType.Disadvantage)
            .AddToDB();

    private static readonly FeatureDefinitionPointPool PointPoolWizardScholar = FeatureDefinitionPointPoolBuilder
        .Create("PointPoolWizardScholar")
        .SetGuiPresentation(Category.Feature)
        .SetPool(HeroDefinitions.PointsPoolType.Expertise, 1)
        .RestrictChoices(
            SkillDefinitions.Arcana,
            SkillDefinitions.History,
            SkillDefinitions.Investigation,
            SkillDefinitions.Medecine,
            SkillDefinitions.Nature,
            SkillDefinitions.Religion)
        .AddToDB();

    private static readonly FeatureDefinitionPointPool PointPoolWarlockInvocation1 = FeatureDefinitionPointPoolBuilder
        .Create(PointPoolWarlockInvocation2, "PointPoolWarlockInvocation1")
        .SetGuiPresentation("PointPoolWarlockInvocationInitial", Category.Feature)
        .SetPool(HeroDefinitions.PointsPoolType.Invocation, 1)
        .AddToDB();

    private static readonly FeatureDefinitionFeatureSet FeatureSetDruidPrimalOrder = FeatureDefinitionFeatureSetBuilder
        .Create("FeatureSetDruidPrimalOrder")
        .SetGuiPresentation(Category.Feature)
        .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
        .SetFeatureSet(
            FeatureDefinitionFeatureSetBuilder
                .Create("FeatureSetDruidPrimalOrderMagician")
                .SetGuiPresentation(Category.Feature)
                .SetFeatureSet(
                    FeatureDefinitionPointPoolBuilder
                        .Create("PointPoolDruidPrimalOrderMagician")
                        .SetGuiPresentationNoContent(true)
                        .SetSpellOrCantripPool(HeroDefinitions.PointsPoolType.Cantrip, 1, extraSpellsTag: "PrimalOrder")
                        .AddCustomSubFeatures(new ModifyAbilityCheckDruidPrimalOrder())
                        .AddToDB())
                .AddToDB(),
            FeatureDefinitionFeatureSetBuilder
                .Create("FeatureSetDruidPrimalOrderWarden")
                .SetGuiPresentation(Category.Feature)
                .SetFeatureSet(
                    FeatureDefinitionProficiencyBuilder
                        .Create("ProficiencyDruidPrimalOrderWardenArmor")
                        .SetGuiPresentationNoContent(true)
                        .SetProficiencies(ProficiencyType.Armor, EquipmentDefinitions.MediumArmorCategory)
                        .AddToDB(),
                    FeatureDefinitionProficiencyBuilder
                        .Create("ProficiencyDruidPrimalOrderWardenWeapon")
                        .SetGuiPresentationNoContent(true)
                        .SetProficiencies(ProficiencyType.Weapon, EquipmentDefinitions.MartialWeaponCategory)
                        .AddToDB())
                .AddToDB())
        .AddToDB();

    private static readonly List<string> DruidWeaponsCategories =
        [.. ProficiencyDruidWeapon.Proficiencies];

    private static readonly List<(string, string)> GuidanceProficiencyPairs =
    [
        (AttributeDefinitions.Dexterity, SkillDefinitions.Acrobatics),
        (AttributeDefinitions.Wisdom, SkillDefinitions.AnimalHandling),
        (AttributeDefinitions.Intelligence, SkillDefinitions.Arcana),
        (AttributeDefinitions.Strength, SkillDefinitions.Athletics),
        (AttributeDefinitions.Charisma, SkillDefinitions.Deception),
        (AttributeDefinitions.Intelligence, SkillDefinitions.History),
        (AttributeDefinitions.Wisdom, SkillDefinitions.Insight),
        (AttributeDefinitions.Charisma, SkillDefinitions.Intimidation),
        (AttributeDefinitions.Intelligence, SkillDefinitions.Investigation),
        (AttributeDefinitions.Wisdom, SkillDefinitions.Medecine),
        (AttributeDefinitions.Intelligence, SkillDefinitions.Nature),
        (AttributeDefinitions.Wisdom, SkillDefinitions.Perception),
        (AttributeDefinitions.Charisma, SkillDefinitions.Performance),
        (AttributeDefinitions.Charisma, SkillDefinitions.Persuasion),
        (AttributeDefinitions.Intelligence, SkillDefinitions.Religion),
        (AttributeDefinitions.Dexterity, SkillDefinitions.SleightOfHand),
        (AttributeDefinitions.Dexterity, SkillDefinitions.Stealth),
        (AttributeDefinitions.Wisdom, SkillDefinitions.Survival)
    ];

    private static readonly List<SpellDefinition> GuidanceSubSpells = [];

    private static readonly ConditionDefinition ConditionSorcererInnateSorcery = ConditionDefinitionBuilder
        .Create("ConditionSorcererInnateSorcery")
        .SetGuiPresentation(Category.Condition, ConditionAuraOfCourage)
        .SetFeatures(
            FeatureDefinitionMagicAffinityBuilder
                .Create("MagicAffinitySorcererInnateSorcery")
                .SetGuiPresentation("PowerSorcererInnateSorcery", Category.Feature)
                .SetCastingModifiers(0, SpellParamsModifierType.None, 1)
                .AddToDB())
        .AddCustomSubFeatures(new ModifyAttackActionModifierInnateSorcery())
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerSorcererInnateSorcery = FeatureDefinitionPowerBuilder
        .Create("PowerSorcererInnateSorcery")
        .SetGuiPresentation(Category.Feature, PowerTraditionShockArcanistGreaterArcaneShock)
        .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest, 1, 2)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Minute, 1)
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetEffectForms(EffectFormBuilder.ConditionForm(ConditionSorcererInnateSorcery))
                .SetCasterEffectParameters(PowerSorcererDraconicElementalResistance)
                .Build())
        .AddCustomSubFeatures(new ValidatorsValidatePowerUse(c =>
            c.GetClassLevel(Sorcerer) < 7 || c.GetRemainingPowerUses(PowerSorcererInnateSorcery) > 0))
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerSorcererSorceryIncarnate = FeatureDefinitionPowerBuilder
        .Create(PowerSorcererInnateSorcery, "PowerSorcererSorceryIncarnate")
        .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.SorceryPoints, 2, 0)
        .AddCustomSubFeatures(new ValidatorsValidatePowerUse(c =>
            c.GetClassLevel(Sorcerer) >= 7 && c.GetRemainingPowerUses(PowerSorcererInnateSorcery) == 0))
        .AddToDB();

    private static readonly FeatureDefinitionFeatureSet FeatureSetSorcererSorceryIncarnate =
        FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetSorcererSorceryIncarnate")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(PowerSorcererSorceryIncarnate)
            .AddToDB();

    private static readonly ConditionDefinition ConditionArcaneApotheosis = ConditionDefinitionBuilder
        .Create("ConditionArcaneApotheosis")
        .SetGuiPresentationNoContent(true)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .SetFixedAmount(0)
        .AddToDB();

    private static readonly FeatureDefinition FeatureSorcererArcaneApotheosis =
        FeatureDefinitionBuilder
            .Create("FeatureSorcererArcaneApotheosis")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new CustomBehaviorArcaneApotheosis())
            .AddToDB();

    private static readonly FeatureDefinitionPower FeatureDefinitionPowerNatureShroud = FeatureDefinitionPowerBuilder
        .Create("PowerRangerNatureShroud")
        .SetGuiPresentation(Category.Feature, Invisibility)
        .SetUsesAbilityBonus(ActivationTime.BonusAction, RechargeRate.LongRest, AttributeDefinitions.Wisdom)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                .SetEffectForms(EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionInvisible))
                .SetParticleEffectParameters(PowerDruidCircleBalanceBalanceOfPower)
                .Build())
        .AddToDB();

    internal static readonly ConditionDefinition ConditionIndomitableSaving = ConditionDefinitionBuilder
        .Create("ConditionIndomitableSaving")
        .SetGuiPresentationNoContent(true)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .AddCustomSubFeatures(new RollSavingThrowInitiatedIndomitableSaving())
        .SetSpecialInterruptions(ConditionInterruption.SavingThrow)
        .AddToDB();

    internal static readonly InvocationDefinition InvocationPactBlade = InvocationDefinitionBuilder
        .Create("InvocationPactBlade")
        .SetGuiPresentation(FeatureSetPactBlade.GuiPresentation)
        .SetGrantedFeature(FeatureSetPactBlade)
        .AddCustomSubFeatures(
            new CanUseAttribute(AttributeDefinitions.Charisma, PatronSoulBlade.CanWeaponBeEmpowered))
        .AddToDB();

    private static readonly InvocationDefinition InvocationPactChain = InvocationDefinitionBuilder
        .Create("InvocationPactChain")
        .SetGuiPresentation(FeatureSetPactChain.GuiPresentation)
        .SetGrantedFeature(FeatureSetPactChain)
        .AddToDB();

    private static readonly InvocationDefinition InvocationPactTome = InvocationDefinitionBuilder
        .Create("InvocationPactTome")
        // need to build a new gui presentation to be able to hide this and don't affect the set itself
        .SetGuiPresentation(FeatureSetPactTome.GuiPresentation.Title, FeatureSetPactTome.GuiPresentation.Description)
        .SetGrantedFeature(FeatureSetPactTome.FeatureSet[0]) // grant pool directly instead of feature set
        .AddToDB();

    private static readonly ConditionDefinition ConditionBardCounterCharmSavingThrowAdvantage =
        ConditionDefinitionBuilder
            .Create("ConditionBardCounterCharmSavingThrowAdvantage")
            .SetGuiPresentation(PowerBardCountercharm.GuiPresentation)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionSavingThrowAffinityBuilder
                    .Create(FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityAdvantageToAll,
                        "SavingThrowAffinityBardCounterCharmAdvantage")
                    .SetGuiPresentation(PowerBardCountercharm.GuiPresentation)
                    .AddToDB())
            .SetSpecialInterruptions(ConditionInterruption.SavingThrow)
            .AddToDB();

    private static readonly FeatureDefinitionPower PowerWarlockMagicalCunning = FeatureDefinitionPowerBuilder
        .Create("PowerWarlockMagicalCunning")
        .SetGuiPresentation(Category.Feature, PowerWizardArcaneRecovery)
        .SetUsesFixed(ActivationTime.Minute1, RechargeRate.LongRest)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetCasterEffectParameters(Banishment)
                .Build())
        .AddCustomSubFeatures(new PowerOrSpellFinishedByMeMagicalCunning())
        .AddToDB();

    private static readonly FeatureDefinition FeatureEldritchMaster = FeatureDefinitionBuilder
        .Create("FeatureEldritchMaster")
        .SetGuiPresentation(Category.Feature)
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerSorcerousRestoration = FeatureDefinitionPowerBuilder
        .Create(PowerSorcererManaPainterTap, "PowerSorcerousRestoration")
        .SetOrUpdateGuiPresentation(Category.Feature)
        .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
        .AddToDB();

    private static readonly ConditionDefinition ConditionTrueStrike2024 = ConditionDefinitionBuilder
        .Create("ConditionTrueStrike2024")
        .SetGuiPresentationNoContent(true)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .SetSpecialDuration()
        .SetFeatures(
            FeatureDefinitionAdditionalDamageBuilder
                .Create("AdditionalDamageTrueStrike")
                .SetGuiPresentationNoContent(true)
                .SetNotificationTag("TrueStrike")
                .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
                .SetDamageDice(DieType.D6, 0)
                .SetSpecificDamageType(DamageTypeRadiant)
                .SetAdvancement(
                    ExtraAdditionalDamageAdvancement.CharacterLevel,
                    DiceByRankBuilder.InterpolateDiceByRankTable(0, 20, (5, 1), (11, 2), (17, 3)))
                .SetImpactParticleReference(SacredFlame
                    .EffectDescription.EffectParticleParameters.effectParticleReference)
                .SetAttackModeOnly()
                .AddToDB())
        .SetSpecialInterruptions(ExtraConditionInterruption.AttacksWithWeaponOrUnarmed)
        .AddCustomSubFeatures(new ModifyAttackActionModifierTrueStrike())
        .AddToDB();

    private static readonly EffectForm EffectFormPowerWordStunStopped = EffectFormBuilder
        .Create()
        .SetFilterId(1)
        .SetConditionForm(
            ConditionDefinitionBuilder
                .Create(CustomConditionsContext.StopMovement, "ConditionPowerWordStunStopped")
                .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                .AddToDB(),
            ConditionForm.ConditionOperation.Add)
        .Build();

    private static readonly FeatureDefinition FeatureFighterTacticalMind = FeatureDefinitionBuilder
        .Create("FeatureFighterTacticalMind")
        .SetGuiPresentation(Category.Feature)
        .AddCustomSubFeatures(new TryAlterOutcomeAttributeCheckTacticalMind())
        .AddToDB();

    private static readonly FeatureDefinition FeatureFighterTacticalShift = FeatureDefinitionBuilder
        .Create("FeatureFighterTacticalShift")
        .SetGuiPresentation(Category.Feature)
        .AddToDB();

    private static readonly ConditionDefinition ConditionStudiedAttacks = ConditionDefinitionBuilder
        .Create("ConditionStudiedAttacks")
        .SetGuiPresentation(Category.Condition, ConditionMarkedByHunter)
        .SetConditionType(ConditionType.Detrimental)
        .AddCustomSubFeatures(new PhysicalAttackFinishedOnMeStudiedAttacks())
        .SetPossessive()
        .AddToDB();

    private static readonly FeatureDefinitionCombatAffinity CombatAffinityStudiedAttacks =
        FeatureDefinitionCombatAffinityBuilder
            .Create("CombatAffinityStudiedAttacks")
            .SetGuiPresentation("Condition/&ConditionStudiedAttacksTitle", Gui.NoLocalization)
            .SetSituationalContext(
                (SituationalContext)ExtraSituationalContext.IsConditionSource, ConditionStudiedAttacks)
            .SetAttackOnMeAdvantage(AdvantageType.Advantage)
            .AddToDB();

    private static readonly FeatureDefinition FeatureFighterStudiedAttacks = FeatureDefinitionBuilder
        .Create("FeatureFighterStudiedAttacks")
        .SetGuiPresentation(Category.Feature)
        .AddCustomSubFeatures(new PhysicalAttackFinishedByMeStudiedAttacks())
        .AddToDB();

    internal static readonly FeatureDefinition FeatureMemorizeSpell = FeatureDefinitionBuilder
        .Create("FeatureWizardMemorizeSpell")
        .SetGuiPresentation(Category.Feature)
        .AddToDB();

    private static readonly RestActivityDefinition RestActivityMemorizeSpell = RestActivityDefinitionBuilder
        .Create("RestActivityMemorizeSpell")
        .SetGuiPresentation("FeatureWizardMemorizeSpell", Category.Feature)
        .SetRestData(
            RestDefinitions.RestStage.AfterRest,
            RestType.ShortRest,
            RestActivityDefinition.ActivityCondition.CanPrepareSpells,
            nameof(FunctorMemorizeSpell),
            string.Empty)
        .AddToDB();

    private static readonly ConditionDefinition ConditionMemorizeSpell = ConditionDefinitionBuilder
        .Create("ConditionMemorizeSpell")
        .SetGuiPresentationNoContent(true)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .SetFixedAmount(0)
        .AddToDB();

    internal static void LateLoad()
    {
        BuildBarbarianBrutalStrike();
        BuildOneDndGuidanceSubspells();
        BuildRogueCunningStrike();
        LoadFighterTacticalShiftCustomBehavior();
        LoadFighterStudiedAttacks();
        LoadMonkHeightenedMetabolism();
        LoadSecondWindToUseOneDndUsagesProgression();
        LoadOneDndEnableBardCounterCharmAsReactionAtLevel7();
        LoadOneDndSpellSpareTheDying();
        LoadOneDndTrueStrike();
        LoadSorcerousRestorationAtLevel5();
        LoadWizardMemorizeSpell();
        SwitchBarbarianBrutalCritical();
        SwitchBarbarianBrutalStrike();
        SwitchBarbarianRecklessSameBuffDebuffDuration();
        SwitchBarbarianRegainOneRageAtShortRest();
        SwitchDruidPrimalOrderAndRemoveMediumArmorProficiency();
        SwitchDruidWeaponProficiencyToUseOneDnd();
        SwitchSpellRitualOnAllCasters();
        SwitchFighterLevelToIndomitableSavingReroll();
        SwitchFighterStudiedAttacks();
        SwitchFighterTacticalProgression();
        SwitchMonkBodyAndMindToReplacePerfectSelf();
        SwitchMonkDoNotRequireAttackActionForBonusUnarmoredAttack();
        SwitchMonkDoNotRequireAttackActionForFlurry();
        SwitchMonkHeightenedMetabolism();
        SwitchMonkSuperiorDefenseToReplaceEmptyBody();
        SwitchOneDndChangeBardicInspirationDurationToOneHour();
        SwitchOneDndEnableBardCounterCharmAsReactionAtLevel7();
        SwitchOneDndEnableBardExpertiseOneLevelBefore();
        SwitchOneDndEnableBardSuperiorInspirationAtLevel18();
        SwitchOneDndEnableBardWordsOfCreationAtLevel20();
        SwitchOneDnDEnableDruidUseMetalArmor();
        SwitchOneDndHealingPotionBonusAction();
        SwitchOneDndDamagingSpellsUpgrade();
        SwitchOneDndHealingSpellsUpgrade();
        SwitchOneDndMonkUnarmedDieTypeProgression();
        SwitchOneDndPaladinLayOnHandAsBonusAction();
        SwitchOneDndPaladinLearnSpellCastingAtOne();
        SwitchOneDndPreparedSpellsTables();
        SwitchOneDndRangerLearnSpellCastingAtOne();
        SwitchOneDndRemoveBardMagicalSecretAt14And18();
        SwitchOneDndRemoveBardSongOfRestAt2();
        SwitchOneDndSpellBarkskin();
        SwitchOneDndSpellDivineFavor();
        SwitchOneDndSpellLesserRestoration();
        SwitchOneDndSpellGuidance();
        SwitchOneDndSpellHideousLaughter();
        SwitchOneDndSpellHuntersMark();
        SwitchOneDndSpellMagicWeapon();
        SwitchOneDndSpellPowerWordStun();
        SwitchOneDndSpellSpareTheDying();
        SwitchOneDndSpellSpiderClimb();
        SwitchOneDndSpellStoneSkin();
        SwitchOneDndSurprisedEnforceDisadvantage();
        SwitchSorcererArcaneApotheosis();
        SwitchSorcererInnateSorcery();
        SwitchSorcerousRestorationAtLevel5();
        SwitchWarlockMagicalCunningAtLevel2AndImprovedEldritchMasterAt20();
        SwitchOneDndWarlockPatronLearningLevel();
        SwitchOneDndWarlockInvocationsProgression();
        SwitchOneDndWizardMemorizeSpell();
        SwitchOneDndWizardScholar();
        SwitchOneDndWizardSchoolOfMagicLearningLevel();
        SwitchPersuasionToFighterSkillOptions();
        SwitchRangerNatureShroud();
        SwitchRogueBlindSense();
        SwitchRogueCunningStrike();
        SwitchRogueReliableTalent();
        SwitchRogueSlipperyMind();
        SwitchRogueSteadyAim();
        SwitchSecondWindToUseOneDndUsagesProgression();
    }

    private static void LoadSecondWindToUseOneDndUsagesProgression()
    {
        PowerFighterSecondWind.AddCustomSubFeatures(
            HasModifiedUses.Marker,
            new ModifyPowerPoolAmount
            {
                PowerPool = PowerFighterSecondWind,
                Type = PowerPoolBonusCalculationType.SecondWind2024,
                Attribute = FighterClass
            });
    }

    private static void LoadFighterTacticalShiftCustomBehavior()
    {
        var powerFighterSecondWindTargeting = FeatureDefinitionPowerBuilder
            .Create(PowerFighterSecondWind, "PowerFighterSecondWindTargeting")
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.Position)
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden, new CustomBehaviorWithdraw())
            .AddToDB();

        PowerFighterSecondWind.AddCustomSubFeatures(
            new PowerOrSpellFinishedByMeSecondWind(powerFighterSecondWindTargeting));
    }

    private static void LoadFighterStudiedAttacks()
    {
        ConditionStudiedAttacks.Features.SetRange(CombatAffinityStudiedAttacks);
    }

    internal static void SwitchFighterLevelToIndomitableSavingReroll()
    {
        UseIndomitableResistance.GuiPresentation.description =
            Main.Settings.AddFighterLevelToIndomitableSavingReroll
                ? "Feature/&EnhancedIndomitableResistanceDescription"
                : "Feature/&IndomitableResistanceDescription";
    }

    internal static void SwitchFighterStudiedAttacks()
    {
        Fighter.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == FeatureFighterStudiedAttacks);

        if (Main.Settings.EnableFighterStudiedAttacks)
        {
            Fighter.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureFighterStudiedAttacks, 13));
        }

        Fighter.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchFighterTacticalProgression()
    {
        Fighter.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == FeatureFighterTacticalMind ||
            x.FeatureDefinition == FeatureFighterTacticalShift);

        if (Main.Settings.EnableFighterTacticalProgression)
        {
            Fighter.FeatureUnlocks.AddRange(
                new FeatureUnlockByLevel(FeatureFighterTacticalMind, 2),
                new FeatureUnlockByLevel(FeatureFighterTacticalShift, 5));
        }

        Fighter.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchSecondWindToUseOneDndUsagesProgression()
    {
        PowerFighterSecondWind.rechargeRate = Main.Settings.EnableSecondWindToUseOneDndUsagesProgression
            ? RechargeRate.LongRest
            : RechargeRate.ShortRest;
    }

    internal static void SwitchPersuasionToFighterSkillOptions()
    {
        if (Main.Settings.AddPersuasionToFighterSkillOptions)
        {
            PointPoolFighterSkillPoints.restrictedChoices.Add(SkillDefinitions.Persuasion);
        }
        else
        {
            PointPoolFighterSkillPoints.restrictedChoices.Remove(SkillDefinitions.Persuasion);
        }
    }

    internal static void SwitchOneDnDEnableDruidUseMetalArmor()
    {
        var active = Main.Settings.EnableDruidUseMetalArmor;

        if (active)
        {
            ProficiencyDruidArmor.ForbiddenItemTags.Clear();
        }
        else
        {
            if (!ProficiencyDruidArmor.ForbiddenItemTags.Contains(
                    TagsDefinitions.ItemTagMetal))
            {
                ProficiencyDruidArmor.ForbiddenItemTags.Add(
                    TagsDefinitions.ItemTagMetal);
            }
        }
    }

    internal static void SwitchDruidPrimalOrderAndRemoveMediumArmorProficiency()
    {
        Druid.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == FeatureSetDruidPrimalOrder);
        ProficiencyDruidArmor.Proficiencies.Remove(EquipmentDefinitions.MediumArmorCategory);

        if (Main.Settings.EnableDruidPrimalOrderAndRemoveMediumArmorProficiency)
        {
            Druid.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureSetDruidPrimalOrder, 1));
        }
        else
        {
            ProficiencyDruidArmor.Proficiencies.Add(EquipmentDefinitions.MediumArmorCategory);
        }

        Druid.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchDruidWeaponProficiencyToUseOneDnd()
    {
        ProficiencyDruidWeapon.proficiencies =
            Main.Settings.SwapDruidToUseOneDndWeaponProficiency
                ? [WeaponCategoryDefinitions.SimpleWeaponCategory.Name]
                : DruidWeaponsCategories;
    }

    internal static void SwitchSpellRitualOnAllCasters()
    {
        var subclasses = SharedSpellsContext.SubclassCasterType.Keys.Select(GetDefinition<CharacterSubclassDefinition>);

        if (Main.Settings.EnableRitualOnAllCasters)
        {
            Paladin.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureSetClericRitualCasting,
                Main.Settings.EnablePaladinSpellCastingAtLevel1 ? 1 : 2));
            Ranger.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureSetClericRitualCasting,
                Main.Settings.EnableRangerSpellCastingAtLevel1 ? 1 : 2));
            Sorcerer.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureSetClericRitualCasting, 1));
        }
        else
        {
            Paladin.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == FeatureSetClericRitualCasting);
            Ranger.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == FeatureSetClericRitualCasting);
            Sorcerer.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == FeatureSetClericRitualCasting);
        }

        Paladin.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        Ranger.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        Sorcerer.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);

        foreach (var subclass in subclasses
                     .Where(x => x.HasSubFeatureOfType<FeatureDefinitionCastSpell>()))
        {
            if (Main.Settings.EnableRitualOnAllCasters)
            {
                subclass.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureSetClericRitualCasting, 3));
            }
            else
            {
                subclass.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == FeatureSetClericRitualCasting);
            }

            subclass.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        }
    }

    internal static void SwitchOneDndSpellBarkskin()
    {
        if (Main.Settings.EnableOneDndBarkskinSpell)
        {
            Barkskin.requiresConcentration = false;
            Barkskin.castingTime = ActivationTime.BonusAction;
            AttributeModifierBarkskin.modifierValue = 17;
            Barkskin.GuiPresentation.description = "Spell/&BarkskinOneDndDescription";
            ConditionBarkskin.GuiPresentation.description = "Rules/&ConditionOneDndBarkskinDescription";
        }
        else
        {
            Barkskin.requiresConcentration = true;
            Barkskin.castingTime = ActivationTime.Action;
            AttributeModifierBarkskin.modifierValue = 16;
            Barkskin.GuiPresentation.description = "Spell/&BarkskinDescription";
            ConditionBarkskin.GuiPresentation.description = "Rules/&ConditionBarkskinDescription";
        }
    }

    private static void BuildOneDndGuidanceSubspells()
    {
        foreach (var (attribute, skill) in GuidanceProficiencyPairs)
        {
            var proficiencyPair = (attribute, skill);
            var affinity = $"AbilityCheckAffinityGuidance{skill}";
            var condition = $"ConditionGuidance{skill}";

            GuidanceSubSpells.Add(
                SpellDefinitionBuilder
                    .Create($"Guidance{skill}")
                    .SetGuiPresentation(Category.Spell, Guidance.GuiPresentation.SpriteReference)
                    .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolDivination)
                    .SetSpellLevel(0)
                    .SetCastingTime(ActivationTime.Action)
                    .SetMaterialComponent(MaterialComponentType.None)
                    .SetVerboseComponent(true)
                    .SetSomaticComponent(true)
                    .SetRequiresConcentration(true)
                    .SetVocalSpellSameType(VocalSpellSemeType.Buff)
                    .SetEffectDescription(
                        EffectDescriptionBuilder
                            .Create()
                            .SetDurationData(DurationType.Minute, 1)
                            .SetTargetingData(Side.Ally, RangeType.Touch, 0, TargetType.IndividualsUnique)
                            .SetEffectForms(EffectFormBuilder.ConditionForm(
                                ConditionDefinitionBuilder
                                    .Create(ConditionGuided, condition)
                                    .SetGuiPresentation(Category.Condition, ConditionGuided)
                                    .SetSpecialInterruptions(ConditionInterruption.None)
                                    .SetFeatures(
                                        FeatureDefinitionAbilityCheckAffinityBuilder
                                            .Create(affinity)
                                            .SetGuiPresentationNoContent(true)
                                            .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.None, DieType.D4,
                                                1, AbilityCheckGroupOperation.AddDie, proficiencyPair)
                                            .AddToDB())
                                    .AddToDB()))
                            .SetParticleEffectParameters(Guidance)
                            .Build())
                    .AddToDB());
        }
    }

    internal static void SwitchOneDndSpellDivineFavor()
    {
        DivineFavor.requiresConcentration = !Main.Settings.EnableOneDndDivineFavorSpell;
    }

    internal static void SwitchOneDndSpellLesserRestoration()
    {
        LesserRestoration.castingTime = Main.Settings.EnableOneDndLesserRestorationSpell
            ? ActivationTime.BonusAction
            : ActivationTime.Action;
    }

    private static void LoadOneDndSpellSpareTheDying()
    {
        SpareTheDying.AddCustomSubFeatures(new ModifyEffectDescriptionSpareTheDying());
    }

    internal static void SwitchOneDndSpellSpareTheDying()
    {
        SpareTheDying.GuiPresentation.description =
            Main.Settings.EnableOneDndSpareTheDyingSpell
                ? "Spell/&SpareTheDyingDescription"
                : "Spell/&SpareTheDyingExtendedDescription";
    }

    internal static void SwitchOneDndSpellSpiderClimb()
    {
        SpiderClimb.EffectDescription.EffectAdvancement.additionalTargetsPerIncrement =
            Main.Settings.EnableOneDndSpiderClimbSpell
                ? 1
                : 0;
        SpiderClimb.EffectDescription.EffectAdvancement.effectIncrementMethod =
            Main.Settings.EnableOneDndSpiderClimbSpell
                ? EffectIncrementMethod.PerAdditionalSlotLevel
                : EffectIncrementMethod.None;
    }

    internal static void SwitchOneDndSpellStoneSkin()
    {
        Stoneskin.GuiPresentation.description = "Spell/&StoneskinExtendedDescription";
        ConditionStoneskin.GuiPresentation.description = "Rules/&ConditionStoneskinExtendedDescription";
        DamageAffinityStoneskinBludgeoning.TagsIgnoringAffinity.Clear();
        DamageAffinityStoneskinPiercing.TagsIgnoringAffinity.Clear();
        DamageAffinityStoneskinSlashing.TagsIgnoringAffinity.Clear();

        if (Main.Settings.EnableOneDndStoneSkinSpell)
        {
            return;
        }

        Stoneskin.GuiPresentation.description = "Spell/&StoneskinDescription";
        ConditionStoneskin.GuiPresentation.description = "Rules/&ConditionStoneskinDescription";
        DamageAffinityStoneskinBludgeoning.TagsIgnoringAffinity.AddRange(
            TagsDefinitions.MagicalWeapon, TagsDefinitions.MagicalEffect);
        DamageAffinityStoneskinPiercing.TagsIgnoringAffinity.AddRange(
            TagsDefinitions.MagicalWeapon, TagsDefinitions.MagicalEffect);
        DamageAffinityStoneskinSlashing.TagsIgnoringAffinity.AddRange(
            TagsDefinitions.MagicalWeapon, TagsDefinitions.MagicalEffect);
    }

    internal static void SwitchOneDndSpellGuidance()
    {
        foreach (var spell in GuidanceSubSpells)
        {
            spell.implemented = false;
        }

        if (Main.Settings.EnableOneDndGuidanceSpell)
        {
            Guidance.spellsBundle = true;
            Guidance.SubspellsList.SetRange(GuidanceSubSpells);
            Guidance.compactSubspellsTooltip = true;
            Guidance.EffectDescription.EffectForms.Clear();
            Guidance.GuiPresentation.description = "Spell/&OneDndGuidanceDescription";
        }
        else
        {
            Guidance.spellsBundle = false;
            Guidance.SubspellsList.Clear();
            Guidance.EffectDescription.EffectForms.SetRange(EffectFormBuilder.ConditionForm(ConditionGuided));
            Guidance.GuiPresentation.description = "Spell/&GuidanceDescription";
        }
    }

    internal static void SwitchOneDndSpellHideousLaughter()
    {
        HideousLaughter.EffectDescription.EffectAdvancement.effectIncrementMethod =
            Main.Settings.EnableOneDndHideousLaughterSpell
                ? EffectIncrementMethod.PerAdditionalSlotLevel
                : EffectIncrementMethod.None;
    }

    internal static void SwitchOneDndSpellHuntersMark()
    {
        FeatureDefinitionAdditionalDamages.AdditionalDamageHuntersMark.specificDamageType = DamageTypeForce;
        FeatureDefinitionAdditionalDamages.AdditionalDamageHuntersMark.additionalDamageType =
            Main.Settings.EnableOneDndHuntersMarkSpell
                ? AdditionalDamageType.Specific
                : AdditionalDamageType.SameAsBaseDamage;
        HuntersMark.GuiPresentation.description =
            Main.Settings.EnableOneDndHuntersMarkSpell
                ? "Spell/&HuntersMarkExtendedDescription"
                : "Spell/&HuntersMarkDescription";
        ConditionMarkedByHunter.GuiPresentation.description =
            Main.Settings.EnableOneDndHuntersMarkSpell
                ? "Rules/&ConditionMarkedByHunterExtendedDescription"
                : "Rules/&ConditionMarkedByHunterDescription";
    }

    internal static void SwitchOneDndSpellMagicWeapon()
    {
        if (Main.Settings.EnableOneDndMagicWeaponSpell)
        {
            MagicWeapon.requiresConcentration = false;
            MagicWeapon.castingTime = ActivationTime.BonusAction;
            MagicWeapon.EffectDescription.EffectForms[0].ItemPropertyForm.FeatureBySlotLevel[1].level = 3;
        }
        else
        {
            MagicWeapon.requiresConcentration = true;
            MagicWeapon.castingTime = ActivationTime.Action;
            MagicWeapon.EffectDescription.EffectForms[0].ItemPropertyForm.FeatureBySlotLevel[1].level = 4;
        }
    }

    internal static void SwitchOneDndSpellPowerWordStun()
    {
        var effectForms = PowerWordStun.EffectDescription.EffectForms;

        if (effectForms.Count > 1)
        {
            effectForms.RemoveAt(1);
            PowerWordStun.EffectDescription.EffectFormFilters.RemoveAt(1);
        }

        PowerWordStun.GuiPresentation.description = "Spell/&PowerWordStunDescription";

        if (!Main.Settings.EnableOneDndPowerWordStunSpell)
        {
            return;
        }

        PowerWordStun.GuiPresentation.description = "Spell/&PowerWordStunExtendedDescription";
        PowerWordStun.EffectDescription.EffectFormFilters.Add(
            new EffectFormFilter { effectFormId = 1, minHitPoints = 151, maxHitPoints = 10000 });
        effectForms.Add(EffectFormPowerWordStunStopped);
    }

    internal static void SwitchOneDndWizardSchoolOfMagicLearningLevel()
    {
        var schools = DatabaseRepository.GetDatabase<CharacterSubclassDefinition>()
            .Where(x =>
                FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions.Subclasses.Contains(x.Name) ||
                x.Name.StartsWith(WizardClass))
            .ToList();

        var fromLevel = 3;
        var toLevel = 2;

        if (Main.Settings.EnableWizardToLearnSchoolAtLevel3)
        {
            fromLevel = 2;
            toLevel = 3;
        }

        foreach (var featureUnlock in schools
                     .SelectMany(school => school.FeatureUnlocks
                         .Where(featureUnlock => featureUnlock.level == fromLevel)))
        {
            featureUnlock.level = toLevel;
        }

        // change spell casting level on Wizard itself
        Wizard.FeatureUnlocks
                .FirstOrDefault(x =>
                    x.FeatureDefinition == FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions)!
                .level =
            toLevel;

        foreach (var school in schools)
        {
            school.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        }

        Wizard.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchWarlockMagicalCunningAtLevel2AndImprovedEldritchMasterAt20()
    {
        Warlock.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == PowerWarlockMagicalCunning ||
            x.FeatureDefinition == FeatureEldritchMaster ||
            x.FeatureDefinition == Level20Context.PowerWarlockEldritchMaster);

        if (Main.Settings.EnableWarlockMagicalCunningAtLevel2AndImprovedEldritchMasterAt20)
        {
            Warlock.FeatureUnlocks.AddRange(
                new FeatureUnlockByLevel(PowerWarlockMagicalCunning, 2),
                new FeatureUnlockByLevel(FeatureEldritchMaster, 20));
        }
        else
        {
            Warlock.FeatureUnlocks.Add(new FeatureUnlockByLevel(Level20Context.PowerWarlockEldritchMaster, 20));
        }

        Warlock.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchOneDndWarlockPatronLearningLevel()
    {
        var patrons = DatabaseRepository.GetDatabase<CharacterSubclassDefinition>()
            .Where(x => x.Name.StartsWith("Patron"))
            .ToList();

        var fromLevel = 3;
        var toLevel = 1;

        // handle this exception that adds features at levels 2 and 3 on sub
        var patronEldritchSurge = GetDefinition<CharacterSubclassDefinition>(PatronEldritchSurge.Name);

        patronEldritchSurge.FeatureUnlocks.RemoveAll(x =>
            x.Level <= 3 && x.FeatureDefinition == EldritchVersatilityBuilders.UnLearn1Versatility);

        if (Main.Settings.EnableWarlockToLearnPatronAtLevel3)
        {
            fromLevel = 1;
            toLevel = 3;
        }

        foreach (var featureUnlock in patrons
                     .SelectMany(patron => patron.FeatureUnlocks
                         .Where(featureUnlock => featureUnlock.level == fromLevel)))
        {
            featureUnlock.level = toLevel;
        }

        // put things back if it should not be changed
        if (!Main.Settings.EnableWarlockToLearnPatronAtLevel3)
        {
            patronEldritchSurge.FeatureUnlocks.AddRange(
                new FeatureUnlockByLevel(EldritchVersatilityBuilders.UnLearn1Versatility, 2),
                new FeatureUnlockByLevel(EldritchVersatilityBuilders.UnLearn1Versatility, 3));
        }

        // change spell casting level on Warlock itself
        Warlock.FeatureUnlocks
            .FirstOrDefault(x =>
                x.FeatureDefinition == FeatureDefinitionSubclassChoices.SubclassChoiceWarlockOtherworldlyPatrons)!
            .level = toLevel;

        foreach (var patron in patrons)
        {
            patron.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        }

        Warlock.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchOneDndPaladinLearnSpellCastingAtOne()
    {
        var level = Main.Settings.EnablePaladinSpellCastingAtLevel1 ? 1 : 2;

        foreach (var featureUnlock in Paladin.FeatureUnlocks
                     .Where(x => x.FeatureDefinition == FeatureDefinitionCastSpells.CastSpellPaladin))
        {
            featureUnlock.level = level;
        }

        // allows back and forth compatibility with EnableRitualOnAllCasters
        foreach (var featureUnlock in Paladin.FeatureUnlocks
                     .Where(x => x.FeatureDefinition == FeatureSetClericRitualCasting))
        {
            featureUnlock.level = level;
        }

        Paladin.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);

        if (Main.Settings.EnablePaladinSpellCastingAtLevel1)
        {
            FeatureDefinitionCastSpells.CastSpellPaladin.slotsPerLevels = SharedSpellsContext.HalfRoundUpCastingSlots;
            SharedSpellsContext.ClassCasterType[PaladinClass] =
                FeatureDefinitionCastSpellBuilder.CasterProgression.HalfRoundUp;
        }
        else
        {
            FeatureDefinitionCastSpells.CastSpellPaladin.slotsPerLevels = SharedSpellsContext.HalfCastingSlots;
            SharedSpellsContext.ClassCasterType[PaladinClass] =
                FeatureDefinitionCastSpellBuilder.CasterProgression.Half;
        }
    }

    internal static void SwitchOneDndRangerLearnSpellCastingAtOne()
    {
        var level = Main.Settings.EnableRangerSpellCastingAtLevel1 ? 1 : 2;

        foreach (var featureUnlock in Ranger.FeatureUnlocks
                     .Where(x => x.FeatureDefinition == FeatureDefinitionCastSpells.CastSpellRanger))
        {
            featureUnlock.level = level;
        }

        // allows back and forth compatibility with EnableRitualOnAllCasters
        foreach (var featureUnlock in Ranger.FeatureUnlocks
                     .Where(x => x.FeatureDefinition == FeatureSetClericRitualCasting))
        {
            featureUnlock.level = level;
        }

        Ranger.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);

        if (Main.Settings.EnableRangerSpellCastingAtLevel1)
        {
            FeatureDefinitionCastSpells.CastSpellRanger.slotsPerLevels = SharedSpellsContext.HalfRoundUpCastingSlots;
            SharedSpellsContext.ClassCasterType[RangerClass] =
                FeatureDefinitionCastSpellBuilder.CasterProgression.HalfRoundUp;
        }
        else
        {
            FeatureDefinitionCastSpells.CastSpellRanger.slotsPerLevels = SharedSpellsContext.HalfCastingSlots;
            SharedSpellsContext.ClassCasterType[RangerClass] =
                FeatureDefinitionCastSpellBuilder.CasterProgression.Half;
        }
    }

    internal static void SwitchOneDndSurprisedEnforceDisadvantage()
    {
        if (Main.Settings.EnableSurprisedToEnforceDisadvantage)
        {
            ConditionDefinitions.ConditionSurprised.Features.SetRange(CombatAffinityConditionSurprised);
            ConditionDefinitions.ConditionSurprised.GuiPresentation.Description = Gui.NoLocalization;
        }
        else
        {
            ConditionDefinitions.ConditionSurprised.Features.SetRange(
                ActionAffinityConditionSurprised,
                MovementAffinityConditionSurprised);
            ConditionDefinitions.ConditionSurprised.GuiPresentation.Description =
                "Rules/&ConditionSurprisedDescription";
        }
    }

    internal static void SwitchOneDndPreparedSpellsTables()
    {
        if (Main.Settings.EnableOneDnDPreparedSpellsTables)
        {
            FeatureDefinitionCastSpells.CastSpellBard.knownSpells =
                [4, 5, 6, 7, 9, 10, 11, 12, 14, 15, 16, 16, 17, 17, 18, 18, 19, 20, 21, 22];

            if (Main.Settings.EnableRangerSpellCastingAtLevel1)
            {
                FeatureDefinitionCastSpells.CastSpellRanger.knownSpells =
                    [2, 3, 4, 5, 6, 6, 7, 7, 9, 9, 10, 10, 11, 11, 12, 12, 14, 14, 15, 15];
            }
            else
            {
                FeatureDefinitionCastSpells.CastSpellRanger.knownSpells =
                    [0, 3, 4, 5, 6, 6, 7, 7, 9, 9, 10, 10, 11, 11, 12, 12, 14, 14, 15, 15];
            }

            FeatureDefinitionCastSpells.CastSpellSorcerer.knownSpells =
                [2, 4, 6, 7, 9, 10, 11, 12, 14, 15, 16, 16, 17, 17, 18, 18, 19, 20, 21, 22];
        }
        else
        {
            FeatureDefinitionCastSpells.CastSpellBard.knownSpells =
                [4, 5, 6, 7, 8, 9, 10, 11, 12, 14, 15, 15, 16, 18, 19, 19, 20, 22, 22, 22];

            if (Main.Settings.EnableRangerSpellCastingAtLevel1)
            {
                FeatureDefinitionCastSpells.CastSpellRanger.knownSpells =
                    [2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11];
            }
            else
            {
                FeatureDefinitionCastSpells.CastSpellRanger.knownSpells =
                    [0, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11];
            }

            FeatureDefinitionCastSpells.CastSpellSorcerer.knownSpells =
                [2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 12, 13, 13, 14, 14, 15, 15, 15, 15];
        }
    }

    internal static void SwitchOneDndPaladinLayOnHandAsBonusAction()
    {
        PowerPaladinLayOnHands.activationTime = Main.Settings.EnablePaladinLayOnHandsAsBonusAction
            ? ActivationTime.BonusAction
            : ActivationTime.Action;
    }

    private static void LoadOneDndEnableBardCounterCharmAsReactionAtLevel7()
    {
        PowerBardCountercharm.AddCustomSubFeatures(
            new ModifyPowerVisibility((_, _, _) => !Main.Settings.EnableBardCounterCharmAsReactionAtLevel7),
            new TryAlterOutcomeSavingThrowBardCounterCharm());
    }

    internal static void SwitchOneDndEnableBardCounterCharmAsReactionAtLevel7()
    {
        var level = Main.Settings.EnableBardCounterCharmAsReactionAtLevel7 ? 7 : 6;

        Bard.FeatureUnlocks.FirstOrDefault(x => x.FeatureDefinition == PowerBardCountercharm)!.level = level;
        if (Main.Settings.EnableBardCounterCharmAsReactionAtLevel7)
        {
            PowerBardCountercharm.GuiPresentation.description = "Feature/&PowerBardCountercharmExtendedDescription";
            PowerBardCountercharm.activationTime = ActivationTime.NoCost;
        }
        else
        {
            PowerBardCountercharm.GuiPresentation.description = "Feature/&PowerBardCountercharmDescription";
            PowerBardCountercharm.activationTime = ActivationTime.Action;
        }

        Bard.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchOneDndEnableBardExpertiseOneLevelBefore()
    {
        var level = Main.Settings.EnableBardExpertiseOneLevelBefore ? 2 : 3;

        foreach (var featureUnlock in Bard.FeatureUnlocks
                     .Where(x => x.FeatureDefinition == PointPoolBardExpertiseLevel3))
        {
            featureUnlock.level = level;
        }

        level = Main.Settings.EnableBardExpertiseOneLevelBefore ? 9 : 10;

        foreach (var featureUnlock in Bard.FeatureUnlocks
                     .Where(x => x.FeatureDefinition == PointPoolBardExpertiseLevel10))
        {
            featureUnlock.level = level;
        }

        if (Main.Settings.EnableBardExpertiseOneLevelBefore)
        {
            PointPoolBardExpertiseLevel3.GuiPresentation.description = "Feature/&BardExpertiseExtendedDescription";
            PointPoolBardExpertiseLevel10.GuiPresentation.description = "Feature/&BardExpertiseExtendedDescription";
        }
        else
        {
            PointPoolBardExpertiseLevel3.GuiPresentation.description = "Feature/&BardExpertiseDescription";
            PointPoolBardExpertiseLevel10.GuiPresentation.description = "Feature/&BardExpertiseDescription";
        }

        Bard.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchOneDndChangeBardicInspirationDurationToOneHour()
    {
        if (Main.Settings.ChangeBardicInspirationDurationToOneHour)
        {
            ConditionDefinitions.ConditionBardicInspiration.durationType = DurationType.Hour;
            ConditionDefinitions.ConditionBardicInspiration.durationParameter = 1;
        }
        else
        {
            ConditionDefinitions.ConditionBardicInspiration.durationType = DurationType.Minute;
            ConditionDefinitions.ConditionBardicInspiration.durationParameter = 10;
        }
    }

    internal static void SwitchOneDndRemoveBardSongOfRestAt2()
    {
        Bard.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == RestHealingModifierBardSongOfRest);

        if (!Main.Settings.RemoveBardSongOfRestAt2)
        {
            Bard.FeatureUnlocks.Add(new FeatureUnlockByLevel(RestHealingModifierBardSongOfRest, 2));
        }

        Bard.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchOneDndRemoveBardMagicalSecretAt14And18()
    {
        Bard.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == PointPoolBardMagicalSecrets14 ||
            x.FeatureDefinition == Level20Context.PointPoolBardMagicalSecrets18);

        if (!Main.Settings.RemoveBardMagicalSecretAt14And18)
        {
            Bard.FeatureUnlocks.AddRange(
                new FeatureUnlockByLevel(PointPoolBardMagicalSecrets14, 14),
                new FeatureUnlockByLevel(Level20Context.PointPoolBardMagicalSecrets18, 18));
        }

        Bard.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchOneDndEnableBardSuperiorInspirationAtLevel18()
    {
        Bard.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == Level20Context.FeatureBardSuperiorInspiration ||
            x.FeatureDefinition == Level20Context.FeatureBardSuperiorInspiration2024);

        Bard.FeatureUnlocks.Add(
            Main.Settings.EnableBardSuperiorInspirationAtLevel18
                ? new FeatureUnlockByLevel(Level20Context.FeatureBardSuperiorInspiration2024, 18)
                : new FeatureUnlockByLevel(Level20Context.FeatureBardSuperiorInspiration, 20));

        Bard.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchOneDndEnableBardWordsOfCreationAtLevel20()
    {
        Bard.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == Level20Context.AutoPreparedSpellsBardWordOfCreation);

        if (Main.Settings.EnableBardWordsOfCreationAtLevel20)
        {
            Bard.FeatureUnlocks.Add(
                new FeatureUnlockByLevel(Level20Context.AutoPreparedSpellsBardWordOfCreation, 20));
        }

        Bard.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchOneDndHealingPotionBonusAction()
    {
        if (Main.Settings.OneDndHealingPotionBonusAction)
        {
            foreach (var potion in DatabaseRepository.GetDatabase<ItemDefinition>()
                         .Where(a =>
                             a.UsableDeviceDescription != null &&
                             a.UsableDeviceDescription.usableDeviceTags.Contains("Potion")))
            {
                potion.StaticProperties.TryAdd(ItemPropertyPotionBonusAction);
            }
        }
        else
        {
            foreach (var potion in DatabaseRepository.GetDatabase<ItemDefinition>()
                         .Where(a =>
                             a.UsableDeviceDescription != null &&
                             a.UsableDeviceDescription.usableDeviceTags.Contains("Potion")))
            {
                potion.StaticProperties.Clear();
            }
        }
    }

    private static void LoadOneDndTrueStrike()
    {
        if (!Main.Settings.EnableOneDndTrueStrikeCantrip)
        {
            return;
        }

        TrueStrike.AddCustomSubFeatures(FixesContext.NoTwinned.Mark, AttackAfterMagicEffect.MarkerAnyWeaponAttack);
        TrueStrike.GuiPresentation.description = "Spell/&TrueStrike2024Description";
        TrueStrike.requiresConcentration = false;
        TrueStrike.effectDescription = EffectDescriptionBuilder
            .Create()
            .SetDurationData(DurationType.Round)
            // 24 seems to be the max range on Solasta ranged weapons
            .SetTargetingData(Side.Enemy, RangeType.Distance, 24, TargetType.IndividualsUnique)
            .SetEffectAdvancement(EffectIncrementMethod.CasterLevelTable, additionalDicePerIncrement: 1)
            .SetEffectForms(
                EffectFormBuilder.ConditionForm(ConditionTrueStrike2024, ConditionForm.ConditionOperation.Add, true))
            .SetParticleEffectParameters(SacredFlame)
            .SetImpactEffectParameters(new AssetReference())
            .Build();
    }

    internal static void SwitchOneDndHealingSpellsUpgrade()
    {
        var dice = Main.Settings.EnableOneDndHealingSpellsUpgrade ? 2 : 1;

        // Cure Wounds, Healing Word got buf on base damage and add dice
        CureWounds.EffectDescription.EffectForms[0].healingForm.diceNumber = dice;
        CureWounds.EffectDescription.effectAdvancement.additionalDicePerIncrement = dice;
        FalseLife.EffectDescription.EffectForms[0].temporaryHitPointsForm.diceNumber = dice;
        HealingWord.EffectDescription.EffectForms[0].healingForm.diceNumber = dice;
        HealingWord.EffectDescription.effectAdvancement.additionalDicePerIncrement = dice;

        // Mass Cure Wounds and Mass Healing Word only got buf on base damage
        MassHealingWord.EffectDescription.EffectForms[0].healingForm.diceNumber = dice;

        dice = Main.Settings.EnableOneDndHealingSpellsUpgrade ? 5 : 3;

        MassCureWounds.EffectDescription.EffectForms[0].healingForm.diceNumber = dice;

        var school = Main.Settings.EnableOneDndHealingSpellsUpgrade ? SchoolAbjuration : SchoolEvocation;
        SpellsContext.AuraOfVitality.schoolOfMagic = school;
        CureWounds.schoolOfMagic = school;
        Heal.schoolOfMagic = school;
        HealingWord.schoolOfMagic = school;
        MassCureWounds.schoolOfMagic = school;
        MassHealingWord.schoolOfMagic = school;
        PrayerOfHealing.schoolOfMagic = school;
    }

    internal static void SwitchOneDndDamagingSpellsUpgrade()
    {
        EffectProxyDefinitions.ProxyArcaneSword.AdditionalFeatures.Clear();

        if (Main.Settings.EnableOneDndDamagingSpellsUpgrade)
        {
            EffectProxyDefinitions.ProxyArcaneSword.damageDie = DieType.D12;
            EffectProxyDefinitions.ProxyArcaneSword.damageDieNum = 4;
            EffectProxyDefinitions.ProxyArcaneSword.addAbilityToDamage = true;
            EffectProxyDefinitions.ProxyArcaneSword.AdditionalFeatures.AddRange(
                FeatureDefinitionMoveModes.MoveModeFly2,
                FeatureDefinitionMoveModes.MoveModeMove6);
            CircleOfDeath.EffectDescription.EffectForms[0].DamageForm.dieType = DieType.D8;
            FlameStrike.EffectDescription.EffectForms[0].DamageForm.diceNumber = 5;
            FlameStrike.EffectDescription.EffectForms[1].DamageForm.diceNumber = 5;
            PrismaticSpray.EffectDescription.EffectForms
                .Where(x => x.FormType == EffectForm.EffectFormType.Damage)
                .Do(y => y.DamageForm.DiceNumber = 12);
            IceStorm.EffectDescription.EffectForms[0].DamageForm.dieType = DieType.D10;
            ViciousMockery.EffectDescription.EffectForms[0].DamageForm.dieType = DieType.D6;
        }
        else
        {
            EffectProxyDefinitions.ProxyArcaneSword.damageDie = DieType.D10;
            EffectProxyDefinitions.ProxyArcaneSword.damageDieNum = 3;
            EffectProxyDefinitions.ProxyArcaneSword.addAbilityToDamage = false;
            EffectProxyDefinitions.ProxyArcaneSword.AdditionalFeatures.AddRange(
                FeatureDefinitionMoveModes.MoveModeFly2,
                FeatureDefinitionMoveModes.MoveModeMove4);
            CircleOfDeath.EffectDescription.EffectForms[0].DamageForm.dieType = DieType.D6;
            FlameStrike.EffectDescription.EffectForms[0].DamageForm.diceNumber = 4;
            FlameStrike.EffectDescription.EffectForms[1].DamageForm.diceNumber = 4;
            PrismaticSpray.EffectDescription.EffectForms
                .Where(x => x.FormType == EffectForm.EffectFormType.Damage)
                .Do(y => y.DamageForm.DiceNumber = 10);
            IceStorm.EffectDescription.EffectForms[0].DamageForm.dieType = DieType.D8;
            ViciousMockery.EffectDescription.EffectForms[0].DamageForm.dieType = DieType.D4;
        }
    }

    internal static void SwitchOneDndWizardScholar()
    {
        Wizard.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == PointPoolWizardScholar);

        if (Main.Settings.EnableWizardToLearnScholarAtLevel2)
        {
            Wizard.FeatureUnlocks.Add(new FeatureUnlockByLevel(PointPoolWizardScholar, 2));
        }

        Wizard.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static bool IsRestActivityMemorizeSpellAvailable(
        RestActivityDefinition activity, RulesetCharacterHero hero)
    {
        return activity != RestActivityMemorizeSpell ||
               (Main.Settings.EnableWizardMemorizeSpell && hero.GetClassLevel(Wizard) >= 5);
    }

    internal static bool IsMemorizeSpellPreparation(RulesetCharacter rulesetCharacter)
    {
        return rulesetCharacter.HasConditionOfCategoryAndType(
            AttributeDefinitions.TagEffect, ConditionMemorizeSpell.Name);
    }

    internal static bool IsInvalidMemorizeSelectedSpell(
        SpellRepertoirePanel spellRepertoirePanel, RulesetCharacter rulesetCharacter, SpellDefinition spell)
    {
        if (!rulesetCharacter.TryGetConditionOfCategoryAndType(
                AttributeDefinitions.TagEffect, ConditionMemorizeSpell.Name, out var activeCondition))
        {
            return false;
        }

        var spellIndex = SpellsContext.Spells.IndexOf(spell);
        var isUncheck = spellRepertoirePanel.preparedSpells.Contains(spell);

        if (isUncheck)
        {
            if (activeCondition.SourceProficiencyBonus != -1 &&
                activeCondition.SourceProficiencyBonus != spellIndex)
            {
                return true;
            }

            activeCondition.Amount = 1;
            activeCondition.SourceProficiencyBonus = spellIndex;

            return false;
        }

        activeCondition.Amount = 0;
        activeCondition.SourceProficiencyBonus = spellIndex;

        return false;
    }

    private static void LoadWizardMemorizeSpell()
    {
        ServiceRepository.GetService<IFunctorService>()
            .RegisterFunctor(nameof(FunctorMemorizeSpell), new FunctorMemorizeSpell());
    }

    internal static void SwitchOneDndWizardMemorizeSpell()
    {
        Wizard.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == FeatureMemorizeSpell);

        if (Main.Settings.EnableWizardMemorizeSpell)
        {
            Wizard.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureMemorizeSpell, 5));
        }

        Wizard.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }


    internal static void SwitchSorcererArcaneApotheosis()
    {
        Sorcerer.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == FeatureSorcererArcaneApotheosis ||
            x.FeatureDefinition == Level20Context.PowerSorcerousRestoration);

        Sorcerer.FeatureUnlocks.Add(
            Main.Settings.EnableSorcererArcaneApotheosis
                ? new FeatureUnlockByLevel(FeatureSorcererArcaneApotheosis, 20)
                : new FeatureUnlockByLevel(Level20Context.PowerSorcerousRestoration, 20));

        Sorcerer.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static bool IsArcaneApotheosisValid(RulesetCharacter rulesetCharacter, RulesetEffect rulesetEffect)
    {
        var character = GameLocationCharacter.GetFromActor(rulesetCharacter);

        return IsArcaneApotheosisValid(character, rulesetEffect, false);
    }

    private static bool IsArcaneApotheosisValid(
        GameLocationCharacter character,
        RulesetEffect rulesetEffect,
        bool validateMetamagicOption = true)
    {
        if (!Main.Settings.EnableSorcererArcaneApotheosis ||
            rulesetEffect is not RulesetEffectSpell rulesetEffectSpell ||
            (validateMetamagicOption && !rulesetEffectSpell.MetamagicOption))
        {
            return false;
        }

        var rulesetCharacter = character.RulesetCharacter;
        var sorcererLevel = rulesetCharacter.GetClassLevel(Sorcerer);

        if (sorcererLevel < 20)
        {
            return false;
        }

        if (Gui.Battle != null &&
            !character.OnceInMyTurnIsValid(FeatureSorcererArcaneApotheosis.Name))
        {
            return false;
        }

        return rulesetCharacter.HasConditionOfCategoryAndType(
            AttributeDefinitions.TagEffect, ConditionSorcererInnateSorcery.Name);
    }

    internal static void SwitchSorcererInnateSorcery()
    {
        Sorcerer.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == PowerSorcererInnateSorcery ||
            x.FeatureDefinition == FeatureSetSorcererSorceryIncarnate);

        if (Main.Settings.EnableSorcererInnateSorceryAndSorceryIncarnate)
        {
            Sorcerer.FeatureUnlocks.AddRange(
                new FeatureUnlockByLevel(PowerSorcererInnateSorcery, 1),
                new FeatureUnlockByLevel(FeatureSetSorcererSorceryIncarnate, 7));
        }

        Sorcerer.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    private static void LoadSorcerousRestorationAtLevel5()
    {
        RestActivityDefinitionBuilder
            .Create("RestActivitySorcerousRestoration")
            .SetGuiPresentation(
                "Feature/&PowerSorcerousRestorationShortTitle", "Feature/&PowerSorcerousRestorationDescription")
            .SetRestData(RestDefinitions.RestStage.AfterRest, RestType.ShortRest,
                RestActivityDefinition.ActivityCondition.CanUsePower, "UsePower", PowerSorcerousRestoration.Name)
            .AddToDB();

        PowerSorcerousRestoration.EffectDescription.EffectForms[0].SpellSlotsForm.type =
            (SpellSlotsForm.EffectType)ExtraEffectType.RecoverSorceryHalfLevelDown;
    }

    internal static void SwitchSorcerousRestorationAtLevel5()
    {
        Sorcerer.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == PowerSorcerousRestoration);

        if (Main.Settings.EnableSorcererSorcerousRestoration)
        {
            Sorcerer.FeatureUnlocks.Add(new FeatureUnlockByLevel(PowerSorcerousRestoration, 5));
        }

        Sorcerer.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchRangerNatureShroud()
    {
        Ranger.FeatureUnlocks
            .RemoveAll(x => x.FeatureDefinition == FeatureDefinitionPowerNatureShroud);

        if (Main.Settings.EnableRangerNatureShroudAt14)
        {
            Ranger.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureDefinitionPowerNatureShroud, 14));
        }

        Ranger.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchOneDndWarlockInvocationsProgression()
    {
        Warlock.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == FeatureSetPactSelection);

        if (Main.Settings.EnableWarlockToUseOneDndInvocationProgression)
        {
            Warlock.FeatureUnlocks.Add(new FeatureUnlockByLevel(PointPoolWarlockInvocation1, 1));
            PointPoolWarlockInvocation2.GuiPresentation.Title =
                "Feature/&PointPoolWarlockInvocationAdditionalTitle";
            PointPoolWarlockInvocation2.GuiPresentation.Description =
                "Feature/&PointPoolWarlockInvocationAdditionalDescription";
            PointPoolWarlockInvocation5.poolAmount = 2;

            foreach (var invocation in DatabaseRepository.GetDatabase<InvocationDefinition>()
                         .Where(x =>
                             x.requiredLevel == 1 &&
                             x != InvocationDefinitions.ArmorOfShadows &&
                             x != InvocationsBuilders.EldritchMind &&
                             (InvocationsContext.Invocations.Contains(x) ||
                              x.ContentPack != CeContentPackContext.CeContentPack)))
            {
                invocation.requiredLevel = 2;
            }

            InvocationPactBlade.GuiPresentation.hidden = false;
            InvocationPactChain.GuiPresentation.hidden = false;
            InvocationPactTome.GuiPresentation.hidden = false;
        }
        else
        {
            Warlock.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == PointPoolWarlockInvocation1);
            PointPoolWarlockInvocation2.GuiPresentation.Title =
                "Feature/&PointPoolWarlockInvocationInitialTitle";
            PointPoolWarlockInvocation2.GuiPresentation.Description =
                "Feature/&PointPoolWarlockInvocationInitialDescription";
            PointPoolWarlockInvocation5.poolAmount = 1;

            foreach (var invocation in DatabaseRepository.GetDatabase<InvocationDefinition>()
                         .Where(x =>
                             x.requiredLevel == 2 &&
                             x != InvocationDefinitions.ArmorOfShadows &&
                             x != InvocationsBuilders.EldritchMind &&
                             (InvocationsContext.Invocations.Contains(x) ||
                              x.ContentPack != CeContentPackContext.CeContentPack)))
            {
                invocation.requiredLevel = 1;
            }

            InvocationPactBlade.GuiPresentation.hidden = true;
            InvocationPactChain.GuiPresentation.hidden = true;
            InvocationPactTome.GuiPresentation.hidden = true;

            Warlock.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureSetPactSelection, 3));
        }

        GuiWrapperContext.RecacheInvocations();

        Warlock.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    private class FunctorMemorizeSpell : Functor
    {
        public override IEnumerator Execute(
            FunctorParametersDescription functorParameters,
            FunctorExecutionContext context)
        {
            var inspectionScreen = Gui.GuiService.GetScreen<CharacterInspectionScreen>();
            var partyStatusScreen = Gui.GuiService.GetScreen<GamePartyStatusScreen>();
            var hero = functorParameters.RestingHero;

            Gui.GuiService.GetScreen<RestModal>().KeepCurrentState = true;

            var spellRepertoire = hero.SpellRepertoires.FirstOrDefault(x =>
                x.SpellCastingFeature.SpellReadyness == SpellReadyness.Prepared);

            if (spellRepertoire == null)
            {
                yield break;
            }

            // make this until any rest to ensure users cannot cheat by reopening the prep screen
            // as conditions on refresh won't update source amount nor source ability bonus used for tracking
            hero.InflictCondition(
                ConditionMemorizeSpell.Name,
                DurationType.UntilAnyRest,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                hero.guid,
                hero.CurrentFaction.Name,
                1,
                ConditionMemorizeSpell.Name,
                // how many spells can be prepared starting at zero as need an unselect event first
                0,
                0,
                // index to the unselected spell starting at -1 to allow any spell to be unselected on first take
                -1);

            partyStatusScreen.SetupDisplayPreferences(false, false, false);
            inspectionScreen.ShowSpellPreparation(
                functorParameters.RestingHero, Gui.GuiService.GetScreen<RestModal>(), spellRepertoire);

            while (context.Async && inspectionScreen.Visible)
            {
                yield return null;
            }

            partyStatusScreen.SetupDisplayPreferences(true, true, true);
        }
    }

    private sealed class CustomBehaviorArcaneApotheosis : IMagicEffectInitiatedByMe, IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(
            CharacterAction action,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            if (!IsArcaneApotheosisValid(attacker, action.ActionParams.RulesetEffect))
            {
                yield break;
            }

            attacker.SetSpecialFeatureUses(FeatureSorcererArcaneApotheosis.Name, 0);

            var rulesetCharacter = attacker.RulesetCharacter;

            if (!rulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, ConditionArcaneApotheosis.Name, out var activeCondition))
            {
                yield break;
            }

            var usedSorceryPoints = activeCondition.Amount;

            rulesetCharacter.usedSorceryPoints = usedSorceryPoints;
            rulesetCharacter.SorceryPointsAltered?.Invoke(rulesetCharacter, usedSorceryPoints);
        }

        public IEnumerator OnMagicEffectInitiatedByMe(
            CharacterAction action,
            RulesetEffect activeEffect,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            if (!IsArcaneApotheosisValid(attacker, action.ActionParams.RulesetEffect))
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            rulesetAttacker.InflictCondition(
                ConditionArcaneApotheosis.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.Guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                ConditionArcaneApotheosis.Name,
                rulesetAttacker.UsedSorceryPoints,
                0,
                0);
        }
    }

    private sealed class TryAlterOutcomeAttributeCheckTacticalMind : ITryAlterOutcomeAttributeCheck
    {
        public IEnumerator OnTryAlterAttributeCheck(
            GameLocationBattleManager battleManager,
            AbilityCheckData abilityCheckData,
            GameLocationCharacter defender,
            GameLocationCharacter helper)
        {
            var rulesetHelper = helper.RulesetCharacter;
            var usablePower = PowerProvider.Get(PowerFighterSecondWind, rulesetHelper);

            if (abilityCheckData.AbilityCheckRoll == 0 ||
                abilityCheckData.AbilityCheckRollOutcome != RollOutcome.Failure ||
                abilityCheckData.AbilityCheckSuccessDelta < -10 ||
                helper != defender ||
                rulesetHelper.GetRemainingUsesOfPower(usablePower) == 0)
            {
                yield break;
            }

            yield return helper.MyReactToDoNothing(
                ExtraActionId.DoNothingFree,
                defender,
                "TacticalMindCheck",
                "CustomReactionTacticalMindCheckDescription".Formatted(Category.Reaction),
                ReactionValidated,
                battleManager: battleManager);

            yield break;

            void ReactionValidated()
            {
                var dieRoll =
                    rulesetHelper.RollDie(DieType.D10, RollContext.None, false, AdvantageType.None, out _, out _);

                var abilityCheckModifier = abilityCheckData.AbilityCheckActionModifier;

                abilityCheckModifier.AbilityCheckModifierTrends.Add(
                    new TrendInfo(dieRoll, FeatureSourceType.CharacterFeature, FeatureFighterTacticalMind.Name,
                        FeatureFighterTacticalMind));

                abilityCheckModifier.AbilityCheckModifier += dieRoll;
                abilityCheckData.AbilityCheckSuccessDelta += dieRoll;

                if (abilityCheckData.AbilityCheckSuccessDelta >= 0)
                {
                    abilityCheckData.AbilityCheckRollOutcome = RollOutcome.Success;
                    usablePower.Consume();
                }

                rulesetHelper.LogCharacterActivatesAbility(
                    "Feature/&FeatureFighterTacticalMindTitle",
                    abilityCheckData.AbilityCheckSuccessDelta >= 0
                        ? "Feedback/&TacticalMindCheckToHitRollSuccess"
                        : "Feedback/&TacticalMindCheckToHitRollFailure",
                    extra: [(ConsoleStyleDuplet.ParameterType.Positive, dieRoll.ToString())]);
            }
        }
    }

    private sealed class PowerOrSpellFinishedByMeSecondWind(FeatureDefinitionPower powerDummyTargeting)
        : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            if (!Main.Settings.EnableFighterTacticalProgression)
            {
                yield break;
            }

            yield return CampaignsContext.SelectPosition(action, powerDummyTargeting);

            var attacker = action.ActingCharacter;
            var position = action.ActionParams.Positions[0];

            if (attacker.LocationPosition == position)
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var distance = (int)int3.Distance(attacker.LocationPosition, position);

            attacker.UsedTacticalMoves -= distance;

            if (attacker.UsedTacticalMoves < 0)
            {
                attacker.UsedTacticalMoves = 0;
            }

            attacker.UsedTacticalMovesChanged?.Invoke(attacker);

            rulesetAttacker.InflictCondition(
                ConditionWithdrawn.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                // all disengaging in game is set under TagCombat (why?)
                AttributeDefinitions.TagCombat,
                rulesetAttacker.Guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                ConditionWithdrawn.Name,
                distance,
                0,
                0);

            attacker.SpendActionType(ActionType.Bonus);
            attacker.MyExecuteActionTacticalMove(position);
        }
    }

    private sealed class PhysicalAttackFinishedOnMeStudiedAttacks : IPhysicalAttackFinishedOnMe
    {
        public IEnumerator OnPhysicalAttackFinishedOnMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            var rulesetDefender = defender.RulesetActor;

            if (rulesetDefender.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, ConditionStudiedAttacks.Name, out var activeCondition) &&
                activeCondition.SourceGuid == attacker.Guid &&
                rollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                rulesetDefender.RemoveCondition(activeCondition);
            }

            yield break;
        }
    }

    private sealed class PhysicalAttackFinishedByMeStudiedAttacks : IPhysicalAttackFinishedByMe
    {
        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            if (rollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetActor;

            rulesetDefender.InflictCondition(
                ConditionStudiedAttacks.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.EndOfSourceTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.Guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                ConditionStudiedAttacks.Name,
                0,
                0,
                0);
        }
    }

    private sealed class ModifyEffectDescriptionSpareTheDying : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return Main.Settings.EnableOneDndSpareTheDyingSpell && definition == SpareTheDying;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            if (!Main.Settings.EnableOneDndSpareTheDyingSpell)
            {
                return effectDescription;
            }

            effectDescription.RangeType = RangeType.Distance;

            var level = character.TryGetAttributeValue(AttributeDefinitions.CharacterLevel);
            var power = level switch
            {
                >= 17 => 3,
                >= 11 => 2,
                >= 5 => 1,
                _ => 0
            };

            effectDescription.rangeParameter = 3 * (int)Math.Pow(2, power);

            return effectDescription;
        }
    }

    private sealed class ModifyAttackActionModifierTrueStrike : IModifyAttackActionModifier
    {
        public void OnAttackComputeModifier(
            RulesetCharacter attacker,
            RulesetCharacter defender,
            BattleDefinitions.AttackProximity attackProximity,
            RulesetAttackMode attackMode,
            string effectName,
            ref ActionModifier attackModifier)
        {
            if (attackMode == null ||
                attacker.SpellsCastByMe.Count == 0)
            {
                return;
            }

            var damageForm = attackMode.EffectDescription.FindFirstDamageForm();

            if (damageForm != null)
            {
                damageForm.damageType = DamageTypeRadiant;
            }

            var oldAttribute = attackMode.AbilityScore;
            var newAttribute = attacker.SpellsCastByMe[attacker.SpellsCastByMe.Count - 1].SourceAbility;

            CanUseAttribute.ChangeAttackModeAttributeIfBetter(
                attacker, attackMode, oldAttribute, newAttribute, true);
        }
    }

    private sealed class PowerOrSpellFinishedByMeMagicalCunning : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var hero = action.ActingCharacter.RulesetCharacter.GetOriginalHero();

            if (hero == null)
            {
                yield break;
            }

            var repertoire = SharedSpellsContext.GetWarlockSpellRepertoire(hero);

            if (repertoire == null)
            {
                yield break;
            }

            hero.ClassesAndLevels.TryGetValue(Warlock, out var warlockClassLevel);

            var slotLevel = SharedSpellsContext.IsMulticaster(hero)
                ? SharedSpellsContext.PactMagicSlotsTab
                : SharedSpellsContext.GetWarlockSpellLevel(hero);
            var maxSlots = SharedSpellsContext.GetWarlockMaxSlots(hero);
            var halfSlotsRoundUp = (maxSlots + 1) / (warlockClassLevel == 20 ? 1 : 2);

            if (!repertoire.usedSpellsSlots.TryGetValue(slotLevel, out var value))
            {
                yield break;
            }

            repertoire.usedSpellsSlots[slotLevel] -= Math.Min(value, halfSlotsRoundUp);

            if (value > 0)
            {
                repertoire.RepertoireRefreshed?.Invoke(repertoire);
            }
        }
    }

    private sealed class TryAlterOutcomeSavingThrowBardCounterCharm : ITryAlterOutcomeSavingThrow
    {
        public IEnumerator OnTryAlterOutcomeSavingThrow(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            SavingThrowData savingThrowData,
            bool hasHitVisual)
        {
            if (!Main.Settings.EnableBardCounterCharmAsReactionAtLevel7)
            {
                yield break;
            }

            if (savingThrowData.SaveOutcome != RollOutcome.Success &&
                !helper.IsOppositeSide(defender.Side) &&
                helper.CanReact() &&
                helper.IsWithinRange(defender, 6) &&
                HasCharmedOrFrightened(savingThrowData.EffectDescription.EffectForms))
            {
                yield return helper.MyReactToDoNothing(
                    ExtraActionId.DoNothingFree, // cannot use DoNothingReaction here as we reroll in validate
                    defender,
                    "BardCounterCharm",
                    FormatReactionDescription(savingThrowData.Title, attacker, defender, helper),
                    ReactionValidated);
            }

            yield break;

            static bool HasCharmedOrFrightened(List<EffectForm> effectForms)
            {
                return effectForms.Any(x =>
                    x.FormType == EffectForm.EffectFormType.Condition &&
                    (x.ConditionForm.ConditionDefinition.IsSubtypeOf(ConditionDefinitions.ConditionCharmed.Name) ||
                     x.ConditionForm.ConditionDefinition.IsSubtypeOf(ConditionDefinitions.ConditionFrightened.Name)));
            }

            void ReactionValidated()
            {
                var rulesetDefender = defender.RulesetCharacter;

                rulesetDefender.InflictCondition(
                    ConditionBardCounterCharmSavingThrowAdvantage.Name,
                    DurationType.Round,
                    1,
                    TurnOccurenceType.StartOfTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetDefender.guid,
                    rulesetDefender.CurrentFaction.Name,
                    1,
                    ConditionBardCounterCharmSavingThrowAdvantage.Name,
                    0,
                    0,
                    0);

                // we need to manually spend the reaction here as rolling the saving again below
                helper.SpendActionType(ActionType.Reaction);
                helper.RulesetCharacter.LogCharacterUsedPower(PowerBardCountercharm);
                EffectHelpers.StartVisualEffect(helper, defender, PowerBardCountercharm,
                    EffectHelpers.EffectType.Caster);
                TryAlterOutcomeSavingThrow.TryRerollSavingThrow(attacker, defender, savingThrowData, hasHitVisual);
            }
        }

        private static string FormatReactionDescription(
            string sourceTitle,
            [CanBeNull] GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper)
        {
            var text = defender == helper ? "Self" : "Ally";

            return $"CustomReactionBardCounterCharmDescription{text}".Formatted(
                Category.Reaction, defender.Name, attacker?.Name ?? ReactionRequestCustom.EnvTitle, sourceTitle);
        }
    }

    private sealed class ModifyAbilityCheckDruidPrimalOrder : IModifyAbilityCheck
    {
        public void MinRoll(
            RulesetCharacter character,
            int baseBonus,
            string abilityScoreName,
            string proficiencyName,
            List<TrendInfo> advantageTrends,
            List<TrendInfo> modifierTrends,
            ref int rollModifier,
            ref int minRoll)
        {
            if (abilityScoreName is not AttributeDefinitions.Intelligence ||
                proficiencyName is not (SkillDefinitions.Arcana or SkillDefinitions.Nature))
            {
                return;
            }

            var wisdom = character.TryGetAttributeValue(AttributeDefinitions.Wisdom);
            var wisMod = AttributeDefinitions.ComputeAbilityScoreModifier(wisdom);
            var modifier = Math.Max(wisMod, 1);

            rollModifier += modifier;

            modifierTrends.Add(new TrendInfo(modifier, FeatureSourceType.CharacterFeature,
                "FeatureSetDruidPrimalOrderMagician", null));
        }
    }

    private sealed class RollSavingThrowInitiatedIndomitableSaving : IRollSavingThrowInitiated
    {
        public void OnSavingThrowInitiated(
            RulesetActor rulesetActorCaster,
            RulesetActor rulesetActorDefender,
            ref int saveBonus,
            ref string abilityScoreName,
            BaseDefinition sourceDefinition,
            List<TrendInfo> modifierTrends,
            List<TrendInfo> advantageTrends,
            ref int rollModifier,
            ref int saveDC,
            ref bool hasHitVisual,
            RollOutcome outcome,
            int outcomeDelta,
            List<EffectForm> effectForms)
        {
            if (rulesetActorDefender is not RulesetCharacterHero rulesetCharacterDefender)
            {
                return;
            }

            var classLevel = rulesetCharacterDefender.GetClassLevel(Fighter);

            rollModifier += classLevel;
            modifierTrends.Add(
                new TrendInfo(classLevel, FeatureSourceType.CharacterFeature,
                    AttributeModifierFighterIndomitable.Name,
                    AttributeModifierFighterIndomitable));
        }
    }

    private sealed class ModifyAttackActionModifierInnateSorcery : IModifyAttackActionModifier
    {
        private readonly TrendInfo _trendInfo =
            new(1, FeatureSourceType.CharacterFeature, "PowerSorcererInnateSorcery", null);

        public void OnAttackComputeModifier(
            RulesetCharacter myself,
            RulesetCharacter defender,
            BattleDefinitions.AttackProximity attackProximity,
            RulesetAttackMode attackMode,
            string effectName,
            ref ActionModifier attackModifier)
        {
            if (attackProximity is not
                (BattleDefinitions.AttackProximity.MagicRange or BattleDefinitions.AttackProximity.MagicReach))
            {
                return;
            }

            attackModifier.AttackAdvantageTrends.Add(_trendInfo);
        }
    }

    #region Monk

    private sealed class CustomBehaviorHeightenedMetabolism(
        ConditionDefinition conditionFlurryOfBlowsHeightenedMetabolism,
        ConditionDefinition conditionFlurryOfBlowsFreedomHeightenedMetabolism)
        : IModifyEffectDescription, IMagicEffectFinishedByMe
    {
        private readonly EffectForm _effectForm =
            EffectFormBuilder.ConditionForm(conditionFlurryOfBlowsHeightenedMetabolism);

        private readonly EffectForm _effectFormFreedom =
            EffectFormBuilder.ConditionForm(conditionFlurryOfBlowsFreedomHeightenedMetabolism);

        public IEnumerator OnMagicEffectFinishedByMe(
            CharacterAction action,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            var definition = action.ActionParams.activeEffect.SourceDefinition;

            if (definition != PowerMonkPatientDefense &&
                definition != PowerMonkPatientDefenseSurvival3 &&
                definition != PowerMonkPatientDefenseSurvival6)
            {
                yield break;
            }

            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;
            var dieType = rulesetCharacter.GetMonkDieType();
            var tempHp = rulesetCharacter.RollDiceAndSum(dieType, RollContext.HealValueRoll, 2, []);

            rulesetCharacter.ReceiveTemporaryHitPoints(
                tempHp, DurationType.Round, 1, TurnOccurenceType.StartOfTurn, rulesetCharacter.Guid);
        }

        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return Main.Settings.EnableMonkHeightenedMetabolism &&
                   character.GetClassLevel(Monk) >= 10 &&
                   (definition == PowerMonkFlurryOfBlows ||
                    definition == PowerTraditionFreedomFlurryOfBlowsSwiftStepsImprovement ||
                    definition == PowerTraditionFreedomFlurryOfBlowsUnendingStrikesImprovement);
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            if (definition == PowerMonkFlurryOfBlows)
            {
                effectDescription.EffectForms.TryAdd(_effectForm);
            }
            else if (definition == PowerTraditionFreedomFlurryOfBlowsSwiftStepsImprovement)
            {
                effectDescription.EffectForms.TryAdd(_effectForm);
            }
            else if (definition == PowerTraditionFreedomFlurryOfBlowsUnendingStrikesImprovement)
            {
                effectDescription.EffectForms.TryAdd(_effectFormFreedom);
            }

            return effectDescription;
        }
    }

    private sealed class CustomLevelUpLogicMonkBodyAndMind : ICustomLevelUpLogic
    {
        public void ApplyFeature([NotNull] RulesetCharacterHero hero, string tag)
        {
            hero.ModifyAttributeAndMax(AttributeDefinitions.Dexterity, 4);
            hero.ModifyAttributeAndMax(AttributeDefinitions.Wisdom, 4);
            hero.RefreshAll();
        }

        public void RemoveFeature([NotNull] RulesetCharacterHero hero, string tag)
        {
            hero.ModifyAttributeAndMax(AttributeDefinitions.Dexterity, -4);
            hero.ModifyAttributeAndMax(AttributeDefinitions.Wisdom, -4);
            hero.RefreshAll();
        }
    }

    private static readonly FeatureDefinition FeatureMonkHeightenedMetabolism = FeatureDefinitionBuilder
        .Create("FeatureMonkHeightenedMetabolism")
        .SetGuiPresentation(Category.Feature)
        .AddCustomSubFeatures(
            new CustomBehaviorHeightenedMetabolism(
                ConditionDefinitionBuilder
                    .Create("ConditionMonkFlurryOfBlowsHeightenedMetabolism")
                    .SetGuiPresentationNoContent(true)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetFeatures(
                        FeatureDefinitionAttackModifierBuilder
                            .Create(AttackModifierMonkFlurryOfBlowsUnarmedStrikeBonus,
                                "AttackModifierMonkFlurryOfBlowsUnarmedStrikeBonusHeightenedMetabolism")
                            .SetUnarmedStrike(3)
                            .AddToDB())
                    .AddToDB(),
                ConditionDefinitionBuilder
                    .Create("ConditionMonkFlurryOfBlowsFreedomHeightenedMetabolism")
                    .SetGuiPresentationNoContent(true)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetFeatures(
                        FeatureDefinitionAttackModifierBuilder
                            .Create(AttackModifierMonkFlurryOfBlowsUnarmedStrikeBonusFreedom,
                                "AttackModifierMonkFlurryOfBlowsUnarmedStrikeBonusFreedomHeightenedMetabolism")
                            .SetUnarmedStrike(4)
                            .AddToDB())
                    .AddToDB()))
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerMonkStepOfTheWindHeightenedMetabolism =
        FeatureDefinitionPowerBuilder
            .Create("PowerMonkStepOfTheWindHeightenedMetabolism")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerStepOfTheWind", Resources.PowerStepOfTheWind, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.KiPoints)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerMonkStepOfTheWindDash)
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .AddEffectForms(PowerMonkStepOftheWindDisengage.EffectDescription.EffectForms[0])
                    .SetCasterEffectParameters(PowerOathOfTirmarGoldenSpeech)
                    .Build())
            .AddToDB();

    private static readonly FeatureDefinitionPower PowerMonkSuperiorDefense = FeatureDefinitionPowerBuilder
        .Create("PowerMonkSuperiorDefense")
        .SetGuiPresentation(Category.Feature,
            Sprites.GetSprite("PowerMonkSuperiorDefense", Resources.PowerMonkSuperiorDefense, 256, 128))
        .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 3, 3)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Minute, 1)
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(
                            ConditionDefinitionBuilder
                                .Create("ConditionMonkSuperiorDefense")
                                .SetGuiPresentation(Category.Condition, ConditionAuraOfProtection)
                                .SetPossessive()
                                .AddFeatures(
                                    DamageAffinityAcidResistance,
                                    DamageAffinityBludgeoningResistanceTrue,
                                    DamageAffinityColdResistance,
                                    DamageAffinityFireResistance,
                                    DamageAffinityLightningResistance,
                                    DamageAffinityNecroticResistance,
                                    DamageAffinityPiercingResistanceTrue,
                                    DamageAffinityPoisonResistance,
                                    DamageAffinityPsychicResistance,
                                    DamageAffinityRadiantResistance,
                                    DamageAffinitySlashingResistanceTrue,
                                    DamageAffinityThunderResistance)
                                .SetConditionParticleReference(ConditionHolyAura)
                                .SetCancellingConditions(
                                    DatabaseRepository.GetDatabase<ConditionDefinition>().Where(x =>
                                        x.IsSubtypeOf(RuleDefinitions.ConditionIncapacitated)).ToArray())
                                .AddCancellingConditions(ConditionCharmedByHypnoticPattern)
                                .AddToDB(),
                            ConditionForm.ConditionOperation.Add)
                        .Build())
                .SetCasterEffectParameters(PowerOathOfTirmarGoldenSpeech)
                .Build())
        .AddToDB();

    private static readonly FeatureDefinition FeatureMonkBodyAndMind = FeatureDefinitionBuilder
        .Create("FeatureMonkBodyAndMind")
        .SetGuiPresentation(Category.Feature)
        .AddCustomSubFeatures(new CustomLevelUpLogicMonkBodyAndMind())
        .AddToDB();

    private static void LoadMonkHeightenedMetabolism()
    {
        var validatePower = new ValidatorsValidatePowerUse(c =>
            !Main.Settings.EnableMonkHeightenedMetabolism || c.GetClassLevel(Monk) < 10);

        PowerMonkStepOfTheWindDash.AddCustomSubFeatures(validatePower);
        PowerMonkStepOftheWindDisengage.AddCustomSubFeatures(validatePower);
    }

    internal static void SwitchMonkDoNotRequireAttackActionForFlurry()
    {
        FeatureSetMonkFlurryOfBlows.GuiPresentation.description =
            Main.Settings.EnableMonkDoNotRequireAttackActionForFlurry
                ? "Feature/&FeatureSetAlternateMonkFlurryOfBlowsDescription"
                : "Feature/&FeatureSetMonkFlurryOfBlowsDescription";
    }

    private static readonly List<DieTypeByRank> MonkUnarmedDieTypeByRank =
        [.. AttackModifierMonkMartialArtsImprovedDamage.DieTypeByRankTable];

    private static readonly List<DieTypeByRank> MonkUnarmedDieTypeByRank2024 =
    [
        new() { dieType = DieType.D6, rank = 1 },
        new() { dieType = DieType.D6, rank = 2 },
        new() { dieType = DieType.D6, rank = 3 },
        new() { dieType = DieType.D6, rank = 4 },
        new() { dieType = DieType.D8, rank = 5 },
        new() { dieType = DieType.D8, rank = 6 },
        new() { dieType = DieType.D8, rank = 7 },
        new() { dieType = DieType.D8, rank = 8 },
        new() { dieType = DieType.D8, rank = 9 },
        new() { dieType = DieType.D8, rank = 10 },
        new() { dieType = DieType.D10, rank = 11 },
        new() { dieType = DieType.D10, rank = 12 },
        new() { dieType = DieType.D10, rank = 13 },
        new() { dieType = DieType.D10, rank = 14 },
        new() { dieType = DieType.D10, rank = 15 },
        new() { dieType = DieType.D10, rank = 16 },
        new() { dieType = DieType.D12, rank = 17 },
        new() { dieType = DieType.D12, rank = 18 },
        new() { dieType = DieType.D12, rank = 19 },
        new() { dieType = DieType.D12, rank = 20 }
    ];

    internal static void SwitchOneDndMonkUnarmedDieTypeProgression()
    {
        AttackModifierMonkMartialArtsImprovedDamage.dieTypeByRankTable =
            Main.Settings.SwapMonkToUseOneDndUnarmedDieTypeProgression
                ? MonkUnarmedDieTypeByRank2024
                : MonkUnarmedDieTypeByRank;

        AttackModifierMonkMartialArtsImprovedDamage.GuiPresentation.Description =
            Main.Settings.SwapMonkToUseOneDndUnarmedDieTypeProgression
                ? "Feature/&AttackModifierMonkMartialArtsExtendedDescription"
                : "Feature/&AttackModifierMonkMartialArtsDescription";
    }

    internal static void SwitchMonkDoNotRequireAttackActionForBonusUnarmoredAttack()
    {
        if (Main.Settings.EnableMonkDoNotRequireAttackActionForBonusUnarmoredAttack)
        {
            PowerMonkMartialArts.GuiPresentation.description =
                "Feature/&AttackModifierMonkMartialArtsUnarmedStrikeBonusDescription";
            PowerMonkMartialArts.GuiPresentation.title =
                "Feature/&AttackModifierMonkMartialArtsUnarmedStrikeBonusTitle";
            PowerMonkMartialArts.GuiPresentation.hidden = true;
            PowerMonkMartialArts.activationTime = ActivationTime.NoCost;
        }
        else
        {
            PowerMonkMartialArts.GuiPresentation.description = "Action/&MartialArtsDescription";
            PowerMonkMartialArts.GuiPresentation.title = "Action/&MartialArtsTitle";
            PowerMonkMartialArts.GuiPresentation.hidden = false;
            PowerMonkMartialArts.activationTime = ActivationTime.OnAttackHitMartialArts;
        }

        if (Main.Settings.EnableMonkDoNotRequireAttackActionForBonusUnarmoredAttack)
        {
            Monk.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        }
    }

    internal static void SwitchMonkHeightenedMetabolism()
    {
        Monk.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == FeatureMonkHeightenedMetabolism ||
            x.FeatureDefinition == PowerMonkStepOfTheWindHeightenedMetabolism);

        if (Main.Settings.EnableMonkHeightenedMetabolism)
        {
            Monk.FeatureUnlocks.AddRange(
                new FeatureUnlockByLevel(FeatureMonkHeightenedMetabolism, 10),
                new FeatureUnlockByLevel(PowerMonkStepOfTheWindHeightenedMetabolism, 10));
        }

        Monk.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchMonkSuperiorDefenseToReplaceEmptyBody()
    {
        Monk.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == Level20Context.PowerMonkEmptyBody ||
            x.FeatureDefinition == PowerMonkSuperiorDefense);

        Monk.FeatureUnlocks.Add(
            Main.Settings.EnableMonkSuperiorDefenseToReplaceEmptyBody
                ? new FeatureUnlockByLevel(PowerMonkSuperiorDefense, 18)
                : new FeatureUnlockByLevel(Level20Context.PowerMonkEmptyBody, 18));

        Monk.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchMonkBodyAndMindToReplacePerfectSelf()
    {
        Monk.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == Level20Context.FeatureMonkPerfectSelf ||
            x.FeatureDefinition == FeatureMonkBodyAndMind);

        Monk.FeatureUnlocks.Add(
            Main.Settings.EnableMonkBodyAndMindToReplacePerfectSelf
                ? new FeatureUnlockByLevel(FeatureMonkBodyAndMind, 20)
                : new FeatureUnlockByLevel(Level20Context.FeatureMonkPerfectSelf, 20));

        Monk.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    #endregion

    #region Barbarian

    private const string BrutalStrike = "BarbarianBrutalStrike";
    private static ConditionDefinition _conditionBrutalStrike;
    private static ConditionDefinition _conditionHamstringBlow;
    private static ConditionDefinition _conditionStaggeringBlow;
    private static ConditionDefinition _conditionStaggeringBlowAoO;
    private static ConditionDefinition _conditionSunderingBlow;
    private static FeatureDefinitionFeatureSet _featureSetBarbarianBrutalStrike;
    private static FeatureDefinitionFeatureSet _featureSetBarbarianBrutalStrikeImprovement13;
    private static FeatureDefinitionFeatureSet _featureSetBarbarianBrutalStrikeImprovement17;

    private static void BuildBarbarianBrutalStrike()
    {
        const string BrutalStrikeImprovement13 = "BarbarianBrutalStrikeImprovement13";
        const string BrutalStrikeImprovement17 = "BarbarianBrutalStrikeImprovement17";

        var additionalDamageBrutalStrike =
            FeatureDefinitionAdditionalDamageBuilder
                .Create("AdditionalDamageBrutalStrike")
                .SetGuiPresentationNoContent(true)
                .SetNotificationTag("BrutalStrike")
                .SetDamageDice(DieType.D10, 1)
                .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 8, 9)
                .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
                .AddCustomSubFeatures(
                    ClassHolder.Barbarian,
                    new ValidateContextInsteadOfRestrictedProperty((_, _, character, _, _, _, _) => (OperationType.Set,
                        character.IsToggleEnabled((Id)ExtraActionId.BrutalStrikeToggle))))
                .AddToDB();

        _conditionBrutalStrike = ConditionDefinitionBuilder
            .Create($"Condition{BrutalStrike}")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(additionalDamageBrutalStrike)
            .SetSpecialInterruptions(ConditionInterruption.Attacks)
            .AddToDB();

        var powerPool = FeatureDefinitionPowerBuilder
            .Create($"Power{BrutalStrike}")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.Individuals)
                    .Build())
            .AddToDB();

        powerPool.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden, new CustomBehaviorBrutalStrike(powerPool));

        // Forceful Blow

        var powerForcefulBlow = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{BrutalStrike}ForcefulBlow")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPool)
            .SetShowCasting(false)
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        // Hamstring Blow

        var powerHamstringBlow = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{BrutalStrike}HamstringBlow")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPool)
            .SetShowCasting(false)
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        _conditionHamstringBlow = ConditionDefinitionBuilder
            .Create("ConditionHamstringBlow")
            .SetGuiPresentation($"Power{BrutalStrike}HamstringBlow", Category.Feature,
                ConditionHindered)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(
                FeatureDefinitionMovementAffinityBuilder
                    .Create("MovementAffinityHamstringBlow")
                    .SetGuiPresentation($"Power{BrutalStrike}HamstringBlow", Category.Feature, Gui.NoLocalization)
                    .SetBaseSpeedAdditiveModifier(-3)
                    .AddToDB())
            .CopyParticleReferences(ConditionSlowed)
            .AddToDB();

        _conditionHamstringBlow.GuiPresentation.description = Gui.EmptyContent;

        // Staggering Blow

        var powerStaggeringBlow = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{BrutalStrike}StaggeringBlow")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPool)
            .SetShowCasting(false)
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        _conditionStaggeringBlow = ConditionDefinitionBuilder
            .Create("ConditionStaggeringBlow")
            .SetGuiPresentation($"Power{BrutalStrike}StaggeringBlow", Category.Feature,
                ConditionDazzled)
            .SetSilent(Silent.WhenRemoved)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(
                FeatureDefinitionSavingThrowAffinityBuilder
                    .Create("SavingThrowAffinityStaggeringBlow")
                    .SetGuiPresentation($"Power{BrutalStrike}StaggeringBlow", Category.Feature, Gui.NoLocalization)
                    .SetAffinities(CharacterSavingThrowAffinity.Disadvantage, false,
                        AttributeDefinitions.AbilityScoreNames)
                    .AddToDB())
            .AddSpecialInterruptions(ConditionInterruption.SavingThrow)
            .CopyParticleReferences(ConditionDazzled)
            .AddToDB();

        _conditionStaggeringBlow.GuiPresentation.description = Gui.EmptyContent;

        _conditionStaggeringBlowAoO = ConditionDefinitionBuilder
            .Create("ConditionStaggeringBlowAoO")
            .SetGuiPresentation(Category.Condition, ConditionDazzled)
            .SetSilent(Silent.WhenAdded)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(Tabletop2014Context.ActionAffinityConditionBlind)
            .AddToDB();

        // Sundering Blow

        var additionalDamageSunderingBlow = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{BrutalStrike}SunderingBlow")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("SunderingBlow")
            .SetDamageDice(DieType.D10, 1)
            .AddToDB();

        var conditionSunderingBlowAlly = ConditionDefinitionBuilder
            .Create("ConditionSunderingBlowAlly")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(additionalDamageSunderingBlow)
            .SetSpecialInterruptions(ConditionInterruption.Attacks)
            .AddToDB();

        var powerSunderingBlow = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{BrutalStrike}SunderingBlow")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPool)
            .SetShowCasting(false)
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        _conditionSunderingBlow = ConditionDefinitionBuilder
            .Create("ConditionSunderingBlow")
            .SetGuiPresentation($"Power{BrutalStrike}SunderingBlow", Category.Feature,
                ConditionTargetedByGuidingBolt)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .AddCustomSubFeatures(new CustomBehaviorSunderingBlow(powerSunderingBlow, conditionSunderingBlowAlly))
            .SetSpecialInterruptions(ExtraConditionInterruption.AfterWasAttackedNotBySource)
            .CopyParticleReferences(ConditionLeadByExampleMarked)
            .AddToDB();

        // MAIN

        PowerBundle.RegisterPowerBundle(powerPool, true,
            powerForcefulBlow, powerHamstringBlow, powerStaggeringBlow, powerSunderingBlow);

        var actionAffinityToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityBrutalStrikeToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((Id)ExtraActionId.BrutalStrikeToggle)
            .AddToDB();

        _featureSetBarbarianBrutalStrike = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{BrutalStrike}")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerPool, actionAffinityToggle, powerForcefulBlow, powerHamstringBlow)
            .AddToDB();

        _featureSetBarbarianBrutalStrikeImprovement13 = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{BrutalStrikeImprovement13}")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerStaggeringBlow, powerSunderingBlow)
            .AddToDB();

        _featureSetBarbarianBrutalStrikeImprovement17 = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{BrutalStrikeImprovement17}")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();
    }

    private sealed class CustomBehaviorBrutalStrike(FeatureDefinitionPower powerBarbarianBrutalStrike)
        : IPhysicalAttackBeforeHitConfirmedOnEnemy, IPhysicalAttackFinishedByMe
    {
        private static readonly EffectForm ForcefulBlowForm = EffectFormBuilder
            .Create()
            .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 3)
            .Build();

        public IEnumerator OnPhysicalAttackBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (!attacker.OnceInMyTurnIsValid(BrutalStrike) ||
                !rulesetAttacker.IsToggleEnabled((Id)ExtraActionId.BrutalStrikeToggle) ||
                !rulesetAttacker.HasConditionOfCategoryAndType(
                    AttributeDefinitions.TagCombat, ConditionDefinitions.ConditionReckless.Name))
            {
                yield break;
            }

            rulesetAttacker.InflictCondition(
                _conditionBrutalStrike.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                _conditionBrutalStrike.Name,
                0,
                0,
                0);

            var usablePower = PowerProvider.Get(powerBarbarianBrutalStrike, rulesetAttacker);

            yield return attacker.MyReactToSpendPowerBundle(
                usablePower,
                [defender],
                attacker,
                powerBarbarianBrutalStrike.Name,
                reactionValidated: ReactionValidated,
                battleManager: battleManager);

            yield break;

            void ReactionValidated(ReactionRequestSpendBundlePower reactionRequest)
            {
                // determine selected power to collect cost
                var option = reactionRequest.SelectedSubOption;
                var subPowers = powerBarbarianBrutalStrike.GetBundle()?.SubPowers;

                if (subPowers == null)
                {
                    return;
                }

                var selectedPower = subPowers[option];

                switch (selectedPower.Name)
                {
                    case $"Power{BrutalStrike}ForcefulBlow":
                        actualEffectForms.Add(ForcefulBlowForm);
                        break;
                    case $"Power{BrutalStrike}HamstringBlow":
                        InflictCondition(rulesetAttacker, defender.RulesetCharacter, _conditionHamstringBlow.Name);
                        break;
                    case $"Power{BrutalStrike}StaggeringBlow":
                        InflictCondition(rulesetAttacker, defender.RulesetCharacter, _conditionStaggeringBlow.Name);
                        InflictCondition(rulesetAttacker, defender.RulesetCharacter, _conditionStaggeringBlowAoO.Name);
                        break;
                    case $"Power{BrutalStrike}SunderingBlow":
                        InflictCondition(rulesetAttacker, defender.RulesetCharacter, _conditionSunderingBlow.Name);
                        break;
                }
            }
        }

        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (!rulesetAttacker.IsToggleEnabled((Id)ExtraActionId.BrutalStrikeToggle) ||
                !rulesetAttacker.HasConditionOfCategoryAndType(
                    AttributeDefinitions.TagCombat, ConditionDefinitions.ConditionReckless.Name))
            {
                yield break;
            }

            attacker.SetSpecialFeatureUses(BrutalStrike, 0);
        }

        private static void InflictCondition(
            RulesetCharacter rulesetAttacker, RulesetCharacter rulesetDefender, string conditionName)
        {
            rulesetDefender.InflictCondition(
                conditionName,
                DurationType.Round,
                1,
                (TurnOccurenceType)ExtraTurnOccurenceType.StartOfSourceTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionName,
                0,
                0,
                0);
        }
    }

    private sealed class CustomBehaviorSunderingBlow(
        FeatureDefinitionPower powerSunderingBlow,
        ConditionDefinition conditionSunderingBlowAlly) : IPhysicalAttackInitiatedOnMe, IMagicEffectAttackInitiatedOnMe
    {
        public IEnumerator OnMagicEffectAttackInitiatedOnMe(
            CharacterActionMagicEffect action,
            RulesetEffect activeEffect,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            bool firstTarget,
            bool checkMagicalAttackDamage)
        {
            var damageType = activeEffect.EffectDescription.FindFirstDamageForm()?.DamageType;
            var rulesetAttacker = attacker.RulesetCharacter;

            if (damageType == null ||
                rulesetAttacker == null ||
                rulesetAttacker is RulesetCharacterEffectProxy)
            {
                yield break;
            }

            AddBonusAttackAndDamageRoll(attacker.RulesetCharacter, defender.RulesetActor, attackModifier);
        }

        public IEnumerator OnPhysicalAttackInitiatedOnMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode)
        {
            var damageType = attackMode.EffectDescription.FindFirstDamageForm()?.DamageType;
            var rulesetAttacker = attacker.RulesetCharacter;

            if (damageType == null ||
                rulesetAttacker == null ||
                rulesetAttacker is RulesetCharacterEffectProxy)
            {
                yield break;
            }

            AddBonusAttackAndDamageRoll(attacker.RulesetCharacter, defender.RulesetActor, attackModifier);
        }

        private void AddBonusAttackAndDamageRoll(
            RulesetCharacter rulesetAttacker,
            RulesetActor rulesetDefender,
            ActionModifier actionModifier)
        {
            if (!rulesetDefender.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, _conditionSunderingBlow.Name, out var activeCondition))
            {
                return;
            }

            var rulesetSource = EffectHelpers.GetCharacterByGuid(activeCondition.SourceGuid);

            if (rulesetAttacker == rulesetSource)
            {
                return;
            }

            rulesetDefender.RemoveCondition(activeCondition);

            var bonusAttackRoll =
                rulesetAttacker.RollDie(DieType.D10, RollContext.None, false, AdvantageType.None, out _, out _);

            actionModifier.AttackRollModifier += bonusAttackRoll;
            actionModifier.AttacktoHitTrends.Add(new TrendInfo(
                bonusAttackRoll, FeatureSourceType.CharacterFeature, powerSunderingBlow.Name, powerSunderingBlow));

            rulesetAttacker.InflictCondition(
                conditionSunderingBlowAlly.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                FactionDefinitions.Party.Name,
                1,
                conditionSunderingBlowAlly.Name,
                0,
                0,
                0);
        }
    }

    internal static void SwitchBarbarianBrutalStrike()
    {
        Barbarian.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == _featureSetBarbarianBrutalStrike ||
            x.FeatureDefinition == _featureSetBarbarianBrutalStrikeImprovement13 ||
            x.FeatureDefinition == _featureSetBarbarianBrutalStrikeImprovement17);

        if (Main.Settings.EnableBarbarianBrutalStrike)
        {
            Barbarian.FeatureUnlocks.AddRange(
                new FeatureUnlockByLevel(_featureSetBarbarianBrutalStrike, 9),
                new FeatureUnlockByLevel(_featureSetBarbarianBrutalStrikeImprovement13, 13),
                new FeatureUnlockByLevel(_featureSetBarbarianBrutalStrikeImprovement17, 17));
        }

        Barbarian.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchBarbarianBrutalCritical()
    {
        Barbarian.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == FeatureSetBarbarianBrutalCritical ||
            x.FeatureDefinition == AttributeModifierBarbarianBrutalCriticalAdd);

        if (!Main.Settings.DisableBarbarianBrutalCritical)
        {
            Barbarian.FeatureUnlocks.AddRange(
                new FeatureUnlockByLevel(FeatureSetBarbarianBrutalCritical, 9),
                new FeatureUnlockByLevel(AttributeModifierBarbarianBrutalCriticalAdd, 13),
                new FeatureUnlockByLevel(AttributeModifierBarbarianBrutalCriticalAdd, 17));
        }

        Barbarian.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchBarbarianRecklessSameBuffDebuffDuration()
    {
        RecklessAttack.GuiPresentation.description = Main.Settings.EnableBarbarianRecklessSameBuffDebuffDuration
            ? "Action/&RecklessAttackExtendedDescription"
            : "Action/&RecklessAttackDescription";
    }

    internal static void SwitchBarbarianRegainOneRageAtShortRest()
    {
        FeatureSetBarbarianRage.GuiPresentation.description = Main.Settings.EnableBarbarianRegainOneRageAtShortRest
            ? "Feature/&FeatureSetRageExtendedDescription"
            : "Feature/&FeatureSetRageDescription";
    }

    #endregion

    #region Rogue

    private const string FeatSteadyAim = "FeatSteadyAim";

    private static readonly FeatureDefinitionPower PowerFeatSteadyAim = FeatureDefinitionPowerBuilder
        .Create($"Power{FeatSteadyAim}")
        .SetGuiPresentation(Category.Feature, Sprites.GetSprite(FeatSteadyAim, Resources.PowerSteadyAim, 256, 128))
        .SetUsesFixed(ActivationTime.BonusAction)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Round)
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(
                            ConditionDefinitionBuilder
                                .Create($"Condition{FeatSteadyAim}Advantage")
                                .SetGuiPresentation(Category.Condition,
                                    ConditionGuided)
                                .SetPossessive()
                                .SetSilent(Silent.WhenAddedOrRemoved)
                                .SetSpecialInterruptions(ConditionInterruption.Attacks)
                                .AddFeatures(
                                    FeatureDefinitionCombatAffinityBuilder
                                        .Create($"CombatAffinity{FeatSteadyAim}")
                                        .SetGuiPresentation($"Power{FeatSteadyAim}", Category.Feature,
                                            Gui.NoLocalization)
                                        .SetMyAttackAdvantage(AdvantageType.Advantage)
                                        .AddToDB())
                                .AddToDB(),
                            ConditionForm.ConditionOperation.Add)
                        .Build(),
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(
                            ConditionDefinitionBuilder
                                .Create($"Condition{FeatSteadyAim}Restrained")
                                .SetGuiPresentation(Category.Condition)
                                .SetSilent(Silent.WhenAddedOrRemoved)
                                .AddFeatures(MovementAffinityConditionRestrained)
                                .AddToDB(),
                            ConditionForm.ConditionOperation.Add)
                        .Build())
                .SetParticleEffectParameters(PowerFunctionWandFearCommand)
                .SetImpactEffectParameters(new AssetReference())
                .Build())
        .AddCustomSubFeatures(
            new ValidatorsValidatePowerUse(c => GameLocationCharacter.GetFromActor(c) is { UsedTacticalMoves: 0 }))
        .AddToDB();

    internal static readonly ConditionDefinition ConditionReduceSneakDice = ConditionDefinitionBuilder
        .Create("ConditionReduceSneakDice")
        .SetGuiPresentationNoContent(true)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .SetConditionType(ConditionType.Detrimental)
        .SetAmountOrigin(ConditionDefinition.OriginOfAmount.Fixed)
        .AllowMultipleInstances()
        .SetSpecialInterruptions(ConditionInterruption.Attacks)
        .AddToDB();

    private static FeatureDefinitionFeatureSet _featureSetRogueCunningStrike;
    private static FeatureDefinition _featureRogueImprovedCunningStrike;
    private static FeatureDefinitionFeatureSet _featureSetRogueDeviousStrike;

    private static void BuildRogueCunningStrike()
    {
        const string Cunning = "RogueCunningStrike";
        const string Devious = "RogueDeviousStrike";

        var powerPool = FeatureDefinitionPowerBuilder
            .Create($"Power{Cunning}")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.Individuals)
                    .Build())
            .AddToDB();

        // Disarm

        var combatAffinityDisarmed = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Cunning}Disarmed")
            .SetGuiPresentation($"Condition{Cunning}Disarmed", Category.Condition, Gui.NoLocalization)
            .SetMyAttackAdvantage(AdvantageType.Disadvantage)
            .AddToDB();

        var conditionDisarmed = ConditionDefinitionBuilder
            .Create($"Condition{Cunning}Disarmed")
            .SetGuiPresentation(Category.Condition, Gui.EmptyContent, ConditionBaned)
            .SetConditionType(ConditionType.Detrimental)
            .AddFeatures(combatAffinityDisarmed)
            .AddToDB();

        var powerDisarm = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Cunning}Disarm")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPool)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Dexterity, 8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(conditionDisarmed, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden, PowerUsesSneakDiceTooltipModifier.Instance)
            .AddToDB();

        // Poison

        var powerPoison = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Cunning}Poison")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPool)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Dexterity, 8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                            .SetConditionForm(
                                ConditionDefinitions.ConditionPoisoned, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden, PowerUsesSneakDiceTooltipModifier.Instance)
            .AddToDB();

        // Trip

        var powerTrip = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Cunning}Trip")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPool)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Dexterity, 8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetMotionForm(MotionForm.MotionType.FallProne)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden, PowerUsesSneakDiceTooltipModifier.Instance)
            .AddToDB();

        // Withdraw

        var powerWithdraw = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Cunning}Withdraw")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPool)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.Position)
                    .Build())
            .AddCustomSubFeatures(
                ModifyPowerVisibility.Hidden,
                PowerUsesSneakDiceTooltipModifier.Instance,
                new CustomBehaviorWithdraw())
            .AddToDB();

        //
        // DEVIOUS STRIKES - LEVEL 14
        //

        // Dazed

        var actionAffinityDazedOnlyMovement = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Devious}DazedOnlyMovement")
            .SetGuiPresentationNoContent(true)
            .SetAllowedActionTypes(false, false, freeOnce: false, reaction: false, noCost: false)
            .AddToDB();

        var conditionDazedOnlyMovement = ConditionDefinitionBuilder
            .Create($"Condition{Devious}DazedOnlyMovement")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetConditionType(ConditionType.Detrimental)
            .AddFeatures(actionAffinityDazedOnlyMovement)
            .AddToDB();

        var actionAffinityDazed = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Devious}Dazed")
            .SetGuiPresentationNoContent(true)
            .SetAllowedActionTypes(reaction: false, bonus: false)
            .AddToDB();

        var conditionDazed = ConditionDefinitionBuilder
            .Create(ConditionDazzled, $"Condition{Devious}Dazed")
            .SetGuiPresentation(Category.Condition, ConditionDazzled)
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(actionAffinityDazed)
            .AddCustomSubFeatures(new ActionFinishedByMeDazed(conditionDazedOnlyMovement))
            .AddToDB();

        var powerDaze = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Devious}Daze")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPool, 2)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Dexterity, 8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(conditionDazed, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden, PowerUsesSneakDiceTooltipModifier.Instance)
            .AddToDB();

        // Knock Out

        var conditionKnockOut = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionIncapacitated, $"Condition{Devious}KnockOut")
            .SetGuiPresentation(Category.Condition, Gui.EmptyContent, ConditionDefinitions.ConditionAsleep)
            .SetParentCondition(ConditionDefinitions.ConditionIncapacitated)
            .SetFeatures()
            .SetSpecialInterruptions(ConditionInterruption.Damaged)
            .AddToDB();

        var powerKnockOut = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Devious}KnockOut")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPool, 5)
            .SetShowCasting(false)
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden, PowerUsesSneakDiceTooltipModifier.Instance)
            .AddToDB();

        var powerKnockOutApply = FeatureDefinitionPowerBuilder
            .Create($"Power{Devious}KnockOutApply")
            .SetGuiPresentation($"Power{Devious}KnockOut", Category.Feature, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Dexterity, 8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                            .SetConditionForm(conditionKnockOut, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        // Obscure

        var powerObscure = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Devious}Obscure")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPool, 3)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Dexterity, 8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(ConditionDefinitions.ConditionBlinded,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden, PowerUsesSneakDiceTooltipModifier.Instance)
            .AddToDB();

        // MAIN

        powerPool.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new CustomBehaviorCunningStrike(powerPool, powerKnockOut, powerKnockOutApply, powerWithdraw));

        PowerBundle.RegisterPowerBundle(powerPool, true,
            powerDisarm, powerPoison, powerTrip, powerWithdraw, powerDaze, powerKnockOut, powerObscure);

        var actionAffinityToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityCunningStrikeToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((Id)ExtraActionId.CunningStrikeToggle)
            .AddToDB();

        _featureSetRogueCunningStrike = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Cunning}")
            .SetGuiPresentation($"Power{Cunning}", Category.Feature)
            .SetFeatureSet(powerPool, actionAffinityToggle, powerDisarm, powerPoison, powerTrip, powerWithdraw)
            .AddToDB();

        _featureRogueImprovedCunningStrike = FeatureDefinitionBuilder
            .Create($"FeatureImproved{Cunning}")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        _featureSetRogueDeviousStrike = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Devious}")
            .SetGuiPresentation($"Power{Devious}", Category.Feature)
            .SetFeatureSet(powerDaze, powerKnockOut, powerObscure)
            .AddToDB();
    }

    internal static void SwitchRogueReliableTalent()
    {
        Rogue.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == DieRollModifierRogueReliableTalent);

        Rogue.FeatureUnlocks.Add(Main.Settings.EnableRogueReliableTalentAt7
            ? new FeatureUnlockByLevel(DieRollModifierRogueReliableTalent, 7)
            : new FeatureUnlockByLevel(DieRollModifierRogueReliableTalent, 11));

        Rogue.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchRogueSlipperyMind()
    {
        ProficiencyRogueSlipperyMind.Proficiencies.Remove(AttributeDefinitions.Charisma);

        if (Main.Settings.EnableRogueSlipperyMind)
        {
            ProficiencyRogueSlipperyMind.Proficiencies.Add(AttributeDefinitions.Charisma);
            ProficiencyRogueSlipperyMind.GuiPresentation.description = "Feature/&RogueSlipperyMindExtendedDescription";
        }
        else
        {
            ProficiencyRogueSlipperyMind.GuiPresentation.description = "Feature/&RogueSlipperyMindDescription";
        }
    }

    internal static void SwitchRogueSteadyAim()
    {
        Rogue.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == PowerFeatSteadyAim);

        if (Main.Settings.EnableRogueSteadyAim)
        {
            Rogue.FeatureUnlocks.Add(new FeatureUnlockByLevel(PowerFeatSteadyAim, 3));
        }

        Rogue.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchRogueBlindSense()
    {
        Rogue.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == FeatureDefinitionSenses.SenseRogueBlindsense);

        if (!Main.Settings.RemoveRogueBlindSense)
        {
            Rogue.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureDefinitionSenses.SenseRogueBlindsense, 14));
        }

        Rogue.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static bool IsSneakAttackValid(
        ActionModifier attackModifier,
        GameLocationCharacter attacker,
        GameLocationCharacter defender)
    {
        // only trigger if it hasn't used sneak attack yet
        if (!attacker.OncePerTurnIsValid("AdditionalDamageRogueSneakAttack") ||
            !attacker.OncePerTurnIsValid("AdditionalDamageRoguishHoodlumNonFinesseSneakAttack") ||
            !attacker.OncePerTurnIsValid("AdditionalDamageRoguishDuelistDaringDuel") ||
            !attacker.OncePerTurnIsValid("AdditionalDamageRoguishUmbralStalkerDeadlyShadows"))
        {
            return false;
        }

        var advantageType = ComputeAdvantage(attackModifier.AttackAdvantageTrends);

        return advantageType switch
        {
            AdvantageType.Advantage => true,
            AdvantageType.Disadvantage => false,
            _ =>
                // it's an attack with a nearby enemy (standard sneak attack)
                ServiceRepository.GetService<IGameLocationBattleService>()
                    .IsConsciousCharacterOfSideNextToCharacter(defender, attacker.Side, attacker) ||
                // it's a Duelist and target is dueling with him
                RoguishDuelist.TargetIsDuelingWithRoguishDuelist(attacker, defender, advantageType) ||
                // it's an Umbral Stalker and source or target are in dim light or darkness
                RoguishUmbralStalker.SourceOrTargetAreNotBright(attacker, defender, advantageType)
        };
    }

    private static readonly ConditionDefinition ConditionWithdrawn = ConditionDefinitionBuilder
        .Create(ConditionDefinitions.ConditionDisengaging, "ConditionWithdrawn")
        .SetSilent(Silent.None)
        .SetParentCondition(ConditionDefinitions.ConditionDisengaging)
        .SetFeatures()
        .SetFixedAmount(3)
        .AddCustomSubFeatures(new ActionFinishedByWithdraw())
        .AddToDB();

    private sealed class CustomBehaviorCunningStrike(
        FeatureDefinitionPower powerRogueCunningStrike,
        FeatureDefinitionPower powerKnockOut,
        FeatureDefinitionPower powerKnockOutApply,
        FeatureDefinitionPower powerWithdraw)
        : IPhysicalAttackBeforeHitConfirmedOnEnemy, IPhysicalAttackFinishedByMe
    {
        private readonly List<FeatureDefinitionPower> _selectedPowers = [];

        public IEnumerator OnPhysicalAttackBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            _selectedPowers.Clear();

            var rulesetAttacker = attacker.RulesetCharacter;

            if (!rulesetAttacker.IsToggleEnabled((Id)ExtraActionId.CunningStrikeToggle) ||
                !IsSneakAttackValid(actionModifier, attacker, defender))
            {
                yield break;
            }

            var aborted = false;
            var attempts = rulesetAttacker.GetClassLevel(Rogue) >= 11 ? 2 : 1;
            var usablePower = PowerProvider.Get(powerRogueCunningStrike, rulesetAttacker);
            RulesetUsablePower savedUsablePower = null;

            for (var i = 0; i < attempts; i++)
            {
                yield return attacker.MyReactToSpendPowerBundle(
                    usablePower,
                    [defender],
                    attacker,
                    powerRogueCunningStrike.Name,
                    reactionValidated: ReactionValidated,
                    reactionNotValidated: ReactionNotValidated,
                    battleManager: battleManager);

                if (aborted)
                {
                    break;
                }

                if (_selectedPowers.Count < 1)
                {
                    continue;
                }

                // don't offer 1st selected effect again
                savedUsablePower = PowerProvider.Get(_selectedPowers[0], rulesetAttacker);
                rulesetAttacker.UsablePowers.Remove(PowerProvider.Get(_selectedPowers[0], rulesetAttacker));
            }

            // recover first selected usable power
            if (savedUsablePower != null)
            {
                rulesetAttacker.UsablePowers.Add(savedUsablePower);
            }

            yield break;

            void ReactionValidated(ReactionRequestSpendBundlePower reactionRequest)
            {
                // determine selected power to collect cost
                var option = reactionRequest.SelectedSubOption;
                var subPowers = powerRogueCunningStrike.GetBundle()?.SubPowers;

                if (subPowers == null)
                {
                    return;
                }

                var selectedPower = subPowers[option];

                _selectedPowers.Add(selectedPower);

                // inflict condition passing power cost on amount to be deducted later on from sneak dice
                rulesetAttacker.InflictCondition(
                    ConditionReduceSneakDice.Name,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.EndOfTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetAttacker.guid,
                    rulesetAttacker.CurrentFaction.Name,
                    1,
                    ConditionReduceSneakDice.Name,
                    selectedPower.CostPerUse,
                    0,
                    0);
            }

            void ReactionNotValidated(ReactionRequestSpendBundlePower reactionRequest)
            {
                aborted = true;
            }
        }

        // handle Knock Out exception which should apply condition after attack
        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            foreach (var selectedPower in _selectedPowers)
            {
                if (selectedPower == powerKnockOut)
                {
                    yield return HandleKnockOut(attacker, defender);
                }
                else if (selectedPower == powerWithdraw)
                {
                    yield return HandleWithdraw(action, attacker);
                }
            }

            _selectedPowers.Clear();
        }

        private IEnumerator HandleWithdraw(CharacterAction action, GameLocationCharacter attacker)
        {
            yield return CampaignsContext.SelectPosition(action, powerWithdraw);

            var rulesetAttacker = attacker.RulesetCharacter;
            var position = action.ActionParams.Positions[0];
            var distance = (int)int3.Distance(attacker.LocationPosition, position);

            attacker.UsedTacticalMoves -= distance;

            if (attacker.UsedTacticalMoves < 0)
            {
                attacker.UsedTacticalMoves = 0;
            }

            attacker.UsedTacticalMovesChanged?.Invoke(attacker);

            rulesetAttacker.InflictCondition(
                ConditionWithdrawn.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                // all disengaging in game is set under TagCombat (why?)
                AttributeDefinitions.TagCombat,
                rulesetAttacker.Guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                ConditionWithdrawn.Name,
                distance,
                0,
                0);

            attacker.SpendActionType(ActionType.Main);
            attacker.MyExecuteActionTacticalMove(position);
        }

        private IEnumerator HandleKnockOut(GameLocationCharacter attacker, GameLocationCharacter defender)
        {
            var rulesetDefender = defender.RulesetActor;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerKnockOutApply, rulesetAttacker);

            attacker.MyExecuteActionSpendPower(usablePower, defender);
        }
    }

    private sealed class ActionFinishedByMeDazed(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionDazedOnlyMovement) : IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction characterAction)
        {
            if (characterAction is not (CharacterActionMove or CharacterActionDash))
            {
                yield break;
            }

            var rulesetCharacter = characterAction.ActingCharacter.RulesetCharacter;

            rulesetCharacter.InflictCondition(
                conditionDazedOnlyMovement.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                conditionDazedOnlyMovement.Name,
                0,
                0,
                0);
        }
    }

    private sealed class CustomBehaviorWithdraw : IFilterTargetingPosition, IIgnoreInvisibilityInterruptionCheck
    {
        public IEnumerator ComputeValidPositions(CursorLocationSelectPosition cursorLocationSelectPosition)
        {
            cursorLocationSelectPosition.validPositionsCache.Clear();

            var actingCharacter = cursorLocationSelectPosition.ActionParams.ActingCharacter;
            var positioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
            var visibilityService = ServiceRepository.GetService<IGameLocationVisibilityService>();

            var halfMaxTacticalMoves = (actingCharacter.MaxTacticalMoves + 1) / 2; // half-rounded up
            var boxInt = new BoxInt(actingCharacter.LocationPosition, int3.zero, int3.zero);

            boxInt.Inflate(halfMaxTacticalMoves, 0, halfMaxTacticalMoves);

            foreach (var position in boxInt.EnumerateAllPositionsWithin())
            {
                if (!visibilityService.MyIsCellPerceivedByCharacter(position, actingCharacter) ||
                    !positioningService.CanPlaceCharacter(
                        actingCharacter, position, CellHelpers.PlacementMode.Station) ||
                    !positioningService.CanCharacterStayAtPosition_Floor(
                        actingCharacter, position, onlyCheckCellsWithRealGround: true))
                {
                    continue;
                }

                cursorLocationSelectPosition.validPositionsCache.Add(position);

                if (cursorLocationSelectPosition.stopwatch.Elapsed.TotalMilliseconds > 0.5)
                {
                    yield return null;
                }
            }
        }
    }

    private sealed class ActionFinishedByWithdraw : IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            if (action is not (CharacterActionMove or CharacterActionMoveStepWalk))
            {
                yield break;
            }

            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;

            if (!rulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagCombat, ConditionWithdrawn.Name, out var activeCondition))
            {
                yield break;
            }

            activeCondition.Amount--;

            if (activeCondition.Amount <= 0)
            {
                rulesetCharacter.RemoveCondition(activeCondition);
            }
        }
    }

    internal static void SwitchRogueCunningStrike()
    {
        Rogue.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == _featureSetRogueCunningStrike ||
            x.FeatureDefinition == _featureRogueImprovedCunningStrike ||
            x.FeatureDefinition == _featureSetRogueDeviousStrike);

        if (Main.Settings.EnableRogueCunningStrike)
        {
            Rogue.FeatureUnlocks.AddRange(
                new FeatureUnlockByLevel(_featureSetRogueCunningStrike, 5),
                new FeatureUnlockByLevel(_featureRogueImprovedCunningStrike, 11),
                new FeatureUnlockByLevel(_featureSetRogueDeviousStrike, 14));
        }

        Rogue.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    #endregion
}
