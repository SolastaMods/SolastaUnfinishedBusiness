using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using TA.AI;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class ConsiderationCanPerceiveCellPatcher
{
    [HarmonyPatch(typeof(TA.AI.Considerations.CanPerceiveCell), nameof(TA.AI.Considerations.CanPerceiveCell.Score))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Clear_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            TA.AI.Considerations.CanPerceiveCell __instance,
            DecisionContext context,
            ConsiderationDescription consideration,
            DecisionParameters parameters,
            ScoringResult scoringResult)
        {
            var locationCharacter = parameters.character.GameLocationCharacter;
            var position = context.position;
            var visibilityService =
                parameters.situationalInformation.VisibilityService as GameLocationVisibilityManager;

            bool flag = Main.Settings.UseOfficialObscurementRules
                ? visibilityService.MyIsCellPerceivedByCharacter(position, locationCharacter)
                : visibilityService!.IsCellPerceivedByCharacter(position, locationCharacter);

            scoringResult.Score = flag ? 1f : 0.0f;

            return false;
        }
    }
}
