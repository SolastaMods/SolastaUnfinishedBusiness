using System.Collections.Generic;
using HarmonyLib;
using SolastaCommunityExpansion.CustomFeatureDefinitions;

namespace SolastaCommunityExpansion.Patches.NotifyConditionRemoval
{
    internal static class RulesetCharacterPatcher
    {
        [HarmonyPatch(typeof(RulesetCharacter), "Kill")]
        internal static class RulesetCharacter_Kill
        {
            internal static void Prefix(RulesetCharacter __instance)
            {
                foreach (KeyValuePair<string, List<RulesetCondition>> keyValuePair in __instance.ConditionsByCategory)
                {
                    foreach (RulesetCondition rulesetCondition in keyValuePair.Value)
                    {
                        var notifiedDefinition = rulesetCondition?.ConditionDefinition as INotifyConditionRemoval;

                        if (notifiedDefinition != null)
                        {
                            notifiedDefinition.BeforeDyingWithCondition(__instance, rulesetCondition);
                        }
                    }
                }
            }
        }
    }
}
