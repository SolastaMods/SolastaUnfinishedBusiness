using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

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
            if (Gui.GameLocation?.Initialized != true)
            {
                return;
            }

            __instance.parameters.hasElevationCorrection = !Main.Settings.EnableElevationCameraToStayAtPosition;
            __instance.parameters.elevationType = Main.Settings.SetElevationCameraMaxHeightBy == 0
                ? CameraModeManualParameters.CameraElevationType.Auto
                : CameraModeManualParameters.CameraElevationType.Free;
        }
    }

    //PATCH: supports camera settings in Mod UI
    [HarmonyPatch(typeof(CameraModeManual), nameof(CameraModeManual.SetBounds))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class TargetBounds_Getter_Patch
    {
        [UsedImplicitly]
        public static void Prefix(ref Bounds bounds, CameraController.CameraBoundsSource source)
        {
            // don't mess up with camera while location is building
            if (Gui.GameLocation?.Initialized != true)
            {
                return;
            }

            if (Main.Settings.SetElevationCameraMaxHeightBy != 0)
            {
                bounds = new Bounds(
                    bounds.center,
                    new Vector3(
                        bounds.size.x,
                        bounds.size.y + Main.Settings.SetElevationCameraMaxHeightBy,
                        bounds.size.z));
            }
        }
    }
}
