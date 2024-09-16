using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using TA.AI;
using TA.AI.Considerations;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches.Considerations;

[UsedImplicitly]
public static class InfluenceEnemyProximityPatcher
{
    //PATCH: allows this influence to be reverted if enemy has StringParameter condition name
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
            var character = parameters.character.GameLocationCharacter;
            var rulesetCharacter = character.RulesetCharacter;
            var denominator = consideration.IntParameter > 0 ? consideration.IntParameter : 1;
            var floatParameter = consideration.FloatParameter;
            var position = consideration.BoolParameter ? context.position : character.LocationPosition;
            var numerator = 0.0f;

            var approachSourceGuid = rulesetCharacter.ConditionsByCategory
                .SelectMany(x => x.Value)
                .FirstOrDefault(x =>
                    x.ConditionDefinition.Name == consideration.StringParameter)?.SourceGuid ?? 0;

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var enemy in parameters.situationalInformation.RelevantEnemies)
            {
                if (!AiLocationDefinitions.IsRelevantTargetForCharacter(
                        character, enemy, parameters.situationalInformation.HasRelevantPerceivedTarget))
                {
                    continue;
                }

                var distance =
                    parameters.situationalInformation.PositioningService
                        .ComputeDistanceBetweenCharactersApproximatingSize(
                            parameters.character.GameLocationCharacter, position, enemy, enemy.LocationPosition);

                //BEGIN PATCH
                if (enemy.Guid == approachSourceGuid)
                {
                    numerator += Mathf.Lerp(0.0f, 1f, Mathf.Clamp(distance / floatParameter, 0.0f, 1f));
                    continue;
                }
                //END PATCH

                numerator += Mathf.Lerp(1f, 0.0f, Mathf.Clamp(distance / floatParameter, 0.0f, 1f));
            }

            scoringResult.Score = numerator / denominator;
        }
    }
}
