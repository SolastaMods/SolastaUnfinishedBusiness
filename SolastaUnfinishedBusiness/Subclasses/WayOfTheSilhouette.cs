using System.Collections;
using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.GameExtensions;
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

internal sealed class WayOfTheSilhouette : AbstractSubclass
{
    internal WayOfTheSilhouette()
    {
        var powerWayOfSilhouetteDarkness = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfSilhouetteDarkness")
            .SetGuiPresentation(Darkness.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints, 2, 2)
            .SetEffectDescription(Darkness.EffectDescription)
            .AddToDB();

        var powerWayOfSilhouetteDarkvision = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfSilhouetteDarkvision")
            .SetGuiPresentation(Darkvision.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints, 2, 2)
            .SetEffectDescription(Darkvision.EffectDescription)
            .AddToDB();

        var powerWayOfSilhouettePassWithoutTrace = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfSilhouettePassWithoutTrace")
            .SetGuiPresentation(PassWithoutTrace.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints, 2, 2)
            .SetEffectDescription(PassWithoutTrace.EffectDescription)
            .AddToDB();

        var powerWayOfSilhouetteSilence = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfSilhouetteSilence")
            .SetGuiPresentation(Silence.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints, 2, 2)
            .SetEffectDescription(Silence.EffectDescription)
            .AddToDB();

        var featureSetWayOfSilhouetteSilhouetteArts = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetWayOfSilhouetteSilhouetteArts")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                powerWayOfSilhouetteDarkness,
                powerWayOfSilhouetteDarkvision,
                powerWayOfSilhouettePassWithoutTrace,
                powerWayOfSilhouetteSilence)
            .AddToDB();

        var powerWayOfSilhouetteSilhouetteStep = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfSilhouetteSilhouetteStep")
            .SetGuiPresentation(Category.Feature, MistyStep)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(MistyStep.EffectDescription)
            .SetShowCasting(true)
            .AddToDB();

        var lightAffinityWayOfSilhouetteCloakOfSilhouettesWeak = FeatureDefinitionLightAffinityBuilder
            .Create("LightAffinityWayOfSilhouetteCloakOfSilhouettesWeak")
            .SetGuiPresentation(Category.Feature)
            .AddLightingEffectAndCondition(new FeatureDefinitionLightAffinity.LightingEffectAndCondition
            {
                lightingState = LocationDefinitions.LightingState.Unlit,
                condition = CustomConditionsContext.InvisibilityEveryRound
            })
            .AddToDB();

        var lightAffinityWayOfSilhouetteStrong = FeatureDefinitionLightAffinityBuilder
            .Create("LightAffinityWayOfSilhouetteCloakOfSilhouettesStrong")
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

        var powerWayOfSilhouetteImprovedSilhouetteStep = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfSilhouetteImprovedSilhouetteStep")
            .SetGuiPresentation(Category.Feature, DimensionDoor)
            .SetOverriddenPower(powerWayOfSilhouetteSilhouetteStep)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction, RechargeRate.ShortRest)
            .SetEffectDescription(DimensionDoor.EffectDescription)
            .SetUniqueInstance()
            .AddToDB();

        var powerWayOfSilhouetteShadowySanctuary = FeatureDefinitionPowerBuilder
            .Create(FeatureDefinitionPowers.PowerPatronTimekeeperTimeShift, "PowerWayOfSilhouetteShadowySanctuary")
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

        powerWayOfSilhouetteShadowySanctuary.SetCustomSubFeatures(
            new AttackBeforeHitConfirmedOnMeShadowySanctuary(powerWayOfSilhouetteShadowySanctuary));

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("WayOfSilhouette")
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite("WayOfTheSilhouette", Resources.WayOfTheSilhouette, 256))
            .AddFeaturesAtLevel(3,
                featureSetWayOfSilhouetteSilhouetteArts,
                lightAffinityWayOfSilhouetteCloakOfSilhouettesWeak,
                FeatureDefinitionCastSpells.CastSpellTraditionLight)
            .AddFeaturesAtLevel(6,
                lightAffinityWayOfSilhouetteStrong,
                powerWayOfSilhouetteSilhouetteStep)
            .AddFeaturesAtLevel(11,
                powerWayOfSilhouetteImprovedSilhouetteStep)
            .AddFeaturesAtLevel(17,
                powerWayOfSilhouetteShadowySanctuary)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceMonkMonasticTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

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
            bool criticalHit,
            bool firstTarget)
        {
            var gameLocationActionManager =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (gameLocationActionManager == null)
            {
                yield break;
            }

            if (!me.CanAct())
            {
                yield break;
            }

            var rulesetMe = me.RulesetCharacter;
            var usablePower = UsablePowersProvider.Get(_featureDefinitionPower, rulesetMe);

            if (!rulesetMe.CanUsePower(_featureDefinitionPower))
            {
                yield break;
            }

            //do not trigger on my own turn, so won't retaliate on AoO
            if (Gui.Battle?.ActiveContenderIgnoringLegendary == me)
            {
                yield break;
            }

            var rulesetEnemy = attacker.RulesetCharacter;

            if (!me.CanReact() ||
                rulesetEnemy == null ||
                rulesetEnemy.IsDeadOrDying)
            {
                yield break;
            }

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

            rulesetMe.UpdateUsageForPower(_featureDefinitionPower, _featureDefinitionPower.CostPerUse);

            var effect = new RulesetEffectPower(rulesetMe, usablePower);

            effect.ApplyEffectOnCharacter(rulesetMe, true, me.LocationPosition);
            actualEffectForms.Clear();
            attackMode.EffectDescription.EffectForms.Clear();
        }
    }
}
