using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class NetworkingManagerPatcher
{
    [HarmonyPatch(typeof(NetworkingManager), "CreateOfflineRoomIfNeeded")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class CreateOfflineRoomIfNeeded_Patch
    {
        private static bool CreateRoom(
            string name,
            RoomOptions roomOptions,
            TypedLobby typedLobby,
            string[] expectedUsers)
        {
            //PATCH: allows up to 6 players to join the game if there are enough heroes available (PARTYSIZE)
            var maxValue = Math.Max(ToolsContext.GamePartySize, Main.Settings.OverridePartySize);

            roomOptions.MaxPlayers = (byte)maxValue;

            return PhotonNetwork.CreateRoom(name, roomOptions, typedLobby, expectedUsers);
        }

        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var createRoomMethod =
                typeof(PhotonNetwork).GetMethod("CreateRoom", BindingFlags.Public | BindingFlags.Static);
            var myCreateRoomMethod = new Func<string, RoomOptions, TypedLobby, string[], bool>(CreateRoom).Method;

            foreach (var instruction in instructions)
            {
                if (instruction.Calls(createRoomMethod))
                {
                    yield return new CodeInstruction(OpCodes.Call, myCreateRoomMethod);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}
