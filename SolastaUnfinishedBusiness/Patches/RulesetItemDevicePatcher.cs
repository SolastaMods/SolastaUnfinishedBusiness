using HarmonyLib;
using SolastaUnfinishedBusiness.Api.Extensions;

namespace SolastaUnfinishedBusiness.Patches;

public static class RulesetItemDevicePatcher
{
    [HarmonyPatch(typeof(RulesetItemDevice), "IsFunctionAvailable")]
    public static class IsFunctionAvailable_Patch
    {
        public static void Postfix(
            ref bool __result,
            RulesetDeviceFunction function,
            RulesetCharacter character)
        {
            if (!__result)
            {
                return;
            }

            var power = function.DeviceFunctionDescription?.FeatureDefinitionPower;

            if (power == null)
            {
                return;
            }

            __result = character.CanUsePower(power, false);
        }
    }
}
