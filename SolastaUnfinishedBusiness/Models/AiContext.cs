using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Builders;
using TA.AI;

namespace SolastaUnfinishedBusiness.Models;

internal static class AiContext
{
    internal static void Load()
    {
        BuildDecisionBreakFreeSpellWeb();
    }

    private static void BuildDecisionBreakFreeSpellWeb()
    {
        var baseDecision = DatabaseHelper.GetDefinition<DecisionDefinition>("BreakConcentration_FlyingInMelee");
        var decisionBreakFree = DecisionDefinitionBuilder
            .Create(baseDecision, "DecisionBreakFreeSpellWeb")
            .SetGuiPresentationNoContent(true)
            .AddToDB();

        decisionBreakFree.Decision.activityType = "BreakFreeSpellWeb"; //"BreakFreeSpellWeb";
        decisionBreakFree.Decision.Scorer.considerations.RemoveAll(x =>
            x.consideration.name is "HasEnemyInMeleeRange" or "IsNotFlyingTooHigh");

        var consideration = decisionBreakFree.Decision.Scorer.considerations.FirstOrDefault(x =>
            x.consideration.name == "HasConditionFlying");

        if (consideration == null)
        {
            return;
        }

        consideration.consideration.name = "HasConditionGrappledRestrainedSpellWeb";
        consideration.consideration.consideration.stringParameter = "ConditionGrappledRestrainedSpellWeb";


        foreach (var decisionPackageDefinition in DatabaseRepository.GetDatabase<DecisionPackageDefinition>())
        {
            decisionPackageDefinition.package.weightedDecisions.Add(
                new WeightedDecisionDescription(decisionBreakFree, 1, 0, false));
        }
    }
}
