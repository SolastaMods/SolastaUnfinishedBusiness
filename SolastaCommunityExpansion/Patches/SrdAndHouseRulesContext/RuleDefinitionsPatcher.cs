using HarmonyLib;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SolastaCommunityExpansion.Patches.SrdAndHouseRulesContext
{
    internal static class RuleDefinitionsPatcher
    {
        [HarmonyPatch(typeof(RuleDefinitions), "ComputeAdvantage")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class RuleDefinitions_ComputeAdvantage_Patch
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
}
