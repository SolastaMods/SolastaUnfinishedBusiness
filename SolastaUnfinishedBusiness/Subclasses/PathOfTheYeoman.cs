using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class PathOfTheYeoman : AbstractSubclass
{
    internal const string Name = "PathOfTheYeoman";

    internal PathOfTheYeoman()
    {
        // LEVEL 03

        // Fletcher

        var proficiencyFletcher = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}Fletcher")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.ToolOrExpertise, ToolDefinitions.ArtisanToolType)
            .AddToDB();

        // Strong Bow

        var attackModifierStrongBow = FeatureDefinitionAttackModifierBuilder
            .Create($"AttackModifier{Name}StrongBow")
            .SetGuiPresentation(Category.Feature)
            .SetDamageRollModifier(1)
            .SetCustomSubFeatures(
                new RestrictedContextValidator((_, _, character, _, _, _, _) =>
                    (OperationType.Set, ValidatorsCharacter.HasLongbow(character))),
                new CanUseAttribute(AttributeDefinitions.Strength,
                    (_, _, character) => ValidatorsCharacter.HasLongbow(character)))
            .AddToDB();

        // LEVEL 06

        // Staggering Blow

        var actionAffinityStaggeringBlow = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Name}StaggeringBlow")
            .SetGuiPresentation(Category.Feature)
            .SetAllowedActionTypes()
            .SetAuthorizedActions(ActionDefinitions.Id.ShoveBonus)
            .SetCustomSubFeatures(
                new ValidatorsDefinitionApplication(
                    ValidatorsCharacter.HasLongbow,
                    ValidatorsCharacter.HasAnyOfConditions(ConditionRaging)))
            .AddToDB();

        // Keen Eye

        var additionalDamageKeenEye = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}KeenEye")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("BrutalCritical")
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.BrutalCriticalDice)
            .SetRequiredProperty(RestrictedContextRequiredProperty.RangeWeapon)
            .SetTriggerCondition(AdditionalDamageTriggerCondition.CriticalHit)
            .AddToDB();

        FeatureDefinitionCombatAffinitys.CombatAffinityReckless.situationalContext = (SituationalContext)
            ExtraSituationalContext.AttackerNextToTargetOrYeomanWithLongbow;

        // LEVEL 10

        // Bulwark

        var movementAffinityBulwark = FeatureDefinitionMovementAffinityBuilder
            .Create($"MovementAffinity{Name}Bulwark")
            .SetGuiPresentation(Category.Feature)
            .SetBaseSpeedMultiplicativeModifier(0)
            .AddToDB();

        var featureBulwark = FeatureDefinitionBuilder
            .Create($"Feature{Name}Bulwark")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(
                new RangedAttackInMeleeDisadvantageRemover(
                    (_, _, character) => ValidatorsCharacter.HasLongbow(character)),
                new CanMakeAoOOnReachEntered
                {
                    WeaponValidator = (_, _, character) => ValidatorsCharacter.HasLongbow(character)
                })
            .AddToDB();

        var combatAffinityBulwark = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}Bulwark")
            .SetGuiPresentation(Category.Feature)
            .SetSituationalContext(SituationalContext.AttackerAwayFromTarget)
            .SetAttackOnMeAdvantage(AdvantageType.Disadvantage)
            .AddToDB();

        var conditionBulwark = ConditionDefinitionBuilder
            .Create($"Condition{Name}Bulwark")
            .SetGuiPresentation(Category.Condition)
            .SetPossessive()
            .AddFeatures(
                movementAffinityBulwark,
                featureBulwark,
                combatAffinityBulwark)
            .AddToDB();

        var powerBulwark = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Bulwark")
            .SetGuiPresentation(Category.Feature)
            .SetUsesAbilityBonus(ActivationTime.BonusAction, RechargeRate.LongRest, AttributeDefinitions.Constitution)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionBulwark, ConditionForm.ConditionOperation.Add, true)
                            .Build())
                    .Build())
            .AddToDB();

        // LEVEL 14

        // Mighty Shot

        var featureMightyShot = FeatureDefinitionBuilder
            .Create($"Feature{Name}MightyShot")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(
                new UpgradeWeaponDice((_, damage) => (damage.diceNumber, DieType.D6, DieType.D10),
                    (_, _, character) => ValidatorsCharacter.HasLongbow(character)))
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.PathOfTheYeoman, 256))
            .AddFeaturesAtLevel(3, proficiencyFletcher, attackModifierStrongBow)
            .AddFeaturesAtLevel(6, actionAffinityStaggeringBlow, additionalDamageKeenEye)
            .AddFeaturesAtLevel(10, powerBulwark)
            .AddFeaturesAtLevel(14, featureMightyShot)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBarbarianPrimalPath;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }
}
