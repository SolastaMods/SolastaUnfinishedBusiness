using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class WizardGraviturgist : AbstractSubclass
{
    private const string Name = "WizardGraviturgist";

    public WizardGraviturgist()
    {
        // LEVEL 02

        // Density Increase

        const string POWER_DENSITY_INCREASE = $"Power{Name}DensityIncrease";

        var conditionDensityIncrease = ConditionDefinitionBuilder
            .Create(ConditionHindered, $"Condition{Name}DensityIncrease")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetPossessive()
            .SetSilent(Silent.None)
            .CopyParticleReferences(ConditionSlowed)
            .AddFeatures(
                FeatureDefinitionMovementAffinityBuilder
                    .Create($"MovementAffinity{Name}DensityIncrease")
                    .SetGuiPresentation(POWER_DENSITY_INCREASE, Category.Feature, Gui.NoLocalization)
                    .SetBaseSpeedAdditiveModifier(-2)
                    .AddToDB(),
                FeatureDefinitionAbilityCheckAffinityBuilder
                    .Create($"AbilityCheckAffinity{Name}DensityIncrease")
                    .SetGuiPresentation(POWER_DENSITY_INCREASE, Category.Feature, Gui.NoLocalization)
                    .BuildAndSetAffinityGroups(
                        CharacterAbilityCheckAffinity.Advantage,
                        abilityProficiencyPairs: (AttributeDefinitions.Strength, SkillDefinitions.Athletics))
                    .AddToDB(),
                FeatureDefinitionSavingThrowAffinityBuilder
                    .Create($"SavingThrowAffinity{Name}DensityIncrease")
                    .SetGuiPresentation(POWER_DENSITY_INCREASE, Category.Feature, Gui.NoLocalization)
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
                    .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetParticleEffectParameters(PowerSpellBladeSpellTyrant)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionDensityIncrease, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .SetShowCasting(true)
            .AddToDB();

        // Density Decrease

        const string POWER_DENSITY_DECREASE = $"Power{Name}DensityDecrease";

        var conditionDensityDecrease = ConditionDefinitionBuilder
            .Create(ConditionJump, $"Condition{Name}DensityDecrease")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetPossessive()
            .SetSilent(Silent.None)
            .CopyParticleReferences(ConditionSlowed)
            .AddFeatures(
                FeatureDefinitionMovementAffinityBuilder
                    .Create($"MovementAffinity{Name}DensityDecrease")
                    .SetGuiPresentation(POWER_DENSITY_DECREASE, Category.Feature, Gui.NoLocalization)
                    .SetBaseSpeedAdditiveModifier(2)
                    .AddToDB(),
                FeatureDefinitionAbilityCheckAffinityBuilder
                    .Create($"AbilityCheckAffinity{Name}DensityDecrease")
                    .SetGuiPresentation(POWER_DENSITY_DECREASE, Category.Feature, Gui.NoLocalization)
                    .BuildAndSetAffinityGroups(
                        CharacterAbilityCheckAffinity.Disadvantage,
                        abilityProficiencyPairs: (AttributeDefinitions.Strength, SkillDefinitions.Athletics))
                    .AddToDB(),
                FeatureDefinitionSavingThrowAffinityBuilder
                    .Create($"SavingThrowAffinity{Name}DensityDecrease")
                    .SetGuiPresentation(POWER_DENSITY_DECREASE, Category.Feature, Gui.NoLocalization)
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
                    .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetParticleEffectParameters(PowerRoguishHoodlumDirtyFighting)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionDensityDecrease, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .SetShowCasting(true)
            .AddToDB();

        conditionDensityIncrease.cancellingConditions.Add(conditionDensityDecrease);
        conditionDensityDecrease.cancellingConditions.Add(conditionDensityIncrease);

        // LEVEL 06

        // Gravity Well

        var powerGravityWell = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}GravityWell")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnAttackOrSpellHitAuto)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.IndividualsUnique)
                    .SetParticleEffectParameters(SpellDefinitions.EldritchBlast)
                    .AddEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 1)
                            .Build())
                    .Build())
            .AddToDB();

        // LEVEL 10

        // Violent Attraction

        var conditionViolentAttraction = ConditionDefinitionBuilder
            .Create($"Condition{Name}ViolentAttraction")
            .SetOrUpdateGuiPresentation(Category.Condition, ConditionDivineFavor)
            .SetPossessive()
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
                    .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetParticleEffectParameters(SpellDefinitions.MoonBeam)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionViolentAttraction, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .SetShowCasting(true)
            .AddToDB();

        // LEVEL 14

        // Event Horizon

        var conditionEventHorizon = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionProne, $"Condition{Name}EventHorizon")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetPossessive()
            .SetSilent(Silent.None)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .CopyParticleReferences(ConditionHitByDirtyFighting)
            .SetFeatures(
                FeatureDefinitionMovementAffinityBuilder
                    .Create($"MovementAffinity{Name}EventHorizon")
                    .SetGuiPresentation($"Condition{Name}EventHorizon", Category.Condition, Gui.NoLocalization)
                    .SetBaseSpeedMultiplicativeModifier(0)
                    .AddToDB())
            .AddToDB();

        var conditionEventHorizonSaved = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionProne, $"Condition{Name}EventHorizonSaved")
            .SetOrUpdateGuiPresentation($"Condition{Name}EventHorizon", Category.Condition)
            .SetPossessive()
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .CopyParticleReferences(ConditionHitByDirtyFighting)
            .SetCancellingConditions(conditionEventHorizon)
            .SetFeatures(
                FeatureDefinitionMovementAffinityBuilder
                    .Create($"MovementAffinity{Name}EventHorizonSaved")
                    .SetGuiPresentation($"Condition{Name}EventHorizon", Category.Condition, Gui.NoLocalization)
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

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Wizard;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }
}
