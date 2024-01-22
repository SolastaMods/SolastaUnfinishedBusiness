using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using TA;
using UnityEngine;
using static LocationDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionCombatAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPerceptionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Models;

internal static class LightingAndObscurementContext
{
    internal static readonly ConditionDefinition ConditionBlindedByDarkness = ConditionDefinitionBuilder
        .Create(ConditionBlinded, "ConditionBlindedByDarkness")
        .SetOrUpdateGuiPresentation(Category.Condition)
        .SetParentCondition(ConditionBlinded)
        .SetFeatures()
        .AddToDB();

    private static readonly ConditionDefinition ConditionLightlyObscured = ConditionDefinitionBuilder
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

    private static readonly EffectForm FormBlinded = EffectFormBuilder.ConditionForm(ConditionBlinded);

    private static readonly EffectForm FormLightlyObscured = EffectFormBuilder.ConditionForm(ConditionLightlyObscured);

    private static readonly EffectForm FormProjectileBlocker = EffectFormBuilder
        .Create()
        .SetTopologyForm(TopologyForm.Type.ProjectileBlocker, true)
        .Build();

    internal static void SwitchOfficialObscurementRules()
    {
        if (Main.Settings.UseOfficialLightingObscurementAndVisionRules)
        {
            SwitchHeavilyObscuredOnObscurementRules();
            SwitchMagicalDarknessOnObscurementRules();
            SwitchMonstersOnObscurementRules();

            ConditionBlinded.Features.SetRange(
                CombatAffinityHeavilyObscured,
                CombatAffinityHeavilyObscuredSelf,
                PerceptionAffinityConditionBlinded);

            SrdAndHouseRulesContext.SwitchConditionBlindedShouldNotAllowOpportunityAttack();

            ConditionBlinded.GuiPresentation.description = ConditionBlindedByDarkness.GuiPresentation.description;

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

            Darkness.EffectDescription.EffectForms[1].ConditionForm.ConditionDefinition = ConditionBlindedByDarkness;

            // >> ConditionHeavilyObscured
            // FogCloud
            // PetalStorm

            FogCloud.EffectDescription.EffectForms[0].ConditionForm.ConditionDefinition = ConditionBlinded;

            SpellsContext.PetalStorm.EffectDescription.EffectForms[1].ConditionForm.ConditionDefinition =
                ConditionBlinded;

            // >> ConditionInStinkingCloud
            // StinkingCloud

            StinkingCloud.EffectDescription.EffectForms[0].ConditionForm.ConditionDefinition = ConditionBlinded;

            // >> ConditionSleetStorm
            // SleetStorm

            SleetStorm.EffectDescription.EffectForms[0].ConditionForm.ConditionDefinition = ConditionBlinded;

            // Cloud Kill / Incendiary Cloud need same debuff as other heavily obscured
            CloudKill.EffectDescription.EffectForms.TryAdd(FormBlinded);
            IncendiaryCloud.EffectDescription.EffectForms.TryAdd(FormBlinded);

            // Make Insect Plague lightly obscured
            InsectPlague.EffectDescription.EffectForms.Add(FormLightlyObscured);
            InsectPlague.EffectDescription.EffectForms[1].TopologyForm.changeType = TopologyForm.Type.None;

            // vanilla has this set as disadvantage so we flip it with nullified requirements
            CombatAffinityHeavilyObscured.attackOnMeAdvantage = RuleDefinitions.AdvantageType.Advantage;
            CombatAffinityHeavilyObscured.nullifiedBySenses = [];
            CombatAffinityHeavilyObscured.nullifiedBySelfSenses =
                [SenseMode.Type.Blindsight, SenseMode.Type.Tremorsense];

            CombatAffinityHeavilyObscuredSelf.nullifiedBySenses = [];
            CombatAffinityHeavilyObscuredSelf.nullifiedBySelfSenses =
                [SenseMode.Type.Blindsight, SenseMode.Type.Tremorsense];
        }
        else
        {
            SwitchHeavilyObscuredOnObscurementRules();
            SwitchMagicalDarknessOnObscurementRules();
            SwitchMonstersOnObscurementRules();

            ConditionBlinded.Features.SetRange(
                CombatAffinityBlinded,
                PerceptionAffinityConditionBlinded);

            SrdAndHouseRulesContext.SwitchConditionBlindedShouldNotAllowOpportunityAttack();

            ConditionBlinded.GuiPresentation.description = "Rules/&ConditionBlindedDescription";

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

            Darkness.EffectDescription.EffectForms[1].ConditionForm.ConditionDefinition = ConditionDarkness;

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
            CloudKill.EffectDescription.EffectForms.Remove(FormBlinded);
            IncendiaryCloud.EffectDescription.EffectForms.Remove(FormBlinded);

            // Remove lightly obscured from Insect Plague
            InsectPlague.EffectDescription.EffectForms.Remove(FormLightlyObscured);
            InsectPlague.effectDescription.EffectForms[1].TopologyForm.changeType = TopologyForm.Type.SightImpaired;

            // vanilla has this set as disadvantage so we flip it with nullified requirements
            CombatAffinityHeavilyObscured.attackOnMeAdvantage = RuleDefinitions.AdvantageType.Disadvantage;
            CombatAffinityHeavilyObscured.nullifiedBySenses =
                [SenseMode.Type.Truesight, SenseMode.Type.Blindsight];
            CombatAffinityHeavilyObscured.nullifiedBySelfSenses = [];

            CombatAffinityHeavilyObscuredSelf.nullifiedBySenses = [];
            CombatAffinityHeavilyObscuredSelf.nullifiedBySelfSenses =
                [SenseMode.Type.Truesight, SenseMode.Type.Blindsight];
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
                if (Main.Settings.MonstersThatShouldHaveDarkvision.Contains(name))
                {
                    monster.Features.TryAdd(SenseDarkvision);
                }

                if (Main.Settings.MonstersThatShouldHaveTrueSight.Contains(name))
                {
                    monster.Features.TryAdd(SenseTruesight16);
                }

                if (Main.Settings.MonstersThatShouldHaveBlindSight.Contains(name))
                {
                    monster.Features.TryAdd(SenseBlindSight16);
                }

                if (Main.Settings.MonstersThatShouldNotHaveTremorSense.Contains(name))
                {
                    monster.Features.Remove(SenseTremorsense16);
                }
            }
            else
            {
                if (Main.Settings.MonstersThatShouldHaveDarkvision.Contains(name))
                {
                    monster.Features.Remove(SenseDarkvision);
                }

                if (Main.Settings.MonstersThatShouldHaveTrueSight.Contains(name))
                {
                    monster.Features.Remove(SenseTruesight16);
                }

                if (Main.Settings.MonstersThatShouldHaveBlindSight.Contains(name))
                {
                    monster.Features.Remove(SenseBlindSight16);
                }

                if (Main.Settings.MonstersThatShouldNotHaveTremorSense.Contains(name))
                {
                    monster.Features.TryAdd(SenseTremorsense16);
                }
            }
        }
    }

    // called from GLBM.CanAttack to correctly determine ADV/DIS scenarios
    internal static void ApplyObscurementRules(BattleDefinitions.AttackEvaluationParams attackParams)
    {
        if (!Main.Settings.UseOfficialLightingObscurementAndVisionRules ||
            attackParams.effectDescription is
                { RangeType: not (RuleDefinitions.RangeType.MeleeHit or RuleDefinitions.RangeType.RangeHit) })
        {
            return;
        }

        const string TAG = "Perceive";

        var attackAdvantageTrends = attackParams.attackModifier.AttackAdvantageTrends;

        var attacker = attackParams.attacker;
        var attackerActor = attacker.RulesetActor;

        var defender = attackParams.defender;
        var defenderActor = defender.RulesetActor;

        HandleTrueSightSpecialCase();

        // nothing to do here if both contenders are already blinded
        if (attackerActor.HasConditionOfTypeOrSubType(RuleDefinitions.ConditionBlinded) &&
            defenderActor.HasConditionOfTypeOrSubType(RuleDefinitions.ConditionBlinded))
        {
            return;
        }

        var attackerIsStealthy = attackerActor.HasConditionOfType(ConditionStealthy);

        var attackerCanPerceiveDefender = attacker.CanPerceiveTarget(defender);
        var attackerHasNoLight = attacker.LightingState is LightingState.Unlit or LightingState.Darkness;

        var defenderCanPerceiveAttacker = defender.CanPerceiveTarget(attacker);
        var defenderHasNoLight = defender.LightingState is LightingState.Unlit or LightingState.Darkness;

        if (!attackerIsStealthy && !defenderCanPerceiveAttacker && (attackerCanPerceiveDefender || attackerHasNoLight))
        {
            attackAdvantageTrends.Add(PerceiveAdvantage());
        }

        if (!attackerCanPerceiveDefender && (defenderCanPerceiveAttacker || defenderHasNoLight))
        {
            attackAdvantageTrends.Add(PerceiveDisadvantage());
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

        static bool BlindedDisadvantage(RuleDefinitions.TrendInfo trendInfo)
        {
            return trendInfo.sourceName == ConditionBlinded.Name && trendInfo.value == -1;
        }

        // conditions with parent inherit their features which makes true sight quite hard to manage
        // the combat affinity won't have true sight as nullified sense so we check it here and revert
        void HandleTrueSightSpecialCase()
        {
            if (attackerActor is not RulesetCharacter attackerCharacter ||
                !attackAdvantageTrends.Any(BlindedDisadvantage) ||
                !TargetIsBlindFromDarkness(defenderActor) ||
                TargetIsBlindNotFromDarkness(defenderActor))
            {
                return;
            }

            var senseModeTrueSight =
                attackerCharacter.SenseModes.FirstOrDefault(x => x.SenseType == SenseMode.Type.Truesight);

            if (senseModeTrueSight == null || !attacker.IsWithinRange(defender, senseModeTrueSight.SenseRange))
            {
                return;
            }

            attackAdvantageTrends.RemoveAll(BlindedDisadvantage);
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
               Main.Settings.EffectsThatTargetDistantIndividualsAndDontRequireSight.Contains(magicEffect.Name) ||
               source.CanPerceiveTarget(target);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TargetIsBlindFromDarkness(RulesetActor actor)
    {
        return actor != null && actor.HasConditionOfType(ConditionBlindedByDarkness);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TargetIsBlindNotFromDarkness(RulesetActor actor)
    {
        return
            actor != null &&
            actor.AllConditions
                .Select(y => y.ConditionDefinition)
                .Any(x => (x == ConditionBlinded || x.parentCondition == ConditionBlinded) &&
                          x != ConditionBlindedByDarkness);
    }

    // improved cell perception routine that takes sight into consideration
    // most of the usages is to determine if a character can perceive a cell in teleport scenarios
    // when target not null it helps determine visibility on attacks and effects targeting scenarios
    internal static bool MyIsCellPerceivedByCharacter(
        this GameLocationVisibilityManager instance,
        int3 cellPosition,
        GameLocationCharacter sensor,
        GameLocationCharacter target = null,
        LightingState additionalBlockedLightingState = LightingState.Darkness)
    {
        // let vanilla do the heavy lift on perception
        var result = instance.IsCellPerceivedByCharacter(cellPosition, sensor);

        // try to get the lighting state from the target otherwise calculate on cell position
        // no need to compute lightning if vanilla cannot perceive cell as it'll end up exiting earlier
        var targetLightingState = result
            ? target?.LightingState ?? ComputeLightingStateOnTargetPosition(sensor, cellPosition)
            // this bright is a never used placeholder because of 2 !result in sequence checks
            // save some cycles avoid calling ComputeLightingStateOnTargetPosition too much
            : LightingState.Bright;

        // if setting is off or vanilla cannot perceive
        if (!result ||
            !Main.Settings.UseOfficialLightingObscurementAndVisionRules)
        {
            if (!result)
            {
                return false;
            }

            // Silhouette Step is the only one using additionalBlockedLightingState as it requires to block BRIGHT
            return additionalBlockedLightingState == LightingState.Darkness ||
                   targetLightingState != additionalBlockedLightingState;
        }

        var distance = DistanceCalculation.GetDistanceFromTwoPositions(sensor.LocationPosition, cellPosition);

        var targetIsNotTouchingGround =
            target != null &&
            !target.RulesetActor.IsTouchingGround();

        var targetIsBlindFromDarkness = TargetIsBlindFromDarkness(target?.RulesetActor);
        var targetIsBlindNotFromDarkness = TargetIsBlindNotFromDarkness(target?.RulesetActor);
        var targetIsBlind = targetIsBlindFromDarkness || targetIsBlindNotFromDarkness;

        // try to find any sense mode that is valid for the current lighting state and is within range
        foreach (var senseMode in sensor.RulesetCharacter.SenseModes)
        {
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (targetLightingState is LightingState.Unlit)
            {
                var senseType = senseMode.SenseType;

                if (senseType == SenseMode.Type.NormalVision ||
                    (senseType == SenseMode.Type.Darkvision && targetIsBlind) ||
                    (senseType == SenseMode.Type.SuperiorDarkvision && targetIsBlind) ||
                    (senseType == SenseMode.Type.Truesight && targetIsBlindNotFromDarkness) ||
                    (senseType == SenseMode.Type.Tremorsense && targetIsNotTouchingGround))
                {
                    continue;
                }
            }

            else if (targetLightingState is LightingState.Darkness)
            {
                var senseType = senseMode.SenseType;

                if (senseType == SenseMode.Type.NormalVision ||
                    senseType == SenseMode.Type.Darkvision ||
                    senseType == SenseMode.Type.SuperiorDarkvision ||
                    (senseType == SenseMode.Type.Truesight && targetIsBlindNotFromDarkness) ||
                    (senseType == SenseMode.Type.Tremorsense && targetIsNotTouchingGround))
                {
                    continue;
                }
            }

            if (distance <= senseMode.SenseRange)
            {
                // Silhouette Step is the only one using additionalBlockedLightingState as it requires to block BRIGHT
                return additionalBlockedLightingState == LightingState.Darkness ||
                       targetLightingState != additionalBlockedLightingState;
            }
        }

        return false;
    }

    private static LightingState ComputeLightingStateOnTargetPosition(
        GameLocationCharacter instance,
        int3 targetPosition)
    {
        var savePosition = new int3(
            instance.LocationPosition.x,
            instance.LocationPosition.y,
            instance.LocationPosition.z);

        instance.LocationPosition = targetPosition;

        var illumination = ComputeIllumination(instance);

        instance.LocationPosition = savePosition;

        return illumination;

        // this is copy-and-paste from vanilla code GameLocationVisibilityManager.ComputeIllumination
        // except for Darkness determination in patch block and some clean up for not required scenarios
        static LightingState ComputeIllumination(IIlluminable illuminable)
        {
            const LightingState UNLIT = LightingState.Unlit;

            if (!illuminable.Valid)
            {
                return UNLIT;
            }

            var visibilityManager =
                ServiceRepository.GetService<IGameLocationVisibilityService>() as GameLocationVisibilityManager;

            visibilityManager!.positionCache.Clear();
            illuminable.GetAllPositionsToCheck(visibilityManager.positionCache);

            if (visibilityManager.positionCache == null || visibilityManager.positionCache.Empty())
            {
                return UNLIT;
            }

            var gridAccessor = new GridAccessor(visibilityManager.positionCache[0]);

            if (visibilityManager.positionCache.Any(position =>
                    (gridAccessor.RuntimeFlags(position) & CellFlags.Runtime.DynamicSightImpaired) !=
                    CellFlags.Runtime.None))
            {
                return LightingState.Darkness;
            }

            var lightingState1 = LightingState.Unlit;

            foreach (var position in visibilityManager.positionCache)
            {
                gridAccessor.FetchSector(position);
                if (gridAccessor.sector == null ||
                    gridAccessor.sector.GlobalLightingState == LightingState.Unlit)
                {
                    continue;
                }

                lightingState1 = gridAccessor.sector.GlobalLightingState;

                if (lightingState1 != LightingState.Bright)
                {
                    continue;
                }

                return LightingState.Bright;
            }

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
                var magnitude = (locationPosition - illuminable.Position).magnitude;

                if (magnitude <= dimRange + (double)illuminable.DetectionRange &&
                    (!visibilityManager.charactersByLight.TryGetValue(key.RulesetLightSource,
                         out var locationCharacter3) ||
                     locationCharacter3 is { IsValidForVisibility: true }))
                {
                    visibilityManager.lightsByDistance.Add(
                        new KeyValuePair<GameLocationLightSource, float>(key, magnitude));
                }
            }

            visibilityManager.lightsByDistance.Sort(visibilityManager.lightSortMethod);

            var lightingState2 = LightingState.Unlit;

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

                    var fromGridPosition1 =
                        visibilityManager.gameLocationPositioningService.GetWorldPositionFromGridPosition(
                            key.LocationPosition);
                    var fromGridPosition2 =
                        visibilityManager.gameLocationPositioningService.GetWorldPositionFromGridPosition(int3);
                    visibilityManager.AdaptRayForVerticalityAndDiagonals(key.LocationPosition, int3,
                        ref fromGridPosition1,
                        true);
                    var flag = true;

                    if (key.RulesetLightSource.IsSpot)
                    {
                        var to = fromGridPosition2 - fromGridPosition1;

                        to.Normalize();
                        flag = Vector3.Angle(key.RulesetLightSource.SpotDirection, to)
                            .IsInferiorOrNearlyEqual(key.RulesetLightSource.SpotAngle * 0.5f);
                    }

                    if (!flag ||
                        visibilityManager.gameLocationPositioningService.RaycastGridSightBlocker(fromGridPosition1,
                            fromGridPosition2, visibilityManager.GameLocationService) ||
                        visibilityManager.gameLocationPositioningService.IsSightImpaired(key.LocationPosition, int3))
                    {
                        continue;
                    }

                    lightingState2 =
                        key.RulesetLightSource.BrightRange <= 0.0 || magnitudeSqr >
                        key.RulesetLightSource.BrightRange * key.RulesetLightSource.BrightRange
                            ? LightingState.Dim
                            : LightingState.Bright;
                    if (lightingState2 == LightingState.Bright)
                    {
                        break;
                    }
                }

                if (lightingState2 != LightingState.Unlit)
                {
                    break;
                }
            }

            return
                lightingState1 != LightingState.Dim ||
                lightingState2 != LightingState.Unlit
                    ? lightingState2
                    : LightingState.Dim;
        }
    }
}
