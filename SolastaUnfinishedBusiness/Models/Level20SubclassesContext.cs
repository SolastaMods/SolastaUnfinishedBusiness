using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
using static ActionDefinitions;
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
    private const string Tag = "PowerClericImprovedDivineIntervention";

    private static readonly FeatureDefinitionPower PowerClericDivineInterventionImprovementCleric =
        FeatureDefinitionPowerBuilder
            .Create(PowerClericDivineInterventionCleric, "PowerClericDivineInterventionImprovementCleric")
            .SetOrUpdateGuiPresentation(Tag, Category.Feature)
            .SetHasCastingFailure(false)
            .SetOverriddenPower(PowerClericDivineInterventionCleric)
            .AddToDB();

    internal static readonly FeatureDefinitionPower PowerClericDivineInterventionImprovementPaladin =
        FeatureDefinitionPowerBuilder
            .Create(PowerClericDivineInterventionPaladin, "PowerClericDivineInterventionImprovementPaladin")
            .SetOrUpdateGuiPresentation(Tag, Category.Feature)
            .SetHasCastingFailure(false)
            .SetOverriddenPower(PowerClericDivineInterventionPaladin)
            .AddToDB();

    internal static readonly FeatureDefinitionPower PowerClericDivineInterventionImprovementWizard =
        FeatureDefinitionPowerBuilder
            .Create(PowerClericDivineInterventionWizard, "PowerClericDivineInterventionImprovementWizard")
            .SetOrUpdateGuiPresentation(Tag, Category.Feature)
            .SetHasCastingFailure(false)
            .SetOverriddenPower(PowerClericDivineInterventionWizard)
            .AddToDB();

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
                DamageAffinityBludgeoningResistanceTrue,
                DamageAffinityPiercingResistanceTrue,
                DamageAffinitySlashingResistanceTrue,
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
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
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
            .AddCustomSubFeatures(new PowerOrSpellFinishedByMeStrikeOfChaos(powerFortuneFavorTheBold))
            .AddToDB();

        powerDomainMischiefStrikeOfChaos17.EffectDescription.EffectForms[0].DamageForm.DiceNumber = 6;

        var powerDomainMischiefStrikeOfChaos20 = FeatureDefinitionPowerBuilder
            .Create(PowerDomainMischiefStrikeOfChaos14, "PowerDomainMischiefStrikeOfChaos20")
            .SetOverriddenPower(powerDomainMischiefStrikeOfChaos17)
            .AddCustomSubFeatures(new PowerOrSpellFinishedByMeStrikeOfChaos(powerFortuneFavorTheBold))
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
                new RestrictReactionAttackMode((_, _, _, mode, _) => mode != null));
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

        DomainBattle.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementPaladin, 20));
        DomainElementalCold.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementWizard, 20));
        DomainElementalFire.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementWizard, 20));
        DomainElementalLighting.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementWizard, 20));
        DomainInsight.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementCleric, 20));
        DomainLaw.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementPaladin, 20));
        DomainLife.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementCleric, 20));
        DomainMischief.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementWizard, 20));
        DomainOblivion.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementCleric, 20));
        DomainSun.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(PowerClericDivineInterventionImprovementWizard, 20));
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
            .SetFeatures(FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityAdvantageToAll)
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

        var powerTraditionOpenHandQuiveringPalmDamage = FeatureDefinitionPowerBuilder
            .Create("PowerTraditionOpenHandQuiveringPalmDamage")
            .SetGuiPresentation("FeatureSetTraditionOpenHandQuiveringPalm", Category.Feature, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.DamageForm(DamageTypeNecrotic, 10, DieType.D10))
                    .SetImpactEffectParameters(PowerPatronTreeExplosiveGrowth)
                    .Build())
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
            .AddCustomSubFeatures(new CustomBehaviorQuiveringPalmTrigger(
                powerTraditionOpenHandQuiveringPalmDamage, conditionTraditionOpenHandQuiveringPalm))
            .AddToDB();

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
                new PowerOrSpellFinishedByMeQuiveringPalm(conditionTraditionOpenHandQuiveringPalm))
            .AddToDB();

        _ = ActionDefinitionBuilder
            .Create(DatabaseHelper.ActionDefinitions.StunningStrikeToggle, "TraditionOpenHandQuiveringPalmToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.QuiveringPalmToggle)
            .SetActivatedPower(powerTraditionOpenHandQuiveringPalm, false)
            .OverrideClassName("Toggle")
            .SetParameter(ActionParameter.TogglePower)
            .AddToDB();

        _ = DamageDefinitionBuilder
            .Create("DamagePure")
            .SetGuiPresentation(Category.Rules)
            .AddToDB();

        var actionAffinityTraditionOpenHandQuiveringPalm = FeatureDefinitionActionAffinityBuilder
            .Create("ActionAffinityTraditionOpenHandQuiveringPalm")
            .SetGuiPresentation("FeatureSetTraditionOpenHandQuiveringPalm", Category.Feature)
            .SetAuthorizedActions((Id)ExtraActionId.QuiveringPalmToggle)
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
            .SetShowCasting(false)
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

        var lightSourceForm = Light.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.LightSource);

        var powerOathOfDevotionHolyNimbus = FeatureDefinitionPowerBuilder
            .Create("PowerOathOfDevotionHolyNimbus")
            .SetGuiPresentation(Category.Feature, PowerTraditionLightBlindingFlash)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(Light)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetRecurrentEffect(
                        RecurrentEffect.OnActivation | RecurrentEffect.OnTurnStart | RecurrentEffect.OnEnter)
                    .ExcludeCaster()
                    .SetEffectForms(
                        EffectFormBuilder.DamageForm(DamageTypeRadiant, 0, DieType.D1, 10),
                        EffectFormBuilder.ConditionForm(
                            conditionOathOfDevotionHolyNimbus,
                            ConditionForm.ConditionOperation.Add, true, true),
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
                DamageAffinityBludgeoningResistanceTrue,
                DamageAffinityPiercingResistanceTrue,
                DamageAffinitySlashingResistanceTrue)
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
            .SetAttackModeOnly()
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
            .SetRetaliate(powerOathOfMotherlandFlamesOfMotherlandRetaliate, 1)
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
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
                    .ExcludeCaster()
                    .SetEffectForms(
                        EffectFormBuilder.DamageForm(DamageTypeFire, 8, DieType.D6),
                        EffectFormBuilder.ConditionForm(
                            conditionOathOfMotherlandFlamesOfMotherland,
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
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 3)
                    .ExcludeCaster()
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionOathOfTirmarInquisitorZeal),
                        EffectFormBuilder.ConditionForm(
                            conditionOathOfTirmarInquisitorSelfZeal,
                            ConditionForm.ConditionOperation.Add, true, true))
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
            .SetActionType(ActionType.Main)
            .SetRestrictedActions(Id.AttackMain)
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

        PowerRoguishHoodlumDirtyFighting.AddCustomSubFeatures(new PowerOrSpellFinishedByMeDirtyFighting());

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
                DamageAffinityBludgeoningResistanceTrue,
                DamageAffinityColdResistance,
                DamageAffinityFireResistance,
                DamageAffinityForceDamageResistance,
                DamageAffinityLightningResistance,
                DamageAffinityNecroticResistance,
                DamageAffinityPiercingResistanceTrue,
                DamageAffinityPoisonResistance,
                DamageAffinityPsychicResistance,
                DamageAffinitySlashingResistanceTrue,
                DamageAffinityThunderResistance)
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
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
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
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
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
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Sphere, 6)
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
            .AddCustomSubFeatures(new OnConditionAddedOrRemovedPossession())
            .SetSpecialInterruptions(Array.Empty<ConditionInterruption>())
            .AddToDB();

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

    private sealed class CustomBehaviorWordOfLaw : IPowerOrSpellInitiatedByMe, IPowerOrSpellFinishedByMe
    {
        private const string ConditionSilenced = "ConditionSilenced";
        private static GameLocationCharacter _attacker;

        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
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

        public IEnumerator OnPowerOrSpellInitiatedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
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
        IPhysicalAttackBeforeHitConfirmedOnEnemy, IPhysicalAttackFinishedByMe,
        IMagicEffectBeforeHitConfirmedOnEnemy, IMagicEffectFinishedByMe
    {
        private const string ConditionSilenced = "ConditionSilenced";
        private static GameLocationCharacter _attacker;

        public IEnumerator OnMagicEffectBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
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

        public IEnumerator OnMagicEffectFinishedByMe(
            CharacterAction action,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            _attacker = null;

            foreach (var defender in targets)
            {
                defender.RulesetCharacter.ConcentrationChanged -= ConcentrationChanged;
            }

            yield break;
        }

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
            if (attackMode == null)
            {
                yield break;
            }

            _attacker = attacker;
            defender.RulesetCharacter.ConcentrationChanged += ConcentrationChanged;
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

            target.ReceiveTemporaryHitPoints(
                clericLevel, DurationType.UntilAnyRest, 1, TurnOccurenceType.EndOfTurn, rulesetCondition.SourceGuid);
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }
    }

    private sealed class PowerOrSpellFinishedByMeStrikeOfChaos(FeatureDefinitionPower powerFortuneFavorTheBold)
        : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var actingCharacter = action.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerFortuneFavorTheBold, rulesetCharacter);

            actingCharacter.MyExecuteActionSpendPower(usablePower, actingCharacter);

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

        public void AfterRoll(
            DieType dieType,
            AdvantageType advantageType,
            RollContext rollContext,
            RulesetCharacter rulesetCharacter,
            ref int firstRoll,
            ref int secondRoll,
            ref int result)
        {
            if (rollContext != RollContext.HealValueRoll)
            {
                return;
            }

            var max = DiceMaxValue[(int)_dieType];

            firstRoll = max;
            secondRoll = max;
            result = max;
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
                .ToArray();

            if (contenders.Length != 0)
            {
                rulesetAlly.LogCharacterUsedFeature(featureKeeperOfOblivion);
            }

            foreach (var unit in contenders
                         .Where(x =>
                             x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                             x.Side == ally.Side &&
                             x.IsWithinRange(ally, 6))
                         .OrderByDescending(x => x.RulesetCharacter.MissingHitPoints)
                         .ToArray())
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
            if (abilityScoreName == AttributeDefinitions.Wisdom &&
                rulesetActorCaster is RulesetCharacterMonster { CharacterFamily: "Fiend" or "Undead" })
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
        private readonly TrendInfo _trendInfo =
            new(1, FeatureSourceType.CharacterFeature, featureDefinition.Name, featureDefinition);

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

            attackModifier.AttackAdvantageTrends.Add(_trendInfo);
        }
    }

    #endregion

    #region Sorcerer

    //
    // Possession
    //

    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    private sealed class OnConditionAddedOrRemovedPossession : IOnConditionAddedOrRemoved
    {
        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // Empty
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            var rulesetAttacker = EffectHelpers.GetCharacterByGuid(rulesetCondition.SourceGuid);
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
            RulesetActor rulesetActorCaster,
            RulesetActor rulesetActorDefender,
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
            if (outcome != RollOutcome.Success ||
                rulesetActorDefender is not RulesetCharacter rulesetCharacter)
            {
                return;
            }

            var hero = rulesetCharacter.GetOriginalHero();

            if (hero == null)
            {
                return;
            }

            var character = GameLocationCharacter.GetFromActor(hero);

            EffectHelpers.StartVisualEffect(character, character, MageArmor, EffectHelpers.EffectType.Caster);
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
            var rulesetCharacter = defender.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerPhysicalPerfection, rulesetCharacter);

            yield return defender.MyReactToSpendPower(
                usablePower,
                attacker,
                "PhysicalPerfection",
                reactionValidated: ReactionValidated);

            yield break;

            void ReactionValidated()
            {
                rulesetCharacter.ForceKiPointConsumption(1);
                defender.MyExecuteActionStabilizeAndStandUp(1);
            }
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

            var rulesetDefender = defender.RulesetActor;

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
        FeatureDefinitionPower powerQuiveringPalmDamage,
        ConditionDefinition conditionDefinition)
        : IFilterTargetingCharacter, IPowerOrSpellFinishedByMe
    {
        public bool EnforceFullSelection => false;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            if (target.RulesetCharacter == null)
            {
                return false;
            }

            var isValid = target.RulesetCharacter.HasConditionOfType(conditionDefinition.Name);

            if (!isValid)
            {
                __instance.actionModifier.FailureFlags.Add("Failure/&MustHaveQuiveringPalmCondition");
            }

            return isValid;
        }

        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition power)
        {
            if (action.ActionParams.TargetCharacters.Count == 0)
            {
                yield break;
            }

            var attacker = action.ActingCharacter;
            var rulesetAttacker = attacker.RulesetCharacter;
            var target = action.ActionParams.TargetCharacters[0];
            var rulesetTarget = target.RulesetCharacter;

            if (!rulesetTarget.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionDefinition.Name, out var activeCondition) ||
                activeCondition.SourceGuid != attacker.Guid)
            {
                yield break;
            }

            rulesetTarget.RemoveCondition(activeCondition);

            // takes 10d10 Necrotic
            if (action.SaveOutcome == RollOutcome.Success)
            {
                var usablePower = PowerProvider.Get(powerQuiveringPalmDamage, rulesetAttacker);

                attacker.MyExecuteActionSpendPower(usablePower, target);

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
    private sealed class PowerOrSpellFinishedByMeQuiveringPalm(ConditionDefinition conditionDefinition)
        : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition power)
        {
            if (Gui.Battle == null)
            {
                yield break;
            }

            var gameLocationDefender = action.actionParams.targetCharacters[0];

            // remove this condition from all other enemies
            foreach (var rulesetDefender in Gui.Battle.GetContenders(gameLocationDefender, isOppositeSide: false)
                         .Select(defender => defender.RulesetActor))
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

    private sealed class PowerOrSpellFinishedByMeDirtyFighting : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            if (Gui.Battle == null)
            {
                yield break;
            }

            var actingCharacter = action.ActingCharacter;
            var targets = action.ActionParams.TargetCharacters
                .Where(x =>
                    x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                    x.RulesetCharacter.HasConditionOfCategoryAndType(
                        AttributeDefinitions.TagEffect, "ConditionHitByDirtyFighting"))
                .ToArray();

            if (targets.Length == 0)
            {
                yield break;
            }

            var attackModeMain = actingCharacter.FindActionAttackMode(Id.AttackMain);

            //get copy to be sure we don't break existing mode
            var attackModeCopy = RulesetAttackMode.AttackModesPool.Get();

            attackModeCopy.Copy(attackModeMain);
            attackModeCopy.ActionType = ActionType.NoCost;

            foreach (var target in targets)
            {
                actingCharacter.MyExecuteActionAttack(
                    Id.AttackFree,
                    target,
                    attackModeCopy,
                    new ActionModifier());
            }
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

            var rulesetDefender = defender.RulesetActor;

            if (attackMode == null || rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
            {
                return false;
            }

            return
                defender.RulesetCharacter?.IsIncapacitated == true ||
                rulesetDefender.HasAnyConditionOfTypeOrSubType(
                    ConditionBlinded,
                    ConditionFrightened,
                    ConditionRestrained,
                    ConditionGrappled,
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

            Gui.Battle.ContenderModified(
                locationCharacter, GameLocationBattle.ContenderModificationMode.Remove, false, false);
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

            Gui.Battle.ContenderModified(
                locationCharacter, GameLocationBattle.ContenderModificationMode.Add, false, false);
        }
    }

    #endregion
}
