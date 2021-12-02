using System.Collections.Generic;
using HarmonyLib;
using SolastaCommunityExpansion.CustomFeatureDefinitions;

namespace SolastaCommunityExpansion.Patches.ConditionRemovedOnSourceTurnStart
{
    internal static class RulesetActorPatcher
    {
        // Yes, the actual method name has a typo
        [HarmonyPatch(typeof(RulesetActor), "ProcessConditionsMatchingOccurenceType")]
        internal static class RulesetActor_ProcessConditionsMatchingOccurenceType
        {
            internal static void Postfix(RulesetActor __instance, RuleDefinitions.TurnOccurenceType occurenceType)
            {
                if (occurenceType == RuleDefinitions.TurnOccurenceType.StartOfTurn)
                {
                    var battleService = ServiceRepository.GetService<IGameLocationBattleService>();

                    if (battleService?.Battle != null)
                    {
                        foreach (GameLocationCharacter contender in battleService.Battle.AllContenders)
                        {
                            if (contender == null || !contender.Valid || contender.RulesetActor == null)
                            {
                                continue;
                            }

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
            }
        }
    }
}
