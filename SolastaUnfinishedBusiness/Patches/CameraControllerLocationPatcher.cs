using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CameraControllerLocationPatcher
{
    [HarmonyPatch(typeof(CameraControllerLocation), "FollowCharacterForBattle")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class FollowCharacterForBattle_Patch
    {
        /// <summary>
        ///     Allows the user to prevent the battle camera always following the current character if that character is
        ///     in view (on the monitor).
        ///     The battle camera will still move if the character is off screen or within x% (definable) of the screen edge.
        /// </summary>
        [UsedImplicitly]
        public static bool Prefix(CameraControllerLocation __instance, GameLocationCharacter character)
        {
            //PATCH: camera don't follow character in battle
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
            var width = Screen.width;
            var height = Screen.height;
            var margin = Main.Settings.DontFollowMargin / 100f;
            var characterLocation = character.LocationPosition;
            var screenPoint = __instance
                .CurrentCameraMode.CameraService.MainCamera
                .WorldToScreenPoint(new Vector3(characterLocation.x, characterLocation.y, characterLocation.z));

            var followCharacter = screenPoint.x < width * margin
                                  || screenPoint.x > width * (1 - margin)
                                  || screenPoint.y < height * margin
                                  || screenPoint.y > height * (1 - margin);

            // return true to allow follow character code to run/false to disable
            return followCharacter;
        }
    }
}
