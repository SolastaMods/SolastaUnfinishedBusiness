using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.Models;
using TA;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;

namespace SolastaUnfinishedBusiness.Api.GameExtensions;

internal static class GameLocationVisibilityManagerExtensions
{
    // improved cell perception routine that takes sight into consideration
    // most of the usages is to determine if a character can perceive a cell in teleport scenarios
    // when target not null it helps determine visibility on attacks and targeting scenarios
    public static bool MyIsCellPerceivedByCharacter(
        this GameLocationVisibilityManager instance,
        int3 cellPosition,
        GameLocationCharacter sensor,
        GameLocationCharacter target = null,
        LocationDefinitions.LightingState additionalBlockedLightingState = LocationDefinitions.LightingState.Darkness)
    {
        // let vanilla do the heavy lift on perception
        var result = instance.IsCellPerceivedByCharacter(cellPosition, sensor);

        // if setting is off or vanilla cannot perceive
        if (!result ||
            !Main.Settings.UseOfficialLightingObscurementAndVisionRules)
        {
            if (!result)
            {
                return false;
            }

            // Silhouette Step is the only one using additionalBlockedLightingState as it requires to block BRIGHT
            return additionalBlockedLightingState == LocationDefinitions.LightingState.Darkness ||
                   sensor.ComputeLightingStateOnTargetPosition(cellPosition) != additionalBlockedLightingState;
        }

        var inRange = false;
        var distance = DistanceCalculation.GetDistanceFromTwoPositions(sensor.LocationPosition, cellPosition);
        var selectedSenseType = SenseMode.Type.None;
        var selectedSenseRange = 0;
        var lightingState = sensor.ComputeLightingStateOnTargetPosition(cellPosition);
        var sourceIsNonMagicallyHeavilyObscured =
            sensor.RulesetActor.AllConditions.Exists(x =>
                x.ConditionDefinition == ConditionBlinded ||
                (x.ConditionDefinition != SrdAndHouseRulesContext.ConditionBlindedByDarkness &&
                 x.ConditionDefinition.parentCondition == ConditionBlinded) ||
                sensor.LightingState is LocationDefinitions.LightingState.Darkness
                    or LocationDefinitions.LightingState.Unlit);
        var sourceIsHeavilyObscured =
            sensor.RulesetActor.HasConditionOfTypeOrSubType(ConditionBlinded.Name);

        // force lighting state to unlit if target is blind and on a dim or bright cell
        if (target != null &&
            target.RulesetActor.HasConditionOfTypeOrSubType(ConditionBlinded.Name) &&
            lightingState is LocationDefinitions.LightingState.Bright or LocationDefinitions.LightingState.Dim)
        {
            lightingState = LocationDefinitions.LightingState.Unlit;
        }

        // try to find any sense mode that is valid for the current lighting state and is within range
        foreach (var senseMode in sensor.RulesetCharacter.SenseModes)
        {
            var senseType = senseMode.SenseType;

            if (!SenseMode.ValidForLighting(senseMode.SenseType, lightingState))
            {
                continue;
            }

            // these are the only senses a blinded creature can use
            if (sourceIsHeavilyObscured &&
                senseType is not (SenseMode.Type.Truesight or SenseMode.Type.Blindsight or SenseMode.Type.Tremorsense))
            {
                continue;
            }

            if (selectedSenseType != senseType && senseMode.SenseRange >= selectedSenseRange)
            {
                // can only use true sight on magical darkness
                if (sourceIsNonMagicallyHeavilyObscured &&
                    senseType == SenseMode.Type.Truesight)
                {
                    continue;
                }

                // can only use tremor sense on non flying creatures
                if (target != null &&
                    !target.RulesetActor.IsTouchingGround() &&
                    senseType == SenseMode.Type.Tremorsense)
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
