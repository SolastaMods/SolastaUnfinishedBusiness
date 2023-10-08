using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class WayOfTheSilhouette : AbstractSubclass
{
    private const string Name = "WayOfSilhouette";

    public WayOfTheSilhouette()
    {
        // LEVEL 03

        // Silhouette Arts

        var powerWayOfSilhouetteDarkness = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Darkness")
            .SetGuiPresentation(Darkness.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints, 2, 2)
            .SetEffectDescription(Darkness.EffectDescription)
            .AddToDB();

        var powerWayOfSilhouetteDarkvision = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Darkvision")
            .SetGuiPresentation(Darkvision.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints, 2, 2)
            .SetEffectDescription(Darkvision.EffectDescription)
            .AddToDB();

        var powerWayOfSilhouettePassWithoutTrace = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}PassWithoutTrace")
            .SetGuiPresentation(PassWithoutTrace.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints, 2, 2)
            .SetEffectDescription(PassWithoutTrace.EffectDescription)
            .AddToDB();

        var powerWayOfSilhouetteSilence = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Silence")
            .SetGuiPresentation(Silence.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints, 2, 2)
            .SetEffectDescription(Silence.EffectDescription)
            .AddToDB();

        var featureSetWayOfSilhouetteSilhouetteArts = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}SilhouetteArts")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                powerWayOfSilhouetteDarkness,
                powerWayOfSilhouetteDarkvision,
                powerWayOfSilhouettePassWithoutTrace,
                powerWayOfSilhouetteSilence)
            .AddToDB();

        // Cloak of Silhouettes Weak

        var lightAffinityWayOfSilhouetteCloakOfSilhouettesWeak = FeatureDefinitionLightAffinityBuilder
            .Create($"LightAffinity{Name}CloakOfSilhouettesWeak")
            .SetGuiPresentation(Category.Feature)
            .AddLightingEffectAndCondition(new FeatureDefinitionLightAffinity.LightingEffectAndCondition
            {
                lightingState = LocationDefinitions.LightingState.Unlit,
                condition = CustomConditionsContext.InvisibilityEveryRound
            })
            .AddToDB();

        // LEVEL 06

        // Silhouette Step

        var powerWayOfSilhouetteSilhouetteStep = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}SilhouetteStep")
            .SetGuiPresentation(Category.Feature, MistyStep)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(MistyStep.EffectDescription)
            .SetShowCasting(true)
            .AddToDB();

        // Strike the Vitals

        var additionalDamageStrikeTheVitalsD6 = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}StrikeTheVitalsD6")
            .SetGuiPresentation($"AdditionalDamage{Name}StrikeTheVitals", Category.Feature)
            .SetNotificationTag("StrikeTheVitals")
            .SetDamageDice(DieType.D6, 1)
            .SetRequiredProperty(RestrictedContextRequiredProperty.UnarmedOrMonkWeapon)
            .SetTriggerCondition(AdditionalDamageTriggerCondition.AdvantageOrNearbyAlly)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .AddToDB();

        var additionalDamageStrikeTheVitalsD8 = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}StrikeTheVitalsD8")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("StrikeTheVitals")
            .SetDamageDice(DieType.D8, 2)
            .SetRequiredProperty(RestrictedContextRequiredProperty.UnarmedOrMonkWeapon)
            .SetTriggerCondition(AdditionalDamageTriggerCondition.AdvantageOrNearbyAlly)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .AddCustomSubFeatures(
                new CustomLevelUpLogicAdditionalDamageStrikeTheVitals(additionalDamageStrikeTheVitalsD6))
            .AddToDB();

        var additionalDamageStrikeTheVitalsD10 = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}StrikeTheVitalsD10")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("StrikeTheVitals")
            .SetDamageDice(DieType.D10, 3)
            .SetRequiredProperty(RestrictedContextRequiredProperty.UnarmedOrMonkWeapon)
            .SetTriggerCondition(AdditionalDamageTriggerCondition.AdvantageOrNearbyAlly)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .AddCustomSubFeatures(
                new CustomLevelUpLogicAdditionalDamageStrikeTheVitals(additionalDamageStrikeTheVitalsD8))
            .AddToDB();

        // LEVEL 11

        // Cloak of Silhouettes Strong

        var lightAffinityCloakOfSilhouettesStrong = FeatureDefinitionLightAffinityBuilder
            .Create($"LightAffinity{Name}CloakOfSilhouettesStrong")
            .SetGuiPresentation(Category.Feature)
            .AddLightingEffectAndCondition(new FeatureDefinitionLightAffinity.LightingEffectAndCondition
            {
                lightingState = LocationDefinitions.LightingState.Dim,
                condition = CustomConditionsContext.InvisibilityEveryRound
            })
            .AddLightingEffectAndCondition(new FeatureDefinitionLightAffinity.LightingEffectAndCondition
            {
                lightingState = LocationDefinitions.LightingState.Darkness,
                condition = CustomConditionsContext.InvisibilityEveryRound
            })
            .AddToDB();

        // Improved Silhouette Step

        var powerWayOfSilhouetteImprovedSilhouetteStep = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ImprovedSilhouetteStep")
            .SetGuiPresentation(Category.Feature, DimensionDoor)
            .SetOverriddenPower(powerWayOfSilhouetteSilhouetteStep)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction, RechargeRate.ShortRest)
            .SetEffectDescription(DimensionDoor.EffectDescription)
            .SetUniqueInstance()
            .AddToDB();

        // LEVEL 17

        // Shadowy Sanctuary

        var powerWayOfSilhouetteShadowySanctuary = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerPatronTimekeeperTimeShift, $"Power{Name}ShadowySanctuary")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.KiPoints, 3)
            .SetReactionContext(ExtraReactionContext.Custom)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(FeatureDefinitionPowers.PowerPatronTimekeeperTimeShift)
                    .SetParticleEffectParameters(Banishment)
                    .Build())
            .SetShowCasting(true)
            .AddToDB();

        powerWayOfSilhouetteShadowySanctuary.AddCustomSubFeatures(
            new AttackBeforeHitConfirmedOnMeShadowySanctuary(powerWayOfSilhouetteShadowySanctuary));

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite(Name, Resources.WayOfTheSilhouette, 256))
            .AddFeaturesAtLevel(3,
                featureSetWayOfSilhouetteSilhouetteArts,
                lightAffinityWayOfSilhouetteCloakOfSilhouettesWeak,
                FeatureDefinitionCastSpells.CastSpellTraditionLight)
            .AddFeaturesAtLevel(6,
                powerWayOfSilhouetteSilhouetteStep,
                additionalDamageStrikeTheVitalsD6)
            .AddFeaturesAtLevel(11,
                lightAffinityCloakOfSilhouettesStrong,
                powerWayOfSilhouetteImprovedSilhouetteStep,
                additionalDamageStrikeTheVitalsD8)
            .AddFeaturesAtLevel(17,
                powerWayOfSilhouetteShadowySanctuary,
                additionalDamageStrikeTheVitalsD10)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Monk;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceMonkMonasticTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class CustomLevelUpLogicAdditionalDamageStrikeTheVitals : ICustomLevelUpLogic
    {
        private readonly FeatureDefinitionAdditionalDamage _additionalDamageToRemove;

        public CustomLevelUpLogicAdditionalDamageStrikeTheVitals(
            FeatureDefinitionAdditionalDamage additionalDamageToRemove)
        {
            _additionalDamageToRemove = additionalDamageToRemove;
        }

        public void ApplyFeature(RulesetCharacterHero hero, string tag)
        {
            foreach (var featureDefinitions in hero.ActiveFeatures.Values)
            {
                featureDefinitions.RemoveAll(x => x == _additionalDamageToRemove);
            }
        }

        public void RemoveFeature(RulesetCharacterHero hero, string tag)
        {
            // Empty
        }
    }

    private class AttackBeforeHitConfirmedOnMeShadowySanctuary : IAttackBeforeHitConfirmedOnMe
    {
        private readonly FeatureDefinitionPower _featureDefinitionPower;

        public AttackBeforeHitConfirmedOnMeShadowySanctuary(FeatureDefinitionPower featureDefinitionPower)
        {
            _featureDefinitionPower = featureDefinitionPower;
        }

        public IEnumerator OnAttackBeforeHitConfirmedOnMe(
            GameLocationBattleManager battle,
            GameLocationCharacter attacker,
            GameLocationCharacter me,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool firstTarget,
            bool criticalHit)
        {
            //do not trigger on my own turn, so won't retaliate on AoO
            if (Gui.Battle?.ActiveContenderIgnoringLegendary == me)
            {
                yield break;
            }

            if (!me.CanReact())
            {
                yield break;
            }

            var rulesetEnemy = attacker.RulesetCharacter;

            if (rulesetEnemy is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var gameLocationActionManager =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (gameLocationActionManager == null)
            {
                yield break;
            }

            var rulesetMe = me.RulesetCharacter;

            if (!rulesetMe.CanUsePower(_featureDefinitionPower))
            {
                yield break;
            }

            var usablePower = UsablePowersProvider.Get(_featureDefinitionPower, rulesetMe);
            var reactionParams =
                new CharacterActionParams(me, (ActionDefinitions.Id)ExtraActionId.DoNothingReaction)
                {
                    StringParameter = "ShadowySanctuary", UsablePower = usablePower
                };

            var previousReactionCount = gameLocationActionManager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestSpendPower(reactionParams);

            gameLocationActionManager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(me, gameLocationActionManager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            // remove any negative effect
            actualEffectForms.Clear();

            rulesetMe.UpdateUsageForPower(_featureDefinitionPower, _featureDefinitionPower.CostPerUse);

            var actionParams = new CharacterActionParams(me, ActionDefinitions.Id.SpendPower)
            {
                ActionDefinition = DatabaseHelper.ActionDefinitions.SpendPower,
                RulesetEffect = ServiceRepository.GetService<IRulesetImplementationService>()
                    .InstantiateEffectPower(rulesetMe, usablePower, false)
                    .AddAsActivePowerToSource()
            };

            actionParams.TargetCharacters.SetRange(me);

            ServiceRepository.GetService<ICommandService>()
                ?.ExecuteAction(actionParams, null, true);
        }
    }
}
