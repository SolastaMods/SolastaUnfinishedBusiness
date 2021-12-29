using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;

namespace SolastaCommunityExpansion.Patches.PartySize
{
    // avoids a trace message when party greater than 4
    //
    // this shouldn't be protected
    //
    [HarmonyPatch(typeof(GameLocationPositioningManager), "CharacterMoved", new Type[] { typeof(GameLocationCharacter), typeof(TA.int3), typeof(TA.int3), typeof(RulesetActor.SizeParameters), typeof(RulesetActor.SizeParameters) })]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationPositioningManager_CharacterMoved
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var logErrorMethod = typeof(Trace).GetMethod("LogError", BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder, new Type[1] { typeof(string) }, null);
            var found = 0;

            foreach (var instruction in instructions)
            {
                if (instruction.Calls(logErrorMethod) && ++found == 1)
                {
                    yield return new CodeInstruction(OpCodes.Pop);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }

    [HarmonyPatch(typeof(GameLocationPositioningManager), "TeleportCharacter")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationPositioningManager_TeleportCharacter
    {
        /// <summary>
        /// Currently when a character is teleported off screen the camera doesn't follow.
        /// This patch will attempt to follow the character if initiated by a teleport game gadget
        /// by following but only if Teleport is called from GameGadet.Update().
        /// The don't follow character in battle patch will interact with this patch to cancel 
        /// following if we're in battle and the character is on screen.
        /// </summary>
        internal static void Postfix(GameLocationCharacter character)
        {
            if (!Main.Settings.FollowCharactersOnTeleport
                // Only follow in DM
                || Gui.GameLocation.UserLocation == null
                // Only follow if teleport initiated by teleport game gadget (not a perfect check but better than nothing)
                || !GameGadget_Update.InUpdate)
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
    }

    /// <summary>
    /// Required by GameLocationPositioningManager_TeleportCharacter to detect if being called from game gadget. 
    /// </summary>
    [HarmonyPatch(typeof(GameGadget), "Update")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameGadget_Update
    {
        internal static bool InUpdate { get; private set; }

        internal static void Prefix()
        {
            InUpdate = true;
        }

        internal static void Postfix()
        {
            InUpdate = false;
        }
    }
}
