using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Validators;
using static ActionDefinitions;
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
            .AddCustomSubFeatures(new PowerOrSpellFinishedByMeDarkness())
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
            .AddToDB();

        additionalDamageStrikeTheVitals.AddCustomSubFeatures(
            new ModifyAdditionalDamageStrikeTheVitals(additionalDamageStrikeTheVitals));

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
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Round)
                .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.Position)
                .SetEffectForms(
                    EffectFormBuilder.MotionForm(MotionForm.MotionType.TeleportToDestination),
                    EffectFormBuilder.AddConditionForm(conditionSilhouetteStep, true, true)
                )
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

    private sealed class PowerOrSpellFinishedByMeDarkness : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            action.ActingCharacter.MyExecuteActionCastNoCost(Darkness, 0, action.ActionParams);

            yield break;
        }
    }

    //
    // Strike the Vitals
    //

    private sealed class ModifyAdditionalDamageStrikeTheVitals(
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
            attacker.RulesetCharacter.LogCharacterUsedFeature(featureShadowFlurry);

            attacker.MyExecuteActionAttack(
                Id.AttackFree,
                defender,
                attackMode,
                action.ActionParams.ActionModifiers[0]);
        }
    }

    //
    // Shadowy Sanctuary
    //

    private class CustomBehaviorShadowySanctuary(FeatureDefinitionPower powerShadowSanctuary)
        : ITryAlterOutcomeAttack, IPreventRemoveConcentrationOnPowerUse
    {
        public int HandlerPriority => -50;

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
            var usablePower = PowerProvider.Get(powerShadowSanctuary, rulesetHelper);

            if (action.AttackRollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess) ||
                defender != helper ||
                !defender.CanReact() ||
                rulesetHelper.GetRemainingUsesOfPower(usablePower) == 0)
            {
                yield break;
            }

            yield return defender.MyReactToUsePower(
                Id.PowerReaction,
                usablePower,
                [defender],
                attacker,
                "ShadowySanctuary",
                reactionValidated: ReactionValidated,
                battleManager: battleManager);

            yield break;

            void ReactionValidated()
            {
                var delta = -action.AttackSuccessDelta - 1;

                action.AttackRollOutcome = RollOutcome.Failure;
                action.AttackSuccessDelta = -1;
                action.AttackRoll += delta;
            }
        }
    }
}
