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
            // don't mess up with camera off battle
            if (!Main.Settings.EnableElevationCameraToStayAtPosition || Gui.Battle == null)
            {
                return;
            }

            __instance.parameters.hasElevationCorrection = false;
            __instance.parameters.elevationType = CameraModeManualParameters.CameraElevationType.Free;
        }
    }

    [HarmonyPatch(typeof(CameraModeManual), nameof(CameraModeManual.SetBounds))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class TargetBounds_Getter_Patch
    {
        [UsedImplicitly]
        public static void Prefix(ref Bounds bounds, CameraController.CameraBoundsSource source)
        {
            // don't mess up with camera off battle
            if (!Main.Settings.EnableElevationCameraToStayAtPosition ||
                source != CameraController.CameraBoundsSource.Battle)
            {
                return;
            }

            bounds = new Bounds(
                bounds.center,
                new Vector3(
                    bounds.size.x,
                    bounds.size.y * 3,
                    bounds.size.z));
        }
    }
}
