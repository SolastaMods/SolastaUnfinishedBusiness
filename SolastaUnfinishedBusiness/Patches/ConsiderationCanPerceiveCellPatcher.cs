using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;
using TA.AI;
using TA.AI.Considerations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class ConsiderationCanPerceiveCellPatcher
{
    //PATCH: supports `UseOfficialLightingObscurementAndVisionRules`
    [HarmonyPatch(typeof(CanPerceiveCell), nameof(CanPerceiveCell.Score))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Score_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(DecisionContext context, DecisionParameters parameters, ScoringResult scoringResult)
        {
            if (!Main.Settings.UseOfficialLightingObscurementAndVisionRules)
            {
                return true;
            }

            var visibilityService =
                parameters.situationalInformation.VisibilityService as GameLocationVisibilityManager;
            var position = context.position;
            var locationCharacter = parameters.character.GameLocationCharacter;
            var score = visibilityService.MyIsCellPerceivedByCharacter(position, locationCharacter);

            scoringResult.Score = score ? 1f : 0.0f;

            return false;
        }
    }
}
