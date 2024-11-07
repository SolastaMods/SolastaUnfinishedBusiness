using System;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using TA.AI;
using TA.AI.Activities;
using TA.AI.Considerations;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = TA.AI.Considerations.Random;

namespace SolastaUnfinishedBusiness.Api.Helpers;

internal static class AiHelpers
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

    internal static DecisionPackageDefinition BuildDecisionPackageBreakFree(
        string conditionName, RandomType randomType = RandomType.RandomMediumHigh)
    {
        var getDefinition = DatabaseHelper.GetDefinition<DecisionDefinition>;
        var baseDecision = getDefinition("BreakConcentration_FlyingInMelee");
        var decisionWithRandom = randomType switch
        {
            RandomType.RandomMediumLow => getDefinition("Move_RestlessLightSensitive"),
            RandomType.RandomMedium => getDefinition("CastMagic_Blindness"),
            RandomType.RandomMediumHigh => getDefinition("CastMagic_Buff_AoE"),
            _ => null
        };

        //
        // common consideration to validate if main action there
        //

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

        var mainActionNotFullyConsumedIfProne = new WeightedConsiderationDescription(
            CreateConsiderationDefinition(
                "MainActionNotFullyConsumedIfProne",
                new ConsiderationDescription
                {
                    considerationType = nameof(ActionTypeStatus),
                    curve = wcdActionTypeStatus.Consideration.curve,
                    boolParameter = true,
                    floatParameter = 1f
                }), 1f);

        //
        // Decision that might use a random consideration
        //

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

        var scorerBreakFree = CreateActivityScorer(baseDecision, $"BreakFree{conditionName}", true,
            hasConditionBreakFree,
            mainActionNotFullyConsumed);

        if (decisionWithRandom)
        {
            var wcdRandom = GetWeightedConsiderationDescriptionByDecisionAndConsideration(
                decisionWithRandom, "Random");

            var random = new WeightedConsiderationDescription(
                CreateConsiderationDefinition(
                    $"Random{randomType}",
                    new ConsiderationDescription
                    {
                        considerationType = nameof(Random), curve = wcdRandom.Consideration.curve
                    }), 1f);

            scorerBreakFree.Scorer.WeightedConsiderations.Add(random);
        }

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

        //
        // Decision that takes Prone into account and don't use a random consideration at all
        //

        var conditionProne = DatabaseHelper.ConditionDefinitions.ConditionProne;
        var hasConditionProne = new WeightedConsiderationDescription(
            CreateConsiderationDefinition(
                $"Has{conditionProne.Name}",
                new ConsiderationDescription
                {
                    considerationType = nameof(HasCondition),
                    curve = wcdHasCondition.Consideration.curve,
                    stringParameter = conditionProne.Name,
                    boolParameter = true,
                    intParameter = 2,
                    floatParameter = 2f
                }), 1f);

        var scorerBreakFreeIfProne = CreateActivityScorer(baseDecision, $"BreakFreeIfProne{conditionName}", true,
            hasConditionBreakFree,
            hasConditionProne,
            mainActionNotFullyConsumedIfProne);

        var decisionBreakFreeIfProne = DecisionDefinitionBuilder
            .Create($"DecisionBreakFreeIfProne{conditionName}")
            .SetGuiPresentationNoContent(true)
            .SetDecisionDescription(
                $"if restrained from {conditionName}, is prone, and can use main action, try to break free",
                nameof(BreakFree),
                scorerBreakFreeIfProne,
                enumParameter: 1,
                floatParameter: 3f)
            .AddToDB();

        // use weight 10f to ensure scenarios that don't prevent enemies from take actions to still consider this
        var packageBreakFree = DecisionPackageDefinitionBuilder
            .Create($"BreakFreeAbilityCheck{conditionName}")
            .SetGuiPresentationNoContent(true)
            .SetWeightedDecisions(
                new WeightedDecisionDescription(decisionBreakFree, 10f, 1, false))
            .AddToDB();

        // only add the break free if prone if vanilla break free has random otherwise it's redundant
        if (decisionWithRandom)
        {
            packageBreakFree.Package.WeightedDecisions.SetRange(
                new WeightedDecisionDescription(decisionBreakFreeIfProne, 15f, 1, false),
                new WeightedDecisionDescription(decisionBreakFree, 10f, 1, false));
        }

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

    internal enum RandomType
    {
        [UsedImplicitly] NoRandom,
        [UsedImplicitly] RandomMediumLow,
        [UsedImplicitly] RandomMedium,
        [UsedImplicitly] RandomMediumHigh
    }

    internal enum BreakFreeType
    {
        DoNoCheckAndRemoveCondition = 10,
        DoStrengthCheckAgainstCasterDC = 20,
        DoWisdomCheckAgainstCasterDC = 30,
        DoStrengthOrDexterityContestCheckAgainstStrengthAthletics = 40
    }
}
