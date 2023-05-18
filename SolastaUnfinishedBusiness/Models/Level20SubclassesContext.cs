using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Properties;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAutoPreparedSpellss;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;

namespace SolastaUnfinishedBusiness.Models;

internal static class Level20SubclassesContext
{
    internal static void Load()
    {
        ClericLoad();
        FighterLoad();
        MonkLoad();
        PaladinLoad();
        RogueLoad();
        SorcererLoad();
    }

    private static void ClericLoad()
    {
        // Divine Intervention

        var powerClericDivineInterventionImprovementCleric = FeatureDefinitionPowerBuilder
            .Create(PowerClericDivineInterventionCleric, "PowerClericDivineInterventionImprovementCleric")
            .SetHasCastingFailure(false)
            .SetOverriddenPower(PowerClericDivineInterventionCleric)
            .AddToDB();

        var powerClericDivineInterventionImprovementPaladin = FeatureDefinitionPowerBuilder
            .Create(PowerClericDivineInterventionPaladin, "PowerClericDivineInterventionImprovementPaladin")
            .SetHasCastingFailure(false)
            .SetOverriddenPower(PowerClericDivineInterventionPaladin)
            .AddToDB();

        var powerClericDivineInterventionImprovementWizard = FeatureDefinitionPowerBuilder
            .Create(PowerClericDivineInterventionWizard, "PowerClericDivineInterventionImprovementWizard")
            .SetHasCastingFailure(false)
            .SetOverriddenPower(PowerClericDivineInterventionWizard)
            .AddToDB();

        DomainBattle.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementPaladin, 20));
        DomainElementalCold.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementWizard, 20));
        DomainElementalFire.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementWizard, 20));
        DomainElementalLighting.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementWizard, 20));
        DomainInsight.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementCleric, 20));
        DomainLaw.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementPaladin, 20));
        DomainLife.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementCleric, 20));
        DomainMischief.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementWizard, 20));
        DomainOblivion.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementCleric, 20));
        DomainSun.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerClericDivineInterventionImprovementWizard, 20));
    }

    private static void FighterLoad()
    {
        //
        // Champion
        //

        var featureMartialChampionSurvivor = FeatureDefinitionBuilder
            .Create("FeatureMartialChampionSurvivor")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new CharacterTurnStartListenerSurvivor())
            .AddToDB();

        MartialChampion.FeatureUnlocks.Add(new FeatureUnlockByLevel(featureMartialChampionSurvivor, 18));

        //
        // Commander
        //

        var conditionMartialCommanderPeerlessCommanderSavings = ConditionDefinitionBuilder
            .Create("ConditionMartialCommanderPeerlessCommanderSavings")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionMagicallyArmored)
            .SetPossessive()
            .AddFeatures(
                FeatureDefinitionSavingThrowAffinityBuilder
                    .Create("SavingThrowAffinityMartialCommanderPeerlessCommander")
                    .SetGuiPresentation("ConditionMartialCommanderPeerlessCommanderSavings", Category.Condition)
                    .SetAffinities(CharacterSavingThrowAffinity.Advantage, false,
                        AttributeDefinitions.Strength,
                        AttributeDefinitions.Dexterity,
                        AttributeDefinitions.Constitution,
                        AttributeDefinitions.Intelligence,
                        AttributeDefinitions.Wisdom,
                        AttributeDefinitions.Charisma)
                    .AddToDB())
            .SetSpecialInterruptions(ConditionInterruption.SavingThrow)
            .AddToDB();

        var conditionMartialCommanderPeerlessCommanderMovement = ConditionDefinitionBuilder
            .Create("ConditionMartialCommanderPeerlessCommanderMovement")
            .SetOrUpdateGuiPresentation(Category.Condition, ConditionDefinitions.ConditionFreedomOfMovement)
            .SetPossessive()
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
            .AddFeatures(
                FeatureDefinitionMovementAffinityBuilder
                    .Create("MovementAffinityMartialCommanderPeerlessCommander")
                    .SetGuiPresentation("ConditionMartialCommanderPeerlessCommanderMovement", Category.Condition)
                    .SetBaseSpeedAdditiveModifier(2)
                    .AddToDB())
            .AddToDB();

        var powerMartialCommanderPeerlessCommander = FeatureDefinitionPowerBuilder
            .Create(PowerMartialCommanderInvigoratingShout, "PowerMartialCommanderPeerlessCommander")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerMartialCommanderInvigoratingShout)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(ConditionDefinitions.ConditionRousingShout,
                                ConditionForm.ConditionOperation.Add)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionMartialCommanderPeerlessCommanderSavings,
                                ConditionForm.ConditionOperation.Add)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionMartialCommanderPeerlessCommanderMovement,
                                ConditionForm.ConditionOperation.Add)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetLevelAdvancement(EffectForm.LevelApplianceType.AddBonus, LevelSourceType.ClassLevel)
                            .SetTempHpForm(0, DieType.D8)
                            .Build())
                    .Build())
            .SetOverriddenPower(PowerMartialCommanderInvigoratingShout)
            .AddToDB();

        var featureSetPeerlessCommander = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetMartialCommanderPeerlessCommander")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerMartialCommanderPeerlessCommander)
            .AddToDB();


        MartialCommander.FeatureUnlocks.Add(new FeatureUnlockByLevel(featureSetPeerlessCommander, 18));

        //
        // Mountaineer
        //

        var attributeModifierMartialMountaineerPositionOfStrength = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierMartialMountaineerPositionOfStrength")
            .SetGuiPresentation("FeatureSetMartialMountaineerPositionOfStrength", Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 2)
            .SetSituationalContext(
                ExtraSituationalContext.NextToWallWithShieldAndMaxMediumArmorAndConsciousAllyNextToTarget)
            .SetCustomSubFeatures(new CustomCodePositionOfStrength())
            .AddToDB();

        var attributeModifierMartialMountaineerPositionOfStrengthAura = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierMartialMountaineerPositionOfStrengthAura")
            .SetGuiPresentation("FeatureSetMartialMountaineerPositionOfStrength", Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 1)
            .AddToDB();

        var conditionMartialMountaineerPositionOfStrengthAura = ConditionDefinitionBuilder
            .Create("ConditionMartialMountaineerPositionOfStrengthAura")
            .SetGuiPresentation("FeatureSetMartialMountaineerPositionOfStrength", Category.Feature)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddFeatures(attributeModifierMartialMountaineerPositionOfStrengthAura)
            .AddToDB();

        var powerMartialMountaineerPositionOfStrengthAura = FeatureDefinitionPowerBuilder
            .Create("PowerMartialMountaineerPositionOfStrengthAura")
            .SetGuiPresentation("FeatureSetMartialMountaineerPositionOfStrength", Category.Feature)
            .SetUsesFixed(ActivationTime.PermanentUnlessIncapacitated)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Cube, 3)
                    .SetDurationData(DurationType.Permanent)
                    .SetRecurrentEffect(RecurrentEffect.OnTurnStart | RecurrentEffect.OnTurnEnd)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionMartialMountaineerPositionOfStrengthAura,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        var featureSetMartialMountaineerPositionOfStrength = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetMartialMountaineerPositionOfStrength")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                attributeModifierMartialMountaineerPositionOfStrength,
                powerMartialMountaineerPositionOfStrengthAura)
            .AddToDB();

        MartialMountaineer.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(featureSetMartialMountaineerPositionOfStrength, 18));

        //
        // Spellblade
        //

        MartialSpellblade.FeatureUnlocks.Add(new FeatureUnlockByLevel(AttackReplaceWithCantripCasterFighting, 18));
    }

    private static void MonkLoad()
    {
        //
        // Freedom
        //

        var attributeModifierTraditionLightPurityOfLight = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierTraditionLightPurityOfLight")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.ForceIfBetter, AttributeDefinitions.AttacksNumber, 3)
            .AddToDB();

        TraditionFreedom.FeatureUnlocks.Add(new FeatureUnlockByLevel(attributeModifierTraditionLightPurityOfLight, 17));

        //
        // Light
        //

        var powerTraditionLightPurityOfLight = FeatureDefinitionPowerBuilder
            .Create(PowerTraditionLightLuminousKi, "PowerTraditionLightPurityOfLight")
            .SetUsesFixed(ActivationTime.OnAttackHitAuto)
            .SetOverriddenPower(PowerTraditionLightLuminousKi)
            .AddToDB();

        var additionalDamageTraditionLightRadiantStrikesLuminousKiD6 = FeatureDefinitionAdditionalDamageBuilder
            .Create(AdditionalDamageTraditionLightRadiantStrikesLuminousKi,
                "AdditionalDamageTraditionLightRadiantStrikesLuminousKiD6")
            .SetNotificationTag("RadiantStrikes")
            .SetDamageDice(DieType.D6, 1)
            .AddToDB();

        var additionalDamageTraditionLightRadiantStrikesShineD6 = FeatureDefinitionAdditionalDamageBuilder
            .Create(AdditionalDamageTraditionLightRadiantStrikesShine,
                "AdditionalDamageTraditionLightRadiantStrikesShineD6")
            .SetNotificationTag("RadiantStrikes")
            .SetDamageDice(DieType.D6, 1)
            .AddToDB();

        var featureTraditionLightPurityOfLife = FeatureDefinitionBuilder
            .Create("FeatureTraditionLightPurityOfLife")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(new CustomBehaviorPurityOfLight())
            .AddToDB();

        var featureSetPurityOfLife = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetTraditionLightPurityOfLight")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                powerTraditionLightPurityOfLight,
                additionalDamageTraditionLightRadiantStrikesLuminousKiD6,
                additionalDamageTraditionLightRadiantStrikesShineD6,
                featureTraditionLightPurityOfLife)
            .AddToDB();

        TraditionLight.FeatureUnlocks.Add(new FeatureUnlockByLevel(featureSetPurityOfLife, 17));

        //
        // Open Hand
        //

        var conditionTraditionOpenHandQuiveringPalm = ConditionDefinitionBuilder
            .Create("ConditionTraditionOpenHandQuiveringPalm")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionRestrictedInsideMagicCircle)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .SetSpecialDuration(DurationType.Day, 1)
            .SetSpecialInterruptions(ConditionInterruption.BattleEnd)
            .CopyParticleReferences(ConditionDefinitions.ConditionBaned)
            .SetFeatures(
                FeatureDefinitionBuilder
                    .Create("SavingThrowAfterRollQuiveringPalm")
                    .SetGuiPresentationNoContent(true)
                    .SetCustomSubFeatures(new SavingThrowAfterRollQuiveringPalm())
                    .AddToDB())
            .AddToDB();

        var powerTraditionOpenHandQuiveringPalmTrigger = FeatureDefinitionPowerBuilder
            .Create("PowerTraditionOpenHandQuiveringPalmTrigger")
            .SetGuiPresentation("FeatureSetTraditionOpenHandQuiveringPalm", Category.Feature,
                Sprites.GetSprite("PowerQuiveringPalm", Resources.PowerQuiveringPalm, 256, 128))
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 24, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Instantaneous)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Dexterity)
                    .SetParticleEffectParameters(DreadfulOmen)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeNecrotic, 10, DieType.D10)
                            .Build())
                    .Build())
            .AddToDB();

        powerTraditionOpenHandQuiveringPalmTrigger.SetCustomSubFeatures(
            new CustomBehaviorQuiveringPalmTrigger(
                powerTraditionOpenHandQuiveringPalmTrigger,
                conditionTraditionOpenHandQuiveringPalm));

        var powerTraditionOpenHandQuiveringPalm = FeatureDefinitionPowerBuilder
            .Create("PowerTraditionOpenHandQuiveringPalm")
            .SetGuiPresentation("FeatureSetTraditionOpenHandQuiveringPalm", Category.Feature, hidden: true)
            .SetUsesFixed(ActivationTime.OnAttackHitMeleeAuto, RechargeRate.KiPoints, 3)
            .SetAutoActivationPowerTag("9024") // this is the action ID for Quivering Palm
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Self, 1, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Instantaneous)
                    .SetParticleEffectParameters(Bane)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionTraditionOpenHandQuiveringPalm,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        powerTraditionOpenHandQuiveringPalm.SetCustomSubFeatures(
            ForcePowerUseInSpendPowerAction.Marker,
            new ActionFinishedQuiveringPalm(
                powerTraditionOpenHandQuiveringPalm,
                conditionTraditionOpenHandQuiveringPalm));

        _ = ActionDefinitionBuilder
            .Create(DatabaseHelper.ActionDefinitions.StunningStrikeToggle, "TraditionOpenHandQuiveringPalmToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .SetActionId(ExtraActionId.QuiveringPalmToggle)
            .SetActivatedPower(powerTraditionOpenHandQuiveringPalm, ActionDefinitions.ActionParameter.TogglePower)
            .RequiresAuthorization()
            .AddToDB();

        _ = DamageDefinitionBuilder
            .Create("DamagePure")
            .SetGuiPresentation(Category.Rules)
            .AddToDB();

        var actionAffinityTraditionOpenHandQuiveringPalm = FeatureDefinitionActionAffinityBuilder
            .Create("ActionAffinityTraditionOpenHandQuiveringPalm")
            .SetGuiPresentation("FeatureSetTraditionOpenHandQuiveringPalm", Category.Feature)
            .SetAllowedActionTypes()
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.QuiveringPalmToggle)
            .AddToDB();

        var featureSetTraditionOpenHandQuiveringPalm = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetTraditionOpenHandQuiveringPalm")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                powerTraditionOpenHandQuiveringPalmTrigger,
                powerTraditionOpenHandQuiveringPalm,
                actionAffinityTraditionOpenHandQuiveringPalm)
            .AddToDB();

        TraditionOpenHand.FeatureUnlocks.Add(new FeatureUnlockByLevel(featureSetTraditionOpenHandQuiveringPalm, 17));

        //
        // Survival
        //

        var conditionTraditionSurvivalPhysicalPerfection = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionTraditionSurvivalUnbreakableBody,
                "ConditionTraditionSurvivalPhysicalPerfection")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .SetRecurrentEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetBonusMode(AddBonusMode.Proficiency)
                    .SetHealingForm(HealingComputation.Dice, 0, DieType.D1, 0, false, HealingCap.MaximumHitPoints)
                    .Build())
            .SetCustomSubFeatures(new ModifyMagicEffectRecurrentPhysicalPerfection())
            .AddToDB();

        var powerTraditionSurvivalPhysicalPerfection = FeatureDefinitionPowerBuilder
            .Create(PowerTraditionSurvivalUnbreakableBody, "PowerTraditionSurvivalPhysicalPerfection")
            .SetGuiPresentation(Category.Feature, PowerTraditionSurvivalUnbreakableBody) // source is hidden
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerTraditionSurvivalUnbreakableBody)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionTraditionSurvivalPhysicalPerfection,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .SetOverriddenPower(PowerTraditionSurvivalUnbreakableBody)
            .AddToDB();

        powerTraditionSurvivalPhysicalPerfection.SetCustomSubFeatures(
            new SourceReducedToZeroHpPhysicalPerfection(powerTraditionSurvivalPhysicalPerfection));

        TraditionSurvival.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerTraditionSurvivalPhysicalPerfection, 17));
    }

    private static void PaladinLoad()
    {
        AutoPreparedSpellsOathOfDevotion.AutoPreparedSpellsGroups.Add(
            BuildSpellGroup(13, GuardianOfFaith, FreedomOfMovement));

        AutoPreparedSpellsOathOfJugement.AutoPreparedSpellsGroups.Add(
            BuildSpellGroup(13, Banishment, Blight));

        AutoPreparedSpellsOathOfMotherland.AutoPreparedSpellsGroups.Add(
            BuildSpellGroup(13, WallOfFire, FireShield));

        AutoPreparedSpellsOathOfTirmar.AutoPreparedSpellsGroups.Add(
            BuildSpellGroup(13, DreadfulOmen, PhantasmalKiller));
    }

    private static void RogueLoad()
    {
        //
        // Darkweaver
        //

        var additionalActionRoguishDarkweaverDarkAssault = FeatureDefinitionAdditionalActionBuilder
            .Create("AdditionalActionRoguishDarkweaverDarkAssault")
            .SetGuiPresentationNoContent(true)
            .SetActionType(ActionDefinitions.ActionType.Main)
            .SetRestrictedActions(ActionDefinitions.Id.AttackMain)
            .SetMaxAttacksNumber(1)
            .AddToDB();

        var movementAffinityRoguishDarkweaverDarkAssault = FeatureDefinitionMovementAffinityBuilder
            .Create("MovementAffinityRoguishDarkweaverDarkAssault")
            .SetGuiPresentationNoContent(true)
            .SetBaseSpeedAdditiveModifier(3)
            .AddToDB();

        var conditionRoguishDarkweaverDarkAssault = ConditionDefinitionBuilder
            .Create("ConditionRoguishDarkweaverDarkAssault")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionStealthy)
            .SetPossessive()
            .SetSpecialDuration()
            .AddFeatures(additionalActionRoguishDarkweaverDarkAssault, movementAffinityRoguishDarkweaverDarkAssault)
            .AddToDB();

        var featureRoguishDarkweaverDarkAssault = FeatureDefinitionBuilder
            .Create("FeatureRoguishDarkweaverDarkAssault")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new CustomBehaviorDarkAssault(conditionRoguishDarkweaverDarkAssault))
            .AddToDB();

        RoguishDarkweaver.FeatureUnlocks.Add(new FeatureUnlockByLevel(featureRoguishDarkweaverDarkAssault, 17));

        //
        // Hoodlum
        //

        PowerRoguishHoodlumDirtyFighting.SetCustomSubFeatures(new AttackAfterMagicEffectBrutalAssault());

        var featureRoguishHoodlumBrutalAssault = FeatureDefinitionBuilder
            .Create("FeatureRoguishHoodlumBrutalAssault")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(
                new CustomAdditionalDamageBrutalAssault(FeatureDefinitionAdditionalDamageBuilder
                    .Create("AdditionalDamageRoguishHoodlumBrutalAssault")
                    .SetGuiPresentation("FeatureRoguishHoodlumBrutalAssault", Category.Feature)
                    .SetNotificationTag("BrutalAssault")
                    .SetDamageValueDetermination(AdditionalDamageValueDetermination.ProficiencyBonus)
                    .AddToDB()))
            .AddToDB();

        RoguishHoodlum.FeatureUnlocks.Add(new FeatureUnlockByLevel(featureRoguishHoodlumBrutalAssault, 17));

        //
        // Shadowcaster
        //

        var conditionRoguishShadowcasterShadowForm = ConditionDefinitionBuilder
            .Create("ConditionRoguishShadowcasterShadowForm")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionChildOfDarkness_DimLight)
            .SetPossessive()
            .CopyParticleReferences(ConditionDefinitions.ConditionMalediction)
            .AddFeatures(
                FeatureDefinitionMovementAffinitys.MovementAffinityFreedomOfMovement,
                FeatureDefinitionCombatAffinitys.CombatAffinityDisengaging,
                DamageAffinityAcidResistance,
                DamageAffinityColdResistance,
                DamageAffinityFireResistance,
                DamageAffinityLightningResistance,
                DamageAffinityNecroticResistance,
                DamageAffinityPoisonResistance,
                DamageAffinityPsychicResistance,
                DamageAffinityThunderResistance,
                FeatureDefinitionDamageAffinityBuilder
                    .Create("DamageAffinityRoguishShadowcasterShadowFormResistanceBludgeoning")
                    .SetGuiPresentationNoContent(true)
                    .SetDamageType(DamageTypeBludgeoning)
                    .SetDamageAffinityType(DamageAffinityType.Resistance)
                    .AddToDB(),
                FeatureDefinitionDamageAffinityBuilder
                    .Create("DamageAffinityRoguishShadowcasterShadowFormResistancePiercing")
                    .SetGuiPresentationNoContent(true)
                    .SetDamageType(DamageTypePiercing)
                    .SetDamageAffinityType(DamageAffinityType.Resistance)
                    .AddToDB(),
                FeatureDefinitionDamageAffinityBuilder
                    .Create("DamageAffinityRoguishShadowcasterShadowFormResistanceSlashing")
                    .SetGuiPresentationNoContent(true)
                    .SetDamageType(DamageTypeSlashing)
                    .SetDamageAffinityType(DamageAffinityType.Resistance)
                    .AddToDB())
            .AddToDB();

        var powerRoguishShadowcasterShadowForm = FeatureDefinitionPowerBuilder
            .Create("PowerRoguishShadowcasterShadowForm")
            .SetGuiPresentation(Category.Feature, Darkvision)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Minute, 1)
                .SetParticleEffectParameters(Malediction)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(conditionRoguishShadowcasterShadowForm, ConditionForm.ConditionOperation.Add)
                        .Build())
                .Build())
            .AddToDB();

        RoguishShadowCaster.FeatureUnlocks.Add(new FeatureUnlockByLevel(powerRoguishShadowcasterShadowForm, 17));

        //
        // Thief
        //

        var featureRoguishThiefThiefReflexes = FeatureDefinitionBuilder
            .Create("FeatureRoguishThiefThiefReflexes")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new InitiativeEndListenerThiefReflexes())
            .AddToDB();

        RoguishThief.FeatureUnlocks.Add(new FeatureUnlockByLevel(featureRoguishThiefThiefReflexes, 17));
    }

    private static void SorcererLoad()
    {
    }

    #region Fighter

    //
    // Position of Strength
    //

    private sealed class CustomCodePositionOfStrength : IFeatureDefinitionCustomCode
    {
        public void ApplyFeature(RulesetCharacterHero hero, string tag)
        {
            foreach (var featureDefinitions in hero.ActiveFeatures.Values)
            {
                featureDefinitions.RemoveAll(x => x == AttributeModifierMartialMountainerTunnelFighter);
            }
        }

        public void RemoveFeature(RulesetCharacterHero hero, string tag)
        {
            // empty
        }
    }

    //
    // Survivor
    //

    private sealed class CharacterTurnStartListenerSurvivor : ICharacterTurnStartListener
    {
        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            var rulesetCharacter = locationCharacter.RulesetCharacter;

            if (rulesetCharacter == null || rulesetCharacter.IsDeadOrDyingOrUnconscious)
            {
                return;
            }

            if (rulesetCharacter.CurrentHitPoints >= rulesetCharacter.MissingHitPoints)
            {
                return;
            }

            var constitution = rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Constitution);
            var constitutionModifier = AttributeDefinitions.ComputeAbilityScoreModifier(constitution);
            var totalHealing = 5 + constitutionModifier;

            EffectHelpers.StartVisualEffect(
                locationCharacter, locationCharacter, Heal, EffectHelpers.EffectType.Effect);
            rulesetCharacter.ReceiveHealing(totalHealing, true, rulesetCharacter.Guid);
        }
    }

    #endregion

    #region Monk

    //
    // Physical Perfection
    //

    private sealed class ModifyMagicEffectRecurrentPhysicalPerfection : IModifyMagicEffectRecurrent
    {
        public void ModifyEffect(RulesetCondition rulesetCondition, EffectForm effectForm, RulesetActor rulesetActor)
        {
            if (rulesetActor is not RulesetCharacter rulesetCharacter)
            {
                return;
            }

            var monkLevel = rulesetCharacter.GetClassLevel(CharacterClassDefinitions.Monk);

            if (monkLevel < 17)
            {
                return;
            }

            if (rulesetCharacter.CurrentHitPoints >= rulesetCharacter.MissingHitPoints)
            {
                return;
            }

            var pb = rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);

            effectForm.HealingForm.bonusHealing = pb;
        }
    }

    private sealed class SourceReducedToZeroHpPhysicalPerfection : ISourceReducedToZeroHp
    {
        private readonly FeatureDefinitionPower _powerPhysicalPerfection;

        public SourceReducedToZeroHpPhysicalPerfection(FeatureDefinitionPower powerPhysicalPerfection)
        {
            _powerPhysicalPerfection = powerPhysicalPerfection;
        }

        public IEnumerator HandleSourceReducedToZeroHp(
            GameLocationCharacter attacker,
            GameLocationCharacter source,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            var rulesetCharacter = source.RulesetCharacter;

            if (rulesetCharacter == null)
            {
                yield break;
            }

            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var battle = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (manager == null || battle == null)
            {
                yield break;
            }

            var reactionParams = new CharacterActionParams(source, (ActionDefinitions.Id)ExtraActionId.DoNothingFree);
            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("PhysicalPerfection", reactionParams);

            manager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(attacker, manager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            var usablePower = UsablePowersProvider.Get(_powerPhysicalPerfection, rulesetCharacter);
            var effectPower = new RulesetEffectPower(rulesetCharacter, usablePower);

            rulesetCharacter.ForceKiPointConsumption(1);
            rulesetCharacter.StabilizeAndGainHitPoints(10);
            rulesetCharacter.InflictCondition(
                ConditionDodging,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfSourceTurn,
                AttributeDefinitions.TagCombat,
                attacker.Guid,
                attacker.RulesetCharacter?.CurrentFaction.Name ?? string.Empty,
                1,
                null,
                0,
                0,
                0);
            effectPower.ApplyEffectOnCharacter(rulesetCharacter, true, source.LocationPosition);

            ServiceRepository.GetService<ICommandService>()
                ?.ExecuteAction(new CharacterActionParams(source, ActionDefinitions.Id.StandUp), null, false);
        }
    }

    //
    // Purity of Light
    //

    private sealed class CustomBehaviorPurityOfLight : IFeatureDefinitionCustomCode, IPhysicalAttackFinished
    {
        public void ApplyFeature(RulesetCharacterHero hero, string tag)
        {
            foreach (var featureDefinitions in hero.ActiveFeatures.Values)
            {
                featureDefinitions.RemoveAll(x =>
                    x == FeatureDefinitionFeatureSets.FeatureSetTraditionLightRadiantStrikes);
            }
        }

        public void RemoveFeature(RulesetCharacterHero hero, string tag)
        {
            // empty
        }

        public IEnumerator OnAttackFinished(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackerAttackMode,
            RollOutcome attackRollOutcome,
            int damageAmount)
        {
            if (attackRollOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure)
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender == null || rulesetDefender.IsDeadOrDying)
            {
                yield break;
            }

            if (!rulesetDefender.HasAnyConditionOfType(
                    ConditionDefinitions.ConditionLuminousKi.Name,
                    ConditionDefinitions.ConditionShine.Name))
            {
                yield break;
            }

            var gameLocationBattleService =
                ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (gameLocationBattleService == null)
            {
                yield break;
            }

            attacker.RulesetCharacter.ReceiveHealing(2, true, attacker.Guid);

            foreach (var ally in gameLocationBattleService.Battle.AllContenders
                         .Where(x => x.Side == attacker.Side &&
                                     x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                                     gameLocationBattleService.IsWithinXCells(attacker, x, 4)))
            {
                ally.RulesetCharacter.ReceiveHealing(2, true, attacker.Guid);
            }
        }
    }

    //
    // Quivering Palm
    //

    private sealed class CustomBehaviorQuiveringPalmTrigger : IFilterTargetingMagicEffect, IActionFinished
    {
        private readonly ConditionDefinition _conditionDefinition;
        private readonly FeatureDefinitionPower _featureDefinitionPower;

        public CustomBehaviorQuiveringPalmTrigger(
            FeatureDefinitionPower featureDefinitionPower,
            ConditionDefinition conditionDefinition)
        {
            _featureDefinitionPower = featureDefinitionPower;
            _conditionDefinition = conditionDefinition;
        }


        public IEnumerator OnActionFinished(CharacterAction characterAction)
        {
            if (characterAction is not CharacterActionUsePower characterActionUsePower ||
                characterActionUsePower.activePower.PowerDefinition != _featureDefinitionPower)
            {
                yield break;
            }

            if (characterAction.ActionParams.TargetCharacters.Count == 0)
            {
                yield break;
            }

            var rulesetDefender = characterAction.ActionParams.TargetCharacters[0].RulesetCharacter;
            var rulesetCondition = rulesetDefender?.AllConditions
                .FirstOrDefault(x => x.ConditionDefinition == _conditionDefinition);

            if (rulesetCondition != null)
            {
                rulesetDefender.RemoveCondition(rulesetCondition);
            }
        }

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            if (__instance.actionParams.RulesetEffect is not RulesetEffectPower rulesetEffectPower ||
                rulesetEffectPower.PowerDefinition != _featureDefinitionPower)
            {
                return true;
            }

            if (target.RulesetCharacter == null)
            {
                return true;
            }

            var isValid = target.RulesetCharacter.HasConditionOfType("ConditionTraditionOpenHandQuiveringPalm");

            if (!isValid)
            {
                __instance.actionModifier.FailureFlags.Add("Tooltip/&MustHaveQuiveringPalmCondition");
            }

            return isValid;
        }
    }

    private sealed class SavingThrowAfterRollQuiveringPalm : ISavingThrowAfterRoll
    {
        public void OnSavingThrowAfterRoll(
            RulesetCharacter caster,
            Side sourceSide,
            RulesetActor target,
            ActionModifier actionModifier,
            bool hasHitVisual,
            bool hasSavingThrow,
            string savingThrowAbility,
            int saveDC,
            bool disableSavingThrowOnAllies,
            bool advantageForEnemies,
            bool ignoreCover,
            FeatureSourceType featureSourceType,
            List<EffectForm> effectForms,
            List<SaveAffinityBySenseDescription> savingThrowAffinitiesBySense,
            List<SaveAffinityByFamilyDescription> savingThrowAffinitiesByFamily,
            string sourceName,
            BaseDefinition sourceDefinition,
            string schoolOfMagic,
            MetamagicOptionDefinition metamagicOption,
            ref RollOutcome saveOutcome,
            ref int saveOutcomeDelta)
        {
            if (target is not RulesetCharacter rulesetTarget)
            {
                return;
            }

            var rulesetCondition = rulesetTarget.AllConditions.FirstOrDefault(x =>
                x.ConditionDefinition.Name == "ConditionTraditionOpenHandQuiveringPalm");

            if (rulesetCondition != null)
            {
                rulesetTarget.RemoveCondition(rulesetCondition);
            }

            if (saveOutcome is not (RollOutcome.Failure or RollOutcome.CriticalFailure))
            {
                return;
            }

            var totalDamage = rulesetTarget.CurrentHitPoints + rulesetTarget.TemporaryHitPoints - 1;

            target.SustainDamage(totalDamage, "DamagePure", false, caster.Guid, null, out _);
            effectForms.SetRange(
                EffectFormBuilder
                    .Create()
                    .SetConditionForm(ConditionDefinitions.ConditionStunned_MonkStunningStrike,
                        ConditionForm.ConditionOperation.Add)
                    .Build());
        }
    }

    private sealed class ActionFinishedQuiveringPalm : IActionFinished
    {
        private readonly ConditionDefinition _conditionDefinition;
        private readonly FeatureDefinitionPower _featureDefinitionPower;

        public ActionFinishedQuiveringPalm(
            FeatureDefinitionPower featureDefinitionPower,
            ConditionDefinition conditionDefinition)
        {
            _featureDefinitionPower = featureDefinitionPower;
            _conditionDefinition = conditionDefinition;
        }

        public IEnumerator OnActionFinished(CharacterAction action)
        {
            var battle = Gui.Battle;

            if (battle == null || action is not CharacterActionUsePower characterActionUsePower ||
                characterActionUsePower.activePower.PowerDefinition != _featureDefinitionPower)
            {
                yield break;
            }

            var gameLocationDefender = action.actionParams.targetCharacters[0];

            // remove this condition from all other enemies
            foreach (var gameLocationCharacter in battle.EnemyContenders
                         .Where(x =>
                             x.RulesetCharacter is { IsDeadOrDying: false } &&
                             x != gameLocationDefender)
                         .ToList())
            {
                var rulesetDefender = gameLocationCharacter.RulesetCharacter;
                var rulesetCondition = rulesetDefender.AllConditions
                    .FirstOrDefault(x => x.ConditionDefinition == _conditionDefinition);

                if (rulesetCondition != null)
                {
                    rulesetDefender.RemoveCondition(rulesetCondition);
                }
            }
        }
    }

    #endregion

    #region Rogue

    //
    // Brutal Assault
    //

    private sealed class AttackAfterMagicEffectBrutalAssault : IAttackAfterMagicEffect
    {
        public IAttackAfterMagicEffect.CanAttackHandler CanAttack { get; } =
            CanMeleeAttack;

        public IAttackAfterMagicEffect.GetAttackAfterUseHandler PerformAttackAfterUse { get; } =
            DefaultAttackHandler;

        public IAttackAfterMagicEffect.CanUseHandler CanBeUsedToAttack { get; } =
            DefaultCanUseHandler;

        private static bool CanMeleeAttack([NotNull] GameLocationCharacter caster, GameLocationCharacter target)
        {
            var attackMode = caster.FindActionAttackMode(ActionDefinitions.Id.AttackMain);

            if (attackMode == null)
            {
                return false;
            }

            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (battleService == null)
            {
                return false;
            }

            var attackModifier = new ActionModifier();
            var evalParams = new BattleDefinitions.AttackEvaluationParams();

            evalParams.FillForPhysicalReachAttack(caster, caster.LocationPosition, attackMode, target,
                target.LocationPosition, attackModifier);

            return battleService.CanAttack(evalParams);
        }

        [NotNull]
        private static List<CharacterActionParams> DefaultAttackHandler([CanBeNull] CharacterActionMagicEffect effect)
        {
            var attacks = new List<CharacterActionParams>();
            var actionParams = effect?.ActionParams;

            if (actionParams == null)
            {
                return attacks;
            }

            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (battleService == null)
            {
                return attacks;
            }

            var caster = actionParams.ActingCharacter;
            var targets = actionParams.TargetCharacters
                .Where(x => x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                            x.RulesetCharacter.HasAnyConditionOfType("ConditionHitByDirtyFighting"))
                .ToList();

            if (caster == null || targets.Empty())
            {
                return attacks;
            }

            var attackMode = caster.FindActionAttackMode(ActionDefinitions.Id.AttackMain);

            if (attackMode == null)
            {
                return attacks;
            }

            //get copy to be sure we don't break existing mode
            var rulesetAttackModeCopy = RulesetAttackMode.AttackModesPool.Get();

            rulesetAttackModeCopy.Copy(attackMode);

            attackMode = rulesetAttackModeCopy;

            //set action type to be same as the one used for the magic effect
            attackMode.ActionType = effect.ActionType;

            var attackModifier = new ActionModifier();

            foreach (var target in targets
                         .Where(t => CanMeleeAttack(caster, t)))
            {
                var attackActionParams =
                    new CharacterActionParams(caster, ActionDefinitions.Id.AttackFree) { AttackMode = attackMode };

                attackActionParams.TargetCharacters.Add(target);
                attackActionParams.ActionModifiers.Add(attackModifier);
                attacks.Add(attackActionParams);
            }

            return attacks;
        }

        private static bool DefaultCanUseHandler(
            [NotNull] CursorLocationSelectTarget targeting,
            GameLocationCharacter caster,
            GameLocationCharacter target, [NotNull] out string failure)
        {
            failure = string.Empty;

            return true;
        }
    }

    private sealed class CustomAdditionalDamageBrutalAssault : CustomAdditionalDamage
    {
        public CustomAdditionalDamageBrutalAssault(IAdditionalDamageProvider provider) : base(provider)
        {
        }

        internal override bool IsValid(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool criticalHit,
            bool firstTarget,
            out CharacterActionParams reactionParams)
        {
            reactionParams = null;

            var rulesetDefender = defender.RulesetCharacter;

            if (attackMode == null || rulesetDefender == null || rulesetDefender.IsDeadOrDyingOrUnconscious)
            {
                return false;
            }

            return rulesetDefender.HasAnyConditionOfType(
                ConditionBlinded,
                ConditionFrightened,
                ConditionRestrained,
                ConditionIncapacitated,
                ConditionParalyzed,
                ConditionPoisoned,
                ConditionProne,
                ConditionStunned);
        }
    }

    //
    // Dark Assault
    //

    private sealed class CustomBehaviorDarkAssault : ICharacterTurnStartListener
    {
        private readonly ConditionDefinition _conditionDarkAssault;

        public CustomBehaviorDarkAssault(ConditionDefinition conditionDarkAssault)
        {
            _conditionDarkAssault = conditionDarkAssault;
        }

        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            var rulesetCharacter = locationCharacter.RulesetCharacter;

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false } ||
                !ValidatorsCharacter.IsNotInBrightLight(rulesetCharacter) ||
                rulesetCharacter.HasConditionOfType(_conditionDarkAssault.Name))
            {
                return;
            }

            EffectHelpers.StartVisualEffect(
                locationCharacter, locationCharacter, PowerShadowcasterShadowDodge, EffectHelpers.EffectType.Caster);
            rulesetCharacter.InflictCondition(
                _conditionDarkAssault.Name,
                _conditionDarkAssault.DurationType,
                _conditionDarkAssault.DurationParameter,
                _conditionDarkAssault.TurnOccurence,
                AttributeDefinitions.TagCombat,
                rulesetCharacter.Guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
            rulesetCharacter.RefreshAttackModes();
        }
    }

    //
    // Thief's Reflexes
    //

    private sealed class InitiativeEndListenerThiefReflexes : IInitiativeEndListener, ICharacterTurnEndListener
    {
        public void OnCharacterTurnEnded(GameLocationCharacter locationCharacter)
        {
            var battle = Gui.Battle;

            if (battle.CurrentRound > 1)
            {
                return;
            }

            var index = battle.InitiativeSortedContenders.FindLastIndex(x => x.Guid == locationCharacter.Guid);

            if (battle.activeContenderIndex != index)
            {
                return;
            }

            battle.InitiativeSortedContenders.RemoveAt(index);

            var gameLocationScreenBattle = Gui.GuiService.GetScreen<GameLocationScreenBattle>();

            gameLocationScreenBattle.initiativeTable.ContenderModified(locationCharacter,
                GameLocationBattle.ContenderModificationMode.Remove, false, false);
        }

        public IEnumerator OnInitiativeEnded(GameLocationCharacter locationCharacter)
        {
            var initiative = locationCharacter.LastInitiative - 10;
            var initiativeSortedContenders = Gui.Battle.InitiativeSortedContenders;
            var positionCharacter = initiativeSortedContenders.FirstOrDefault(x => x.LastInitiative < initiative);

            if (positionCharacter == null)
            {
                initiativeSortedContenders.Add(locationCharacter);

                yield break;
            }

            var positionCharacterIndex = initiativeSortedContenders.IndexOf(positionCharacter);

            initiativeSortedContenders.Insert(positionCharacterIndex, locationCharacter);
        }
    }

    #endregion
}
