using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.Camera
{
    [HarmonyPatch(typeof(CameraControllerLocation), "FollowCharacterForBattle")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CameraControllerLocation_FollowCharacterForBattle
    {
        /// <summary>
        /// Allows the user to prevent the battle camera always following the current character if that character is 
        /// in view (on the monitor).
        /// The battle camera will still move if the character is off screen or within x% (definable) of the screen edge.
        /// </summary>
        internal static bool Prefix(CameraControllerLocation __instance, GameLocationCharacter character)
        {
            //
            // TODO @IMPPHIL: FIX EXIT ROUTE
            //
            try
            {
                if (Main.Settings.DontFollowCharacterInBattle)
                {
                    if (!__instance 
                        || character == null 
                        || !__instance.CurrentCameraMode 
                        || __instance.CurrentCameraMode.CameraService == null
                        || !__instance.CurrentCameraMode.CameraService.MainCamera)
                    {
                        return true;
                    }

                    var lp = character.LocationPosition;
                    var sp = __instance.CurrentCameraMode.CameraService.MainCamera.WorldToScreenPoint(new Vector3(lp.x, lp.y, lp.z));

                    Main.Log($"{character.Name}: ({sp.x},{sp.y},{sp.z})");

                    var w = Screen.width;
                    var h = Screen.height;

                    var margin = Main.Settings.DontFollowMargin / 100f;

                    if (sp.x < w * margin || sp.x > w * (1 - margin) || sp.y < h * margin || sp.y > h * (1 - margin))
                    {
                        Main.Log($"CameraControllerLocation_FollowCharacterForBattle (off screen): {character.Name}");
                        return true;
                    }

                    Main.Log($"CameraControllerLocation_FollowCharacterForBattle: {character.Name} - ignored.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Main.Error(ex);
            }

            return true;
        }
    }
}
