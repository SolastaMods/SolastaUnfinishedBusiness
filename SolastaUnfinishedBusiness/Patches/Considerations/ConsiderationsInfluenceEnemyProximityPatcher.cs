using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using TA.AI;
using TA.AI.Considerations;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches.Considerations;

[UsedImplicitly]
public static class InfluenceEnemyProximityPatcher
{
    //PATCH: allows this influence to be reverted if boolSecParameter is true
    //used on Command Spell, approach command
    [HarmonyPatch(typeof(InfluenceEnemyProximity), nameof(InfluenceEnemyProximity.Score))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Score_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            DecisionContext context,
            ConsiderationDescription consideration,
            DecisionParameters parameters,
            ScoringResult scoringResult)
        {
            Score(context, consideration, parameters, scoringResult);

            return false;
        }

        // mainly vanilla code except for BEGIN/END blocks
        private static void Score(
            DecisionContext context,
            ConsiderationDescription consideration,
            DecisionParameters parameters,
            ScoringResult scoringResult)
        {
            var denominator = consideration.IntParameter > 0 ? consideration.IntParameter : 1;
            var floatParameter = consideration.FloatParameter;
            var position = consideration.BoolParameter
                ? context.position
                : parameters.character.GameLocationCharacter.LocationPosition;
            var numerator = 0.0f;

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var enemy in parameters.situationalInformation.RelevantEnemies)
            {
                if (!AiLocationDefinitions.IsRelevantTargetForCharacter(
                        parameters.character.GameLocationCharacter,
                        enemy, parameters.situationalInformation.HasRelevantPerceivedTarget))
                {
                    continue;
                }

                var distance =
                    parameters.situationalInformation.PositioningService
                        .ComputeDistanceBetweenCharactersApproximatingSize(
                            parameters.character.GameLocationCharacter, position, enemy, enemy.LocationPosition);

                //BEGIN PATCH
                if (consideration.boolSecParameter)
                {
                    distance = floatParameter - distance + 1;
                }
                //END PATCH

                numerator += Mathf.Lerp(1f, 0.0f, Mathf.Clamp(distance / floatParameter, 0.0f, 1f));
            }

            scoringResult.Score = numerator / denominator;
        }
    }
}
