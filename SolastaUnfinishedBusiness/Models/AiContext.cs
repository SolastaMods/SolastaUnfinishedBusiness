using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Builders;
using TA.AI;
using TA.AI.Activities;
using TA.AI.Considerations;
using UnityEngine;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;

namespace SolastaUnfinishedBusiness.Models;

internal static class AiContext
{
    internal static ActivityScorerDefinition CreateActivityScorer(
        ActivityScorerDefinition baseScorer, string name)
    {
        var result = Object.Instantiate(baseScorer);

        result.name = name;
        result.scorer = new ActivityScorer();

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var weightedConsideration in baseScorer.scorer.WeightedConsiderations)
        {
            var sourceDescription = weightedConsideration.Consideration;
            var targetDescription = new ConsiderationDescription
            {
                considerationType = sourceDescription.considerationType,
                curve = AnimationCurve.Constant(0f, 1f, 1f),
                boolParameter = sourceDescription.boolParameter,
                boolSecParameter = sourceDescription.boolSecParameter,
                boolTerParameter = sourceDescription.boolTerParameter,
                byteParameter = sourceDescription.byteParameter,
                intParameter = sourceDescription.intParameter,
                floatParameter = sourceDescription.floatParameter,
                stringParameter = sourceDescription.stringParameter
            };

            var weightedConsiderationDescription = new WeightedConsiderationDescription(
                CreateConsiderationDefinition(weightedConsideration.ConsiderationDefinition.name, targetDescription),
                weightedConsideration.weight);

            result.scorer.WeightedConsiderations.Add(weightedConsiderationDescription);
        }

        return result;
    }

    private static ConsiderationDefinition CreateConsiderationDefinition(
        string name, ConsiderationDescription consideration)
    {
        var baseDescription =
            FixesContext.DecisionMoveAfraid.Decision.Scorer.WeightedConsiderations[0].ConsiderationDefinition;

        var result = Object.Instantiate(baseDescription);

        result.name = name;
        result.consideration = consideration;

        return result;
    }

    internal static void Load()
    {
        ConditionRestrainedByEntangle.amountOrigin = ConditionDefinition.OriginOfAmount.Fixed;
        ConditionRestrainedByEntangle.baseAmount = (int)BreakFreeType.DoStrengthCheckAgainstCasterDC;

        var battlePackage = BuildDecisionPackageBreakFree(
            ConditionRestrainedByEntangle.Name, BreakFreeType.DoStrengthCheckAgainstCasterDC);

        ConditionRestrainedByEntangle.addBehavior = true;
        ConditionRestrainedByEntangle.battlePackage = battlePackage;
    }

    internal static DecisionPackageDefinition BuildDecisionPackageBreakFree(string conditionName, BreakFreeType action)
    {
        var mainActionNotFullyConsumed = new WeightedConsiderationDescription(
            CreateConsiderationDefinition(
                "MainActionNotFullyConsumed",
                new ConsiderationDescription
                {
                    considerationType = nameof(ActionTypeStatus),
                    stringParameter = string.Empty,
                    boolParameter = true,
                    floatParameter = 1f
                }), 1f);

        var hasConditionBreakFree = new WeightedConsiderationDescription(
            CreateConsiderationDefinition(
                $"Has{conditionName}",
                new ConsiderationDescription
                {
                    considerationType = nameof(HasCondition),
                    stringParameter = conditionName,
                    boolParameter = true,
                    intParameter = 2,
                    floatParameter = 2f
                }), 1f);

        // create scorer

        var baseDecision = DatabaseHelper.GetDefinition<DecisionDefinition>("BreakConcentration_FlyingInMelee");
        var scorerBreakFree = CreateActivityScorer(baseDecision.Decision.scorer, $"BreakFree{conditionName}");

        scorerBreakFree.scorer.considerations = [hasConditionBreakFree, mainActionNotFullyConsumed];

        var decisionBreakFree = DecisionDefinitionBuilder
            .Create($"DecisionBreakFree{conditionName}")
            .SetGuiPresentationNoContent(true)
            .SetDecisionDescription(
                $"if restrained from {conditionName}, and can use main action, try to break free",
                nameof(BreakFree),
                scorerBreakFree,
                enumParameter: (int)action)
            .AddToDB();

        var packageBreakFree = DecisionPackageDefinitionBuilder
            .Create($"BreakFreeAbilityCheck{conditionName}")
            .SetGuiPresentationNoContent(true)
            .SetWeightedDecisions(new WeightedDecisionDescription { decision = decisionBreakFree, weight = 1f })
            .AddToDB();

        return packageBreakFree;
    }

    internal enum BreakFreeType
    {
        DoNothing,
        DoStrengthCheckAgainstCasterDC
    }
}
