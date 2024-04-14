using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class RulesetItemDevicePatcher
{
    [HarmonyPatch(typeof(RulesetItemDevice), nameof(RulesetItemDevice.IsFunctionAvailable))]
    [UsedImplicitly]
    public static class IsFunctionAvailable_Patch
    {
        [UsedImplicitly]
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

            if (!power)
            {
                return;
            }

            __result = character.CanUsePower(power, false);
        }
    }
}
