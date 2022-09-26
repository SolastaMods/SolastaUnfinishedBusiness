using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

internal static class RulesetEffectPowerPatcher
{
    [HarmonyPatch(typeof(RulesetEffectPower), "SaveDC", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SaveDC_Getter_Patch
    {
        public static void Postfix(RulesetEffectPower __instance, ref int __result)
        {
            //PATCH: allow devices have DC based on user stats, instead of static value
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

    [HarmonyPatch(typeof(RulesetEffectPower), "EffectDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class EffectDescription_Getter_Patch
    {
        internal static void Postfix(RulesetEffectPower __instance, ref EffectDescription __result)
        {
            //PATCH: support for `ICustomMagicEffectBasedOnCaster` and `IModifySpellEffect` 
            // allowing to pick and/or tweak power effect depending on some properties of the user
            __result = CustomFeaturesContext.ModifyPowerEffect(__result, __instance);
        }
    }
}
