using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches;

internal static class ActionModifierPatcher
{
    //PATCH: Apply SRD setting `UseOfficialAdvantageDisadvantageRules`
    [HarmonyPatch(typeof(ActionModifier), "AttackAdvantageTrend", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class AttackAdvantageTrend_Getter_Patch
    {
        internal static bool Prefix(ActionModifier __instance, ref int __result)
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
}
