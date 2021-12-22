using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.CustomFeatureDefinitions;

namespace SolastaCommunityExpansion.Patches.NotifyConditionRemoval
{
    //
    // this patch shouldn't be protected
    //
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
