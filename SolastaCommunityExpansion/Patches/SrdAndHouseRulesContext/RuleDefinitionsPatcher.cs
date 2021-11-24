using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace SolastaCommunityExpansion.Patches
{
    [HarmonyPatch(typeof(RuleDefinitions), "ComputeAdvantage")]
    internal static class RuleDefinitions_ComputeAdvantage
    {
        public static void Postfix(List<RuleDefinitions.TrendInfo> trends, RuleDefinitions.AdvantageType __result)
        {
            if (Main.Settings.EnableSRDAdvantageRules)
            {
                var hasAdvantage = trends.Any(t => t.value > 0);
                var hasDisadvantage = trends.Any(t => t.value < 0);

                if (!(hasAdvantage ^ hasDisadvantage))
                {
                    __result = RuleDefinitions.AdvantageType.None;
                }
                else if (hasAdvantage)
                {
                    __result = RuleDefinitions.AdvantageType.Advantage;
                }
                else
                {
                    __result = RuleDefinitions.AdvantageType.Disadvantage;
                }
            }
        }
    }
}
