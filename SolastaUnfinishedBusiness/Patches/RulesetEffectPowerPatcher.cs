using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.CustomBehaviors;

namespace SolastaUnfinishedBusiness.Patches;

internal static class RulesetEffectPowerPatcher
{
    [HarmonyPatch(typeof(RulesetEffectPower), "SaveDC", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class get_SaveDC_Patch
    {
        public static void Postfix(RulesetEffectPower __instance, ref int __result)
        {
            var originItem = __instance.OriginItem;

            if (originItem == null || originItem.UsableDeviceDescription.SaveDC != -1)
            {
                return;
            }

            var usablePower = __instance.UsablePower;

            UsablePowersProvider.UpdateSaveDc(__instance.User, usablePower);
            __result = usablePower.SaveDC;
        }
    }
}
