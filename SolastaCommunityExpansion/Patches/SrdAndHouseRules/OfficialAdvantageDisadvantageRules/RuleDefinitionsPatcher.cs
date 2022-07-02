using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using static RuleDefinitions;

namespace SolastaCommunityExpansion.Patches.SrdAndHouseRules.OfficialAdvantageDisadvantageRules;

[HarmonyPatch(typeof(RuleDefinitions), "ComputeAdvantage")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RuleDefinitions_ComputeAdvantage
{
    public static void Postfix(List<TrendInfo> trends, ref AdvantageType __result)
    {
        if (!Main.Settings.UseOfficialAdvantageDisadvantageRules)
        {
            return;
        }

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
