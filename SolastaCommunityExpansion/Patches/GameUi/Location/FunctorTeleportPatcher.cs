using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;

namespace SolastaCommunityExpansion.Patches.GameUi.Location
{
    /// <summary>
    /// Currently when a character is teleported off screen the camera doesn't follow.
    /// This patch will attempt to follow the character if initiated by a teleport game gadget
    /// The don't follow character in battle patch will interact with this patch to cancel
    /// following if we're in battle and the character is on screen.
    /// </summary>
    [HarmonyPatch(typeof(FunctorTeleport), "Execute")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FunctorTeleport_Execute
    {
        internal static void FollowCharacterOnTeleport(GameLocationCharacter character)
        {
            if (!Main.Settings.FollowCharactersOnTeleport || Gui.GameLocation.UserLocation == null)
            {
                return;
            }

            var camera = ServiceRepository.GetService<ICameraService>().CurrentCameraController as CameraControllerLocation;

            if (camera != null)
            {
                if (Gui.Battle == null)
                {
                    camera.FollowCharacterForExploration(character);
                }
                else
                {
                    camera.FollowCharacterForBattle(character);
                }
            }

            Main.Log($"Teleported: {character.Name}");
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var teleportCharacterMethod = typeof(GameLocationPositioningManager).GetMethod("TeleportCharacter");

#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
            var followCharacterOnTeleportMethod = typeof(FunctorTeleport).GetMethod("FollowCharacterOnTeleport", BindingFlags.NonPublic | BindingFlags.Static);
            var characterField = typeof(FunctorTeleport).GetField("'<index>5__4'", BindingFlags.NonPublic | BindingFlags.Instance);
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields

            foreach (var instruction in instructions)
            {
                yield return instruction;

                if (instruction.Calls(teleportCharacterMethod))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, characterField);
                    yield return new CodeInstruction(OpCodes.Call, followCharacterOnTeleportMethod);
                }
            }
        }
    }
}
