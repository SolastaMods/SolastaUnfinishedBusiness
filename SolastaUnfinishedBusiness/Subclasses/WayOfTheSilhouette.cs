using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using Resources = SolastaUnfinishedBusiness.Properties.Resources;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class WayOfTheSilhouette : AbstractSubclass
{
    private const string Name = "WayOfSilhouette";

    public WayOfTheSilhouette()
    {
        // LEVEL 03

        // Silhouette Arts

        var powerDarkness = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Darkness")
            .SetGuiPresentation(Darkness.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(Darkness)
                    .SetTargetingData(Side.All, RangeType.Distance, 12, TargetType.Sphere, 3)
                    .SetEffectForms()
                    .Build())
            .AddCustomSubFeatures(new MagicEffectFinishedByMeDarkness())
            .AddToDB();

        var featureSetWayOfSilhouetteSilhouetteArts = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}SilhouetteArts")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(SenseDarkvision, powerDarkness)
            .AddToDB();

        // Strike the Vitals

        var additionalDamageStrikeTheVitals = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}StrikeTheVitals")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag("StrikeTheVitals")
            .SetDamageDice(DieType.D4, 1)
            .SetRequiredProperty(RestrictedContextRequiredProperty.UnarmedOrMonkWeapon)
            .SetTriggerCondition(AdditionalDamageTriggerCondition.AdvantageOrNearbyAlly)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .AddCustomSubFeatures(new ModifyAdditionalDamageFormStrikeTheVitals())
            .AddToDB();

        // LEVEL 06

        var conditionSilhouetteStep = ConditionDefinitionBuilder
            .Create($"Condition{Name}SilhouetteStep")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHeraldOfBattle)
            .SetPossessive()
            .SetSpecialInterruptions(ConditionInterruption.Attacks)
            .SetFeatures(
                FeatureDefinitionCombatAffinityBuilder
                    .Create($"CombatAffinity{Name}SilhouetteStep")
                    .SetGuiPresentation($"Condition{Name}SilhouetteStep", Category.Condition, Gui.NoLocalization)
                    .SetMyAttackAdvantage(AdvantageType.Advantage)
                    .AddToDB())
            .AddToDB();

        var powerWayOfSilhouetteSilhouetteStep = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}SilhouetteStep")
            .SetGuiPresentation(Category.Feature, Sprites.GetSprite(Name, Resources.PowerSilhouetteStep, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.Position)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionSilhouetteStep, ConditionForm.ConditionOperation.Add,
                            true, true),
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.TeleportToDestination)
                            .Build())
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerRoguishDarkweaverShadowy)
                    .Build())
            .AddCustomSubFeatures(
                new ValidatorsValidatePowerUse(ValidatorsCharacter.IsNotInBrightLight),
                new FilterTargetingPositionSilhouetteStep())
            .AddToDB();

        // LEVEL 11

        // Shadow Flurry

        var featureShadowFlurry = FeatureDefinitionBuilder
            .Create($"Feature{Name}ShadowFlurry")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureShadowFlurry.AddCustomSubFeatures(new PhysicalAttackFinishedByMeShadowFlurry(featureShadowFlurry));

        // LEVEL 17

        // Shadowy Sanctuary

        var powerWayOfSilhouetteShadowySanctuary = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerPatronTimekeeperTimeShift, $"Power{Name}ShadowySanctuary")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints, 3)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(FeatureDefinitionPowers.PowerPatronTimekeeperTimeShift)
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerGlabrezuGeneralShadowEscape_at_will)
                    .Build())
            .SetShowCasting(true)
            .AddToDB();

        powerWayOfSilhouetteShadowySanctuary.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new ValidatorsValidatePowerUse(ValidatorsCharacter.IsNotInBrightLight),
            new CustomBehaviorShadowySanctuary(powerWayOfSilhouetteShadowySanctuary));

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.WayOfTheSilhouette, 256))
            .AddFeaturesAtLevel(3, additionalDamageStrikeTheVitals, featureSetWayOfSilhouetteSilhouetteArts)
            .AddFeaturesAtLevel(6, powerWayOfSilhouetteSilhouetteStep)
            .AddFeaturesAtLevel(11, featureShadowFlurry)
            .AddFeaturesAtLevel(17, powerWayOfSilhouetteShadowySanctuary)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Monk;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceMonkMonasticTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Darkness
    //

    private sealed class MagicEffectFinishedByMeDarkness : IMagicEffectFinishedByMe
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
                .InstantiateEffectSpell(rulesetCharacter, null, Darkness, 2, false);

            var actionParams = action.ActionParams.Clone();

            actionParams.ActionDefinition = actionService.AllActionDefinitions[ActionDefinitions.Id.CastNoCost];
            actionParams.RulesetEffect = effectSpell;

            rulesetCharacter.SpellsCastByMe.TryAdd(effectSpell);
            actionService.ExecuteAction(actionParams, null, true);
        }
    }

    //
    // Strike the Vitals
    //

    private sealed class ModifyAdditionalDamageFormStrikeTheVitals : IModifyAdditionalDamageForm
    {
        public DamageForm AdditionalDamageForm(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            FeatureDefinitionAdditionalDamage featureDefinitionAdditionalDamage,
            DamageForm damageForm)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var dieType = rulesetAttacker.GetMonkDieType();
            var levels = rulesetAttacker.GetClassLevel(CharacterClassDefinitions.Monk);
            var diceNumber = levels switch
            {
                >= 17 => 3,
                >= 11 => 2,
                _ => 1
            };

            damageForm.dieType = dieType;
            damageForm.diceNumber = diceNumber;

            return damageForm;
        }
    }

    //
    // Silhouette Step
    //

    private class FilterTargetingPositionSilhouetteStep : IFilterTargetingPosition
    {
        public IEnumerator ComputeValidPositions(CursorLocationSelectPosition cursorLocationSelectPosition)
        {
            yield return cursorLocationSelectPosition.MyComputeValidPositions(LocationDefinitions.LightingState.Bright);
        }
    }

    //
    // Shadow Flurry
    //

    private sealed class PhysicalAttackFinishedByMeShadowFlurry(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition featureShadowFlurry) : IPhysicalAttackFinishedByMe
    {
        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            if (rollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess ||
                !attacker.OnceInMyTurnIsValid(featureShadowFlurry.Name) ||
                !attacker.RulesetCharacter.IsMonkWeapon(attackMode.SourceDefinition as ItemDefinition))
            {
                yield break;
            }

            attacker.UsedSpecialFeatures.TryAdd(featureShadowFlurry.Name, 1);

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();

            if (actionService == null)
            {
                yield break;
            }

            var actionParams = action.ActionParams.Clone();

            actionParams.ActionDefinition = actionService.AllActionDefinitions[ActionDefinitions.Id.AttackFree];

            attacker.RulesetCharacter.LogCharacterUsedFeature(featureShadowFlurry);
            ServiceRepository.GetService<IGameLocationActionService>()?
                .ExecuteAction(actionParams, null, true);
        }
    }

    //
    // Shadowy Sanctuary
    //

    private class CustomBehaviorShadowySanctuary(FeatureDefinitionPower powerShadowSanctuary)
        : IPhysicalAttackBeforeHitConfirmedOnMe, IMagicEffectBeforeHitConfirmedOnMe,
            IPreventRemoveConcentrationOnPowerUse
    {
        public IEnumerator OnMagicEffectBeforeHitConfirmedOnMe(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            if (rulesetEffect.EffectDescription.RangeType is not (RangeType.MeleeHit or RangeType.RangeHit))
            {
                yield break;
            }

            yield return HandleReaction(battleManager, attacker, defender, actualEffectForms);
        }

        public IEnumerator OnPhysicalAttackBeforeHitConfirmedOnMe(
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
            yield return HandleReaction(battleManager, attacker, defender, actualEffectForms);
        }

        private IEnumerator HandleReaction(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            // ReSharper disable once SuggestBaseTypeForParameter
            List<EffectForm> actualEffectForms)
        {
            var gameLocationActionManager =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (battleManager is not { IsBattleInProgress: true } || gameLocationActionManager == null)
            {
                yield break;
            }

            if (!defender.CanReact())
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender.GetRemainingPowerUses(powerShadowSanctuary) == 0)
            {
                yield break;
            }

            var rulesetEnemy = attacker.RulesetCharacter;

            if (rulesetEnemy is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var implementationManagerService =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerShadowSanctuary, rulesetDefender);
            var actionParams =
                new CharacterActionParams(defender, ActionDefinitions.Id.PowerReaction)
                {
                    StringParameter = "ShadowySanctuary",
                    ActionModifiers = { new ActionModifier() },
                    RulesetEffect = implementationManagerService
                        .MyInstantiateEffectPower(rulesetDefender, usablePower, false),
                    UsablePower = usablePower,
                    TargetCharacters = { defender }
                };

            var count = gameLocationActionManager.PendingReactionRequestGroups.Count;

            gameLocationActionManager.ReactToUsePower(actionParams, "UsePower", defender);

            yield return battleManager.WaitForReactions(attacker, gameLocationActionManager, count);

            if (!actionParams.ReactionValidated)
            {
                yield break;
            }

            actualEffectForms.Clear();
        }
    }
}
