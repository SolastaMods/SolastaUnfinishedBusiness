using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CameraModeManualPatcher
{
    //PATCH: supports camera settings in Mod UI
    [HarmonyPatch(typeof(CameraModeManual), nameof(CameraModeManual.Parameters), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Parameters_Getter_Patch
    {
        [UsedImplicitly]
        public static void Prefix(CameraModeManual __instance)
        {
            // don't mess up with camera while location is building
            if (Gui.GameLocation?.Ready == true)
            {
                __instance.parameters.hasElevationCorrection = !Main.Settings.EnableElevationCameraToStayAtPosition;
            }
        }
    }
}
