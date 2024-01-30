using System.Collections;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using static RuleDefinitions;
using Coroutine = TA.Coroutine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterActionAttackPatcher
{
    [HarmonyPatch(typeof(CharacterActionAttack), nameof(CharacterActionAttack.ExecuteImpl))]
    [UsedImplicitly]
    public static class ExecuteImpl_Patch
    {
        // FULL VANILLA CODE FOR REFERENCE
        private static IEnumerator ExecuteImpl(CharacterActionAttack __instance)
        {
            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();
            var gameLocationPositioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
            var worldLocationEntityFactoryService = ServiceRepository.GetService<IWorldLocationEntityFactoryService>();
            var rulesetImplementationService = ServiceRepository.GetService<IRulesetImplementationService>();
            var itemService = ServiceRepository.GetService<IGameLocationItemService>();
            var positioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
            var num = __instance.ActingCharacter.GetActionStatus(ActionDefinitions.Id.AttackMain,
                          ActionDefinitions.ActionScope.Battle,
                          optionalAttackMode: __instance.ActionParams.AttackMode) ==
                      ActionDefinitions.ActionStatus.Available
                ? 1
                : 0;
            var flag = __instance.ActingCharacter.GetActionStatus(ActionDefinitions.Id.AttackOff,
                           ActionDefinitions.ActionScope.Battle,
                           optionalAttackMode: __instance.ActionParams.AttackMode) ==
                       ActionDefinitions.ActionStatus.Available;
            if ((num == 0 && __instance.ActionType == ActionDefinitions.ActionType.Main) ||
                (!flag && __instance.ActionType == ActionDefinitions.ActionType.Bonus))
            {
                __instance.Abort(CharacterAction.InterruptionType.Invalid);

                if (!__instance.isChargeAttack)
                {
                    yield break;
                }

                __instance.ActingCharacter.IsCharging = false;
                __instance.ActingCharacter.MovingToDestination = false;
            }
            else
            {
                var target = __instance.ActionParams.TargetCharacters[0];
                var defenderWasConscious = !target.RulesetActor.IsDeadOrDyingOrUnconscious;
                var attackParams = new BattleDefinitions.AttackEvaluationParams();
                var attackModifier = new ActionModifier();
                var attackMode = __instance.ActionParams.AttackMode;

                if (!attackMode.Ranged)
                {
                    attackParams.FillForPhysicalReachAttack(__instance.ActingCharacter,
                        __instance.ActingCharacter.LocationPosition, attackMode, target, target.LocationPosition,
                        attackModifier);
                }
                else
                {
                    attackParams.FillForPhysicalRangeAttack(__instance.ActingCharacter,
                        __instance.ActingCharacter.LocationPosition, attackMode, target, target.LocationPosition,
                        attackModifier);
                }

                attackParams.opportunityAttack = __instance.ActionId == ActionDefinitions.Id.AttackOpportunity;
                attackParams.readiedAttack = __instance.ActionId == ActionDefinitions.Id.AttackReadied;

                if (!battleService.CanAttack(attackParams))
                {
                    attackModifier = __instance.ActionParams.ActionModifiers[0];
                }

                yield return battleService.HandleCharacterPhysicalAttackInitiated(
                    __instance, __instance.ActingCharacter, target, attackModifier, attackMode);

                __instance.AttackRollOutcome = RollOutcome.Failure;

                var opportunity = __instance.ActionId == ActionDefinitions.Id.AttackOpportunity;
                var rangeAttack = attackModifier.Proximity == AttackProximity.Range;

                if (!attackMode.AutomaticHit)
                {
                    __instance.AttackRoll = __instance.ActingCharacter.RulesetCharacter.RollAttackMode(
                        attackMode,
                        rangeAttack,
                        target.RulesetActor,
                        attackMode.SourceDefinition,
                        attackModifier.AttacktoHitTrends,
                        attackModifier.IgnoreAdvantage,
                        attackModifier.AttackAdvantageTrends,
                        opportunity,
                        attackModifier.AttackRollModifier,
                        out var outcome,
                        out var successDelta,
                        -1,
                        true);
                    __instance.AttackRollOutcome = outcome;
                    __instance.AttackSuccessDelta = successDelta;
                }
                else
                {
                    __instance.AttackRollOutcome = RollOutcome.Success;
                    __instance.AttackSuccessDelta = 0;
                }

                Coroutine actingCharacterTurnCoroutine = null;
                Coroutine targetCharacterTurnCoroutine = null;

                if (!__instance.skipAnimationWarmup)
                {
                    __instance.ActingCharacter.TurnTowards(target, false, false);
                    actingCharacterTurnCoroutine = Coroutine.StartCoroutine(
                        __instance.ActingCharacter.EventSystem.UpdateMotionsAndWaitForEvent(
                            GameLocationCharacterEventSystem.Event.RotationEnd));
                }

                var isTargetAware = target.PerceivedAllies.Contains(__instance.ActingCharacter) ||
                                    target.PerceivedFoes.Contains(__instance.ActingCharacter);

                if (isTargetAware && !rangeAttack && !target.Prone)
                {
                    var rulesetCharacter = target.RulesetCharacter;

                    if (rulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                        !target.MoveStepInProgress &&
                        !target.IsCharging)
                    {
                        target.TurnTowards(__instance.ActingCharacter, false, false);
                        targetCharacterTurnCoroutine = Coroutine.StartCoroutine(
                            target.EventSystem.UpdateMotionsAndWaitForEvent(GameLocationCharacterEventSystem.Event
                                .RotationEnd));
                    }
                }

                while (targetCharacterTurnCoroutine is { Empty: false } ||
                       actingCharacterTurnCoroutine is { Empty: false })
                {
                    if (actingCharacterTurnCoroutine is { Empty: false })
                    {
                        actingCharacterTurnCoroutine.Run();

                        if (actingCharacterTurnCoroutine.IsFinished)
                        {
                            actingCharacterTurnCoroutine.Reset();
                            actingCharacterTurnCoroutine = null;
                        }
                    }

                    if (targetCharacterTurnCoroutine is { Empty: false })
                    {
                        targetCharacterTurnCoroutine.Run();

                        if (targetCharacterTurnCoroutine.IsFinished)
                        {
                            targetCharacterTurnCoroutine.Reset();
                            actingCharacterTurnCoroutine = null;
                        }
                    }

                    yield return null;
                }

                if (!__instance.skipAnimationWarmup)
                {
                    yield return Coroutine.WaitForSeconds(GameConfiguration.CharacterAnimation.WaitingTimeAfterMove);
                }

                if (__instance.ActingCharacter.RulesetCharacter is RulesetCharacterMonster &&
                    attackMode.SourceDefinition is MonsterAttackDefinition definition &&
                    __instance.ActingCharacter.SetCurrentMonsterAttack(definition))
                {
                    yield return __instance.ActingCharacter.EventSystem.WaitForEvent(
                        GameLocationCharacterEventSystem.Event.MonsterWeaponSwapped);
                }

                yield return __instance.ActingCharacter.WaitForHitAnimation();

                __instance.ActingCharacter.AttackOn(
                    target, __instance.AttackRollOutcome, __instance.ActionParams, attackMode, attackModifier);

                yield return __instance.ActingCharacter.EventSystem.WaitForEvent(
                    GameLocationCharacterEventSystem.Event.AttackImpact);

                if (__instance.AttackRollOutcome == RollOutcome.Failure)
                {
                    yield return battleService.HandleBardicInspirationForAttack(
                        __instance, __instance.ActingCharacter, target, attackModifier);
                }

                if (rangeAttack)
                {
                    var returnProjectileOnly = attackMode.ReturnProjectileOnly;

                    worldLocationEntityFactoryService.TryFindWorldCharacter(
                        __instance.ActingCharacter, out var worldLocationCharacter);

                    var boneType = AnimationDefinitions.BoneType.Prop1;

                    switch (returnProjectileOnly)
                    {
                        case false when attackMode.SourceDefinition is MonsterAttackDefinition attackDefinition:
                            boneType = attackDefinition.ProjectileBone;
                            break;
                        case true:
                            boneType = AnimationDefinitions.BoneType.Prop1;
                            break;
                        default:
                        {
                            if (attackMode.SourceDefinition is ItemDefinition sourceDefinition &&
                                !string.IsNullOrEmpty(attackMode.SlotName))
                            {
                                if (sourceDefinition.IsWeapon)
                                {
                                    boneType = sourceDefinition.WeaponDescription.WeaponTypeDefinition.IsBow ||
                                               attackMode.SlotName != EquipmentDefinitions.SlotTypeOffHand
                                        ? AnimationDefinitions.BoneType.Prop1
                                        : AnimationDefinitions.BoneType.Prop2;
                                }
                            }

                            break;
                        }
                    }

                    var transformPosition =
                        worldLocationCharacter.TryGetBoneTransformPosition(boneType, out var position1);

                    worldLocationCharacter.TryGetBoneTransformPosition(boneType, out var position2, true);

                    if (!transformPosition)
                    {
                        Trace.LogError(
                            $"Couldn't find bone {boneType} on worldLocationCharacter {worldLocationCharacter.name}. Using backup Head bone.");
                        transformPosition =
                            worldLocationCharacter.TryGetBoneTransformPosition(
                                AnimationDefinitions.BoneType.Head, out position1);
                        worldLocationCharacter.TryGetBoneTransformPosition(
                            AnimationDefinitions.BoneType.Head, out position2, true);
                    }

                    if (!transformPosition)
                    {
                        position1 = worldLocationCharacter.transform.position;
                        position2 = worldLocationCharacter.DeterministicPosition;
                    }

                    var impactCenter = new Vector3();
                    var vector3 = new Vector3();
                    var identity1 = Quaternion.identity;
                    var identity2 = Quaternion.identity;

                    gameLocationPositioningService.ComputeImpactCenterPositionAndRotation(
                        target, ref impactCenter, ref identity1);
                    gameLocationPositioningService.ComputeImpactCenterPositionAndRotation(
                        target, ref vector3, ref identity2, true);

                    if (__instance.AttackRollOutcome != RollOutcome.Success &&
                        __instance.AttackRollOutcome != RollOutcome.CriticalSuccess)
                    {
                        var impactPlanePosition = gameLocationPositioningService.GetImpactPlanePosition(vector3);

                        gameLocationPositioningService.ComputeMissedImpactPositionAndRotation(
                            position2, ref vector3, ref identity2, ref impactPlanePosition);
                        impactCenter = vector3;
                    }

                    var projectileFlightDuration = Vector3.Distance(position2, vector3) /
                                                   GameConfiguration.CharacterAnimation.ProjectileSpeedCellsPerSecond;

                    yield return battleService.HandleRangeAttackVFX(
                        __instance.ActingCharacter, target, attackMode, position1, impactCenter,
                        projectileFlightDuration);
                }

                MotionForm resultingActionMotionForm = null;
                var isResultingActionSpendPowerWithMotionForm = false;
                var targetKilledBySideEffect = false;
                var attackHasDamaged = false;
                var hit = false;
                var damageReceived = 0;

                if (__instance.AttackRollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
                {
                    if (__instance.AttackRoll != DiceMaxValue[8] && !attackMode.AutomaticHit)
                    {
                        yield return battleService.HandleCharacterAttackHitPossible(
                            __instance.ActingCharacter,
                            target,
                            attackMode,
                            null,
                            attackModifier,
                            __instance.AttackRoll,
                            __instance.AttackSuccessDelta,
                            rangeAttack);
                    }

                    if (!attackMode.AutomaticHit)
                    {
                        __instance.ActingCharacter.RulesetCharacter.RollAttackMode(
                            attackMode, rangeAttack,
                            target.RulesetActor,
                            attackMode.SourceDefinition,
                            attackModifier.AttacktoHitTrends,
                            attackModifier.IgnoreAdvantage,
                            attackModifier.AttackAdvantageTrends,
                            opportunity,
                            attackModifier.AttackRollModifier,
                            out var outcome, out var successDelta,
                            __instance.AttackRoll,
                            false);
                        __instance.AttackRollOutcome = outcome;
                        __instance.AttackSuccessDelta = successDelta;
                    }
                    else
                    {
                        var attackAutomaticHit = __instance.ActingCharacter.RulesetCharacter.AttackAutomaticHit;

                        attackAutomaticHit?.Invoke(
                            __instance.ActingCharacter.RulesetCharacter, target.RulesetActor,
                            attackMode.SourceDefinition);
                    }

                    if (__instance.AttackRollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
                    {
                        hit = true;
                        __instance.ActingCharacter.AttackedHitCreatureIds.TryAdd(target.RulesetActor.Guid);
                        __instance.ActingCharacter.RulesetCharacter.AcknowledgeAttackHit(
                            target, attackMode, attackModifier.Proximity);
                        __instance.actualEffectForms.Clear();
                        __instance.actualEffectForms.AddRange(attackMode.EffectDescription.EffectForms);

                        if (attackMode.HasOneDamageForm)
                        {
                            attackHasDamaged = true;

                            yield return battleService.HandleCharacterAttackHitConfirmed(
                                __instance,
                                __instance.ActingCharacter,
                                target,
                                attackModifier,
                                attackMode,
                                rangeAttack,
                                attackModifier.IgnoreAdvantage
                                    ? AdvantageType.None
                                    : ComputeAdvantage(attackModifier.AttackAdvantageTrends),
                                __instance.actualEffectForms,
                                null, __instance.AttackRollOutcome == RollOutcome.CriticalSuccess, true);
                        }

                        targetKilledBySideEffect = target.RulesetActor.IsDead;

                        var hasBorrowedLuck =
                            target.RulesetActor.HasConditionOfTypeOrSubType("ConditionDomainMischiefBorrowedLuck");

                        __instance.RolledSaveThrow = attackMode.TryRollSavingThrow(
                            __instance.ActingCharacter.RulesetCharacter,
                            target.RulesetActor,
                            attackModifier,
                            __instance.actualEffectForms,
                            out var saveOutcome,
                            out _);
                        __instance.SaveOutcome = saveOutcome;
                        __instance.SaveOutcomeDelta = __instance.SaveOutcomeDelta;

                        if (__instance.RolledSaveThrow)
                        {
                            target.RulesetActor?.GrantConditionOnSavingThrowOutcome(
                                attackMode.EffectDescription,
                                saveOutcome,
                                true);

                            if (__instance.SaveOutcome == RollOutcome.Failure)
                            {
                                yield return battleService.HandleFailedSavingThrow(
                                    __instance,
                                    __instance.ActingCharacter,
                                    target,
                                    attackModifier,
                                    false,
                                    hasBorrowedLuck);
                            }
                        }

                        foreach (var resultingAction in __instance.ResultingActions)
                        {
                            if (resultingAction is not CharacterActionSpendPower actionSpendPower)
                            {
                                continue;
                            }

                            foreach (var effectForm in actionSpendPower.ActionParams.RulesetEffect.EffectDescription
                                         .EffectForms.Where(effectForm =>
                                             effectForm.FormType == EffectForm.EffectFormType.Motion &&
                                             MotionForm.IsPushMotion(effectForm.MotionForm.Type)))
                            {
                                isResultingActionSpendPowerWithMotionForm = true;
                                effectForm.MotionForm.ForceTurnTowardsSourceCharacterAfterPush = true;
                                effectForm.MotionForm.ForceSourceCharacterTurnTowardsTargetAfterPush = true;
                                resultingActionMotionForm = effectForm.MotionForm;
                                target.WillBePushedByMagicalEffect = true;
                                break;
                            }
                        }

                        var wasDeadOrDyingOrUnconscious =
                            target is { RulesetCharacter: not null } &&
                            target.RulesetActor.IsDeadOrDyingOrUnconscious;
                        var formParams = new RulesetImplementationDefinitions.ApplyFormsParams();

                        formParams.FillSourceAndTarget(__instance.ActingCharacter.RulesetCharacter,
                            target.RulesetActor);
                        formParams.FillFromAttackMode(attackMode);
                        formParams.FillAttackModeSpecialParameters(
                            __instance.RolledSaveThrow,
                            attackModifier,
                            __instance.SaveOutcome,
                            __instance.SaveOutcomeDelta,
                            __instance.AttackRollOutcome == RollOutcome.CriticalSuccess);
                        formParams.effectSourceType = EffectSourceType.Attack;

                        if (__instance.ActionParams.UsablePower != null)
                        {
                            __instance.actualEffectForms.AddRange(
                                __instance.ActionParams.UsablePower.PowerDefinition.EffectDescription.EffectForms);
                            formParams.classLevel =
                                __instance.ActingCharacter.RulesetCharacter.TryGetAttributeValue("CharacterLevel");
                        }

                        __instance.ActingCharacter.RulesetCharacter.EvaluateAndNotifyBardicNegativeInspiration(
                            RollContext.AttackDamageValueRoll);

                        var saveOutcomeSuccess =
                            __instance.SaveOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess;

                        yield return battleService.HandleDefenderBeforeDamageReceived(
                            __instance.ActingCharacter,
                            target,
                            attackMode,
                            null,
                            formParams.actionModifier,
                            __instance.RolledSaveThrow,
                            saveOutcomeSuccess);

                        damageReceived = rulesetImplementationService.ApplyEffectForms(
                            __instance.actualEffectForms,
                            formParams,
                            __instance.effectiveDamageTypes,
                            out var damageAbsorbedByTemporaryHitPoints,
                            out _,
                            effectApplication: attackMode.EffectDescription.EffectApplication,
                            filters: attackMode.EffectDescription.EffectFormFilters);
                        __instance.ActingCharacter.AttackImpactOn(
                            target,
                            __instance.AttackRollOutcome,
                            __instance.ActionParams,
                            attackMode,
                            formParams.actionModifier);

                        if ((damageReceived > 0) | damageAbsorbedByTemporaryHitPoints)
                        {
                            yield return battleService.HandleDefenderOnDamageReceived(
                                __instance.ActingCharacter,
                                target,
                                damageReceived,
                                null,
                                __instance.effectiveDamageTypes);

                            yield return battleService.HandleAttackerOnDefenderDamageReceived(
                                __instance.ActingCharacter,
                                target,
                                damageReceived,
                                null,
                                __instance.effectiveDamageTypes);

                            if (!damageAbsorbedByTemporaryHitPoints)
                            {
                                yield return battleService.HandleReactionToDamageShare(target, damageReceived);
                            }
                        }

                        if (!wasDeadOrDyingOrUnconscious && target.RulesetActor.IsDeadOrDyingOrUnconscious)
                        {
                            isResultingActionSpendPowerWithMotionForm = false;

                            yield return battleService.HandleTargetReducedToZeroHP(
                                __instance.ActingCharacter,
                                target,
                                attackMode,
                                null);
                        }
                    }
                }
                else
                {
                    __instance.ActingCharacter.RulesetCharacter.RollAttackMode(
                        attackMode,
                        rangeAttack,
                        target.RulesetActor,
                        attackMode.SourceDefinition,
                        attackModifier.AttacktoHitTrends,
                        attackModifier.IgnoreAdvantage,
                        attackModifier.AttackAdvantageTrends,
                        opportunity, attackModifier.AttackRollModifier,
                        out var outcome,
                        out var successDelta,
                        __instance.AttackRoll,
                        false);
                    __instance.AttackRollOutcome = outcome;
                    __instance.AttackSuccessDelta = successDelta;
                    __instance.ActingCharacter.AttackImpactOn(
                        target,
                        __instance.AttackRollOutcome,
                        __instance.ActionParams,
                        attackMode,
                        attackModifier);
                }

                var multiAttackInProgress =
                    __instance.ActingCharacter.ControllerId == 4242 &&
                    __instance.ActingCharacter.GetActionAvailableIterations(ActionDefinitions.Id.AttackMain) > 1;

                target.WillBePushedByMagicalEffect = false;

                if (target.RulesetCharacter != null &&
                    !target.Prone &&
                    !targetKilledBySideEffect &&
                    damageReceived > 0 &&
                    !multiAttackInProgress &&
                    !isResultingActionSpendPowerWithMotionForm &&
                    !target.RulesetCharacter.IsDeadOrDying)
                {
                    yield return target.WaitForHitAnimation();
                }

                __instance.ActingCharacter.HasAttackedSinceLastTurn = true;
                __instance.ActingCharacter.RulesetCharacter.AcknowledgeAttackUse(
                    attackMode,
                    attackModifier.Proximity,
                    hit,
                    out var droppedItem,
                    out var needToRefresh);
                __instance.ActingCharacter.RulesetCharacter.AcknowledgeAttackedCharacter(
                    target.RulesetCharacter,
                    attackModifier.Proximity);

                if (droppedItem != null)
                {
                    needToRefresh = true;

                    if (positioningService.TryGetWalkableGroundBelowPosition(
                            target?.LocationPosition ?? __instance.ActingCharacter.LocationPosition,
                            out var groundPosition))
                    {
                        itemService.CreateItem(droppedItem, groundPosition, false);
                    }
                }

                __instance.ActingCharacter.RulesetCharacter.ProcessConditionsMatchingInterruption(
                    ConditionInterruption.Attacks);

                if (__instance.ActingCharacter.RulesetCharacter.IsWieldingBow())
                {
                    __instance.ActingCharacter.RulesetCharacter.ProcessConditionsMatchingInterruption(
                        ConditionInterruption.AttacksWithBow);
                }

                yield return battleService.HandleCharacterPhysicalAttackFinished(
                    __instance,
                    __instance.ActingCharacter,
                    target,
                    attackParams.attackMode,
                    __instance.AttackRollOutcome,
                    damageReceived);

                yield return battleService.HandleCharacterAttackFinished(
                    __instance,
                    __instance.ActingCharacter,
                    target,
                    attackParams.attackMode,
                    null,
                    __instance.AttackRollOutcome,
                    damageReceived);

                if (attackHasDamaged)
                {
                    __instance.ActingCharacter.RulesetCharacter.ProcessConditionsMatchingInterruption(
                        ConditionInterruption.AttacksAndDamages, damageReceived);
                }

                if (defenderWasConscious && target.RulesetActor.IsDeadOrDyingOrUnconscious)
                {
                    ++__instance.ActingCharacter.EnemiesDownedByAttack;
                }

                if (needToRefresh)
                {
                    __instance.ActingCharacter.RulesetCharacter.RefreshAttackModes();
                }

                if (!isResultingActionSpendPowerWithMotionForm)
                {
                    yield return __instance.ActingCharacter.EventSystem.WaitForEvent(
                        GameLocationCharacterEventSystem.Event.AttackAnimationEnd);
                }
                else
                {
                    __instance.ActingCharacter.EventSystem.AbsorbNextEvent(
                        GameLocationCharacterEventSystem.Event.AttackAnimationEnd);
                }

                if (!__instance.ActingCharacter.RulesetActor.IsDeadOrDyingOrUnconscious && !multiAttackInProgress)
                {
                    if (!isResultingActionSpendPowerWithMotionForm)
                    {
                        __instance.ActingCharacter.TurnTowards(target);
                        actingCharacterTurnCoroutine = Coroutine.StartCoroutine(
                            __instance.ActingCharacter.EventSystem.UpdateMotionsAndWaitForEvent(
                                GameLocationCharacterEventSystem.Event.RotationEnd));
                    }
                }
                else if (resultingActionMotionForm != null)
                {
                    resultingActionMotionForm.ForceSourceCharacterTurnTowardsTargetAfterPush = false;
                }

                if (isTargetAware && !rangeAttack && !target.Prone)
                {
                    var rulesetCharacter = target.RulesetCharacter;

                    if (rulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                        !target.IsCharging &&
                        !target.MoveStepInProgress &&
                        !multiAttackInProgress)
                    {
                        if (!isResultingActionSpendPowerWithMotionForm)
                        {
                            target.TurnTowards(__instance.ActingCharacter);
                            targetCharacterTurnCoroutine = Coroutine.StartCoroutine(
                                target.EventSystem.UpdateMotionsAndWaitForEvent(
                                    GameLocationCharacterEventSystem.Event.RotationEnd));
                        }

                        goto label_115;
                    }
                }

                if (resultingActionMotionForm != null)
                {
                    resultingActionMotionForm.ForceTurnTowardsSourceCharacterAfterPush = false;
                }

                label_115:

                while (targetCharacterTurnCoroutine is { Empty: false } ||
                       actingCharacterTurnCoroutine is { Empty: false })
                {
                    if (actingCharacterTurnCoroutine is { Empty: false })
                    {
                        actingCharacterTurnCoroutine.Run();

                        if (actingCharacterTurnCoroutine.IsFinished)
                        {
                            actingCharacterTurnCoroutine.Reset();
                            actingCharacterTurnCoroutine = null;
                        }
                    }

                    if (targetCharacterTurnCoroutine is { Empty: false })
                    {
                        targetCharacterTurnCoroutine.Run();

                        if (targetCharacterTurnCoroutine.IsFinished)
                        {
                            targetCharacterTurnCoroutine.Reset();
                            actingCharacterTurnCoroutine = null;
                        }
                    }

                    yield return null;
                }

                if (__instance.isChargeAttack)
                {
                    __instance.ActingCharacter.IsCharging = false;
                    __instance.ActingCharacter.MovingToDestination = false;
                }

                target.RulesetActor.ProcessConditionsMatchingInterruption(
                    ConditionInterruption.PhysicalAttackReceivedExecuted);

                yield return battleService.HandleCharacterAttackOrMagicEffectFinishedLate(
                    __instance, __instance.ActingCharacter);
            }
        }
    }
}
