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
            if (!Main.Settings.DontFollowCharacterInBattle)
            {
                return true;
            }

            // Ensure all Unity objects are valid
            if (!__instance
                || character == null
                || !__instance.CurrentCameraMode
                || __instance.CurrentCameraMode.CameraService == null
                || !__instance.CurrentCameraMode.CameraService.MainCamera)
            {
                return true;
            }

            // Get character's location in screen co-ords
            var characterLocation = character.LocationPosition;

            var screenPoint = __instance
                .CurrentCameraMode.CameraService.MainCamera
                .WorldToScreenPoint(new Vector3(characterLocation.x, characterLocation.y, characterLocation.z));

            var width = Screen.width;
            var height = Screen.height;
            var margin = Main.Settings.DontFollowMargin / 100f;

            var followCharacter = ShouldFollowCharacter();

            Main.Log($"CameraControllerLocation_FollowCharacterForBattle {(followCharacter ? "(follow)" : "don't follow")}: {character.Name}");

            // return true to allow follow character code to run/false to disable
            return followCharacter;

            bool ShouldFollowCharacter()
            {
                return screenPoint.x < width * margin
                    || screenPoint.x > width * (1 - margin)
                    || screenPoint.y < height * margin
                    || screenPoint.y > height * (1 - margin);
            }
        }
    }
}
