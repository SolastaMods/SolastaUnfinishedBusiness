using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using TA.AI;
using TA.AI.Considerations;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Models;

internal static class AiContext
{
    internal const string DoNothing = "1";
    internal const string DoStrengthCheckCasterDC = "2";

    internal static readonly List<string> DoNothingConditions =
        ["ConditionNoxiousSpray", "ConditionVileBrew", "ConditionGrappledRestrainedIceBound"];

    internal static readonly List<string> DoStrengthCheckCasterDCConditions =
    [
        "ConditionFlashFreeze", "ConditionGrappledRestrainedEnsnared",
        "ConditionGrappledRestrainedSpellWeb", "ConditionRestrainedByEntangle"
    ];

    internal static ActivityScorerDefinition CreateActivityScorer(
        ActivityScorerDefinition baseScorer, string name)
    {
        var result = Object.Instantiate(baseScorer);

        result.name = name;
        result.scorer = result.Scorer.DeepCopy();

        return result;
    }

    private static ConsiderationDefinition CreateConsiderationDefinition(
        string name, ConsiderationDescription consideration)
    {
        var baseDescription = FixesContext.DecisionMoveAfraid.Decision.Scorer.WeightedConsiderations[0]
            .ConsiderationDefinition;

        baseDescription.name = name;
        baseDescription.consideration = consideration;

        return Object.Instantiate(baseDescription);
    }

    internal static void Load()
    {
        // order matters as same weight
        // this code needs a refactoring. meanwhile check:
        // - CharacterActionPanelPatcher SelectBreakFreeMode and add condition there if spell also aims allies
        foreach (var condition in DoNothingConditions)
        {
            BuildDecisionBreakFreeFromCondition(condition, DoNothing);
        }

        foreach (var condition in DoStrengthCheckCasterDCConditions)
        {
            BuildDecisionBreakFreeFromCondition(condition, DoStrengthCheckCasterDC);
        }
    }

    // boolParameter false won't do any ability check
    private static void BuildDecisionBreakFreeFromCondition(string conditionName, string action)
    {
        var mainActionNotFullyConsumed = new WeightedConsiderationDescription
        {
            consideration = CreateConsiderationDefinition(
                "MainActionNotFullyConsumed",
                new ConsiderationDescription
                {
                    considerationType = nameof(HasCondition), boolParameter = true, floatParameter = 1f
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

        // create scorer copy

        var baseDecision = DatabaseHelper.GetDefinition<DecisionDefinition>("BreakConcentration_FlyingInMelee");
        var scorerBreakFree = CreateActivityScorer(baseDecision.Decision.scorer, "BreakFree");

        scorerBreakFree.scorer.considerations = [hasConditionBreakFree, mainActionNotFullyConsumed];

        // create and assign decision definition to all decision packages

        var decisionBreakFree = DecisionDefinitionBuilder
            .Create($"DecisionBreakFree{conditionName}")
            .SetGuiPresentationNoContent(true)
            .SetDecisionDescription(
                "if restrained and can use main action, try to break free",
                "BreakFree",
                scorerBreakFree,
                action,
                enumParameter: 1,
                floatParameter: 3f)
            .AddToDB();

        foreach (var decisionPackageDefinition in DatabaseRepository.GetDatabase<DecisionPackageDefinition>())
        {
            decisionPackageDefinition.package.weightedDecisions.Add(
                new WeightedDecisionDescription(decisionBreakFree, 1, 0, false));
        }
    }
}
