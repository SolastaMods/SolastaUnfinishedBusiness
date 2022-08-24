using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUi.Gamepad;

[HarmonyPatch(typeof(Gui), "GamepadActive", MethodType.Getter)]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class Gui_GamepadActive_Getter
{
    internal static bool Prefix(ref bool __result)
    {
        __result = Main.Settings.EnableGamepad;

        return false;
    }
}
