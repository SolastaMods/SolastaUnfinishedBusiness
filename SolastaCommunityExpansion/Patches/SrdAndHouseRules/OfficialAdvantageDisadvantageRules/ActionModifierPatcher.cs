using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.SrdAndHouseRules.OfficialAdvantageDisadvantageRules;

[HarmonyPatch(typeof(ActionModifier), "AttackAdvantageTrend", MethodType.Getter)]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class ActionModifier_AttackAdvantageTrend
{
    public static bool Prefix(ActionModifier __instance, ref int __result)
    {
        if (!Main.Settings.UseOfficialAdvantageDisadvantageRules)
        {
            return true;
        }

        var advantage = __instance.attackAdvantageTrends.Any(t => t.value > 0) ? 1 : 0;
        var disadvantage = __instance.attackAdvantageTrends.Any(t => t.value < 0) ? -1 : 0;

        __result = advantage + disadvantage;

        return false;
    }
}
