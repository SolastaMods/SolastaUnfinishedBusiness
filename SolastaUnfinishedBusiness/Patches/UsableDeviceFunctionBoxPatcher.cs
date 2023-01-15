using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class UsableDeviceFunctionBoxPatcher
{
    [HarmonyPatch(typeof(UsableDeviceFunctionBox), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
            UsableDeviceFunctionBox __instance,
            RulesetItemDevice usableDevice,
            RulesetDeviceFunction usableDeviceFunction)
        {
            var deviceDescription = usableDevice.UsableDeviceDescription;
            var functionDescription = usableDeviceFunction.DeviceFunctionDescription;
            var power = functionDescription.FeatureDefinitionPower;

            IMagicEffect magic = functionDescription.Type == DeviceFunctionDescription.FunctionType.Spell
                ? functionDescription.SpellDefinition
                : power;

            var advancement = magic.EffectDescription.EffectAdvancement;
            var canOvercharge = functionDescription.CanOverchargeSpell;
            var minCharge = 1;

            if (power != null)
            {
                var provider = power.GetFirstSubFeatureOfType<ICustomOverchargeProvider>();

                if (provider != null)
                {
                    var steps = provider.OverchargeSteps(Global.CurrentCharacter);

                    if (steps == null || steps.Length < 1)
                    {
                        canOvercharge = false;
                    }
                    else
                    {
                        minCharge = steps[0].Item1;
                    }
                }
            }

            if (deviceDescription.Usage != EquipmentDefinitions.ItemUsage.Charges
                || functionDescription.UseAffinity != DeviceFunctionDescription.FunctionUseAffinity.ChargeCost
                || advancement.EffectIncrementMethod != RuleDefinitions.EffectIncrementMethod.PerAdditionalSlotLevel
                || !canOvercharge
                || usableDevice.RemainingCharges < functionDescription.UseAmount + minCharge)
            {
                return;
            }

            __instance.overchargeButton.gameObject.SetActive(true);
        }
    }
}
