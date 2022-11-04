using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches;

public static class FunctorTeleportPatcher
{
    /// <summary>
    ///     Currently when a character is teleported off screen the camera doesn't follow.
    ///     This patch will attempt to follow the character if initiated by a teleport game gadget
    ///     The don't follow character in battle patch will interact with this patch to cancel
    ///     following if we're in battle and the character is on screen.
    /// </summary>
    [HarmonyPatch(typeof(FunctorTeleport), "Execute")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Execute_Patch
    {
        //PATCH: follow characters on teleport
        private static void FollowCharacterOnTeleport(GameLocationCharacter character)
        {
            if (!Main.Settings.FollowCharactersOnTeleport || Gui.GameLocation.UserLocation == null)
            {
                return;
            }

            var camera =
                ServiceRepository.GetService<ICameraService>().CurrentCameraController as CameraControllerLocation;

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

        //TODO: REVIEW LOGGING
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var teleportCharacterMethod = typeof(GameLocationPositioningManager).GetMethod("TeleportCharacter");
            var followCharacterOnTeleportMethod = new Action<GameLocationCharacter>(FollowCharacterOnTeleport).Method;
            var characterField =
                typeof(FunctorTeleport).GetField("'<index>5__4'", BindingFlags.NonPublic | BindingFlags.Instance);

            var code = instructions.ToList();
            var bindIndex = code.FindIndex(x => x.Calls(teleportCharacterMethod));

            if (bindIndex >= 0)
            {
                code.InsertRange(bindIndex + 1,
                    new CodeInstruction[]
                    {
                        new(OpCodes.Call, teleportCharacterMethod), new(OpCodes.Ldarg_0),
                        new(OpCodes.Ldfld, characterField), new(OpCodes.Call, followCharacterOnTeleportMethod)
                    });
            }

            return code;
        }
    }
}
