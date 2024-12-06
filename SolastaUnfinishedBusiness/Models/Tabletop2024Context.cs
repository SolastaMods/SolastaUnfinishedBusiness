using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
using SolastaUnfinishedBusiness.Subclasses;
using SolastaUnfinishedBusiness.Subclasses.Builders;
using SolastaUnfinishedBusiness.Validators;
using TA;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionRestHealingModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMovementAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPointPools;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionProficiencys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ItemDefinitions;

namespace SolastaUnfinishedBusiness.Models;

internal static partial class Tabletop2024Context
{
    private static readonly FeatureDefinitionActionAffinity ActionAffinityPotionBonusAction =
        FeatureDefinitionActionAffinityBuilder
            .Create("ActionAffinityPotionBonusAction")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(
                new ValidateDeviceFunctionUse((_, device, _) =>
                    device.UsableDeviceDescription.UsableDeviceTags.Contains("Potion")))
            .SetAuthorizedActions(Id.UseItemBonus)
            .AddToDB();

    private static readonly FeatureDefinitionActionAffinity ActionAffinityPoisonBonusAction =
        FeatureDefinitionActionAffinityBuilder
            .Create("ActionAffinityPoisonBonusAction")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(
                new ValidateDeviceFunctionUse((_, device, _) =>
                    device.UsableDeviceDescription.UsableDeviceTags.Contains("Poison")))
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

    private static readonly ItemPropertyDescription ItemPropertyPoisonBonusAction =
        new(RingFeatherFalling.StaticProperties[0])
        {
            appliesOnItemOnly = false,
            type = ItemPropertyDescription.PropertyType.Feature,
            featureDefinition = ActionAffinityPoisonBonusAction,
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
        LoadBarbarianInstinctivePounce();
        LoadBarbarianPersistentRage();
        LoadFighterSecondWind();
        LoadFighterStudiedAttacks();
        LoadFighterTacticalProgression();
        LoadMonkHeightenedMetabolism();
        LoadOneDndEnableBardCounterCharmAsReactionAtLevel7();
        LoadOneDndSpellSpareTheDying();
        LoadOneDndTrueStrike();
        LoadSorcerousRestorationAtLevel5();
        LoadWizardMemorizeSpell();
        SwitchBarbarianBrutalStrike();
        SwitchBarbarianInstinctivePounce();
        SwitchBarbarianPersistentRage();
        SwitchBarbarianRecklessSameBuffDebuffDuration();
        SwitchBarbarianRegainOneRageAtShortRest();
        SwitchBarbarianRelentlessRage();
        SwitchDruidPrimalOrderAndRemoveMediumArmorProficiency();
        SwitchDruidWeaponProficiencyToUseOneDnd();
        SwitchFighterIndomitableSaving();
        SwitchFighterSecondWind();
        SwitchFighterSkillOptions();
        SwitchFighterStudiedAttacks();
        SwitchFighterTacticalProgression();
        SwitchMonkBodyAndMindToReplacePerfectSelf();
        SwitchMonkDoNotRequireAttackActionForBonusUnarmoredAttack();
        SwitchMonkDoNotRequireAttackActionForFlurry();
        SwitchMonkHeightenedMetabolism();
        SwitchMonkSuperiorDefenseToReplaceEmptyBody();
        SwitchOneDnDEnableDruidUseMetalArmor();
        SwitchOneDndAllPotionsBonusAction();
        SwitchOneDndChangeBardicInspirationDurationToOneHour();
        SwitchOneDndDamagingSpellsUpgrade();
        SwitchOneDndEnableBardCounterCharmAsReactionAtLevel7();
        SwitchOneDndEnableBardExpertiseOneLevelBefore();
        SwitchOneDndEnableBardSuperiorInspirationAtLevel18();
        SwitchOneDndEnableBardWordsOfCreationAtLevel20();
        SwitchOneDndHealingSpellsUpgrade();
        SwitchOneDndMonkUnarmedDieTypeProgression();
        SwitchOneDndPaladinLayOnHandAsBonusAction();
        SwitchOneDndPaladinLearnSpellCastingAtOne();
        SwitchOneDndPoisonsBonusAction();
        SwitchOneDndPreparedSpellsTables();
        SwitchOneDndRangerLearnSpellCastingAtOne();
        SwitchOneDndRemoveBardMagicalSecretAt14And18();
        SwitchOneDndRemoveBardSongOfRestAt2();
        SwitchOneDndSpellBarkskin();
        SwitchOneDndSpellDivineFavor();
        SwitchOneDndSpellGuidance();
        SwitchOneDndSpellHideousLaughter();
        SwitchOneDndSpellHuntersMark();
        SwitchOneDndSpellLesserRestoration();
        SwitchOneDndSpellMagicWeapon();
        SwitchOneDndSpellPowerWordStun();
        SwitchOneDndSpellSpareTheDying();
        SwitchOneDndSpellSpiderClimb();
        SwitchOneDndSpellStoneSkin();
        SwitchOneDndSurprisedEnforceDisadvantage();
        SwitchOneDndWarlockInvocationsProgression();
        SwitchOneDndWarlockPatronLearningLevel();
        SwitchOneDndWizardMemorizeSpell();
        SwitchOneDndWizardScholar();
        SwitchOneDndWizardSchoolOfMagicLearningLevel();
        SwitchRangerNatureShroud();
        SwitchRogueBlindSense();
        SwitchRogueCunningStrike();
        SwitchRogueReliableTalent();
        SwitchRogueSlipperyMind();
        SwitchRogueSteadyAim();
        SwitchSorcererArcaneApotheosis();
        SwitchSorcererInnateSorcery();
        SwitchSorcerousRestorationAtLevel5();
        SwitchSpellRitualOnAllCasters();
        SwitchWarlockMagicalCunningAtLevel2AndImprovedEldritchMasterAt20();
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

    internal static void SwitchOneDndAllPotionsBonusAction()
    {
        if (Main.Settings.OneDndAllPotionsBonusAction)
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

    internal static void SwitchOneDndPoisonsBonusAction()
    {
        if (Main.Settings.OneDndPoisonsBonusAction)
        {
            foreach (var poison in DatabaseRepository.GetDatabase<ItemDefinition>()
                         .Where(a =>
                             a.UsableDeviceDescription != null &&
                             a.UsableDeviceDescription.usableDeviceTags.Contains("Poison")))
            {
                poison.StaticProperties.TryAdd(ItemPropertyPoisonBonusAction);
            }
        }
        else
        {
            foreach (var poison in DatabaseRepository.GetDatabase<ItemDefinition>()
                         .Where(a =>
                             a.UsableDeviceDescription != null &&
                             a.UsableDeviceDescription.usableDeviceTags.Contains("Poison")))
            {
                poison.StaticProperties.Clear();
            }
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

    private static bool TryGetMemorizeSpellCondition(RulesetCharacter character, out RulesetCondition condition)
    {
        return character.TryGetConditionOfCategoryAndType(
            AttributeDefinitions.TagEffect, ConditionMemorizeSpell.Name, out condition);
    }

    internal static bool IsMemorizeSpellPreparation(RulesetCharacter character)
    {
        return TryGetMemorizeSpellCondition(character, out _);
    }

    internal static bool IsInvalidMemorizeSelectedSpell(
        SpellRepertoirePanel spellRepertoirePanel, RulesetCharacter rulesetCharacter, SpellDefinition spell)
    {
        if (!TryGetMemorizeSpellCondition(rulesetCharacter, out var activeCondition))
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

    private sealed class CustomBehaviorFilterTargetingPositionHalfMove : IFilterTargetingPosition,
        IIgnoreInvisibilityInterruptionCheck
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
}
