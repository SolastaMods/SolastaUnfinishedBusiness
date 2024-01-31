using System.Collections;
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
        [UsedImplicitly]
        public static IEnumerator Postfix(
#pragma warning disable IDE0060
            IEnumerator values,
#pragma warning restore IDE0060
            CharacterActionAttack __instance)
        {
            yield return ExecuteImpl(__instance);
        }

        private static IEnumerator ExecuteImpl(CharacterActionAttack __instance)
        {
            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();
            var locationPositioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
            var locationEntityFactoryService = ServiceRepository.GetService<IWorldLocationEntityFactoryService>();
            var implementationService = ServiceRepository.GetService<IRulesetImplementationService>();
            var itemService = ServiceRepository.GetService<IGameLocationItemService>();
            var positioningService = ServiceRepository.GetService<IGameLocationPositioningService>();

            // Check action params
            var canAttackMain =
                __instance.ActingCharacter.GetActionStatus(
                    ActionDefinitions.Id.AttackMain,
                    ActionDefinitions.ActionScope.Battle,
                    optionalAttackMode: __instance.ActionParams.AttackMode) == ActionDefinitions.ActionStatus.Available;

            var canAttackOff =
                __instance.ActingCharacter.GetActionStatus(
                    ActionDefinitions.Id.AttackOff,
                    ActionDefinitions.ActionScope.Battle,
                    optionalAttackMode: __instance.ActionParams.AttackMode) == ActionDefinitions.ActionStatus.Available;

            if ((!canAttackMain && __instance.ActionType == ActionDefinitions.ActionType.Main)
                || (!canAttackOff && __instance.ActionType == ActionDefinitions.ActionType.Bonus))
            {
                Trace.Assert(false,
                    $"CharacterActionAttack called with an unavailable action type : {__instance.ActionType}");

                __instance.Abort(CharacterAction.InterruptionType.Invalid);

                if (!__instance.isChargeAttack)
                {
                    yield break;
                }

                __instance.ActingCharacter.IsCharging = false;
                __instance.ActingCharacter.MovingToDestination = false; // Safety

                yield break;
            }

            var targets = __instance.ActionParams.TargetCharacters;
            var target = targets[0];
            var defenderWasConscious = !target.RulesetActor.IsDeadOrDyingOrUnconscious;

            // Check if the attack is possible, and compute modifiers
            var attackParams = new BattleDefinitions.AttackEvaluationParams();
            var attackModifier = new ActionModifier();
            var attackMode = __instance.ActionParams.AttackMode;

            if (!attackMode.Ranged)
            {
                attackParams.FillForPhysicalReachAttack(
                    __instance.ActingCharacter,
                    __instance.ActingCharacter.LocationPosition,
                    attackMode, target, target.LocationPosition, attackModifier);
            }
            else
            {
                attackParams.FillForPhysicalRangeAttack(
                    __instance.ActingCharacter,
                    __instance.ActingCharacter.LocationPosition,
                    attackMode, target, target.LocationPosition, attackModifier);
            }

            attackParams.opportunityAttack = __instance.ActionId == ActionDefinitions.Id.AttackOpportunity;
            attackParams.readiedAttack = __instance.ActionId == ActionDefinitions.Id.AttackReadied;

            var canAttack = battleService.CanAttack(attackParams);

            if (!canAttack)
            {
                var attackModifiers = __instance.ActionParams.ActionModifiers;

                attackModifier = attackModifiers[0];
            }

            yield return battleService.HandleCharacterPhysicalAttackInitiated(
                __instance, __instance.ActingCharacter, target, attackModifier, attackMode);

            // Determine the attack success
            __instance.AttackRollOutcome = RollOutcome.Failure;

            var opportunity = __instance.ActionId == ActionDefinitions.Id.AttackOpportunity;
            var rangeAttack = attackModifier.Proximity == AttackProximity.Range;

            // This is only a test attack for now, as a Shield spell could fail the attack after all, raising the armor class

            // Automatic hit is for Flaming Sphere, which hits automatically with a saving throw
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

            if (isTargetAware &&
                target.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                !rangeAttack &&
                !target.Prone &&
                !target.MoveStepInProgress &&
                !target.IsCharging)
            {
                target.TurnTowards(__instance.ActingCharacter, false, false);
                targetCharacterTurnCoroutine = Coroutine.StartCoroutine(
                    target.EventSystem.UpdateMotionsAndWaitForEvent(
                        GameLocationCharacterEventSystem.Event.RotationEnd));
            }

            // Update turn animation in parallel for Acting / Target characters
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
                // HACK : Ensure the character animation is idle, as a trigger is not considered if the animator is in a transition
                yield return Coroutine.WaitForSeconds(GameConfiguration.CharacterAnimation.WaitingTimeAfterMove);
            }

            // Wait for monster weapon swap ?
            if (__instance.ActingCharacter.RulesetCharacter is RulesetCharacterMonster &&
                attackMode.SourceDefinition is MonsterAttackDefinition definition)
            {
                var hasChanged =
                    __instance.ActingCharacter.SetCurrentMonsterAttack(definition);

                if (hasChanged)
                {
                    yield return __instance.ActingCharacter.EventSystem.WaitForEvent(
                        GameLocationCharacterEventSystem.Event.MonsterWeaponSwapped);
                }
            }

            // Acting character could be trying to act during a hit animation
            yield return __instance.ActingCharacter.WaitForHitAnimation();

            __instance.ActingCharacter.AttackOn(
                target, __instance.AttackRollOutcome, __instance.ActionParams, attackMode, attackModifier);

            yield return __instance.ActingCharacter.EventSystem.WaitForEvent(
                GameLocationCharacterEventSystem.Event.AttackImpact);

            // If this roll is failed (not critically), can we use a bardic inspiration to change the outcome?
            if (__instance.AttackRollOutcome == RollOutcome.Failure)
            {
                yield return battleService.HandleBardicInspirationForAttack(
                    __instance, __instance.ActingCharacter, target, attackModifier);
            }

            if (rangeAttack)
            {
                var isMonkReturnMissile = attackMode.ReturnProjectileOnly;

                locationEntityFactoryService.TryFindWorldCharacter(
                    __instance.ActingCharacter, out var worldLocationCharacter);

                var boneType = AnimationDefinitions.BoneType.Prop1;

                if (!isMonkReturnMissile && attackMode.SourceDefinition is MonsterAttackDefinition attackDefinition)
                {
                    boneType = attackDefinition.ProjectileBone;
                }
                else
                {
                    // Get correct bone based on equipped slot
                    if (isMonkReturnMissile)
                    {
                        boneType = AnimationDefinitions.BoneType.Prop1;
                    }
                    else if (attackMode.SourceDefinition is ItemDefinition itemDefinition &&
                             !string.IsNullOrEmpty(attackMode.SlotName))
                    {
                        if (itemDefinition.IsWeapon)
                        {
                            var isBow = itemDefinition.WeaponDescription.WeaponTypeDefinition.IsBow;

                            boneType = !isBow && attackMode.SlotName == EquipmentDefinitions.SlotTypeOffHand
                                ? AnimationDefinitions.BoneType.Prop2
                                : AnimationDefinitions.BoneType.Prop1;
                        }
                    }
                }

                var foundBone = worldLocationCharacter.TryGetBoneTransformPosition(boneType, out var sourcePoint);

                worldLocationCharacter.TryGetBoneTransformPosition(boneType, out var sourcePointDeterministic, true);

                if (!foundBone)
                {
                    // Backup bone
                    Trace.LogError(
                        $"Couldn't find bone {boneType} on worldLocationCharacter {worldLocationCharacter.name}. Using backup Head bone.");

                    foundBone = worldLocationCharacter.TryGetBoneTransformPosition(AnimationDefinitions.BoneType.Head,
                        out sourcePoint);
                    worldLocationCharacter.TryGetBoneTransformPosition(AnimationDefinitions.BoneType.Head,
                        out sourcePointDeterministic, true);
                }

                if (!foundBone)
                {
                    // Last resort
                    sourcePoint = worldLocationCharacter.transform.position;
                    sourcePointDeterministic = worldLocationCharacter.DeterministicPosition;
                }

                var impactPoint = new Vector3();
                var impactPointDeterministic = new Vector3();
                var impactRotation = Quaternion.identity;
                var impactRotationDeterministic = Quaternion.identity;

                locationPositioningService.ComputeImpactCenterPositionAndRotation(
                    target, ref impactPoint, ref impactRotation);
                locationPositioningService.ComputeImpactCenterPositionAndRotation(
                    target, ref impactPointDeterministic, ref impactRotationDeterministic, true);

                if (__instance.AttackRollOutcome != RollOutcome.Success &&
                    __instance.AttackRollOutcome != RollOutcome.CriticalSuccess)
                {
                    var impactPlanePoint =
                        locationPositioningService.GetImpactPlanePosition(impactPointDeterministic);

                    locationPositioningService.ComputeMissedImpactPositionAndRotation(sourcePointDeterministic,
                        ref impactPointDeterministic, ref impactRotationDeterministic, ref impactPlanePoint);
                    impactPoint = impactPointDeterministic;
                }

                // Compute the flight duration
                var distanceToTarget = Vector3.Distance(sourcePointDeterministic, impactPointDeterministic);
                var projectileFlightDuration =
                    distanceToTarget / GameConfiguration.CharacterAnimation.ProjectileSpeedCellsPerSecond;

                //TODO: Mask thrown weapon here !
                yield return battleService.HandleRangeAttackVFX(
                    __instance.ActingCharacter, target, attackMode, sourcePoint, impactPoint, projectileFlightDuration);
            }

            var isResultingActionSpendPowerWithMotionForm = false;

            MotionForm resultingActionMotionForm = null;

            var targetKilledBySideEffect = false;
            var attackHasDamaged = false;
            var hit = false;
            var damageReceived = 0;

            if (__instance.AttackRollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                // No need to test this even if the hit was automatic (natural 20). Warning: Critical Success can occur without rolling a 20 (Champion fighter crits at 19)
                if (__instance.AttackRoll != DiceMaxValue[(int)DieType.D20] && !attackMode.AutomaticHit)
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

                // Execute the final step of the attack
                if (!attackMode.AutomaticHit)
                {
                    __instance.ActingCharacter.RulesetCharacter.RollAttackMode(
                        attackMode,
                        rangeAttack,
                        target.RulesetActor,
                        attackMode.SourceDefinition,
                        attackModifier.AttacktoHitTrends,
                        attackModifier.IgnoreAdvantage,
                        attackModifier.AttackAdvantageTrends,
                        opportunity,
                        attackModifier.AttackRollModifier,
                        out var attackRollOutcome2,
                        out var successDelta2,
                        __instance.AttackRoll,
                        false);
                    __instance.AttackRollOutcome = attackRollOutcome2;
                    __instance.AttackSuccessDelta = successDelta2;
                }
                else
                {
                    __instance.ActingCharacter.RulesetCharacter.AttackAutomaticHit?.Invoke(
                        __instance.ActingCharacter.RulesetCharacter, target.RulesetActor, attackMode.SourceDefinition);
                }

                // Is this still a success?
                if (__instance.AttackRollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
                {
                    hit = true;
                    __instance.ActingCharacter.AttackedHitCreatureIds.TryAdd(target.RulesetActor.Guid);

                    // For recovering ammunition
                    __instance.ActingCharacter.RulesetCharacter.AcknowledgeAttackHit(
                        target, attackMode, attackModifier.Proximity);

                    __instance.actualEffectForms.Clear();
                    __instance.actualEffectForms.AddRange(attackMode.EffectDescription.EffectForms);

                    if (attackMode.HasOneDamageForm)
                    {
                        // Handle special modification of damages (sneak attack, smite, uncanny dodge)
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
                            null,
                            __instance.AttackRollOutcome == RollOutcome.CriticalSuccess,
                            true);
                    }

                    // Check if the target was killed by a reaction
                    targetKilledBySideEffect = target.RulesetActor.IsDead;

                    // Saving throw?
                    var hasBorrowedLuck = target.RulesetActor.HasConditionOfTypeOrSubType(ConditionBorrowedLuck);

                    // These bool information must be store as a class member, as it is passed to HandleFailedSavingThrow
                    __instance.RolledSaveThrow = attackMode.TryRollSavingThrow(
                        __instance.ActingCharacter.RulesetCharacter, target.RulesetActor, attackModifier,
                        __instance.actualEffectForms, out var saveOutcome, out var saveOutcomeDelta);
                    __instance.SaveOutcome = saveOutcome;
                    __instance.SaveOutcomeDelta = saveOutcomeDelta;

                    if (__instance.RolledSaveThrow)
                    {
                        target.RulesetActor?.GrantConditionOnSavingThrowOutcome(
                            attackMode.EffectDescription, saveOutcome, true);

                        // Legendary Resistance or Indomitable?
                        if (__instance.SaveOutcome == RollOutcome.Failure)
                        {
                            yield return battleService.HandleFailedSavingThrow(
                                __instance, __instance.ActingCharacter,
                                target, attackModifier, false, hasBorrowedLuck);
                        }
                    }

                    // Check for resulting actions, if any of them is a CharacterSpendPower w/ a Motion effect form, don't wait for hit animation
                    foreach (var resultingAction in __instance.ResultingActions)
                    {
                        if (resultingAction is not CharacterActionSpendPower characterActionSpendPower)
                        {
                            continue;
                        }

                        var effectDescription =
                            characterActionSpendPower.ActionParams.RulesetEffect.EffectDescription;

                        foreach (var effectForm in effectDescription.EffectForms)
                        {
                            if (effectForm.FormType != EffectForm.EffectFormType.Motion ||
                                !MotionForm.IsPushMotion(effectForm.MotionForm.Type))
                            {
                                continue;
                            }

                            isResultingActionSpendPowerWithMotionForm = true;
                            effectForm.MotionForm.ForceTurnTowardsSourceCharacterAfterPush = true;
                            effectForm.MotionForm.ForceSourceCharacterTurnTowardsTargetAfterPush = true;
                            resultingActionMotionForm = effectForm.MotionForm;
                            target.WillBePushedByMagicalEffect = true;
                            break;
                        }
                    }

                    // Apply Effect forms
                    var wasDeadOrDyingOrUnconscious = target.RulesetCharacter is { IsDeadOrDyingOrUnconscious: true };
                    var formParams = new RulesetImplementationDefinitions.ApplyFormsParams();

                    formParams.FillSourceAndTarget(__instance.ActingCharacter.RulesetCharacter, target.RulesetActor);
                    formParams.FillFromAttackMode(attackMode);
                    formParams.FillAttackModeSpecialParameters(
                        __instance.RolledSaveThrow,
                        attackModifier,
                        __instance.SaveOutcome,
                        __instance.SaveOutcomeDelta,
                        __instance.AttackRollOutcome == RollOutcome.CriticalSuccess);
                    formParams.effectSourceType = EffectSourceType.Attack;

                    // Do we need to add special effect forms from a power?
                    // Do not call RulesetCharacter.UsePower(__instance.ActionParams.UsablePower) here, it should have been
                    // done earlier in the parent action since we must pay the power's price no matter if the attack hits or not
                    if (__instance.ActionParams.UsablePower != null)
                    {
                        // Add the effect forms
                        __instance.actualEffectForms.AddRange(
                            __instance.ActionParams.UsablePower.PowerDefinition.EffectDescription.EffectForms);

                        // Specify the class level for forms which depend on it
                        formParams.classLevel =
                            __instance.ActingCharacter.RulesetCharacter.TryGetAttributeValue(
                                AttributeDefinitions.CharacterLevel);
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
                        attackModifier,
                        __instance.RolledSaveThrow,
                        saveOutcomeSuccess);
                    damageReceived = implementationService.ApplyEffectForms(
                        __instance.actualEffectForms,
                        formParams,
                        __instance.effectiveDamageTypes,
                        effectApplication: attackMode.EffectDescription.EffectApplication,
                        filters: attackMode.EffectDescription.EffectFormFilters,
                        damageAbsorbedByTemporaryHitPoints: out var damageAbsorbedByTemporaryHitPoints,
                        terminateEffectOnTarget: out _);

                    __instance.ActingCharacter.AttackImpactOn(
                        target, __instance.AttackRollOutcome, __instance.ActionParams, attackMode, attackModifier);

                    // Call this now that the damage has been properly applied
                    if (damageReceived > 0 || damageAbsorbedByTemporaryHitPoints)
                    {
                        yield return battleService.HandleDefenderOnDamageReceived(
                            __instance.ActingCharacter, target, damageReceived, null, __instance.effectiveDamageTypes);

                        yield return battleService.HandleAttackerOnDefenderDamageReceived(
                            __instance.ActingCharacter, target, damageReceived, null, __instance.effectiveDamageTypes);

                        if (!damageAbsorbedByTemporaryHitPoints)
                        {
                            yield return battleService.HandleReactionToDamageShare(target, damageReceived);
                        }
                    }

                    if (!wasDeadOrDyingOrUnconscious && target.RulesetActor.IsDeadOrDyingOrUnconscious)
                    {
                        // Reset this since we do not want to apply the motion form later on
                        isResultingActionSpendPowerWithMotionForm = false;

                        yield return battleService.HandleTargetReducedToZeroHP(
                            __instance.ActingCharacter, target, attackMode, null);
                    }
                }
            }
            else
            {
                __instance.ActingCharacter.RulesetCharacter.RollAttackMode(
                    attackMode, rangeAttack,
                    target.RulesetActor,
                    attackMode.SourceDefinition,
                    attackModifier.AttacktoHitTrends,
                    attackModifier.IgnoreAdvantage,
                    attackModifier.AttackAdvantageTrends,
                    opportunity, attackModifier.AttackRollModifier,
                    out var attackRollOutcome2,
                    out var successDelta2,
                    __instance.AttackRoll,
                    false);
                __instance.AttackRollOutcome = attackRollOutcome2;
                __instance.AttackSuccessDelta = successDelta2;

                __instance.ActingCharacter.AttackImpactOn(
                    target, __instance.AttackRollOutcome, __instance.ActionParams, attackMode, attackModifier);
            }

            var multiAttackInProgress =
                __instance.ActingCharacter.ControllerId == PlayerControllerManager.DmControllerId &&
                __instance.ActingCharacter.GetActionAvailableIterations(ActionDefinitions.Id.AttackMain) > 1;

            // Reset this flag after the application of the attack effect forms
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
                attackMode, attackModifier.Proximity, hit, out var droppedItem, out var needToRefresh);
            __instance.ActingCharacter.RulesetCharacter.AcknowledgeAttackedCharacter(
                target.RulesetCharacter, attackModifier.Proximity);

            if (droppedItem != null)
            {
                // Drop the item on the floor
                needToRefresh = true;
                var droppingPoint =
                    target?.LocationPosition ?? __instance.ActingCharacter.LocationPosition;

                if (positioningService.TryGetWalkableGroundBelowPosition(droppingPoint, out var groundPosition))
                {
                    _ = itemService.CreateItem(droppedItem, groundPosition, false);
                }
                else
                {
                    Trace.Log("Thrown object has been destroyed, no position to fall to was found below {0}",
                        droppingPoint);
                }
            }

            // Possible condition interruption, after the attack is done. Example: if an invisible character performs an attack, his invisible condition is broken
            __instance.ActingCharacter.RulesetCharacter.ProcessConditionsMatchingInterruption(
                ConditionInterruption.Attacks);

            if (__instance.ActingCharacter.RulesetCharacter.IsWieldingBow())
            {
                __instance.ActingCharacter.RulesetCharacter.ProcessConditionsMatchingInterruption(
                    ConditionInterruption.AttacksWithBow);
            }

            // Handle specific reactions after the attack has been executed
            yield return battleService.HandleCharacterPhysicalAttackFinished(
                __instance, __instance.ActingCharacter,
                target, attackParams.attackMode, __instance.AttackRollOutcome, damageReceived);
            yield return battleService.HandleCharacterAttackFinished(
                __instance, __instance.ActingCharacter, target,
                attackParams.attackMode, null, __instance.AttackRollOutcome, damageReceived);

            if (attackHasDamaged)
            {
                __instance.ActingCharacter.RulesetCharacter.ProcessConditionsMatchingInterruption(
                    ConditionInterruption.AttacksAndDamages, damageReceived);
            }

            // Did I down the target?
            if (defenderWasConscious && target.RulesetActor.IsDeadOrDyingOrUnconscious)
            {
                __instance.ActingCharacter.EnemiesDownedByAttack++;
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

            if (!__instance.ActingCharacter.RulesetActor.IsDeadOrDyingOrUnconscious &&
                !multiAttackInProgress)
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

            if (isTargetAware &&
                target.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                !rangeAttack &&
                !target.Prone &&
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
            }
            else if (resultingActionMotionForm != null)
            {
                resultingActionMotionForm.ForceTurnTowardsSourceCharacterAfterPush = false;
            }

            // Update turn animation in parallel for Acting / Target characters

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
