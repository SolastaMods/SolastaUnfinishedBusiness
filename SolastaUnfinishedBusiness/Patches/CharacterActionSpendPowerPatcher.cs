using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
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
            var actingCharacter = __instance.ActingCharacter;
            var actionParams = __instance.ActionParams;
            var rulesetEffect = actionParams.RulesetEffect;
            var effectDescription = rulesetEffect.EffectDescription;

            var implementationService = ServiceRepository.GetService<IRulesetImplementationService>();

            // Retrieve the target
            __instance.targets.Clear();
            __instance.targets.AddRange(actionParams.TargetCharacters);

            // BEGIN PATCH

            if (rulesetEffect is not RulesetEffectPower { OriginItem: null } activePower)
            {
                yield break;
            }

            // END PATCH

            // Create an active power
            __instance.activePower = activePower;

            //PATCH: supports `IMagicEffectInitiatedByMe`
            foreach (var magicEffectInitiatedByMe in actingCharacter.RulesetCharacter
                         .GetSubFeaturesByType<IMagicEffectInitiatedByMe>())
            {
                yield return magicEffectInitiatedByMe.OnMagicEffectInitiatedByMe(
                    __instance,
                    rulesetEffect,
                    actingCharacter,
                    __instance.targets);
            }

            var actionModifier = new ActionModifier();

            // Spend the power, if this does not come from an item
            if (__instance.activePower is { OriginItem: null })
            {
                // Fire shield retaliation has no class or race origin
                if (__instance.activePower.UsablePower.OriginClass)
                {
                    actingCharacter.RulesetCharacter.UsePower(__instance.activePower.UsablePower);
                }
                else if (__instance.activePower.UsablePower.OriginRace)
                {
                    actingCharacter.RulesetCharacter.UsePower(__instance.activePower.UsablePower);
                }

                // BEGIN PATCH

                //PATCH: support for shared pool powers that character got from conditions to properly consume uses when triggered
                if (activePower.UsablePower.powerDefinition.HasSubFeatureOfType<ForcePowerUseInSpendPowerAction>())
                {
                    __instance.ActingCharacter.RulesetCharacter.UsePower(activePower.UsablePower);
                }

                // END PATCH
            }
            else
            {
                if (__instance.activePower != null)
                {
                    actingCharacter.RulesetCharacter.UseDevicePower(
                        __instance.activePower.OriginItem, __instance.activePower.PowerDefinition);
                }
            }

            for (var i = 0; i < __instance.targets.Count; i++)
            {
                var target = __instance.targets[i];

                // These bool information must be store as a class member, as it is passed to HandleFailedSavingThrow
                var hasBorrowedLuck =
                    target.RulesetActor.HasConditionOfTypeOrSubType(RuleDefinitions.ConditionBorrowedLuck);

                if (__instance.activePower != null)
                {
                    __instance.RolledSaveThrow = __instance.activePower.TryRollSavingThrow(
                        actingCharacter.RulesetCharacter, actingCharacter.Side,
                        target.RulesetActor,
                        actionModifier,
                        __instance.activePower.EffectDescription.EffectForms,
                        false,
                        out var saveOutcome,
                        out var saveOutcomeDelta);
                    __instance.SaveOutcome = saveOutcome;
                    __instance.SaveOutcomeDelta = saveOutcomeDelta;

                    var battleManager =
                        ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

                    if (__instance.RolledSaveThrow)
                    {
                        // Legendary Resistance or Indomitable?
                        if (__instance.SaveOutcome == RuleDefinitions.RollOutcome.Failure)
                        {
                            yield return battleManager.HandleFailedSavingThrow(
                                __instance, actingCharacter, target, actionModifier, false, hasBorrowedLuck);
                        }

                        //PATCH: support for `ITryAlterOutcomeSavingThrow`
                        foreach (var tryAlterOutcomeSavingThrow in TryAlterOutcomeSavingThrow.Handler(
                                     battleManager, __instance, actingCharacter, target, actionModifier, false,
                                     hasBorrowedLuck))
                        {
                            yield return tryAlterOutcomeSavingThrow;
                        }
                    }

                    // Apply the forms of the power
                    var applyFormsParams = new RulesetImplementationDefinitions.ApplyFormsParams();

                    applyFormsParams.FillSourceAndTarget(actingCharacter.RulesetCharacter, target.RulesetCharacter);
                    applyFormsParams.FillFromActiveEffect(__instance.activePower);
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
                    var targetCurrentHitPoints = target.RulesetCharacter.CurrentHitPoints;
                    //END BUGFIX

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
                            targetCurrentHitPoints - target.RulesetCharacter.CurrentHitPoints,
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
                        Source = __instance.activePower.Name,
                        EffectDescription = effectDescription,
                        Caster = actingCharacter,
                        Targets = __instance.targets,
                        TargetIndex = i,
                        SubTargets = actionParams.SubTargets,
                        SubTargetIndex = 0,
                        ImpactPoint = impactPoint,
                        ImpactRotation = impactRotation,
                        ImpactPlanePoint = impactPlanePoint,
                        ActionType = __instance.ActionType,
                        ActionId = __instance.ActionId,
                        IsDivertHit = false,
                        ComputedTargetParameter = __instance.activePower.ComputeTargetParameter()
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

            //PATCH: support for `IMagicEffectFinishedByMe`
            foreach (var magicEffectFinishedByMe in actingCharacter.RulesetCharacter
                         .GetSubFeaturesByType<IMagicEffectFinishedByMe>())
            {
                yield return
                    magicEffectFinishedByMe.OnMagicEffectFinishedByMe(__instance, actingCharacter, __instance.targets);
            }

            //PATCH: support for `IMagicEffectFinishedOnMe`
            foreach (var target in __instance.targets)
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
                        __instance, actingCharacter, target, __instance.targets);
                }
            }
            
            //PATCH: support for `IMagicEffectFinishedByMeOrAlly`
            var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var contenders =
                locationCharacterService.PartyCharacters.Union(locationCharacterService.GuestCharacters);
            
            foreach (var ally in contenders
                         .Where(x => x.Side == actingCharacter.Side
                                     && x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
                         .ToList())
            {
                foreach (var magicEffectFinishedByMeOrAlly in ally.RulesetCharacter
                             .GetSubFeaturesByType<IMagicEffectFinishedByMeOrAlly>())
                {
                    yield return magicEffectFinishedByMeOrAlly
                        .OnMagicEffectFinishedByMeOrAlly(
                            null, __instance, actingCharacter, ally, __instance.targets);
                }
            }

            __instance.PersistantEffectAction();

            yield return null;
        }
    }
}
