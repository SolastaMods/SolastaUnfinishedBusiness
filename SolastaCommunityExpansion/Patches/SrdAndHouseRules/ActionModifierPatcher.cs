using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using static RuleDefinitions;

namespace SolastaCommunityExpansion.Patches.SrdAndHouseRules
{
    [HarmonyPatch(typeof(ActionModifier), "AttackAdvantageTrend", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ActionModifier_AttackAdvantageTrend
    {
        public static bool Prefix(ref int __result, List<TrendInfo> ___attackAdvantageTrends)
        {
            if (Main.Settings.UseOfficialAdvantageDisadvantageRules)
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
