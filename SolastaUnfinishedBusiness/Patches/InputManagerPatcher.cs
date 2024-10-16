using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class InputManagerPatcher
{
    //PATCH: handle mod keybinding settings with the proper mod instance
    [HarmonyPatch(typeof(InputManager), nameof(InputManager.RegisterCommand))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RegisterCommand_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            InputManager __instance,
            InputCommands.Id command,
            int primaryKeyCode,
            int primaryModifier1,
            int primaryModifier2,
            int secondaryKeyCode,
            int secondaryModifier1,
            int secondaryModifier2)
        {
            InputContext.RegisterCommand(
                __instance, command,
                primaryKeyCode, primaryModifier1, primaryModifier2,
                secondaryKeyCode, secondaryModifier1, secondaryModifier2);

            return false;
        }
    }

    [HarmonyPatch(typeof(InputManager), nameof(InputManager.ReadSettings))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ReadSettings_Patch
    {
        [UsedImplicitly]
        public static void Prefix()
        {
            InputContext.InputModManagerInstance.ReadSettings();
        }
    }

    [HarmonyPatch(typeof(InputManager), nameof(InputManager.RegisterDefaultCommands))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RegisterDefaultCommands_Patch
    {
        [UsedImplicitly]
        public static void Prefix(InputManager __instance)
        {
            SettingsContext.InputModManager.RegisterDefaultCommands(__instance);
        }
    }

    [HarmonyPatch(typeof(InputManager), nameof(InputManager.ResetDefaults))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ResetDefaults_Patch
    {
        [UsedImplicitly]
        public static void Prefix()
        {
            InputContext.InputModManagerInstance.ResetDefaults();
        }
    }
}
