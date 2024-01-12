using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
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
using SolastaUnfinishedBusiness.Models;
using TA;
using UnityEngine;
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
                    .SetEffectForms()
                    .Build())
            .AddCustomSubFeatures(new MagicEffectFinishedByMeDarkness())
            .AddToDB();

        #region

        // kept for backward compatibility
        _ = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Darkvision")
            .SetGuiPresentation(Darkvision.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints, 2, 2)
            .SetEffectDescription(Darkvision.EffectDescription)
            .AddToDB();

        // kept for backward compatibility
        _ = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}PassWithoutTrace")
            .SetGuiPresentation(PassWithoutTrace.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints, 2, 2)
            .SetEffectDescription(PassWithoutTrace.EffectDescription)
            .AddToDB();

        // kept for backward compatibility
        _ = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Silence")
            .SetGuiPresentation(Silence.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints, 2, 2)
            .SetEffectDescription(Silence.EffectDescription)
            .AddToDB();

        #endregion

        var featureSetWayOfSilhouetteSilhouetteArts = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}SilhouetteArts")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                SenseDarkvision12,
                powerDarkness)
            .AddToDB();

        #region

        // kept for backward compatibility
        _ = FeatureDefinitionLightAffinityBuilder
            .Create($"LightAffinity{Name}CloakOfSilhouettesWeak")
            .SetGuiPresentation(Category.Feature)
            .AddLightingEffectAndCondition(new FeatureDefinitionLightAffinity.LightingEffectAndCondition
            {
                lightingState = LocationDefinitions.LightingState.Unlit,
                condition = CustomConditionsContext.InvisibilityEveryRound
            })
            .AddToDB();

        #endregion

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

        #region

        // kept for backward compatibility
        _ = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}StrikeTheVitalsD6")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("StrikeTheVitals")
            .SetDamageDice(DieType.D6, 1)
            .SetRequiredProperty(RestrictedContextRequiredProperty.UnarmedOrMonkWeapon)
            .SetTriggerCondition(AdditionalDamageTriggerCondition.AdvantageOrNearbyAlly)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .AddToDB();

        // kept for backward compatibility
        _ = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}StrikeTheVitalsD8")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("StrikeTheVitals")
            .SetDamageDice(DieType.D8, 2)
            .SetRequiredProperty(RestrictedContextRequiredProperty.UnarmedOrMonkWeapon)
            .SetTriggerCondition(AdditionalDamageTriggerCondition.AdvantageOrNearbyAlly)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .AddToDB();

        // kept for backward compatibility
        _ = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}StrikeTheVitalsD10")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("StrikeTheVitals")
            .SetDamageDice(DieType.D10, 3)
            .SetRequiredProperty(RestrictedContextRequiredProperty.UnarmedOrMonkWeapon)
            .SetTriggerCondition(AdditionalDamageTriggerCondition.AdvantageOrNearbyAlly)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .AddToDB();

        #endregion

        // LEVEL 06

        var conditionSilhouetteStep = ConditionDefinitionBuilder
            .Create($"Condition{Name}SilhouetteStep")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHeraldOfBattle)
            .SetPossessive()
            .SetSpecialInterruptions(ConditionInterruption.Attacks, ConditionInterruption.AnyBattleTurnEnd)
            .SetFeatures(
                FeatureDefinitionCombatAffinityBuilder
                    .Create($"CombatAffinity{Name}SilhouetteStep")
                    .SetGuiPresentation($"Condition{Name}SilhouetteStep", Category.Condition, Gui.NoLocalization)
                    .SetMyAttackAdvantage(AdvantageType.Advantage)
                    .AddToDB())
            .AddToDB();

        var powerWayOfSilhouetteSilhouetteStep = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}SilhouetteStep")
            .SetGuiPresentation(Category.Feature, MistyStep)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.Position)
                    .SetDurationData(DurationType.Round)
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

        // this is key to ensure teleport will work correctly under Darkness conditions
        powerWayOfSilhouetteSilhouetteStep.EffectDescription.requiresVisibilityForPosition = false;
        
        // LEVEL 11

        #region

        // kept for backward compatibility
        _ = FeatureDefinitionLightAffinityBuilder
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

        // kept for backward compatibility
        _ = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ImprovedSilhouetteStep")
            .SetGuiPresentation(Category.Feature, DimensionDoor)
            .SetOverriddenPower(powerWayOfSilhouetteSilhouetteStep)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction, RechargeRate.ShortRest)
            .SetEffectDescription(DimensionDoor.EffectDescription)
            .SetUniqueInstance()
            .AddToDB();

        #endregion

        // Shadow Flurry

        var featureShadowFlurry = FeatureDefinitionBuilder
            .Create($"Feature{Name}ShadowFlurry")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureShadowFlurry.AddCustomSubFeatures(new TryAlterOutcomePhysicalAttackShadowFlurry(featureShadowFlurry));

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
            new ValidatorsValidatePowerUse(ValidatorsCharacter.IsNotInBrightLight),
            new AttackBeforeHitConfirmedOnMeShadowySanctuary(powerWayOfSilhouetteShadowySanctuary));

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
            var actingCharacter = action.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;
            var actionParams = action.ActionParams.Clone();

            actionParams.ActionDefinition = DatabaseHelper.ActionDefinitions.CastNoCost;
            actionParams.RulesetEffect = ServiceRepository.GetService<IRulesetImplementationService>()
                .InstantiateEffectSpell(rulesetCharacter, null, Darkness, -1, false);

            ServiceRepository.GetService<ICommandService>()?.ExecuteAction(actionParams, null, true);

            yield break;
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
            IAdditionalDamageProvider provider,
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
        public void EnumerateValidPositions(
            CursorLocationSelectPosition cursorLocationSelectPosition,
            List<int3> validPositions)
        {
            var visibilityService = ServiceRepository.GetService<IGameLocationVisibilityService>();
            var positioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
            var actingCharacter = cursorLocationSelectPosition.ActionParams.ActingCharacter;
            var savePosition = new int3(
                actingCharacter.LocationPosition.x,
                actingCharacter.LocationPosition.y,
                actingCharacter.LocationPosition.z);
            var boxInt = new BoxInt(cursorLocationSelectPosition.ActionParams.ActingCharacter.LocationPosition,
                new int3(0),
                new int3(0));

            boxInt.Inflate((int)cursorLocationSelectPosition.maxDistance);

            var maxSenseRange = actingCharacter.RulesetCharacter.GetFeaturesByType<FeatureDefinitionSense>()
                .Where(x =>
                    x.SenseType is SenseMode.Type.Blindsight or SenseMode.Type.Truesight or SenseMode.Type.Tremorsense)
                .Select(x => x.SenseRange)
                .AddItem(1)
                .Max();

            foreach (var int3 in boxInt.EnumerateAllPositionsWithin())
            {
                if (DistanceCalculation.GetDistanceFromTwoPositions(cursorLocationSelectPosition.centerPosition, int3) >
                    cursorLocationSelectPosition.maxDistance ||
                    !positioningService.CanPlaceCharacter(actingCharacter, int3, CellHelpers.PlacementMode.Station) ||
                    !positioningService.CanCharacterStayAtPosition_Floor(
                        actingCharacter, int3, onlyCheckCellsWithRealGround: true))
                {
                    continue;
                }

                actingCharacter.LocationPosition = int3;
                
                var lightningState = ComputeIllumination(
                    visibilityService as GameLocationVisibilityManager, actingCharacter, actingCharacter);

                actingCharacter.LocationPosition = savePosition;
                
                if (lightningState == LocationDefinitions.LightingState.Darkness)
                {
                    if (DistanceCalculation.GetDistanceFromTwoPositions(savePosition, int3) <= maxSenseRange)
                    {
                        cursorLocationSelectPosition.validPositionsCache.Add(int3);
                    }

                    continue;
                }
                                
                if (!visibilityService.IsCellPerceivedByCharacter(int3, actingCharacter))
                {
                    continue;
                }
                
                if (lightningState != LocationDefinitions.LightingState.Bright)
                {
                    cursorLocationSelectPosition.validPositionsCache.Add(int3);
                }
            }
        }

        private static LocationDefinitions.LightingState ComputeIllumination(
            GameLocationVisibilityManager __instance,
            IIlluminable illuminable,
            GameLocationCharacter previewCharacter = null,
            int3 previewPosition = default,
            GameLocationCharacter characterToIgnore = null)
        {
            var illumination1 = LocationDefinitions.LightingState.Unlit;
            if (!illuminable.Valid)
            {
                return illumination1;
            }

            __instance.positionCache.Clear();
            illuminable.GetAllPositionsToCheck(__instance.positionCache);
            if (__instance.positionCache == null || __instance.positionCache.Empty())
            {
                return illumination1;
            }

            if (illuminable is GameLocationCharacter locationCharacter1 &&
                locationCharacter1.RulesetCharacter != null &&
                locationCharacter1.RulesetCharacter.HasConditionOfType("ConditionDarkness"))
            {
                if (previewCharacter == null)
                {
                    illuminable.LightingState = LocationDefinitions.LightingState.Darkness;
                }

                return LocationDefinitions.LightingState.Darkness;
            }

            var gridAccessor = new GridAccessor(__instance.positionCache[0]);
            foreach (var position in __instance.positionCache)
            {
                if ((gridAccessor.RuntimeFlags(position) & CellFlags.Runtime.DynamicSightImpaired) !=
                    CellFlags.Runtime.None)
                {
                    if (previewCharacter == null)
                    {
                        illuminable.LightingState = LocationDefinitions.LightingState.Unlit;
                    }

                    return LocationDefinitions.LightingState.Darkness;
                }
            }

            var lightingState1 = LocationDefinitions.LightingState.Unlit;
            foreach (var position in __instance.positionCache)
            {
                gridAccessor.FetchSector(position);
                if (gridAccessor.sector != null &&
                    gridAccessor.sector.GlobalLightingState != LocationDefinitions.LightingState.Unlit)
                {
                    lightingState1 = gridAccessor.sector.GlobalLightingState;
                    if (lightingState1 == LocationDefinitions.LightingState.Bright)
                    {
                        if (previewCharacter == null)
                        {
                            illuminable.LightingState = lightingState1;
                        }

                        return LocationDefinitions.LightingState.Bright;
                    }
                }
            }

            __instance.lightsByDistance.Clear();
            foreach (var lightSources in __instance.lightSourcesMap)
            {
                var key = lightSources.Value;
                var locationPosition = key.LocationPosition;
                if (previewCharacter != null)
                {
                    using (var enumerator = __instance.charactersByLight.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            if (enumerator.Current.Key == key.RulesetLightSource &&
                                enumerator.Current.Value == previewCharacter)
                            {
                                __instance.ComputeCharacterLightPosition(previewCharacter, ref locationPosition, true,
                                    previewPosition);
                                break;
                            }
                        }
                    }
                }

                if (characterToIgnore != null)
                {
                    GameLocationCharacter locationCharacter2 = null;
                    if (__instance.charactersByLight.TryGetValue(key.RulesetLightSource, out locationCharacter2) &&
                        locationCharacter2 == characterToIgnore)
                    {
                        continue;
                    }
                }

                if (key.RulesetLightSource.DayCycleType == LightSourceDayCycleType.Always ||
                    key.RulesetLightSource.IsDayCycleActive)
                {
                    var dimRange = key.RulesetLightSource.DimRange;
                    var magnitude = (locationPosition - illuminable.Position).magnitude;
                    GameLocationCharacter locationCharacter3;
                    if (magnitude <= dimRange + (double)illuminable.DetectionRange &&
                        (!__instance.charactersByLight.TryGetValue(key.RulesetLightSource, out locationCharacter3) ||
                         (locationCharacter3 != null && locationCharacter3.IsValidForVisibility)))
                    {
                        __instance.lightsByDistance.Add(
                            new KeyValuePair<GameLocationLightSource, float>(key, magnitude));
                    }
                }
            }

            __instance.lightsByDistance.Sort(__instance.lightSortMethod);
            var lightingState2 = LocationDefinitions.LightingState.Unlit;
            foreach (var int3 in __instance.positionCache)
            {
                foreach (var keyValuePair in __instance.lightsByDistance)
                {
                    var key = keyValuePair.Key;
                    var dimRange = key.RulesetLightSource.DimRange;
                    var locationPosition = key.LocationPosition;
                    if (previewCharacter != null)
                    {
                        using (var enumerator = __instance.charactersByLight.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                if (enumerator.Current.Key == key.RulesetLightSource &&
                                    enumerator.Current.Value == previewCharacter)
                                {
                                    __instance.ComputeCharacterLightPosition(previewCharacter, ref locationPosition,
                                        true, previewPosition);
                                    break;
                                }
                            }
                        }
                    }

                    if (characterToIgnore != null)
                    {
                        GameLocationCharacter locationCharacter4 = null;
                        if (__instance.charactersByLight.TryGetValue(key.RulesetLightSource, out locationCharacter4) &&
                            locationCharacter4 == characterToIgnore)
                        {
                            continue;
                        }
                    }

                    var magnitudeSqr = (locationPosition - int3).magnitudeSqr;
                    if (magnitudeSqr.IsInferiorOrNearlyEqual(dimRange * dimRange))
                    {
                        var fromGridPosition1 =
                            __instance.gameLocationPositioningService.GetWorldPositionFromGridPosition(
                                key.LocationPosition);
                        var fromGridPosition2 =
                            __instance.gameLocationPositioningService.GetWorldPositionFromGridPosition(int3);
                        __instance.AdaptRayForVerticalityAndDiagonals(key.LocationPosition, int3, ref fromGridPosition1,
                            true);
                        var flag = true;
                        if (key.RulesetLightSource.IsSpot)
                        {
                            var to = fromGridPosition2 - fromGridPosition1;
                            to.Normalize();
                            flag = Vector3.Angle(key.RulesetLightSource.SpotDirection, to)
                                .IsInferiorOrNearlyEqual(key.RulesetLightSource.SpotAngle * 0.5f);
                        }

                        if (flag &&
                            !__instance.gameLocationPositioningService.RaycastGridSightBlocker(fromGridPosition1,
                                fromGridPosition2, __instance.GameLocationService) &&
                            !__instance.gameLocationPositioningService.IsSightImpaired(key.LocationPosition, int3))
                        {
                            lightingState2 =
                                key.RulesetLightSource.BrightRange <= 0.0 || magnitudeSqr >
                                key.RulesetLightSource.BrightRange * (double)key.RulesetLightSource.BrightRange
                                    ? LocationDefinitions.LightingState.Dim
                                    : LocationDefinitions.LightingState.Bright;
                            if (lightingState2 == LocationDefinitions.LightingState.Bright)
                            {
                                break;
                            }
                        }
                    }
                }

                if (lightingState2 != LocationDefinitions.LightingState.Unlit)
                {
                    break;
                }
            }

            var illumination2 =
                lightingState1 != LocationDefinitions.LightingState.Dim ||
                lightingState2 != LocationDefinitions.LightingState.Unlit
                    ? lightingState2
                    : LocationDefinitions.LightingState.Dim;
            if (previewCharacter == null)
            {
                illuminable.LightingState = illumination2;
            }

            return illumination2;
        }
    }

    //
    // Shadow Flurry
    //

    private class TryAlterOutcomePhysicalAttackShadowFlurry(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition featureShadowFlurry)
        : ITryAlterOutcomePhysicalAttack
    {
        public IEnumerator OnAttackTryAlterOutcome(
            GameLocationBattleManager battle,
            CharacterAction action,
            GameLocationCharacter me,
            GameLocationCharacter target,
            ActionModifier attackModifier)
        {
            var rulesetMe = me.RulesetCharacter;

            if (rulesetMe is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (!ValidatorsCharacter.IsNotInBrightLight(rulesetMe))
            {
                yield break;
            }

            if (!me.OncePerTurnIsValid(featureShadowFlurry.Name) || !me.PerceivedFoes.Contains(target))
            {
                yield break;
            }

            me.UsedSpecialFeatures.TryAdd(featureShadowFlurry.Name, 1);

            var attackMode = action.actionParams.attackMode;
            var totalRoll = (action.AttackRoll + attackMode.ToHitBonus).ToString();
            var rollCaption = action.AttackRoll == 1
                ? "Feedback/&RollCheckCriticalFailureTitle"
                : "Feedback/&CriticalAttackFailureOutcome";

            rulesetMe.LogCharacterUsedFeature(featureShadowFlurry,
                "Feedback/&TriggerRerollLine",
                false,
                (ConsoleStyleDuplet.ParameterType.Base, $"{action.AttackRoll}+{attackMode.ToHitBonus}"),
                (ConsoleStyleDuplet.ParameterType.FailedRoll, Gui.Format(rollCaption, totalRoll)));

            var roll = rulesetMe.RollAttack(
                attackMode.toHitBonus,
                target.RulesetCharacter,
                attackMode.sourceDefinition,
                attackModifier.attackToHitTrends,
                attackModifier.IgnoreAdvantage,
                attackModifier.AttackAdvantageTrends,
                attackMode.ranged,
                false,
                attackModifier.attackRollModifier,
                out var outcome,
                out var successDelta,
                -1,
                // testMode true avoids the roll to display on combat log as the original one will get there with altered results
                true);

            action.AttackRollOutcome = outcome;
            action.AttackSuccessDelta = successDelta;
            action.AttackRoll = roll;
        }
    }

    //
    // Shadowy Sanctuary
    //

    private class AttackBeforeHitConfirmedOnMeShadowySanctuary(FeatureDefinitionPower featureDefinitionPower)
        : IAttackBeforeHitConfirmedOnMe
    {
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
            if (!me.CanReact())
            {
                yield break;
            }

            var rulesetMe = me.RulesetCharacter;

            if (!ValidatorsCharacter.IsNotInBrightLight(rulesetMe))
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

            if (!rulesetMe.CanUsePower(featureDefinitionPower))
            {
                yield break;
            }

            var usablePower = UsablePowersProvider.Get(featureDefinitionPower, rulesetMe);
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

            rulesetMe.UpdateUsageForPower(featureDefinitionPower, featureDefinitionPower.CostPerUse);

            var actionParams = new CharacterActionParams(me, ActionDefinitions.Id.SpendPower)
            {
                ActionDefinition = DatabaseHelper.ActionDefinitions.SpendPower,
                RulesetEffect = ServiceRepository.GetService<IRulesetImplementationService>()
                    //CHECK: no need for AddAsActivePowerToSource
                    .InstantiateEffectPower(rulesetMe, usablePower, false)
            };

            actionParams.TargetCharacters.SetRange(me);

            EffectHelpers.StartVisualEffect(me, attacker,
                FeatureDefinitionPowers.PowerGlabrezuGeneralShadowEscape_at_will, EffectHelpers.EffectType.Caster);
            ServiceRepository.GetService<ICommandService>()
                ?.ExecuteAction(actionParams, null, false);
        }
    }
}
