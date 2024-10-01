using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Interfaces;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterActionMoveStepWalkPatcher
{
    //PATCH: support for `IMoveStepStarted`
    [HarmonyPatch(typeof(CharacterActionMoveStepWalk),
        nameof(CharacterActionMoveStepWalk.ChangeStartProneStatusIfNecessary))]
    [UsedImplicitly]
    public static class ChangeStartProneStatusIfNecessary_Patch
    {
        [UsedImplicitly]
        public static void Prefix(
            CharacterActionMoveStepWalk __instance,
            CharacterActionMoveStepWalk.MoveStep currentStep)
        {
            var mover = __instance.ActingCharacter;
            var source = mover.LocationPosition;
            var destination = currentStep.position;

            MovementTracker.RecordMovement(mover, destination);

            foreach (var moveStepStarted in mover.RulesetCharacter.GetSubFeaturesByType<IMoveStepStarted>())
            {
                moveStepStarted.MoveStepStarted(mover, source, destination);
            }
        }
    }

#if false
    //PATCH: support for `IMoveStepFinished`
    [HarmonyPatch(typeof(CharacterActionMoveStepWalk),
        nameof(CharacterActionMoveStepWalk.ChangeEndProneStatusIfNecessary))]
    [UsedImplicitly]
    public static class ChangeEndProneStatusIfNecessary_Patch
    {
        [UsedImplicitly]
        public static void Prefix(CharacterActionMoveStepWalk __instance)
        {
            var mover = __instance.ActingCharacter;

            if (!MovementTracker.TryGetMovement(mover.Guid, out var movement))
            {
                return;
            }

            var source = movement.Item1;
            var destination = movement.Item2;

            foreach (var moveStepFinished in mover.RulesetCharacter.GetSubFeaturesByType<IMoveStepFinished>())
            {
                moveStepFinished.MoveStepFinished(mover, source, destination);
            }
        }
    }
#endif
}
