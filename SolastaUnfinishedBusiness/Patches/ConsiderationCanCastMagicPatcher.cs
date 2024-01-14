using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using TA.AI;
using TA.AI.Considerations;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class ConsiderationCanCastMagicPatcher
{
    [HarmonyPatch(typeof(CanCastMagic), nameof(CanCastMagic.Score))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CanCastMagic_Patch
    {
        [UsedImplicitly]
#pragma warning disable IDE0060
        public static bool Prefix(
            DecisionContext context,
            ConsiderationDescription consideration,
            DecisionParameters parameters,
            ScoringResult scoringResult)
#pragma warning restore IDE0060
        {
            var enumParameter =
                (RuleDefinitions.MagicType)parameters.situationalInformation.DecisionDefinition.Decision.EnumParameter;
            var stringParameter = parameters.situationalInformation.DecisionDefinition.Decision.StringParameter;
            var boolParameter = parameters.situationalInformation.DecisionDefinition.Decision.BoolParameter;
            var floatParameter = consideration.FloatParameter;
            var locationCharacter = parameters.character.GameLocationCharacter;

            foreach (var availableMagicEffect in parameters.situationalInformation.AvailableMagicEffects
                         .Where(availableMagicEffect =>
                             AiLocationDefinitions.IsValidMagicEffect(
                                 parameters.character,
                                 locationCharacter.LocationPosition,
                                 parameters.situationalInformation.GameLocationService,
                                 parameters.situationalInformation.BattleService,
                                 availableMagicEffect,
                                 enumParameter,
                                 stringParameter,
                                 true,
                                 optionalTarget: context.character)))
            {
                // BEGIN PATCH
                if (Main.Settings.UseOfficialObscurementRules &&
                    context.character != null &&
                    !locationCharacter.CanPerceiveTarget(context.character))
                {
                    Main.Info($"{locationCharacter.Name} => {availableMagicEffect.Name} : OBSCUREMENT DISCARDED");

                    continue;
                }
                // END PATCH

                if (availableMagicEffect.EffectDescription.IsAoE)
                {
                    AiLocationDefinitions.ComputeAoETargets(
                        context.position, availableMagicEffect,
                        locationCharacter,
                        locationCharacter.LocationPosition,
                        parameters.situationalInformation.PositioningService,
                        parameters.situationalInformation.TargetingService,
                        parameters.situationalInformation.VisibilityService,
                        parameters.situationalInformation.CharactersCache,
                        null);

                    var aoeRawScore = AiLocationDefinitions.ComputeAoERawScore(
                        locationCharacter,
                        availableMagicEffect,
                        parameters.situationalInformation.CharactersCache,
                        boolParameter);

                    var num = GameConfiguration.AI.AreaOfEffectVolumeScoreModifier.Evaluate(
                        availableMagicEffect.EffectDescription.Volume);

                    var b = floatParameter > 0.0
                        ? Mathf.Clamp(aoeRawScore * num / floatParameter, 0.0f, 1f)
                        : Mathf.Clamp(aoeRawScore * num, 0.0f, 1f);

                    scoringResult.Score = Mathf.Max(scoringResult.Score, b);

                    Main.Info($"{locationCharacter.Name} => {availableMagicEffect.Name} : {scoringResult.Score}");
                }
                else if (context.character != null)
                {
                    if (!AiLocationDefinitions.IsRelevantTargetForEffect(
                            locationCharacter, context.character, availableMagicEffect.EffectDescription))
                    {
                        Main.Info($"{locationCharacter.Name} => {availableMagicEffect.Name} : NOT RELEVANT");

                        continue;
                    }

                    parameters.situationalInformation.ActionModifier.Reset();

                    var attackParams = new BattleDefinitions.AttackEvaluationParams();
                    var flag = attackParams.FillForMagic(
                        locationCharacter,
                        locationCharacter.LocationPosition,
                        availableMagicEffect.EffectDescription,
                        availableMagicEffect.Name,
                        context.character,
                        context.position,
                        parameters.situationalInformation.ActionModifier);

                    if (!parameters.situationalInformation.BattleService.CanAttack(attackParams))
                    {
                        continue;
                    }

                    scoringResult.Score =
                        !flag || parameters.situationalInformation.ActionModifier.AttackAdvantageTrend > 0
                            ? Mathf.Max(1f, scoringResult.Score)
                            : parameters.situationalInformation.ActionModifier.AttackAdvantageTrend >= 0
                                ? Mathf.Max(0.6666667f, scoringResult.Score)
                                : Mathf.Max(0.33333334f, scoringResult.Score);

                    Main.Info($"{locationCharacter.Name} => {availableMagicEffect.Name} : {scoringResult.Score}");
                }
                else if (availableMagicEffect.EffectDescription.TargetType == RuleDefinitions.TargetType.Position &&
                         availableMagicEffect.EffectDescription.RangeType == RuleDefinitions.RangeType.Distance)
                {
                    var magnitude = (context.position - locationCharacter.LocationPosition).magnitude;

                    scoringResult.Score = magnitude <= (double)availableMagicEffect.EffectDescription.RangeParameter
                        ? 1f
                        : 0.0f;

                    Main.Info($"{locationCharacter.Name} => {availableMagicEffect.Name} : {scoringResult.Score}");
                }
                else
                {
                    Trace.LogAssertion(
                        $"-AI-Magic effect not handled {availableMagicEffect.Name} - {availableMagicEffect.EffectDescription.RangeType}.");

                    scoringResult.Score = 1f;
                }
            }

            return false;
        }
    }
}
