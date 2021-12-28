using System.Collections.Generic;
using HarmonyLib;
using TA;
using UnityEngine;

namespace SolastaCommunityExpansion.Models
{
    internal static class TeleporterContext
    {
        internal const InputCommands.Id CTRL_SHIFT_T = (InputCommands.Id)44440006;

        internal static void Load()
        {
            ServiceRepository.GetService<IInputService>().RegisterCommand(CTRL_SHIFT_T, (int)KeyCode.T, 304, 306, -1, -1, -1);
        }

        internal static void ConfirmTeleportParty(InputCommands.Id command)
        {
            if (Main.Settings.EnableTeleportParty && command == CTRL_SHIFT_T)
            {
                var position = GetEncounterPosition();

                Gui.GuiService.ShowMessage(
                    MessageModal.Severity.Attention2,
                    "Message/&TeleportPartyTitle",
                    Gui.Format("Message/&TeleportPartyDescription", position.x.ToString(), position.x.ToString()),
                    "Message/&MessageYesTitle", "Message/&MessageNoTitle",
                    new MessageModal.MessageValidatedHandler(() => { TeleportParty(position); }),
                    null);
            }
        }

        private static int3 GetEncounterPosition()
        {
            var gameLocationService = ServiceRepository.GetService<IGameLocationService>();

            int x = (int)gameLocationService.GameLocation.LastCameraPosition.x;
            int z = (int)gameLocationService.GameLocation.LastCameraPosition.z;

            return new int3(x, 0, z);
        }

        private static void TeleportParty(int3 position)
        {
            var gameLocationActionService = ServiceRepository.GetService<IGameLocationActionService>();
            var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var gameLocationPositioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
            var formationPositions = new List<int3>();
            var partyAndGuests = new List<GameLocationCharacter>();
            var positions = new List<int3>();

            for (var iy = 0; iy < 4; iy++)         
            {
                for (var ix = 0; ix < 2; ix++)
                {
                    formationPositions.Add(new int3(ix, 0, iy));
                }
            }

            partyAndGuests.AddRange(gameLocationCharacterService.PartyCharacters);
            partyAndGuests.AddRange(gameLocationCharacterService.GuestCharacters);

            gameLocationPositioningService.ComputeFormationPlacementPositions(partyAndGuests, position, LocationDefinitions.Orientation.North, formationPositions, CellHelpers.PlacementMode.Station, positions, new List<RulesetActor.SizeParameters>(), 25);

            for (var index = 0; index < positions.Count; index++)
            {
                partyAndGuests[index].LocationPosition = positions[index];

                // rotates the characters in position to force the game to redrawn them
                gameLocationActionService.MoveCharacter(partyAndGuests[index], positions[(index + 1) % positions.Count], LocationDefinitions.Orientation.North, 0, ActionDefinitions.MoveStance.Walk);
            }              
        }
    }
}
