using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class WizardGraviturgist : AbstractSubclass
{
    private const string Name = "WizardGraviturgist";

    public WizardGraviturgist()
    {
        // LEVEL 02

        // Density Pool

        var powerDensity = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Density")
            .SetGuiPresentationNoContent(true)
            .SetUsesAbilityBonus(ActivationTime.Action, RechargeRate.ShortRest, AttributeDefinitions.Intelligence)
            .AddToDB();

        // Density Increase

        const string POWER_DENSITY_INCREASE = $"Power{Name}DensityIncrease";

        var conditionDensityIncrease = ConditionDefinitionBuilder
            .Create($"Condition{Name}DensityIncrease")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHeavilyEncumbered)
            .SetPossessive()
            .CopyParticleReferences(PowerBerserkerFrenzy)
            .SetFeatures(
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

        var powerDensityIncrease = FeatureDefinitionPowerSharedPoolBuilder
            .Create(POWER_DENSITY_INCREASE)
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Heroism)
            .SetSharedPool(ActivationTime.Action, powerDensity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionDensityIncrease, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(PowerSpellBladeSpellTyrant)
                    .Build())
            .SetShowCasting(true)
            .AddToDB();

        // Density Decrease

        const string POWER_DENSITY_DECREASE = $"Power{Name}DensityDecrease";

        var conditionDensityDecrease = ConditionDefinitionBuilder
            .Create($"Condition{Name}DensityDecrease")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionFlying)
            .SetPossessive()
            .CopyParticleReferences(ConditionMonkSlowFall)
            .SetFeatures(
                FeatureDefinitionMovementAffinityBuilder
                    .Create($"MovementAffinity{Name}DensityDecrease")
                    .SetGuiPresentation(POWER_DENSITY_DECREASE, Category.Feature, Gui.NoLocalization)
                    .SetBaseSpeedAdditiveModifier(2)
                    .SetAdditionalJumpCells(2, true)
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

        var powerDensityDecrease = FeatureDefinitionPowerSharedPoolBuilder
            .Create(POWER_DENSITY_DECREASE)
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Bless)
            .SetSharedPool(ActivationTime.Action, powerDensity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionDensityDecrease, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetCasterEffectParameters(PowerTraditionCourtMageExpandedSpellShield)
                    .SetEffectEffectParameters(PowerSorcererManaPainterDrain)
                    .Build())
            .SetShowCasting(true)
            .AddToDB();

        conditionDensityIncrease.cancellingConditions.Add(conditionDensityDecrease);
        conditionDensityDecrease.cancellingConditions.Add(conditionDensityIncrease);

        // LEVEL 06

        // Gravity Well

        var actionAffinityGravityWellToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityGravityWellToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.GravityWellToggle)
            .AddToDB();
        
        var powerGravityWell = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}GravityWell")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.OnAttackOrSpellHitAuto)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.IndividualsUnique)
                    .SetParticleEffectParameters(SpellDefinitions.EldritchBlast)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 1)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(new ValidatorsValidatePowerUse(c =>
                    c.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.OverChannelToggle)))
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
                    .AddCustomSubFeatures(
                        new ValidateContextInsteadOfRestrictedProperty((_, _, _, _, _, mode, _) =>
                            (OperationType.Set, ValidatorsWeapon.IsMeleeOrUnarmed(mode))))
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
                    .SetCasterEffectParameters(PowerSorcererChildRiftOffering)
                    .SetEffectEffectParameters(PowerMagebaneSpellCrusher)
                    .Build())
            .SetShowCasting(true)
            .AddToDB();

        // LEVEL 14

        // Event Horizon

        var conditionEventHorizon = ConditionDefinitionBuilder
            .Create($"Condition{Name}EventHorizon")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionRestrained)
            .SetPossessive()
            .SetConditionType(ConditionType.Neutral)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .CopyParticleReferences(ConditionDefinitions.ConditionRestrained)
            .SetFeatures(
                FeatureDefinitionMovementAffinityBuilder
                    .Create($"MovementAffinity{Name}EventHorizon")
                    .SetGuiPresentation($"Condition{Name}EventHorizon", Category.Condition, Gui.NoLocalization)
                    .SetBaseSpeedMultiplicativeModifier(0)
                    .AddToDB())
            .AddToDB();

        var conditionEventHorizonSaved = ConditionDefinitionBuilder
            .Create($"Condition{Name}EventHorizonSaved")
            .SetGuiPresentation($"Condition{Name}EventHorizon", Category.Condition, ConditionSlowed)
            .SetPossessive()
            .SetConditionType(ConditionType.Neutral)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .CopyParticleReferences(ConditionSlowed)
            .SetCancellingConditions(conditionEventHorizon)
            .SetFeatures(
                FeatureDefinitionMovementAffinityBuilder
                    .Create($"MovementAffinity{Name}EventHorizonSaved")
                    .SetGuiPresentation($"Condition{Name}EventHorizon", Category.Condition, Gui.NoLocalization)
                    .SetBaseSpeedMultiplicativeModifier(0.5f)
                    .AddToDB())
            .AddToDB();

        var conditionEventHorizonSelf = ConditionDefinitionBuilder
            .Create($"Condition{Name}EventHorizonSelf")
            .SetGuiPresentation(Category.Condition, ConditionLightSensitiveSorakSaboteur)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .CopyParticleReferences(ConditionSpiritGuardiansSelf)
            .SetFeatures()
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
                            .Build())
                    .SetCasterEffectParameters(PowerGreen_Hag_Invisibility)
                    .Build())
            .AddToDB();

        powerEventHorizon.AddCustomSubFeatures(new PowerOrSpellInitiatedByMeEventHorizon(conditionEventHorizonSelf));

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.WizardGravityMage, 256))
            .AddFeaturesAtLevel(2, powerDensity, powerDensityIncrease, powerDensityDecrease)
            .AddFeaturesAtLevel(6, actionAffinityGravityWellToggle, powerGravityWell)
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

    private sealed class PowerOrSpellInitiatedByMeEventHorizon(ConditionDefinition conditionSelfSFX)
        : IPowerOrSpellInitiatedByMe
    {
        public IEnumerator OnPowerOrSpellInitiatedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;

            rulesetCharacter.InflictCondition(
                conditionSelfSFX.Name,
                DurationType.Minute,
                1,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                conditionSelfSFX.Name,
                0,
                0,
                0);

            yield break;
        }
    }
}
