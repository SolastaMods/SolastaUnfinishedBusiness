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
using SolastaUnfinishedBusiness.Interfaces;
using TA;
using UnityEngine;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterActionMagicEffectPatcher
{
    [HarmonyPatch(typeof(CharacterActionMagicEffect), nameof(CharacterActionMagicEffect.ExecuteImpl))]
    [UsedImplicitly]
    public static class ExecuteImpl_Patch
    {
        [UsedImplicitly]
        public static IEnumerator Postfix(
#pragma warning disable IDE0060
            IEnumerator values,
#pragma warning restore IDE0060
            CharacterActionMagicEffect __instance)
        {
            yield return ExecuteImpl(__instance);
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

            //PATCH: skip spell animation if this is "attack after cast" spell
            if (baseDefinition.HasSubFeatureOfType<IAttackAfterMagicEffect>())
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

            //PATCH: supports `IMagicEffectInitiatedByMe`
            var magicEffectInitiatedByMe = baseDefinition.GetFirstSubFeatureOfType<IMagicEffectInitiatedByMe>();

            if (magicEffectInitiatedByMe != null)
            {
                yield return magicEffectInitiatedByMe.OnMagicEffectInitiatedByMe(__instance, baseDefinition);
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

            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();
            var rulesetService = ServiceRepository.GetService<IRulesetImplementationService>();
            var targetingService = ServiceRepository.GetService<IGameLocationTargetingService>();
            var gameLocationPositioningService = ServiceRepository.GetService<IGameLocationPositioningService>();

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
                : gameLocationPositioningService.GetWorldPositionFromGridPosition(impactGridPoint);
            var lineOrigin = gameLocationPositioningService.GetWorldPositionFromGridPosition(impactGridPoint);
            var castingPoint = gameLocationPositioningService.GetWorldPositionFromGridPosition(castingGridPoint);
            var impactPlanePoint = gameLocationPositioningService.GetImpactPlanePosition(impactPoint);

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
            if (actingCharacter is { RulesetActor: RulesetCharacterHero hero } &&
                actionParams.RulesetEffect.OriginItem != null)
            {
                var slot = hero.CharacterInventory.FindSlotHoldingItem(actionParams.RulesetEffect.OriginItem);

                if (slot != null && !slot.SlotTypeDefinition.BodySlot)
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

            if (effectDescription.TargetType == TargetType.Cube && effectDescription.RangeType == RangeType.Distance)
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

            __instance.SpendMagicEffectUses();

            // This is used to remove invisibility (for example) when casting a spell
            __instance.CheckInterruptionBefore();

            // Handle spell countering
            yield return __instance.WaitSpellCastAction(battleService);

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
                    if (!__instance.isResultingActionSpendPowerWithMotionForm && !target.RulesetCharacter.IsDeadOrDying)
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
                yield return battleService.HandleCharacterAttackFinished(
                    __instance, actingCharacter, target, null, actionParams.RulesetEffect, __instance.AttackRollOutcome,
                    damageReceived);
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
                         target.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
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
                if (battleService.IsBattleInProgress)
                {
                    yield return battleService.HandleCharacterMoveEnd(actingCharacter);
                }
            }

            yield return __instance.HandlePostExecution();

            yield return battleService.HandleCharacterAttackOrMagicEffectFinishedLate(__instance, actingCharacter);

            // BEGIN PATCH

            //PATCH: supports `IPerformAttackAfterMagicEffectUse`
            var attackAfterMagicEffect = baseDefinition.GetFirstSubFeatureOfType<IAttackAfterMagicEffect>();

            if (attackAfterMagicEffect != null)
            {
                var performAttackAfterUse = attackAfterMagicEffect.PerformAttackAfterUse;
                var characterActionAttacks = performAttackAfterUse?.Invoke(__instance);

                if (characterActionAttacks != null)
                {
                    __instance.ResultingActions.AddRange(
                        characterActionAttacks.Select(attackParams => new CharacterActionAttack(attackParams)));
                }
            }

            //PATCH: supports `IMagicEffectFinishedByMe`
            var magicEffectFinishedByMe = baseDefinition.GetFirstSubFeatureOfType<IMagicEffectFinishedByMe>();

            if (magicEffectFinishedByMe != null)
            {
                yield return magicEffectFinishedByMe.OnMagicEffectFinishedByMe(__instance, baseDefinition);
            }

            // END PATCH
        }
    }

    [HarmonyPatch(typeof(CharacterActionMagicEffect), nameof(CharacterActionMagicEffect.ExecuteMagicAttack))]
    [UsedImplicitly]
    public static class ExecuteMagicAttack_Patch
    {
        [UsedImplicitly]
        public static IEnumerator Postfix(
#pragma warning disable IDE0060
            IEnumerator values,
#pragma warning restore IDE0060
            CharacterActionMagicEffect __instance,
            RulesetEffect activeEffect,
            GameLocationCharacter target,
            ActionModifier attackModifier,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool checkMagicalAttackDamage)
        {
            yield return ExecuteMagicAttack(__instance,
                activeEffect, target, attackModifier, actualEffectForms, firstTarget, checkMagicalAttackDamage);
        }

        private static IEnumerator ExecuteMagicAttack(
            CharacterActionMagicEffect __instance,
            RulesetEffect activeEffect,
            GameLocationCharacter target,
            ActionModifier attackModifier,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool checkMagicalAttackDamage)
        {
            var actingCharacter = __instance.ActingCharacter;
            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();
            var effectDescription = activeEffect.EffectDescription;

            __instance.AttackRollOutcome = RollOutcome.Success;

            var needToRollDie = effectDescription.NeedsToRollDie();
            var hasSavingThrowAnimation = !effectDescription.HideSavingThrowAnimation && !needToRollDie;

            if (needToRollDie)
            {
                // Roll dice + handle target reaction
                __instance.AttackRoll = actingCharacter.RulesetCharacter.RollMagicAttack(
                    activeEffect,
                    target.RulesetActor,
                    activeEffect.GetEffectSource(),
                    attackModifier.AttacktoHitTrends,
                    attackModifier.AttackAdvantageTrends,
                    false,
                    attackModifier.AttackRollModifier,
                    out var outcome,
                    out var successDelta,
                    -1,
                    true);
                __instance.AttackRollOutcome = outcome;
                __instance.AttackSuccessDelta = successDelta;

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
                foreach (var extraEvents in actingCharacter.RulesetCharacter
                             .GetSubFeaturesByType<ITryAlterOutcomePhysicalAttack>()
                             .TakeWhile(_ =>
                                 __instance.AttackRollOutcome == RollOutcome.Failure &&
                                 __instance.AttackSuccessDelta < 0)
                             .Select(feature =>
                                 feature.OnAttackTryAlterOutcome(battleService as GameLocationBattleManager, __instance,
                                     actingCharacter, target, attackModifier)))
                {
                    while (extraEvents.MoveNext())
                    {
                        yield return extraEvents.Current;
                    }
                }

                // END PATCH

                __instance.isResultingActionSpendPowerWithMotionForm = false;

                // Is this a success?
                if (__instance.AttackRollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
                {
                    // No need to test this even if the hit was automatic (natural 20). Warning: Critical Success can occur without rolling a 20 (Champion fighter crits at 19)
                    if (__instance.AttackRoll != DiceMaxValue[(int)DieType.D20])
                    {
                        // Can the target do anything to change the outcome of the hit?
                        yield return battleService.HandleCharacterAttackHitPossible(
                            actingCharacter,
                            target,
                            null,
                            activeEffect,
                            attackModifier,
                            __instance.AttackRoll,
                            __instance.AttackSuccessDelta,
                            effectDescription.RangeType == RangeType.RangeHit);
                    }

                    // Execute the final step of the attack
                    actingCharacter.RulesetCharacter.RollMagicAttack(
                        activeEffect, target.RulesetActor,
                        activeEffect.GetEffectSource(),
                        attackModifier.AttacktoHitTrends,
                        attackModifier.AttackAdvantageTrends,
                        false,
                        attackModifier.AttackRollModifier,
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
                            yield return battleService.HandleCharacterMagicalAttackHitConfirmed(
                                __instance,
                                actingCharacter,
                                target,
                                attackModifier,
                                activeEffect,
                                actualEffectForms,
                                firstTarget,
                                __instance.AttackRollOutcome == RollOutcome.CriticalSuccess);
                        }
                    }
                }
                else
                {
                    actingCharacter.RulesetCharacter.RollMagicAttack(
                        activeEffect, target.RulesetActor,
                        activeEffect.GetEffectSource(),
                        attackModifier.AttacktoHitTrends,
                        attackModifier.AttackAdvantageTrends,
                        false, attackModifier.AttackRollModifier,
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
                    yield return battleService.HandleCharacterMagicalAttackHitConfirmed(
                        __instance,
                        actingCharacter,
                        target,
                        attackModifier,
                        activeEffect,
                        actualEffectForms,
                        firstTarget,
                        false);
                }
            }

            // No need to roll the saving throw if the attack missed
            if (!needToRollDie ||
                __instance.AttackRollOutcome == RollOutcome.Success ||
                __instance.AttackRollOutcome == RollOutcome.CriticalSuccess ||
                activeEffect.EffectDescription.HalfDamageOnAMiss)
            {
                // Roll the saving throw, if it is the right time
                if (activeEffect.EffectDescription.RecurrentEffect == RecurrentEffect.No ||
                    (activeEffect.EffectDescription.RecurrentEffect & RecurrentEffect.OnActivation) != 0)
                {
                    var hasBorrowedLuck = target.RulesetActor.HasConditionOfTypeOrSubType(ConditionBorrowedLuck);
                    var side = actingCharacter?.Side ?? Side.Neutral;

                    __instance.RolledSaveThrow = activeEffect.TryRollSavingThrow(
                        actingCharacter?.RulesetCharacter,
                        side,
                        target.RulesetActor,
                        attackModifier,
                        actualEffectForms,
                        hasSavingThrowAnimation,
                        out var saveOutcome,
                        out var saveOutcomeDelta);
                    __instance.SaveOutcome = saveOutcome;
                    __instance.SaveOutcomeDelta = saveOutcomeDelta;

                    target.RulesetActor?.GrantConditionOnSavingThrowOutcome(
                        activeEffect.EffectDescription, saveOutcome, false);

                    // Legendary Resistance or Indomitable?
                    if (__instance.RolledSaveThrow && __instance.SaveOutcome == RollOutcome.Failure)
                    {
                        yield return battleService.HandleFailedSavingThrow(
                            __instance,
                            actingCharacter,
                            target,
                            attackModifier,
                            !needToRollDie,
                            hasBorrowedLuck);
                    }

                    // BEGIN PATCH

                    //PATCH: support for `ITryAlterOutcomeSavingThrow`
                    // should also happen outside battles
                    var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
                    var contenders =
                        (Gui.Battle?.AllContenders ??
                         locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters))
                        .ToList();

                    foreach (var unit in contenders
                                 .Where(u => u.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false }))
                    {
                        foreach (var feature in unit.RulesetCharacter
                                     .GetSubFeaturesByType<ITryAlterOutcomeSavingThrow>())
                        {
                            yield return feature.OnSavingThrowTryAlterOutcome(
                                battleService as GameLocationBattleManager, __instance, actingCharacter,
                                target,
                                unit, attackModifier, false, hasBorrowedLuck);
                        }
                    }

                    // END PATCH
                }
            }

            if (!__instance.RolledSaveThrow && activeEffect.EffectDescription.HasShoveRoll)
            {
                __instance.successfulShove =
                    CharacterActionShove.ResolveRolls(actingCharacter, target, ActionDefinitions.Id.Shove);
            }
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

            if (!effectDescription.HasForceSelfCondition)
            {
                return true;
            }

            var implementationService = ServiceRepository.GetService<IRulesetImplementationService>();

            var formsParams = new RulesetImplementationDefinitions.ApplyFormsParams();
            var effectLevel = 0;
            var effectSourceType = EffectSourceType.Power;

            switch (__instance)
            {
                case CharacterActionCastSpell spell:
                    effectSourceType = EffectSourceType.Spell;
                    effectLevel = spell.ActiveSpell.SlotLevel;
                    break;
                case CharacterActionUsePower power:
                    effectLevel = power.activePower.EffectLevel;
                    break;
            }

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

    [HarmonyPatch(typeof(CharacterActionMagicEffect),
        nameof(CharacterActionMagicEffect.HandlePostApplyMagicEffectOnZoneOrTargets))]
    [UsedImplicitly]
    public static class HandlePostApplyMagicEffectOnZoneOrTargets_Patch
    {
        [UsedImplicitly]
        public static IEnumerator Postfix(
            [NotNull] IEnumerator values,
            CharacterActionMagicEffect __instance,
            GameLocationCharacter target)
        {
            while (values.MoveNext())
            {
                yield return values.Current;
            }

            var actingCharacter = __instance.ActingCharacter;

            //PATCH: support for `IMagicalAttackFinishedByMe`
            foreach (var magicalAttackFinishedByMe in actingCharacter.RulesetCharacter
                         .GetSubFeaturesByType<IMagicEffectFinishedByMeAny>())
            {
                yield return
                    magicalAttackFinishedByMe.OnMagicEffectFinishedByMeAny(__instance, actingCharacter, target);
            }

            //PATCH: support for `IMagicalAttackFinishedByMeOrAlly`
            var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var contenders =
                (Gui.Battle?.AllContenders ??
                 locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters))
                .ToList();

            foreach (var ally in contenders
                         .Where(x => x.Side == actingCharacter.Side
                                     && x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false }))
            {
                foreach (var magicalAttackFinishedByMeOrAlly in ally.RulesetCharacter
                             .GetSubFeaturesByType<IMagicEffectFinishedByMeOrAllyAny>())
                {
                    yield return magicalAttackFinishedByMeOrAlly
                        .OnMagicEffectFinishedByMeOrAllyAny(__instance, actingCharacter, target, ally);
                }
            }
        }
    }
}
