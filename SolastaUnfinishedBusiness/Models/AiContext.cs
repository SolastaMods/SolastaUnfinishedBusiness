using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Builders;
using TA.AI;

namespace SolastaUnfinishedBusiness.Models;

internal static class AiContext
{
    internal static void Load()
    {
        BuildDecisionBreakFreeFromCondition("ConditionGrappledRestrainedIceBound");
        BuildDecisionBreakFreeFromCondition("ConditionGrappledRestrainedSpellWeb");
    }

    private static void BuildDecisionBreakFreeFromCondition(string conditionName)
    {
        var baseDecision = DatabaseHelper.GetDefinition<DecisionDefinition>("BreakConcentration_FlyingInMelee");
        var decisionBreakFree = DecisionDefinitionBuilder
            .Create(baseDecision, $"DecisionBreakFree{conditionName}")
            .SetGuiPresentationNoContent(true)
            .AddToDB();

        decisionBreakFree.Decision.activityType = "BreakFree";
        decisionBreakFree.Decision.Scorer.considerations.RemoveAll(x =>
            x.consideration.name is "HasEnemyInMeleeRange" or "IsNotFlyingTooHigh");

        var consideration = decisionBreakFree.Decision.Scorer.considerations.FirstOrDefault(x =>
            x.consideration.name == "HasConditionFlying");

        if (consideration == null)
        {
            return;
        }

        consideration.consideration.name = $"Has{conditionName}";
        consideration.consideration.consideration.stringParameter = conditionName;

        foreach (var decisionPackageDefinition in DatabaseRepository.GetDatabase<DecisionPackageDefinition>())
        {
            decisionPackageDefinition.package.weightedDecisions.Add(
                new WeightedDecisionDescription(decisionBreakFree, 1, 0, false));
        }
    }
}
