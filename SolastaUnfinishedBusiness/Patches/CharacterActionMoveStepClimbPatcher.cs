using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Interfaces;
using UnityEngine;
using Coroutine = TA.Coroutine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterActionMoveStepClimbPatcher
{
    //PATCH: allow check reactions on climb checks regardless of success / failure
    [HarmonyPatch(typeof(CharacterActionMoveStepClimb), nameof(CharacterActionMoveStepClimb.ExecuteImpl))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ExecuteImpl_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(ref IEnumerator __result, CharacterActionMoveStepClimb __instance)
        {
            __result = Process(__instance);

            return false;
        }

        private static IEnumerator Process(CharacterActionMoveStepClimb action)
        {
            var actingCharacter = action.ActingCharacter;
            var actionModifier = action.ActionParams.ActionModifiers[0];
            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();
            var positioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
            var actionService = ServiceRepository.GetService<IGameLocationActionService>();

            yield return CharacterActionStandUp.StandUp(actingCharacter, true);

            var moveCost = 0;

            if (action.ActionId == ActionDefinitions.Id.TacticalMove)
            {
                moveCost += action.MovePath.Sum(pathStep => pathStep.moveCost);
            }

            if (moveCost == 0 && action.dc > 0)
            {
                moveCost = CharacterActionMoveStepClimb.ComputeClimbingCost(
                    actingCharacter, action.startPosition, action.landingPosition);
            }

            yield return action.WaitForCanMove(action.landingPosition, actingCharacter.Orientation, moveCost);

            if (action.CanMove)
            {
                if (actingCharacter.LocationPosition == action.startPosition)
                {
                    var fastClimber = false;
                    var flag = actingCharacter.CanMoveInSituation(RulesetCharacter.MotionRange.ExpertClimber);

                    // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                    foreach (var movementModifier in action.ActionParams.ActingCharacter.RulesetCharacter
                                 .GetMovementModifiers())
                    {
                        if ((movementModifier as IMovementAffinityProvider).FastClimber)
                        {
                            fastClimber = true;
                        }
                    }

                    var num1 = action.dc > 0 ? 1 : 0;
                    var failed = false;
                    var criticalFail = false;
                    var offset = action.landingPosition - action.startPosition;
                    var num2 = Mathf.Abs(offset.y);

                    if (num1 != 0 &&
                        !flag &&
                        num2 > 1 &&
                        !action.easyClimb &&
                        !actingCharacter.RulesetCharacter.MoveModes.ContainsKey(1))
                    {
                        const RuleDefinitions.AdvantageType BASE_AFFINITY = RuleDefinitions.AdvantageType.None;

                        var abilityCheckRoll = actingCharacter.RollAbilityCheck(
                            AttributeDefinitions.Strength,
                            SkillDefinitions.Athletics,
                            action.dc,
                            BASE_AFFINITY,
                            actionModifier,
                            false,
                            -1,
                            out var outcome,
                            out var successDelta,
                            true);

                        var abilityCheckData = new AbilityCheckData
                        {
                            AbilityCheckRoll = abilityCheckRoll,
                            AbilityCheckRollOutcome = outcome,
                            AbilityCheckSuccessDelta = successDelta,
                            AbilityCheckActionModifier = actionModifier
                        };

                        var battleManager = ServiceRepository.GetService<IGameLocationBattleService>();

                        yield return battleManager
                            .HandleFailedAbilityCheck(action, actingCharacter, actionModifier);

                        action.AbilityCheckRoll = abilityCheckData.AbilityCheckRoll;
                        action.AbilityCheckRollOutcome = abilityCheckData.AbilityCheckRollOutcome;
                        action.AbilityCheckSuccessDelta = abilityCheckData.AbilityCheckSuccessDelta;

                        if (action.AbilityCheckRollOutcome == RuleDefinitions.RollOutcome.Failure)
                        {
                            failed = true;
                            criticalFail = outcome == RuleDefinitions.RollOutcome.CriticalFailure;
                        }
                    }

                    var climbingDirection = action.landingPosition.y > action.startPosition.y ? 1f : -1f;
                    var failAltitude = 0;
                    var orientation = offset.x != 0 || offset.z != 0
                        ? climbingDirection <= 0.0
                            ? positioningService.GetLocationOrientationFromTo(
                                action.landingPosition, action.startPosition)
                            : positioningService.GetLocationOrientationFromTo(
                                action.startPosition, action.landingPosition)
                        : actingCharacter.Orientation;
                    var canMove = true;

                    actingCharacter.TurnTowards(orientation);

                    yield return actingCharacter.EventSystem.UpdateMotionsAndWaitForEvent(
                        GameLocationCharacterEventSystem.Event.RotationEnd);

                    if (action.ActionId == ActionDefinitions.Id.TacticalMove)
                    {
                        yield return battleService.HandleCharacterMoveStart(
                            actingCharacter, action.landingPosition);
                    }

                    if (!actingCharacter.CanContinueMoving(action.ActionId))
                    {
                        action.MovePath.Clear();
                        action.Abort(CharacterAction.InterruptionType.Failed);
                    }
                    else
                    {
                        if (failed)
                        {
                            var num3 = climbingDirection > 0.0
                                ? action.landingPosition.y - 1
                                : action.startPosition.y - 1;
                            var min = climbingDirection > 0.0
                                ? action.startPosition.y + 1
                                : action.landingPosition.y + 1;
                            failAltitude = min != num3
                                ? criticalFail ? num3 : RandomExtensions.RangeInclusive(min, num3 - 1, true)
                                : num3;
                        }

                        var climbPosition = climbingDirection <= 0.0 ? action.landingPosition : action.startPosition;

                        actingCharacter.Climbing = true;

                        if (action.abortASAP && battleService.IsBattleInProgress)
                        {
                            action.abortASAP = false;
                        }

                        var y = action.startPosition.y + (int)climbingDirection;
                        while (Math.Abs(y - (action.landingPosition.y + (double)climbingDirection)) > 0 &&
                               !action.abortASAP)
                        {
                            var time = 0.0f;

                            climbPosition.y = y;

                            for (canMove = positioningService.CanPlaceCharacter(
                                     actingCharacter, climbPosition, CellHelpers.PlacementMode.OnTheMove);
                                 !canMove && time < 2.0;
                                 canMove = positioningService.CanPlaceCharacter(actingCharacter, climbPosition,
                                     CellHelpers.PlacementMode.OnTheMove))
                            {
                                actingCharacter.StopMoving(orientation);

                                yield return Coroutine.WaitForSeconds(0.2f);

                                time += 0.2f;
                            }

                            if (canMove)
                            {
                                var lastMove = y == action.landingPosition.y;

                                if (lastMove)
                                {
                                    climbPosition = action.landingPosition;
                                }

                                actingCharacter.StartClimbTo(
                                    climbPosition, orientation, climbingDirection, lastMove, fastClimber);

                                yield return actingCharacter.EventSystem.UpdateMotionsAndWaitForEvent(
                                    GameLocationCharacterEventSystem.Event.MovementStepEnd);

                                if (failed && y == failAltitude)
                                {
                                    if (battleService.HasBattleStarted)
                                    {
                                        actingCharacter.UsedTacticalMoves += moveCost;
                                    }

                                    action.Abort(CharacterAction.InterruptionType.Failed);
                                    actingCharacter.Climbing = false;
                                    actingCharacter.FinishMoveTo(climbPosition, orientation);

                                    yield break;
                                }

                                actingCharacter.FinishMoveTo(climbPosition, orientation);
                                y += climbingDirection > 0.0 ? 1 : -1;

                                if (action.abortASAP && battleService.IsBattleInProgress)
                                {
                                    action.abortASAP = false;
                                }

                                if (action.abortASAP &&
                                    !actingCharacter.RulesetCharacter.IsDeadOrDyingOrUnconscious &&
                                    GameLocationCharacter.IsValidCharacter(actingCharacter))
                                {
                                    var wait = true;
                                    var timeout = 1f;
                                    while (wait)
                                    {
                                        if (actionService.CanMovementQueuedActionProceed(actingCharacter,
                                                PathfindingNode.InformationFlag.Climbing, out var result))
                                        {
                                            wait = false;

                                            if (result)
                                            {
                                                continue;
                                            }

                                            actionService.CancelMovementWarmUp(actingCharacter);
                                            action.abortASAP = false;
                                        }
                                        else
                                        {
                                            yield return Coroutine.WaitForSeconds(0.1f);
                                            timeout -= 0.1f;

                                            if (timeout > 0)
                                            {
                                                continue;
                                            }

                                            actionService.CancelMovementWarmUp(actingCharacter);
                                            wait = false;
                                            action.abortASAP = false;
                                        }
                                    }
                                }

                                if (action.abortASAP && battleService.IsBattleInProgress)
                                {
                                    action.abortASAP = false;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (canMove && !failed && action.landingPosition == actingCharacter.LocationPosition)
                        {
                            action.abortASAP = false;

                            if (battleService.HasBattleStarted)
                            {
                                actingCharacter.UsedTacticalMoves += moveCost;
                            }

                            actingCharacter.SignalClimbFinished(
                                action.startPosition, action.landingPosition, action.easyClimb);
                            actingCharacter.FinishMoveTo(action.landingPosition, orientation);
                        }

                        if (action.ActionId == ActionDefinitions.Id.TacticalMove)
                        {
                            yield return battleService.HandleCharacterMoveEnd(actingCharacter);
                        }
                    }
                }
            }
            else
            {
                action.MovePath.Clear();
                action.Abort(CharacterAction.InterruptionType.Failed);
            }

            if (!action.abortASAP)
            {
                actingCharacter.Climbing = false;
                actingCharacter.CheckCharacterFooting(false);
            }

            action.CheckAndProcessMovementFinished();
        }
    }
}
