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
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Subclasses;
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

namespace SolastaUnfinishedBusiness.Models;

internal static class Tabletop2024Context
{
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

    private static readonly FeatureDefinitionPower PowerWarlockMagicalCunning = FeatureDefinitionPowerBuilder
        .Create("PowerWarlockMagicalCunning")
        .SetGuiPresentation(Category.Feature, PowerWizardArcaneRecovery)
        .SetUsesFixed(ActivationTime.Rest, RechargeRate.LongRest)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Self)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetSpellForm(5)
                        .Build())
                .SetParticleEffectParameters(PowerWizardArcaneRecovery)
                .Build())
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
                    FeatureDefinitionAbilityCheckAffinityBuilder
                        .Create("AbilityCheckDruidPrimalOrderMagician")
                        .SetGuiPresentation("FeatureSetDruidPrimalOrderMagician", Category.Feature)
                        .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.None, DieType.D6, 1,
                            AbilityCheckGroupOperation.AddDie,
                            (AttributeDefinitions.Intelligence, SkillDefinitions.Arcana),
                            (AttributeDefinitions.Intelligence, SkillDefinitions.Nature))
                        .AddToDB(),
                    FeatureDefinitionPointPoolBuilder
                        .Create("PointPoolDruidPrimalOrderMagician")
                        .SetGuiPresentationNoContent(true)
                        .SetSpellOrCantripPool(HeroDefinitions.PointsPoolType.Cantrip, 1)
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

    private static readonly FeatureDefinitionPower PowerSorcererInnateSorcery = FeatureDefinitionPowerBuilder
        .Create("PowerSorcererInnateSorcery")
        .SetGuiPresentation(Category.Feature, PowerTraditionShockArcanistGreaterArcaneShock)
        .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest, 1, 2)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Minute, 1)
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetEffectForms(EffectFormBuilder.ConditionForm(
                    ConditionDefinitionBuilder
                        .Create("ConditionSorcererInnateSorcery")
                        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionConjuredCreature)
                        .SetFeatures(
                            FeatureDefinitionMagicAffinityBuilder
                                .Create("MagicAffinitySorcererInnateSorcery")
                                .SetGuiPresentation("PowerSorcererInnateSorcery", Category.Feature)
                                .SetCastingModifiers(0, SpellParamsModifierType.None, 1)
                                .AddToDB())
                        .AddCustomSubFeatures(new ModifyAttackActionModifierInnateSorcery())
                        .AddToDB()))
                .SetParticleEffectParameters(Shield)
                .Build())
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

    internal static void LateLoad()
    {
        BuildBarbarianBrutalStrike();
        BuildRogueCunningStrike();
        BuildOneDndGuidanceSubspells();
        LoadMonkHeightenedMetabolism();
        LoadSecondWindToUseOneDndUsagesProgression();
        EnableOneDndBarkskinSpell();
        EnableOneDndGuidanceSpell();
        SwitchOneDnDEnableDruidUseMetalArmor();
        SwitchDruidWeaponProficiencyToUseOneDnd();
        SwitchEnableDruidPrimalOrderAndRemoveMediumArmorProficiency();
        SwitchEnableRitualOnAllCasters();
        SwitchOneDndPreparedSpellsTables();
        SwitchOneDndPaladinLayOnHandAsBonusAction();
        SwitchOneDndEnableBardSuperiorInspirationAtLevel18();
        SwitchOneDndEnableBardWordsOfCreationAtLevel20();
        SwitchOneDndRemoveBardSongOfRestAt2();
        SwitchOneDndRemoveBardMagicalSecretAt14And18();
        SwitchOneDndChangeBardicInspirationDurationToOneHour();
        SwitchOneDndEnableBardExpertiseOneLevelBefore();
        SwitchOneDndWarlockInvocationsProgression();
        SwitchOneDndWarlockMagicalCunningAtLevel2();
        SwitchOneDndHealingPotionBonusAction();
        SwitchOneDndHealingSpellsBuf();
        SwitchOneDndWizardScholar();
        SwitchOneDndWizardSchoolOfMagicLearningLevel();
        SwitchOneDndPaladinLearnSpellCastingAtOne();
        SwitchOneDndRangerLearnSpellCastingAtOne();
        SwitchOneDndSurprisedEnforceDisadvantage();
        SwitchRangerNatureShroud();
        SwitchBarbarianBrutalStrike();
        SwitchBarbarianBrutalCritical();
        SwitchBarbarianRecklessSameBuffDebuffDuration();
        SwitchBarbarianRegainOneRageAtShortRest();
        SwitchRogueBlindSense();
        SwitchRogueCunningStrike();
        SwitchRogueSteadyAim();
        SwitchPersuasionToFighterSkillOptions();
        SwitchFighterLevelToIndomitableSavingReroll();
        SwitchSorcererInnateSorcery();
        SwitchMonkHeightenedMetabolism();
        SwitchMonkSuperiorDefenseToReplaceEmptyBody();
        SwitchMonkBodyAndMindToReplacePerfectSelf();
        SwitchOneDndMonkUnarmedDieTypeProgression();
        SwitchMonkDoNotRequireAttackActionForBonusUnarmoredAttack();
        SwitchMonkDoNotRequireAttackActionForFlurry();
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
            },
            new ModifyPowerPoolAmount
            {
                PowerPool = PowerFighterSecondWind,
                Type = PowerPoolBonusCalculationType.SecondWind2024,
                Attribute = FighterClass
            });
    }

    internal static void SwitchFighterLevelToIndomitableSavingReroll()
    {
        UseIndomitableResistance.GuiPresentation.description =
            Main.Settings.AddFighterLevelToIndomitableSavingReroll
                ? "Feature/&EnhancedIndomitableResistanceDescription"
                : "Feature/&IndomitableResistanceDescription";
    }

    internal static void SwitchPersuasionToFighterSkillOptions()
    {
        if (Main.Settings.AddPersuasionToFighterSkillOptions)
        {
            PointPoolFighterSkillPoints.restrictedChoices.TryAdd(SkillDefinitions.Persuasion);
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

    internal static void SwitchEnableDruidPrimalOrderAndRemoveMediumArmorProficiency()
    {
        if (Main.Settings.EnableDruidPrimalOrderAndRemoveMediumArmorProficiency)
        {
            ProficiencyDruidArmor.Proficiencies.Remove(
                EquipmentDefinitions.MediumArmorCategory);

            Druid.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureSetDruidPrimalOrder, 1));
        }
        else
        {
            ProficiencyDruidArmor.Proficiencies.TryAdd(
                EquipmentDefinitions.MediumArmorCategory);

            Druid.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == FeatureSetDruidPrimalOrder);
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

    internal static void SwitchEnableRitualOnAllCasters()
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

        foreach (var subclass in subclasses)
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

    internal static void EnableOneDndBarkskinSpell()
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

    internal static void EnableOneDndGuidanceSpell()
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

        foreach (var featureUnlock in Wizard.FeatureUnlocks.Where(featureUnlock => featureUnlock.level == fromLevel))
        {
            featureUnlock.level = toLevel;
        }

        foreach (var school in schools)
        {
            school.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        }

        Wizard.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
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
            FeatureDefinitionCastSpells.CastSpellSorcerer.knownSpells =
                [2, 4, 6, 7, 9, 10, 11, 12, 14, 15, 16, 16, 17, 17, 18, 18, 19, 20, 21, 22];
        }
        else
        {
            FeatureDefinitionCastSpells.CastSpellBard.knownSpells =
                [4, 5, 6, 7, 8, 9, 10, 11, 12, 14, 15, 15, 16, 18, 19, 19, 20, 22, 22, 22];
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
            PowerFunctionPotionOfHealing.activationTime = ActivationTime.BonusAction;
            PowerFunctionPotionOfHealingOther.activationTime = ActivationTime.BonusAction;
            PowerFunctionPotionOfGreaterHealing.activationTime = ActivationTime.BonusAction;
            PowerFunctionPotionOfGreaterHealingOther.activationTime = ActivationTime.BonusAction;
            PowerFunctionPotionOfSuperiorHealing.activationTime = ActivationTime.BonusAction;
            PowerFunctionPotionOfSuperiorHealingOther.activationTime = ActivationTime.BonusAction;
            PowerFunctionPotionRemedy.activationTime = ActivationTime.BonusAction;
            PowerFunctionRemedyOther.activationTime = ActivationTime.BonusAction;
            PowerFunctionAntitoxin.activationTime = ActivationTime.BonusAction;
        }
        else
        {
            PowerFunctionPotionOfHealing.activationTime = ActivationTime.Action;
            PowerFunctionPotionOfHealingOther.activationTime = ActivationTime.Action;
            PowerFunctionPotionOfGreaterHealing.activationTime = ActivationTime.Action;
            PowerFunctionPotionOfGreaterHealingOther.activationTime = ActivationTime.Action;
            PowerFunctionPotionOfSuperiorHealing.activationTime = ActivationTime.Action;
            PowerFunctionPotionOfSuperiorHealingOther.activationTime = ActivationTime.Action;
            PowerFunctionPotionRemedy.activationTime = ActivationTime.Action;
            PowerFunctionRemedyOther.activationTime = ActivationTime.Action;
            PowerFunctionAntitoxin.activationTime = ActivationTime.Action;
        }
    }

    internal static void SwitchOneDndHealingSpellsBuf()
    {
        var dice = Main.Settings.EnableOneDndHealingSpellsUpgrade ? 2 : 1;

        // Cure Wounds, Healing Word got buf on base damage and add dice
        CureWounds.effectDescription.EffectForms[0].healingForm.diceNumber = dice;
        CureWounds.effectDescription.effectAdvancement.additionalDicePerIncrement = dice;
        HealingWord.effectDescription.EffectForms[0].healingForm.diceNumber = dice;
        HealingWord.effectDescription.effectAdvancement.additionalDicePerIncrement = dice;

        // Mass Cure Wounds and Mass Healing Word only got buf on base damage
        MassHealingWord.effectDescription.EffectForms[0].healingForm.diceNumber = dice;

        dice = Main.Settings.EnableOneDndHealingSpellsUpgrade ? 5 : 3;

        MassCureWounds.effectDescription.EffectForms[0].healingForm.diceNumber = dice;
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

    internal static void SwitchSorcererInnateSorcery()
    {
        Sorcerer.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == PowerSorcererInnateSorcery);

        if (Main.Settings.EnableSorcererInnateSorcery)
        {
            Sorcerer.FeatureUnlocks.TryAdd(new FeatureUnlockByLevel(PowerSorcererInnateSorcery, 1));
        }

        Sorcerer.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchRangerNatureShroud()
    {
        Ranger.FeatureUnlocks
            .RemoveAll(x => x.FeatureDefinition == FeatureDefinitionPowerNatureShroud);

        if (Main.Settings.EnableRangerNatureShroudAt14)
        {
            Ranger.FeatureUnlocks.TryAdd(
                new FeatureUnlockByLevel(FeatureDefinitionPowerNatureShroud, 14));
        }

        Ranger.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchOneDndWarlockMagicalCunningAtLevel2()
    {
        Warlock.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == PowerWarlockMagicalCunning);

        if (Main.Settings.EnableWarlockMagicalCunningAtLevel2)
        {
            Warlock.FeatureUnlocks.Add(new FeatureUnlockByLevel(PowerWarlockMagicalCunning, 2));
        }

        Warlock.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchOneDndWarlockInvocationsProgression()
    {
        if (Main.Settings.SwapWarlockToUseOneDndInvocationProgression)
        {
            Warlock.FeatureUnlocks.Add(new FeatureUnlockByLevel(PointPoolWarlockInvocation1, 1));
            PointPoolWarlockInvocation2.GuiPresentation.Title =
                "Feature/&PointPoolWarlockInvocationAdditionalTitle";
            PointPoolWarlockInvocation2.GuiPresentation.Description =
                "Feature/&PointPoolWarlockInvocationAdditionalDescription";
            PointPoolWarlockInvocation5.poolAmount = 2;
        }
        else
        {
            Warlock.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == PointPoolWarlockInvocation1);
            PointPoolWarlockInvocation2.GuiPresentation.Title =
                "Feature/&PointPoolWarlockInvocationInitialTitle";
            PointPoolWarlockInvocation2.GuiPresentation.Description =
                "Feature/&PointPoolWarlockInvocationInitialDescription";
            PointPoolWarlockInvocation5.poolAmount = 1;
        }

        Warlock.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
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
        if (Main.Settings.EnableMonkHeightenedMetabolism)
        {
            Monk.FeatureUnlocks.TryAdd(
                new FeatureUnlockByLevel(FeatureMonkHeightenedMetabolism, 10));
            Monk.FeatureUnlocks.TryAdd(
                new FeatureUnlockByLevel(PowerMonkStepOfTheWindHeightenedMetabolism, 10));
        }
        else
        {
            Monk.FeatureUnlocks
                .RemoveAll(x => x.level == 10 &&
                                (x.FeatureDefinition == FeatureMonkHeightenedMetabolism ||
                                 x.FeatureDefinition == PowerMonkStepOfTheWindHeightenedMetabolism));
        }

        Monk.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchMonkSuperiorDefenseToReplaceEmptyBody()
    {
        Monk.FeatureUnlocks
            .RemoveAll(x => x.level == 18 &&
                            (x.FeatureDefinition == Level20Context.PowerMonkEmptyBody ||
                             x.FeatureDefinition == PowerMonkSuperiorDefense));

        Monk.FeatureUnlocks.TryAdd(
            Main.Settings.EnableMonkSuperiorDefenseToReplaceEmptyBody
                ? new FeatureUnlockByLevel(PowerMonkSuperiorDefense, 18)
                : new FeatureUnlockByLevel(Level20Context.PowerMonkEmptyBody, 18));

        Monk.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchMonkBodyAndMindToReplacePerfectSelf()
    {
        Monk.FeatureUnlocks
            .RemoveAll(x => x.level == 20 &&
                            (x.FeatureDefinition == Level20Context.FeatureMonkPerfectSelf ||
                             x.FeatureDefinition == FeatureMonkBodyAndMind));

        Monk.FeatureUnlocks.TryAdd(
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

        public IEnumerator OnPhysicalAttackFinishedByMe(GameLocationBattleManager battleManager, CharacterAction action,
            GameLocationCharacter attacker, GameLocationCharacter defender, RulesetAttackMode attackMode,
            RollOutcome rollOutcome, int damageAmount)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (!rulesetAttacker.IsToggleEnabled((Id)ExtraActionId.BrutalStrikeToggle) ||
                !rulesetAttacker.HasConditionOfCategoryAndType(
                    AttributeDefinitions.TagCombat, ConditionDefinitions.ConditionReckless.Name))
            {
                yield break;
            }

            attacker.UsedSpecialFeatures.TryAdd(BrutalStrike, 0);
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
        if (Main.Settings.EnableBarbarianBrutalStrike)
        {
            Barbarian.FeatureUnlocks.TryAdd(
                new FeatureUnlockByLevel(_featureSetBarbarianBrutalStrike, 9));
            Barbarian.FeatureUnlocks.TryAdd(
                new FeatureUnlockByLevel(_featureSetBarbarianBrutalStrikeImprovement13, 13));
            Barbarian.FeatureUnlocks.TryAdd(
                new FeatureUnlockByLevel(_featureSetBarbarianBrutalStrikeImprovement17, 17));
        }
        else
        {
            Barbarian.FeatureUnlocks.RemoveAll(x =>
                x.level == 9 && x.FeatureDefinition == _featureSetBarbarianBrutalStrike);
            Barbarian.FeatureUnlocks.RemoveAll(x =>
                x.level == 13 && x.FeatureDefinition == _featureSetBarbarianBrutalStrikeImprovement13);
            Barbarian.FeatureUnlocks.RemoveAll(x =>
                x.level == 17 && x.FeatureDefinition == _featureSetBarbarianBrutalStrikeImprovement17);
        }

        Barbarian.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchBarbarianBrutalCritical()
    {
        if (Main.Settings.DisableBarbarianBrutalCritical)
        {
            Barbarian.FeatureUnlocks.RemoveAll(x =>
                x.level == 9 && x.FeatureDefinition == FeatureSetBarbarianBrutalCritical);
            Barbarian.FeatureUnlocks.RemoveAll(x =>
                x.level == 13 && x.FeatureDefinition == AttributeModifierBarbarianBrutalCriticalAdd);
            Barbarian.FeatureUnlocks.RemoveAll(x =>
                x.level == 17 && x.FeatureDefinition == AttributeModifierBarbarianBrutalCriticalAdd);
        }
        else
        {
            if (!Barbarian.FeatureUnlocks.Exists(x =>
                    x.level == 9 && x.FeatureDefinition == FeatureSetBarbarianBrutalCritical))
            {
                Barbarian.FeatureUnlocks.TryAdd(
                    new FeatureUnlockByLevel(FeatureSetBarbarianBrutalCritical, 9));
            }

            if (!Barbarian.FeatureUnlocks.Exists(x =>
                    x.level == 13 && x.FeatureDefinition == AttributeModifierBarbarianBrutalCriticalAdd))
            {
                Barbarian.FeatureUnlocks.TryAdd(
                    new FeatureUnlockByLevel(AttributeModifierBarbarianBrutalCriticalAdd, 13));
            }

            if (!Barbarian.FeatureUnlocks.Exists(x =>
                    x.level == 17 && x.FeatureDefinition == AttributeModifierBarbarianBrutalCriticalAdd))
            {
                Barbarian.FeatureUnlocks.TryAdd(
                    new FeatureUnlockByLevel(AttributeModifierBarbarianBrutalCriticalAdd, 17));
            }
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
        .SetSpecialInterruptions(ConditionInterruption.Attacks)
        .AddToDB();

    private static FeatureDefinitionFeatureSet _featureSetRogueCunningStrike;
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
            .AddFeatureSet(powerPool, actionAffinityToggle, powerDisarm, powerPoison, powerTrip, powerWithdraw)
            .AddToDB();

        _featureSetRogueDeviousStrike = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Devious}")
            .SetGuiPresentation($"Power{Devious}", Category.Feature)
            .AddFeatureSet(powerDaze, powerKnockOut, powerObscure)
            .AddToDB();
    }

    internal static void SwitchRogueSteadyAim()
    {
        if (Main.Settings.EnableRogueSteadyAim)
        {
            Rogue.FeatureUnlocks.TryAdd(new FeatureUnlockByLevel(PowerFeatSteadyAim, 3));
        }
        else
        {
            Rogue.FeatureUnlocks.RemoveAll(x =>
                x.level == 3 && x.FeatureDefinition == PowerFeatSteadyAim);
        }

        Rogue.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchRogueBlindSense()
    {
        Rogue.FeatureUnlocks.RemoveAll(x =>
            x.level == 3 && x.FeatureDefinition == FeatureDefinitionSenses.SenseRogueBlindsense);

        if (!Main.Settings.RemoveRogueBlindSense)
        {
            Rogue.FeatureUnlocks.TryAdd(new FeatureUnlockByLevel(FeatureDefinitionSenses.SenseRogueBlindsense, 14));
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

    private sealed class CustomBehaviorCunningStrike(
        FeatureDefinitionPower powerRogueCunningStrike,
        FeatureDefinitionPower powerKnockOut,
        FeatureDefinitionPower powerKnockOutApply,
        FeatureDefinitionPower powerWithdraw)
        : IPhysicalAttackBeforeHitConfirmedOnEnemy, IPhysicalAttackFinishedByMe
    {
        private FeatureDefinitionPower _selectedPower;

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
            _selectedPower = null;

            var rulesetAttacker = attacker.RulesetCharacter;

            if (!rulesetAttacker.IsToggleEnabled((Id)ExtraActionId.CunningStrikeToggle) ||
                !IsSneakAttackValid(actionModifier, attacker, defender))
            {
                yield break;
            }

            var usablePower = PowerProvider.Get(powerRogueCunningStrike, rulesetAttacker);

            yield return attacker.MyReactToSpendPowerBundle(
                usablePower,
                [defender],
                attacker,
                powerRogueCunningStrike.Name,
                reactionValidated: ReactionValidated,
                battleManager: battleManager);

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

                _selectedPower = subPowers[option];

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
                    _selectedPower.CostPerUse,
                    0,
                    0);
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
            if (_selectedPower == powerKnockOut)
            {
                yield return HandleKnockOut(attacker, defender);
            }
            else if (_selectedPower == powerWithdraw)
            {
                yield return HandleWithdraw(action, attacker);
            }

            _selectedPower = null;
        }

        private IEnumerator HandleWithdraw(CharacterAction action, GameLocationCharacter attacker)
        {
            yield return CampaignsContext.SelectPosition(action, powerWithdraw);

            var rulesetAttacker = attacker.RulesetCharacter;
            var position = action.ActionParams.Positions[0];
            var distance = int3.Distance(attacker.LocationPosition, position);

            attacker.UsedTacticalMoves -= (int)distance;

            if (attacker.UsedTacticalMoves < 0)
            {
                attacker.UsedTacticalMoves = 0;
            }

            attacker.UsedTacticalMovesChanged?.Invoke(attacker);

            rulesetAttacker.InflictCondition(
                RuleDefinitions.ConditionDisengaging,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                // all disengaging in game is set under TagCombat (why?)
                AttributeDefinitions.TagCombat,
                rulesetAttacker.Guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                RuleDefinitions.ConditionDisengaging,
                0,
                0,
                0);

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

    internal static void SwitchRogueCunningStrike()
    {
        if (Main.Settings.EnableRogueCunningStrike)
        {
            Rogue.FeatureUnlocks.TryAdd(new FeatureUnlockByLevel(_featureSetRogueCunningStrike, 5));
            Rogue.FeatureUnlocks.TryAdd(new FeatureUnlockByLevel(_featureSetRogueDeviousStrike, 14));
        }
        else
        {
            Rogue.FeatureUnlocks.RemoveAll(x => x.level == 5 && x.FeatureDefinition == _featureSetRogueCunningStrike);
            Rogue.FeatureUnlocks.RemoveAll(x => x.level == 14 && x.FeatureDefinition == _featureSetRogueDeviousStrike);
        }

        Rogue.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    #endregion
}
