using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class WizardGraviturgist : AbstractSubclass
{
    private const string Name = "WizardGraviturgist";

    internal WizardGraviturgist()
    {
        //
        // Density Increase
        //

        const string POWER_DENSITY_INCREASE = $"Power{Name}DensityIncrease";

        var conditionDensityIncrease = ConditionDefinitionBuilder
            .Create(ConditionHindered, $"Condition{Name}DensityIncrease")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetPossessive()
            .SetSilent(Silent.None)
            .AddFeatures(
                FeatureDefinitionMovementAffinityBuilder
                    .Create($"MovementAffinity{Name}DensityIncrease")
                    .SetGuiPresentation(POWER_DENSITY_INCREASE, Category.Feature)
                    .SetBaseSpeedAdditiveModifier(-2)
                    .AddToDB(),
                FeatureDefinitionAbilityCheckAffinityBuilder
                    .Create($"AbilityCheckAffinity{Name}DensityIncrease")
                    .SetGuiPresentation(POWER_DENSITY_INCREASE, Category.Feature)
                    .BuildAndSetAffinityGroups(
                        CharacterAbilityCheckAffinity.Advantage,
                        DieType.D8, 0,
                        (AttributeDefinitions.Strength, SkillDefinitions.Athletics))
                    .AddToDB(),
                FeatureDefinitionSavingThrowAffinityBuilder
                    .Create($"SavingThrowAffinity{Name}DensityIncrease")
                    .SetGuiPresentation(POWER_DENSITY_INCREASE, Category.Feature)
                    .SetAffinities(CharacterSavingThrowAffinity.Advantage, false, AttributeDefinitions.Strength)
                    .AddToDB())
            .AddToDB();

        var powerDensityIncrease = FeatureDefinitionPowerBuilder
            .Create(POWER_DENSITY_INCREASE)
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Bane)
            .SetUsesAbilityBonus(ActivationTime.Action, RechargeRate.ShortRest, AttributeDefinitions.Intelligence)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.Individuals)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionDensityIncrease, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .SetShowCasting(true)
            .AddToDB();

        //
        // Density Decrease
        //

        const string POWER_DENSITY_DECREASE = $"Power{Name}DensityDecrease";

        var conditionDensityDecrease = ConditionDefinitionBuilder
            .Create(ConditionJump, $"Condition{Name}DensityDecrease")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetPossessive()
            .SetSilent(Silent.None)
            .AddFeatures(
                FeatureDefinitionMovementAffinityBuilder
                    .Create($"MovementAffinity{Name}DensityDecrease")
                    .SetGuiPresentation(POWER_DENSITY_DECREASE, Category.Feature)
                    .SetBaseSpeedAdditiveModifier(2)
                    .AddToDB(),
                FeatureDefinitionAbilityCheckAffinityBuilder
                    .Create($"AbilityCheckAffinity{Name}DensityDecrease")
                    .SetGuiPresentation(POWER_DENSITY_DECREASE, Category.Feature)
                    .BuildAndSetAffinityGroups(
                        CharacterAbilityCheckAffinity.Disadvantage,
                        DieType.D8, 0,
                        (AttributeDefinitions.Strength, SkillDefinitions.Athletics))
                    .AddToDB(),
                FeatureDefinitionSavingThrowAffinityBuilder
                    .Create($"SavingThrowAffinity{Name}DensityDecrease")
                    .SetGuiPresentation(POWER_DENSITY_DECREASE, Category.Feature)
                    .SetAffinities(CharacterSavingThrowAffinity.Disadvantage, false, AttributeDefinitions.Strength)
                    .AddToDB())
            .AddToDB();

        var powerDensityDecrease = FeatureDefinitionPowerBuilder
            .Create(POWER_DENSITY_DECREASE)
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Bless)
            .SetUsesAbilityBonus(ActivationTime.Action, RechargeRate.ShortRest, AttributeDefinitions.Intelligence)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.Individuals)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionDensityDecrease, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .SetShowCasting(true)
            .AddToDB();

        conditionDensityIncrease.CancellingConditions.Add(conditionDensityDecrease);
        conditionDensityDecrease.CancellingConditions.Add(conditionDensityIncrease);

        //
        // Gravity Well
        //

        var powerGravityWell = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}GravityWell")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnAttackSpellHitAutomatic)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Instantaneous)
                    .AddEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 1)
                            .Build())
                    .Build())
            .AddToDB();

        //
        // Violent Attraction
        //

        var conditionViolentAttraction = ConditionDefinitionBuilder
            .Create($"Condition{Name}ViolentAttraction")
            .SetOrUpdateGuiPresentation(Category.Condition, ConditionDivineFavor)
            .SetFeatures(
                FeatureDefinitionAdditionalDamageBuilder
                    .Create($"AdditionalDamage{Name}ViolentAttraction")
                    .SetGuiPresentationNoContent(true)
                    .SetNotificationTag("ViolentAttraction")
                    .SetDamageDice(DieType.D10, 1)
                    .SetFrequencyLimit(FeatureLimitedUsage.OnceInMyTurn)
                    .SetRequiredProperty(RestrictedContextRequiredProperty.MeleeWeapon)
                    .SetIgnoreCriticalDoubleDice(true)
                    .AddToDB())
            .AddToDB();

        var powerViolentAttraction = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ViolentAttraction")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.BrandingSmite)
            .SetUsesAbilityBonus(ActivationTime.BonusAction, RechargeRate.LongRest, AttributeDefinitions.Intelligence)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.Individuals)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionViolentAttraction, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .SetShowCasting(true)
            .AddToDB();

        //
        // Event Horizon
        //

        var conditionEventHorizon = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionProne, $"Condition{Name}EventHorizon")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetPossessive()
            .SetSilent(Silent.None)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .SetFeatures(
                FeatureDefinitionMovementAffinityBuilder
                    .Create($"MovementAffinity{Name}EventHorizon")
                    .SetGuiPresentationNoContent(true)
                    .SetBaseSpeedMultiplicativeModifier(0)
                    .AddToDB())
            .AddToDB();

        var conditionEventHorizonSaved = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionProne, $"Condition{Name}EventHorizonSaved")
            .SetOrUpdateGuiPresentation($"Condition{Name}EventHorizon", Category.Condition)
            .SetPossessive()
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .SetCancellingConditions(conditionEventHorizon)
            .SetFeatures(
                FeatureDefinitionMovementAffinityBuilder
                    .Create($"MovementAffinity{Name}EventHorizonSaved")
                    .SetGuiPresentationNoContent(true)
                    .SetBaseSpeedMultiplicativeModifier(0.5f)
                    .AddToDB())
            .AddToDB();

        var conditionEventHorizonSelf = ConditionDefinitionBuilder
            .Create(ConditionSpiritGuardiansSelf, $"Condition{Name}EventHorizonSelf")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var powerEventHorizon = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}EventHorizon")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.DispelEvilAndGood)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetParticleEffectParameters(SpellDefinitions.Darkness)
                    .SetSavingThrowData(false, AttributeDefinitions.Strength, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetRecurrentEffect(RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeForce, 2, DieType.D10)
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionEventHorizonSaved, ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.None)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionEventHorizon, ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionEventHorizonSelf, ConditionForm.ConditionOperation.Add, true)
                            .Build())
                    .Build())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.WizardGravityMage, 256))
            .AddFeaturesAtLevel(2, powerDensityIncrease, powerDensityDecrease)
            .AddFeaturesAtLevel(6, powerGravityWell)
            .AddFeaturesAtLevel(10, powerViolentAttraction)
            .AddFeaturesAtLevel(14, powerEventHorizon)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }
}
