using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Interfaces;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterActionSpendPowerPatcher
{
    [HarmonyPatch(typeof(CharacterActionSpendPower), nameof(CharacterActionSpendPower.ExecuteImpl))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ExecuteImpl_Patch
    {
        private static IEnumerator HandlePostApplyMagicEffectOnZoneOrTargets(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter target,
            RulesetEffect effect,
            bool targetWasDeadOrDyingOrUnconscious,
            int damageReceived,
            bool damageAbsorbedByTemporaryHitPoints,
            List<string> effectiveDamageTypes)
        {
            if (damageReceived <= 0 && !damageAbsorbedByTemporaryHitPoints)
            {
                yield break;
            }

            yield return battleManager.HandleDefenderOnDamageReceived(
                attacker, target, damageReceived, effect, effectiveDamageTypes);
            yield return battleManager.HandleAttackerOnDefenderDamageReceived(
                attacker, target, damageReceived, effect, effectiveDamageTypes);

            if (!damageAbsorbedByTemporaryHitPoints)
            {
                yield return battleManager.HandleReactionToDamageShare(target, damageReceived);
            }

            if (!targetWasDeadOrDyingOrUnconscious && target.RulesetActor.IsDeadOrDyingOrUnconscious)
            {
                yield return battleManager.HandleTargetReducedToZeroHP(attacker, target, null, effect);
            }
        }

        [UsedImplicitly]
        public static bool Prefix(
            out IEnumerator __result,
            CharacterActionSpendPower __instance)
        {
            __result = ExecuteImpl(__instance);

            return false;
        }

        private static IEnumerator ExecuteImpl(CharacterActionSpendPower __instance)
        {
            var battleManager =
                ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;
            var actingCharacter = __instance.ActingCharacter;
            var actionParams = __instance.ActionParams;
            var rulesetEffect = actionParams.RulesetEffect;
            var effectDescription = rulesetEffect.EffectDescription;
            var targets = actionParams.TargetCharacters;
            var implementationService = ServiceRepository.GetService<IRulesetImplementationService>();
            var activePower = __instance.ActionParams.RulesetEffect as RulesetEffectPower;

            // Retrieve the target
            __instance.targets.SetRange(targets);

            // Create an active power
            __instance.activePower = activePower;

            //PATCH: supports `IMagicEffectInitiatedByMe`
            foreach (var magicEffectInitiatedByMe in actingCharacter.RulesetCharacter
                         .GetSubFeaturesByType<IMagicEffectInitiatedByMe>())
            {
                yield return magicEffectInitiatedByMe.OnMagicEffectInitiatedByMe(
                    __instance, rulesetEffect, actingCharacter, targets);
            }

            var actionModifier = new ActionModifier();

            // BEGIN PATCH
            PowerBundle.SpendBundledPowerIfNeeded(__instance);

            // END PATCH

            // Spend the power, if this does not come from an item
            if (activePower is { OriginItem: null })
            {
                // Fire shield retaliation has no class or race origin
                if (activePower.UsablePower.OriginClass)
                {
                    actingCharacter.RulesetCharacter.UsePower(activePower.UsablePower);
                }
                else if (activePower.UsablePower.OriginRace)
                {
                    actingCharacter.RulesetCharacter.UsePower(activePower.UsablePower);
                }

                // BEGIN PATCH

                //PATCH: support for shared pool powers that character got from conditions to properly consume uses when triggered
                else if (activePower.UsablePower.powerDefinition.HasSubFeatureOfType<ForcePowerUseInSpendPowerAction>())
                {
                    __instance.ActingCharacter.RulesetCharacter.UsePower(activePower.UsablePower);
                }

                // END PATCH
            }
            else if (activePower != null)
            {
                actingCharacter.RulesetCharacter.UseDevicePower(activePower.OriginItem, activePower.PowerDefinition);
            }

            actingCharacter.RulesetCharacter.ProcessConditionsMatchingInterruption(
                (RuleDefinitions.ConditionInterruption)ExtraConditionInterruption.SpendPower);

            for (var i = 0; i < targets.Count; i++)
            {
                var target = targets[i];

                // These bool information must be store as a class member, as it is passed to HandleFailedSavingThrow
                var hasBorrowedLuck =
                    target.RulesetActor.HasConditionOfTypeOrSubType(RuleDefinitions.ConditionBorrowedLuck);

                if (activePower != null)
                {
                    __instance.RolledSaveThrow = activePower.TryRollSavingThrow(
                        actingCharacter.RulesetCharacter,
                        actingCharacter.Side,
                        target.RulesetActor,
                        actionModifier,
                        activePower.EffectDescription.EffectForms,
                        false,
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

                    // Apply the forms of the power
                    var applyFormsParams = new RulesetImplementationDefinitions.ApplyFormsParams();

                    applyFormsParams.FillSourceAndTarget(actingCharacter.RulesetCharacter, target.RulesetCharacter);
                    applyFormsParams.FillFromActiveEffect(activePower);
                    applyFormsParams.FillSpecialParameters(
                        __instance.RolledSaveThrow,
                        0, 0, 0, 0,
                        actionModifier,
                        __instance.SaveOutcome,
                        __instance.SaveOutcomeDelta,
                        false,
                        0,
                        1,
                        null);
                    applyFormsParams.effectSourceType = RuleDefinitions.EffectSourceType.Power;

                    // BEGIN PATCH

                    var effectForms = effectDescription.EffectForms.DeepCopy();

                    //PATCH: support for `IMagicEffectBeforeHitConfirmedOnEnemy`
                    // should also happen outside battles
                    if (actingCharacter.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
                    {
                        var controller = actingCharacter.GetEffectControllerOrSelf();

                        foreach (var magicalAttackBeforeHitConfirmedOnEnemy in controller.RulesetCharacter
                                     .GetSubFeaturesByType<IMagicEffectBeforeHitConfirmedOnEnemy>())
                        {
                            yield return magicalAttackBeforeHitConfirmedOnEnemy.OnMagicEffectBeforeHitConfirmedOnEnemy(
                                battleManager, controller, target, actionModifier,
                                rulesetEffect, effectForms, i == 0, false);
                        }

                        var hero = controller.RulesetCharacter.GetOriginalHero();

                        if (hero != null)
                        {
                            foreach (var magicalAttackBeforeHitConfirmedOnEnemy in hero.TrainedMetamagicOptions
                                         .SelectMany(metamagic =>
                                             metamagic
                                                 .GetAllSubFeaturesOfType<IMagicEffectBeforeHitConfirmedOnEnemy>()))
                            {
                                yield return magicalAttackBeforeHitConfirmedOnEnemy
                                    .OnMagicEffectBeforeHitConfirmedOnEnemy(
                                        battleManager, controller, target, actionModifier,
                                        rulesetEffect, effectForms, i == 0, false);
                            }
                        }
                    }

                    //PATCH: support for `IMagicEffectBeforeHitConfirmedOnMe`
                    // should also happen outside battles
                    if (target.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
                    {
                        foreach (var magicalAttackBeforeHitConfirmedOnMe in target.RulesetCharacter
                                     .GetSubFeaturesByType<IMagicEffectBeforeHitConfirmedOnMe>())
                        {
                            yield return magicalAttackBeforeHitConfirmedOnMe.OnMagicEffectBeforeHitConfirmedOnMe(
                                battleManager, actingCharacter, target, actionModifier,
                                rulesetEffect, effectForms, i == 0, false);
                        }
                    }

                    // END PATCH

                    //BUGFIX: vanilla doesn't handle triggers on enemy death
                    var effectDamageForm =
                        effectForms.FirstOrDefault(x => x.FormType == EffectForm.EffectFormType.Damage);
                    var targetWasDeadOrDyingOrUnconscious =
                        target.RulesetCharacter is { IsDeadOrDyingOrUnconscious: true };
                    //don't use rulesetCharacter here
                    var targetCurrentHitPoints = target.RulesetActor.CurrentHitPoints;
                    //END BUGFIX

                    //BEGIN PATCH

                    //PATCH: support for `ForcePushOrDragFromEffectPoint`
                    var positions = __instance.ActionParams.Positions;
                    var sourceDefinition = activePower.SourceDefinition;

                    if (positions.Count != 0 && sourceDefinition.HasSubFeatureOfType<ForcePushOrDragFromEffectPoint>())
                    {
                        applyFormsParams.position = positions[0];
                    }
                    // END PATCH

                    implementationService.ApplyEffectForms(
                        effectForms,
                        applyFormsParams,
                        null,
                        effectApplication: effectDescription.EffectApplication,
                        filters: effectDescription.EffectFormFilters,
                        damageAbsorbedByTemporaryHitPoints: out var damageAbsorbedByTemporaryHitPoints,
                        terminateEffectOnTarget: out _);

                    //BUGFIX: vanilla doesn't handle triggers on enemy death
                    if (effectDamageForm != null)
                    {
                        var effectDamageTypes = new List<string> { effectDamageForm.DamageForm.DamageType };

                        yield return HandlePostApplyMagicEffectOnZoneOrTargets(
                            battleManager,
                            actingCharacter,
                            target,
                            rulesetEffect,
                            targetWasDeadOrDyingOrUnconscious,
                            //don't use rulesetCharacter here
                            targetCurrentHitPoints - target.RulesetActor.CurrentHitPoints,
                            damageAbsorbedByTemporaryHitPoints,
                            effectDamageTypes);
                    }
                    //END BUGFIX

                    // Impact particles
                    var positioningService =
                        ServiceRepository.GetService<IGameLocationPositioningService>();
                    var impactPoint = new Vector3();
                    var impactRotation = Quaternion.identity;

                    positioningService.ComputeImpactCenterPositionAndRotation(
                        target, ref impactPoint, ref impactRotation);

                    var impactPlanePoint = positioningService.GetImpactPlanePosition(impactPoint);
                    var impactTarget = new ActionDefinitions.MagicEffectCastData
                    {
                        Source = activePower.Name,
                        EffectDescription = effectDescription,
                        Caster = actingCharacter,
                        Targets = targets,
                        TargetIndex = i,
                        SubTargets = actionParams.SubTargets,
                        SubTargetIndex = 0,
                        ImpactPoint = impactPoint,
                        ImpactRotation = impactRotation,
                        ImpactPlanePoint = impactPlanePoint,
                        ActionType = __instance.ActionType,
                        ActionId = __instance.ActionId,
                        IsDivertHit = false,
                        ComputedTargetParameter = activePower.ComputeTargetParameter()
                    };

                    ServiceRepository.GetService<IGameLocationActionService>()?
                        .MagicEffectHitTarget?.Invoke(ref impactTarget);
                }

                if (!__instance.RolledSaveThrow ||
                    __instance.SaveOutcome == RuleDefinitions.RollOutcome.Failure ||
                    __instance.SaveOutcome == RuleDefinitions.RollOutcome.CriticalFailure)
                {
                    yield return __instance.HandlePostSuccessfulExecutionOnTarget();
                }
            }

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

            //PATCH: support for `IMagicEffectFinishedByMe`
            foreach (var magicEffectFinishedByMe in actingCharacter.RulesetCharacter
                         .GetSubFeaturesByType<IMagicEffectFinishedByMe>())
            {
                yield return
                    magicEffectFinishedByMe.OnMagicEffectFinishedByMe(__instance, actingCharacter, targets);
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

            actingCharacter.RulesetCharacter.ProcessConditionsMatchingInterruption(
                (RuleDefinitions.ConditionInterruption)ExtraConditionInterruption.SpendPowerExecuted);

            __instance.PersistantEffectAction();
        }
    }
}
