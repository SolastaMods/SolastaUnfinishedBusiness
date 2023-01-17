using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class NetworkingManagerPatcher
{
    [HarmonyPatch(typeof(NetworkingManager), nameof(NetworkingManager.CreateRoom))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CreateOfflineRoomIfNeeded_Patch
    {
        [UsedImplicitly]
        public static void Prefix(ref NetworkingDefinitions.RoomInfo roomInfo)
        {
            //PATCH: allows up to 6 players to join the game if there are enough heroes available (PARTYSIZE)
            roomInfo.maxPlayers = Main.Settings.OverridePartySize;
        }
    }
}
