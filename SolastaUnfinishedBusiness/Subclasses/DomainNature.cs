using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using Category = SolastaUnfinishedBusiness.Builders.Category;
using CharacterSubclassDefinitionBuilder = SolastaUnfinishedBusiness.Builders.CharacterSubclassDefinitionBuilder;
using EffectDescriptionBuilder = SolastaUnfinishedBusiness.Builders.EffectDescriptionBuilder;
using EffectFormBuilder = SolastaUnfinishedBusiness.Builders.EffectFormBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class DomainNature : AbstractSubclass
{
    public DomainNature()
    {
        const string NAME = "DomainNature";

        var autoPreparedSpellsDomainNature = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{NAME}")
            .SetGuiPresentation("ExpandedSpells", Category.Feature)
            .SetAutoTag("Domain")
            .SetPreparedSpellGroups(
                BuildSpellGroup(1, AnimalFriendship, Entangle),
                BuildSpellGroup(3, Barkskin, SpikeGrowth),
                BuildSpellGroup(5, ConjureAnimals, WindWall),
                BuildSpellGroup(7, DominateBeast, FreedomOfMovement),
                BuildSpellGroup(9, InsectPlague, Contagion))
            .SetSpellcastingClass(CharacterClassDefinitions.Cleric)
            .AddToDB();

        // LEVEL 01 - Acolyte of Nature

        var pointPoolCantrip = FeatureDefinitionPointPoolBuilder
            .Create($"PointPool{NAME}Cantrip")
            .SetGuiPresentationNoContent(true)
            .SetSpellOrCantripPool(HeroDefinitions.PointsPoolType.Cantrip, 1, SpellListDefinitions.SpellListDruid,
                "Domain")
            .AddToDB();

        var pointPoolSkills = FeatureDefinitionPointPoolBuilder
            .Create($"PointPool{NAME}Skills")
            .SetGuiPresentationNoContent(true)
            .SetPool(HeroDefinitions.PointsPoolType.Skill, 1)
            .OnlyUniqueChoices()
            .RestrictChoices(
                SkillDefinitions.AnimalHandling,
                SkillDefinitions.Nature,
                SkillDefinitions.Survival)
            .AddToDB();

        var proficiencyHeavyArmor = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{NAME}HeavyArmor")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Armor, EquipmentDefinitions.HeavyArmorCategory)
            .AddToDB();

        var featureSetAcolyteOfNature = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}AcolyteOfNature")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(pointPoolCantrip, pointPoolSkills, proficiencyHeavyArmor)
            .AddToDB();

        //
        // Level 2 - Charm Animals and Plants
        //

        var divinePowerPrefix = Gui.Localize("Feature/&ClericChannelDivinityTitle") + ": ";

        var powerCharmAnimalsAndPlants = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}CharmAnimalsAndPlants")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ChannelDivinity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetRestrictedCreatureFamilies("Beast", "Plants")
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Wisdom,
                        true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(ConditionDefinitions.ConditionCharmed,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        var featureSetCharmAnimalsAndPlants = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}CharmAnimalsAndPlants")
            .SetGuiPresentation(
                divinePowerPrefix + powerCharmAnimalsAndPlants.FormatTitle(),
                powerCharmAnimalsAndPlants.FormatDescription())
            .AddFeatureSet(powerCharmAnimalsAndPlants)
            .AddToDB();

        //
        // LEVEL 6 - Dampen Elements
        //

        var conditionDampenElements = ConditionDefinitionBuilder
            .Create($"Condition{NAME}DampenElements")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var powerDampenElements = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}DampenElements")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Permanent)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Permanent)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetRecurrentEffect(
                        RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionDampenElements))
                    .Build())
            .AddToDB();

        //
        // LEVEL 08 - Divine Strike
        //

        var additionalDamageDivineStrikeCold = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}DivineStrikeCold")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("DivineStrike")
            .SetDamageDice(DieType.D8, 1)
            .SetSpecificDamageType(DamageTypeCold)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 8, 6)
            .SetFrequencyLimit(FeatureLimitedUsage.OnceInMyTurn)
            .SetAttackModeOnly()
            .AddToDB();

        var additionalDamageDivineStrikeFire = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}DivineStrikeFire")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("DivineStrike")
            .SetDamageDice(DieType.D8, 1)
            .SetSpecificDamageType(DamageTypeFire)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 8, 6)
            .SetFrequencyLimit(FeatureLimitedUsage.OnceInMyTurn)
            .SetAttackModeOnly()
            .AddToDB();

        var additionalDamageDivineStrikeLightning = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}DivineStrikeLightning")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("DivineStrike")
            .SetDamageDice(DieType.D8, 1)
            .SetSpecificDamageType(DamageTypeLightning)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 8, 6)
            .SetFrequencyLimit(FeatureLimitedUsage.OnceInMyTurn)
            .SetAttackModeOnly()
            .AddToDB();

        var featureSetDivineStrike = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}DivineStrike")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                additionalDamageDivineStrikeCold,
                additionalDamageDivineStrikeFire,
                additionalDamageDivineStrikeLightning)
            .AddToDB();

        // LEVEL 17 - Master of Nature

        var powerMasterOfNature = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}MasterOfNature")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ChannelDivinity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetRestrictedCreatureFamilies("Beast", "Plants")
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Wisdom,
                        true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(ConditionDefinitions.ConditionCharmed,
                                ConditionForm.ConditionOperation.Add)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetConditionForm(ConditionDefinitions.ConditionMindDominatedByCaster,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .SetOverriddenPower(powerCharmAnimalsAndPlants)
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Subclass) //, CharacterSubclassDefinitions.TraditionGreenmage)
            .AddFeaturesAtLevel(1, autoPreparedSpellsDomainNature, featureSetAcolyteOfNature)
            .AddFeaturesAtLevel(2, featureSetCharmAnimalsAndPlants)
            .AddFeaturesAtLevel(6, powerDampenElements)
            .AddFeaturesAtLevel(8, featureSetDivineStrike)
            .AddFeaturesAtLevel(10, PowerClericDivineInterventionWizard)
            .AddFeaturesAtLevel(17, powerMasterOfNature)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Cleric;

    internal override CharacterSubclassDefinition Subclass { get; }

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override FeatureDefinitionSubclassChoice SubclassChoice { get; }

    internal override DeityDefinition DeityDefinition => DeityDefinitions.Maraike;
}
