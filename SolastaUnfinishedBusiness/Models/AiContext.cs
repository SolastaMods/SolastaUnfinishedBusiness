using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
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
        result.scorer = result.Scorer.DeepCopy();

        result.scorer.WeightedConsiderations.SetRange(result.scorer.WeightedConsiderations.Select(x =>
            new WeightedConsiderationDescription
            {
                consideration =
                    CreateConsiderationDefinition(x.consideration.name, x.consideration.Consideration),
                weight = x.weight
            }).ToList());

        return result;
    }

    private static ConsiderationDefinition CreateConsiderationDefinition(
        string name, ConsiderationDescription consideration)
    {
        var baseDescription =
            FixesContext.DecisionMoveAfraid.Decision.Scorer.WeightedConsiderations[0].ConsiderationDefinition;

        var result = Object.Instantiate(baseDescription);

        result.name = name;
        result.consideration = consideration.DeepCopy();

        return result;
    }

    internal static void Load()
    {
        ConditionRestrainedByEntangle.amountOrigin = ConditionDefinition.OriginOfAmount.Fixed;
        ConditionRestrainedByEntangle.baseAmount = (int)BreakFreeType.DoStrengthCheckAgainstCasterDC;

        var battlePackage = BuildDecisionBreakFreeFromCondition(ConditionRestrainedByEntangle.Name,
            BreakFreeType.DoStrengthCheckAgainstCasterDC);

        ConditionRestrainedByEntangle.addBehavior = true;
        ConditionRestrainedByEntangle.battlePackage = battlePackage;
    }

    internal static DecisionPackageDefinition BuildDecisionBreakFreeFromCondition(
        string conditionName, BreakFreeType action)
    {
        var mainActionNotFullyConsumed = new WeightedConsiderationDescription
        {
            consideration = CreateConsiderationDefinition(
                "MainActionNotFullyConsumed",
                new ConsiderationDescription
                {
                    considerationType = nameof(ActionTypeStatus),
                    stringParameter = string.Empty,
                    boolParameter = true,
                    floatParameter = 1f
                }),
            weight = 1f
        };

        var hasConditionBreakFree = new WeightedConsiderationDescription
        {
            consideration = CreateConsiderationDefinition(
                $"Has{conditionName}",
                new ConsiderationDescription
                {
                    considerationType = nameof(HasCondition),
                    stringParameter = conditionName,
                    boolParameter = true,
                    intParameter = 2,
                    floatParameter = 2f
                }),
            weight = 1f
        };

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
                ((int)action).ToString(),
                enumParameter: 1,
                floatParameter: 3f)
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
