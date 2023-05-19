using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Builders;
using TA.AI;

namespace SolastaUnfinishedBusiness.Models;

internal static class AiContext
{
    internal static void Load()
    {
        BuildDecisionBreakFreeFromCondition("ConditionGrappledRestrainedIceBound", false);
        BuildDecisionBreakFreeFromCondition("ConditionGrappledRestrainedSpellWeb");
    }

    // boolParameter false won't do any ability check
    private static void BuildDecisionBreakFreeFromCondition(string conditionName, bool boolParameter = true)
    {
        var baseDecision = DatabaseHelper.GetDefinition<DecisionDefinition>("BreakConcentration_FlyingInMelee");
        var decisionBreakFree = DecisionDefinitionBuilder
            .Create(baseDecision, $"DecisionBreakFree{conditionName}")
            .SetGuiPresentationNoContent(true)
            .AddToDB();

        decisionBreakFree.Decision.activityType = "BreakFree";
        decisionBreakFree.Decision.Scorer.considerations.RemoveAll(x =>
            x.consideration.name is "HasEnemyInMeleeRange" or "IsNotFlyingTooHigh");
        decisionBreakFree.Decision.boolParameter = boolParameter;

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
