using System.Collections;
using SolastaUnfinishedBusiness.Models;
using TA;

namespace SolastaUnfinishedBusiness.Api.GameExtensions;

internal static class CursorLocationSelectPositionExtensions
{
    // this is vanilla code where only change is call MyIsCellPerceivedByCharacter instead of default one
    public static IEnumerator MyComputeValidPositions(
        this CursorLocationSelectPosition __instance,
        LocationDefinitions.LightingState additionalBlockedState = LocationDefinitions.LightingState.Darkness,
        int maxDistance = 0)
    {
        var boxInt = new BoxInt(__instance.ActionParams.ActingCharacter.LocationPosition, int3.zero, int3.zero);

        if (maxDistance == 0)
        {
            maxDistance = (int)__instance.maxDistance;
        }

        boxInt.Inflate(maxDistance);

        var positioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
        var visibilityService =
            ServiceRepository.GetService<IGameLocationVisibilityService>() as GameLocationVisibilityManager;
        var onlyFeedbackGroundCells = __instance.isTeleportingSpell;

        foreach (var int3 in boxInt.EnumerateAllPositionsWithin())
        {
            var locationCharacter = __instance.ActionParams?.ActingCharacter;

            if (locationCharacter == null)
            {
                break;
            }

            // must use vanilla distance calculation here
            if (int3.Distance(__instance.centerPosition, int3) <= maxDistance &&
                positioningService.CanPlaceCharacter(locationCharacter, int3, CellHelpers.PlacementMode.Station) &&
                positioningService.CanCharacterStayAtPosition_Floor(
                    locationCharacter, int3, onlyCheckCellsWithRealGround: onlyFeedbackGroundCells))
            {
                if (visibilityService.MyIsCellPerceivedByCharacter(int3, locationCharacter,
                        additionalBlockedLightingState: additionalBlockedState,
                        requireLineOfSight: __instance.requiresVisibilityForPosition))
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
