using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class ActionModifierPatcher
{
    [HarmonyPatch(typeof(ActionModifier), nameof(ActionModifier.AttackAdvantageTrend), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class AttackAdvantageTrend_Getter_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(ActionModifier __instance, ref int __result)
        {
            //PATCH: Apply SRD setting `UseOfficialAdvantageDisadvantageRules`
            if (!Main.Settings.UseOfficialAdvantageDisadvantageRules)
            {
                return true;
            }

            var advantage = __instance.AttackAdvantageTrends.Any(t => t.value > 0) ? 1 : 0;
            var disadvantage = __instance.AttackAdvantageTrends.Any(t => t.value < 0) ? -1 : 0;

            __result = advantage + disadvantage;

            return false;
        }
    }
}
