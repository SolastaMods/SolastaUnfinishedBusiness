using System;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using TA.AI;
using TA.AI.Activities;
using TA.AI.Considerations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SolastaUnfinishedBusiness.Models;

internal static class AiContext
{
    internal static ActivityScorerDefinition CreateActivityScorer(
        DecisionDefinition baseDecision, string name,
        bool overwriteConsiderations = false,
        params WeightedConsiderationDescription[] considerations)
    {
        var result = Object.Instantiate(baseDecision.Decision.scorer);

        result.name = name;
        result.scorer = new ActivityScorer();

        if (!overwriteConsiderations)
        {
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var weightedConsideration in baseDecision.Decision.scorer.scorer.WeightedConsiderations)
            {
                var sourceDescription = weightedConsideration.Consideration;
                var targetDescription = new ConsiderationDescription
                {
                    considerationType = sourceDescription.considerationType,
                    curve = sourceDescription.curve,
                    boolParameter = sourceDescription.boolParameter,
                    boolSecParameter = sourceDescription.boolSecParameter,
                    boolTerParameter = sourceDescription.boolTerParameter,
                    byteParameter = sourceDescription.byteParameter,
                    intParameter = sourceDescription.intParameter,
                    floatParameter = sourceDescription.floatParameter,
                    stringParameter = sourceDescription.stringParameter
                };

                var weightedConsiderationDescription = new WeightedConsiderationDescription(
                    CreateConsiderationDefinition(weightedConsideration.ConsiderationDefinition.name,
                        targetDescription),
                    weightedConsideration.weight);

                result.Scorer.WeightedConsiderations.Add(weightedConsiderationDescription);
            }
        }

        result.Scorer.WeightedConsiderations.AddRange(considerations);

        return result;
    }

    private static ConsiderationDefinition CreateConsiderationDefinition(
        string name, ConsiderationDescription consideration)
    {
        var result = ScriptableObject.CreateInstance<ConsiderationDefinition>();

        result.name = name;
        result.consideration = consideration;

        return result;
    }

    private static WeightedConsiderationDescription GetWeightedConsiderationDescriptionByDecisionAndConsideration(
        DecisionDefinition decisionDefinition, string considerationType)
    {
        return decisionDefinition.Decision.Scorer.WeightedConsiderations
                   .FirstOrDefault(y => y.ConsiderationDefinition.Consideration.considerationType == considerationType)
               ?? throw new Exception();
    }

    internal static DecisionPackageDefinition BuildDecisionPackageBreakFree(string conditionName)
    {
        var baseDecision = DatabaseHelper.GetDefinition<DecisionDefinition>("BreakConcentration_FlyingInMelee");

        var wcdHasCondition = GetWeightedConsiderationDescriptionByDecisionAndConsideration(
            baseDecision, "HasCondition");

        var hasConditionBreakFree = new WeightedConsiderationDescription(
            CreateConsiderationDefinition(
                $"Has{conditionName}",
                new ConsiderationDescription
                {
                    considerationType = nameof(HasCondition),
                    curve = wcdHasCondition.Consideration.curve,
                    stringParameter = conditionName,
                    boolParameter = true,
                    intParameter = 2,
                    floatParameter = 2f
                }), 1f);

        var wcdActionTypeStatus = GetWeightedConsiderationDescriptionByDecisionAndConsideration(
            baseDecision, "ActionTypeStatus");

        var mainActionNotFullyConsumed = new WeightedConsiderationDescription(
            CreateConsiderationDefinition(
                "MainActionNotFullyConsumed",
                new ConsiderationDescription
                {
                    considerationType = nameof(ActionTypeStatus),
                    curve = wcdActionTypeStatus.Consideration.curve,
                    boolParameter = true,
                    floatParameter = 1f
                }), 1f);

        var scorerBreakFree = CreateActivityScorer(baseDecision, $"BreakFree{conditionName}", true,
            hasConditionBreakFree,
            mainActionNotFullyConsumed);

        var decisionBreakFree = DecisionDefinitionBuilder
            .Create($"DecisionBreakFree{conditionName}")
            .SetGuiPresentationNoContent(true)
            .SetDecisionDescription(
                $"if restrained from {conditionName}, and can use main action, try to break free",
                nameof(BreakFree),
                scorerBreakFree,
                enumParameter: 1,
                floatParameter: 3f)
            .AddToDB();

        // use weight 10f to ensure scenarios that don't prevent enemies from take actions to still consider this
        var packageBreakFree = DecisionPackageDefinitionBuilder
            .Create($"BreakFreeAbilityCheck{conditionName}")
            .SetGuiPresentationNoContent(true)
            .SetWeightedDecisions(new WeightedDecisionDescription(decisionBreakFree, 10f, 1, false))
            .AddToDB();

        return packageBreakFree;
    }

    internal static RulesetCondition GetRestrainingCondition(RulesetCharacter rulesetCharacter)
    {
        return rulesetCharacter
            .GetFeaturesByType<FeatureDefinitionActionAffinity>()
            .Where(actionAffinity => actionAffinity.AuthorizedActions.Contains(ActionDefinitions.Id.BreakFree))
            .Select(rulesetCharacter.FindFirstConditionHoldingFeature)
            .FirstOrDefault(rulesetCondition => rulesetCondition != null);
    }

    internal enum BreakFreeType
    {
        DoNoCheckAndRemoveCondition = 10,
        DoStrengthCheckAgainstCasterDC = 20,
        DoWisdomCheckAgainstCasterDC = 30,
        DoStrengthOrDexterityContestCheckAgainstStrengthAthletics = 40
    }
}
