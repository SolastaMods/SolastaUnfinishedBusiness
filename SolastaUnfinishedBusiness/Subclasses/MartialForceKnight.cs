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
using UnityEngine.AddressableAssets;
using static FeatureDefinitionAttributeModifier;
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

    internal static readonly FeatureDefinitionPower PowerPsionicInitiate = FeatureDefinitionPowerBuilder
        .Create($"Power{Name}PsionicInitiate")
        .SetGuiPresentation(Category.Feature)
        .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest, 1, 3)
        .AddCustomSubFeatures(
            HasModifiedUses.Marker,
            IsModifyPowerPool.Marker,
            ModifyPowerVisibility.Hidden)
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
            .AddCustomSubFeatures(new ActionItemDiceBoxForcePoweredStrike())
            .AddToDB();

        var additionalDamageForcePoweredStrike = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}ForcePoweredStrike")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("ForcePoweredStrike")
            .SetDamageDice(DieType.D6, 1)
            .SetSpecificDamageType(DamageTypeForce)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .SetImpactParticleReference(
                SpellDefinitions.ArcaneSword.EffectDescription.EffectParticleParameters.impactParticleReference)
            .AddCustomSubFeatures(
                new ModifyAdditionalDamageFormPoweredStrike(),
                ValidatorsRestrictedContext.IsWeaponOrUnarmedAttack)
            .AddToDB();

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
                    .SetModifier(AttributeModifierOperation.AddConditionAmount, AttributeDefinitions.ArmorClass)
                    .AddToDB())
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .SetAmountOrigin(ExtraOriginOfAmount.SourceAbilityBonus, AttributeDefinitions.Intelligence)
            .AddToDB();

        var powerKineticBarrier = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}KineticBarrier")
            .SetGuiPresentation(Category.Feature)
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
            ModifyPowerVisibility.Hidden,
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
            new ValidatorsValidatePowerUse(c => c.GetRemainingPowerUses(powerForceDriveOncePerShort) == 0));

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
            new ValidateDefinitionApplication(ValidatorsCharacter.HasAvailablePowerUsage(PowerPsionicInitiate)),
            new CustomBehaviorForcePoweredStrike(conditionForcePoweredStrike, powerPsionicAdept));

        var featureSetPsionicAdept = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}PsionicAdept")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(PowerPsionicInitiate, powerPsionicAdeptProne, powerPsionicAdeptPush)
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
                .SetGuiPresentation($"Power{Name}PsionicPropulsion", Category.Feature,
                    ConditionDefinitions.ConditionFlying)
                .SetPossessive()
                .SetParentCondition(ConditionDefinitions.ConditionFlying)
                .SetFeatures(moveModeFly, moveModeMove, FeatureDefinitionCombatAffinitys.CombatAffinityDisengaging)
                .AddToDB();

            conditionPsionicPropulsion.GuiPresentation.description = Gui.NoLocalization;

            // there is indeed a typo on tag
            // ReSharper disable once StringLiteralTypo
            conditionPsionicPropulsion.ConditionTags.Add("Verticality");
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
                    .SetGuiPresentation($"Power{Name}ForceBulwark", Category.Feature)
                    .SetPermanentCover(CoverType.Half)
                    .AddToDB())
            .AddToDB();

        conditionForceBulwark.GuiPresentation.description = Gui.NoLocalization;

        var conditionForceBulwarkSelf = ConditionDefinitionBuilder
            .Create($"Condition{Name}ForceBulwarkSelf")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetCancellingConditions(ConditionDefinitions.ConditionIncapacitated)
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
                            ConditionForm.ConditionOperation.Add, true, true))
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
                            ConditionForm.ConditionOperation.Add, true, true))
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

        var sprite = Sprites.GetSprite(SpellName, Resources.Telekinesis, 128, 128);

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
                    .Build())
            .AddToDB();

        spell.EffectDescription.EffectParticleParameters.conditionStartParticleReference = new AssetReference();
        spell.EffectDescription.EffectParticleParameters.conditionParticleReference = new AssetReference();
        spell.EffectDescription.EffectParticleParameters.conditionEndParticleReference = new AssetReference();

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
            new MagicEffectFinishedByMeTelekineticGrasp(spell));

        var powerTelekineticGrasp = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}TelekineticGrasp")
            .SetGuiPresentation(Category.Feature, SpellsContext.Telekinesis)
            .SetSharedPool(ActivationTime.Action, PowerPsionicInitiate)
            .SetShowCasting(false)
            .AddCustomSubFeatures(
                new ValidatorsValidatePowerUse(
                    c => c.GetRemainingPowerUses(powerTelekineticGraspOncePerLong) == 0,
                    ValidatorsCharacter.HasNoneOfConditions(conditionTelekinesis.Name)),
                new MagicEffectFinishedByMeTelekineticGrasp(spell))
            .AddToDB();

        var featureSetTelekineticGrasp = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}TelekineticGrasp")
            .SetGuiPresentation($"Power{Name}TelekineticGrasp", Category.Feature)
            .AddFeatureSet(powerTelekineticGrasp, powerTelekineticGraspOncePerLong)
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.WizardGravityMage, 256))
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
            >= 17 => DieType.D12,
            >= 11 => DieType.D10,
            >= 5 => DieType.D8,
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
        : IPhysicalAttackInitiatedByMe, IPhysicalAttackBeforeHitConfirmedOnEnemy, IPhysicalAttackFinishedByMe
    {
        private bool _considerTriggerPsionicAdept;

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
            var rulesetAttacker = attacker.RulesetCharacter;

            if (!attacker.OnceInMyTurnIsValid("ForcePoweredStrike") ||
                rulesetAttacker.GetRemainingPowerUses(PowerPsionicInitiate) == 0 ||
                !rulesetAttacker.IsToggleEnabled(ForcePoweredStrikeToggle))
            {
                yield break;
            }

            if (!ValidatorsWeapon.IsMelee(attackMode) && !ValidatorsWeapon.IsUnarmed(attackMode))
            {
                yield break;
            }

            _considerTriggerPsionicAdept = true;

            attacker.UsedSpecialFeatures.TryAdd("ForcePoweredStrike", 0);
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
            if (!_considerTriggerPsionicAdept)
            {
                yield break;
            }

            var actionManager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (actionManager == null)
            {
                yield break;
            }

            if (defender.RulesetCharacter is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var rulesetCharacter = attacker.RulesetCharacter;

            var levels = rulesetCharacter.GetClassLevel(CharacterClassDefinitions.Fighter);

            if (levels < 7)
            {
                yield break;
            }

            var implementationManagerService =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerPsionicAdept, rulesetCharacter);
            var actionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.PowerNoCost)
            {
                ActionModifiers = { new ActionModifier() },
                StringParameter = "PsionicAdept",
                RulesetEffect = implementationManagerService
                    .MyInstantiateEffectPower(rulesetCharacter, usablePower, false),
                UsablePower = usablePower,
                TargetCharacters = { defender }
            };

            var count = actionManager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestSpendBundlePower(actionParams);

            actionManager.AddInterruptRequest(reactionRequest);

            yield return battleManager.WaitForReactions(attacker, actionManager, count);
        }

        public IEnumerator OnPhysicalAttackInitiatedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode)
        {
            _considerTriggerPsionicAdept = false;

            yield break;
        }
    }

    private sealed class ModifyAdditionalDamageFormPoweredStrike : IModifyAdditionalDamageForm
    {
        public DamageForm AdditionalDamageForm(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            FeatureDefinitionAdditionalDamage featureDefinitionAdditionalDamage,
            DamageForm damageForm)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var dieType = GetForcePoweredStrikeSize(rulesetAttacker);
            var intMod = GetIntModifier(rulesetAttacker);

            damageForm.BonusDamage = intMod;
            damageForm.DieType = dieType;

            return damageForm;
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
        : IAttackBeforeHitPossibleOnMeOrAlly
    {
        public IEnumerator OnAttackBeforeHitPossibleOnMeOrAlly(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect,
            int attackRoll)
        {
            if (rulesetEffect != null &&
                rulesetEffect.EffectDescription.RangeType is not (RangeType.MeleeHit or RangeType.RangeHit))
            {
                yield break;
            }

            var gameLocationActionManager =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (gameLocationActionManager == null)
            {
                yield break;
            }

            var rulesetHelper = helper.RulesetCharacter;

            if (!helper.CanReact() ||
                !helper.CanPerceiveTarget(defender) ||
                rulesetHelper.GetRemainingPowerUses(powerKineticBarrier) == 0)
            {
                yield break;
            }

            var armorClass = defender.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.ArmorClass);
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

            var implementationManagerService =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerKineticBarrier, rulesetHelper);
            var actionParams =
                new CharacterActionParams(helper, ActionDefinitions.Id.PowerReaction)
                {
                    StringParameter = "KineticBarrier",
                    StringParameter2 = FormatReactionDescription(attacker, defender, helper),
                    ActionModifiers = { new ActionModifier() },
                    RulesetEffect = implementationManagerService
                        .MyInstantiateEffectPower(rulesetHelper, usablePower, false),
                    UsablePower = usablePower,
                    TargetCharacters = { defender }
                };

            var count = gameLocationActionManager.PendingReactionRequestGroups.Count;

            gameLocationActionManager.ReactToUsePower(actionParams, "UsePower", helper);

            yield return battleManager.WaitForReactions(attacker, gameLocationActionManager, count);
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
                intMod, DurationType.Minute, 1, TurnOccurenceType.EndOfTurn, rulesetCharacter.Guid);
        }

        public void OnSavingThrowInitiated(
            RulesetCharacter caster,
            RulesetCharacter defender,
            ref string abilityScoreName,
            BaseDefinition sourceDefinition,
            List<TrendInfo> advantageTrends,
            int saveDC,
            bool hasHitVisual,
            List<EffectForm> effectForms)
        {
            var intelligence = defender.TryGetAttributeValue(AttributeDefinitions.Intelligence);

            if (abilityScoreName == AttributeDefinitions.Wisdom)
            {
                var wisdom = defender.TryGetAttributeValue(AttributeDefinitions.Wisdom);

                if (intelligence > wisdom)
                {
                    abilityScoreName = AttributeDefinitions.Intelligence;

                    defender.LogCharacterUsedFeature(featureForceOfWill);
                }
            }

            // ReSharper disable once InvertIf
            if (abilityScoreName == AttributeDefinitions.Charisma)
            {
                var charisma = defender.TryGetAttributeValue(AttributeDefinitions.Charisma);

                // ReSharper disable once InvertIf
                if (intelligence > charisma)
                {
                    abilityScoreName = AttributeDefinitions.Intelligence;

                    defender.LogCharacterUsedFeature(featureForceOfWill);
                }
            }
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

    private sealed class MagicEffectFinishedByMeTelekineticGrasp(SpellDefinition spellTelekineticGrasp)
        : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var actionService = ServiceRepository.GetService<IGameLocationActionService>();

            if (actionService == null)
            {
                yield break;
            }

            var actingCharacter = action.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;

            var effectSpell = ServiceRepository.GetService<IRulesetImplementationService>()
                .InstantiateEffectSpell(rulesetCharacter, null, spellTelekineticGrasp, 5, false);

            var actionParams = action.ActionParams.Clone();

            actionParams.ActionDefinition = actionService.AllActionDefinitions[ActionDefinitions.Id.CastNoCost];
            actionParams.RulesetEffect = effectSpell;

            actionService.ExecuteAction(actionParams, null, true);
        }
    }
}
