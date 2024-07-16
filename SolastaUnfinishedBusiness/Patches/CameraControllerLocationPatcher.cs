using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using TA;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CameraControllerLocationPatcher
{
    /// <summary>
    ///     Allows the user to prevent the battle camera always following the current character if that character is
    ///     in view (on the monitor).
    ///     The battle camera will still move if the character is off screen or within x% (definable) of the screen edge.
    /// </summary>
    private static bool InterruptCamera(
        CameraControllerLocation __instance, GameLocationCharacter character, Vector3 position)
    {
        //PATCH: camera don't follow character in battle
        if (!Main.Settings.DontFollowCharacterInBattle)
        {
            return true;
        }

        if (Main.Settings.DontFollowMargin == 0)
        {
            return false;
        }

        // Ensure all Unity objects are valid
        if (!__instance ||
            (character == null && position == Vector3.zero) ||
            !__instance.CurrentCameraMode ||
            __instance.CurrentCameraMode.CameraService == null ||
            !__instance.CurrentCameraMode.CameraService.MainCamera)
        {
            return true;
        }

        // Get character's location in screen co-ords
        var width = Screen.width;
        var height = Screen.height;
        var margin = Main.Settings.DontFollowMargin / 100f;

        Vector3 finalPosition;

        if (character != null)
        {
            var characterLocation = character.LocationPosition;
                
            finalPosition = new Vector3(characterLocation.x, characterLocation.y, characterLocation.z);
        }
        else
        {
            finalPosition = position;
        }

        var screenPoint = __instance
            .CurrentCameraMode.CameraService.MainCamera
            .WorldToScreenPoint(finalPosition);

        var followCharacter = screenPoint.x < width * margin
                              || screenPoint.x > width * (1 - margin)
                              || screenPoint.y < height * margin
                              || screenPoint.y > height * (1 - margin);

        // return true to allow follow character code to run/false to disable
        return followCharacter;
    }

    [HarmonyPatch(typeof(CameraControllerLocation), nameof(CameraControllerLocation.FollowCharacterForBattle))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class FollowCharacterForBattle_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(CameraControllerLocation __instance, GameLocationCharacter character)
        {
            return InterruptCamera(__instance, character, Vector3.zero);
        }
    }

    [HarmonyPatch(typeof(CameraControllerLocation), nameof(CameraControllerLocation.FocusCharacterOnMap))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class FocusCharacterOnMap_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(CameraControllerLocation __instance, GameLocationCharacter character)
        {
            return InterruptCamera(__instance, character, Vector3.zero);
        }
    }
    
    [HarmonyPatch(typeof(CameraControllerLocation), nameof(CameraControllerLocation.FocusPositionForBattle))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class FocusPositionForBattle_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(CameraControllerLocation __instance, Vector3 position)
        {
            return InterruptCamera(__instance, null, position);
        }
    }

    [HarmonyPatch(typeof(CameraControllerLocation), nameof(CameraControllerLocation.FocusCharacterToManualBattle))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class FocusCharacterToManualBattle_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(CameraControllerLocation __instance, GameLocationCharacter character)
        {
            return InterruptCamera(__instance, character, Vector3.zero);
        }
    }
}
