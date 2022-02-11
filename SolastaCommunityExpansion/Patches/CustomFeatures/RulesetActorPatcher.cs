using System.Collections.Concurrent;
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

    //
    // INotifyConditionRemoval patches
    //

    [HarmonyPatch(typeof(RulesetActor), "RemoveCondition")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetActor_RemoveCondition
    {
        internal static void Postfix(RulesetActor __instance, RulesetCondition rulesetCondition)
        {
            if (rulesetCondition?.ConditionDefinition is INotifyConditionRemoval notifiedDefinition)
            {
                notifiedDefinition.AfterConditionRemoved(__instance, rulesetCondition);
            }
        }
    }

    [HarmonyPatch(typeof(RulesetActor), "RollDie")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetActor_RollDie
    {
        private static readonly ConcurrentDictionary<RulesetActor, int> NextAbilityCheckMinimum = new();

        internal static void Postfix(
            RulesetActor __instance,
            RuleDefinitions.DieType dieType,
            RuleDefinitions.RollContext rollContext,
            ref int __result)
        {
            if (dieType != RuleDefinitions.DieType.D20 || rollContext != RuleDefinitions.RollContext.AbilityCheck)
            {
                return;
            }

            // This will only come up when RulesetCharacter.ResolveContestCheck is called (usually for shove checks).
            // The ResolveContestCheck patch checks for what the minimum die roll should be when RollDie is called.

            if (!NextAbilityCheckMinimum.TryRemove(__instance, out int minimum))
            {
                // There isn't an entry for the current instance; do nothing
                return;
            }

            if (minimum > __result)
            {
                __result = minimum;
            }
        }

        internal static void SetNextAbilityCheckMinimum(RulesetActor actor, int minimum)
        {
            if (actor == null)
            {
                return;
            }

            NextAbilityCheckMinimum[actor] = minimum;
        }
    }
}
