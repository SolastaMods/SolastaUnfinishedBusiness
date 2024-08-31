using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using TA.AI;
using TA.AI.Activities;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class ActivitiesBreakFreePatcher
{
    [HarmonyPatch(typeof(BreakFree), nameof(BreakFree.ExecuteImpl))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class BreakFree_Patch
    {
        [UsedImplicitly]
        public static IEnumerator Postfix(
            [NotNull] IEnumerator values,
            [NotNull] BreakFree __instance,
            AiLocationCharacter character,
            DecisionDefinition decisionDefinition)
        {
            RulesetCondition restrainingCondition = null;

            var gameLocationCharacter = character.GameLocationCharacter;
            var rulesetCharacter = gameLocationCharacter.RulesetCharacter;

            if (rulesetCharacter == null)
            {
                yield break;
            }

            foreach (var definitionActionAffinity in rulesetCharacter
                         .GetFeaturesByType<FeatureDefinitionActionAffinity>()
                         .Where(x => x.AuthorizedActions.Contains(ActionDefinitions.Id.BreakFree)))
            {
                restrainingCondition = rulesetCharacter.FindFirstConditionHoldingFeature(definitionActionAffinity);
            }

            if (restrainingCondition == null)
            {
                yield break;
            }

            var success = true;

            // no ability check
            switch (decisionDefinition.Decision.stringParameter)
            {
                case AiContext.DoNothing:
                    rulesetCharacter.RemoveCondition(restrainingCondition);
                    break;

                case AiContext.DoStrengthCheckCasterDC:
                    var checkDC = 10;
                    var sourceGuid = restrainingCondition.SourceGuid;

                    if (RulesetEntity.TryGetEntity(sourceGuid, out RulesetCharacterHero rulesetCharacterHero))
                    {
                        checkDC = rulesetCharacterHero.SpellRepertoires
                            .Select(x => x.SaveDC)
                            .Max();
                    }

                    var actionModifier = new ActionModifier();

                    rulesetCharacter.ComputeBaseAbilityCheckBonus(
                        AttributeDefinitions.Strength, actionModifier.AbilityCheckModifierTrends, string.Empty);
                    gameLocationCharacter.ComputeAbilityCheckActionModifier(
                        AttributeDefinitions.Strength, string.Empty, actionModifier);

                    var abilityCheckRoll = gameLocationCharacter.RollAbilityCheck(
                        AttributeDefinitions.Strength, string.Empty, checkDC, AdvantageType.None, actionModifier,
                        false, -1, out var rollOutcome, out var successDelta, true);

                    //PATCH: support for Bardic Inspiration roll off battle and ITryAlterOutcomeAttributeCheck
                    var abilityCheckData = new AbilityCheckData
                    {
                        AbilityCheckRoll = abilityCheckRoll,
                        AbilityCheckRollOutcome = rollOutcome,
                        AbilityCheckSuccessDelta = successDelta,
                        AbilityCheckActionModifier = actionModifier
                    };

                    yield return TryAlterOutcomeAttributeCheck
                        .HandleITryAlterOutcomeAttributeCheck(gameLocationCharacter, abilityCheckData);

                    success = abilityCheckData.AbilityCheckRollOutcome
                        is RollOutcome.Success
                        or RollOutcome.CriticalSuccess;

                    if (success)
                    {
                        rulesetCharacter.RemoveCondition(restrainingCondition);
                    }

                    break;

                default:
                    while (values.MoveNext())
                    {
                        yield return values.Current;
                    }

                    yield break;
            }

            gameLocationCharacter.SpendActionType(ActionDefinitions.ActionType.Main);

            var breakFreeExecuted = rulesetCharacter.BreakFreeExecuted;

            breakFreeExecuted?.Invoke(rulesetCharacter, success);
        }
    }
}
