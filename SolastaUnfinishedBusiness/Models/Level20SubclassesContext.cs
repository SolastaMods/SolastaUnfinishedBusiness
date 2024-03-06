using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;
using MirrorImage = SolastaUnfinishedBusiness.Behaviors.Specific.MirrorImage;

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
        //
        // Battle
        //

        // Paragon of Battle

        var powerDomainBattleImprovedHeraldOfBattle = FeatureDefinitionPowerBuilder
            .Create(PowerDomainBattleHeraldOfBattle, "PowerDomainBattleImprovedHeraldOfBattle")
            .SetOverriddenPower(PowerDomainBattleHeraldOfBattle)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerDomainBattleHeraldOfBattle)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 3)
                    .Build())
            .AddToDB();

        var featureSetDomainBattleParagonOfBattle = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetDomainBattleParagonOfBattle")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                DamageAffinityBludgeoningResistance,
                DamageAffinityPiercingResistance,
                DamageAffinitySlashingResistance,
                powerDomainBattleImprovedHeraldOfBattle)
            .AddToDB();

        DomainBattle.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(featureSetDomainBattleParagonOfBattle, 17));

        //
        // Cold
        //

        // Summon Blizzard

        var powerDomainColdSummonBlizzard = FeatureDefinitionPowerBuilder
            .Create("PowerDomainColdSummonBlizzard")
            .SetGuiPresentation(Category.Feature, ConjureElemental)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Hour, 1)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 18, TargetType.Position)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetSummonCreatureForm(2, "Ice_Elemental")
                            .Build())
                    .SetParticleEffectParameters(ConjureElementalFire)
                    .SetCasterEffectParameters(SleetStorm)
                    .Build())
            .AddToDB();

        DomainElementalCold.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerDomainColdSummonBlizzard, 17));

        //
        // Fire
        //

        // Summon Inferno

        var powerDomainFireSummonInferno = FeatureDefinitionPowerBuilder
            .Create("PowerDomainFireSummonInferno")
            .SetGuiPresentation(Category.Feature, ConjureElemental)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Hour, 1)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 18, TargetType.Position)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetSummonCreatureForm(2, "Fire_Elemental")
                            .Build())
                    .SetParticleEffectParameters(ConjureElementalFire)
                    .Build())
            .AddToDB();

        DomainElementalFire.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerDomainFireSummonInferno, 17));

        //
        // Insight
        //

        // Avatar of Knowledge

        var proficiencyDomainInsightAvatarOfKnowledge = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyDomainInsightAvatarOfKnowledgeSavingThrow")
            .SetGuiPresentation("SavingThrowProficiency", Category.Feature)
            .SetProficiencies(ProficiencyType.SavingThrow, AttributeDefinitions.Intelligence)
            .AddToDB();

        var powerDomainInsightAvatarOfKnowledge = FeatureDefinitionPowerBuilder
            .Create(PowerDomainInsightForeknowledge, "PowerDomainInsightAvatarOfKnowledge")
            .SetOverriddenPower(PowerDomainInsightForeknowledge)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerDomainInsightForeknowledge)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 3)
                    .Build())
            .AddToDB();

        var featureSetDomainInsightAvatarOfKnowledge = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetDomainInsightAvatarOfKnowledge")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(proficiencyDomainInsightAvatarOfKnowledge, powerDomainInsightAvatarOfKnowledge)
            .AddToDB();

        DomainInsight.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(featureSetDomainInsightAvatarOfKnowledge, 17));

        //
        // Law
        //

        // Final Word

        var featureFinalWord = FeatureDefinitionBuilder
            .Create("FeatureDomainLawFinalWord")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new CustomBehaviorFinalWord())
            .AddToDB();

        PowerDomainLawWordOfLaw.AddCustomSubFeatures(new CustomBehaviorWordOfLaw());

        DomainLaw.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(featureFinalWord, 17));

        //
        // Life
        //

        // Supreme Healing

        var featureDomainLifeSupremeHealing = FeatureDefinitionBuilder
            .Create("DomainLifeSupremeHealing")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new ModifyDiceRollSupremeHealing())
            .AddToDB();

        DomainLife.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(featureDomainLifeSupremeHealing, 17));

        //
        // Lightning
        //

        // Living Tempest

        var powerDomainLightningLivingTempestSprout = FeatureDefinitionPowerBuilder
            .Create("PowerDomainLightningLivingTempestSprout")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerLivingTempest", Resources.PowerLivingTempest, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Permanent)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(ConditionDefinitions.ConditionFlyingAdaptive,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(PowerDomainElementalHeraldOfTheElementsThunder)
                    .Build())
            .AddCustomSubFeatures(
                new ValidatorsValidatePowerUse(ValidatorsCharacter.HasNoneOfConditions(ConditionFlyingAdaptive)))
            .AddToDB();

        var powerDomainLightningLivingTempestDismiss = FeatureDefinitionPowerBuilder
            .Create("PowerDomainLightningLivingTempestDismiss")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerLivingTempest", Resources.PowerLivingTempest, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionFlyingAdaptive,
                            ConditionForm.ConditionOperation.Remove))
                    .Build())
            .AddCustomSubFeatures(
                new ValidatorsValidatePowerUse(ValidatorsCharacter.HasAnyOfConditions(ConditionFlyingAdaptive)))
            .AddToDB();

        var featureSetDomainLightningLivingTempest = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetDomainLightningLivingTempest")
            .SetGuiPresentation("PowerDomainLightningLivingTempestSprout", Category.Feature)
            .AddFeatureSet(powerDomainLightningLivingTempestSprout, powerDomainLightningLivingTempestDismiss)
            .AddToDB();

        DomainElementalLighting.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(featureSetDomainLightningLivingTempest, 17));

        //
        // Mischief
        //

        // Fortune Favor The Bold

        var conditionAdvantage = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionStrikeOfChaosAttackAdvantage, "ConditionFortuneFavorTheBoldAdvantage")
            .SetConditionType(ConditionType.Beneficial)
            .SetSpecialInterruptions(ConditionInterruption.Attacks)
            .AddToDB();

        var conditionDisadvantage = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionStrikeOfChaosAttackDisadvantage,
                "ConditionFortuneFavorTheBoldDisadvantage")
            .SetSpecialInterruptions(ConditionInterruption.Attacks)
            .AddToDB();

        var conditionMirrorImage = ConditionDefinitionBuilder
            .Create("ConditionFortuneFavorTheBoldMirrorImage")
            .SetGuiPresentation(MirrorImage.Condition.Name, Category.Condition)
            .SetPossessive()
            .CopyParticleReferences(ConditionDefinitions.ConditionBlurred)
            .AddCustomSubFeatures(MirrorImage.DuplicateProvider.Mark)
            .AddToDB();

        var conditionPsychicDamage = ConditionDefinitionBuilder
            .Create("ConditionFortuneFavorTheBoldPsychicDamage")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBrandingSmite)
            .SetPossessive()
            .SetFeatures(
                FeatureDefinitionAdditionalDamageBuilder
                    .Create("AdditionalDamageFortuneFavorTheBoldPsychicDamage")
                    .SetGuiPresentationNoContent(true)
                    .SetNotificationTag("FortuneFavorTheBold")
                    .SetDamageDice(DieType.D8, 4)
                    .SetSpecificDamageType(DamageTypePsychic)
                    .AddToDB())
            .SetSpecialInterruptions(ConditionInterruption.AttacksAndDamages)
            .AddToDB();

        var conditionTemporaryHitPoints = ConditionDefinitionBuilder
            .Create("ConditionFortuneFavorTheBoldTempHitPoints")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(new OnConditionAddedOrRemovedFortuneFavorTheBoldTempHitPoints())
            .AddToDB();

        var powerFortuneFavorTheBold = FeatureDefinitionPowerBuilder
            .Create("PowerDomainMischiefFortuneFavorTheBold")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                null,
                                ConditionForm.ConditionOperation.AddRandom,
                                false, false,
                                conditionDisadvantage,
                                conditionAdvantage,
                                conditionMirrorImage,
                                conditionPsychicDamage,
                                conditionTemporaryHitPoints,
                                null)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        var powerDomainMischiefStrikeOfChaos17 = FeatureDefinitionPowerBuilder
            .Create(PowerDomainMischiefStrikeOfChaos14, "PowerDomainMischiefStrikeOfChaos17")
            .SetOverriddenPower(PowerDomainMischiefStrikeOfChaos14)
            .AddCustomSubFeatures(new MagicEffectFinishedByMeStrikeOfChaos(powerFortuneFavorTheBold))
            .AddToDB();

        powerDomainMischiefStrikeOfChaos17.EffectDescription.EffectForms[0].DamageForm.DiceNumber = 6;

        var powerDomainMischiefStrikeOfChaos20 = FeatureDefinitionPowerBuilder
            .Create(PowerDomainMischiefStrikeOfChaos14, "PowerDomainMischiefStrikeOfChaos20")
            .SetOverriddenPower(powerDomainMischiefStrikeOfChaos17)
            .AddCustomSubFeatures(new MagicEffectFinishedByMeStrikeOfChaos(powerFortuneFavorTheBold))
            .AddToDB();

        powerDomainMischiefStrikeOfChaos20.EffectDescription.EffectForms[0].DamageForm.DiceNumber = 7;

        DomainMischief.FeatureUnlocks.AddRange(
        [
            new FeatureUnlockByLevel(powerFortuneFavorTheBold, 17),
            new FeatureUnlockByLevel(powerDomainMischiefStrikeOfChaos17, 17),
            new FeatureUnlockByLevel(powerDomainMischiefStrikeOfChaos20, 20)
        ]);

        //FIX: strike of chaos powers should not trigger on magical attacks
        foreach (var powerStrikeOfChaos in DatabaseRepository.GetDatabase<FeatureDefinitionPower>()
                     .Where(x => x.Name.StartsWith("PowerDomainMischiefStrikeOfChaos")))
        {
            powerStrikeOfChaos.AddCustomSubFeatures(
                new RestrictReactionAttackMode((_, _, _, mode, _) =>
                    ValidatorsWeapon.IsMelee(mode) || ValidatorsWeapon.IsUnarmed(mode)));
        }

        //
        // Oblivion
        //

        // Keeper of Oblivion

        var featureDomainOblivionKeeperOfOblivion = FeatureDefinitionBuilder
            .Create("FeatureDomainOblivionKeeperOfOblivion")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureDomainOblivionKeeperOfOblivion.AddCustomSubFeatures(
            new OnReducedToZeroHpByMeOrAllyKeeperOfOblivion(featureDomainOblivionKeeperOfOblivion));

        DomainOblivion.FeatureUnlocks.Add(new FeatureUnlockByLevel(featureDomainOblivionKeeperOfOblivion, 17));

        //
        // Sun
        //

        // Rising Dawn

        var featureDomainSunRisingDawn = FeatureDefinitionBuilder
            .Create("FeatureDomainSunRisingDawn")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(new ModifyDamageResistanceRisingDawn())
            .AddToDB();

        var featureSetDomainSunRisingDawn = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetDomainSunRisingDawn")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                featureDomainSunRisingDawn,
                DamageAffinityFireResistance,
                DamageAffinityRadiantResistance)
            .AddToDB();

        DomainSun.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(featureSetDomainSunRisingDawn, 17));

        //
        // Divine Intervention [ALL CLERICS]
        //

        const string TAG = "PowerClericImprovedDivineIntervention";

        var powerClericDivineInterventionImprovementCleric = FeatureDefinitionPowerBuilder
            .Create(PowerClericDivineInterventionCleric, "PowerClericDivineInterventionImprovementCleric")
            .SetOrUpdateGuiPresentation(TAG, Category.Feature)
            .SetHasCastingFailure(false)
            .SetOverriddenPower(PowerClericDivineInterventionCleric)
            .AddToDB();

        var powerClericDivineInterventionImprovementPaladin = FeatureDefinitionPowerBuilder
            .Create(PowerClericDivineInterventionPaladin, "PowerClericDivineInterventionImprovementPaladin")
            .SetOrUpdateGuiPresentation(TAG, Category.Feature)
            .SetHasCastingFailure(false)
            .SetOverriddenPower(PowerClericDivineInterventionPaladin)
            .AddToDB();

        var powerClericDivineInterventionImprovementWizard = FeatureDefinitionPowerBuilder
            .Create(PowerClericDivineInterventionWizard, "PowerClericDivineInterventionImprovementWizard")
            .SetOrUpdateGuiPresentation(TAG, Category.Feature)
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
            .AddCustomSubFeatures(new CharacterTurnStartListenerSurvivor())
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
                    .SetGuiPresentation("ConditionMartialCommanderPeerlessCommanderSavings", Category.Condition,
                        Gui.NoLocalization)
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
            .AddFeatures(
                FeatureDefinitionMovementAffinityBuilder
                    .Create("MovementAffinityMartialCommanderPeerlessCommander")
                    .SetGuiPresentation("ConditionMartialCommanderPeerlessCommanderMovement", Category.Condition,
                        Gui.NoLocalization)
                    .SetBaseSpeedAdditiveModifier(2)
                    .AddToDB())
            .AddToDB();

        var powerMartialCommanderPeerlessCommander = FeatureDefinitionPowerBuilder
            .Create(PowerMartialCommanderInvigoratingShout, "PowerMartialCommanderPeerlessCommander")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerMartialCommanderInvigoratingShout)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
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
            .AddCustomSubFeatures(new CustomLevelUpLogicPositionOfStrength())
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

        var attributeModifierTraditionFreedomFluidStrikes = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierTraditionFreedomFluidStrikes")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.ForceIfBetter, AttributeDefinitions.AttacksNumber, 3)
            .AddToDB();

        TraditionFreedom.FeatureUnlocks.Add(new FeatureUnlockByLevel(attributeModifierTraditionFreedomFluidStrikes,
            17));

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
            .AddCustomSubFeatures(new CustomBehaviorPurityOfLight())
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
            .SetSpecialInterruptions(ConditionInterruption.BattleEnd)
            .CopyParticleReferences(ConditionDefinitions.ConditionBaned)
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
                    .SetSavingThrowData(
                        false, AttributeDefinitions.Constitution, true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Dexterity)
                    .SetParticleEffectParameters(DreadfulOmen)
                    .Build())
            .AddToDB();

        powerTraditionOpenHandQuiveringPalmTrigger.AddCustomSubFeatures(
            new CustomBehaviorQuiveringPalmTrigger(
                powerTraditionOpenHandQuiveringPalmTrigger,
                conditionTraditionOpenHandQuiveringPalm));

        var powerTraditionOpenHandQuiveringPalm = FeatureDefinitionPowerBuilder
            .Create("PowerTraditionOpenHandQuiveringPalm")
            .SetGuiPresentation("FeatureSetTraditionOpenHandQuiveringPalm", Category.Feature)
            .SetUsesFixed(ActivationTime.OnAttackHitMeleeAuto, RechargeRate.KiPoints, 3, 3)
            .SetAutoActivationPowerTag(((int)ExtraActionId.QuiveringPalmToggle).ToString())
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Self, 1, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Day, 1)
                    .SetParticleEffectParameters(Bane)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionTraditionOpenHandQuiveringPalm))
                    .Build())
            .AddCustomSubFeatures(
                ForcePowerUseInSpendPowerAction.Marker,
                new MagicEffectFinishedByMeQuiveringPalm(conditionTraditionOpenHandQuiveringPalm))
            .AddToDB();

        _ = ActionDefinitionBuilder
            .Create(DatabaseHelper.ActionDefinitions.StunningStrikeToggle, "TraditionOpenHandQuiveringPalmToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .SetActionId(ExtraActionId.QuiveringPalmToggle)
            .SetActivatedPower(
                powerTraditionOpenHandQuiveringPalm, ActionDefinitions.ActionParameter.TogglePower, false)
            .RequiresAuthorization()
            .AddToDB();

        _ = DamageDefinitionBuilder
            .Create("DamagePure")
            .SetGuiPresentation(Category.Rules)
            .AddToDB();

        var actionAffinityTraditionOpenHandQuiveringPalm = FeatureDefinitionActionAffinityBuilder
            .Create("ActionAffinityTraditionOpenHandQuiveringPalm")
            .SetGuiPresentation("FeatureSetTraditionOpenHandQuiveringPalm", Category.Feature)
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
                    .SetHealingForm(HealingComputation.Dice, 0, DieType.D1, 0, false, HealingCap.HalfMaximumHitPoints)
                    .SetCreatedBy()
                    .Build(),
                EffectFormBuilder
                    .Create()
                    .SetBonusMode(AddBonusMode.Proficiency)
                    .SetHealingForm(HealingComputation.Dice, 0, DieType.D1, 0, false, HealingCap.MaximumHitPoints)
                    .SetCreatedBy()
                    .Build())
            .AddToDB();

        var powerTraditionSurvivalPhysicalPerfection = FeatureDefinitionPowerBuilder
            .Create(PowerTraditionSurvivalUnbreakableBody, "PowerTraditionSurvivalPhysicalPerfection")
            .SetGuiPresentation(Category.Feature, PowerTraditionSurvivalUnbreakableBody)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerTraditionSurvivalUnbreakableBody)
                    .SetDurationData(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionDodging),
                        EffectFormBuilder.ConditionForm(conditionTraditionSurvivalPhysicalPerfection))
                    .Build())
            .SetOverriddenPower(PowerTraditionSurvivalUnbreakableBody)
            .AddToDB();

        powerTraditionSurvivalPhysicalPerfection.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new OnReducedToZeroHpByEnemyPhysicalPerfection(powerTraditionSurvivalPhysicalPerfection));

        TraditionSurvival.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerTraditionSurvivalPhysicalPerfection, 17));
    }

    private static void PaladinLoad()
    {
        //
        // Oath of Devotion
        //

        // Devotion Aura

        var powerOathOfDevotionAuraDevotion18 = FeatureDefinitionPowerBuilder
            .Create(PowerOathOfDevotionAuraDevotion, "PowerOathOfDevotionAuraDevotion18")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetOverriddenPower(PowerOathOfDevotionAuraDevotion)
            .AddToDB();

        powerOathOfDevotionAuraDevotion18.EffectDescription.targetParameter = 13;

        OathOfDevotion.FeatureUnlocks.Add(new FeatureUnlockByLevel(powerOathOfDevotionAuraDevotion18, 18));

        // Holy Nimbus

        var savingThrowAffinityOathOfDevotionHolyNimbus = FeatureDefinitionBuilder
            .Create("SavingThrowAffinityOathOfDevotionHolyNimbus")
            .SetGuiPresentation("ConditionOathOfDevotionHolyNimbus", Category.Condition)
            .AddToDB();

        savingThrowAffinityOathOfDevotionHolyNimbus.AddCustomSubFeatures(
            new ModifySavingThrowHolyNimbus(savingThrowAffinityOathOfDevotionHolyNimbus));

        var conditionOathOfDevotionHolyNimbus = ConditionDefinitionBuilder
            .Create("ConditionOathOfDevotionHolyNimbus")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionProtectedFromEnergyLightning)
            .SetPossessive()
            .SetFeatures(savingThrowAffinityOathOfDevotionHolyNimbus)
            .AddToDB();

        var lightSourceForm = FaerieFire.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.LightSource);

        var powerOathOfDevotionHolyNimbus = FeatureDefinitionPowerBuilder
            .Create("PowerOathOfDevotionHolyNimbus")
            .SetGuiPresentation(Category.Feature, PowerTraditionLightBlindingFlash)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 13)
                    .SetRecurrentEffect(RecurrentEffect.OnActivation | RecurrentEffect.OnTurnStart |
                                        RecurrentEffect.OnEnter)
                    .ExcludeCaster()
                    .SetEffectForms(
                        EffectFormBuilder.DamageForm(DamageTypeRadiant, 0, DieType.D1, 10),
                        EffectFormBuilder.ConditionForm(conditionOathOfDevotionHolyNimbus,
                            ConditionForm.ConditionOperation.Add, true),
                        EffectFormBuilder
                            .Create()
                            .SetLightSourceForm(
                                LightSourceType.Basic, 6, 6,
                                lightSourceForm.lightSourceForm.color,
                                lightSourceForm.lightSourceForm.graphicsPrefabReference, true)
                            .Build())
                    .SetParticleEffectParameters(PowerTraditionLightBlindingFlash)
                    .Build())
            .AddToDB();

        OathOfDevotion.FeatureUnlocks.Add(new FeatureUnlockByLevel(powerOathOfDevotionHolyNimbus, 20));

        //
        // Oath of Judgement
        //

        // Aura of Rightenousness

        var powerOathOfJugementAuraRightenousness18 = FeatureDefinitionPowerBuilder
            .Create(PowerOathOfJugementAuraRightenousness, "PowerOathOfJugementAuraRightenousness18")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetOverriddenPower(PowerOathOfJugementAuraRightenousness)
            .AddToDB();

        powerOathOfJugementAuraRightenousness18.EffectDescription.targetParameter = 13;

        OathOfJugement.FeatureUnlocks.Add(new FeatureUnlockByLevel(powerOathOfJugementAuraRightenousness18, 18));

        // Final Judgement

        var conditionOathOfJugementFinalJudgementCaster = ConditionDefinitionBuilder
            .Create("ConditionOathOfJugementFinalJudgementCaster")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionProtectedFromEnergyLightning)
            .SetPossessive()
            .SetFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create("AttributeModifierOathOfJugementFinalJudgementCaster")
                    .SetGuiPresentationNoContent(true)
                    .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.CriticalThreshold, -1)
                    .AddToDB(),
                AttributeModifierThirdExtraAttack,
                DamageAffinityBludgeoningResistance,
                DamageAffinityPiercingResistance,
                DamageAffinitySlashingResistance)
            .CopyParticleReferences(ConditionDefinitions.ConditionShine)
            .AddToDB();

        var powerOathOfJugementFinalJudgement = FeatureDefinitionPowerBuilder
            .Create("PowerOathOfJugementFinalJudgement")
            .SetGuiPresentation(Category.Feature, PowerTraditionCourtMageSpellShield)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionOathOfJugementFinalJudgementCaster))
                    .SetParticleEffectParameters(PowerTraditionLightBlindingFlash)
                    .Build())
            .AddToDB();

        OathOfJugement.FeatureUnlocks.Add(new FeatureUnlockByLevel(powerOathOfJugementFinalJudgement, 20));

        //
        // Oath of Motherland
        //

        // Volcanic Aura

        var powerOathOfMotherlandVolcanicAura18 = FeatureDefinitionPowerBuilder
            .Create(PowerOathOfMotherlandVolcanicAura, "PowerOathOfMotherlandVolcanicAura18")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetOverriddenPower(PowerOathOfMotherlandVolcanicAura)
            .AddToDB();

        powerOathOfMotherlandVolcanicAura18.EffectDescription.targetParameter = 13;

        OathOfTheMotherland.FeatureUnlocks.Add(new FeatureUnlockByLevel(powerOathOfMotherlandVolcanicAura18, 18));

        // Flames of Motherland

        var additionalDamageOathOfMotherlandFlamesOfMotherland = FeatureDefinitionAdditionalDamageBuilder
            .Create("AdditionalDamageOathOfMotherlandFlamesOfMotherland")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("FlamesOfMotherland")
            .SetDamageDice(DieType.D6, 2)
            .SetSpecificDamageType(DamageTypeFire)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
            .SetImpactParticleReference(FireBolt.EffectDescription.EffectParticleParameters.impactParticleReference)
            .AddToDB();

        var powerOathOfMotherlandFlamesOfMotherlandRetaliate = FeatureDefinitionPowerBuilder
            .Create("PowerOathOfMotherlandRetaliateFlamesOfMotherland")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypePiercing, 0, DieType.D1, 10))
                    .SetParticleEffectParameters(FireBolt)
                    .Build())
            .AddToDB();

        var damageAffinityOathOfMotherlandFlamesOfMotherland = FeatureDefinitionDamageAffinityBuilder
            .Create("DamageAffinityOathOfMotherlandFlamesOfMotherland")
            .SetGuiPresentationNoContent(true)
            .SetDamageAffinityType(DamageAffinityType.None)
            .SetDamageType(DamageTypeFire)
            .SetRetaliate(powerOathOfMotherlandFlamesOfMotherlandRetaliate, 1, true)
            .AddToDB();

        var conditionOathOfMotherlandFlamesOfMotherland = ConditionDefinitionBuilder
            .Create("ConditionOathOfMotherlandFlamesOfMotherland")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDivineFavor)
            .SetPossessive()
            .SetFeatures(
                additionalDamageOathOfMotherlandFlamesOfMotherland, damageAffinityOathOfMotherlandFlamesOfMotherland)
            .CopyParticleReferences(FireShieldWarm)
            .AddToDB();

        var powerOathOfMotherlandFlamesOfMotherland = FeatureDefinitionPowerBuilder
            .Create("PowerOathOfMotherlandFlamesOfMotherland")
            .SetGuiPresentation(Category.Feature, PowerDomainElementalFireBurst)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 13)
                    .ExcludeCaster()
                    .SetEffectForms(
                        EffectFormBuilder.DamageForm(DamageTypeFire, 8, DieType.D6),
                        EffectFormBuilder.ConditionForm(conditionOathOfMotherlandFlamesOfMotherland,
                            ConditionForm.ConditionOperation.Add, true, true))
                    .SetParticleEffectParameters(Fireball)
                    .Build())
            .AddToDB();

        OathOfTheMotherland.FeatureUnlocks.Add(new FeatureUnlockByLevel(powerOathOfMotherlandFlamesOfMotherland, 20));

        //
        // Oath of Tirmar
        //

        var powerOathOfTirmarAuraTruth18 = FeatureDefinitionPowerBuilder
            .Create(PowerOathOfTirmarAuraTruth, "PowerOathOfTirmarAuraTruth18")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetOverriddenPower(PowerOathOfTirmarAuraTruth)
            .AddToDB();

        powerOathOfTirmarAuraTruth18.EffectDescription.targetParameter = 13;

        OathOfTirmar.FeatureUnlocks.Add(new FeatureUnlockByLevel(powerOathOfTirmarAuraTruth18, 18));

        // Inquisitor's Zeal

        var savingThrowAffinityOathOfTirmarInquisitorZeal = FeatureDefinitionSavingThrowAffinityBuilder
            .Create("SavingThrowAffinityOathOfTirmarInquisitorZeal")
            .SetGuiPresentation("ConditionOathOfTirmarInquisitorZeal", Category.Condition, Gui.NoLocalization)
            .SetAffinities(CharacterSavingThrowAffinity.Advantage, false, AttributeDefinitions.Wisdom)
            .AddToDB();

        var conditionOathOfTirmarInquisitorZeal = ConditionDefinitionBuilder
            .Create("ConditionOathOfTirmarInquisitorZeal")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionTruesight)
            .SetPossessive()
            .SetFeatures(savingThrowAffinityOathOfTirmarInquisitorZeal)
            .AddToDB();

        var featureOathOfTirmarInquisitorZealAdvantage = FeatureDefinitionBuilder
            .Create("FeatureOathOfTirmarInquisitorZealAdvantage")
            .SetGuiPresentation("ConditionOathOfTirmarInquisitorZeal", Category.Condition)
            .AddToDB();

        featureOathOfTirmarInquisitorZealAdvantage.AddCustomSubFeatures(
            new ModifyAttackActionModifierInquisitorZeal(featureOathOfTirmarInquisitorZealAdvantage));

        var conditionOathOfTirmarInquisitorSelfZeal = ConditionDefinitionBuilder
            .Create("ConditionOathOfTirmarInquisitorSelfZeal")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionTruesight)
            .SetPossessive()
            .SetFeatures(
                savingThrowAffinityOathOfTirmarInquisitorZeal,
                FeatureDefinitionSenses.SenseTruesight24,
                featureOathOfTirmarInquisitorZealAdvantage)
            .AddToDB();

        var powerOathOfJugementInquisitorZeal = FeatureDefinitionPowerBuilder
            .Create("PowerOathOfTirmarInquisitorZeal")
            .SetGuiPresentation(Category.Feature, PowerPactChainQuasit)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 7)
                    .ExcludeCaster()
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionOathOfTirmarInquisitorZeal),
                        EffectFormBuilder.ConditionForm(conditionOathOfTirmarInquisitorSelfZeal,
                            ConditionForm.ConditionOperation.Add, true))
                    .SetParticleEffectParameters(TrueSeeing)
                    .Build())
            .AddToDB();

        OathOfTirmar.FeatureUnlocks.Add(new FeatureUnlockByLevel(powerOathOfJugementInquisitorZeal, 20));
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
            .SetGuiPresentation("ConditionRoguishDarkweaverDarkAssault", Category.Condition, Gui.NoLocalization)
            .SetBaseSpeedAdditiveModifier(3)
            .AddToDB();

        var conditionRoguishDarkweaverDarkAssault = ConditionDefinitionBuilder
            .Create("ConditionRoguishDarkweaverDarkAssault")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionStealthy)
            .SetPossessive()
            .AddFeatures(additionalActionRoguishDarkweaverDarkAssault, movementAffinityRoguishDarkweaverDarkAssault)
            .AddToDB();

        var featureRoguishDarkweaverDarkAssault = FeatureDefinitionBuilder
            .Create("FeatureRoguishDarkweaverDarkAssault")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new CustomBehaviorDarkAssault(conditionRoguishDarkweaverDarkAssault))
            .AddToDB();

        RoguishDarkweaver.FeatureUnlocks.Add(new FeatureUnlockByLevel(featureRoguishDarkweaverDarkAssault, 17));

        //
        // Hoodlum
        //

        PowerRoguishHoodlumDirtyFighting.AddCustomSubFeatures(new AttackAfterMagicEffectBrutalAssault());

        var featureRoguishHoodlumBrutalAssault = FeatureDefinitionBuilder
            .Create("FeatureRoguishHoodlumBrutalAssault")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(
                new CustomAdditionalDamageBrutalAssault(
                    FeatureDefinitionAdditionalDamageBuilder
                        .Create("AdditionalDamageRoguishHoodlumBrutalAssault")
                        .SetGuiPresentationNoContent(true)
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
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetParticleEffectParameters(Malediction)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionRoguishShadowcasterShadowForm,
                                ConditionForm.ConditionOperation.Add)
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
            .AddCustomSubFeatures(new CustomBehaviorThiefReflexes())
            .AddToDB();

        RoguishThief.FeatureUnlocks.Add(new FeatureUnlockByLevel(featureRoguishThiefThiefReflexes, 17));
    }

    private static void SorcererLoad()
    {
        //
        // Child of The Rift
        //

        var magicAffinityChildRiftMagicMastery = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinitySorcererChildRiftMagicMastery")
            .SetGuiPresentation(Category.Feature)
            .SetPreserveSlotRolls(18, 9)
            .AddToDB();

        SorcerousChildRift.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(magicAffinityChildRiftMagicMastery, 18));

        //
        // Draconic Bloodline
        //

        var powerSorcererDraconicBloodlineAweOrFearPresence = FeatureDefinitionPowerBuilder
            .Create("PowerSorcererDraconicBloodlineAweOrFearPresence")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.SorceryPoints, 5, 0)
            .SetEffectDescription( // for display purposes only
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 13)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .Build())
            .AddToDB();

        var powerSorcererDraconicBloodlineAwePresence = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSorcererDraconicBloodlineAwePresence")
            .SetGuiPresentation(Category.Feature, PowerTraditionLightBlindingFlash)
            .SetSharedPool(ActivationTime.Action, powerSorcererDraconicBloodlineAweOrFearPresence, 5)
            // .SetUsesFixed(ActivationTime.Action, RechargeRate.SorceryPoints, 5, 0)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 13)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(ConditionDefinitions.ConditionCharmed,
                                ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                            .Build())
                    .SetParticleEffectParameters(PowerTraditionLightBlindingFlash)
                    .Build())
            .SetShowCasting(true)
            .AddToDB();

        var powerSorcererDraconicBloodlineFearPresence = FeatureDefinitionPowerSharedPoolBuilder
            .Create("PowerSorcererDraconicBloodlineFearPresence")
            .SetGuiPresentation(Category.Feature, Fear)
            .SetSharedPool(ActivationTime.Action, powerSorcererDraconicBloodlineAweOrFearPresence, 5)
            // .SetUsesFixed(ActivationTime.Action, RechargeRate.SorceryPoints, 5, 0)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 13)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(ConditionDefinitions.ConditionFrightened,
                                ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                            .Build())
                    .SetParticleEffectParameters(PowerDragonFrightfulPresence)
                    .SetImpactEffectParameters(new AssetReference())
                    .Build())
            .SetShowCasting(true)
            .AddToDB();

        var featureSetSorcererDraconicBloodlinePresence = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetSorcererDraconicBloodlinePresence")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                powerSorcererDraconicBloodlineAweOrFearPresence,
                powerSorcererDraconicBloodlineAwePresence,
                powerSorcererDraconicBloodlineFearPresence)
            .AddToDB();

        SorcerousDraconicBloodline.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(featureSetSorcererDraconicBloodlinePresence, 18));

        //
        // Haunted Soul
        //

        var conditionMindDominatedByHauntedSoul = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionMindDominatedByCaster, "ConditionMindDominatedByHauntedSoul")
            .SetSpecialInterruptions(Array.Empty<ConditionInterruption>())
            .AddToDB();

        conditionMindDominatedByHauntedSoul.AddCustomSubFeatures(
            new OnConditionAddedOrRemovedPossession(conditionMindDominatedByHauntedSoul));

        var powerSorcererHauntedSoulPossession = FeatureDefinitionPowerBuilder
            .Create("PowerSorcererHauntedSoulPossession")
            .SetGuiPresentation(Category.Feature, PowerSorcererHauntedSoulVengefulSpirits)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 24, TargetType.Cube, 4)
                    .SetSavingThrowData(false, AttributeDefinitions.Charisma, true,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeNecrotic, 6, DieType.D10)
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionMindDominatedByHauntedSoul, ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .Build())
                    .SetParticleEffectParameters(PowerSorcererHauntedSoulSpiritVisage)
                    .SetImpactEffectParameters(RayOfEnfeeblement)
                    .Build())
            .AddToDB();

        SorcerousHauntedSoul.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(powerSorcererHauntedSoulPossession, 18));

        //
        // Mana Painter
        //

        var powerSorcererManaPainterMasterDrain = FeatureDefinitionPowerBuilder
            .Create(PowerSorcererManaPainterDrain, "PowerSorcererManaPainterManaOverflow")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetOverriddenPower(PowerSorcererManaPainterDrain)
            .AddToDB();

        powerSorcererManaPainterMasterDrain.EffectDescription.rangeParameter = 1;
        powerSorcererManaPainterMasterDrain.EffectDescription.EffectForms[0].DamageForm.diceNumber = 4;
        powerSorcererManaPainterMasterDrain.EffectDescription.EffectForms[1].SpellSlotsForm.sorceryPointsGain = 2;

        var featureSetSorcererManaPainterManaOverflow = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetSorcererManaPainterManaOverflow")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerSorcererManaPainterMasterDrain)
            .AddToDB();

        powerSorcererManaPainterMasterDrain.AddCustomSubFeatures(
            new RollSavingThrowFinishedManaOverflow(featureSetSorcererManaPainterManaOverflow));

        SorcerousManaPainter.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(featureSetSorcererManaPainterManaOverflow, 18));
    }

    #region Cleric

    private sealed class CustomBehaviorWordOfLaw : IMagicEffectInitiatedByMe, IMagicEffectFinishedByMe
    {
        private const string ConditionSilenced = "ConditionSilenced";
        private static GameLocationCharacter _attacker;

        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var attacker = action.ActingCharacter;

            if (attacker.RulesetCharacter.GetClassLevel(CharacterClassDefinitions.Cleric) < 17)
            {
                yield break;
            }

            var defender = action.ActionParams.TargetCharacters[0];

            _attacker = null;
            defender.RulesetCharacter.ConcentrationChanged -= ConcentrationChanged;
        }

        public IEnumerator OnMagicEffectInitiatedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var attacker = action.ActingCharacter;

            if (attacker.RulesetCharacter.GetClassLevel(CharacterClassDefinitions.Cleric) < 17)
            {
                yield break;
            }

            var defender = action.ActionParams.TargetCharacters[0];

            _attacker = attacker;
            defender.RulesetCharacter.ConcentrationChanged += ConcentrationChanged;
        }

        private static void ConcentrationChanged(RulesetCharacter character)
        {
            var rulesetAttacker = _attacker.RulesetCharacter;

            character.InflictCondition(
                ConditionSilenced,
                DurationType.Round,
                1,
                TurnOccurenceType.EndOfSourceTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.Guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                ConditionSilenced,
                0,
                0,
                0);
        }
    }

    private sealed class CustomBehaviorFinalWord :
        IAttackBeforeHitConfirmedOnEnemy, IPhysicalAttackFinishedByMe,
        IMagicEffectBeforeHitConfirmedOnEnemy, IMagicEffectFinishedByMeAny
    {
        private const string ConditionSilenced = "ConditionSilenced";
        private static GameLocationCharacter _attacker;

        public IEnumerator OnAttackBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool firstTarget,
            bool criticalHit)
        {
            if (attackMode == null)
            {
                yield break;
            }

            _attacker = attacker;
            defender.RulesetCharacter.ConcentrationChanged += ConcentrationChanged;
        }

        public IEnumerator OnMagicEffectBeforeHitConfirmedOnEnemy(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            _attacker = attacker;
            defender.RulesetCharacter.ConcentrationChanged += ConcentrationChanged;

            yield break;
        }

        public IEnumerator OnMagicEffectFinishedByMeAny(
            CharacterActionMagicEffect action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender)
        {
            _attacker = null;
            defender.RulesetCharacter.ConcentrationChanged -= ConcentrationChanged;

            yield break;
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
            _attacker = null;
            defender.RulesetCharacter.ConcentrationChanged -= ConcentrationChanged;

            yield break;
        }

        private static void ConcentrationChanged(RulesetCharacter character)
        {
            var rulesetAttacker = _attacker.RulesetCharacter;

            character.InflictCondition(
                ConditionSilenced,
                DurationType.Round,
                1,
                TurnOccurenceType.EndOfSourceTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.Guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                ConditionSilenced,
                0,
                0,
                0);
        }
    }

    private sealed class OnConditionAddedOrRemovedFortuneFavorTheBoldTempHitPoints : IOnConditionAddedOrRemoved
    {
        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            var clericLevel = target.GetClassLevel(CharacterClassDefinitions.Cleric);

            target.ReceiveTemporaryHitPoints(clericLevel, DurationType.Minute, 1, TurnOccurenceType.EndOfTurn,
                rulesetCondition.SourceGuid);
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }
    }

    private sealed class MagicEffectFinishedByMeStrikeOfChaos(FeatureDefinitionPower powerFortuneFavorTheBold)
        : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var actingCharacter = action.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;

            var implementationManagerService =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerFortuneFavorTheBold, rulesetCharacter);
            var actionParams = new CharacterActionParams(actingCharacter, ActionDefinitions.Id.PowerNoCost)
            {
                ActionModifiers = { new ActionModifier() },
                RulesetEffect = implementationManagerService
                    //CHECK: no need for AddAsActivePowerToSource
                    .MyInstantiateEffectPower(rulesetCharacter, usablePower, false),
                UsablePower = usablePower,
                TargetCharacters = { actingCharacter }
            };

            ServiceRepository.GetService<ICommandService>()
                ?.ExecuteAction(actionParams, null, true);

            yield break;
        }
    }

    private sealed class ModifyDamageResistanceRisingDawn : IModifyDamageAffinity
    {
        public void ModifyDamageAffinity(RulesetActor attacker, RulesetActor defender, List<FeatureDefinition> features)
        {
            features.RemoveAll(x =>
                x is IDamageAffinityProvider
                {
                    DamageAffinityType: DamageAffinityType.Resistance,
                    DamageType: DamageTypeFire or DamageTypeRadiant
                });
        }
    }

    private sealed class ModifyDiceRollSupremeHealing : IModifyDiceRoll
    {
        private static DieType _dieType;

        public void BeforeRoll(
            RollContext rollContext,
            RulesetCharacter rulesetCharacter,
            ref DieType dieType,
            ref AdvantageType advantageType)
        {
            _dieType = dieType;
        }

        public void AfterRoll(RollContext rollContext, RulesetCharacter rulesetCharacter, ref int result)
        {
            if (rollContext == RollContext.HealValueRoll)
            {
                result = DiceMaxValue[(int)_dieType];
            }
        }
    }

    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    private sealed class OnReducedToZeroHpByMeOrAllyKeeperOfOblivion(FeatureDefinition featureKeeperOfOblivion)
        : IOnReducedToZeroHpByMeOrAlly
    {
        public IEnumerator HandleReducedToZeroHpByMeOrAlly(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            GameLocationCharacter ally,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            if (!ally.OncePerTurnIsValid(featureKeeperOfOblivion.Name))
            {
                yield break;
            }

            ally.UsedSpecialFeatures.TryAdd(featureKeeperOfOblivion.Name, 1);

            var rulesetAlly = ally.RulesetCharacter;
            var clericLevel = rulesetAlly.GetClassLevel(CharacterClassDefinitions.Cleric);
            var healingPool = clericLevel;

            // haven't died within 30 ft of Cleric
            if (!downedCreature.IsWithinRange(ally, 6))
            {
                yield break;
            }

            var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var contenders =
                (Gui.Battle?.AllContenders ??
                 locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters))
                .ToList();

            if (contenders.Count != 0)
            {
                rulesetAlly.LogCharacterUsedFeature(featureKeeperOfOblivion);
            }

            foreach (var unit in contenders
                         .Where(x =>
                             x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                             x.Side == ally.Side &&
                             x.IsWithinRange(ally, 6))
                         .OrderByDescending(x => x.RulesetCharacter.MissingHitPoints)
                         .ToList())
            {
                var rulesetUnit = unit.RulesetCharacter;

                if (rulesetUnit.MissingHitPoints >= healingPool)
                {
                    EffectHelpers.StartVisualEffect(ally, unit, CureWounds, EffectHelpers.EffectType.Caster);
                    rulesetUnit.ReceiveHealing(healingPool, true, ally.Guid);
                    healingPool = 0;
                }
                else if (rulesetUnit.MissingHitPoints > 0)
                {
                    healingPool -= rulesetUnit.MissingHitPoints;
                    EffectHelpers.StartVisualEffect(ally, unit, CureWounds, EffectHelpers.EffectType.Caster);
                    rulesetUnit.ReceiveHealing(rulesetUnit.MissingHitPoints, true, ally.Guid);
                }

                if (healingPool <= 0)
                {
                    break;
                }
            }
        }
    }

    #endregion

    #region Paladin

    //
    // Holy Nimbus
    //

    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    private sealed class ModifySavingThrowHolyNimbus(FeatureDefinition featureDefinition) : IRollSavingThrowInitiated
    {
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
            if (abilityScoreName == AttributeDefinitions.Wisdom
                && caster is RulesetCharacterMonster { CharacterFamily: "Fiend" or "Undead" })
            {
                advantageTrends.Add(
                    new TrendInfo(1, FeatureSourceType.CharacterFeature, featureDefinition.Name, featureDefinition));
            }
        }
    }

    //
    // Inquisitor's Zeal
    //

    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    private sealed class ModifyAttackActionModifierInquisitorZeal(FeatureDefinition featureDefinition)
        : IModifyAttackActionModifier
    {
        public void OnAttackComputeModifier(
            RulesetCharacter myself,
            RulesetCharacter defender,
            BattleDefinitions.AttackProximity attackProximity,
            RulesetAttackMode attackMode,
            string effectName,
            ref ActionModifier attackModifier)
        {
            // only weapon attacks
            if (attackMode == null)
            {
                return;
            }

            // only enemies with darkvision
            if (defender.GetFeaturesByType<FeatureDefinitionSense>().All(x => x.senseType != SenseMode.Type.Darkvision))
            {
                return;
            }

            attackModifier.attackAdvantageTrends.Add(
                new TrendInfo(1, FeatureSourceType.CharacterFeature, featureDefinition.Name, featureDefinition));
        }
    }

    #endregion

    #region Sorcerer

    //
    // Possession
    //

    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    private sealed class OnConditionAddedOrRemovedPossession(ConditionDefinition conditionPossession)
        : IOnConditionAddedOrRemoved
    {
        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // Empty
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            if (rulesetCondition.ConditionDefinition != conditionPossession)
            {
                return;
            }

            var rulesetAttacker = EffectHelpers.GetCharacterByGuid(rulesetCondition.SourceGuid);

            if (rulesetAttacker == null)
            {
                return;
            }

            var conditionExhausted = ConditionDefinitions.ConditionExhausted;

            target.InflictCondition(
                conditionExhausted.Name,
                conditionExhausted.DurationType,
                conditionExhausted.DurationParameter,
                conditionExhausted.TurnOccurence,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionExhausted.Name,
                0,
                0,
                0);
        }
    }

    //
    // Mana Overflow
    //

    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    private sealed class RollSavingThrowFinishedManaOverflow(FeatureDefinition featureManaOverflow)
        : IRollSavingThrowFinished
    {
        public void OnSavingThrowFinished(
            RulesetCharacter caster,
            RulesetCharacter defender,
            int saveBonus,
            string abilityScoreName,
            BaseDefinition sourceDefinition,
            List<TrendInfo> modifierTrends,
            List<TrendInfo> advantageTrends,
            int rollModifier,
            int saveDC,
            bool hasHitVisual,
            ref RollOutcome outcome,
            ref int outcomeDelta,
            List<EffectForm> effectForms)
        {
            if (outcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                return;
            }

            var hero = defender.GetOriginalHero();

            if (hero == null)
            {
                return;
            }

            var character = GameLocationCharacter.GetFromActor(hero);

            if (character != null)
            {
                EffectHelpers.StartVisualEffect(character, character, MageArmor, EffectHelpers.EffectType.Caster);
            }

            hero.LogCharacterUsedFeature(featureManaOverflow);
            hero.GainSorceryPoints(1);
        }
    }

    #endregion

    #region Fighter

    //
    // Position of Strength
    //

    private sealed class CustomLevelUpLogicPositionOfStrength : ICustomLevelUpLogic
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

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false })
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

    private sealed class OnReducedToZeroHpByEnemyPhysicalPerfection(FeatureDefinitionPower powerPhysicalPerfection)
        : IOnReducedToZeroHpByEnemy
    {
        public IEnumerator HandleReducedToZeroHpByEnemy(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            var gameLocationActionService =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var gameLocationBattleService =
                ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (gameLocationActionService == null || gameLocationBattleService is not { IsBattleInProgress: true })
            {
                yield break;
            }

            var rulesetCharacter = defender.RulesetCharacter;

            if (rulesetCharacter == null)
            {
                yield break;
            }

            var implementationManagerService =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerPhysicalPerfection, rulesetCharacter);
            var reactionParams = new CharacterActionParams(defender, ActionDefinitions.Id.PowerNoCost)
            {
                StringParameter = "PhysicalPerfection",
                ActionModifiers = { new ActionModifier() },
                RulesetEffect = implementationManagerService
                    .MyInstantiateEffectPower(rulesetCharacter, usablePower, false),
                UsablePower = usablePower,
                TargetCharacters = { defender }
            };

            var count = gameLocationActionService.PendingReactionRequestGroups.Count;

            gameLocationActionService.ReactToUsePower(reactionParams, "UsePower", defender);

            yield return gameLocationBattleService.WaitForReactions(attacker, gameLocationActionService, count);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            rulesetCharacter.ForceKiPointConsumption(1);
            rulesetCharacter.StabilizeAndGainHitPoints(1);

            ServiceRepository.GetService<ICommandService>()
                ?.ExecuteAction(new CharacterActionParams(defender, ActionDefinitions.Id.StandUp), null, true);
        }
    }

    //
    // Purity of Light
    //

    private sealed class CustomBehaviorPurityOfLight : ICustomLevelUpLogic, IPhysicalAttackFinishedByMe
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

        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            if (rollOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure)
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (!rulesetDefender.HasAnyConditionOfTypeOrSubType(
                    ConditionDefinitions.ConditionLuminousKi.Name,
                    ConditionDefinitions.ConditionShine.Name))
            {
                yield break;
            }

            if (!battleManager.IsBattleInProgress)
            {
                yield break;
            }

            attacker.RulesetCharacter.ReceiveHealing(2, true, attacker.Guid);

            foreach (var ally in battleManager.Battle
                         .GetContenders(attacker, isOppositeSide: false, hasToPerceiveTarget: false, withinRange: 4))
            {
                ally.RulesetCharacter.ReceiveHealing(2, true, attacker.Guid);
            }
        }
    }

    //
    // Quivering Palm
    //

    private sealed class CustomBehaviorQuiveringPalmTrigger(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower featureDefinitionPower,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionDefinition)
        : IFilterTargetingCharacter, IMagicEffectFinishedByMe
    {
        public bool EnforceFullSelection => false;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            if (__instance.actionParams.RulesetEffect is not RulesetEffectPower rulesetEffectPower
                || rulesetEffectPower.PowerDefinition != featureDefinitionPower)
            {
                return true;
            }

            if (target.RulesetCharacter == null)
            {
                return true;
            }

            var isValid = target.RulesetCharacter.HasConditionOfType(conditionDefinition.Name);

            if (!isValid)
            {
                __instance.actionModifier.FailureFlags.Add("Tooltip/&MustHaveQuiveringPalmCondition");
            }

            return isValid;
        }

        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition power)
        {
            if (action.ActionParams.TargetCharacters.Count == 0)
            {
                yield break;
            }

            var target = action.ActionParams.TargetCharacters[0];
            var rulesetTarget = target.RulesetCharacter;

            if (!rulesetTarget.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionDefinition.Name, out var activeCondition))
            {
                yield break;
            }

            rulesetTarget.RemoveCondition(activeCondition);

            var attacker = action.ActingCharacter;
            var rulesetAttacker = attacker.RulesetCharacter;

            // takes 10d10 Necrotic
            if (action.SaveOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                var damageForm = new DamageForm
                {
                    DamageType = DamageTypeNecrotic, DieType = DieType.D10, DiceNumber = 10, BonusDamage = 0
                };
                var rolls = new List<int>();
                var damageRoll = rulesetAttacker.RollDamage(damageForm, 0, false, 0, 0, 1, false, false, false, rolls);

                RulesetActor.InflictDamage(
                    damageRoll,
                    damageForm,
                    damageForm.DamageType,
                    new RulesetImplementationDefinitions.ApplyFormsParams { targetCharacter = rulesetTarget },
                    rulesetTarget,
                    false,
                    rulesetAttacker.Guid,
                    false,
                    [],
                    new RollInfo(damageForm.DieType, rolls, 0),
                    false,
                    out _);

                yield break;
            }

            // reduces to 1 hit point
            var totalDamage = rulesetTarget.CurrentHitPoints + rulesetTarget.TemporaryHitPoints - 1;
            var condition = ConditionDefinitions.ConditionStunned_MonkStunningStrike;

            rulesetTarget.SustainDamage(totalDamage, "DamagePure", false, action.ActingCharacter.Guid, null, out _);
            rulesetTarget.InflictCondition(
                condition.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                condition.Name,
                0,
                0,
                0);
        }
    }

    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    private sealed class MagicEffectFinishedByMeQuiveringPalm(ConditionDefinition conditionDefinition)
        : IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition power)
        {
            if (Gui.Battle == null)
            {
                yield break;
            }

            var gameLocationDefender = action.actionParams.targetCharacters[0];

            // remove this condition from all other enemies
            foreach (var rulesetDefender in Gui.Battle.GetContenders(gameLocationDefender, isOppositeSide: false)
                         .Select(defender => defender.RulesetCharacter))
            {
                if (rulesetDefender.TryGetConditionOfCategoryAndType(
                        AttributeDefinitions.TagEffect,
                        conditionDefinition.Name,
                        out var activeCondition))
                {
                    rulesetDefender.RemoveCondition(activeCondition);
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

            var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();
            var attackModifier = new ActionModifier();
            var evalParams = new BattleDefinitions.AttackEvaluationParams();

            evalParams.FillForPhysicalReachAttack(caster, caster.LocationPosition, attackMode, target,
                target.LocationPosition, attackModifier);

            return gameLocationBattleService.CanAttack(evalParams);
        }

        [CanBeNull]
        private static IEnumerable<CharacterActionParams> DefaultAttackHandler(
            [CanBeNull] CharacterActionMagicEffect effect)
        {
            var actionParams = effect?.ActionParams;

            if (actionParams == null)
            {
                return null;
            }

            var caster = actionParams.ActingCharacter;
            var targets = actionParams.TargetCharacters
                .Where(x => x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                            x.RulesetCharacter.HasAnyConditionOfTypeOrSubType("ConditionHitByDirtyFighting"))
                .ToList(); // avoid changing enumerator

            if (caster == null || targets.Count == 0)
            {
                return null;
            }

            var attackMode = caster.FindActionAttackMode(ActionDefinitions.Id.AttackMain);

            if (attackMode == null)
            {
                return null;
            }

            //get copy to be sure we don't break existing mode
            var rulesetAttackModeCopy = RulesetAttackMode.AttackModesPool.Get();

            rulesetAttackModeCopy.Copy(attackMode);

            attackMode = rulesetAttackModeCopy;

            //set action type to be same as the one used for the magic effect
            attackMode.ActionType = effect.ActionType;

            var attackModifier = new ActionModifier();

            return targets
                .Where(t => CanMeleeAttack(caster, t))
                .Select(target =>
                    new CharacterActionParams(caster, ActionDefinitions.Id.AttackFree)
                    {
                        AttackMode = attackMode, TargetCharacters = { target }, ActionModifiers = { attackModifier }
                    });
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

    private sealed class CustomAdditionalDamageBrutalAssault(IAdditionalDamageProvider provider)
        : CustomAdditionalDamage(provider)
    {
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

            if (attackMode == null || rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
            {
                return false;
            }

            return rulesetDefender.HasAnyConditionOfTypeOrSubType(
                ConditionBlinded,
                ConditionFrightened,
                ConditionRestrained,
                ConditionGrappled,
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

    private sealed class CustomBehaviorDarkAssault(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionDarkAssault) : ICharacterTurnStartListener
    {
        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            var rulesetCharacter = locationCharacter.RulesetCharacter;

            // ReSharper disable once InvertIf
            if (rulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                ValidatorsCharacter.IsNotInBrightLight(rulesetCharacter) &&
                !rulesetCharacter.HasConditionOfType(conditionDarkAssault.Name))
            {
                EffectHelpers.StartVisualEffect(
                    locationCharacter, locationCharacter, PowerShadowcasterShadowDodge,
                    EffectHelpers.EffectType.Caster);
                rulesetCharacter.InflictCondition(
                    conditionDarkAssault.Name,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.EndOfTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetCharacter.Guid,
                    rulesetCharacter.CurrentFaction.Name,
                    1,
                    conditionDarkAssault.Name,
                    0,
                    0,
                    0);
                rulesetCharacter.RefreshAttackModes();
            }
        }
    }

    //
    // Thief's Reflexes
    //

    private sealed class CustomBehaviorThiefReflexes : IInitiativeEndListener, ICharacterBeforeTurnEndListener
    {
        public void OnCharacterBeforeTurnEnded(GameLocationCharacter locationCharacter)
        {
            var battle = Gui.Battle;

            if (battle == null || battle.CurrentRound > 1)
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
