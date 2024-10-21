using System.Collections;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Subclasses;

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

    //PATCH: support for Circle of the Wildfire cauterizing flames
    [HarmonyPatch(typeof(CharacterActionMoveStepWalk),
        nameof(CharacterActionMoveStepWalk.ChangeEndProneStatusIfNecessary))]
    [UsedImplicitly]
    public static class ChangeEndProneStatusIfNecessary_Patch
    {
        [UsedImplicitly]
        public static IEnumerator Postfix(IEnumerator values, CharacterActionMoveStepWalk __instance)
        {
            yield return CircleOfTheWildfire.HandleCauterizingFlamesBehavior(__instance.ActingCharacter);

            while (values.MoveNext())
            {
                yield return values.Current;
            }
        }
    }
}
