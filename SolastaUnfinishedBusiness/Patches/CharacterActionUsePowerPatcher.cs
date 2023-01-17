using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterActionUsePowerPatcher
{
    [HarmonyPatch(typeof(CharacterActionUsePower), nameof(CharacterActionUsePower.CheckInterruptionBefore))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CheckInterruptionBefore_Patch
    {
        [UsedImplicitly]
        public static bool Prefix([NotNull] CharacterActionUsePower __instance)
        {
            //PATCH: ignores interruptions processing for certain powers so they won't interrupt invisibility
            return !Global.PowersThatIgnoreInterruptions.Contains(__instance.activePower.PowerDefinition);
        }
    }

    [HarmonyPatch(typeof(CharacterActionUsePower), nameof(CharacterActionUsePower.CheckInterruptionAfter))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CheckInterruptionAfter_Patch
    {
        [UsedImplicitly]
        public static bool Prefix([NotNull] CharacterActionUsePower __instance)
        {
            //PATCH: ignores interruptions processing for certain powers so they won't interrupt invisibility
            return !Global.PowersThatIgnoreInterruptions.Contains(__instance.activePower.PowerDefinition);
        }
    }

    [HarmonyPatch(typeof(CharacterActionUsePower), nameof(CharacterActionUsePower.GetAdvancementData))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class GetAdvancementData_Patch
    {
        [UsedImplicitly]
        public static bool Prefix([NotNull] CharacterActionUsePower __instance)
        {
            //PATCH: Calculate advancement data for `RulesetEffectPowerWithAdvancement`
            return RulesetEffectPowerWithAdvancement.GetAdvancementData(__instance);
        }
    }

    [HarmonyPatch(typeof(CharacterActionUsePower), nameof(CharacterActionUsePower.HandleEffectUniqueness))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class HandleEffectUniqueness_Patch
    {
        [UsedImplicitly]
        public static void Postfix([NotNull] CharacterActionUsePower __instance)
        {
            //PATCH: Support for limited power effect instances
            //terminates earliest power effect instances of same limit, if limit reached
            //used to limit Inventor's infusions
            GlobalUniqueEffects.EnforceLimitedInstancePower(__instance);
        }
    }

    [HarmonyPatch(typeof(CharacterActionUsePower), nameof(CharacterActionUsePower.SpendMagicEffectUses))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SpendMagicEffectUses_Patch
    {
        [UsedImplicitly]
        public static bool Prefix([NotNull] CharacterActionUsePower __instance)
        {
            //PATCH: Calculate extra charge usage for `RulesetEffectPowerWithAdvancement`

            if (__instance.activePower.OriginItem == null
                || __instance.activePower is not RulesetEffectPowerWithAdvancement power)
            {
                return true;
            }

            var usableDevice = power.OriginItem;

            foreach (var usableFunction in usableDevice.UsableFunctions)
            {
                var functionDescription = usableFunction.DeviceFunctionDescription;

                if (functionDescription.Type != DeviceFunctionDescription.FunctionType.Power
                    || functionDescription.FeatureDefinitionPower != power.PowerDefinition)
                {
                    continue;
                }

                __instance.ActingCharacter.RulesetCharacter
                    .UseDeviceFunction(usableDevice, usableFunction, power.ExtraCharges);
                break;
            }

            ServiceRepository.GetService<IGameLocationActionService>()
                .ItemUsed?.Invoke(usableDevice.ItemDefinition.Name);

            return false;
        }
    }
}
