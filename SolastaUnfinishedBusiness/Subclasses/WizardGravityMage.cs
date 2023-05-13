using System.Drawing;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class WizardGravityMage : AbstractSubclass
{
    internal WizardGravityMage()
    {
        var adjustDensityIncrease = CreateAdjustDensityIncrease();
        var adjustDensityDecrease = CreateAdjustDensityDecrease();
        var gravityWell = CreateGravityWell();
        var violentAttraction = CreateViolentAttraction();
        var eventHorizon = CreateEventHorizon();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("WizardGravityMage")
            .SetGuiPresentation("TraditionWizardGravityMage", Category.Subclass,
                Sprites.GetSprite("TraditionWizardGravityMage", Resources.WizardGravityMage, 256))
            .AddFeaturesAtLevel(2, adjustDensityIncrease, adjustDensityDecrease)
            .AddFeaturesAtLevel(6, gravityWell)
            .AddFeaturesAtLevel(10, violentAttraction)
            .AddFeaturesAtLevel(14, eventHorizon)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;
    internal override DeityDefinition DeityDefinition { get; }

    //Create AdjustDensity Powers
    private static FeatureDefinitionPower CreateAdjustDensityIncrease()
    {
        var AdjustDensityHeavyEffectForm = EffectFormBuilder
            .Create()
            .SetConditionForm(CreateConditionAdjustDensityHeavy(), ConditionForm.ConditionOperation.Add)
            .Build();

        var AdjustDensityHeavyEffectDescription = EffectDescriptionBuilder
            .Create()
            //.SetCreatedByCharacter()
            .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.Individuals)
            .SetDurationData(DurationType.Minute, 1)
            .SetNoSavingThrow()
            .SetEffectForms(AdjustDensityHeavyEffectForm)
            .Build();

        var AdjustDensityHeavy = FeatureDefinitionPowerBuilder
            .Create("WizardGravityMageAdjustDensityIncrease")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Bane.GuiPresentation.SpriteReference)
            .SetUsesAbilityBonus(ActivationTime.Action, RechargeRate.AtWill, AttributeDefinitions.Intelligence)
            .SetEffectDescription(AdjustDensityHeavyEffectDescription)
            .SetShowCasting(true) //Casting flourish.
            .AddToDB();
        return AdjustDensityHeavy;

        //--------

        static ConditionDefinition CreateConditionAdjustDensityHeavy()
        {
            var speeddebuff = FeatureDefinitionMovementAffinityBuilder
                .Create("WizardGravityMageSpeedDecrease")
                .SetBaseSpeedAdditiveModifier(-2)
                .AddToDB();

            var heavyskill = FeatureDefinitionAbilityCheckAffinityBuilder
                .Create("WizardGravityMageAthleticsIncrease")
                .BuildAndSetAffinityGroups(
                    CharacterAbilityCheckAffinity.Advantage,
                    DieType.D8, 0,
                    (AttributeDefinitions.Strength, SkillDefinitions.Athletics)
                )
                .AddToDB();

            var heavysave = FeatureDefinitionSavingThrowAffinityBuilder
                .Create("WizardGravityMageStrengthIncrease")
                .SetAffinities(CharacterSavingThrowAffinity.Advantage, false, AttributeDefinitions.Strength)
                .AddToDB();

            return ConditionDefinitionBuilder
                .Create(ConditionHindered, "HigherDensity")
                .SetOrUpdateGuiPresentation("ConditionHeavy", Category.Condition)
                .AddFeatures(
                    speeddebuff,
                    heavyskill,
                    heavysave
                )
                .SetSpecialDuration(DurationType.Minute, 1)
                .AddToDB();
        }
    }

    private static FeatureDefinitionPower CreateAdjustDensityDecrease()
    {
        var AdjustDensityLightEffectForm = EffectFormBuilder
            .Create()
            .SetConditionForm(CreateConditionAdjustDensityLight(), ConditionForm.ConditionOperation.Add)
            .Build();

        var AdjustDensityLightEffectDescription = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.Individuals)
            .SetDurationData(DurationType.Minute, 1)
            .SetNoSavingThrow()
            .SetEffectForms(AdjustDensityLightEffectForm)
            .Build();

        var AdjustDensityLight = FeatureDefinitionPowerBuilder
            .Create("WizardGravityMageAdjustDensityDecrease")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Bless.GuiPresentation.SpriteReference)
            .SetUsesAbilityBonus(ActivationTime.Action, RechargeRate.AtWill, AttributeDefinitions.Intelligence)
            .SetEffectDescription(AdjustDensityLightEffectDescription)
            .SetShowCasting(true) //Casting flourish.
            .AddToDB();
        return AdjustDensityLight;

        //--------

        static ConditionDefinition CreateConditionAdjustDensityLight()
        {
            var speedbuff = FeatureDefinitionMovementAffinityBuilder
                .Create("WizardGravityMageSpeedIncrease")
                .SetBaseSpeedAdditiveModifier(2)
                .AddToDB();

            var lightskill = FeatureDefinitionAbilityCheckAffinityBuilder
                .Create("WizardGravityMageAthleticsDecrease")
                .BuildAndSetAffinityGroups(
                    CharacterAbilityCheckAffinity.Disadvantage,
                    DieType.D8, 0,
                    (AttributeDefinitions.Strength, SkillDefinitions.Athletics)
                )
                .AddToDB();

            var lightsave = FeatureDefinitionSavingThrowAffinityBuilder
                .Create("WizardGravityMageStrengthDecrease")
                .SetAffinities(CharacterSavingThrowAffinity.Disadvantage, false, AttributeDefinitions.Strength)
                .AddToDB();

            return ConditionDefinitionBuilder
                .Create(ConditionJump, "LowerDensity")
                .SetOrUpdateGuiPresentation("ConditionLight", Category.Condition)
                .AddFeatures(
                    speedbuff,
                    lightskill,
                    lightsave,
                    FeatureDefinitionMovementAffinitys.MovementAffinityJump
                )
                .SetSpecialDuration(DurationType.Minute, 1)
                .AddToDB();
        }
    }

    private static FeatureDefinitionPower CreateGravityWell()
    {
        //This used to work fine in the old codebase, not sure what changed.
        var GravityWellEffectForm = EffectFormBuilder
            .Create()
            .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 1)
            .Build();

        var GravityWellEffectDescription = EffectDescriptionBuilder
            .Create()
            .AddEffectForms(GravityWellEffectForm)
            .Build();

        var GravityWell = FeatureDefinitionPowerBuilder
            .Create("WizardGravityMageGravityWell")
            .SetGuiPresentation(Category.Feature)
            .SetUsesAbilityBonus(ActivationTime.OnAttackSpellHitAutomatic, RechargeRate.AtWill, AttributeDefinitions.Intelligence)
            .SetEffectDescription(GravityWellEffectDescription)
            .AddToDB();
        return GravityWell;
    }

    private static FeatureDefinitionPower CreateViolentAttraction()
    {
        var ViolentAttractionEffectForm = EffectFormBuilder
            .Create()
            .SetConditionForm(CreateConditionViolentAttraction(), ConditionForm.ConditionOperation.Add)
            .Build();

        var ViolentAttractionEffectDescription = EffectDescriptionBuilder
            .Create()
            .SetDurationData(DurationType.Minute, 1)
            .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.Individuals)
            .SetNoSavingThrow()
            .SetEffectForms(ViolentAttractionEffectForm)
            .Build();

        var ViolentAttraction = FeatureDefinitionPowerBuilder
            .Create("WizardGravityMageViolentAttraction")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.BrandingSmite.GuiPresentation.SpriteReference)
            .SetUsesAbilityBonus(ActivationTime.BonusAction, RechargeRate.LongRest, AttributeDefinitions.Intelligence)
            .SetEffectDescription(ViolentAttractionEffectDescription)
            .SetShowCasting(true) //Casting flourish.
            .AddToDB();
        return ViolentAttraction;

        static ConditionDefinition CreateConditionViolentAttraction()
        {
            var violentattractionbuff = FeatureDefinitionAdditionalDamageBuilder
                .Create("WizardGravityMageViolentAttractionBuff")
                .SetDamageValueDetermination(AdditionalDamageValueDetermination.Die)
                .SetDamageDice(DieType.D10, 1)
                .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
                .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
                .AddToDB();

            return ConditionDefinitionBuilder
                .Create("ViolentAttraction")
                .SetGuiPresentation(Category.Condition, ConditionDivineFavor.GuiPresentation.SpriteReference)
                .SetOrUpdateGuiPresentation("ConditionViolentAttraction", Category.Condition)
                .SetFeatures(violentattractionbuff)
                .SetSpecialDuration(DurationType.Minute, 1)
                .AddToDB();
        }
    }

    private static FeatureDefinitionPower CreateEventHorizon()
        //Functional, somewhat awkward without accurate visual effects
        //Some way to scale up Spirit Guardians or Globe of Invulnerability VFX?
        //Ideally would have Concentration too.
    {
        //Damage Componenent
        var EventHorizonDamageEffectForm = EffectFormBuilder
            .Create()
            .SetDamageForm(DamageTypeForce, 2, DieType.D10)
            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
            .Build();

        //Immobilizing Componenent
        var EventHorizonSlowEffectForm = EffectFormBuilder
            .Create()
            .SetConditionForm(CreateConditionEventHorizon(), ConditionForm.ConditionOperation.Add)
            .HasSavingThrow(EffectSavingThrowType.Negates)
            .Build();

        //Condition to center on self
        var EventHorizonSelfEffectForm = EffectFormBuilder
            .Create()
            .SetConditionForm(CreateConditionEventHorizonSelf(), ConditionForm.ConditionOperation.Add, true, true)
            .Build();

        //Combine components
        var EventHorizonEffectDescription = EffectDescriptionBuilder
            .Create()
            .SetParticleEffectParameters(SpellDefinitions.Darkness) //On-Cast Visual Effects line
            .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
            .SetDurationData(DurationType.Minute, 1)
            .SetSavingThrowData(
                false,
                AttributeDefinitions.Strength,
                true,
                EffectDifficultyClassComputation.SpellCastingFeature)
            .SetEffectForms(EventHorizonDamageEffectForm, EventHorizonSlowEffectForm, EventHorizonSelfEffectForm)
            .SetRecurrentEffect(RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
            .Build();

        //Condition icon, activation/recharge parameters, call combined Effect
        var EventHorizon = FeatureDefinitionPowerBuilder
            .Create("WizardGravityMageEventHorizon")
            .SetGuiPresentation(Category.Action, SpellDefinitions.DispelEvilAndGood.GuiPresentation.SpriteReference)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(EventHorizonEffectDescription)
            .AddToDB();
        return EventHorizon;

        static ConditionDefinition CreateConditionEventHorizon()
        {
            var EventHorizonSlow = FeatureDefinitionMovementAffinityBuilder
                .Create("EventHorizonSlow")
                .SetBaseSpeedMultiplicativeModifier(0)
                .AddToDB();

            return ConditionDefinitionBuilder
                .Create("EventHorizon")
                .SetGuiPresentation(Category.Condition,
                    ConditionDefinitions.ConditionProne.GuiPresentation.SpriteReference)
                .SetOrUpdateGuiPresentation("ConditionEventHorizon", Category.Condition)
                .SetConditionType(ConditionType.Detrimental)
                .SetFeatures(EventHorizonSlow)      
                .SetSpecialDuration(DurationType.Round,0)
                .SetSilent(Silent.WhenRemoved)
                .AddToDB();
        }

        static ConditionDefinition CreateConditionEventHorizonSelf()
        {
            return ConditionDefinitionBuilder
                .Create("EventHorizonSelf")
                .SetGuiPresentation(Category.Condition, ConditionSpiritGuardiansSelf.GuiPresentation.SpriteReference)
                .SetConditionParticleReference(ConditionSpiritGuardiansSelf.conditionParticleReference)
                .SetOrUpdateGuiPresentation("ConditionEventHorizonSelf", Category.Condition)
                .SetSpecialDuration(DurationType.Minute, 1)
                .AddToDB();
        }
    }
}
