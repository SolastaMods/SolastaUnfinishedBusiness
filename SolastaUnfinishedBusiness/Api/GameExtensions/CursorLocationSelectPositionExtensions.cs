using System.Collections;
using TA;

namespace SolastaUnfinishedBusiness.Api.GameExtensions;

internal static class CursorLocationSelectPositionExtensions
{
    // this is vanilla code where only change is call MyIsCellPerceivedByCharacter instead of default one
    public static IEnumerator MyComputeValidPositions(
        this CursorLocationSelectPosition __instance,
        LocationDefinitions.LightingState additionalBlockedState = LocationDefinitions.LightingState.Darkness)
    {
        var boxInt = new BoxInt(__instance.ActionParams.ActingCharacter.LocationPosition, new int3(0), new int3(0));

        boxInt.Inflate((int)__instance.maxDistance);

        var positioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
        var visibilityService =
            ServiceRepository.GetService<IGameLocationVisibilityService>() as GameLocationVisibilityManager;
        var locationService = ServiceRepository.GetService<IGameLocationService>();
        var onlyFeedbackGroundCells = __instance.isTeleportingSpell;

        foreach (var int3 in boxInt.EnumerateAllPositionsWithin())
        {
            var locationCharacter = __instance.ActionParams?.ActingCharacter;

            if (locationCharacter == null)
            {
                break;
            }

            if (int3.Distance(__instance.centerPosition, int3) <= (double)__instance.maxDistance &&
                positioningService.CanPlaceCharacter(locationCharacter, int3, CellHelpers.PlacementMode.Station) &&
                positioningService.CanCharacterStayAtPosition_Floor(
                    locationCharacter, int3, onlyCheckCellsWithRealGround: onlyFeedbackGroundCells))
            {
                if (!__instance.requiresVisibilityForPosition
                        ? new GridAccessor(locationService).Visited(int3)
                        : visibilityService.MyIsCellPerceivedByCharacter(
                            int3, locationCharacter, additionalBlockedLightingState: additionalBlockedState))
                {
                    __instance.validPositionsCache.Add(int3);
                }
            }

            if (__instance.stopwatch.Elapsed.TotalMilliseconds > 0.5)
            {
                yield return null;
            }
        }
    }
}
