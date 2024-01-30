using System.Collections;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
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
        [UsedImplicitly]
        public static IEnumerator Postfix(
            [NotNull] IEnumerator values,
            CharacterActionSpendPower __instance)
        {
            while (values.MoveNext())
            {
                yield return values.Current;
            }

            //PATCH: support for shared pool powers that character got from conditions to properly consume uses when triggered
            if (__instance.ActionParams.RulesetEffect is not RulesetEffectPower { OriginItem: null } activePower)
            {
                yield break;
            }

            var usablePower = activePower.UsablePower;

            // in this case base game already calls `UsePower`
            if (usablePower.OriginClass != null || usablePower.OriginRace != null)
            {
                yield break;
            }

            if (usablePower.powerDefinition.HasSubFeatureOfType<ForcePowerUseInSpendPowerAction>())
            {
                __instance.ActingCharacter.RulesetCharacter.UsePower(usablePower);
            }
        }

        // FULL VANILLA CODE FOR REFERENCE
        private static IEnumerator ExecuteImpl(CharacterActionSpendPower __instance)
        {
            var implementationService = ServiceRepository.GetService<IRulesetImplementationService>();

            __instance.targets.Clear();
            __instance.targets.AddRange(__instance.ActionParams.TargetCharacters);

            var effectDescription = __instance.ActionParams.RulesetEffect.EffectDescription;

            __instance.activePower = __instance.ActionParams.RulesetEffect as RulesetEffectPower;

            var actionModifier = new ActionModifier();

            if (__instance.activePower is { OriginItem: null })
            {
                if (__instance.activePower.UsablePower.OriginClass != null ||
                    __instance.activePower.UsablePower.OriginRace != null)
                {
                    __instance.ActingCharacter.RulesetCharacter.UsePower(__instance.activePower.UsablePower);
                }
            }
            else
            {
                if (__instance.activePower != null)
                {
                    __instance.ActingCharacter.RulesetCharacter.UseDevicePower(
                        __instance.activePower.OriginItem, __instance.activePower.PowerDefinition);
                }
            }

            for (var i = 0; i < __instance.targets.Count; ++i)
            {
                var target = __instance.targets[i];
                var hasBorrowedLuck =
                    target.RulesetActor.HasConditionOfTypeOrSubType("ConditionDomainMischiefBorrowedLuck");

                if (__instance.activePower != null)
                {
                    __instance.RolledSaveThrow = __instance.activePower.TryRollSavingThrow(
                        __instance.ActingCharacter.RulesetCharacter,
                        __instance.ActingCharacter.Side,
                        target.RulesetActor,
                        actionModifier, __instance.activePower.EffectDescription.EffectForms,
                        false,
                        out var saveOutcome,
                        out var saveOutcomeDelta);

                    __instance.SaveOutcome = saveOutcome;
                    __instance.SaveOutcomeDelta = saveOutcomeDelta;

                    if (__instance.RolledSaveThrow)
                    {
                        var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

                        if (__instance.SaveOutcome == RuleDefinitions.RollOutcome.Failure)
                        {
                            yield return gameLocationBattleService.HandleFailedSavingThrow(
                                __instance, __instance.ActingCharacter, target, actionModifier, false, hasBorrowedLuck);
                        }
                    }

                    var formsParams = new RulesetImplementationDefinitions.ApplyFormsParams();

                    formsParams.FillSourceAndTarget(
                        __instance.ActingCharacter.RulesetCharacter, target.RulesetCharacter);
                    formsParams.FillFromActiveEffect(__instance.activePower);
                    formsParams.FillSpecialParameters(__instance.RolledSaveThrow, 0, 0, 0, 0,
                        actionModifier, __instance.SaveOutcome, __instance.SaveOutcomeDelta, false, 0, 1, null);
                    formsParams.effectSourceType = RuleDefinitions.EffectSourceType.Power;
                    implementationService.ApplyEffectForms(effectDescription.EffectForms, formsParams, null,
                        out _, out _, effectApplication: effectDescription.EffectApplication,
                        filters: effectDescription.EffectFormFilters);

                    var positioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
                    var impactCenter = new Vector3();
                    var identity = Quaternion.identity;

                    positioningService.ComputeImpactCenterPositionAndRotation(target, ref impactCenter, ref identity);

                    var impactPlanePosition = positioningService.GetImpactPlanePosition(impactCenter);
                    var data = new ActionDefinitions.MagicEffectCastData
                    {
                        Source = __instance.activePower.Name,
                        EffectDescription = effectDescription,
                        Caster = __instance.ActingCharacter,
                        Targets = __instance.targets,
                        TargetIndex = i,
                        SubTargets = __instance.ActionParams.SubTargets,
                        SubTargetIndex = 0,
                        ImpactPoint = impactCenter,
                        ImpactRotation = identity,
                        ImpactPlanePoint = impactPlanePosition,
                        ActionType = __instance.ActionType,
                        ActionId = __instance.ActionId,
                        IsDivertHit = false,
                        ComputedTargetParameter = __instance.activePower.ComputeTargetParameter()
                    };
                    var magicEffectHitTarget =
                        ServiceRepository.GetService<IGameLocationActionService>().MagicEffectHitTarget;

                    magicEffectHitTarget?.Invoke(ref data);
                }

                if (!__instance.RolledSaveThrow ||
                    __instance.SaveOutcome == RuleDefinitions.RollOutcome.Failure ||
                    __instance.SaveOutcome == RuleDefinitions.RollOutcome.CriticalFailure)
                {
                    yield return __instance.HandlePostSuccessfulExecutionOnTarget();
                }
            }

            __instance.PersistantEffectAction();
        }
    }
}
