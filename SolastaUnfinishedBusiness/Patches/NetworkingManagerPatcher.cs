using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class NetworkingManagerPatcher
{
    [HarmonyPatch(typeof(NetworkingManager), "CreateRoom")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class CreateOfflineRoomIfNeeded_Patch
    {
        public static void Prefix(ref NetworkingDefinitions.RoomInfo roomInfo)
        {
            //PATCH: allows up to 6 players to join the game if there are enough heroes available (PARTYSIZE)
            roomInfo.maxPlayers = Main.Settings.OverridePartySize;
        }
    }
}
