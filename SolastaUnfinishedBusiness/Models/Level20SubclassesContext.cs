using System.Collections;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomValidators;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAutoPreparedSpellss;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;

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
        var featureMartialChampionSurvivor = FeatureDefinitionBuilder
            .Create("FeatureMartialChampionSurvivor")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new CharacterTurnStartListenerSurvivor())
            .AddToDB();

        MartialChampion.FeatureUnlocks.Add(new FeatureUnlockByLevel(featureMartialChampionSurvivor, 18));

        var conditionPeerlessCommander = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionRousingShout, "ConditionMartialCommanderPeerlessCommander")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .AddFeatures(
                FeatureDefinitionMovementAffinityBuilder
                    .Create("MovementAffinityMartialCommanderPeerlessCommander")
                    .SetGuiPresentation("ConditionMartialCommanderPeerlessCommander", Category.Condition)
                    .SetBaseSpeedAdditiveModifier(2)
                    .AddToDB(),
                FeatureDefinitionSavingThrowAffinityBuilder
                    .Create("SavingThrowAffinityMartialCommanderPeerlessCommander")
                    .SetGuiPresentation("ConditionMartialCommanderPeerlessCommander", Category.Condition)
                    .SetAffinities(CharacterSavingThrowAffinity.Advantage, true,
                        AttributeDefinitions.Strength,
                        AttributeDefinitions.Dexterity,
                        AttributeDefinitions.Constitution,
                        AttributeDefinitions.Intelligence,
                        AttributeDefinitions.Wisdom,
                        AttributeDefinitions.Charisma)
                    .AddToDB())
            .AddToDB();

        var powerMartialCommanderPeerlessCommander = FeatureDefinitionPowerBuilder
            .Create(PowerMartialCommanderRousingShout, "PowerMartialCommanderPeerlessCommander")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerMartialCommanderRousingShout)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionPeerlessCommander,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .SetOverriddenPower(PowerMartialCommanderRousingShout)
            .AddToDB();

        MartialCommander.FeatureUnlocks.Add(new FeatureUnlockByLevel(powerMartialCommanderPeerlessCommander, 18));

        var attributeModifierMartialMountaineerPositionOfStrength = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierMartialMountaineerPositionOfStrength")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 3)
            .SetSituationalContext(
                ExtraSituationalContext.NextToWallWithShieldAndMaxMediumArmorAndConsciousAllyNextToTarget)
            .SetCustomSubFeatures(new CustomCodePositionOfStrength())
            .AddToDB();

        MartialMountaineer.SetCustomSubFeatures(
            new FeatureUnlockByLevel(attributeModifierMartialMountaineerPositionOfStrength, 18));

        MartialSpellblade.FeatureUnlocks.Add(new FeatureUnlockByLevel(AttackReplaceWithCantripCasterFighting, 18));
    }

    private static void MonkLoad()
    {
        TraditionFreedom.FeatureUnlocks.Add(new FeatureUnlockByLevel(AttributeModifierMonkExtraAttack, 17));

        var powerTraditionLightPurityOfLight = FeatureDefinitionPowerBuilder
            .Create(PowerTraditionLightLuminousKi, "PowerTraditionLightPurityOfLight")
            .SetUsesFixed(ActivationTime.OnAttackHitAuto)
            .SetOverriddenPower(PowerTraditionLightLuminousKi)
            .AddToDB();

        var additionalDamageTraditionLightRadiantStrikesLuminousKiD6 = FeatureDefinitionAdditionalDamageBuilder
            .Create(AdditionalDamageTraditionLightRadiantStrikesLuminousKi,
                "AdditionalDamageTraditionLightRadiantStrikesLuminousKiD6")
            .SetDamageDice(DieType.D6, 1)
            .AddToDB();

        var additionalDamageTraditionLightRadiantStrikesShineD6 = FeatureDefinitionAdditionalDamageBuilder
            .Create(AdditionalDamageTraditionLightRadiantStrikesShine,
                "AdditionalDamageTraditionLightRadiantStrikesShineD6")
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

        var damageAffinityTraditionSurvivalPhysicalPerfection = FeatureDefinitionDamageAffinityBuilder
            .Create(DamageAffinityHalfOrcRelentlessEndurance, "DamageAffinityTraditionSurvivalPhysicalPerfection")
            .SetGuiPresentation("FeatureSetTraditionSurvivalPhysicalPerfection", Category.Feature)
            .AddToDB();

        var conditionTraditionSurvivalPhysicalPerfection = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionTraditionSurvivalUnbreakableBody,
                "ConditionTraditionSurvivalPhysicalPerfection")
            .SetOrUpdateGuiPresentation(Category.Condition)
            .AddToDB();

        var powerTraditionSurvivalPhysicalPerfection = FeatureDefinitionPowerBuilder
            .Create(PowerTraditionSurvivalUnbreakableBody, "PowerTraditionSurvivalPhysicalPerfection")
            .SetOrUpdateGuiPresentation("FeatureSetTraditionSurvivalPhysicalPerfection", Category.Feature)
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
            .SetCustomSubFeatures(new ModifyMagicEffectPhysicalPerfection())
            .AddToDB();

        var featureSetTraditionSurvivalPhysicalPerfection = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetTraditionSurvivalPhysicalPerfection")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(damageAffinityTraditionSurvivalPhysicalPerfection, powerTraditionSurvivalPhysicalPerfection)
            .AddToDB();

        TraditionSurvival.FeatureUnlocks.Add(
            new FeatureUnlockByLevel(featureSetTraditionSurvivalPhysicalPerfection, 17));
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
        var attributeModifierRoguishDarkweaverDarkAssault = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierRoguishDarkweaverDarkAssault")
            .SetGuiPresentationNoContent(true)
            .SetModifier(AttributeModifierOperation.ForceIfBetter, AttributeDefinitions.AttacksNumber, 2)
            .AddToDB();

        var movementAffinityRoguishDarkweaverDarkAssault = FeatureDefinitionMovementAffinityBuilder
            .Create("MovementAffinityRoguishDarkweaverDarkAssault")
            .SetGuiPresentationNoContent(true)
            .SetBaseSpeedAdditiveModifier(3)
            .AddToDB();

        var conditionRoguishDarkweaverDarkAssault = ConditionDefinitionBuilder
            .Create("ConditionRoguishDarkweaverDarkAssault")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionAided)
            .SetPossessive()
            .SetSpecialDuration()
            .AddFeatures(attributeModifierRoguishDarkweaverDarkAssault, movementAffinityRoguishDarkweaverDarkAssault)
            .AddToDB();

        var featureSetRoguishDarkweaverDarkAssault = FeatureDefinitionBuilder
            .Create("FeatureRoguishDarkweaverDarkAssault")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new CharacterTurnEndListenerDarkAssault(conditionRoguishDarkweaverDarkAssault))
            .AddToDB();

        RoguishDarkweaver.FeatureUnlocks.Add(new FeatureUnlockByLevel(featureSetRoguishDarkweaverDarkAssault, 17));

        var conditionRoguishShadowcasterShadowForm = ConditionDefinitionBuilder
            .Create("ConditionRoguishShadowcasterShadowForm")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionShielded)
            .SetPossessive()
            .CopyParticleReferences(ConditionDefinitions.ConditionInvisibleGreater)
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
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Minute, 1)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(conditionRoguishShadowcasterShadowForm, ConditionForm.ConditionOperation.Add)
                        .Build())
                .Build())
            .AddToDB();

        RoguishShadowCaster.FeatureUnlocks.Add(new FeatureUnlockByLevel(powerRoguishShadowcasterShadowForm, 17));

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
                    x == AdditionalDamageTraditionLightRadiantStrikesLuminousKi ||
                    x == AdditionalDamageTraditionLightRadiantStrikesShine);
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

            if (rulesetDefender == null)
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
                                     gameLocationBattleService.IsWithinXCells(attacker, x, 2)))
            {
                ally.RulesetCharacter.ReceiveHealing(2, true, attacker.Guid);
            }
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

            rulesetCharacter.ReceiveHealing(totalHealing, true, rulesetCharacter.Guid);
        }
    }

    //
    // Physical Perfection
    //

    private sealed class ModifyMagicEffectPhysicalPerfection : IModifyMagicEffectRecurrent
    {
        public void ModifyEffect(
            RulesetCondition rulesetCondition,
            EffectForm effectForm,
            RulesetActor rulesetActor)
        {
            if (rulesetActor is not RulesetCharacter rulesetCharacter)
            {
                return;
            }

            if (effectForm.FormType != EffectForm.EffectFormType.Healing)
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

            var index = battle.InitiativeSortedContenders.FindIndex(0, 2, x => x == locationCharacter);

            if (battle.activeContenderIndex == index)
            {
                battle.InitiativeSortedContenders.RemoveAt(index);
            }
        }

        public IEnumerator OnInitiativeEnded(GameLocationCharacter locationCharacter)
        {
            var initiative = locationCharacter.LastInitiative - 10;
            var initiativeSortedContenders = Gui.Battle.InitiativeSortedContenders;
            var positionCharacter = initiativeSortedContenders.First(x => x.LastInitiative < initiative);
            var positionCharacterIndex = initiativeSortedContenders.IndexOf(positionCharacter);

            if (positionCharacterIndex >= 0)
            {
                initiativeSortedContenders.Insert(positionCharacterIndex, locationCharacter);
            }

            yield break;
        }
    }

    //
    // Dark Assault
    //

    private sealed class CharacterTurnEndListenerDarkAssault : ICharacterTurnEndListener
    {
        private readonly ConditionDefinition _conditionDarkAssault;

        public CharacterTurnEndListenerDarkAssault(ConditionDefinition conditionDarkAssault)
        {
            _conditionDarkAssault = conditionDarkAssault;
        }

        public void OnCharacterTurnEnded(GameLocationCharacter locationCharacter)
        {
            var rulesetCharacter = locationCharacter.RulesetCharacter;

            if (rulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                ValidatorsCharacter.IsNotInBrightLight(rulesetCharacter))
            {
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
            }
        }
    }
}
