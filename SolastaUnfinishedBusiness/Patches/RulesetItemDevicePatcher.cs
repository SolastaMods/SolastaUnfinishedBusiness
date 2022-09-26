using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.Patches;

internal static class RulesetItemDevicePatcher
{
    [HarmonyPatch(typeof(RulesetItemDevice), "IsFunctionAvailable")]
    internal static class IsFunctionAvailable_Patch
    {
        internal static void Postfix([NotNull] RulesetItemDevice __instance,
            ref bool __result,
            RulesetDeviceFunction function,
            RulesetCharacter character,
            bool inCombat,
            bool usedMainSpell,
            bool usedBonusSpell,
            ref string failureFlag)
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

            var validator = power.GetFirstSubFeatureOfType<IPowerUseValidity>();


            if (validator == null)
            {
                return;
            }

            __result = validator.CanUsePower(character);
        }
    }
}