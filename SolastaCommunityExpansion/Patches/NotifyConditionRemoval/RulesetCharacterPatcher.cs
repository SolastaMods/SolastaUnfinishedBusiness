using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.CustomFeatureDefinitions;

namespace SolastaCommunityExpansion.Patches.NotifyConditionRemoval
{
    [HarmonyPatch(typeof(RulesetCharacter), "Kill")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
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
