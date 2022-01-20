using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.CustomFeatureDefinitions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures
{
    //
    // this patch shouldn't be protected
    //
    [HarmonyPatch(typeof(RulesetCharacter), "Kill")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacter_Kill
    {
        internal static void Prefix(RulesetCharacter __instance)
        {
            foreach (var keyValuePair in __instance.ConditionsByCategory)
            {
                foreach (RulesetCondition rulesetCondition in keyValuePair.Value)
                {
                    if (rulesetCondition?.ConditionDefinition is INotifyConditionRemoval notifiedDefinition)
                    {
                        notifiedDefinition.BeforeDyingWithCondition(__instance, rulesetCondition);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), "RechargePowersForTurnStart")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacter_RechargePowersForTurnStart
    {
        internal static void Postfix(RulesetCharacter __instance)
        {
            foreach (RulesetUsablePower usablePower in __instance.UsablePowers)
            {
                if (usablePower?.PowerDefinition is IStartOfTurnRecharge startOfTurnRecharge && usablePower.RemainingUses < usablePower.MaxUses)
                {
                    usablePower.Recharge();

                    if (!startOfTurnRecharge.IsRechargeSilent && __instance.PowerRecharged != null)
                    {
                        __instance.PowerRecharged(__instance, usablePower);
                    }
                }
            }
        }
    }
}
