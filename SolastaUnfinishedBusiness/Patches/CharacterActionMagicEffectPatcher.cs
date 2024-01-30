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

            var service = ServiceRepository.GetService<IRulesetImplementationService>();
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

            service.ApplyEffectForms(effectDescription.EffectForms,
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

    [HarmonyPatch(typeof(CharacterActionMagicEffect), nameof(CharacterActionMagicEffect.ExecuteImpl))]
    [UsedImplicitly]
    public static class ExecuteImpl_Patch
    {
        [UsedImplicitly]
        public static void Prefix([NotNull] CharacterActionMagicEffect __instance)
        {
            var definition = __instance.GetBaseDefinition();
            var actingCharacter = __instance.ActingCharacter;
            var actionParams = __instance.ActionParams;

            //PATCH: skip spell animation if this is "attack after cast" spell
            if (definition.HasSubFeatureOfType<IAttackAfterMagicEffect>())
            {
                actionParams.SkipAnimationsAndVFX = true;
            }

            //PATCH: support for Altruistic metamagic - add caster as first target if necessary
            var effect = actionParams.RulesetEffect;
            var baseEffectDescription = (effect.SourceDefinition as IMagicEffect)?.EffectDescription;
            var effectDescription = effect.EffectDescription;
            var targets = actionParams.TargetCharacters;

            if (!effectDescription.InviteOptionalAlly
                || baseEffectDescription?.TargetType != TargetType.Self
                || targets.Count <= 0
                || targets[0] == actingCharacter)
            {
                return;
            }

            targets.Insert(0, actingCharacter);
            actionParams.ActionModifiers.Insert(0, actionParams.ActionModifiers[0]);
        }

        [UsedImplicitly]
        public static IEnumerator Postfix(
            [NotNull] IEnumerator values,
            CharacterActionMagicEffect __instance)
        {
            var baseDefinition = __instance.GetBaseDefinition();

            //PATCH: supports `IMagicEffectInitiatedByMe`
            // no need to check for gui.battle != null
            var magicEffectInitiatedByMe = baseDefinition.GetFirstSubFeatureOfType<IMagicEffectInitiatedByMe>();

            if (magicEffectInitiatedByMe != null)
            {
                yield return magicEffectInitiatedByMe.OnMagicEffectInitiatedByMe(__instance, baseDefinition);
            }

            // VANILLA EVENTS
            while (values.MoveNext())
            {
                yield return values.Current;
            }

            //PATCH: supports `IPerformAttackAfterMagicEffectUse`
            // no need to check for gui.battle != null
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
            // no need to check for gui.battle != null
            var magicEffectFinishedByMe = baseDefinition.GetFirstSubFeatureOfType<IMagicEffectFinishedByMe>();

            if (magicEffectFinishedByMe != null)
            {
                yield return magicEffectFinishedByMe.OnMagicEffectFinishedByMe(__instance, baseDefinition);
            }
        }

        // FULL VANILLA CODE FOR REFERENCE
        private static IEnumerator ExecuteImpl(CharacterActionMagicEffect __instance)
        {
            var actingCharacter = __instance.ActingCharacter;
            var actionParams = __instance.ActionParams;

            if (actionParams == null)
            {
                Trace.LogException(new Exception(
                    "[TACTICAL INVISIBLE FOR PLAYERS] null ActionParams in CharacterActionMagicEffect.ExecuteImpl()."));
            }
            else
            {
                var rulesetEffect = actionParams.RulesetEffect;

                if (rulesetEffect == null)
                {
                    Trace.LogException(new Exception(
                        "[TACTICAL INVISIBLE FOR PLAYERS] null RulesetEffect in CharacterActionMagicEffect.ExecuteImpl()."));
                }
                else if (rulesetEffect.EntityImplementation is not GameLocationEffect)
                {
                    Trace.LogError($"Error Context : {rulesetEffect}");
                    Trace.LogException(new Exception(
                        "[TACTICAL INVISIBLE FOR PLAYERS] null GameLocationEffect in CharacterActionMagicEffect.ExecuteImpl()."));
                }
                else
                {
                    yield return actingCharacter.WaitForHitAnimation();

                    __instance.Countered = false;
                    __instance.ExecutionFailed = false;
                    __instance.immuneTargets.Clear();
                    __instance.hitTargets.Clear();
                    __instance.showCasting = !actionParams.SkipAnimationsAndVFX;
                    __instance.needToWaitCastAnimation = false;

                    var battleService = ServiceRepository.GetService<IGameLocationBattleService>();
                    var implementationService = ServiceRepository.GetService<IRulesetImplementationService>();
                    var targetingService = ServiceRepository.GetService<IGameLocationTargetingService>();
                    var positioningService = ServiceRepository.GetService<IGameLocationPositioningService>();

                    var targets = actionParams.TargetCharacters;
                    var targetPositions = actionParams.Positions;
                    var actionModifiers = actionParams.ActionModifiers;
                    var effectDescription = actionParams.RulesetEffect.EffectDescription;

                    implementationService.ClearDamageFormsByIndex();

                    __instance.TargetItem = actionParams.TargetItem;
                    __instance.actualEffectForms = [];

                    var locationPosition1 = actingCharacter.LocationPosition;
                    var locationPosition2 = actionParams.Positions.Count > 0
                        ? actionParams.Positions[0]
                        : locationPosition1;
                    var locationPosition3 = actionParams.Positions.Count > 1
                        ? actionParams.Positions[1]
                        : int3.zero;

                    var isUsingFloatingImpactPoint = IsUsingFloatingImpactPoint(
                        effectDescription.RangeType, effectDescription.TargetType);
                    var impactPoint = !actionParams.HasMagneticTargeting & isUsingFloatingImpactPoint
                        ? actionParams.CursorHoveredPosition
                        : positioningService.GetWorldPositionFromGridPosition(locationPosition2);
                    var fromGridPosition = positioningService.GetWorldPositionFromGridPosition(locationPosition2);
                    var castingPoint = positioningService.GetWorldPositionFromGridPosition(locationPosition1);
                    var impactPlanePoint = positioningService.GetImpactPlanePosition(impactPoint);

                    if (actionParams.RulesetEffect.EntityImplementation is GameLocationEffect entityImplementation)
                    {
                        entityImplementation.Position = locationPosition2;
                        entityImplementation.Position2 = locationPosition3;
                        entityImplementation.HasMagneticTargeting = actionParams.HasMagneticTargeting;

                        if (actionParams.RulesetEffect.EffectDescription.HasFormOfType(
                                EffectForm.EffectFormType.Motion) &&
                            actionParams.RulesetEffect.EffectDescription
                                .GetFirstFormOfType(EffectForm.EffectFormType.Motion).MotionForm.Type ==
                            MotionForm.MotionType.Telekinesis && actionParams.Positions.Count > 0)
                        {
                            entityImplementation.Position = actionParams.Positions[0];
                        }

                        entityImplementation.SourceOriginalPosition =
                            actingCharacter.LocationPosition;
                    }

                    var origin = new Vector3();
                    var direction = new Vector3();
                    var shapeType = effectDescription.ShapeType;

                    if (actingCharacter is { RulesetActor: RulesetCharacterHero } &&
                        actionParams.RulesetEffect.OriginItem != null)
                    {
                        var slotHoldingItem =
                            (actingCharacter.RulesetActor as RulesetCharacterHero)?.CharacterInventory
                            .FindSlotHoldingItem(actionParams.RulesetEffect.OriginItem);

                        if (slotHoldingItem != null && !slotHoldingItem.SlotTypeDefinition.BodySlot)
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
                        var vector3 = new Vector3();
                        var targetParameter = actionParams.RulesetEffect.EffectDescription
                            .TargetParameter;

                        if (actionParams.HasMagneticTargeting)
                        {
                            if (targetParameter % 2 == 0)
                            {
                                vector3 = new Vector3(0.5f, 0.5f, 0.5f);
                            }
                        }
                        else
                        {
                            vector3 = new Vector3(0.0f, (float)((0.5 * targetParameter) - 0.5), 0.0f);
                            if (targetParameter % 2 == 0)
                            {
                                vector3 += new Vector3(0.5f, 0.0f, 0.5f);
                            }
                        }

                        impactPoint += vector3;
                    }

                    var locationCharacterList = new List<GameLocationCharacter>();
                    var int3List = new List<int3>();

                    __instance.computedTargetParameter =
                        actionParams.RulesetEffect.ComputeTargetParameter();

                    if (effectDescription.TargetType == TargetType.PerceivingWithinDistance)
                    {
                        targetingService.CollectPerceivingTargetsWithinDistance(actingCharacter,
                            effectDescription, locationCharacterList, actionModifiers, int3List);
                    }
                    else
                    {
                        targetingService.ComputeTargetsOfAreaOfEffect(
                            origin,
                            direction,
                            fromGridPosition,
                            shapeType,
                            actingCharacter.Side,
                            effectDescription,
                            __instance.computedTargetParameter,
                            actionParams.RulesetEffect.ComputeTargetParameter2(),
                            locationCharacterList, actionParams.HasMagneticTargeting,
                            actingCharacter,
                            int3List,
                            groundOnly: effectDescription.AffectOnlyGround);
                    }

                    if (targets.Count == 1)
                    {
                        targetingService.ComputeAndSortSubtargets(
                            actionParams.ActingCharacter, actionParams.RulesetEffect, targets[0],
                            __instance.subTargets);

                        if (!__instance.subTargets.Empty())
                        {
                            targets.AddRange(__instance.subTargets);
                            actionModifiers.AddRange(__instance.subTargets.Select(_ => new ActionModifier()));
                        }
                    }

                    actionModifiers.AddRange(locationCharacterList
                        .Where(locationCharacter => targets.TryAdd(locationCharacter))
                        .Select(_ => new ActionModifier()));

                    __instance.SpendMagicEffectUses();
                    __instance.CheckInterruptionBefore();

                    yield return __instance.WaitSpellCastAction(battleService);

                    if (__instance.Countered)
                    {
                        actionParams.RulesetEffect.Terminate(false);
                    }
                    else
                    {
                        yield return __instance.CheckExecutionFailure();

                        if (__instance.ExecutionFailed)
                        {
                            actionParams.RulesetEffect.Terminate(false);
                        }
                        else
                        {
                            __instance.RemoveConcentrationAsNeeded();

                            yield return __instance.HandleSpecialCastingTime();

                            __instance.HandleEffectUniqueness();
                            __instance.GetAdvancementData();

                            yield return __instance.CounterEffectAction(__instance);

                            __instance.ApplyTargetFiltering(
                                effectDescription, targets, __instance.GetBaseDefinition());

                            if (effectDescription.TargetType == TargetType.ClosestWithinDistance)
                            {
                                targetingService.FilterClosestTargets(
                                    (Vector3Int)actingCharacter.LocationPosition, targets);
                            }

                            if (effectDescription.TargetType == TargetType.Position ||
                                effectDescription.HasEffectProxy)
                            {
                                yield return __instance.MagicEffectExecuteOnPositions(
                                    targetPositions, castingPoint, impactPoint, impactPlanePoint);

                                __instance.ShowCasting = false;
                            }

                            if (effectDescription.TargetType != TargetType.Position ||
                                effectDescription.HasEffectProxy)
                            {
                                if (IsTargeted(effectDescription.TargetType))
                                {
                                    yield return __instance.MagicEffectExecuteOnTargets(targets, actionModifiers,
                                        castingPoint, impactPoint, impactPlanePoint, origin, direction);
                                }
                                else
                                {
                                    yield return __instance.MagicEffectExecuteOnZone(targets, actionModifiers,
                                        castingPoint, impactPoint, impactPlanePoint, origin, direction);
                                }
                            }

                            foreach (var locationCharacter in targets)
                            {
                                locationCharacter.WillBePushedByMagicalEffect = false;
                            }

                            __instance.PersistantEffectAction();

                            var hasDamageForm = effectDescription.HasDamageForm();

                            if (hasDamageForm &&
                                __instance.ActionId != ActionDefinitions.Id.CastReaction &&
                                __instance.ActionId != ActionDefinitions.Id.PowerReaction)
                            {
                                foreach (var locationCharacter in targets
                                             .Where(x =>
                                                 !__instance.immuneTargets.Contains(x) &&
                                                 __instance.hitTargets.Contains(x) &&
                                                 x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                                                 !x.Prone))
                                {
                                    if (!__instance.isResultingActionSpendPowerWithMotionForm &&
                                        !locationCharacter.RulesetCharacter.IsDeadOrDying)
                                    {
                                        yield return locationCharacter.WaitForHitAnimation();
                                    }

                                    __instance.hitTargets.Remove(locationCharacter);
                                }
                            }

                            for (var i = 0; i < targets.Count; ++i)
                            {
                                var defender = targets[i];

                                if (!__instance.damagePerTargetIndexCache.TryGetValue(i, out var damageAmount))
                                {
                                    damageAmount = 0;
                                }

                                yield return battleService.HandleCharacterAttackFinished(
                                    __instance,
                                    actingCharacter,
                                    defender,
                                    null,
                                    actionParams.RulesetEffect,
                                    __instance.AttackRollOutcome,
                                    damageAmount);
                            }

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

                            __instance.CheckInterruptionAfter();

                            if (castingPoint != impactPoint)
                            {
                                var rulesetCharacter =
                                    actingCharacter.RulesetCharacter as RulesetCharacterEffectProxy;

                                if ((__instance.ActionId == ActionDefinitions.Id.CastReadied ||
                                     __instance.ActionId == ActionDefinitions.Id.AttackReadied ||
                                     __instance.ActionType != ActionDefinitions.ActionType.Reaction) &&
                                    (rulesetCharacter == null || rulesetCharacter.EffectProxyDefinition.CanMove))
                                {
                                    actingCharacter.TurnTowards(impactPoint);

                                    yield return actingCharacter.EventSystem
                                        .UpdateMotionsAndWaitForEvent(
                                            GameLocationCharacterEventSystem.Event.RotationEnd);
                                }
                            }

                            var rangeAttack =
                                effectDescription.RangeType != RangeType.MeleeHit &&
                                effectDescription.RangeType != RangeType.Touch;

                            if (!rangeAttack)
                            {
                                foreach (var locationCharacter in targets
                                             .Where(x =>
                                                 x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                                                 x != actingCharacter &&
                                                 !x.Prone &&
                                                 !x.MoveStepInProgress &&
                                                 !x.IsCharging &&
                                                 (x.PerceivedAllies.Contains(actingCharacter) ||
                                                  x.PerceivedFoes.Contains(actingCharacter))))
                                {
                                    locationCharacter.TurnTowards(actingCharacter);

                                    yield return locationCharacter.EventSystem.UpdateMotionsAndWaitForEvent(
                                        GameLocationCharacterEventSystem.Event.RotationEnd);
                                }
                            }

                            __instance.StartConcentrationAsNeeded();
                            implementationService.ClearDamageFormsByIndex();

                            if (__instance.isPostSpecialMove && battleService.IsBattleInProgress)
                            {
                                yield return battleService.HandleCharacterMoveEnd(actingCharacter);
                            }

                            yield return __instance.HandlePostExecution();
                            yield return battleService.HandleCharacterAttackOrMagicEffectFinishedLate(
                                __instance, actingCharacter);
                        }
                    }
                }
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

            var attacker = __instance.ActingCharacter;

            //PATCH: support for `IMagicalAttackFinishedByMe`
            // no need to check for gui.battle != null
            foreach (var magicalAttackFinishedByMe in attacker.RulesetCharacter
                         .GetSubFeaturesByType<IMagicalAttackFinishedByMe>())
            {
                yield return magicalAttackFinishedByMe.OnMagicalAttackFinishedByMe(__instance, attacker, target);
            }

            //PATCH: support for `IMagicalAttackFinishedByMeOrAlly`
            // should also happen outside battles
            var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var contenders =
                (Gui.Battle?.AllContenders ??
                 locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters))
                .ToList();

            foreach (var ally in contenders
                         .Where(x => x.Side == attacker.Side
                                     && x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false }))
            {
                foreach (var magicalAttackFinishedByMeOrAlly in ally.RulesetCharacter
                             .GetSubFeaturesByType<IMagicalAttackFinishedByMeOrAlly>())
                {
                    yield return magicalAttackFinishedByMeOrAlly
                        .OnMagicalAttackFinishedByMeOrAlly(__instance, attacker, target, ally);
                }
            }
        }
    }
}
