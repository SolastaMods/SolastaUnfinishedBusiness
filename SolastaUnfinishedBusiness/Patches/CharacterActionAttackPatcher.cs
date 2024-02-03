using System.Collections;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Interfaces;
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
        public static bool Prefix(
            out IEnumerator __result,
            CharacterActionAttack __instance)
        {
            __result = ExecuteImpl(__instance);

            return false;
        }

        private static IEnumerator ExecuteImpl(CharacterActionAttack __instance)
        {
            var actingCharacter = __instance.ActingCharacter;
            var actionParams = __instance.ActionParams;
            var attackMode = actionParams.AttackMode;

            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();
            var locationPositioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
            var locationEntityFactoryService = ServiceRepository.GetService<IWorldLocationEntityFactoryService>();
            var implementationService = ServiceRepository.GetService<IRulesetImplementationService>();
            var itemService = ServiceRepository.GetService<IGameLocationItemService>();
            var positioningService = ServiceRepository.GetService<IGameLocationPositioningService>();

            // Check action params
            var canAttackMain =
                actingCharacter.GetActionStatus(
                    ActionDefinitions.Id.AttackMain,
                    ActionDefinitions.ActionScope.Battle,
                    optionalAttackMode: attackMode) == ActionDefinitions.ActionStatus.Available;

            var canAttackOff =
                actingCharacter.GetActionStatus(
                    ActionDefinitions.Id.AttackOff,
                    ActionDefinitions.ActionScope.Battle,
                    optionalAttackMode: attackMode) == ActionDefinitions.ActionStatus.Available;

            if ((!canAttackMain && __instance.ActionType == ActionDefinitions.ActionType.Main)
                || (!canAttackOff && __instance.ActionType == ActionDefinitions.ActionType.Bonus))
            {
                // ReSharper disable once InvocationIsSkipped
                Trace.Assert(false,
                    $"CharacterActionAttack called with an unavailable action type : {__instance.ActionType}");

                __instance.Abort(CharacterAction.InterruptionType.Invalid);

                if (!__instance.isChargeAttack)
                {
                    yield break;
                }

                actingCharacter.IsCharging = false;
                actingCharacter.MovingToDestination = false; // Safety

                yield break;
            }

            var targets = actionParams.TargetCharacters;
            var target = targets[0];
            var defenderWasConscious = !target.RulesetActor.IsDeadOrDyingOrUnconscious;

            // Check if the attack is possible, and compute modifiers
            var attackParams = new BattleDefinitions.AttackEvaluationParams();
            var attackModifier = new ActionModifier();

            if (!attackMode.Ranged)
            {
                attackParams.FillForPhysicalReachAttack(
                    actingCharacter,
                    actingCharacter.LocationPosition,
                    attackMode, target, target.LocationPosition, attackModifier);
            }
            else
            {
                attackParams.FillForPhysicalRangeAttack(
                    actingCharacter,
                    actingCharacter.LocationPosition,
                    attackMode, target, target.LocationPosition, attackModifier);
            }

            attackParams.opportunityAttack = __instance.ActionId == ActionDefinitions.Id.AttackOpportunity;
            attackParams.readiedAttack = __instance.ActionId == ActionDefinitions.Id.AttackReadied;

            var canAttack = battleService.CanAttack(attackParams);

            if (!canAttack)
            {
                var attackModifiers = actionParams.ActionModifiers;

                attackModifier = attackModifiers[0];
            }

            yield return battleService.HandleCharacterPhysicalAttackInitiated(
                __instance, actingCharacter, target, attackModifier, attackMode);

            // Determine the attack success
            __instance.AttackRollOutcome = RollOutcome.Failure;

            var opportunity = __instance.ActionId == ActionDefinitions.Id.AttackOpportunity;
            var rangeAttack = attackModifier.Proximity == AttackProximity.Range;

            // This is only a test attack for now, as a Shield spell could fail the attack after all, raising the armor class

            // Automatic hit is for Flaming Sphere, which hits automatically with a saving throw
            if (!attackMode.AutomaticHit)
            {
                __instance.AttackRoll = actingCharacter.RulesetCharacter.RollAttackMode(
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
                actingCharacter.TurnTowards(target, false, false);

                actingCharacterTurnCoroutine = Coroutine.StartCoroutine(
                    actingCharacter.EventSystem.UpdateMotionsAndWaitForEvent(
                        GameLocationCharacterEventSystem.Event.RotationEnd));
            }

            var isTargetAware = target.PerceivedAllies.Contains(actingCharacter) ||
                                target.PerceivedFoes.Contains(actingCharacter);

            if (isTargetAware &&
                target.RulesetCharacter is null or { IsDeadOrDyingOrUnconscious: false } &&
                !rangeAttack &&
                !target.Prone &&
                !target.MoveStepInProgress &&
                !target.IsCharging)
            {
                target.TurnTowards(actingCharacter, false, false);
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
            if (actingCharacter.RulesetCharacter is RulesetCharacterMonster &&
                attackMode.SourceDefinition is MonsterAttackDefinition definition)
            {
                var hasChanged =
                    actingCharacter.SetCurrentMonsterAttack(definition);

                if (hasChanged)
                {
                    yield return actingCharacter.EventSystem.WaitForEvent(
                        GameLocationCharacterEventSystem.Event.MonsterWeaponSwapped);
                }
            }

            // Acting character could be trying to act during a hit animation
            yield return actingCharacter.WaitForHitAnimation();

            actingCharacter.AttackOn(
                target, __instance.AttackRollOutcome, actionParams, attackMode, attackModifier);

            yield return actingCharacter.EventSystem.WaitForEvent(
                GameLocationCharacterEventSystem.Event.AttackImpact);

            // If this roll is failed (not critically), can we use a bardic inspiration to change the outcome?
            if (__instance.AttackRollOutcome == RollOutcome.Failure)
            {
                yield return battleService.HandleBardicInspirationForAttack(
                    __instance, actingCharacter, target, attackModifier);

                // BEGIN PATCH

                //BUGFIX: vanilla doesn't add the bardic die roll to attack success delta
                if (__instance.AttackRollOutcome == RollOutcome.Success &&
                    __instance.BardicDieRoll > 0)
                {
                    __instance.AttackSuccessDelta += __instance.BardicDieRoll;
                }

                // END PATCH
            }

            // BEGIN PATCH

            //PATCH: support for IAlterAttackOutcome
            yield return actingCharacter.RulesetCharacter
                .GetSubFeaturesByType<ITryAlterOutcomePhysicalAttackByMe>()
                .TakeWhile(_ =>
                    __instance.AttackRollOutcome == RollOutcome.Failure &&
                    __instance.AttackSuccessDelta < 0)
                .Select(feature =>
                    feature.OnAttackTryAlterOutcomeByMe(battleService as GameLocationBattleManager,
                        __instance, actingCharacter, target, attackModifier));

            // END PATCH

            if (rangeAttack)
            {
                var isMonkReturnMissile = attackMode.ReturnProjectileOnly;

                locationEntityFactoryService.TryFindWorldCharacter(
                    actingCharacter, out var worldLocationCharacter);

                var boneType = AnimationDefinitions.BoneType.Prop1;

                switch (isMonkReturnMissile)
                {
                    case false when attackMode.SourceDefinition is MonsterAttackDefinition attackDefinition:
                        boneType = attackDefinition.ProjectileBone;
                        break;
                    // Get correct bone based on equipped slot
                    case true:
                        boneType = AnimationDefinitions.BoneType.Prop1;
                        break;
                    default:
                    {
                        if (attackMode.SourceDefinition is ItemDefinition itemDefinition &&
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

                        break;
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
                    actingCharacter, target, attackMode, sourcePoint, impactPoint, projectileFlightDuration);
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
                        actingCharacter,
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
                    actingCharacter.RulesetCharacter.RollAttackMode(
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
                    actingCharacter.RulesetCharacter.AttackAutomaticHit?.Invoke(
                        actingCharacter.RulesetCharacter, target.RulesetActor, attackMode.SourceDefinition);
                }

                // Is this still a success?
                if (__instance.AttackRollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
                {
                    hit = true;
                    actingCharacter.AttackedHitCreatureIds.TryAdd(target.RulesetActor.Guid);

                    // For recovering ammunition
                    actingCharacter.RulesetCharacter.AcknowledgeAttackHit(
                        target, attackMode, attackModifier.Proximity);

                    __instance.actualEffectForms.Clear();
                    __instance.actualEffectForms.AddRange(attackMode.EffectDescription.EffectForms);

                    if (attackMode.HasOneDamageForm)
                    {
                        // Handle special modification of damages (sneak attack, smite, uncanny dodge)
                        attackHasDamaged = true;
                        yield return battleService.HandleCharacterAttackHitConfirmed(
                            __instance,
                            actingCharacter,
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
                        actingCharacter.RulesetCharacter, target.RulesetActor, attackModifier,
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
                                __instance, actingCharacter,
                                target, attackModifier, false, hasBorrowedLuck);
                        }

                        // BEGIN PATCH

                        //PATCH: support for `ITryAlterOutcomeSavingThrow`
                        yield return TryAlterOutcomeSavingThrowFromAllyOrEnemy.Handler(
                            battleService as GameLocationBattleManager,
                            __instance, actingCharacter, target, attackModifier, hasBorrowedLuck);

                        // END PATCH
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

                        foreach (var effectForm in effectDescription.EffectForms
                                     .Where(effectForm =>
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

                    // Apply Effect forms
                    var wasDeadOrDyingOrUnconscious = target.RulesetCharacter is { IsDeadOrDyingOrUnconscious: true };
                    var formParams = new RulesetImplementationDefinitions.ApplyFormsParams();

                    formParams.FillSourceAndTarget(actingCharacter.RulesetCharacter, target.RulesetActor);
                    formParams.FillFromAttackMode(attackMode);
                    formParams.FillAttackModeSpecialParameters(
                        __instance.RolledSaveThrow,
                        attackModifier,
                        __instance.SaveOutcome,
                        __instance.SaveOutcomeDelta,
                        __instance.AttackRollOutcome == RollOutcome.CriticalSuccess);
                    formParams.effectSourceType = EffectSourceType.Attack;

                    // Do we need to add special effect forms from a power?
                    // Do not call RulesetCharacter.UsePower(actionParams.UsablePower) here, it should have been
                    // done earlier in the parent action since we must pay the power's price no matter if the attack hits or not
                    if (actionParams.UsablePower != null)
                    {
                        // Add the effect forms
                        __instance.actualEffectForms.AddRange(
                            actionParams.UsablePower.PowerDefinition.EffectDescription.EffectForms);

                        // Specify the class level for forms which depend on it
                        formParams.classLevel =
                            actingCharacter.RulesetCharacter.TryGetAttributeValue(
                                AttributeDefinitions.CharacterLevel);
                    }

                    actingCharacter.RulesetCharacter.EvaluateAndNotifyBardicNegativeInspiration(
                        RollContext.AttackDamageValueRoll);

                    var saveOutcomeSuccess =
                        __instance.SaveOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess;

                    yield return battleService.HandleDefenderBeforeDamageReceived(
                        actingCharacter,
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

                    actingCharacter.AttackImpactOn(
                        target, __instance.AttackRollOutcome, actionParams, attackMode, attackModifier);

                    // Call this now that the damage has been properly applied
                    if (damageReceived > 0 || damageAbsorbedByTemporaryHitPoints)
                    {
                        yield return battleService.HandleDefenderOnDamageReceived(
                            actingCharacter, target, damageReceived, null, __instance.effectiveDamageTypes);

                        yield return battleService.HandleAttackerOnDefenderDamageReceived(
                            actingCharacter, target, damageReceived, null, __instance.effectiveDamageTypes);

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
                            actingCharacter, target, attackMode, null);
                    }
                }
            }
            else
            {
                actingCharacter.RulesetCharacter.RollAttackMode(
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

                actingCharacter.AttackImpactOn(
                    target, __instance.AttackRollOutcome, actionParams, attackMode, attackModifier);
            }

            var multiAttackInProgress =
                actingCharacter.ControllerId == PlayerControllerManager.DmControllerId &&
                actingCharacter.GetActionAvailableIterations(ActionDefinitions.Id.AttackMain) > 1;

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

            actingCharacter.HasAttackedSinceLastTurn = true;
            actingCharacter.RulesetCharacter.AcknowledgeAttackUse(
                attackMode, attackModifier.Proximity, hit, out var droppedItem, out var needToRefresh);
            actingCharacter.RulesetCharacter.AcknowledgeAttackedCharacter(
                target.RulesetCharacter, attackModifier.Proximity);

            if (droppedItem != null)
            {
                // Drop the item on the floor
                needToRefresh = true;
                var droppingPoint =
                    target?.LocationPosition ?? actingCharacter.LocationPosition;

                if (positioningService.TryGetWalkableGroundBelowPosition(droppingPoint, out var groundPosition))
                {
                    _ = itemService.CreateItem(droppedItem, groundPosition, false);
                }
                else
                {
                    // ReSharper disable once InvocationIsSkipped
                    Trace.Log(
                        "Thrown object has been destroyed, no position to fall to was found below {0}", droppingPoint);
                }
            }

            // Possible condition interruption, after the attack is done. Example: if an invisible character performs an attack, his invisible condition is broken
            actingCharacter.RulesetCharacter.ProcessConditionsMatchingInterruption(
                ConditionInterruption.Attacks);

            if (actingCharacter.RulesetCharacter.IsWieldingBow())
            {
                actingCharacter.RulesetCharacter.ProcessConditionsMatchingInterruption(
                    ConditionInterruption.AttacksWithBow);
            }

            // Handle specific reactions after the attack has been executed
            yield return battleService.HandleCharacterPhysicalAttackFinished(
                __instance, actingCharacter,
                target, attackParams.attackMode, __instance.AttackRollOutcome, damageReceived);

            yield return battleService.HandleCharacterAttackFinished(
                __instance, actingCharacter, target,
                attackParams.attackMode, null, __instance.AttackRollOutcome, damageReceived);

            // BEGIN PATCH

            //PATCH: support for Sentinel Fighting Style - allows attacks of opportunity on enemies attacking allies
            yield return AttacksOfOpportunity.ProcessOnCharacterAttackFinished(
                battleService as GameLocationBattleManager, actingCharacter, target);

            //PATCH: support for Defensive Strike Power - allows adding Charisma modifier and chain reactions
            yield return DefensiveStrikeAttack.ProcessOnCharacterAttackFinished(
                battleService as GameLocationBattleManager, actingCharacter, target);

            //PATCH: support for Aura of the Guardian power - allows swapping hp on enemy attacking ally
            yield return GuardianAura.ProcessOnCharacterAttackHitFinished(
                battleService as GameLocationBattleManager, actingCharacter, target, attackMode, null, damageReceived);

            // END PATCH

            if (attackHasDamaged)
            {
                actingCharacter.RulesetCharacter.ProcessConditionsMatchingInterruption(
                    ConditionInterruption.AttacksAndDamages, damageReceived);
            }

            // Did I down the target?
            if (defenderWasConscious && target.RulesetActor.IsDeadOrDyingOrUnconscious)
            {
                actingCharacter.EnemiesDownedByAttack++;
            }

            if (needToRefresh)
            {
                actingCharacter.RulesetCharacter.RefreshAttackModes();
            }

            if (!isResultingActionSpendPowerWithMotionForm)
            {
                yield return actingCharacter.EventSystem.WaitForEvent(
                    GameLocationCharacterEventSystem.Event.AttackAnimationEnd);
            }
            else
            {
                actingCharacter.EventSystem.AbsorbNextEvent(
                    GameLocationCharacterEventSystem.Event.AttackAnimationEnd);
            }

            if (!actingCharacter.RulesetActor.IsDeadOrDyingOrUnconscious &&
                !multiAttackInProgress)
            {
                if (!isResultingActionSpendPowerWithMotionForm)
                {
                    actingCharacter.TurnTowards(target);
                    actingCharacterTurnCoroutine = Coroutine.StartCoroutine(
                        actingCharacter.EventSystem.UpdateMotionsAndWaitForEvent(
                            GameLocationCharacterEventSystem.Event.RotationEnd));
                }
            }
            else if (resultingActionMotionForm != null)
            {
                resultingActionMotionForm.ForceSourceCharacterTurnTowardsTargetAfterPush = false;
            }

            if (isTargetAware &&
                target.RulesetCharacter is null or { IsDeadOrDyingOrUnconscious: false } &&
                !rangeAttack &&
                !target.Prone &&
                !target.IsCharging &&
                !target.MoveStepInProgress &&
                !multiAttackInProgress)
            {
                if (!isResultingActionSpendPowerWithMotionForm)
                {
                    target.TurnTowards(actingCharacter);
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
                actingCharacter.IsCharging = false;
                actingCharacter.MovingToDestination = false;
            }

            target.RulesetActor.ProcessConditionsMatchingInterruption(
                ConditionInterruption.PhysicalAttackReceivedExecuted);

            yield return battleService.HandleCharacterAttackOrMagicEffectFinishedLate(
                __instance, actingCharacter);
        }
    }
}
