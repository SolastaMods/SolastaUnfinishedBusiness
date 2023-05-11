using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;

namespace SolastaUnfinishedBusiness.Subclasses;
internal sealed class WizardGravityMage : AbstractSubclass
{
    /*private static readonly Guid SubclassNamespace = new("6252c084-d1a8-4c56-991a-ff496314f95a");
    private readonly CharacterSubclassDefinition Subclass;
    internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
    {
        return FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions;
    }
    internal override CharacterSubclassDefinition GetSubclass()
    {
        return Subclass;
    }*/
    internal WizardGravityMage()
    {

        var AdjustDensityIncrease = CreateAdjustDensityIncrease();
        var AdjustDensityDecrease = CreateAdjustDensityDecrease();

        var GravityWell = CreateGravityWell();

        var ViolentAttraction = CreateViolentAttraction();

        var EventHorizon = CreateEventHorizon();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("WizardGravityMage")
            .SetGuiPresentation("TraditionWizardGravityMage", Category.Subclass, Sprites.GetSprite("TraditionWizardGravityMage", Resources.DomainSmith, 256))
            .AddFeaturesAtLevel(2, AdjustDensityIncrease, AdjustDensityDecrease)
            .AddFeaturesAtLevel(6, GravityWell)
            .AddFeaturesAtLevel(10, ViolentAttraction)
            .AddFeaturesAtLevel(14, EventHorizon)
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
            .SetConditionForm(CreateConditionAdjustDensityHeavy(), ConditionForm.ConditionOperation.Add, false, false)
            .Build();

        var AdjustDensityHeavyEffectDescription = EffectDescriptionBuilder
        .Create()
        //.SetCreatedByCharacter()
        .SetTargetingData(RuleDefinitions.Side.All, RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals)
        .SetDurationData(RuleDefinitions.DurationType.Minute, 1)
        .SetNoSavingThrow()
        .SetEffectForms(AdjustDensityHeavyEffectForm)
        .Build();

        var AdjustDensityHeavy = FeatureDefinitionPowerBuilder
            .Create("WizardGravityMageAdjustDensityIncrease")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Bane.GuiPresentation.SpriteReference)
            .SetUsesAbilityBonus(ActivationTime.Action, RechargeRate.AtWill, AttributeDefinitions.Intelligence)
            .SetEffectDescription(AdjustDensityHeavyEffectDescription)            
            .SetShowCasting(true)//Casting flourish.
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
                    RuleDefinitions.CharacterAbilityCheckAffinity.Advantage,
                    RuleDefinitions.DieType.D8, 0,
                    (AttributeDefinitions.Strength, SkillDefinitions.Athletics)
                    )
                .AddToDB();

            var heavysave = FeatureDefinitionSavingThrowAffinityBuilder
                .Create("WizardGravityMageStrengthIncrease")
                .SetAffinities(RuleDefinitions.CharacterSavingThrowAffinity.Advantage, false, AttributeDefinitions.Strength)
                .AddToDB();

            return ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionHindered, "HigherDensity")
            .SetOrUpdateGuiPresentation("ConditionHeavy", Category.Condition)
            .AddFeatures(
                speeddebuff,
                heavyskill,
                heavysave
                )
            //.SetAllowMultipleInstances(false) //Test if allows multiple instances at all or just on the same target
            //.SetDuration(RuleDefinitions.DurationType.Minute, 1)
            .SetSpecialDuration(DurationType.Minute,1)
            //.SetTurnOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn)
            .AddToDB();
        }
    }
    
    private static FeatureDefinitionPower CreateAdjustDensityDecrease()
    {
        var AdjustDensityLightEffectForm = EffectFormBuilder
            .Create()
            .SetConditionForm(CreateConditionAdjustDensityLight(), ConditionForm.ConditionOperation.Add, false, false)
            .Build();

    var AdjustDensityLightEffectDescription = EffectDescriptionBuilder
        .Create()
        //.SetCreatedByCharacter()
        .SetTargetingData(RuleDefinitions.Side.All, RuleDefinitions.RangeType.Distance, 6, RuleDefinitions.TargetType.Individuals)
        .SetDurationData(RuleDefinitions.DurationType.Minute, 1)
        .SetNoSavingThrow()
        .SetEffectForms(AdjustDensityLightEffectForm)
        .Build();

    var AdjustDensityLight = FeatureDefinitionPowerBuilder
        .Create("WizardGravityMageAdjustDensityDecrease")
        .SetGuiPresentation(Category.Feature, SpellDefinitions.Bless.GuiPresentation.SpriteReference)
        .SetUsesAbilityBonus(ActivationTime.Action, RechargeRate.AtWill, AttributeDefinitions.Intelligence)
        .SetEffectDescription(AdjustDensityLightEffectDescription)
        .SetShowCasting(true)//Casting flourish.
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
                    RuleDefinitions.CharacterAbilityCheckAffinity.Disadvantage,
                    RuleDefinitions.DieType.D8, 0,
                                (AttributeDefinitions.Strength, SkillDefinitions.Athletics)
                            )
                .AddToDB();

            var lightsave = FeatureDefinitionSavingThrowAffinityBuilder
                .Create("WizardGravityMageStrengthDecrease")
                .SetAffinities(RuleDefinitions.CharacterSavingThrowAffinity.Disadvantage, false, AttributeDefinitions.Strength)
                .AddToDB();

            return ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionJump, "LowerDensity")
            .SetOrUpdateGuiPresentation("ConditionLight", Category.Condition)
            .AddFeatures(
                speedbuff,
                lightskill,
                lightsave,
                FeatureDefinitionMovementAffinitys.MovementAffinityJump
                )            
            //.SetAllowMultipleInstances(false) //Test if allows multiple instances at all or just on the same target
            .SetSpecialDuration(DurationType.Minute,1)
            //.SetDuration(RuleDefinitions.DurationType.Minute, 1)
            //.SetTurnOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn)
            .AddToDB();
    }
}
    
    private static FeatureDefinitionPower CreateGravityWell()
    {        
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
            .SetUsesAbilityBonus(ActivationTime.OnSpellCast, RechargeRate.AtWill, AttributeDefinitions.Intelligence)
            .SetEffectDescription(GravityWellEffectDescription)
            /*
            .Configure(
                0, RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence,
                RuleDefinitions.ActivationTime.OnAttackSpellHitAutomatic, 0, RuleDefinitions.RechargeRate.AtWill,
                false, false, AttributeDefinitions.Intelligence, GravityWellEffectDescription, false)*/
            .AddToDB();
        return GravityWell;
    }
    
    private static FeatureDefinitionPower CreateViolentAttraction()
    {
        var ViolentAttractionEffectForm = EffectFormBuilder
            .Create()
            .SetConditionForm(CreateConditionViolentAttraction(), ConditionForm.ConditionOperation.Add, false, false)
            .Build();

        var ViolentAttractionEffectDescription = EffectDescriptionBuilder
            .Create()
            .SetDurationData(RuleDefinitions.DurationType.Minute, 1)
            .SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Distance, 12, RuleDefinitions.TargetType.Individuals)
            .SetNoSavingThrow()
            .SetEffectForms(ViolentAttractionEffectForm)
            .Build();

        var ViolentAttraction = FeatureDefinitionPowerBuilder
            .Create("WizardGravityMageViolentAttraction")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.BrandingSmite.GuiPresentation.SpriteReference)
            .SetUsesAbilityBonus(ActivationTime.BonusAction, RechargeRate.LongRest, AttributeDefinitions.Intelligence)
            .SetEffectDescription(ViolentAttractionEffectDescription)
        .SetShowCasting(true)//Casting flourish.
        .AddToDB();
        return ViolentAttraction;

        static ConditionDefinition CreateConditionViolentAttraction()
        {
            var violentattractionbuff = FeatureDefinitionAdditionalDamageBuilder
            .Create("WizardGravityMageViolentAttractionBuff")
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.Die)
            .SetDamageDice(DieType.D10,1)
            .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            //.SetRequiredProperty(RestrictedContextRequiredProperty.MeleeWeapon)
            .AddToDB();
            
            return ConditionDefinitionBuilder
            .Create("ViolentAttraction")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDivineFavor.GuiPresentation.SpriteReference)
            .SetOrUpdateGuiPresentation("ConditionViolentAttraction", Category.Condition)
            .SetFeatures(violentattractionbuff)
            //.SetAllowMultipleInstances(false)
            .SetSpecialDuration(RuleDefinitions.DurationType.Minute, 1)
            //.SetTurnOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn)
            .AddToDB();
        }
    }
    
    private static FeatureDefinitionPower CreateEventHorizon()
        //Repeat saves for damage effect but slow effect seems permenant
        //Otherwise functional, somewhat awkward without persistant visual effects
    {
        var EventHorizonDamageEffectForm = EffectFormBuilder
            .Create()
            .SetDamageForm(DamageTypeForce,2,DieType.D10)
            .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.HalfDamage)
            .Build();

        var EventHorizonSlowEffectForm = EffectFormBuilder
            .Create()
            .SetConditionForm(CreateConditionEventHorizon(), ConditionForm.ConditionOperation.Add, false, false)
            //.CanSaveToCancel(RuleDefinitions.TurnOccurenceType.StartOfTurn)
            .HasSavingThrow(RuleDefinitions.EffectSavingThrowType.Negates)
            .Build();

        var EventHorizonSelfEffectForm = EffectFormBuilder
            .Create()
            .SetConditionForm(CreateConditionEventHorizonSelf(), ConditionForm.ConditionOperation.Add,true,true)
            .Build();

        var EventHorizonEffectDescription = EffectDescriptionBuilder
            .Create()
            //.SetCreatedByCharacter()
            .SetParticleEffectParameters(SpellDefinitions.Darkness) //On-Cast Visual Effects line
            .SetTargetingData(RuleDefinitions.Side.Enemy,RuleDefinitions.RangeType.Self,0,RuleDefinitions.TargetType.Sphere,6)
            .SetDurationData(RuleDefinitions.DurationType.Minute,1)
            .SetSavingThrowData(
                false,
                AttributeDefinitions.Strength,
                true,
                EffectDifficultyClassComputation.SpellCastingFeature,
                AttributeDefinitions.Intelligence,
                12)
            .SetEffectForms(EventHorizonDamageEffectForm, EventHorizonSlowEffectForm, EventHorizonSelfEffectForm)
            .SetRecurrentEffect(RuleDefinitions.RecurrentEffect.OnEnter | RuleDefinitions.RecurrentEffect.OnTurnStart)
            .Build();

        var EventHorizon = FeatureDefinitionPowerBuilder
            .Create("WizardGravityMageEventHorizon")
            .SetGuiPresentation(Category.Action, SpellDefinitions.DispelEvilAndGood.GuiPresentation.SpriteReference)
            .SetUsesFixed(ActivationTime.Action,RechargeRate.LongRest,1)
            /*.Configure(1, RuleDefinitions.UsesDetermination.Fixed,
                    AttributeDefinitions.Intelligence,
                    RuleDefinitions.ActivationTime.Action, 1,
                    RuleDefinitions.RechargeRate.LongRest, false, false,
                    AttributeDefinitions.Intelligence,
                    EventHorizonEffectDescription
                )*/
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
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionProne.GuiPresentation.SpriteReference)
            .SetOrUpdateGuiPresentation("ConditionEventHorizon", Category.Condition)
            .SetConditionType(RuleDefinitions.ConditionType.Detrimental)
            .SetFeatures(EventHorizonSlow)
            //.SetAllowMultipleInstances(false) //With true stacks same condition over and over            
            .SetSpecialDuration(RuleDefinitions.DurationType.Round,0)
            .SetSilent(Silent.WhenAddedOrRemoved)
            //.SetSilentWhenAdded(true)
            //.SetSilentWhenRemoved(true)
            //.SetTurnOccurence(RuleDefinitions.TurnOccurenceType.StartOfTurn)
            .AddToDB();
        }

        static ConditionDefinition CreateConditionEventHorizonSelf()
        {
            return ConditionDefinitionBuilder
                .Create("EventHorizonSelf") //Or ConditionSpiritGuardiansSelf
                .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionSpiritGuardiansSelf.GuiPresentation.SpriteReference)
                .SetOrUpdateGuiPresentation("ConditionEventHorizonSelf", Category.Condition)
                //.SetTurnOccurence(RuleDefinitions.TurnOccurenceType.StartOfTurn)
                .SetSpecialDuration(RuleDefinitions.DurationType.Minute, 1)
                .AddToDB();
        }
    }
}
