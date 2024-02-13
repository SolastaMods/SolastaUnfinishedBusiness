using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class RangerSurvivalist : AbstractSubclass
{
    internal const string Name = "RangerSurvivalist";

    private static readonly FeatureDefinitionFeatureSet FeatureSetAnalyticalMind = FeatureDefinitionFeatureSetBuilder
        .Create($"FeatureSet{Name}AnalyticalMind")
        .SetGuiPresentation(Category.Feature)
        .AddToDB();

    public RangerSurvivalist()
    {
        //
        // LEVEL 03
        //

        var autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation(Category.Feature)
            .SetAutoTag("Ranger")
            .SetSpellcastingClass(CharacterClassDefinitions.Ranger)
            .SetPreparedSpellGroups(
                BuildSpellGroup(2, Entangle),
                BuildSpellGroup(5, SpellsContext.Web),
                BuildSpellGroup(9, SleetStorm),
                BuildSpellGroup(13, GreaterInvisibility),
                BuildSpellGroup(17, HoldMonster))
            .AddToDB();

        // Wandering Outcast

        var proficiencyWanderingOutcast = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}WanderingOutcast")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Skill, SkillDefinitions.Survival)
            .AddToDB();

        // Disabling Strike

        var conditionDisablingStrike = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionHindered_By_Frost, $"Condition{Name}DisablingStrike")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetParentCondition(ConditionDefinitions.ConditionHindered)
            .SetPossessive()
            .AddToDB();

        conditionDisablingStrike.specialDuration = false;

        // kept name for backward compatibility
        var additionalDamageDisablingStrike = FeatureDefinitionPowerBuilder
            .Create($"AdditionalDamage{Name}DisablingStrike")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnAttackHitAuto, RechargeRate.TurnStart)
            .AddCustomSubFeatures(ValidatorsRestrictedContext.IsWeaponOrUnarmedAttack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionDisablingStrike, ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                            .Build())
                    .Build())
            .AddToDB();

        //
        // LEVEL 07
        //

        // Analytical Mind

        var abilityCheckAnalyticalMind = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create($"AbilityCheck{Name}AnalyticalMind")
            .SetGuiPresentation($"FeatureSet{Name}AnalyticalMind", Category.Feature)
            .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.Advantage, DieType.D1, 0,
                (AttributeDefinitions.Wisdom, SkillDefinitions.Survival))
            .AddToDB();

        FeatureSetAnalyticalMind.FeatureSet.Add(abilityCheckAnalyticalMind);

        //
        // LEVEL 11
        //

        // Improved Disabling Strike

        var attributeModifierImprovedDisablingStrike = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}ImprovedDisablingStrike")
            .SetGuiPresentation($"Condition{Name}ImprovedDisablingStrike", Category.Condition)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, -2)
            .AddToDB();

        var conditionImprovedDisablingStrike = ConditionDefinitionBuilder
            .Create(conditionDisablingStrike, $"Condition{Name}ImprovedDisablingStrike")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetParentCondition(ConditionDefinitions.ConditionHindered)
            .AddFeatures(attributeModifierImprovedDisablingStrike)
            .AddToDB();

        // kept name for backward compatibility
        var powerImprovedDisablingStrike = FeatureDefinitionPowerBuilder
            .Create($"AdditionalDamage{Name}ImprovedDisablingStrike")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnAttackHitAuto)
            .AddCustomSubFeatures(ValidatorsRestrictedContext.IsWeaponOrUnarmedAttack)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionImprovedDisablingStrike, ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                            .Build())
                    .Build())
            .SetOverriddenPower(additionalDamageDisablingStrike)
            .AddToDB();

        //
        // LEVEL 15
        //

        // Unmatched Experience

        var savingThrowAffinityUnmatchedExperience = FeatureDefinitionSavingThrowAffinityBuilder
            .Create($"SavingThrowAffinity{Name}UnmatchedExperience")
            .SetGuiPresentation(Category.Feature)
            .SetAffinities(CharacterSavingThrowAffinity.ProficiencyBonusOrPlus1, false, AttributeDefinitions.Wisdom)
            .AddToDB();

        //
        // MAIN
        //

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.RangerSurvivalist, 256))
            .AddFeaturesAtLevel(3,
                autoPreparedSpells,
                additionalDamageDisablingStrike,
                proficiencyWanderingOutcast)
            .AddFeaturesAtLevel(7,
                FeatureSetAnalyticalMind)
            .AddFeaturesAtLevel(11,
                powerImprovedDisablingStrike)
            .AddFeaturesAtLevel(15,
                savingThrowAffinityUnmatchedExperience,
                FeatureDefinitionFeatureSets.AdditionalDamageRangerFavoredEnemyChoice)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Ranger;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRangerArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    internal static void LateLoad()
    {
        FeatureSetAnalyticalMind.FeatureSet.Add(
            GetDefinition<FeatureDefinitionProficiency>("ProficiencyFeatExecutioner"));
    }
}
