using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Feats;
using SolastaUnfinishedBusiness.Interfaces;
using TA;
using UnityEngine;
using static LocationDefinitions;
using static SenseMode;
using static SolastaUnfinishedBusiness.Spells.SpellBuilders;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.EffectProxyDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionCombatAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPerceptionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.InvocationDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Models;

internal static class LightingAndObscurementContext
{
    private static string[] MonstersThatShouldHaveDarkvision { get; } =
    [
        "Adam_The_Twelth",
        "DLC3_Elven_07_Guard",
        "SRD_DLC_Mage",
        "SRD_Mage",
        "DLC1_NPC_Forge_Escorted_01",
        "DLC3_NPC_Generic_ElvenCitizen_Husk",
        "Generic_Darkweaver",
        "DLC3_ElvenClans_Leralyn",
        "DLC3_NPC_Elven3_DLC3_Ending",
        "DLC3_NPC_Elven5_DLC3_Ending",
        "Generic_HighPriest",
        "DLC3_Elven_Suspect_05_Guard_Traitor",
        "DLC3_Elven_06_Guard",
        "SRD_DLC3_Archmage",
        "Generic_ShockArcanist"
    ];

    private static string[] MonstersThatShouldHaveTrueSight { get; } =
    [
        "Couatl",
        "CubeOfLight"
    ];

    private static string[] MonstersThatShouldHaveBlindSight { get; } =
    [
        "Aksha",
        "Aksha_Legendary"
    ];

    private static string[] MonstersThatShouldNotHaveTremorSense { get; } =
    [
        "Aksha",
        "Aksha_Legendary"
    ];

    private static string[] EffectsThatTargetDistantIndividualsAndDontRequireSight { get; } =
    [
        "AcidSplash",
        "Aid",
        "AnimalShapes",
        "BeaconOfHope",
        "Bless",
        "BlessingOfRime",
        "BoomingBlade",
        "DispelMagic",
        "FeatherFall",
        "Knock",
        "Levitate",
        "MassCureWounds",
        "PassWithoutTrace",
        "PowerBarbarianBrutalStrike",
        "PowerBardHopeWordsOfHope6",
        "PowerBardTraditionManacalonsPerfection",
        "PowerCelestialSearingVengeance",
        "PowerCollegeOfAudacityDefensiveWhirl",
        "PowerCollegeOfAudacityMobileWhirl",
        "PowerCollegeOfValianceHeroicInspiration",
        "PowerDisableGrapple",
        "PowerDomainElementalHeraldOfTheElementsThunder",
        "PowerFeatChefCookMeal",
        "PowerFeatOrcishAggression",
        "PowerGrapple",
        "PowerInnovationWeaponArcaneJolt",
        "PowerOathOfJugementPurgeCorruption",
        "PowerOathOfJugementRetribution",
        "PowerOathOfThunderThunderousRebuke",
        "PowerPatronFiendHurlThroughHell",
        "PowerRangerHellWalkerMarkOfTheDammed",
        "PowerRangerLightBearerBlessedWarrior",
        "PowerRiftWalkerRiftStrike",
        "PowerSorcerousPsionMindOverMatter",
        "PowerSorcerousPsionMindOverMatter",
        "PowerTraditionOpenHandQuiveringPalmTrigger",
        "PowerWayOfTheDistantHandZenArrowTechnique",
        "PowerWayOfTheDistantHandZenArrowUpgradedTechnique",
        "RayOfEnfeeblement",
        "ResonatingStrike",
        "Sanctuary",
        "ShieldOfFaith",
        "Sparkle",
        "SunlightBlade",
        "TrueStrike"
    ];

    // called from GLBM.CanAttack to correctly determine ADV/DIS scenarios
    internal static void ApplyObscurementRules(BattleDefinitions.AttackEvaluationParams attackParams)
    {
        var attackAdvantageTrends = attackParams.attackModifier.AttackAdvantageTrends;
        var abilityCheckAdvantageTrends = attackParams.attackModifier.AbilityCheckAdvantageTrends;

        var defender = attackParams.defender;
        var defenderActor = defender.RulesetActor;
        var defenderHasAlertFeat =
            defenderActor is RulesetCharacter rulesetCharacter &&
            rulesetCharacter.GetOriginalHero() is { } rulesetCharacterHero &&
            rulesetCharacterHero.TrainedFeats.Contains(OtherFeats.FeatAlert);

        if (defenderHasAlertFeat)
        {
            if (attackAdvantageTrends.Any(BlindedAdvantage))
            {
                attackAdvantageTrends.RemoveAll(BlindedAdvantage);
                abilityCheckAdvantageTrends.RemoveAll(BlindedAdvantage);
            }

            if (attackAdvantageTrends.Any(InvisibleAdvantage))
            {
                attackAdvantageTrends.RemoveAll(InvisibleAdvantage);
                abilityCheckAdvantageTrends.RemoveAll(InvisibleAdvantage);
            }
        }

        if (!Main.Settings.UseOfficialLightingObscurementAndVisionRules ||
            attackParams.effectDescription is
                { RangeType: not (RuleDefinitions.RangeType.MeleeHit or RuleDefinitions.RangeType.RangeHit) })
        {
            return;
        }

        var attacker = attackParams.attacker;
        var attackerActor = attacker.RulesetActor;

        HandleTrueSightSpecialCase();

        if (Main.Settings.OfficialObscurementRulesCancelAdvDisPairs)
        {
            if (attackAdvantageTrends.Any(BlindedAdvantage) &&
                attackAdvantageTrends.Any(BlindedDisadvantage))
            {
                attackAdvantageTrends.RemoveAll(BlindedAdvantage);
                attackAdvantageTrends.RemoveAll(BlindedDisadvantage);

                abilityCheckAdvantageTrends.RemoveAll(BlindedAdvantage);
                abilityCheckAdvantageTrends.RemoveAll(BlindedDisadvantage);
            }

            if (attackAdvantageTrends.Any(InvisibleAdvantage) &&
                attackAdvantageTrends.Any(InvisibleDisadvantage))
            {
                attackAdvantageTrends.RemoveAll(InvisibleAdvantage);
                attackAdvantageTrends.RemoveAll(InvisibleDisadvantage);

                abilityCheckAdvantageTrends.RemoveAll(InvisibleAdvantage);
                abilityCheckAdvantageTrends.RemoveAll(InvisibleDisadvantage);
            }
        }

        const string TAG = "Perceive";

        var attackerIsBlind = attackerActor.HasConditionOfTypeOrSubType(RuleDefinitions.ConditionBlinded);
        var attackerIsInvisible = attackerActor.HasConditionOfTypeOrSubType(RuleDefinitions.ConditionInvisible);
        var attackerIsStealthy = attackerActor.HasConditionOfType(ConditionStealthy);
        var attackerHasNoLight = attacker.LightingState is LightingState.Unlit or LightingState.Darkness;

        var defenderIsBlind = defenderActor.HasConditionOfTypeOrSubType(RuleDefinitions.ConditionBlinded);
        var defenderIsInvisible = defenderActor.HasConditionOfTypeOrSubType(RuleDefinitions.ConditionInvisible);
        var defenderHasNoLight = defender.LightingState is LightingState.Unlit or LightingState.Darkness;

        var attackerCanPerceiveDefender = attacker.CanPerceiveTarget(defender);
        var defenderCanPerceiveAttacker = defenderHasAlertFeat || defender.CanPerceiveTarget(attacker);

        var adv =
            !attackerIsStealthy &&
            !attackerIsInvisible &&
            !defenderIsBlind &&
            !defenderCanPerceiveAttacker &&
            (attackerCanPerceiveDefender || attackerHasNoLight);

        var dis =
            !defenderIsInvisible &&
            !attackerIsBlind &&
            !attackerCanPerceiveDefender &&
            (defenderCanPerceiveAttacker || defenderHasNoLight);

        if (Main.Settings.OfficialObscurementRulesCancelAdvDisPairs && !(adv ^ dis))
        {
            return;
        }

        if (adv)
        {
            attackAdvantageTrends.Add(PerceiveAdvantage());
            abilityCheckAdvantageTrends.Add(PerceiveAdvantage());
        }

        // ReSharper disable once InvertIf
        if (dis)
        {
            attackAdvantageTrends.Add(PerceiveDisadvantage());
            abilityCheckAdvantageTrends.Add(PerceiveDisadvantage());
        }

        return;

        RuleDefinitions.TrendInfo PerceiveAdvantage()
        {
            return new RuleDefinitions.TrendInfo(1, RuleDefinitions.FeatureSourceType.Lighting, TAG, attackerActor);
        }

        RuleDefinitions.TrendInfo PerceiveDisadvantage()
        {
            return new RuleDefinitions.TrendInfo(-1, RuleDefinitions.FeatureSourceType.Lighting, TAG, defenderActor);
        }

        static bool BlindedAdvantage(RuleDefinitions.TrendInfo trendInfo)
        {
            return trendInfo is { sourceName: RuleDefinitions.ConditionBlinded, value: 1 };
        }

        static bool BlindedDisadvantage(RuleDefinitions.TrendInfo trendInfo)
        {
            return trendInfo is { sourceName: RuleDefinitions.ConditionBlinded, value: -1 };
        }

        static bool InvisibleAdvantage(RuleDefinitions.TrendInfo trendInfo)
        {
            return trendInfo is { sourceName: RuleDefinitions.ConditionInvisible, value: 1 };
        }

        static bool InvisibleDisadvantage(RuleDefinitions.TrendInfo trendInfo)
        {
            return trendInfo is { sourceName: RuleDefinitions.ConditionInvisible, value: -1 };
        }

        // conditions with parent inherit their features which makes true sight quite hard to manage
        // the combat affinity won't have true sight as nullified sense, so we check it here and revert
        void HandleTrueSightSpecialCase()
        {
            if (IsBlindNotFromDarkness(attackerActor) ||
                IsBlindNotFromDarkness(defenderActor))
            {
                return;
            }

            if (attackerActor is RulesetCharacter attackerCharacter)
            {
                var senseModeTrueSightAttacker =
                    attackerCharacter.SenseModes.FirstOrDefault(x => x.SenseType == Type.Truesight);

                if (senseModeTrueSightAttacker != null &&
                    attacker.IsWithinRange(defender, senseModeTrueSightAttacker.SenseRange))
                {
                    attackAdvantageTrends.RemoveAll(BlindedDisadvantage);
                }
            }

            // ReSharper disable once InvertIf
            if (defenderActor is RulesetCharacter defenderCharacter)
            {
                var senseModeTrueSightDefender =
                    defenderCharacter.SenseModes.FirstOrDefault(x => x.SenseType == Type.Truesight);

                if (senseModeTrueSightDefender != null &&
                    defender.IsWithinRange(attacker, senseModeTrueSightDefender.SenseRange))
                {
                    attackAdvantageTrends.RemoveAll(BlindedAdvantage);
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsMagicEffectValidIfHeavilyObscuredOrInNaturalDarkness(
        GameLocationCharacter source,
        IMagicEffect magicEffect,
        GameLocationCharacter target)
    {
        return target == null ||
               !Main.Settings.UseOfficialLightingObscurementAndVisionRules ||
               magicEffect.EffectDescription is not
               {
                   RangeType: RuleDefinitions.RangeType.Distance,
                   TargetType: RuleDefinitions.TargetType.Individuals or RuleDefinitions.TargetType.IndividualsUnique
               } ||
               EffectsThatTargetDistantIndividualsAndDontRequireSight.Contains(magicEffect.Name) ||
               source.CanPerceiveTarget(target);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsBlindFromDarkness(RulesetActor actor)
    {
        return actor != null && actor.HasConditionOfType(ConditionBlindedByDarkness);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsBlindNotFromDarkness(RulesetActor actor)
    {
        return
            actor != null &&
            actor.ConditionsByCategory
                .SelectMany(x => x.Value)
                .Select(y => y.ConditionDefinition)
                .Any(z => z.IsSubtypeOf(ConditionBlinded.Name) && z != ConditionBlindedByDarkness);
    }

    // improved cell perception routine that takes sight into consideration
    // most of the usages is to determine if a character can perceive a cell in teleport scenarios
    // when target not null it helps determine visibility on attacks and effects targeting scenarios
    internal static bool MyIsCellPerceivedByCharacter(
        this IGameLocationVisibilityService instance,
        int3 cellPosition,
        GameLocationCharacter sensor,
        GameLocationCharacter target = null,
        LightingState additionalBlockedLightingState = LightingState.Darkness,
        bool requireLineOfSight = false)
    {
        // gadgets cannot perceive anything
        if (sensor.RulesetActor is RulesetGadget)
        {
            return false;
        }

        // if a proxy need to perceive from controller
        var finalSensor = sensor;

        if (sensor.RulesetCharacter is RulesetCharacterEffectProxy effectProxy)
        {
            var controller = EffectHelpers.GetCharacterByGuid(effectProxy.ControllerGuid);

            if (controller != null)
            {
                var locationController = GameLocationCharacter.GetFromActor(controller);

                if (locationController != null)
                {
                    finalSensor = locationController;
                }
            }
        }

        //check line of sight
        if ((requireLineOfSight || !Main.Settings.UseOfficialLightingObscurementAndVisionRules)
            && !instance.IsCellPerceivedByCharacter(cellPosition, finalSensor))
        {
            return false;
        }

        // use the improved lighting state detection to diff between darkness and heavily obscured
        var targetLightingState = ComputeLightingStateOnTargetPosition(finalSensor, cellPosition);

        // use vanilla if setting is off but still supporting additionalBlockedLightingState logic
        if (!Main.Settings.UseOfficialLightingObscurementAndVisionRules)
        {
            // Silhouette Step is the only one using additionalBlockedLightingState as it requires to block BRIGHT
            return additionalBlockedLightingState == LightingState.Darkness ||
                   targetLightingState != additionalBlockedLightingState;
        }

        // determine constraints
        // must use vanilla distance calculation here
        var distance = int3.Distance(finalSensor.LocationPosition, cellPosition);
        var sensorCharacter = finalSensor.RulesetCharacter;
        var sourceIsBlindFromDarkness = IsBlindFromDarkness(sensorCharacter);
        var sourceIsBlindNotFromDarkness = IsBlindNotFromDarkness(sensorCharacter);
        var targetIsNotTouchingGround = target != null && !target.RulesetActor.IsTouchingGround();
        var targetIsInvisible =
            target != null && target.RulesetActor.HasConditionOfTypeOrSubType(ConditionInvisible.Name);

        var senseModesToPrevent = new List<Type>();

        if (target != null)
        {
            foreach (var modifier in target.RulesetActor.GetSubFeaturesByType<IPreventEnemySenseMode>())
            {
                senseModesToPrevent.AddRange(modifier.PreventedSenseModes(finalSensor, target.RulesetCharacter));
            }
        }

        // try to find any sense mode that is valid for the current lighting state and constraints
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var senseMode in sensorCharacter.SenseModes
                     .Where(x => !senseModesToPrevent.Contains(x.SenseType)))
        {
            if (distance > senseMode.SenseRange)
            {
                continue;
            }

            var senseType = senseMode.SenseType;

            // UNLIT
            if (targetLightingState is LightingState.Unlit && senseType is Type.NormalVision)
            {
                continue;
            }

            // MAGICAL DARKNESS
            if (sourceIsBlindFromDarkness && senseType is
                    Type.DetectInvisibility or
                    Type.NormalVision or
                    Type.Darkvision or
                    Type.SuperiorDarkvision)
            {
                continue;
            }

            if (targetLightingState is LightingState.Darkness && senseType is
                    Type.DetectInvisibility or
                    Type.NormalVision or
                    Type.Darkvision or
                    Type.SuperiorDarkvision)
            {
                continue;
            }

            // HEAVILY OBSCURED
            if (sourceIsBlindNotFromDarkness && senseType is
                    Type.DetectInvisibility or
                    Type.NormalVision or
                    Type.Darkvision or
                    Type.SuperiorDarkvision or
                    Type.Truesight)
            {
                continue;
            }

            if (targetLightingState is (LightingState)MyLightingState.HeavilyObscured && senseType is
                    Type.DetectInvisibility or
                    Type.NormalVision or
                    Type.Darkvision or
                    Type.SuperiorDarkvision or
                    Type.Truesight)
            {
                continue;
            }

            // TREMOR SENSE
            if (targetIsNotTouchingGround && senseType is Type.Tremorsense)
            {
                continue;
            }

            // INVISIBLE
            if (targetIsInvisible && senseType is
                    Type.NormalVision or
                    Type.Darkvision or
                    Type.SuperiorDarkvision or
                    Type.Truesight)
            {
                continue;
            }

            // Silhouette Step is the only one using additionalBlockedLightingState as it requires to block BRIGHT
            return additionalBlockedLightingState == LightingState.Darkness ||
                   targetLightingState != additionalBlockedLightingState;
        }

        return false;
    }

    private static LightingState ComputeLightingStateOnTargetPosition(
        GameLocationCharacter instance,
        int3 targetPosition)
    {
        // this is a hack to allow calling GetAllPositionsToCheck inside ComputeIllumination
        // we save the character's current position and set it to the target location we need to calculate
        var savePosition = new int3(
            instance.LocationPosition.x,
            instance.LocationPosition.y,
            instance.LocationPosition.z);

        instance.LocationPosition = targetPosition;

        var illumination = ComputeIllumination(instance, targetPosition);

        instance.LocationPosition = savePosition;

        return illumination;

        //
        // extended lighting state determination routine that differentiates darkness effect from other obscured ones
        //
        static LightingState ComputeIllumination(IIlluminable illuminable, int3 targetPosition)
        {
            var visibilityManager =
                ServiceRepository.GetService<IGameLocationVisibilityService>() as GameLocationVisibilityManager;

            visibilityManager!.positionCache.Clear();
            illuminable.GetAllPositionsToCheck(visibilityManager.positionCache);

            if (visibilityManager.positionCache == null || visibilityManager.positionCache.Count == 0)
            {
                return LightingState.Unlit;
            }

            //
            // try to determine if heavily obscured or darkness
            //

            // alternate heavily obscured determination that differentiate between heavily obscured and darkness
            var gridAccessor = new GridAccessor(visibilityManager.positionCache[0]);
            var locationCharacters = new List<GameLocationCharacter>();

            if (gridAccessor.Occupants_TryGet(targetPosition, out var value))
            {
                locationCharacters.SetRange(value);
            }

            var isDarkness = false;
            var targetPositionCharacterLightingState = (LightingState)MyLightingState.Invalid;

            foreach (var locationCharacter in locationCharacters)
            {
                if (locationCharacter.RulesetActor is RulesetCharacterHero or RulesetCharacterMonster)
                {
                    targetPositionCharacterLightingState = locationCharacter.LightingState;

                    continue;
                }

                if (locationCharacter.RulesetActor is not RulesetCharacterEffectProxy rulesetProxy)
                {
                    continue;
                }

                if (rulesetProxy.EffectProxyDefinition == ProxyDarkness)
                {
                    isDarkness = true;
                }
                else
                {
                    if (SightImpairedProxies.All(effectProxy => rulesetProxy.EffectProxyDefinition != effectProxy))
                    {
                        continue;
                    }

                    return (LightingState)MyLightingState.HeavilyObscured;
                }
            }

            // it's indeed only darkness and no other heavily obscured effect
            if (isDarkness)
            {
                return LightingState.Darkness;
            }

            // if there is a character at target position don't waste time re-calculating it's lighting state
            if (targetPositionCharacterLightingState != (LightingState)MyLightingState.Invalid)
            {
                return targetPositionCharacterLightingState;
            }

            //
            // try to determine if outside and if in bright daylight exit earlier
            //

            var globalLightingState = LightingState.Unlit;

            foreach (var position in visibilityManager.positionCache)
            {
                gridAccessor.FetchSector(position);

                if (gridAccessor.sector == null ||
                    gridAccessor.sector.GlobalLightingState == LightingState.Unlit)
                {
                    continue;
                }

                globalLightingState = gridAccessor.sector.GlobalLightingState;

                if (globalLightingState != LightingState.Bright)
                {
                    continue;
                }

                return LightingState.Bright;
            }

            //
            // try to fetch all light sources to correctly determine if bright, dim or unlit
            //

            visibilityManager.lightsByDistance.Clear();

            foreach (var lightSources in visibilityManager.lightSourcesMap)
            {
                var key = lightSources.Value;
                var locationPosition = key.LocationPosition;

                if (key.RulesetLightSource.DayCycleType != RuleDefinitions.LightSourceDayCycleType.Always &&
                    !key.RulesetLightSource.IsDayCycleActive)
                {
                    continue;
                }

                var dimRange = key.RulesetLightSource.DimRange;
                var magnitude = (locationPosition - targetPosition).magnitude;

                if (magnitude <= dimRange + illuminable.DetectionRange &&
                    (!visibilityManager.charactersByLight.TryGetValue(key.RulesetLightSource, out var glc) ||
                     glc is { IsValidForVisibility: true }))
                {
                    visibilityManager.lightsByDistance.Add(
                        new KeyValuePair<GameLocationLightSource, float>(key, magnitude));
                }
            }

            visibilityManager.lightsByDistance.Sort(visibilityManager.lightSortMethod);

            var lightsLightingState = LightingState.Unlit;

            foreach (var int3 in visibilityManager.positionCache)
            {
                foreach (var keyValuePair in visibilityManager.lightsByDistance)
                {
                    var key = keyValuePair.Key;
                    var dimRange = key.RulesetLightSource.DimRange;
                    var locationPosition = key.LocationPosition;
                    var magnitudeSqr = (locationPosition - int3).magnitudeSqr;

                    if (!magnitudeSqr.IsInferiorOrNearlyEqual(dimRange * dimRange))
                    {
                        continue;
                    }

                    var hasLineOfSight = true;
                    var sourcePosition =
                        visibilityManager.gameLocationPositioningService.GetWorldPositionFromGridPosition(
                            locationPosition);
                    var destinationPosition =
                        visibilityManager.gameLocationPositioningService.GetWorldPositionFromGridPosition(int3);

                    visibilityManager.AdaptRayForVerticalityAndDiagonals(
                        locationPosition, int3, ref sourcePosition, true);

                    if (key.RulesetLightSource.IsSpot)
                    {
                        var to = destinationPosition - sourcePosition;

                        to.Normalize();
                        hasLineOfSight = Vector3.Angle(key.RulesetLightSource.SpotDirection, to)
                            .IsInferiorOrNearlyEqual(key.RulesetLightSource.SpotAngle * 0.5f);
                    }

                    if (!hasLineOfSight ||
                        visibilityManager.gameLocationPositioningService.RaycastGridSightBlocker(
                            sourcePosition, destinationPosition, visibilityManager.GameLocationService) ||
                        visibilityManager.gameLocationPositioningService.IsSightImpaired(locationPosition, int3))
                    {
                        continue;
                    }

                    lightsLightingState =
                        key.RulesetLightSource.BrightRange <= 0.0 || magnitudeSqr >
                        key.RulesetLightSource.BrightRange * key.RulesetLightSource.BrightRange
                            ? LightingState.Dim
                            : LightingState.Bright;

                    if (lightsLightingState == LightingState.Bright)
                    {
                        break;
                    }
                }

                if (lightsLightingState != LightingState.Unlit)
                {
                    break;
                }
            }

            return globalLightingState != LightingState.Dim || lightsLightingState != LightingState.Unlit
                ? lightsLightingState
                : LightingState.Dim;
        }
    }

    private enum MyLightingState
    {
        HeavilyObscured = 9000,
        Invalid = 9001
    }

    #region conditions

    private const string BlindTitle = "Condition/&ConditionBlindedByTitle";
    private const string BlindDescription = "Rules/&ConditionBlindedDescription";
    private const string BlindExtendedDescription = "Condition/&ConditionBlindedExtendedDescription";

    // ProxyDarkness is a special use case that is handled apart
    private static readonly HashSet<EffectProxyDefinition> SightImpairedProxies =
    [
        ProxyCloudKill,
        ProxyFogCloud,
        ProxyIncendiaryCloud,
        ProxyPetalStorm,
        ProxySleetStorm,
        ProxyStinkingCloud
    ];

    internal static readonly ConditionDefinition ConditionBlindedByDarkness = ConditionDefinitionBuilder
        .Create(ConditionBlinded, "ConditionBlindedByDarkness")
        .SetGuiPresentation(Gui.Format(BlindTitle, Darkness.FormatTitle()), BlindDescription, ConditionBlinded)
        .SetParentCondition(ConditionBlinded)
        .SetFeatures()
        .AddToDB();

    private static readonly ConditionDefinition ConditionBlindedByCloudKill = ConditionDefinitionBuilder
        .Create(ConditionBlinded, "ConditionBlindedByCloudKill")
        .SetGuiPresentation(Gui.Format(BlindTitle, CloudKill.FormatTitle()), BlindDescription, ConditionBlinded)
        .SetParentCondition(ConditionBlinded)
        .SetFeatures()
        .AddToDB();

    private static readonly ConditionDefinition ConditionBlindedByFogCloud = ConditionDefinitionBuilder
        .Create(ConditionBlinded, "ConditionBlindedByFogCloud")
        .SetGuiPresentation(Gui.Format(BlindTitle, FogCloud.FormatTitle()), BlindDescription, ConditionBlinded)
        .SetParentCondition(ConditionBlinded)
        .SetFeatures()
        .AddToDB();

    private static readonly ConditionDefinition ConditionBlindedByIncendiaryCloud = ConditionDefinitionBuilder
        .Create(ConditionBlinded, "ConditionBlindedByIncendiaryCloud")
        .SetGuiPresentation(Gui.Format(BlindTitle, IncendiaryCloud.FormatTitle()), BlindDescription, ConditionBlinded)
        .SetParentCondition(ConditionBlinded)
        .SetFeatures()
        .AddToDB();

    private static readonly ConditionDefinition ConditionBlindedByPetalStorm = ConditionDefinitionBuilder
        .Create(ConditionBlinded, "ConditionBlindedByPetalStorm")
        .SetGuiPresentation(Gui.Format(BlindTitle, SpellsContext.PetalStorm.FormatTitle()), BlindDescription,
            ConditionBlinded)
        .SetParentCondition(ConditionBlinded)
        .SetFeatures()
        .AddToDB();

    private static readonly ConditionDefinition ConditionBlindedBySleetStorm = ConditionDefinitionBuilder
        .Create(ConditionBlinded, "ConditionBlindedBySleetStorm")
        .SetGuiPresentation(Gui.Format(BlindTitle, SleetStorm.FormatTitle()), BlindDescription, ConditionBlinded)
        .SetParentCondition(ConditionBlinded)
        .SetFeatures()
        .AddToDB();

    private static readonly ConditionDefinition ConditionBlindedByStinkingCloud = ConditionDefinitionBuilder
        .Create(ConditionBlinded, "ConditionBlindedByStinkingCloud")
        .SetGuiPresentation(Gui.Format(BlindTitle, StinkingCloud.FormatTitle()), BlindDescription, ConditionBlinded)
        .SetParentCondition(ConditionBlinded)
        .SetFeatures(ConditionPoisoned.Features)
        .AddFeatures(ActionAffinityConditionRetchingReeling)
        .AddToDB();

    internal static readonly ConditionDefinition ConditionLightlyObscured = ConditionDefinitionBuilder
        .Create(ConditionHeavilyObscured, "ConditionLightlyObscured")
        .SetOrUpdateGuiPresentation(Category.Condition)
        .SetFeatures(
            FeatureDefinitionAbilityCheckAffinityBuilder
                .Create("AbilityCheckAffinityLightlyObscured")
                .SetOrUpdateGuiPresentation("ConditionLightlyObscured", Category.Condition)
                .BuildAndSetAffinityGroups(RuleDefinitions.CharacterAbilityCheckAffinity.Disadvantage,
                    abilityProficiencyPairs: (AttributeDefinitions.Wisdom, SkillDefinitions.Perception))
                .AddToDB())
        .AddToDB();

    private static readonly EffectForm FormBlindedByCloudKill =
        EffectFormBuilder.ConditionForm(ConditionBlindedByCloudKill);

    private static readonly EffectForm FormBlindedByIncendiaryCloud =
        EffectFormBuilder.ConditionForm(ConditionBlindedByIncendiaryCloud);

    private static readonly EffectForm FormLightlyObscured = EffectFormBuilder.ConditionForm(ConditionLightlyObscured);

    private static readonly EffectForm FormProjectileBlocker = EffectFormBuilder
        .Create()
        .SetTopologyForm(TopologyForm.Type.ProjectileBlocker, true)
        .Build();

    #endregion

    #region mod UI

    internal static void LateLoad()
    {
        SwitchOfficialObscurementRules();
    }

    internal static void SwitchOfficialObscurementRules()
    {
        var searchTerm = Main.Settings.UseOfficialLightingObscurementAndVisionRules
            ? BlindDescription
            : BlindExtendedDescription;

        var replaceTerm = !Main.Settings.UseOfficialLightingObscurementAndVisionRules
            ? BlindDescription
            : BlindExtendedDescription;

        foreach (var condition in DatabaseRepository.GetDatabase<ConditionDefinition>()
                     .Where(x => x.GuiPresentation.description == searchTerm))
        {
            condition.GuiPresentation.description = replaceTerm;
        }

        SwitchCombatAffinityInvisibleSenses();
        SwitchHeavilyObscuredOnObscurementRules();
        SwitchMagicalDarknessOnObscurementRules();
        SwitchMonstersOnObscurementRules();

        if (Main.Settings.UseOfficialLightingObscurementAndVisionRules)
        {
            ConditionBlinded.Features.SetRange(
                CombatAffinityHeavilyObscured,
                CombatAffinityHeavilyObscuredSelf,
                PerceptionAffinityConditionBlinded);

            // >> ConditionVeil
            // ConditionAffinityVeilImmunity
            // PowerDefilerDarkness

            ConditionAffinityVeilImmunity.conditionType = ConditionBlindedByDarkness.Name;

            PowerDefilerDarkness.EffectDescription.EffectForms[1].ConditionForm.ConditionDefinition =
                ConditionBlindedByDarkness;

            // >> ConditionDarkness
            // ConditionAffinityInvocationDevilsSight
            // Darkness

            FeatureSetInvocationDevilsSight.FeatureSet.SetRange(SenseTruesight16, SenseDarkvision24);
            DevilsSight.GuiPresentation.description = "Invocation/&DevilsSightExtendedDescription";

            Darkness.EffectDescription.EffectForms[1].ConditionForm.ConditionDefinition = ConditionBlindedByDarkness;
            SpellsContext.MaddeningDarkness.EffectDescription.EffectForms[1].ConditionForm.ConditionDefinition =
                ConditionBlindedByDarkness;

            // >> ConditionHeavilyObscured
            // FogCloud
            // PetalStorm

            FogCloud.EffectDescription.EffectForms[0].ConditionForm.ConditionDefinition = ConditionBlindedByFogCloud;

            SpellsContext.PetalStorm.EffectDescription.EffectForms[1].ConditionForm.ConditionDefinition =
                ConditionBlindedByPetalStorm;

            // >> ConditionInStinkingCloud
            // StinkingCloud

            StinkingCloud.EffectDescription.EffectForms[0].ConditionForm.ConditionDefinition =
                ConditionBlindedByStinkingCloud;

            // >> ConditionSleetStorm
            // SleetStorm

            SleetStorm.EffectDescription.EffectForms[0].ConditionForm.ConditionDefinition =
                ConditionBlindedBySleetStorm;

            // Cloud Kill / Incendiary Cloud need same debuff as other heavily obscured
            CloudKill.EffectDescription.EffectForms.TryAdd(FormBlindedByCloudKill);
            IncendiaryCloud.EffectDescription.EffectForms.TryAdd(FormBlindedByIncendiaryCloud);

            // Make Insect Plague lightly obscured
            InsectPlague.EffectDescription.EffectForms.Add(FormLightlyObscured);
            InsectPlague.EffectDescription.EffectForms[1].TopologyForm.changeType = TopologyForm.Type.None;

            // vanilla has this set as disadvantage, so we flip it with nullified requirements
            CombatAffinityHeavilyObscured.attackOnMeAdvantage = RuleDefinitions.AdvantageType.Advantage;
            CombatAffinityHeavilyObscured.nullifiedBySenses = [];
            CombatAffinityHeavilyObscured.nullifiedBySelfSenses = [Type.Blindsight, Type.Tremorsense];

            CombatAffinityHeavilyObscuredSelf.nullifiedBySenses = [];
            CombatAffinityHeavilyObscuredSelf.nullifiedBySelfSenses = [Type.Blindsight, Type.Tremorsense];
        }
        else
        {
            ConditionBlinded.Features.SetRange(
                CombatAffinityBlinded,
                PerceptionAffinityConditionBlinded);

            // >> ConditionVeil
            // ConditionAffinityVeilImmunity
            // PowerDefilerDarkness

            ConditionAffinityVeilImmunity.conditionType = ConditionVeil.Name;

            PowerDefilerDarkness.EffectDescription.EffectForms[1].ConditionForm.ConditionDefinition = ConditionVeil;

            // >> ConditionDarkness
            // ConditionAffinityInvocationDevilsSight
            // Darkness

            FeatureSetInvocationDevilsSight.FeatureSet.SetRange(
                SenseBlindSight16,
                SenseSeeInvisible16,
                ConditionAffinityInvocationDevilsSight);
            DevilsSight.GuiPresentation.description = "Invocation/&DevilsSightDescription";

            Darkness.EffectDescription.EffectForms[1].ConditionForm.ConditionDefinition = ConditionDarkness;
            SpellsContext.MaddeningDarkness.EffectDescription.EffectForms[1].ConditionForm.ConditionDefinition =
                ConditionDarkness;

            // >> ConditionHeavilyObscured
            // FogCloud
            // PetalStorm

            FogCloud.EffectDescription.EffectForms[0].ConditionForm.ConditionDefinition = ConditionHeavilyObscured;

            SpellsContext.PetalStorm.EffectDescription.EffectForms[1].ConditionForm.ConditionDefinition =
                ConditionHeavilyObscured;

            // >> ConditionInStinkingCloud
            // StinkingCloud

            StinkingCloud.EffectDescription.EffectForms[0].ConditionForm.ConditionDefinition = ConditionInStinkingCloud;

            // >> ConditionSleetStorm
            // SleetStorm

            SleetStorm.EffectDescription.EffectForms[0].ConditionForm.ConditionDefinition = ConditionSleetStorm;

            // Cloud Kill / Incendiary Cloud need same debuff as other heavily obscured
            CloudKill.EffectDescription.EffectForms.Remove(FormBlindedByCloudKill);
            IncendiaryCloud.EffectDescription.EffectForms.Remove(FormBlindedByIncendiaryCloud);

            // Remove lightly obscured from Insect Plague
            InsectPlague.EffectDescription.EffectForms.Remove(FormLightlyObscured);
            InsectPlague.effectDescription.EffectForms[1].TopologyForm.changeType = TopologyForm.Type.SightImpaired;

            // vanilla has this set as disadvantage, so we flip it with nullified requirements
            CombatAffinityHeavilyObscured.attackOnMeAdvantage = RuleDefinitions.AdvantageType.Disadvantage;
            CombatAffinityHeavilyObscured.nullifiedBySenses = [Type.Truesight, Type.Blindsight];
            CombatAffinityHeavilyObscured.nullifiedBySelfSenses = [];

            CombatAffinityHeavilyObscuredSelf.nullifiedBySenses = [];
            CombatAffinityHeavilyObscuredSelf.nullifiedBySelfSenses = [Type.Truesight, Type.Blindsight];
        }

        Tabletop2014Context.SwitchConditionBlindedShouldNotAllowOpportunityAttack();
    }

    private static void SwitchCombatAffinityInvisibleSenses()
    {
        var modSenses = new List<Type> { Type.Blindsight, Type.Tremorsense, Type.Truesight, Type.DetectInvisibility };
        var vanillaSenses = new List<Type> { Type.Truesight, Type.DetectInvisibility };

        if (Main.Settings.UseOfficialLightingObscurementAndVisionRules)
        {
            CombatAffinityInvisible.nullifiedBySenses = modSenses;
            CombatAffinityInvisibleStalker.nullifiedBySenses = modSenses;
        }
        else
        {
            CombatAffinityInvisible.nullifiedBySenses = vanillaSenses;
            CombatAffinityInvisibleStalker.nullifiedBySenses = vanillaSenses;
        }
    }

    internal static void SwitchHeavilyObscuredOnObscurementRules()
    {
        if (Main.Settings.OfficialObscurementRulesHeavilyObscuredAsProjectileBlocker)
        {
            FogCloud.EffectDescription.EffectForms.TryAdd(FormProjectileBlocker);
            SpellsContext.PetalStorm.EffectDescription.EffectForms.TryAdd(FormProjectileBlocker);
            StinkingCloud.EffectDescription.EffectForms.TryAdd(FormProjectileBlocker);
            SleetStorm.EffectDescription.EffectForms.TryAdd(FormProjectileBlocker);
            CloudKill.EffectDescription.EffectForms.TryAdd(FormProjectileBlocker);
            IncendiaryCloud.EffectDescription.EffectForms.TryAdd(FormProjectileBlocker);
        }
        else
        {
            FogCloud.EffectDescription.EffectForms.Remove(FormProjectileBlocker);
            SpellsContext.PetalStorm.EffectDescription.EffectForms.Remove(FormProjectileBlocker);
            StinkingCloud.EffectDescription.EffectForms.Remove(FormProjectileBlocker);
            SleetStorm.EffectDescription.EffectForms.Remove(FormProjectileBlocker);
            CloudKill.EffectDescription.EffectForms.Remove(FormProjectileBlocker);
            IncendiaryCloud.EffectDescription.EffectForms.Remove(FormProjectileBlocker);
        }
    }

    internal static void SwitchMagicalDarknessOnObscurementRules()
    {
        if (Main.Settings.OfficialObscurementRulesMagicalDarknessAsProjectileBlocker)
        {
            PowerDefilerDarkness.EffectDescription.EffectForms.TryAdd(FormProjectileBlocker);
            Darkness.EffectDescription.EffectForms.TryAdd(FormProjectileBlocker);
        }
        else
        {
            PowerDefilerDarkness.EffectDescription.EffectForms.Remove(FormProjectileBlocker);
            Darkness.EffectDescription.EffectForms.Remove(FormProjectileBlocker);
        }
    }

    internal static void SwitchMonstersOnObscurementRules()
    {
        foreach (var monster in DatabaseRepository.GetDatabase<MonsterDefinition>())
        {
            var name = monster.Name;

            if (Main.Settings.OfficialObscurementRulesTweakMonsters)
            {
                if (MonstersThatShouldHaveDarkvision.Contains(name))
                {
                    monster.Features.TryAdd(SenseDarkvision);
                }

                if (MonstersThatShouldHaveTrueSight.Contains(name))
                {
                    monster.Features.TryAdd(SenseTruesight16);
                }

                if (MonstersThatShouldHaveBlindSight.Contains(name))
                {
                    monster.Features.TryAdd(SenseBlindSight16);
                }

                if (MonstersThatShouldNotHaveTremorSense.Contains(name))
                {
                    monster.Features.Remove(SenseTremorsense16);
                }
            }
            else
            {
                if (MonstersThatShouldHaveDarkvision.Contains(name))
                {
                    monster.Features.Remove(SenseDarkvision);
                }

                if (MonstersThatShouldHaveTrueSight.Contains(name))
                {
                    monster.Features.Remove(SenseTruesight16);
                }

                if (MonstersThatShouldHaveBlindSight.Contains(name))
                {
                    monster.Features.Remove(SenseBlindSight16);
                }

                if (MonstersThatShouldNotHaveTremorSense.Contains(name))
                {
                    monster.Features.TryAdd(SenseTremorsense16);
                }
            }
        }
    }

    #endregion
}
