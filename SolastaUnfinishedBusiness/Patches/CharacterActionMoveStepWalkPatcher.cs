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
            var mover = __instance.ActingCharacter;

            //PATCH: support for Circle of the Wildfire cauterizing flames
            yield return CircleOfTheWildfire.HandleCauterizingFlamesBehavior(mover);

            //PATCH: support for Polearm Expert AoO. processes saved movement to trigger AoO when appropriate
            var extraAoOEvents = AttacksOfOpportunity.ProcessOnCharacterMoveEnd(mover);

            while (extraAoOEvents.MoveNext())
            {
                yield return extraAoOEvents.Current;
            }

            // record movement after we try to process extra AoO events so above will properly kick after animations
            MovementTracker.RecordMovement(mover, mover.DestinationPosition);

            while (values.MoveNext())
            {
                yield return values.Current;
            }
        }
    }
}
