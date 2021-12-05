using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using static RuleDefinitions;

namespace SolastaCommunityExpansion.Patches.SrdAndHouseRulesContext
{
    internal static class RuleDefinitionsPatcher
    {
        [HarmonyPatch(typeof(RuleDefinitions), "ComputeAdvantage")]
        internal static class RuleDefinitions_ComputeAdvantage_Patch
        {
            public static void Postfix(List<TrendInfo> trends, ref AdvantageType __result)
            {
                if (Main.Settings.EnableSRDAdvantageRules)
                {
                    var original = __result;

                    var hasAdvantage = trends.Any(t => t.value > 0);
                    var hasDisadvantage = trends.Any(t => t.value < 0);

                    if (!(hasAdvantage ^ hasDisadvantage))
                    {
                        __result = AdvantageType.None;
                    }
                    else if (hasAdvantage)
                    {
                        __result = AdvantageType.Advantage;
                    }
                    else
                    {
                        __result = AdvantageType.Disadvantage;
                    }

                    Main.Log($"ComputeAdvantage:{trends.Where(t => t.value > 0).Sum(t => (int?)t.value) ?? 0}, {hasAdvantage}, Disadvantage:{trends.Where(t => t.value < 0).Sum(t => (int?)t.value) ?? 0}, {hasDisadvantage}, original={original}, updated={__result}.");
                }
                else
                {
                    Main.Log($"ComputeAdvantage:{trends.Where(t => t.value > 0).Sum(t => (int?)t.value) ?? 0}, Disadvantage:{trends.Where(t => t.value < 0).Sum(t => (int?)t.value) ?? 0}, result={__result}.");
                }
            }
        }

        [HarmonyPatch(typeof(ActionModifier), "AttackAdvantageTrend", MethodType.Getter)]
        internal static class ActionModifier_AttackAdvantageTrend_Patch
        {
            public static bool Prefix(ref int __result, List<TrendInfo> ___attackAdvantageTrends)
            {
                if (Main.Settings.EnableSRDAdvantageRules)
                {
                    var advantage = ___attackAdvantageTrends.Any(t => t.value > 0) ? 1 : 0;
                    var disadvantage = ___attackAdvantageTrends.Any(t => t.value < 0) ? -1 : 0;

                    __result = advantage + disadvantage;

                    Main.Log($"AttackAdvantage:{___attackAdvantageTrends.Where(t => t.value > 0).Sum(t => (int?)t.value) ?? 0}, {advantage}, Disadvantage:{___attackAdvantageTrends.Where(t => t.value < 0).Sum(t => (int?)t.value) ?? 0}, {disadvantage}, updated={__result}.");
                    return false;
                }

                Main.Log($"AttackAdvantage:{___attackAdvantageTrends.Where(t => t.value > 0).Sum(t => (int?)t.value) ?? 0}, Disadvantage:{___attackAdvantageTrends.Where(t => t.value < 0).Sum(t => (int?)t.value) ?? 0}, result={__result}.");
                return true;
            }
        }
    }
}
