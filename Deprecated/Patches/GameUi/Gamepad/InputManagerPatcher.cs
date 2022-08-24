using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUi.Gamepad;

[HarmonyPatch(typeof(InputManager), "SwitchControlScheme")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class InputManager_SwitchControlScheme
{
    internal static void Prefix(ref string controlScheme)
    {
        if (!Main.Settings.EnableGamepad)
        {
            return;
        }

        controlScheme = InputDefinitions.ControlSchemeGamepad;
    }
}
