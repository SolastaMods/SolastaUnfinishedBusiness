using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Interfaces;
using TA;
using UnityEngine;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.GameExtensions.GameLocationBattleExtensions;
using Coroutine = TA.Coroutine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterActionMagicEffectPatcher
{
    [HarmonyPatch(typeof(CharacterActionMagicEffect), nameof(CharacterActionMagicEffect.MagicEffectExecuteOnPositions))]
    [UsedImplicitly]
    public static class MagicEffectExecuteOnPositions_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            out IEnumerator __result,
            CharacterActionMagicEffect __instance,
            List<int3> positions,
            Vector3 castingPoint,
            Vector3 impactPoint,
            Vector3 impactPlanePoint)
        {
            __result = MagicEffectExecuteOnPositions(
                __instance, positions, castingPoint, impactPoint, impactPlanePoint);

            return false;
        }

        private static IEnumerator MagicEffectExecuteOnPositions(
            CharacterActionMagicEffect actionMagicEffect,
            List<int3> positions,
            Vector3 castingPoint,
            Vector3 impactPoint,
            Vector3 impactPlanePoint)
        {
            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var service = ServiceRepository.GetService<IGameLocationPositioningService>();
            var rulesetActiveEffect = actionMagicEffect.ActionParams.RulesetEffect;
            var effectDescription = rulesetActiveEffect.EffectDescription;

            //BEGIN PATCH
            var source = rulesetActiveEffect.SourceDefinition;
            var modifyTeleportEffectBehavior = source.GetFirstSubFeatureOfType<IModifyTeleportEffectBehavior>();
            //END PATCH

            if (effectDescription.InviteOptionalAlly &&
                actionMagicEffect.ActionParams.TargetCharacters != null &&
                !actionMagicEffect.ActionParams.TargetCharacters.Empty())
            {
                var occupiedPositions = new List<int3>();
                var foundPositions = new Dictionary<GameLocationCharacter, int3>();

                occupiedPositions.Add(positions[0]);

                //BEGIN PATCH
#if false
                var coroutine = new Coroutine();
                var locationPosition = actionMagicEffect.ActionParams.TargetCharacters[0].LocationPosition;

                actionMagicEffect.ActionParams.TargetCharacters[0].LocationPosition = positions[0];

                service.ComputeUnstackedPlacementPositionsForCharacter(
                    actionMagicEffect.ActionParams.TargetCharacters[0], coroutine,
                    ServiceRepository.GetService<IGameLocationPathfindingService>(), occupiedPositions, foundPositions);

                positions.Add(foundPositions.Count > 0
                    ? foundPositions[actionMagicEffect.ActionParams.TargetCharacters[0]]
                    : positions[0]);

                actionMagicEffect.ActionParams.TargetCharacters[0].LocationPosition = locationPosition;
#endif
                if (modifyTeleportEffectBehavior != null &&
                    rulesetActiveEffect.EffectDescription.HasSavingThrow)
                {
                    var attacker = actionMagicEffect.ActingCharacter;
                    var actionParams = actionMagicEffect.ActionParams;
                    var currentTargets = actionParams.TargetCharacters.ToArray();

                    for (var i = 0; i < currentTargets.Length; i++)
                    {
                        var currentTarget = currentTargets[i];
                        var actionModifier = actionParams.ActionModifiers[i];

                        if (!currentTarget.IsOppositeSide(attacker.Side))
                        {
                            continue;
                        }

                        rulesetActiveEffect.TryRollSavingThrow(
                            attacker.RulesetCharacter,
                            attacker.Side,
                            currentTarget.RulesetActor,
                            actionModifier, actionMagicEffect.ActionParams.RulesetEffect.EffectDescription.EffectForms,
                            true,
                            out var saveOutcome, out _);

                        if (saveOutcome is RollOutcome.Success)
                        {
                            actionParams.TargetCharacters.Remove(currentTarget);
                        }
                    }
                }

                foreach (var target in actionMagicEffect.ActionParams.TargetCharacters)
                {
                    var coroutine = new Coroutine();
                    var locationPosition = target.LocationPosition;

                    target.LocationPosition = positions[0];

                    service.ComputeUnstackedPlacementPositionsForCharacter(
                        target, coroutine,
                        ServiceRepository.GetService<IGameLocationPathfindingService>(), occupiedPositions,
                        foundPositions);

                    positions.Add(foundPositions.Count > 0
                        ? foundPositions.TryGetValue(target, out var position) ? position : locationPosition
                        : positions[0]);

                    target.LocationPosition = locationPosition;
                }

                if (modifyTeleportEffectBehavior is { TeleportSelf: false })
                {
                    for (var i = positions.Count - 1; i > 0; i--)
                    {
                        positions[i] = positions[i - 1];
                    }
                }

                //END PATCH
            }

            actionMagicEffect.impactPositionPoints.Clear();

            foreach (var position in positions)
            {
                actionMagicEffect.impactPositionPoints.Add(service.GetWorldPositionFromGridPosition(position));
            }

            ActionDefinitions.MagicEffectCastData magicEffectCastData1;

            if (actionMagicEffect.ShowCasting)
            {
                if (positions.Count > 0 && actionMagicEffect.ActingCharacter.LocationPosition != positions[0])
                {
                    actionMagicEffect.ActingCharacter.TurnTowards(actionMagicEffect.impactPositionPoints[0], false);

                    yield return actionMagicEffect.ActingCharacter.EventSystem.UpdateMotionsAndWaitForEvent(
                        GameLocationCharacterEventSystem.Event.RotationEnd);
                }

                var magicEffectCastData2 = new ActionDefinitions.MagicEffectCastData
                {
                    Source = rulesetActiveEffect.Name,
                    EffectDescription = effectDescription,
                    Caster = actionMagicEffect.ActingCharacter,
                    Targets = effectDescription.InviteOptionalAlly
                        ? actionMagicEffect.ActionParams.TargetCharacters
                        : null,
                    TargetIndex = 0,
                    ActionType = actionMagicEffect.ActionType,
                    ActionId = actionMagicEffect.ActionId,
                    ImpactPoint = impactPoint,
                    ComputedTargetParameter = actionMagicEffect.computedTargetParameter,
                    Subtle = rulesetActiveEffect.MetamagicOption &&
                             rulesetActiveEffect.MetamagicOption.Type == MetamagicType.SubtleSpell
                };
                var magicEffectCastData3 = magicEffectCastData2;

                actionMagicEffect.ActingCharacter.Cast(ref magicEffectCastData3);
                actionMagicEffect.needToWaitCastAnimation = true;

                if (actionMagicEffect.ShowVFX && actionService.MagicEffectPreparing != null)
                {
                    actionService.MagicEffectPreparing(ref magicEffectCastData3);
                }

                for (var index = 0; index < actionMagicEffect.ActionParams.Positions.Count; ++index)
                {
                    if (!actionMagicEffect.ShowVFX ||
                        actionService.MagicEffectPreparingOnTarget == null)
                    {
                        continue;
                    }

                    magicEffectCastData2 = new ActionDefinitions.MagicEffectCastData
                    {
                        Source = rulesetActiveEffect.Name,
                        EffectDescription = effectDescription,
                        Caster = actionMagicEffect.ActingCharacter,
                        ImpactPoint = actionMagicEffect.impactPositionPoints[index],
                        ImpactRotation = Quaternion.identity,
                        ActionType = actionMagicEffect.ActionType,
                        ActionId = actionMagicEffect.ActionId,
                        ComputedTargetParameter = actionMagicEffect.computedTargetParameter
                    };

                    var data = magicEffectCastData2;

                    actionService.MagicEffectPreparingOnTarget(ref data);
                }

                yield return actionMagicEffect.ActingCharacter.EventSystem.WaitForEvent(
                    GameLocationCharacterEventSystem.Event.MagicEffectLaunchPoint);

                if (!IsTargeted(effectDescription.TargetType) && actionMagicEffect.ShowVFX)
                {
                    var impactTime = actionMagicEffect.GetImpactTime(effectDescription, castingPoint, impactPoint);

                    magicEffectCastData1 = new ActionDefinitions.MagicEffectCastData
                    {
                        Source = rulesetActiveEffect.Name,
                        EffectDescription = effectDescription,
                        Caster = actionMagicEffect.ActingCharacter,
                        ImpactPoint = impactPoint,
                        ImpactPlanePoint = impactPlanePoint,
                        ImpactTime = impactTime,
                        ActionType = actionMagicEffect.ActionType,
                        ActionId = actionMagicEffect.ActionId,
                        ComputedTargetParameter = actionMagicEffect.computedTargetParameter
                    };

                    var data1 = magicEffectCastData1;
                    var magicEffectLaunch = actionService.MagicEffectLaunch;

                    magicEffectLaunch?.Invoke(ref data1);

                    if (impactTime.IsReallySuperior(0.0f))
                    {
                        yield return Coroutine.WaitForSeconds(impactTime);
                    }

                    magicEffectCastData1 = new ActionDefinitions.MagicEffectCastData
                    {
                        Source = rulesetActiveEffect.Name,
                        EffectDescription = effectDescription,
                        Caster = actionMagicEffect.ActingCharacter,
                        ImpactPoint = impactPoint,
                        ImpactPlanePoint = impactPlanePoint,
                        ImpactTime = impactTime,
                        ActionType = actionMagicEffect.ActionType,
                        ActionId = actionMagicEffect.ActionId,
                        ComputedTargetParameter = actionMagicEffect.computedTargetParameter
                    };

                    var data2 = magicEffectCastData1;
                    var effectCastOnZone = actionService.MagicEffectCastOnZone;

                    effectCastOnZone?.Invoke(ref data2);
                }
            }

            actionMagicEffect.ForceApplyConditionOrLightOnSelf();

            for (var index = 0; index < actionMagicEffect.ActionParams.Positions.Count; ++index)
            {
                var position = actionMagicEffect.ActionParams.Positions[index];

                if (actionMagicEffect.IsPositionAffectedBySpellImmunity(position,
                        actionMagicEffect.ActingCharacter.LocationPosition))
                {
                    continue;
                }

                if (actionMagicEffect.ShowVFX &&
                    effectDescription.TargetType != TargetType.WallLine &&
                    //BEGIN PATCH
                    modifyTeleportEffectBehavior is not { TeleportSelf: false })
                    //END PATCH
                {
                    magicEffectCastData1 = new ActionDefinitions.MagicEffectCastData
                    {
                        Source = rulesetActiveEffect.Name,
                        EffectDescription = effectDescription,
                        Caster = actionMagicEffect.ActingCharacter,
                        ImpactPoint = actionMagicEffect.impactPositionPoints[index],
                        ImpactRotation = Quaternion.identity,
                        ImpactPlanePoint = actionMagicEffect.impactPositionPoints[index],
                        ActionType = actionMagicEffect.ActionType,
                        ActionId = actionMagicEffect.ActionId,
                        IsDivertHit = false,
                        ComputedTargetParameter = actionMagicEffect.computedTargetParameter
                    };
                    var data = magicEffectCastData1;

                    actionService.MagicEffectBeforeHitTarget(ref data);
                }

                //BEGIN PATCH: changed index == 1 to index >= 1
                if (effectDescription.InviteOptionalAlly && index >= 1 &&
                    actionMagicEffect.ActionParams.TargetCharacters != null &&
                    !actionMagicEffect.ActionParams.TargetCharacters.Empty())
                {
                    actionMagicEffect.ApplyMagicEffect(position,
                        actionMagicEffect.ActionParams.TargetCharacters[index - 1]);
                }
                //BEGIN PATCH: added this IF check
                else if (modifyTeleportEffectBehavior is not { TeleportSelf: false })
                {
                    actionMagicEffect.ApplyMagicEffect(position, null);
                }

                // ReSharper disable once InvertIf
                if (actionMagicEffect.ShowVFX &&
                    effectDescription.TargetType != TargetType.WallLine)
                {
                    magicEffectCastData1 = new ActionDefinitions.MagicEffectCastData
                    {
                        Source = rulesetActiveEffect.Name,
                        EffectDescription = effectDescription,
                        Caster = actionMagicEffect.ActingCharacter,
                        ImpactPoint = actionMagicEffect.impactPositionPoints[index],
                        ImpactRotation = Quaternion.identity,
                        ImpactPlanePoint = actionMagicEffect.impactPositionPoints[index],
                        ActionType = actionMagicEffect.ActionType,
                        ActionId = actionMagicEffect.ActionId,
                        IsDivertHit = false,
                        ComputedTargetParameter = actionMagicEffect.computedTargetParameter
                    };

                    var data = magicEffectCastData1;
                    var magicEffectHitTarget = actionService.MagicEffectHitTarget;

                    magicEffectHitTarget?.Invoke(ref data);

                    // ReSharper disable once InvertIf
                    if (actionMagicEffect.ShowCasting && positions.Count > 0 &&
                        actionMagicEffect.ActingCharacter.LocationPosition != positions[0])
                    {
                        actionMagicEffect.ActingCharacter.TurnTowards(actionMagicEffect.impactPositionPoints[0]);
                        yield return actionMagicEffect.ActingCharacter.EventSystem.UpdateMotionsAndWaitForEvent(
                            GameLocationCharacterEventSystem.Event.RotationEnd);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(CharacterActionMagicEffect), nameof(CharacterActionMagicEffect.ExecuteImpl))]
    [UsedImplicitly]
    public static class ExecuteImpl_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            out IEnumerator __result,
            CharacterActionMagicEffect __instance)
        {
            __result = ExecuteImpl(__instance);

            return false;
        }

        private static IEnumerator ExecuteImpl(CharacterActionMagicEffect __instance)
        {
            var baseDefinition = __instance.GetBaseDefinition();
            var actingCharacter = __instance.ActingCharacter;
            var actionParams = __instance.ActionParams;

            if (actionParams == null)
            {
                Trace.LogException(new Exception(
                    "[TACTICAL INVISIBLE FOR PLAYERS] null ActionParams in CharacterActionMagicEffect.ExecuteImpl()."));
                yield break;
            }

            // ReSharper disable once InvocationIsSkipped
            Trace.Assert(
                actionParams.TargetCharacters.Count == actionParams.ActionModifiers.Count,
                $"Mismatch between number of targets ({actionParams.TargetCharacters.Count}) and number of action modifiers ({actionParams.ActionModifiers.Count}).");

            var rulesetEffect = actionParams.RulesetEffect;
            var effectDescription = rulesetEffect.EffectDescription;
            var targets = actionParams.TargetCharacters;
            var targetPositions = actionParams.Positions;
            var actionModifiers = actionParams.ActionModifiers;

            // BEGIN PATCH

            //PATCH: skip spell animation if this is an AttackAfterMagicEffect spell
            if (baseDefinition.HasSubFeatureOfType<AttackAfterMagicEffect>())
            {
                actionParams.SkipAnimationsAndVFX = true;
            }

            //PATCH: supports Altruistic Metamagic
            var baseEffectDescription = (rulesetEffect.SourceDefinition as IMagicEffect)?.EffectDescription;

            // add caster as first target if necessary
            if (effectDescription.InviteOptionalAlly &&
                baseEffectDescription?.TargetType == TargetType.Self &&
                targets.Count > 0 &&
                targets[0] != actingCharacter)
            {
                targets.Insert(0, actingCharacter);
                actionParams.ActionModifiers.Insert(0, actionParams.ActionModifiers[0]);
            }

            // END PATCH

            if (rulesetEffect == null)
            {
                Trace.LogException(new Exception(
                    "[TACTICAL INVISIBLE FOR PLAYERS] null RulesetEffect in CharacterActionMagicEffect.ExecuteImpl()."));
                yield break;
            }

            if (rulesetEffect.EntityImplementation is not GameLocationEffect)
            {
                Trace.LogError($"Error Context : {rulesetEffect}");
                Trace.LogException(new Exception(
                    "[TACTICAL INVISIBLE FOR PLAYERS] null GameLocationEffect in CharacterActionMagicEffect.ExecuteImpl()."));
                yield break;
            }

            // Acting character could be trying to act during a hit animation
            yield return actingCharacter.WaitForHitAnimation();

            __instance.Countered = false;
            __instance.ExecutionFailed = false;
            __instance.immuneTargets.Clear();
            __instance.hitTargets.Clear();
            __instance.showCasting = !actionParams.SkipAnimationsAndVFX;
            __instance.needToWaitCastAnimation = false;

            var battleManager =
                ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (!battleManager)
            {
                yield break;
            }

            var rulesetService = ServiceRepository.GetService<IRulesetImplementationService>();
            var targetingService = ServiceRepository.GetService<IGameLocationTargetingService>();
            var positioningService = ServiceRepository.GetService<IGameLocationPositioningService>();

            rulesetService.ClearDamageFormsByIndex();
            __instance.TargetItem = actionParams.TargetItem;
            __instance.actualEffectForms = [];

            // Calculate casting point / impact point
            var castingGridPoint = actingCharacter.LocationPosition;
            var impactGridPoint = actionParams.Positions.Count > 0
                ? actionParams.Positions[0]
                : castingGridPoint;
            var lineGridOrigin = actionParams.Positions.Count > 1
                ? actionParams.Positions[1]
                : int3.zero;

            var useFloatingImpactPoint =
                IsUsingFloatingImpactPoint(effectDescription.RangeType, effectDescription.TargetType);

            var impactPoint = !actionParams.HasMagneticTargeting && useFloatingImpactPoint
                ? actionParams.CursorHoveredPosition
                : positioningService.GetWorldPositionFromGridPosition(impactGridPoint);
            var lineOrigin = positioningService.GetWorldPositionFromGridPosition(impactGridPoint);
            var castingPoint = positioningService.GetWorldPositionFromGridPosition(castingGridPoint);
            var impactPlanePoint = positioningService.GetImpactPlanePosition(impactPoint);

            // Store the position in the active effect.
            if (actionParams.RulesetEffect.EntityImplementation is GameLocationEffect gameLocationEffect)
            {
                gameLocationEffect.Position = impactGridPoint;
                gameLocationEffect.Position2 = lineGridOrigin;
                gameLocationEffect.HasMagneticTargeting = actionParams.HasMagneticTargeting;

                // Special case for telekinesis origin (opposite target)
                if (actionParams.RulesetEffect.EffectDescription.HasFormOfType(EffectForm.EffectFormType.Motion)
                    && actionParams.RulesetEffect.EffectDescription
                        .GetFirstFormOfType(EffectForm.EffectFormType.Motion).MotionForm.Type ==
                    MotionForm.MotionType.Telekinesis
                    && actionParams.Positions.Count > 0)
                {
                    gameLocationEffect.Position = actionParams.Positions[0];
                }

                // Store the caster original position
                gameLocationEffect.SourceOriginalPosition = actingCharacter.LocationPosition;
            }

            var origin = new Vector3();
            var direction = new Vector3();
            var shapeType = effectDescription.ShapeType;

            // Spend inventory action as needed
            if (actingCharacter is { RulesetActor: RulesetCharacterHero } &&
                actionParams.RulesetEffect.OriginItem != null)
            {
                var slot = actingCharacter.RulesetCharacter.CharacterInventory
                    .FindSlotHoldingItem(actionParams.RulesetEffect.OriginItem);

                if (slot != null && !slot.SlotTypeDefinition.BodySlot &&
                    !Main.Settings.EnableUnlimitedInventoryActions) //don't spend if unlimited enabled
                {
                    actingCharacter.SpendActionType(ActionDefinitions.ActionType.FreeOnce);
                }
            }

            targetingService.ComputeTargetingParameters(
                impactPoint,
                actingCharacter,
                actingCharacter.LocationPosition,
                shapeType,
                actionParams.RulesetEffect.EffectDescription.RangeType,
                ref origin,
                ref direction);

            if (effectDescription.TargetType == TargetType.Cube &&
                effectDescription.RangeType == RangeType.Distance)
            {
                var offset = new Vector3();
                var edgeSize = actionParams.RulesetEffect.EffectDescription.TargetParameter;

                if (actionParams.HasMagneticTargeting)
                {
                    // Cube is centered on the enemy
                    if (edgeSize % 2 == 0)
                    {
                        offset = new Vector3(0.5f, 0.5f, 0.5f);
                    }
                }
                else
                {
                    // Cube is grazing the ground
                    offset = new Vector3(0, (0.5f * edgeSize) - 0.5f, 0);

                    if (edgeSize % 2 == 0)
                    {
                        offset += new Vector3(0.5f, 0, 0.5f);
                    }
                }

                impactPoint += offset;
            }

            // Compute all targets, to retrieve cover positions etc.
            var affectedCharacters = new List<GameLocationCharacter>();
            var coveredPositions = new List<int3>();

            __instance.computedTargetParameter = actionParams.RulesetEffect.ComputeTargetParameter();

            if (effectDescription.TargetType == TargetType.PerceivingWithinDistance)
            {
                targetingService.CollectPerceivingTargetsWithinDistance(
                    actingCharacter,
                    effectDescription,
                    affectedCharacters,
                    actionModifiers,
                    coveredPositions);
            }
            else
            {
                targetingService.ComputeTargetsOfAreaOfEffect(
                    origin,
                    direction,
                    lineOrigin,
                    shapeType,
                    actingCharacter.Side,
                    effectDescription,
                    __instance.computedTargetParameter,
                    actionParams.RulesetEffect.ComputeTargetParameter2(),
                    affectedCharacters,
                    actionParams.HasMagneticTargeting,
                    actingCharacter,
                    coveredPositions,
                    groundOnly: effectDescription.AffectOnlyGround);
            }

            // Check sub targets
            if (targets.Count == 1)
            {
                targetingService.ComputeAndSortSubtargets(actionParams.ActingCharacter,
                    actionParams.RulesetEffect, targets[0], __instance.subTargets);
                if (!__instance.subTargets.Empty())
                {
                    targets.AddRange(__instance.subTargets);

                    actionModifiers.AddRange(__instance.subTargets.Select(_ => new ActionModifier()));
                }
            }

            // Safety: merge lists of target, in case the client did not fill them.
            actionModifiers.AddRange(affectedCharacters
                .Where(affectedCharacter => targets.TryAdd(affectedCharacter))
                .Select(_ => new ActionModifier()));

            // BEGIN PATCH

            //PATCH: supports `IPowerOrSpellInitiatedByMe`
            var powerOrSpellInitiatedByMe = baseDefinition.GetFirstSubFeatureOfType<IPowerOrSpellInitiatedByMe>();

            if (powerOrSpellInitiatedByMe != null)
            {
                yield return powerOrSpellInitiatedByMe.OnPowerOrSpellInitiatedByMe(__instance, baseDefinition);
            }

            //PATCH: supports `IMagicEffectInitiatedByMe`
            foreach (var magicEffectInitiatedByMe in actingCharacter.RulesetCharacter
                         .GetSubFeaturesByType<IMagicEffectInitiatedByMe>())
            {
                yield return magicEffectInitiatedByMe.OnMagicEffectInitiatedByMe(
                    __instance,
                    rulesetEffect,
                    actingCharacter,
                    targets);
            }

            //PATCH: supports `IMagicEffectInitiatedByMe` on metamagic
            var hero = actingCharacter.RulesetCharacter.GetOriginalHero();

            if (hero != null)
            {
                foreach (var magicEffectInitiatedByMe in hero.TrainedMetamagicOptions
                             .SelectMany(metamagic =>
                                 metamagic.GetAllSubFeaturesOfType<IMagicEffectInitiatedByMe>()))
                {
                    yield return magicEffectInitiatedByMe.OnMagicEffectInitiatedByMe(
                        __instance,
                        rulesetEffect,
                        actingCharacter,
                        targets);
                }
            }

            // END PATCH

            __instance.SpendMagicEffectUses();

            // This is used to remove invisibility (for example) when casting a spell
            __instance.CheckInterruptionBefore();

            // Handle spell countering
            yield return __instance.WaitSpellCastAction(battleManager);

            if (__instance.Countered)
            {
                var activeEffect = actionParams.RulesetEffect;

                activeEffect.Terminate(false);
                yield break;
            }

            // Did the execution fail (Example: Slow spell can provoke spellcasting failure
            yield return __instance.CheckExecutionFailure();

            if (__instance.ExecutionFailed)
            {
                var activeEffect = actionParams.RulesetEffect;

                activeEffect.Terminate(false);
                yield break;
            }

            // Terminate previous concentrated spell if necessary
            __instance.RemoveConcentrationAsNeeded();

            yield return __instance.HandleSpecialCastingTime();

            // Unique effects have to remove their previous version now, before forms are applied and conditions could clash
            __instance.HandleEffectUniqueness();

            // Has the magic effect been cast with a higher level ?
            __instance.GetAdvancementData();

            // Is the magic effect countering something ?
            yield return __instance.CounterEffectAction(__instance);

            // Targets sub filtering
            __instance.ApplyTargetFiltering(effectDescription, targets, __instance.GetBaseDefinition());

            // Get the closest targets only: used for bard Thundering voice for example
            if (effectDescription.TargetType == TargetType.ClosestWithinDistance)
            {
                targetingService.FilterClosestTargets((Vector3Int)actingCharacter.LocationPosition, targets);
            }

            if (effectDescription.TargetType == TargetType.Position ||
                effectDescription.HasEffectProxy)
            {
                // CELL BASED SPELLS (InvokeGoblinoids, etc...)
                // Warning: this does not handle (for now) recurrent effect, so this will always be triggered even if RecurrentEffect.OnActivation == false (Delayed Blast Fireball)
                yield return __instance.MagicEffectExecuteOnPositions(
                    targetPositions, castingPoint, impactPoint, impactPlanePoint);

                // This is necessary otherwise the casting animation will show twice
                __instance.ShowCasting = false;
            }

            // Some proxy spells need to summon a proxy at a position, and apply an effect on targets (Daylight)
            if (effectDescription.TargetType != TargetType.Position ||
                effectDescription.HasEffectProxy)
            {
                if (IsTargeted(effectDescription.TargetType))
                {
                    // TARGETED SPELLS (MagicMissile, Firebolt, etc...)
                    yield return __instance.MagicEffectExecuteOnTargets(
                        targets, actionModifiers, castingPoint, impactPoint, impactPlanePoint, origin, direction);
                }
                else
                {
                    // NON-TARGETED SPELLS (Fireball, BurningHands, etc..)
                    yield return __instance.MagicEffectExecuteOnZone(
                        targets, actionModifiers, castingPoint, impactPoint, impactPlanePoint, origin, direction);
                }
            }

            foreach (var target in targets)
            {
                // Reset this flag after the application of the magic effect forms
                target.WillBePushedByMagicalEffect = false;
            }

            // Is the magic effect on going ?
            __instance.PersistantEffectAction();

            // Apply environmental damage
            var applyDamage =
                effectDescription.EffectForms.Any(effectForm =>
                    effectForm.FormType == EffectForm.EffectFormType.Damage);

            if (applyDamage &&
                __instance.ActionId != ActionDefinitions.Id.CastReaction &&
                __instance.ActionId != ActionDefinitions.Id.PowerReaction)
            {
                // Wait for targets to take damage
                foreach (var target in targets.Where(target =>
                             !__instance.immuneTargets.Contains(target) &&
                             __instance.hitTargets.Contains(target) &&
                             target.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                             !target.Prone))
                {
                    if (!__instance.isResultingActionSpendPowerWithMotionForm &&
                        !target.RulesetCharacter.IsDeadOrDying)
                    {
                        yield return target.WaitForHitAnimation();
                    }

                    __instance.hitTargets.Remove(target);
                }
            }

            for (var i = 0; i < targets.Count; i++)
            {
                var target = targets[i];

                if (!__instance.damagePerTargetIndexCache.TryGetValue(i, out var damageReceived))
                {
                    damageReceived = 0;
                }

                // Handle specific reactions after the attack has been executed
                yield return battleManager.HandleCharacterAttackFinished(
                    __instance, actingCharacter, target, null, actionParams.RulesetEffect,
                    __instance.AttackRollOutcome,
                    damageReceived);

                // BEGIN PATCH

                if (rulesetEffect.EffectDescription.RangeType is not (RangeType.MeleeHit or RangeType.RangeHit))
                {
                    continue;
                }

                //PATCH: support for Sentinel Fighting Style - allows attacks of opportunity on enemies attacking allies
                var extraAttacksOfOpportunityEvents =
                    AttacksOfOpportunity.ProcessOnCharacterAttackFinished(battleManager,
                        actingCharacter, target);

                while (extraAttacksOfOpportunityEvents.MoveNext())
                {
                    yield return extraAttacksOfOpportunityEvents.Current;
                }

                //PATCH: support for Defensive Strike Power - allows adding Charisma modifier and chain reactions
                var extraDefensiveStrikeAttackEvents =
                    DefensiveStrikeAttack.ProcessOnCharacterAttackFinished(battleManager,
                        actingCharacter, target);

                while (extraDefensiveStrikeAttackEvents.MoveNext())
                {
                    yield return extraDefensiveStrikeAttackEvents.Current;
                }

                //PATCH: support for Aura of the Guardian power - allows swapping hp on enemy attacking ally
                var extraGuardianAuraEvents =
                    GuardianAura.ProcessOnCharacterAttackHitFinished(battleManager,
                        actingCharacter, target, null, actionParams.RulesetEffect, damageReceived);

                while (extraGuardianAuraEvents.MoveNext())
                {
                    yield return extraGuardianAuraEvents.Current;
                }

                // END PATCH
            }

            // Wait for cast animation to finish
            if (__instance.needToWaitCastAnimation)
            {
                if (!__instance.isResultingActionSpendPowerWithMotionForm)
                {
                    yield return actingCharacter.EventSystem.WaitForEvent(
                        GameLocationCharacterEventSystem.Event.MagicEffectAnimationEnd);
                }
                else
                {
                    actingCharacter.EventSystem.AbsorbNextEvent(
                        GameLocationCharacterEventSystem.Event.MagicEffectAnimationEnd);
                }

                actingCharacter.CastEnd(__instance.ActionId);
                __instance.needToWaitCastAnimation = false;
            }

            // This is used for Force of Law (Law Domain), so that the condition which imposes save disadvantage to the target is removed after the spell is cast
            __instance.CheckInterruptionAfter();

            // Replace character
            if (castingPoint != impactPoint)
            {
                var actingProxy = actingCharacter.RulesetCharacter as RulesetCharacterEffectProxy;

                // HACK : As the reaction could run in parallel of another movement action, we do not permit rotation for now.
                if ((__instance.ActionId == ActionDefinitions.Id.CastReadied ||
                     __instance.ActionId == ActionDefinitions.Id.AttackReadied ||
                     __instance.ActionType != ActionDefinitions.ActionType.Reaction) &&
                    (actingProxy == null || actingProxy.EffectProxyDefinition.CanMove))
                {
                    actingCharacter.TurnTowards(impactPoint);
                    yield return actingCharacter.EventSystem.UpdateMotionsAndWaitForEvent(
                        GameLocationCharacterEventSystem.Event.RotationEnd);
                }
            }

            var rangeAttack = effectDescription.RangeType != RangeType.MeleeHit &&
                              effectDescription.RangeType != RangeType.Touch;

            foreach (var target in targets.Where(target =>
                         target != actingCharacter &&
                         !rangeAttack &&
                         !target.Prone &&
                         target.RulesetCharacter is null or { IsDeadOrDyingOrUnconscious: false } &&
                         !target.MoveStepInProgress &&
                         !target.IsCharging &&
                         (target.PerceivedAllies.Contains(actingCharacter) ||
                          target.PerceivedFoes.Contains(actingCharacter))))
            {
                target.TurnTowards(actingCharacter);

                yield return target.EventSystem.UpdateMotionsAndWaitForEvent(
                    GameLocationCharacterEventSystem.Event.RotationEnd);
            }

            // Concentrate on the new spell
            __instance.StartConcentrationAsNeeded();

            rulesetService.ClearDamageFormsByIndex();

            if (__instance.isPostSpecialMove)
            {
                // Check for end of move triggers (readied attacks for instance)
                if (battleManager.IsBattleInProgress)
                {
                    yield return battleManager.HandleCharacterMoveEnd(actingCharacter);
                }
            }

            // BEGIN PATCH

            //PATCH: support for `IMagicEffectFinishedOnMe`
            foreach (var target in targets)
            {
                var rulesetTarget = target.RulesetCharacter;

                if (rulesetTarget is not { IsDeadOrDyingOrUnconscious: false })
                {
                    continue;
                }

                foreach (var magicEffectFinishedOnMe in rulesetTarget
                             .GetSubFeaturesByType<IMagicEffectFinishedOnMe>())
                {
                    yield return magicEffectFinishedOnMe.OnMagicEffectFinishedOnMe(
                        __instance, actingCharacter, target, targets);
                }
            }

            //PATCH: supports `IPowerOrSpellFinishedByMe`
            var powerOrSpellFinishedByMe = baseDefinition.GetFirstSubFeatureOfType<IPowerOrSpellFinishedByMe>();

            if (powerOrSpellFinishedByMe != null)
            {
                yield return powerOrSpellFinishedByMe.OnPowerOrSpellFinishedByMe(__instance, baseDefinition);
            }

            //PATCH: support for `IMagicEffectFinishedByMe`
            foreach (var magicEffectFinishedByMe in actingCharacter.RulesetCharacter
                         .GetSubFeaturesByType<IMagicEffectFinishedByMe>())
            {
                yield return
                    magicEffectFinishedByMe.OnMagicEffectFinishedByMe(__instance, actingCharacter, targets);
            }

            //PATCH: supports `IMagicEffectFinishedByMe` on metamagic
            if (hero != null)
            {
                foreach (var magicEffectFinishedByMe in hero.TrainedMetamagicOptions
                             .SelectMany(metamagic =>
                                 metamagic.GetAllSubFeaturesOfType<IMagicEffectFinishedByMe>()))
                {
                    yield return
                        magicEffectFinishedByMe.OnMagicEffectFinishedByMe(__instance, actingCharacter, targets);
                }
            }

            //PATCH: support for `IMagicEffectFinishedByMeOrAlly`
            var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var contenders =
                locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters);

            foreach (var ally in contenders
                         .Where(x => x.Side == actingCharacter.Side
                                     && x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
                         .ToArray())
            {
                foreach (var magicEffectFinishedByMeOrAlly in ally.RulesetCharacter
                             .GetSubFeaturesByType<IMagicEffectFinishedByMeOrAlly>())
                {
                    yield return magicEffectFinishedByMeOrAlly
                        .OnMagicEffectFinishedByMeOrAlly(
                            battleManager, __instance, actingCharacter, ally, targets);
                }
            }

            //PATCH: supports `AttackAfterMagicEffect`
            var attackAfterMagicEffect = baseDefinition.GetFirstSubFeatureOfType<AttackAfterMagicEffect>();

            if (attackAfterMagicEffect != null)
            {
                foreach (var actionParam in attackAfterMagicEffect.PerformAttackAfterUse(__instance))
                {
                    // don't use ExecuteAction here to ensure compatibility with War Caster feat
                    if (__instance.ActionType == ActionDefinitions.ActionType.Reaction)
                    {
                        var actionAttack = new CharacterActionAttack(actionParam);

                        yield return CharacterActionAttackPatcher.ExecuteImpl_Patch.ExecuteImpl(actionAttack);
                    }
                    else
                    {
                        ServiceRepository.GetService<IGameLocationActionService>()
                            .ExecuteAction(actionParam, null, true);
                    }
                }
            }

            // END PATCH

            yield return __instance.HandlePostExecution();
            yield return battleManager.HandleCharacterAttackOrMagicEffectFinishedLate(__instance, actingCharacter);
        }
    }

    [HarmonyPatch(typeof(CharacterActionMagicEffect), nameof(CharacterActionMagicEffect.ExecuteMagicAttack))]
    [UsedImplicitly]
    public static class ExecuteMagicAttack_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            out IEnumerator __result,
            CharacterActionMagicEffect __instance,
            RulesetEffect activeEffect,
            GameLocationCharacter target,
            ActionModifier attackModifier,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool checkMagicalAttackDamage)
        {
            __result = ExecuteMagicAttack(__instance,
                activeEffect, target, attackModifier, actualEffectForms, firstTarget, checkMagicalAttackDamage);

            return false;
        }

        private static IEnumerator ExecuteMagicAttack(
            CharacterActionMagicEffect __instance,
            RulesetEffect rulesetEffect,
            GameLocationCharacter target,
            ActionModifier actionModifier,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool checkMagicalAttackDamage)
        {
            var battleManager =
                ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (!battleManager)
            {
                yield break;
            }

            var actingCharacter = __instance.ActingCharacter;
            var rulesetTarget = target.RulesetActor;
            var effectDescription = rulesetEffect.EffectDescription;

            __instance.AttackRollOutcome = RollOutcome.Success;

            var needToRollDie = effectDescription.NeedsToRollDie();
            var hasSavingThrowAnimation = !effectDescription.HideSavingThrowAnimation && !needToRollDie;

            // BEGIN PATCH

            //PATCH: supports `IMagicalAttackInitiatedOnMe`
            foreach (var magicEffectInitiatedOnMe in rulesetTarget
                         .GetSubFeaturesByType<IMagicEffectAttackInitiatedOnMe>())
            {
                yield return magicEffectInitiatedOnMe.OnMagicEffectAttackInitiatedOnMe(
                    __instance,
                    rulesetEffect,
                    actingCharacter,
                    target,
                    actionModifier,
                    firstTarget,
                    checkMagicalAttackDamage);
            }

            // END PATCH

            if (needToRollDie)
            {
                // Roll dice + handle target reaction
                __instance.AttackRoll = actingCharacter.RulesetCharacter.RollMagicAttack(
                    rulesetEffect,
                    rulesetTarget,
                    rulesetEffect.GetEffectSource(),
                    actionModifier.AttacktoHitTrends,
                    actionModifier.AttackAdvantageTrends,
                    false,
                    actionModifier.AttackRollModifier,
                    out var outcome,
                    out var successDelta,
                    -1,
                    true);
                __instance.AttackRollOutcome = outcome;
                __instance.AttackSuccessDelta = successDelta;

                // If this roll is failed (not critically), can we use a bardic inspiration to change the outcome?
                if (__instance.AttackRollOutcome == RollOutcome.Failure)
                {
                    yield return battleManager.HandleBardicInspirationForAttack(
                        __instance, actingCharacter, target, actionModifier);

                    // BEGIN PATCH

                    //BUGFIX: vanilla doesn't add the bardic die roll to attack success delta
                    if (__instance.AttackRollOutcome == RollOutcome.Success &&
                        __instance.BardicDieRoll > 0)
                    {
                        __instance.AttackSuccessDelta += __instance.BardicDieRoll;
                    }

                    // END PATCH
                }

                __instance.isResultingActionSpendPowerWithMotionForm = false;

                //PATCH: support for `ITryAlterOutcomeAttack`
                foreach (var tryAlterOutcomeAttack in TryAlterOutcomeAttack.HandlerNegativePriority(
                             battleManager, __instance, actingCharacter, target, actionModifier, null,
                             rulesetEffect))
                {
                    yield return tryAlterOutcomeAttack;
                }
                //END PATCH

                // Is this a success?
                if (__instance.AttackRollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
                {
                    // No need to test this even if the hit was automatic (natural 20). Warning: Critical Success can occur without rolling a 20 (Champion fighter crits at 19)
                    if (__instance.AttackRoll != DiceMaxValue[(int)DieType.D20])
                    {
                        // Can the target do anything to change the outcome of the hit?
                        yield return battleManager.HandleCharacterAttackHitPossible(
                            actingCharacter,
                            target,
                            null,
                            rulesetEffect,
                            actionModifier,
                            __instance.AttackRoll,
                            __instance.AttackSuccessDelta,
                            effectDescription.RangeType == RangeType.RangeHit);
                    }

                    //PATCH: support for `ITryAlterOutcomeAttack`
                    foreach (var tryAlterOutcomeAttack in TryAlterOutcomeAttack.HandlerNonNegativePriority(
                                 battleManager, __instance, actingCharacter, target, actionModifier, null,
                                 rulesetEffect))
                    {
                        yield return tryAlterOutcomeAttack;
                    }
                    //END PATCH

                    // Execute the final step of the attack
                    actingCharacter.RulesetCharacter.RollMagicAttack(
                        rulesetEffect,
                        rulesetTarget,
                        rulesetEffect.GetEffectSource(),
                        actionModifier.AttacktoHitTrends,
                        actionModifier.AttackAdvantageTrends,
                        false,
                        actionModifier.AttackRollModifier,
                        out outcome,
                        out successDelta,
                        __instance.AttackRoll,
                        false);
                    __instance.AttackRollOutcome = outcome;
                    __instance.AttackSuccessDelta = successDelta;

                    // Is this still a success?
                    if (__instance.AttackRollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
                    {
                        // Handle special cases, if there is at least one damage
                        if (checkMagicalAttackDamage && __instance.HasOneDamageForm(actualEffectForms))
                        {
                            yield return battleManager.HandleCharacterMagicalAttackHitConfirmed(
                                __instance,
                                actingCharacter,
                                target,
                                actionModifier,
                                rulesetEffect,
                                actualEffectForms,
                                firstTarget,
                                __instance.AttackRollOutcome == RollOutcome.CriticalSuccess);
                        }
                    }
                }
                else
                {
                    actingCharacter.RulesetCharacter.RollMagicAttack(
                        rulesetEffect,
                        rulesetTarget,
                        rulesetEffect.GetEffectSource(),
                        actionModifier.AttacktoHitTrends,
                        actionModifier.AttackAdvantageTrends,
                        false, actionModifier.AttackRollModifier,
                        out outcome,
                        out successDelta,
                        __instance.AttackRoll,
                        false);
                    __instance.AttackRollOutcome = outcome;
                    __instance.AttackSuccessDelta = successDelta;
                }

                // Possible condition interruption, after the attack is done. Example: if a creature attacks with the Rousing Shout condition
                actingCharacter.RulesetCharacter.ProcessConditionsMatchingInterruption(
                    ConditionInterruption.Attacks);
            }
            else
            {
                // Handle special cases, if there is at least one damage
                if (checkMagicalAttackDamage && __instance.HasOneDamageForm(actualEffectForms))
                {
                    yield return battleManager.HandleCharacterMagicalAttackHitConfirmed(
                        __instance,
                        actingCharacter,
                        target,
                        actionModifier,
                        rulesetEffect,
                        actualEffectForms,
                        firstTarget,
                        false);
                }
            }

            // No need to roll the saving throw if the attack missed
            if (!needToRollDie ||
                __instance.AttackRollOutcome == RollOutcome.Success ||
                __instance.AttackRollOutcome == RollOutcome.CriticalSuccess ||
                rulesetEffect.EffectDescription.HalfDamageOnAMiss)
            {
                // Roll the saving throw, if it is the right time
                if (rulesetEffect.EffectDescription.RecurrentEffect == RecurrentEffect.No ||
                    (rulesetEffect.EffectDescription.RecurrentEffect & RecurrentEffect.OnActivation) != 0)
                {
                    // Saving throw?
                    var hasBorrowedLuck = rulesetTarget.HasConditionOfTypeOrSubType(ConditionBorrowedLuck);

                    __instance.RolledSaveThrow = rulesetEffect.TryRollSavingThrow(
                        actingCharacter.RulesetCharacter,
                        actingCharacter.Side,
                        rulesetTarget,
                        actionModifier,
                        actualEffectForms,
                        hasSavingThrowAnimation,
                        out var saveOutcome,
                        out var saveOutcomeDelta);

                    __instance.SaveOutcome = saveOutcome;
                    __instance.SaveOutcomeDelta = saveOutcomeDelta;

                    if (__instance.RolledSaveThrow)
                    {
                        var savingThrowData = new SavingThrowData
                        {
                            SaveActionModifier = actionModifier,
                            SaveOutcome = __instance.SaveOutcome,
                            SaveOutcomeDelta = __instance.SaveOutcomeDelta,
                            SaveDC = RulesetActorExtensions.SaveDC,
                            SaveBonusAndRollModifier = RulesetActorExtensions.SaveBonusAndRollModifier,
                            SavingThrowAbility = RulesetActorExtensions.SavingThrowAbility,
                            SourceDefinition = null,
                            EffectDescription = rulesetEffect.EffectDescription,
                            Title = __instance.FormatTitle(),
                            Action = __instance
                        };

                        yield return TryAlterOutcomeSavingThrow.Handler(
                            battleManager,
                            actingCharacter,
                            target,
                            savingThrowData,
                            hasBorrowedLuck,
                            rulesetEffect.EffectDescription);
                    }
                }
            }

            if (rulesetEffect.EffectDescription.RangeType is RangeType.MeleeHit or RangeType.RangeHit)
            {
                ProcessExtraAfterAttackConditionsMatchingInterruption(actingCharacter, rulesetTarget);
            }

            //PATCH: allows ITryAlterOutcomeAttributeCheck to interact with context checks
            //BEGIN - ORIGINAL CODE
#if false
            if (!__instance.RolledSaveThrow && rulesetEffect.EffectDescription.HasShoveRoll)
            {
                __instance.successfulShove =
                    CharacterActionShove.ResolveRolls(actingCharacter, target, ActionDefinitions.Id.Shove);
            }
#endif
            //END - ORIGINAL CODE
            if (__instance.RolledSaveThrow || !rulesetEffect.EffectDescription.HasShoveRoll)
            {
                yield break;
            }

            var abilityCheckData =
                new AbilityCheckData { AbilityCheckActionModifier = new ActionModifier(), Action = __instance };
            var opponentAbilityCheckData =
                new AbilityCheckData { AbilityCheckActionModifier = new ActionModifier(), Action = __instance };

            yield return TryAlterOutcomeAttributeCheck.ResolveRolls(
                actingCharacter, target, ActionDefinitions.Id.Shove, abilityCheckData, opponentAbilityCheckData);

            __instance.successfulShove =
                abilityCheckData.AbilityCheckRollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess;
        }
    }

    [HarmonyPatch(typeof(CharacterActionMagicEffect), nameof(CharacterActionMagicEffect.ApplyForms))]
    [HarmonyPatch([
        typeof(GameLocationCharacter), // caster
        typeof(RulesetEffect), // activeEffect
        typeof(int), // addDice,
        typeof(int), // addHP,
        typeof(int), // addTempHP,
        typeof(int), // effectLevel,
        typeof(GameLocationCharacter), // target,
        typeof(ActionModifier), // actionModifier,
        typeof(RollOutcome), // outcome,
        typeof(bool), // criticalSuccess,
        typeof(bool), // rolledSaveThrow,
        typeof(RollOutcome), // saveOutcome,
        typeof(int), // saveOutcomeDelta,
        typeof(int), // targetIndex,
        typeof(int), // totalTargetsNumber,
        typeof(RulesetItem), // targetITem,
        typeof(EffectSourceType), // sourceType,
        typeof(int), // ref damageReceive
        typeof(bool), //out damageAbsorbedByTemporaryHitPoints
        typeof(bool) //out terminateEffectOnTarget
    ], [
        ArgumentType.Normal, // caster
        ArgumentType.Normal, // activeEffect
        ArgumentType.Normal, // addDice,
        ArgumentType.Normal, // addHP,
        ArgumentType.Normal, // addTempHP,
        ArgumentType.Normal, // effectLevel,
        ArgumentType.Normal, // target,
        ArgumentType.Normal, // actionModifier,
        ArgumentType.Normal, // outcome,
        ArgumentType.Normal, // criticalSuccess,
        ArgumentType.Normal, // rolledSaveThrow,
        ArgumentType.Normal, // saveOutcome,
        ArgumentType.Normal, // saveOutcomeDelta,
        ArgumentType.Normal, // targetIndex,
        ArgumentType.Normal, // totalTargetsNumber,
        ArgumentType.Normal, // targetITem,
        ArgumentType.Normal, // sourceType,
        ArgumentType.Ref, //ref damageReceive
        ArgumentType.Out, //out damageAbsorbedByTemporaryHitPoints
        ArgumentType.Out //out terminateEffectOnTarget
    ])]
    [UsedImplicitly]
    public static class ApplyForms_Patch
    {
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: support for `PushesFromEffectPoint`
            // allows push/grab motion effects to work relative to casting point, instead of caster's position
            // used for Grenadier's force grenades
            // sets position of the formsParams to the first position from ActionParams, when applicable
            var method =
                typeof(ForcePushOrDragFromEffectPoint).GetMethod(
                    nameof(ForcePushOrDragFromEffectPoint.SetPositionAndApplyForms),
                    BindingFlags.Static | BindingFlags.NonPublic);

            return instructions.ReplaceCall(
                "ApplyEffectForms",
                -1, "CharacterActionMagicEffect.ApplyForms",
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, method));
        }
    }

    [HarmonyPatch(typeof(CharacterActionMagicEffect),
        nameof(CharacterActionMagicEffect.ForceApplyConditionOrLightOnSelf))]
    [UsedImplicitly]
    public static class ForceApplyConditionOnSelf_Patch
    {
        [UsedImplicitly]
        public static bool Prefix([NotNull] CharacterActionMagicEffect __instance)
        {
            //PATCH: compute effect level of forced on self conditions
            //used for Spirit Shroud to grant more damage with extra spell slots
            var actionParams = __instance.ActionParams;
            var effectDescription = actionParams.RulesetEffect.EffectDescription;

            if (!effectDescription.HasForceSelfCondition ||
                effectDescription.EffectAdvancement.EffectIncrementMethod == EffectIncrementMethod.None)
            {
                return true;
            }

            var implementationService = ServiceRepository.GetService<IRulesetImplementationService>();

            var formsParams = new RulesetImplementationDefinitions.ApplyFormsParams();
            var effectLevel = __instance.ActionParams.RulesetEffect?.EffectLevel ?? 0;
            var effectSourceType = __instance is CharacterActionCastSpell
                ? EffectSourceType.Spell
                : EffectSourceType.Power;
            var character = __instance.ActingCharacter.RulesetCharacter;

            formsParams.FillSourceAndTarget(character, character);
            formsParams.FillFromActiveEffect(actionParams.RulesetEffect);
            formsParams.FillSpecialParameters(
                false, 0, 0, 0, effectLevel, null, RollOutcome.Success, 0, false, 0, 1, null);
            formsParams.effectSourceType = effectSourceType;

            if (effectDescription.RangeType is RangeType.MeleeHit or RangeType.RangeHit)
            {
                formsParams.attackOutcome = RollOutcome.Success;
            }

            implementationService.ApplyEffectForms(
                effectDescription.EffectForms,
                formsParams,
                null,
                out _,
                forceSelfConditionOrLightOnly: true,
                effectApplication: effectDescription.EffectApplication,
                filters: effectDescription.EffectFormFilters,
                terminateEffectOnTarget: out _);

            return false;
        }
    }
}
