using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
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
            .AddCustomSubFeatures(
                ValidatorsRestrictedContext.IsWeaponOrUnarmedAttack,
                ForcePowerUseInSpendPowerAction.Marker)
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
            .AddCustomSubFeatures(
                ValidatorsRestrictedContext.IsWeaponOrUnarmedAttack)
            .AddToDB();

        //
        // LEVEL 15
        //

        // Unmatched Experience

        // kept for backward compatibility
        _ = FeatureDefinitionSavingThrowAffinityBuilder
            .Create($"SavingThrowAffinity{Name}UnmatchedExperience")
            .SetGuiPresentation(Category.Feature)
            .SetAffinities(CharacterSavingThrowAffinity.ProficiencyBonusOrPlus1, false, AttributeDefinitions.Wisdom)
            .AddToDB();

        var featureSetBlessingWilderness = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}BlessingWilderness")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                FeatureDefinitionConditionAffinityBuilder
                    .Create($"ConditionAffinity{Name}DeafenedImmunity")
                    .SetGuiPresentationNoContent(true)
                    .SetConditionType(ConditionDefinitions.ConditionDeafened)
                    .SetConditionAffinityType(ConditionAffinityType.Immunity)
                    .AddCustomSubFeatures(new TryAlterOutcomeAttackBlessingWilderness())
                    .AddToDB(),
                FeatureDefinitionConditionAffinitys.ConditionAffinityBlindnessImmunity,
                FeatureDefinitionConditionAffinitys.ConditionAffinityFrightenedImmunity,
                FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity)
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
                featureSetBlessingWilderness,
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

    private sealed class TryAlterOutcomeAttackBlessingWilderness : ITryAlterOutcomeAttack
    {
        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager instance,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier)
        {
            if (action.AttackRollOutcome is not RollOutcome.CriticalSuccess ||
                helper != defender ||
                !defender.CanPerceiveTarget(attacker) ||
                defender.RulesetCharacter.HasConditionOfTypeOrSubType(ConditionIncapacitated))
            {
                yield break;
            }

            action.AttackRoll--;
            action.AttackRollOutcome = RollOutcome.Success;
        }
    }
}
