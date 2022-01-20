using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.CustomFeatureDefinitions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures
{
    // Yes, the actual method name has a typo
    [HarmonyPatch(typeof(RulesetActor), "ProcessConditionsMatchingOccurenceType")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetActor_ProcessConditionsMatchingOccurenceType
    {
        internal static void Postfix(RulesetActor __instance, RuleDefinitions.TurnOccurenceType occurenceType)
        {
            if (occurenceType != RuleDefinitions.TurnOccurenceType.StartOfTurn)
            {
                return;
            }

            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (battleService?.Battle == null)
            {
                return;
            }

            foreach (GameLocationCharacter contender in battleService.Battle.AllContenders
                .Where(x => x != null && x.Valid && x.RulesetActor != null))
            {
                var conditionsToRemove = new List<RulesetCondition>();

                foreach (KeyValuePair<string, List<RulesetCondition>> keyValuePair in contender.RulesetActor.ConditionsByCategory)
                {
                    foreach (RulesetCondition rulesetCondition in keyValuePair.Value)
                    {
                        if (rulesetCondition.SourceGuid == __instance.Guid && rulesetCondition.ConditionDefinition is IConditionRemovedOnSourceTurnStart)
                        {
                            conditionsToRemove.Add(rulesetCondition);
                        }
                    }
                }

                foreach (RulesetCondition conditionToRemove in conditionsToRemove)
                {
                    contender.RulesetActor.RemoveCondition(conditionToRemove);
                }
            }
        }
    }

    [HarmonyPatch(typeof(RulesetActor), "InflictCondition")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]

    internal static class RulesetActor_InflictCondition
    {

        internal static void Prefix(string conditionDefinitionName,
            ref int sourceAmount)
        {
            if (RulesetImplementationManagerLocation_ApplySummonForm.ConditionToAmount.ContainsKey(conditionDefinitionName))
            {
                sourceAmount = RulesetImplementationManagerLocation_ApplySummonForm.ConditionToAmount[conditionDefinitionName];
            }
        }
    }

    [HarmonyPatch(typeof(RulesetActor), "RemoveCondition")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetActor_RemoveCondition
    {
        internal static void Postfix(RulesetActor __instance, RulesetCondition rulesetCondition)
        {
            var notifiedDefinition = rulesetCondition?.ConditionDefinition as INotifyConditionRemoval;

            if (notifiedDefinition != null)
            {
                notifiedDefinition.AfterConditionRemoved(__instance, rulesetCondition);
            }
        }
    }
}
