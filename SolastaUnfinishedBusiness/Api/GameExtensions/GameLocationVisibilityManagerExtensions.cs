using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.Models;
using TA;
using static LocationDefinitions;
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
        LightingState additionalBlockedLightingState = LightingState.Darkness)
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
            return additionalBlockedLightingState == LightingState.Darkness ||
                   sensor.ComputeLightingStateOnTargetPosition(cellPosition) != additionalBlockedLightingState;
        }

        var inRange = false;
        var distance = DistanceCalculation.GetDistanceFromTwoPositions(sensor.LocationPosition, cellPosition);
        var selectedSenseType = SenseMode.Type.None;
        var selectedSenseRange = 0;
        var sourceIsHeavilyObscured =
            sensor.RulesetActor.HasConditionOfTypeOrSubType(ConditionBlinded.Name) ||
            sensor.LightingState == LightingState.Unlit;
        var targetIsNonMagicallyHeavilyObscured =
            target != null &&
            !target.RulesetActor.HasConditionOfType(SrdAndHouseRulesContext.ConditionBlindedByDarkness) &&
            target.RulesetActor.AllConditions.Exists(x =>
                (x.ConditionDefinition == ConditionBlinded ||
                 x.ConditionDefinition.parentCondition == ConditionBlinded) &&
                x.ConditionDefinition != SrdAndHouseRulesContext.ConditionBlindedByDarkness);
        var targetLightingState =
            target != null &&
            target.RulesetActor.HasConditionOfTypeOrSubType(ConditionBlinded.Name) &&
            target.LightingState is LightingState.Bright or LightingState.Dim
                ? LightingState.Unlit
                : sensor.ComputeLightingStateOnTargetPosition(cellPosition);

        // try to find any sense mode that is valid for the current lighting state and is within range
        foreach (var senseMode in sensor.RulesetCharacter.SenseModes)
        {
            var senseType = senseMode.SenseType;

            if (!SenseMode.ValidForLighting(senseMode.SenseType, targetLightingState))
            {
                continue;
            }

            // these are the only senses a heavily obscured source can use
            if (sourceIsHeavilyObscured &&
                senseType is not (
                    SenseMode.Type.Truesight or
                    SenseMode.Type.Blindsight or
                    SenseMode.Type.Tremorsense))
            {
                continue;
            }

            if (selectedSenseType != senseType && senseMode.SenseRange >= selectedSenseRange)
            {
                // can only use true sight on magically heavily obscured
                if (!targetIsNonMagicallyHeavilyObscured &&
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
               (additionalBlockedLightingState == LightingState.Darkness ||
                targetLightingState != additionalBlockedLightingState);
    }
}
