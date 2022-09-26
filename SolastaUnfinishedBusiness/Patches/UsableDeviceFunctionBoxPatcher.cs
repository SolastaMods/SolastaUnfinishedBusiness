using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches;

internal static class UsableDeviceFunctionBoxPatcher
{
    [HarmonyPatch(typeof(UsableDeviceFunctionBox), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Bind_Patch
    {
        internal static void Postfix(UsableDeviceFunctionBox __instance,
            RulesetItemDevice usableDevice,
            RulesetDeviceFunction usableDeviceFunction)
        {
            var deviceDescription = usableDevice.UsableDeviceDescription;
            var functionDescription = usableDeviceFunction.DeviceFunctionDescription;

            var advancement = (functionDescription.Type == DeviceFunctionDescription.FunctionType.Spell
                ? functionDescription.SpellDefinition.EffectDescription
                : functionDescription.FeatureDefinitionPower.EffectDescription).EffectAdvancement;

            if (deviceDescription.Usage != EquipmentDefinitions.ItemUsage.Charges
                || functionDescription.UseAffinity != DeviceFunctionDescription.FunctionUseAffinity.ChargeCost
                || advancement.EffectIncrementMethod != RuleDefinitions.EffectIncrementMethod.PerAdditionalSlotLevel
                || !functionDescription.CanOverchargeSpell
                || usableDevice.RemainingCharges <= functionDescription.UseAmount)
            {
                return;
            }

            __instance.overchargeButton.gameObject.SetActive(true);
        }
    }
}
