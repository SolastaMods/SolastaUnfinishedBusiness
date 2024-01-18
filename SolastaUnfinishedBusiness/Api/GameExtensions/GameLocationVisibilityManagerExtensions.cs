using System.Linq;
using SolastaUnfinishedBusiness.CustomBehaviors;
using TA;

namespace SolastaUnfinishedBusiness.Api.GameExtensions;

internal static class GameLocationVisibilityManagerExtensions
{
    // improved cell perception routine that takes sight into consideration
    public static bool MyIsCellPerceivedByCharacter(
        this GameLocationVisibilityManager instance,
        int3 cellPosition,
        GameLocationCharacter sensor,
        GameLocationCharacter target = null,
        LocationDefinitions.LightingState additionalBlockedLightingState = LocationDefinitions.LightingState.Darkness)
    {
        var result = instance.IsCellPerceivedByCharacter(cellPosition, sensor);

        // if setting is off or vanilla cannot perceive
        if (!result ||
            !Main.Settings.UseOfficialLightingObscurementAndVisionRules)
        {
            if (!result)
            {
                return false;
            }

            // might wanna dup some code and remove Silhouette Step handling below for performance reasons
            var lightningState = sensor.ComputeLightingStateOnTargetPosition(cellPosition);

            // Silhouette Step is the only one using additionalBlockedLightingState as it requires to block BRIGHT
            return additionalBlockedLightingState == LocationDefinitions.LightingState.Darkness ||
                   lightningState != additionalBlockedLightingState;
        }

        {
            var inRange = false;
            var distance = DistanceCalculation.GetDistanceFromTwoPositions(sensor.LocationPosition, cellPosition);
            var lightingState = sensor.ComputeLightingStateOnTargetPosition(cellPosition);
            var nonMagicalDarkness = target?.LightingState != LocationDefinitions.LightingState.Darkness;
            var selectedSenseType = SenseMode.Type.None;
            var selectedSenseRange = 0;

            // try to find any sense mode that is valid for the current lighting state and is within range
            foreach (var senseMode in sensor.RulesetCharacter.SenseModes
                         .Where(senseMode => SenseMode.ValidForLighting(senseMode.SenseType, lightingState)))
            {
                var senseType = senseMode.SenseType;

                if (selectedSenseType != senseType && senseMode.SenseRange >= selectedSenseRange)
                {
                    if (nonMagicalDarkness &&
                        lightingState == LocationDefinitions.LightingState.Darkness &&
                        senseType == SenseMode.Type.Truesight)
                    {
                        continue;
                    }

                    selectedSenseType = senseType;
                    selectedSenseRange = senseMode.SenseRange;
                }

                inRange = distance <= senseMode.SenseRange;

                if (inRange)
                {
                    break;
                }
            }

            return inRange &&
                   // Silhouette Step is the only one using additionalBlockedLightingState as it requires to block BRIGHT
                   (additionalBlockedLightingState == LocationDefinitions.LightingState.Darkness ||
                    lightingState != additionalBlockedLightingState);
        }
    }
}
