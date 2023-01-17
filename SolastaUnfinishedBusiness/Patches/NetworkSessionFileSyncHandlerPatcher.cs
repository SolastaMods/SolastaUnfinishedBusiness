using System.Diagnostics.CodeAnalysis;
using System.IO;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class NetworkSessionFileSyncHandlerPatcher
{
    [HarmonyPatch(typeof(NetworkSessionFileSyncHandler), nameof(NetworkSessionFileSyncHandler.ForceCharacterPaths))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ForceCharacterPaths_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(Session session)
        {
            //PATCH: allows up to 6 players to join the game if there are enough heroes available (PARTYSIZE)
            var service = ServiceRepository.GetService<INetworkingService>();

            session.ClearAssignedCharacters();

            for (var optionalIndex = 0; optionalIndex < Main.Settings.OverridePartySize; ++optionalIndex)
            {
                NetworkingDefinitions.GetFileTypeInformation(NetworkingDefinitions.FileType.Character, optionalIndex,
                    out var characterFilename, out _);

                var finalFilename = Path.Combine(TacticalAdventuresApplication.MultiplayerFilesDirectory,
                    characterFilename);

                session.AddCharacterToPlayer(service.MasterClientNumber, finalFilename);
            }

            session.RefreshDefaultCharacterAssignments();

            return false;
        }
    }
}
