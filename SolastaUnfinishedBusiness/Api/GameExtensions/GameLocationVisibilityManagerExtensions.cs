using System.Linq;
using HarmonyLib;
using SolastaUnfinishedBusiness.CustomBehaviors;
using TA;

namespace SolastaUnfinishedBusiness.Api.GameExtensions;

internal static class GameLocationVisibilityManagerExtensions
{
    public static bool MyIsCellPerceivedByCharacter(
        this GameLocationVisibilityManager instance,
        int3 cellPosition,
        GameLocationCharacter sensor,
        LocationDefinitions.LightingState additionalBlockedLightingState = LocationDefinitions.LightingState.Darkness)
    {
        var result = instance.IsCellPerceivedByCharacter(cellPosition, sensor);

        if (!Main.Settings.UseOfficialObscurementRules || !result)
        {
            if (!result)
            {
                return false;
            }

            var lightningState = sensor.ComputeLightingStateOnTargetPosition(cellPosition);

            return additionalBlockedLightingState == LocationDefinitions.LightingState.Darkness ||
                   lightningState != additionalBlockedLightingState;
        }

        {
            var maxSenseRange = sensor.RulesetCharacter.GetFeaturesByType<FeatureDefinitionSense>()
                .Where(x =>
                    x.SenseType is SenseMode.Type.Blindsight or SenseMode.Type.Truesight or SenseMode.Type.Tremorsense)
                .Select(x => x.SenseRange)
                .AddItem(0)
                .Max();

            var lightningState = sensor.ComputeLightingStateOnTargetPosition(cellPosition);

            if (lightningState == LocationDefinitions.LightingState.Darkness)
            {
                return DistanceCalculation.GetDistanceFromTwoPositions(sensor.LocationPosition, cellPosition) <=
                       maxSenseRange;
            }

            return additionalBlockedLightingState == LocationDefinitions.LightingState.Darkness ||
                   lightningState != additionalBlockedLightingState;
        }
    }
}
