using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomGenericBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;

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
            var isPowerFunction = __instance.ActionParams.RulesetEffect.Name.Contains("PowerFunction");

            if (isPowerFunction && Main.Settings.KeepInvisibilityWhenUsingItems)
            {
                return false;
            }

            return !__instance.activePower.PowerDefinition.HasSubFeatureOfType<IIgnoreInterruptionCheck>();
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
            var isPowerFunction = __instance.ActionParams.RulesetEffect.Name.Contains("PowerFunction");

            if (isPowerFunction && Main.Settings.KeepInvisibilityWhenUsingItems)
            {
                return false;
            }

            return !__instance.activePower.PowerDefinition.HasSubFeatureOfType<IIgnoreInterruptionCheck>();
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
        public static bool Prefix([NotNull] CharacterActionUsePower __instance)
        {
            //PATCH: terminates all matching spells and powers of same group
            ForceGlobalUniqueEffects.TerminateMatchingUniqueEffect(
                __instance.ActingCharacter.RulesetCharacter,
                __instance.activePower);

            //PATCH: Support for limited power effect instances
            //terminates earliest power effect instances of same limit, if limit reached
            //used to limit Inventor's infusions
            ForceGlobalUniqueEffects.EnforceLimitedInstancePower(__instance);

            return false;
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
            //PATCH: we get an empty originItem under MP (GRENADIER) (MULTIPLAYER)
            if (__instance.activePower.OriginItem == null)
            {
                var provider = __instance.activePower.PowerDefinition.GetFirstSubFeatureOfType<PowerPoolDevice>();

                if (provider != null)
                {
                    __instance.activePower.originItem = provider.GetDevice(__instance.ActingCharacter.RulesetCharacter);
                }
            }

            //PATCH: Calculate extra charge usage for `RulesetEffectPowerWithAdvancement`
            if (__instance.activePower.OriginItem == null
                || __instance.activePower is not RulesetEffectPowerWithAdvancement power)
            {
                return true;
            }

            var usableDevice = power.OriginItem;

            foreach (var usableFunction in usableDevice.UsableFunctions
                         .Select(usableFunction => new
                         {
                             usableFunction, functionDescription = usableFunction.DeviceFunctionDescription
                         })
                         .Where(t =>
                             t.functionDescription.Type == DeviceFunctionDescription.FunctionType.Power &&
                             t.functionDescription.FeatureDefinitionPower == power.PowerDefinition)
                         .Select(t => t.usableFunction))
            {
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
