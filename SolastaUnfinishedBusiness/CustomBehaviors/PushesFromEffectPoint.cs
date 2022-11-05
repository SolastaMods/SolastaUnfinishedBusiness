using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using TA;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

/**allows marking spells/powers to make push.drag effects from them work relative to target point, not caster position*/
internal sealed class PushesFromEffectPoint
{
    private PushesFromEffectPoint()
    {
        // Empty
    }

    public static PushesFromEffectPoint Marker { get; } = new();

    internal static IEnumerable<CodeInstruction> ModifyApplyFormsCall(IEnumerable<CodeInstruction> instructions)
    {
        var method =
            new Func<IRulesetImplementationService, List<EffectForm>, RulesetImplementationDefinitions.ApplyFormsParams,
                List<string>, bool, bool, bool, RuleDefinitions.EffectApplication, List<EffectFormFilter>,
                CharacterActionMagicEffect, int>(SetPositionAndApplyForms).Method;

        return instructions.ReplaceCode(
            instruction => $"{instruction.operand}".Contains("ApplyEffectForms"),
            -1, "PushesFromEffectPoint.ModifyApplyFormsCall",
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Call, method));
    }

    private static int SetPositionAndApplyForms(
        IRulesetImplementationService service,
        List<EffectForm> effectForms,
        RulesetImplementationDefinitions.ApplyFormsParams formsParams,
        List<string> effectiveDamageTypes,
        bool retargeting,
        bool proxyOnly,
        bool forceSelfConditionOnly,
        RuleDefinitions.EffectApplication effectApplication,
        List<EffectFormFilter> filters,
        CharacterActionMagicEffect action
    )
    {
        var positions = action.ActionParams.Positions;

        if (!positions.Empty()
            && formsParams.activeEffect.SourceDefinition.HasSubFeatureOfType<PushesFromEffectPoint>())
        {
            formsParams.position = positions[0];
        }

        return service.ApplyEffectForms(effectForms, formsParams, effectiveDamageTypes, retargeting, proxyOnly,
            forceSelfConditionOnly, effectApplication, filters);
    }

    public static bool TryPushFromEffectTargetPoint(EffectForm effectForm,
        RulesetImplementationDefinitions.ApplyFormsParams formsParams)
    {
        var source = formsParams.activeEffect?.SourceDefinition;

        if (source == null)
        {
            return true;
        }

        var position = formsParams.position;
        var active = source.HasSubFeatureOfType<PushesFromEffectPoint>();

        if (!active || position == int3.zero)
        {
            return true;
        }

        if (formsParams.targetCharacter == null || !formsParams.targetCharacter.CanReceiveMotion ||
            (formsParams.rolledSaveThrow &&
             effectForm.SavingThrowAffinity != RuleDefinitions.EffectSavingThrowType.None &&
             formsParams.saveOutcome != RuleDefinitions.RollOutcome.Failure &&
             formsParams.saveOutcome != RuleDefinitions.RollOutcome.CriticalFailure))
        {
            return true;
        }

        var motionForm = effectForm.MotionForm;

        if (motionForm.Type != MotionForm.MotionType.PushFromOrigin
            && motionForm.Type != MotionForm.MotionType.DragToOrigin)
        {
            return true;
        }

        var target = GameLocationCharacter.GetFromActor(formsParams.targetCharacter);

        if (target == null)
        {
            return true;
        }

        //if origin point matches target - skip pushing
        if (position == target.LocationPosition)
        {
            return false;
        }

        var reverse = motionForm.Type == MotionForm.MotionType.DragToOrigin;

        if (!ServiceRepository.GetService<IGameLocationEnvironmentService>()
                .ComputePushDestination(position, target, motionForm.Distance, reverse,
                    ServiceRepository.GetService<IGameLocationPositioningService>(),
                    out var destination, out var _))
        {
            return false;
        }

        var actionService = ServiceRepository.GetService<IGameLocationActionService>();

        actionService.StopCharacterActions(target, CharacterAction.InterruptionType.ForcedMovement);
        actionService.ExecuteAction(
            new CharacterActionParams(target, ActionDefinitions.Id.Pushed, destination)
            {
                CanBeCancelled = false, CanBeAborted = false, BoolParameter4 = false
            }, null, false);

        return false;
    }
}
