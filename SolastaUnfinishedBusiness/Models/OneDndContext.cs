using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Validators;
using TA.AI;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ItemDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionRegenerations;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionRestHealingModifiers;

namespace SolastaUnfinishedBusiness.Models;

internal static class OneDndContext
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
        .Create(FeatureDefinitionPointPools.PointPoolWarlockInvocation2, "PointPoolWarlockInvocation1")
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
        [.. FeatureDefinitionProficiencys.ProficiencyDruidWeapon.Proficiencies];

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
    
        internal static void LateLoad()
    {
        BuildOneDndGuidanceSubspells();
        SwapOneDndBarkskinSpell();
        SwapOneDndGuidanceSpell();
        SwitchOneDnDEnableDruidToUseMetalArmor();
        SwitchDruidWeaponProficiencyToUseOneDnd();
        SwitchEnableDruidPrimalOrderAndRemoveMediumArmorProficiency();
        SwitchEnableRitualOnAllCasters();
        SwitchOneDndPreparedSpellsTables();
        SwitchOneDndPaladinLayOnHandAsBonusAction();
        SwitchOneDndEnableBardSuperiorInspirationAtLevel18();
        SwitchOneDndEnableBardWordsOfCreationAtLevel20();
        SwitchOneDndRemoveBardSongOfRest();
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
    }
       
        
    internal static void SwitchOneDnDEnableDruidToUseMetalArmor()
    {
        var active = Main.Settings.EnableDruidToUseMetalArmor;

        if (active)
        {
            FeatureDefinitionProficiencys.ProficiencyDruidArmor.ForbiddenItemTags.Clear();
        }
        else
        {
            if (!FeatureDefinitionProficiencys.ProficiencyDruidArmor.ForbiddenItemTags.Contains(
                    TagsDefinitions.ItemTagMetal))
            {
                FeatureDefinitionProficiencys.ProficiencyDruidArmor.ForbiddenItemTags.Add(
                    TagsDefinitions.ItemTagMetal);
            }
        }
    }

    internal static void SwitchEnableDruidPrimalOrderAndRemoveMediumArmorProficiency()
    {
        if (Main.Settings.EnableDruidPrimalOrderAndRemoveMediumArmorProficiency)
        {
            FeatureDefinitionProficiencys.ProficiencyDruidArmor.Proficiencies.Remove(
                EquipmentDefinitions.MediumArmorCategory);

            Druid.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureSetDruidPrimalOrder, 1));
        }
        else
        {
            FeatureDefinitionProficiencys.ProficiencyDruidArmor.Proficiencies.TryAdd(
                EquipmentDefinitions.MediumArmorCategory);

            Druid.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == FeatureSetDruidPrimalOrder);
        }

        Druid.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchDruidWeaponProficiencyToUseOneDnd()
    {
        FeatureDefinitionProficiencys.ProficiencyDruidWeapon.proficiencies =
            Main.Settings.SwapDruidWeaponProficiencyToUseOneDnd
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
    
    internal static void SwapOneDndBarkskinSpell()
    {
        if (Main.Settings.SwapOneDndBarkskinSpell)
        {
            Barkskin.requiresConcentration = false;
            Barkskin.castingTime = ActivationTime.BonusAction;
            FeatureDefinitionAttributeModifiers.AttributeModifierBarkskin.modifierValue = 17;
            Barkskin.GuiPresentation.description = "Spell/&BarkskinOneDndDescription";
            ConditionBarkskin.GuiPresentation.description = "Rules/&ConditionOneDndBarkskinDescription";
        }
        else
        {
            Barkskin.requiresConcentration = true;
            Barkskin.castingTime = ActivationTime.Action;
            FeatureDefinitionAttributeModifiers.AttributeModifierBarkskin.modifierValue = 16;
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

    internal static void SwapOneDndGuidanceSpell()
    {
        foreach (var spell in GuidanceSubSpells)
        {
            spell.implemented = false;
        }

        if (Main.Settings.SwapOneDndGuidanceSpell)
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
                FeatureDefinitionActionAffinitys.ActionAffinityConditionSurprised,
                FeatureDefinitionMovementAffinitys.MovementAffinityConditionSurprised);
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

        foreach (var featureUnlock in Ranger.FeatureUnlocks
                     .Where(x => x.FeatureDefinition == FeatureDefinitionPointPools.PointPoolBardExpertiseLevel3))
        {
            featureUnlock.level = level;
        }

        level = Main.Settings.EnableBardExpertiseOneLevelBefore ? 9 : 10;

        foreach (var featureUnlock in Ranger.FeatureUnlocks
                     .Where(x => x.FeatureDefinition == FeatureDefinitionPointPools.PointPoolBardExpertiseLevel10))
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

    internal static void SwitchOneDndRemoveBardSongOfRest()
    {
        Bard.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == RestHealingModifierBardSongOfRest);

        if (!Main.Settings.RemoveBardSongOfRest)
        {
            Bard.FeatureUnlocks.Add(new FeatureUnlockByLevel(RestHealingModifierBardSongOfRest, 2));
        }

        Bard.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchOneDndRemoveBardMagicalSecretAt14And18()
    {
        Bard.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == FeatureDefinitionPointPools.PointPoolBardMagicalSecrets14 ||
            x.FeatureDefinition == Level20Context.PointPoolBardMagicalSecrets18);

        if (!Main.Settings.RemoveBardMagicalSecretAt14And18)
        {
            Bard.FeatureUnlocks.AddRange(
                new FeatureUnlockByLevel(FeatureDefinitionPointPools.PointPoolBardMagicalSecrets14, 14),
                new FeatureUnlockByLevel(Level20Context.PointPoolBardMagicalSecrets18, 18));
        }

        Bard.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchOneDndEnableBardSuperiorInspirationAtLevel18()
    {
        if (Main.Settings.RemoveBardMagicalSecretAt14And18)
        {
            Bard.FeatureUnlocks.Add(
                new FeatureUnlockByLevel(Level20Context.FeatureBardSuperiorInspiration2024, 18));
        }
        else
        {
            Bard.FeatureUnlocks.RemoveAll(x =>
                x.FeatureDefinition == Level20Context.FeatureBardSuperiorInspiration2024);
        }

        Bard.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchOneDndEnableBardWordsOfCreationAtLevel20()
    {
        if (Main.Settings.RemoveBardMagicalSecretAt14And18)
        {
            Bard.FeatureUnlocks.Add(
                new FeatureUnlockByLevel(Level20Context.AutoPreparedSpellsBardWordOfCreation, 20));
        }
        else
        {
            Bard.FeatureUnlocks.RemoveAll(x =>
                x.FeatureDefinition == Level20Context.AutoPreparedSpellsBardWordOfCreation);
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
        var dice = Main.Settings.EnableOneDndHealingSpellsBuf ? 2 : 1;

        // Cure Wounds, Healing Word got buf on base damage and add dice
        CureWounds.effectDescription.EffectForms[0].healingForm.diceNumber = dice;
        CureWounds.effectDescription.effectAdvancement.additionalDicePerIncrement = dice;
        HealingWord.effectDescription.EffectForms[0].healingForm.diceNumber = dice;
        HealingWord.effectDescription.effectAdvancement.additionalDicePerIncrement = dice;

        // Mass Cure Wounds and Mass Healing Word only got buf on base damage
        MassHealingWord.effectDescription.EffectForms[0].healingForm.diceNumber = dice;

        dice = Main.Settings.EnableOneDndHealingSpellsBuf ? 5 : 3;

        MassCureWounds.effectDescription.EffectForms[0].healingForm.diceNumber = dice;
    }

    internal static void SwitchOneDndWizardScholar()
    {
        if (Main.Settings.EnableWizardToLearnScholarAtLevel2)
        {
            Wizard.FeatureUnlocks.Add(new FeatureUnlockByLevel(PointPoolWizardScholar, 2));
        }
        else
        {
            Wizard.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == PointPoolWizardScholar);
        }

        Wizard.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchOneDndWarlockMagicalCunningAtLevel2()
    {
        if (Main.Settings.EnableWarlockMagicalCunningAtLevel2)
        {
            Warlock.FeatureUnlocks.Add(new FeatureUnlockByLevel(PowerWarlockMagicalCunning, 2));
        }
        else
        {
            Warlock.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == PowerWarlockMagicalCunning);
        }

        Warlock.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchOneDndWarlockInvocationsProgression()
    {
        if (Main.Settings.SwapWarlockToUseOneDndInvocationProgression)
        {
            Warlock.FeatureUnlocks.Add(new FeatureUnlockByLevel(PointPoolWarlockInvocation1, 1));
            FeatureDefinitionPointPools.PointPoolWarlockInvocation2.GuiPresentation.Title =
                "Feature/&PointPoolWarlockInvocationAdditionalTitle";
            FeatureDefinitionPointPools.PointPoolWarlockInvocation2.GuiPresentation.Description =
                "Feature/&PointPoolWarlockInvocationAdditionalDescription";
            FeatureDefinitionPointPools.PointPoolWarlockInvocation5.poolAmount = 2;
        }
        else
        {
            Warlock.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == PointPoolWarlockInvocation1);
            FeatureDefinitionPointPools.PointPoolWarlockInvocation2.GuiPresentation.Title =
                "Feature/&PointPoolWarlockInvocationInitialTitle";
            FeatureDefinitionPointPools.PointPoolWarlockInvocation2.GuiPresentation.Description =
                "Feature/&PointPoolWarlockInvocationInitialDescription";
            FeatureDefinitionPointPools.PointPoolWarlockInvocation5.poolAmount = 1;
        }

        Warlock.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }
}
