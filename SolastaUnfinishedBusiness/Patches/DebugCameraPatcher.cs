using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;
using TA;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class DebugCameraPatcher
{
    private static void UpdateCamera(DebugCamera __instance)
    {
        var cartesian = SphericalCoordinates.ToCartesian(__instance.radius, __instance.theta, __instance.phi);
        var position = cartesian + __instance.orbitPosition;
        var quaternion = Quaternion.LookRotation(__instance.orbitPosition - position);

        __instance.virtualCamera.transform.SetPositionAndRotation(position, quaternion);
    }

    [HarmonyPatch(typeof(DebugCamera), nameof(DebugCamera.UpdateFreeCamera))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class UpdateFreeCamera_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(DebugCamera __instance)
        {
            if (!GameUiContext.IsVttCameraEnabled)
            {
                return true;
            }

            // handle ROTATE in orbital mode
            if (Input.GetKeyDown(KeyCode.Mouse1) ||
                Input.GetKeyDown(KeyCode.Q) ||
                Input.GetKeyDown(KeyCode.E))
            {
                __instance.currentMode = DebugCamera.DebugCameraMode.Orbital;
                __instance.StorePreviousState();
            }

            // ZOOM IN / OUT with keyboard
            if (Input.GetKey(KeyCode.PageUp) && __instance.radius + __instance.orbitPosition.y > 15.0)
            {
                __instance.StorePreviousState();
                __instance.radius = Mathf.Max(0.1f, __instance.radius - __instance.radiusSpeed);
                UpdateCamera(__instance);
            }
            else if (Input.GetKey(KeyCode.PageDown) && __instance.radius + __instance.orbitPosition.y < 155.0)
            {
                __instance.StorePreviousState();
                __instance.radius = Mathf.Max(0.1f, __instance.radius + __instance.radiusSpeed);
                UpdateCamera(__instance);
            }
            // ZOOM IN / OUT with mouse wheel
            else
            {
                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                switch (Event.current.type)
                {
                    case EventType.ScrollWheel when
                        Event.current.delta.y > 0.0 &&
                        __instance.radius + __instance.orbitPosition.y > 15.0:

                        __instance.StorePreviousState();
                        __instance.radius -= __instance.radiusSpeed * 4f;

                        UpdateCamera(__instance);

                        break;
                    case EventType.ScrollWheel when
                        Event.current.delta.y < 0.0 &&
                        __instance.radius + __instance.orbitPosition.y < 155.0:

                        __instance.StorePreviousState();
                        __instance.radius += __instance.radiusSpeed * 4f;

                        UpdateCamera(__instance);

                        break;

                    default:
                    {
                        var y = Input.GetAxis("Vertical") * __instance.movementStrafeSpeed;
                        var x = Input.GetAxis("Horizontal") * __instance.movementStrafeSpeed;

                        if (Input.GetKey(KeyCode.Mouse2) && !Input.GetKey(KeyCode.Mouse1))
                        {
                            y = -Input.GetAxis("Mouse Y") * __instance.movementStrafeSpeed;
                            x = -Input.GetAxis("Mouse X") * __instance.movementStrafeSpeed;
                        }

                        if (__instance.virtualCamera.transform.position.y < 3)
                        {
                            y = 0;
                        }

                        var transform = __instance.virtualCamera.transform;

                        __instance.virtualCamera.transform.Translate(new Vector3(x, y, 0), (Space)1);
                        __instance.orbitPosition = transform.position +
                                                   (transform.forward * __instance.orbitalPlacementDistance * 2);

                        if (__instance.orbitPosition.y < 0)
                        {
                            __instance.orbitPosition.y = 0;
                        }

                        __instance.angleX = Mathf.Clamp(__instance.angleX, __instance.pitchMax, __instance.pitchMin);
                        __instance.virtualCamera.transform.rotation =
                            Quaternion.Euler(__instance.angleX, __instance.angleY, 0);

                        break;
                    }
                }
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(DebugCamera), nameof(DebugCamera.UpdateOrbitalCamera))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class UpdateOrbitalCamera_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(DebugCamera __instance)
        {
            if (!GameUiContext.IsVttCameraEnabled)
            {
                return true;
            }

            if (Input.GetKeyUp(KeyCode.Mouse1) ||
                Input.GetKeyUp(KeyCode.Q) ||
                Input.GetKeyUp(KeyCode.E))
            {
                __instance.currentMode = DebugCamera.DebugCameraMode.Free;
                __instance.StorePreviousState();
            }
            else
            {
                // __instance.theta = Mathf.Clamp(
                //     __instance.theta - (Input.GetAxis("Mouse Y") * __instance.inclinationSpeed), 0.14f,
                //     1.56079638f);

                __instance.theta = 1.56079638f;

                if (Input.GetKey(KeyCode.Q))
                {
                    __instance.phi += __instance.azimuthSpeed;
                }
                else if (Input.GetKey(KeyCode.E))
                {
                    __instance.phi -= __instance.azimuthSpeed;
                }
                else
                {
                    __instance.phi -= Input.GetAxis("Mouse X") * __instance.azimuthSpeed;
                }

                UpdateCamera(__instance);
            }

            return false;
        }
    }
}
