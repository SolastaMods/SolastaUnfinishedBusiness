using HarmonyLib;
using SolastaCommunityExpansion.CustomFeatureDefinitions;

namespace SolastaCommunityExpansion.Patches.NotifyConditionRemoval
{
    internal static class RulesetActorPatcher
    {
        [HarmonyPatch(typeof(RulesetActor), "RemoveCondition")]
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
}
