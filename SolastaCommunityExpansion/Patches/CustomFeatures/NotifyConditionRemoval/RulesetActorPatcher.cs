using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.NotifyConditionRemoval
{
    //
    // INotifyConditionRemoval
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
}
