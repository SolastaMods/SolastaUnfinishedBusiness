using HarmonyLib;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using static RuleDefinitions;

namespace SolastaCommunityExpansion.Patches.SrdAndHouseRules
{
    [HarmonyPatch(typeof(RuleDefinitions), "ComputeAdvantage")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RuleDefinitions_ComputeAdvantage
    {
        public static void Postfix(List<TrendInfo> trends, ref AdvantageType __result)
        {
            if (Main.Settings.EnableSRDAdvantageRules)
            {
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
            }
        }
    }

    [HarmonyPatch(typeof(ActionModifier), "AttackAdvantageTrend", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ActionModifier_AttackAdvantageTrend
    {
        public static bool Prefix(ref int __result, List<TrendInfo> ___attackAdvantageTrends)
        {
            if (Main.Settings.EnableSRDAdvantageRules)
            {
                var advantage = ___attackAdvantageTrends.Any(t => t.value > 0) ? 1 : 0;
                var disadvantage = ___attackAdvantageTrends.Any(t => t.value < 0) ? -1 : 0;

                __result = advantage + disadvantage;

                return false;
            }

            return true;
        }
    }
}
