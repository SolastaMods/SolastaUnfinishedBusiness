using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using TA.AI;
using TA.AI.Considerations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class ConsiderationCanPerceiveCellPatcher
{
    [HarmonyPatch(typeof(CanPerceiveCell), nameof(CanPerceiveCell.Score))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Score_Patch
    {
        [UsedImplicitly]
#pragma warning disable IDE0060
        public static bool Prefix(
            CanPerceiveCell __instance,
            DecisionContext context,
            ConsiderationDescription consideration,
            DecisionParameters parameters,
            ScoringResult scoringResult)
#pragma warning restore IDE0060
        {
            var locationCharacter = parameters.character.GameLocationCharacter;
            var position = context.position;
            var visibilityService =
                parameters.situationalInformation.VisibilityService as GameLocationVisibilityManager;

            var flag = Main.Settings.UseOfficialObscurementRules
                ? visibilityService.MyIsCellPerceivedByCharacter(position, locationCharacter)
                : visibilityService!.IsCellPerceivedByCharacter(position, locationCharacter);

            Main.Info($"{locationCharacter.Name} => {position} : {scoringResult.Score}");
            scoringResult.Score = flag ? 1f : 0.0f;

            return false;
        }
    }
}
