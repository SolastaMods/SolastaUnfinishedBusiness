using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using TA;

namespace SolastaUnfinishedBusiness.Behaviors;

/**allows marking spells/powers to make push.drag effects from them work relative to target point, not caster position*/
internal sealed class ForcePushOrDragFromEffectPoint
{
    private ForcePushOrDragFromEffectPoint()
    {
        // Empty
    }

    public static ForcePushOrDragFromEffectPoint Marker { get; } = new();

    internal static int SetPositionAndApplyForms(
        IRulesetImplementationService service,
        List<EffectForm> effectForms,
        RulesetImplementationDefinitions.ApplyFormsParams formsParams,
        List<string> effectiveDamageTypes,
        out bool damageAbsorbedByTemporaryHitPoints,
        out bool terminateEffectOnTarget,
        bool retargeting,
        bool proxyOnly,
        bool forceSelfConditionOnly,
        RuleDefinitions.EffectApplication effectApplication,
        List<EffectFormFilter> filters,
        CharacterActionMagicEffect action)
    {
        var positions = action.ActionParams.Positions;

        if (positions.Count != 0 &&
            formsParams.activeEffect.SourceDefinition.HasSubFeatureOfType<ForcePushOrDragFromEffectPoint>())
        {
            formsParams.position = positions[0];
        }

        return service.ApplyEffectForms(
            effectForms,
            formsParams,
            effectiveDamageTypes,
            out damageAbsorbedByTemporaryHitPoints,
            out terminateEffectOnTarget,
            retargeting,
            proxyOnly,
            forceSelfConditionOnly,
            effectApplication,
            filters);
    }

    public static bool TryPushFromEffectTargetPoint(
        EffectForm effectForm,
        RulesetImplementationDefinitions.ApplyFormsParams formsParams)
    {
        var source = formsParams.activeEffect?.SourceDefinition;

        if (source == null)
        {
            return true;
        }

        var position = formsParams.position;
        var active = source.HasSubFeatureOfType<ForcePushOrDragFromEffectPoint>();

        if (!active || position == int3.zero)
        {
            return true;
        }

        if (formsParams.targetCharacter is not { CanReceiveMotion: true } ||
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
                    out var destination, out _))
        {
            return false;
        }

        var actionService = ServiceRepository.GetService<IGameLocationActionService>();

        actionService?.StopCharacterActions(target, CharacterAction.InterruptionType.ForcedMovement);
        actionService?.ExecuteAction(
            new CharacterActionParams(target, ActionDefinitions.Id.Pushed, destination)
            {
                CanBeCancelled = false, CanBeAborted = false, BoolParameter4 = false
            }, null, false);

        return false;
    }
}
