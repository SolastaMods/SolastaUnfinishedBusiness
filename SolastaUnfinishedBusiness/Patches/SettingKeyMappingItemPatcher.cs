using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class SettingKeyMappingItemPatcher
{
    //PATCH: extend mod keybinding settings into vanilla settings screen
    [HarmonyPatch(typeof(SettingKeyMappingItem), nameof(SettingKeyMappingItem.OnValidateKeyCode))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class DoApplyKeyCode_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(SettingKeyMappingItem __instance, bool primaryBinding, KeyCode keyCode)
        {
            OnValidateKeyCode(__instance, primaryBinding, keyCode);

            return false;
        }

        private static void OnValidateKeyCode(SettingKeyMappingItem __instance, bool primaryBinding, KeyCode keyCode)
        {
            var inputService = ServiceRepository.GetService<IInputService>();
            var commandIdFromName = inputService.GetCommandIdFromName(__instance.Setting.PropertyName);

            __instance.selectedKeyCode = (int)keyCode;
            __instance.primaryBinding = primaryBinding;

            if (inputService.IsKeyCodeAlreadyMapped(
                    keyCode, commandIdFromName, out __instance.boundCommand, out __instance.boundPrimary))
            {
                //PATCH: ensure we get a proper name here instead of a number on mod commands
                var name = InputContext.GetCommandName(__instance.boundCommand);
                var str = Gui.Localize("Setting/&" + name + "Title");

                Gui.GuiService.ShowMessage(
                    MessageModal.Severity.Attention2,
                    "Message/&MessageKeyAlreadyBoundTitle",
                    Gui.FormatWithHighlight("Message/&MessageKeyAlreadyBoundDescription", keyCode.ToString(), str),
                    "Message/&MessageOverrideTitle",
                    "Message/&MessageCancelTitle",
                    __instance.DoOverride,
                    __instance.OnCancelKeyCode);
            }
            else
            {
                __instance.DoApplyKeyCode();
            }
        }
    }

    //PATCH: extend mod keybinding settings into vanilla settings screen
    [HarmonyPatch(typeof(SettingKeyMappingItem), nameof(SettingKeyMappingItem.DoOverride))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class DoOverride_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(SettingKeyMappingItem __instance)
        {
            DoOverride(__instance);

            return false;
        }

        private static void DoOverride(SettingKeyMappingItem __instance)
        {
            var commandMapping =
                ServiceRepository.GetService<IInputService>().GetCommandMapping(__instance.boundCommand);

            if (__instance.boundPrimary)
            {
                commandMapping.PrimaryKeyCode = -1;
            }
            else
            {
                commandMapping.SecondaryKeyCode = -1;
            }

            //PATCH: ensure we get a proper name here instead of a number on mod commands
            SettingItem.ApplyValueToSetting?.Invoke(
                InputContext.GetCommandName(__instance.boundCommand), commandMapping.DumpToString());

            __instance.DoApplyKeyCode();
        }
    }
}
