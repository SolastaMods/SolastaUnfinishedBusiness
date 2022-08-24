using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.NotifyConditionRemoval;

//
// INotifyConditionRemoval
//
[HarmonyPatch(typeof(RulesetCharacter), "Kill")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetCharacter_Kill
{
    internal static void Prefix(RulesetCharacter __instance)
    {
        foreach (var rulesetCondition in __instance.ConditionsByCategory
                     .SelectMany(keyValuePair => keyValuePair.Value))
        {
            if (rulesetCondition?.ConditionDefinition is INotifyConditionRemoval notifiedDefinition)
            {
                notifiedDefinition.BeforeDyingWithCondition(__instance, rulesetCondition);
            }
        }
    }
}
