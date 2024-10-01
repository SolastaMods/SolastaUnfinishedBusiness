using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Spells;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ActionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMagicAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSubclassChoices;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class MartialForceKnight : AbstractSubclass
{
    private const string Name = "MartialForceKnight";

    private const ActionDefinitions.Id ForcePoweredStrikeToggle =
        (ActionDefinitions.Id)ExtraActionId.ForcePoweredStrikeToggle;

    private static readonly FeatureDefinitionPower PowerPsionicInitiate = FeatureDefinitionPowerBuilder
        .Create($"Power{Name}PsionicInitiate")
        .SetGuiPresentation(Category.Feature)
        .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest, 1, 3)
        .AddCustomSubFeatures(HasModifiedUses.Marker, ModifyPowerVisibility.Hidden)
        .AddToDB();

    public MartialForceKnight()
    {
        // LEVEL 03

        // Force-Powered Strike

        _ = ActionDefinitionBuilder
            .Create(MetamagicToggle, "ForcePoweredStrikeToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.ForcePoweredStrikeToggle)
            .SetActivatedPower(PowerPsionicInitiate, false)
            .OverrideClassName("Toggle")
            .AddCustomSubFeatures(new ActionItemDiceBoxForcePoweredStrike())
            .AddToDB();

        var additionalDamageForcePoweredStrike = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}ForcePoweredStrike")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("ForcePoweredStrike")
            .SetDamageDice(DieType.D6, 1)
            .SetSpecificDamageType(DamageTypeForce)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .SetAttackModeOnly()
            .SetImpactParticleReference(
                SpellDefinitions.ArcaneSword.EffectDescription.EffectParticleParameters.impactParticleReference)
            .AddToDB();

        additionalDamageForcePoweredStrike.AddCustomSubFeatures(
            new ModifyAdditionalDamagePoweredStrike(additionalDamageForcePoweredStrike));

        var conditionForcePoweredStrike = ConditionDefinitionBuilder
            .Create($"Condition{Name}ForcePoweredStrike")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(additionalDamageForcePoweredStrike)
            .SetSpecialInterruptions(ConditionInterruption.Attacks)
            .AddToDB();

        var actionAffinityForcePoweredStrikeToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityForcePoweredStrikeToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions(ForcePoweredStrikeToggle)
            .AddToDB();

        // Kinetic Barrier

        var conditionKineticBarrier = ConditionDefinitionBuilder
            .Create($"Condition{Name}KineticBarrier")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionShielded)
            .SetPossessive()
            .SetFeatures(
                MagicAffinityConditionShielded,
                FeatureDefinitionAttributeModifierBuilder
                    .Create($"AttributeModifier{Name}KineticBarrier")
                    .SetGuiPresentation($"Condition{Name}KineticBarrier", Category.Condition, Gui.NoLocalization)
                    .SetAddConditionAmount(AttributeDefinitions.ArmorClass)
                    .AddToDB())
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .SetAmountOrigin(ExtraOriginOfAmount.SourceAbilityBonus, AttributeDefinitions.Intelligence)
            .AddToDB();

        var powerKineticBarrier = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}KineticBarrier")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetSharedPool(ActivationTime.NoCost, PowerPsionicInitiate)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionKineticBarrier))
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerTraditionOpenHandTranquility)
                    .UseQuickAnimations()
                    .Build())
            .AddToDB();

        powerKineticBarrier.AddCustomSubFeatures(
            new AttackBeforeHitPossibleOnMeOrAllyKineticBarrier(powerKineticBarrier));

        // Force Drive

        var forceDriveSprite = Sprites.GetSprite("PowerForceDrive", Resources.PowerForceDrive, 256, 128);

        var conditionForceDrive = ConditionDefinitionBuilder
            .Create($"Condition{Name}ForceDrive")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBrandingSmite)
            .SetPossessive()
            .AddCustomSubFeatures(new ModifyWeaponModifyAttackModeForceDrive())
            .AddToDB();

        var powerForceDriveOncePerShort = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ForceDriveOncePerShort")
            .SetGuiPresentation($"Power{Name}ForceDrive", Category.Feature, forceDriveSprite)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionForceDrive))
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerTraditionShockArcanistArcaneFury)
                    .Build())
            .AddToDB();

        powerForceDriveOncePerShort.AddCustomSubFeatures(
            new ValidatorsValidatePowerUse(c => c.GetRemainingPowerUses(powerForceDriveOncePerShort) > 0));

        var powerForceDrive = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}ForceDrive")
            .SetGuiPresentation(Category.Feature, forceDriveSprite)
            .SetSharedPool(ActivationTime.NoCost, PowerPsionicInitiate)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionForceDrive))
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerTraditionShockArcanistArcaneFury)
                    .Build())
            .AddToDB();

        powerForceDrive.AddCustomSubFeatures(
            new ValidatorsValidatePowerUse(c =>
                c.GetRemainingPowerUses(powerForceDriveOncePerShort) == 0 &&
                !c.HasConditionOfCategoryAndType(AttributeDefinitions.TagEffect, conditionForceDrive.Name)));

        // Psionic Initiate

        var featureSetPsionicInitiate = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}PsionicInitiate")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                PowerPsionicInitiate,
                actionAffinityForcePoweredStrikeToggle,
                powerKineticBarrier,
                powerForceDrive,
                powerForceDriveOncePerShort)
            .AddToDB();

        // LEVEL 07

        // Psionic Adept

        var powerPsionicAdept = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}PsionicAdept")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.NoCost)
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        var powerPsionicAdeptPush = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}PsionicAdeptPush")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPsionicAdept)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.Individuals)
                    .SetSavingThrowData(true, AttributeDefinitions.Strength, true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Intelligence,
                        8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 3)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        var powerPsionicAdeptProne = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}PsionicAdeptProne")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerPsionicAdept)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.Individuals)
                    .SetSavingThrowData(true, AttributeDefinitions.Strength, true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Intelligence,
                        8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetMotionForm(MotionForm.MotionType.FallProne)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        PowerBundle.RegisterPowerBundle(powerPsionicAdept, true,
            powerPsionicAdeptProne, powerPsionicAdeptPush);

        actionAffinityForcePoweredStrikeToggle.AddCustomSubFeatures(
            new CustomBehaviorForcePoweredStrike(conditionForcePoweredStrike, powerPsionicAdept));

        var featureSetPsionicAdept = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}PsionicAdept")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerPsionicAdept, powerPsionicAdeptProne, powerPsionicAdeptPush)
            .AddToDB();

        // Psionic Propulsion

        _ = FeatureDefinitionMoveModeBuilder
            .Create(FeatureDefinitionMoveModes.MoveModeFly2, "MoveModeFly14")
            .SetMode(MoveMode.Fly, 14)
            .AddToDB();

        for (var i = 14; i <= 18; i += 2)
        {
            _ = FeatureDefinitionMoveModeBuilder
                .Create(FeatureDefinitionMoveModes.MoveModeMove2, $"MoveModeMove{i}")
                .SetMode(MoveMode.Walk, i)
                .AddToDB();
        }

        var psionicPropulsionSprite =
            Sprites.GetSprite("PowerPsionicPropulsion", Resources.PowerPsionicPropulsion, 256, 128);

        for (var i = 2; i <= 18; i += 2)
        {
            if (!DatabaseRepository.GetDatabase<FeatureDefinitionMoveMode>()
                    .TryGetElement($"MoveModeFly{i}", out var moveModeFly))
            {
                continue;
            }

            if (!DatabaseRepository.GetDatabase<FeatureDefinitionMoveMode>()
                    .TryGetElement($"MoveModeMove{i}", out var moveModeMove))
            {
                continue;
            }

            var conditionPsionicPropulsion = ConditionDefinitionBuilder
                .Create(ConditionDefinitions.ConditionFlying, $"Condition{Name}PsionicPropulsion{i}")
                .SetOrUpdateGuiPresentation($"Power{Name}PsionicPropulsion", Category.Feature)
                .SetPossessive()
                .SetParentCondition(ConditionDefinitions.ConditionFlying)
                .SetFeatures(moveModeFly, moveModeMove, FeatureDefinitionCombatAffinitys.CombatAffinityDisengaging)
                .AddToDB();

            conditionPsionicPropulsion.ConditionTags.Clear();
        }

        var powerPsionicPropulsionOncePerShort = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}PsionicPropulsionOncePerShort")
            .SetGuiPresentation($"Power{Name}PsionicPropulsion", Category.Feature, psionicPropulsionSprite)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ShortRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    // only a placeholder
                    .SetEffectForms(EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionFlying))
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerSorcererDraconicDragonWingsSprout)
                    .Build())
            .AddToDB();

        powerPsionicPropulsionOncePerShort.AddCustomSubFeatures(
            new ModifyEffectDescriptionPsionicPropulsion(powerPsionicPropulsionOncePerShort),
            new ValidatorsValidatePowerUse(c => c.GetRemainingPowerUses(powerPsionicPropulsionOncePerShort) > 0));

        var powerPsionicPropulsion = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}PsionicPropulsion")
            .SetGuiPresentation(Category.Feature, psionicPropulsionSprite)
            .SetSharedPool(ActivationTime.BonusAction, PowerPsionicInitiate)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionFlying))
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerSorcererDraconicDragonWingsSprout)
                    .Build())
            .AddToDB();

        powerPsionicPropulsion.AddCustomSubFeatures(
            new ModifyEffectDescriptionPsionicPropulsion(powerPsionicPropulsion),
            new ValidatorsValidatePowerUse(c => c.GetRemainingPowerUses(powerPsionicPropulsionOncePerShort) == 0));

        var featureSetPsionicPropulsion = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}PsionicPropulsion")
            .SetGuiPresentation($"Power{Name}PsionicPropulsion", Category.Feature)
            .AddFeatureSet(powerPsionicPropulsion, powerPsionicPropulsionOncePerShort)
            .AddToDB();

        // LEVEL 10

        // Force of Will

        var featureForceOfWill = FeatureDefinitionBuilder
            .Create($"Feature{Name}ForceOfWill")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureForceOfWill.AddCustomSubFeatures(new ForceOfWill(featureForceOfWill));

        // LEVEL 15

        // Force Bulwark

        var forceBulwarkSprite =
            Sprites.GetSprite("PowerForceBulwark", Resources.PowerForceBulwark, 256, 128);

        var conditionForceBulwark = ConditionDefinitionBuilder
            .Create($"Condition{Name}ForceBulwark")
            .SetGuiPresentation($"Power{Name}ForceBulwark", Category.Feature,
                ConditionDefinitions.ConditionMagicallyArmored)
            .SetPossessive()
            .SetFeatures(
                FeatureDefinitionCombatAffinityBuilder
                    .Create($"CombatAffinity{Name}ForceBulwark")
                    .SetGuiPresentation($"Power{Name}ForceBulwark", Category.Feature, "UI/&HasHalfCover")
                    .SetPermanentCover(CoverType.Half)
                    .AddToDB())
            .AddToDB();

        conditionForceBulwark.GuiPresentation.description = Gui.EmptyContent;

        var conditionForceBulwarkSelf = ConditionDefinitionBuilder
            .Create($"Condition{Name}ForceBulwarkSelf")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetCancellingConditions(
                DatabaseRepository.GetDatabase<ConditionDefinition>().Where(x =>
                    x.IsSubtypeOf(ConditionIncapacitated)).ToArray())
            .AddCustomSubFeatures(new OnConditionAddedOrRemovedForceBulwark(conditionForceBulwark))
            .AddToDB();

        var powerForceBulwarkOncePerLong = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ForceBulwarkOncePerLong")
            .SetGuiPresentation($"Power{Name}ForceBulwark", Category.Feature, forceBulwarkSprite)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionForceBulwark),
                        EffectFormBuilder.ConditionForm(conditionForceBulwarkSelf,
                            ConditionForm.ConditionOperation.Add, true))
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerTraditionCourtMageSpellShield)
                    .Build())
            .AddToDB();

        powerForceBulwarkOncePerLong.AddCustomSubFeatures(
            new ModifyEffectDescriptionForceBulwark(powerForceBulwarkOncePerLong),
            new ValidatorsValidatePowerUse(c => c.GetRemainingPowerUses(powerForceBulwarkOncePerLong) > 0));

        var powerForceBulwark = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}ForceBulwark")
            .SetGuiPresentation(Category.Feature, forceBulwarkSprite)
            .SetSharedPool(ActivationTime.BonusAction, PowerPsionicInitiate)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionForceBulwark),
                        EffectFormBuilder.ConditionForm(conditionForceBulwarkSelf,
                            ConditionForm.ConditionOperation.Add, true))
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerTraditionCourtMageSpellShield)
                    .Build())
            .AddToDB();

        powerForceBulwark.AddCustomSubFeatures(
            new ModifyEffectDescriptionForceBulwark(powerForceBulwark),
            new ValidatorsValidatePowerUse(c => c.GetRemainingPowerUses(powerForceBulwarkOncePerLong) == 0));

        var featureSetForceBulwark = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}ForceBulwark")
            .SetGuiPresentation($"Power{Name}ForceBulwark", Category.Feature)
            .AddFeatureSet(powerForceBulwark, powerForceBulwarkOncePerLong)
            .AddToDB();

        // LEVEL 18

        // Telekinetic Grasp

        const string SpellName = "Telekinesis";

        var sprite = Sprites.GetSprite(SpellName, Resources.Telekinesis, 128);

        var powerTelekinesis = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}SpellTelekineticGrasp")
            .SetGuiPresentation(SpellName, Category.Spell, sprite)
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.All, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .ExcludeCaster()
                    .SetParticleEffectParameters(SpellDefinitions.MistyStep)
                    .Build())
            .AddToDB();

        var conditionTelekinesis = ConditionDefinitionBuilder
            .Create($"Condition{Name}TelekineticGrasp")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionRevealedByDetectGoodOrEvil)
            .SetPossessive()
            .SetFeatures(powerTelekinesis)
            .AddCustomSubFeatures(
                AddUsablePowersFromCondition.Marker,
                SpellBuilders.OnConditionAddedOrRemovedTelekinesis.Marker,
                new AddExtraMainHandAttack(ActionDefinitions.ActionType.Bonus))
            .CopyParticleReferences(SpellDefinitions.SpiderClimb)
            .AddToDB();

        var powerTelekinesisNoCost = FeatureDefinitionPowerBuilder
            .Create(powerTelekinesis, $"Power{Name}SpellTelekineticGraspNoCost")
            .SetUsesFixed(ActivationTime.NoCost)
            .AddToDB();

        var conditionTelekinesisNoCost = ConditionDefinitionBuilder
            .Create($"Condition{Name}TelekineticGraspNoCost")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(powerTelekinesisNoCost)
            .AddCustomSubFeatures(
                AddUsablePowersFromCondition.Marker,
                SpellBuilders.OnConditionAddedOrRemovedTelekinesis.Marker)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var spell = SpellDefinitionBuilder
            .Create(SpellName + Name)
            .SetGuiPresentation(SpellName, Category.Spell, sprite)
            .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolTransmutation)
            .SetSpellLevel(5)
            .SetCastingTime(ActivationTime.Action)
            .SetMaterialComponent(MaterialComponentType.None)
            .SetVerboseComponent(true)
            .SetSomaticComponent(true)
            .SetVocalSpellSameType(VocalSpellSemeType.Debuff)
            .SetRequiresConcentration(true)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 10)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionTelekinesisNoCost),
                        EffectFormBuilder.ConditionForm(conditionTelekinesis))
                    .SetParticleEffectParameters(SpellDefinitions.MindTwist)
                    .SetConditionEffectParameters()
                    .Build())
            .AddToDB();

        var customBehavior = new SpellBuilders.CustomBehaviorTelekinesis(conditionTelekinesisNoCost, spell);

        powerTelekinesis.AddCustomSubFeatures(customBehavior);
        powerTelekinesisNoCost.AddCustomSubFeatures(customBehavior, ValidatorsValidatePowerUse.InCombat);

        var powerTelekineticGraspOncePerLong = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}TelekineticGraspOncePerLong")
            .SetGuiPresentation($"Power{Name}TelekineticGrasp", Category.Feature, SpellsContext.Telekinesis)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetShowCasting(false)
            .AddToDB();

        powerTelekineticGraspOncePerLong.AddCustomSubFeatures(
            new ValidatorsValidatePowerUse(
                c => c.GetRemainingPowerUses(powerTelekineticGraspOncePerLong) > 0,
                ValidatorsCharacter.HasNoneOfConditions(conditionTelekinesis.Name)),
            new PowerOrSpellFinishedByMeTelekineticGrasp(spell));

        var powerTelekineticGrasp = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}TelekineticGrasp")
            .SetGuiPresentation(Category.Feature, SpellsContext.Telekinesis)
            .SetSharedPool(ActivationTime.Action, PowerPsionicInitiate)
            .SetShowCasting(false)
            .AddCustomSubFeatures(
                new ValidatorsValidatePowerUse(
                    c => c.GetRemainingPowerUses(powerTelekineticGraspOncePerLong) == 0,
                    ValidatorsCharacter.HasNoneOfConditions(conditionTelekinesis.Name)),
                new PowerOrSpellFinishedByMeTelekineticGrasp(spell))
            .AddToDB();

        var featureSetTelekineticGrasp = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}TelekineticGrasp")
            .SetGuiPresentation($"Power{Name}TelekineticGrasp", Category.Feature)
            .AddFeatureSet(powerTelekineticGrasp, powerTelekineticGraspOncePerLong)
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.MartialForceKnight, 256))
            .AddFeaturesAtLevel(3, featureSetPsionicInitiate)
            .AddFeaturesAtLevel(6, BuildPowerModifier(PowerPsionicInitiate, 6))
            .AddFeaturesAtLevel(7, featureSetPsionicAdept, featureSetPsionicPropulsion)
            .AddFeaturesAtLevel(9, BuildPowerModifier(PowerPsionicInitiate, 9))
            .AddFeaturesAtLevel(10, featureForceOfWill)
            .AddFeaturesAtLevel(12, BuildPowerModifier(PowerPsionicInitiate, 12))
            .AddFeaturesAtLevel(15, BuildPowerModifier(PowerPsionicInitiate, 15), featureSetForceBulwark)
            .AddFeaturesAtLevel(18, BuildPowerModifier(PowerPsionicInitiate, 18), featureSetTelekineticGrasp)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Fighter;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => SubclassChoiceFighterMartialArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static FeatureDefinitionPowerUseModifier BuildPowerModifier(
        FeatureDefinitionPower powerPsionicInitiate, int level)
    {
        return FeatureDefinitionPowerUseModifierBuilder
            .Create($"PowerUseModifier{Name}PsionicInitiate{level:00}")
            .SetGuiPresentationNoContent(true)
            .SetFixedValue(powerPsionicInitiate, 1)
            .AddToDB();
    }

    private static int GetIntModifier(
        // ReSharper disable once SuggestBaseTypeForParameter
        RulesetCharacter rulesetCharacter)
    {
        var intelligence = rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Intelligence);
        var intMod = AttributeDefinitions.ComputeAbilityScoreModifier(intelligence);

        return Math.Max(1, intMod);
    }

    private static DieType GetForcePoweredStrikeSize(RulesetCharacter character)
    {
        var level = character.GetSubclassLevel(CharacterClassDefinitions.Fighter, Name);

        return level switch
        {
            >= 18 => DieType.D12,
            >= 10 => DieType.D10,
            >= 3 => DieType.D8,
            _ => DieType.D6
        };
    }

    //
    // Force Powered Strike
    //

    private sealed class CustomBehaviorForcePoweredStrike(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionForcePoweredStrike,
        FeatureDefinitionPower powerPsionicAdept)
        : IPhysicalAttackBeforeHitConfirmedOnEnemy, IPhysicalAttackFinishedByMe
    {
        public IEnumerator OnPhysicalAttackBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            attacker.UsedSpecialFeatures.TryAdd(powerPsionicAdept.Name, 0);

            var rulesetAttacker = attacker.RulesetCharacter;

            if (!attacker.OnceInMyTurnIsValid(conditionForcePoweredStrike.Name) ||
                rulesetAttacker.GetRemainingPowerUses(PowerPsionicInitiate) == 0 ||
                !rulesetAttacker.IsToggleEnabled(ForcePoweredStrikeToggle))
            {
                yield break;
            }

            attacker.UsedSpecialFeatures.TryAdd(conditionForcePoweredStrike.Name, 0);
            attacker.UsedSpecialFeatures[powerPsionicAdept.Name] = 1;
            rulesetAttacker.UpdateUsageForPower(PowerPsionicInitiate, 1);
            rulesetAttacker.InflictCondition(
                conditionForcePoweredStrike.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.Guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionForcePoweredStrike.Name,
                0,
                0,
                0);
        }

        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var levels = rulesetAttacker.GetClassLevel(CharacterClassDefinitions.Fighter);

            if (!attacker.UsedSpecialFeatures.TryGetValue(powerPsionicAdept.Name, out var value) || value == 0 ||
                defender.RulesetActor is not { IsDeadOrDyingOrUnconscious: false } ||
                (rollOutcome != RollOutcome.Success && rollOutcome != RollOutcome.CriticalSuccess) ||
                levels < 7)
            {
                yield break;
            }

            attacker.UsedSpecialFeatures[powerPsionicAdept.Name] = 0;

            var usablePower = PowerProvider.Get(powerPsionicAdept, rulesetAttacker);

            //TODO: double check this behavior with a push from origin power
            yield return attacker.MyReactToSpendPowerBundle(
                usablePower,
                [defender],
                attacker,
                "PsionicAdept",
                battleManager: battleManager);
        }
    }

    private sealed class ModifyAdditionalDamagePoweredStrike(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionAdditionalDamage additionalDamage) : IModifyAdditionalDamage
    {
        public void ModifyAdditionalDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            FeatureDefinitionAdditionalDamage featureDefinitionAdditionalDamage,
            List<EffectForm> actualEffectForms,
            ref DamageForm damageForm)
        {
            if (featureDefinitionAdditionalDamage != additionalDamage)
            {
                return;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var dieType = GetForcePoweredStrikeSize(rulesetAttacker);
            var intMod = GetIntModifier(rulesetAttacker);

            damageForm.BonusDamage = intMod;
            damageForm.DieType = dieType;
        }
    }

    private sealed class ActionItemDiceBoxForcePoweredStrike : IActionItemDiceBox
    {
        public (DieType type, int number, string format) GetDiceInfo(RulesetCharacter character)
        {
            return (GetForcePoweredStrikeSize(character), character.GetRemainingPowerUses(PowerPsionicInitiate),
                "Tooltip/&ForcePoweredStrikeDieDescription");
        }
    }

    //
    // Kinetic Barrier
    //

    private class AttackBeforeHitPossibleOnMeOrAllyKineticBarrier(FeatureDefinitionPower powerKineticBarrier)
        : ITryAlterOutcomeAttack
    {
        public int HandlerPriority => -10;

        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect)
        {
            var rulesetHelper = helper.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerKineticBarrier, rulesetHelper);

            if (action.AttackRollOutcome is not RollOutcome.Success ||
                helper.IsOppositeSide(defender.Side) ||
                !helper.CanReact() ||
                !helper.CanPerceiveTarget(defender) ||
                // must use GetRemainingPowerUses as a shared pool power
                rulesetHelper.GetRemainingPowerUses(powerKineticBarrier) == 0)
            {
                yield break;
            }

            var armorClass = defender.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.ArmorClass);
            var attackRoll = action.AttackRoll;
            var totalAttack =
                attackRoll +
                (attackMode?.ToHitBonus ?? rulesetEffect?.MagicAttackBonus ?? 0) +
                actionModifier.AttackRollModifier;

            if (armorClass > totalAttack)
            {
                yield break;
            }

            var intMod = GetIntModifier(rulesetHelper);

            if (armorClass + intMod <= totalAttack)
            {
                yield break;
            }

            yield return helper.MyReactToUsePower(
                ActionDefinitions.Id.PowerReaction,
                usablePower,
                [defender],
                attacker,
                "KineticBarrier",
                FormatReactionDescription(attacker, defender, helper),
                battleManager: battleManager);
        }

        private static string FormatReactionDescription(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper)
        {
            var text = defender == helper ? "Self" : "Ally";

            return $"UseKineticBarrierReactDescription{text}"
                .Formatted(Category.Reaction, attacker.Name, defender.Name);
        }
    }

    //
    // Force Drive
    //

    private sealed class ModifyWeaponModifyAttackModeForceDrive : IModifyWeaponAttackMode
    {
        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            attackMode.reachRange += 6;
        }
    }

    //
    // Psionic Propulsion
    //

    private sealed class ModifyEffectDescriptionPsionicPropulsion(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerPsionicPropulsion) : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerPsionicPropulsion;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var glc = GameLocationCharacter.GetFromActor(character);

            if (glc == null)
            {
                return effectDescription;
            }

            var flyMoves = Math.Min(glc.MaxTacticalMoves, 9) * 2;

            if (!DatabaseRepository.GetDatabase<ConditionDefinition>()
                    .TryGetElement($"Condition{Name}PsionicPropulsion{flyMoves}", out var condition))
            {
                return effectDescription;
            }

            effectDescription.EffectForms[0].ConditionForm.ConditionDefinition = condition;

            return effectDescription;
        }
    }

    //
    // Force of Will
    //

    private sealed class ForceOfWill(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition featureForceOfWill) : ICharacterTurnStartListener, IRollSavingThrowInitiated
    {
        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            var rulesetCharacter = locationCharacter.RulesetCharacter;

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false })
            {
                return;
            }

            var intelligence = rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Intelligence);
            var intMod = AttributeDefinitions.ComputeAbilityScoreModifier(intelligence);

            rulesetCharacter.ReceiveTemporaryHitPoints(
                intMod, DurationType.UntilAnyRest, 0, TurnOccurenceType.StartOfTurn, rulesetCharacter.Guid);
        }

        public void OnSavingThrowInitiated(
            RulesetActor rulesetActorCaster,
            RulesetActor rulesetActorDefender,
            ref int saveBonus,
            ref string abilityScoreName,
            BaseDefinition sourceDefinition,
            List<TrendInfo> modifierTrends,
            List<TrendInfo> advantageTrends,
            ref int rollModifier,
            ref int saveDC,
            ref bool hasHitVisual,
            RollOutcome outcome,
            int outcomeDelta,
            List<EffectForm> effectForms)
        {
            if (rulesetActorDefender is not RulesetCharacter rulesetCharacterDefender)
            {
                return;
            }

            var changed = false;
            var intelligence =
                ComputeBaseBonus(rulesetCharacterDefender, AttributeDefinitions.Intelligence, out var intModifier);

            if (abilityScoreName == AttributeDefinitions.Wisdom)
            {
                var wisdom = ComputeBaseBonus(rulesetCharacterDefender, AttributeDefinitions.Wisdom, out _);

                if (intelligence > wisdom)
                {
                    abilityScoreName = AttributeDefinitions.Intelligence;
                    changed = true;
                }
            }

            if (abilityScoreName == AttributeDefinitions.Charisma)
            {
                var charisma = ComputeBaseBonus(rulesetCharacterDefender, AttributeDefinitions.Charisma, out _);

                if (intelligence > charisma)
                {
                    abilityScoreName = AttributeDefinitions.Intelligence;
                    changed = true;
                }
            }

            if (!changed)
            {
                return;
            }

            saveBonus = intelligence;
            modifierTrends.RemoveAll(x =>
                x.sourceType is FeatureSourceType.AbilityScore or FeatureSourceType.Proficiency);
            modifierTrends.AddRange(intModifier);
            rulesetCharacterDefender.LogCharacterUsedFeature(featureForceOfWill);
        }

        private static int ComputeBaseBonus(
            RulesetCharacter defender,
            string abilityScoreName,
            out List<TrendInfo> savingThrowModifierTrends)
        {
            savingThrowModifierTrends = [];

            var bonus =
                AttributeDefinitions.ComputeAbilityScoreModifier(defender.TryGetAttributeValue(abilityScoreName));

            savingThrowModifierTrends.Add(new TrendInfo(bonus, FeatureSourceType.AbilityScore, abilityScoreName, null));

            var proficiency = defender.GetFeaturesByType<FeatureDefinitionProficiency>()
                .FirstOrDefault(x =>
                    x.ProficiencyType == ProficiencyType.SavingThrow &&
                    x.Proficiencies.Contains(abilityScoreName));

            if (!proficiency)
            {
                return bonus;
            }

            var pb = defender.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);

            bonus += pb;
            savingThrowModifierTrends.Add(new TrendInfo(pb, FeatureSourceType.Proficiency, string.Empty, null));

            return bonus;
        }
    }

    //
    // Force Bulwark
    //

    private sealed class OnConditionAddedOrRemovedForceBulwark(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionForceBulwark) : IOnConditionAddedOrRemoved
    {
        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var allies = locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters);

            foreach (var ally in allies
                         .Select(y => y.RulesetCharacter)
                         .Where(x => x is { IsDeadOrDyingOrUnconscious: false }))
            {
                // should only check the condition from the same source
                if (ally.TryGetConditionOfCategoryAndType(
                        AttributeDefinitions.TagEffect, conditionForceBulwark.Name, out var activeCondition) &&
                    activeCondition.SourceGuid == target.Guid)
                {
                    ally.RemoveCondition(activeCondition);
                }
            }
        }
    }

    private sealed class ModifyEffectDescriptionForceBulwark(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerForceBulwark) : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerForceBulwark;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var intMod = GetIntModifier(character);

            effectDescription.targetParameter = intMod;

            return effectDescription;
        }
    }

    //
    // Telekinetic Grasp
    //

    private sealed class PowerOrSpellFinishedByMeTelekineticGrasp(SpellDefinition spellTelekineticGrasp)
        : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            action.ActingCharacter.MyExecuteActionCastNoCost(spellTelekineticGrasp, 6, action.ActionParams);

            yield break;
        }
    }
}
