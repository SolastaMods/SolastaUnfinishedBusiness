using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomBehaviors;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class UsableDeviceFunctionBoxPatcher
{
    [HarmonyPatch(typeof(UsableDeviceFunctionBox), nameof(UsableDeviceFunctionBox.Bind))]
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
            UpdateOverchargeForPowerDevice(__instance, usableDevice, usableDeviceFunction);
            UpdateUsabilityForPoisoner(__instance, usableDevice, usableDeviceFunction);
        }

        private static void UpdateOverchargeForPowerDevice(
            UsableDeviceFunctionBox box,
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

            box.overchargeButton.gameObject.SetActive(true);
        }

        private static void UpdateUsabilityForPoisoner(
            UsableDeviceFunctionBox box,
            RulesetItemDevice usableDevice,
            RulesetDeviceFunction rulesetDeviceFunction)
        {
            /*
             GetComponentInParent for some reason returns one layer deeper than needed - have TA added extra component by mistake?
             DeviceSelectionPanel component is present in
             `/Application/GUI/BackgroundCanvas/ForegroundCanvas/DeviceSelectionPanel`
             and its child
             `/Application/GUI/BackgroundCanvas/ForegroundCanvas/DeviceSelectionPanel/DeviceLinesTable`
             this makes GetComponentInParent<DeviceSelectionPanel>() called on UsableDeviceFunctionBox 
             return DeviceLinesTable's DeviceSelectionPanel component (which doesn't seem to be setup at all)
             instead of properly setup component of DeviceSelectionPanel object.
             This forces us to use chaining parent getters.
            */
            var panel = box.transform.parent.parent.parent.parent.GetComponent<DeviceSelectionPanel>();
            var actionType = panel.ActionType;
            if (actionType != ActionDefinitions.ActionType.Bonus)
            {
                return;
            }

            var user = box.GuiCharacter.RulesetCharacter;

            if (user.GetSubFeaturesByType<IActionPerformanceProvider>()
                .Any(f => ValidPerformanceProvider(f, user, usableDevice, rulesetDeviceFunction)))
            {
                return;
            }

            if (user.GetSubFeaturesByType<IAdditionalActionsProvider>()
                .Any(f => ValidAdditinalActionProvider(f, user, usableDevice, rulesetDeviceFunction)))
            {
                return;
            }

            box.button.interactable = false;
            box.canvasGroup.interactable = false;
            box.canvasGroup.alpha = 0.1f;
        }

        private static bool ValidPerformanceProvider(
            IActionPerformanceProvider provider,
            RulesetCharacter user,
            RulesetItemDevice device,
            RulesetDeviceFunction deviceFunction)
        {
            if (!provider.AuthorizedActions.Contains(ActionDefinitions.Id.UseItemBonus))
            {
                return false;
            }

            if (provider is not FeatureDefinition feature)
            {
                return false;
            }

            var validator = feature.GetFirstSubFeatureOfType<ValidateDeviceFunctionUse>();
            return validator == null || validator(user, device, deviceFunction);
        }

        private static bool ValidAdditinalActionProvider(
            IAdditionalActionsProvider provider,
            RulesetCharacter user,
            RulesetItemDevice device,
            RulesetDeviceFunction deviceFunction)
        {
            if (!provider.AuthorizedActions.Contains(ActionDefinitions.Id.UseItemBonus))
            {
                return false;
            }

            if (provider is not FeatureDefinition feature)
            {
                return false;
            }

            var validator = feature.GetFirstSubFeatureOfType<ValidateDeviceFunctionUse>();
            return validator == null || validator(user, device, deviceFunction);
        }
    }
}
