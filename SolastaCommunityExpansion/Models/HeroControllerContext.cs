using System.Collections.Generic;

namespace SolastaCommunityExpansion.Models
{
    internal static class HeroControllerContext
    {
        internal static readonly int[] CharacterPlayersChoices = new int[7];

        private static GameLocationCharacter ActiveContender { get; set; }

        private static int ControllerId { get; set; } = -1;

        internal static bool IsMaster()
        {
            var playerControllerService = ServiceRepository.GetService<IPlayerControllerService>();

            return Gui.ActivePlayerController.ControllerId == playerControllerService.GetMasterPlayerControllerId();
        }

        internal static bool IsLocalPlayer()
        {
            var networkingService = ServiceRepository.GetService<INetworkingService>();

            return networkingService.IsLocalPlayerController(Gui.ActivePlayerController);
        }

        internal static List<string> GetPlayersList()
        {
            var sessionService = ServiceRepository.GetService<ISessionService>();
            var session = sessionService?.Session;
            var playerSlotsList = session?.PlayerSlotsList;
            var players = new List<string>();

            if (playerSlotsList != null)
            {
                for (int i = 0; i < playerSlotsList.Count; i++)
                {
                    var name = playerSlotsList[i].PlayerNickname;

                    players.Add(name == string.Empty ? "Human" : name);
                }

                players.Add("Computer");
            }

            return players;
        }

        internal static List<GameLocationCharacter> GetCharactersList()
        {
            var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
            var partyCharacters = new List<GameLocationCharacter>();

            if (gameLocationCharacterService != null)
            {
                partyCharacters.AddRange(gameLocationCharacterService.PartyCharacters);
            }

            return partyCharacters;
        }

        internal static void Start()
        {
            if (Main.Settings.EnableControllersOverride)
            {
                var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();
                var playerControllerService = ServiceRepository.GetService<IPlayerControllerService>();
                var sessionService = ServiceRepository.GetService<ISessionService>();

                var battle = gameLocationBattleService?.Battle;
                var activePlayerController = playerControllerService?.ActivePlayerController;
                var session = sessionService?.Session;

                var partyCharacters = GetCharactersList();
                var playerSlotsList = session.PlayerSlotsList;

                var activeContender = battle?.ActiveContender;
                var activeContenderIndex = partyCharacters.IndexOf(activeContender);

                ActiveContender = activeContender;

                if (activeContenderIndex >= 0)
                {
                    var playerIndex = CharacterPlayersChoices[activeContenderIndex];
                    var controllerId = playerControllerService.ControllerIdsList[playerIndex];

                    if (playerIndex < playerSlotsList.Count)
                    {
                        ControllerId = partyCharacters[activeContenderIndex].ControllerId;

                        partyCharacters[activeContenderIndex].ControllerId = controllerId;
                        activePlayerController.DirtyControlledCharacters();
                    }
                    else if (playerIndex == playerSlotsList.Count)
                    {
                        partyCharacters[activeContenderIndex].ControllerId = Settings.DM_CONTROLLER_ID;
                        activePlayerController.DirtyControlledCharacters();
                    }
                }
            }
        }

        internal static void Stop()
        {
            if (Main.Settings.EnableControllersOverride && ControllerId > 0)
            {
                var playerControllerService = ServiceRepository.GetService<IPlayerControllerService>();
                var sessionService = ServiceRepository.GetService<ISessionService>();

                var activePlayerController = playerControllerService?.ActivePlayerController;
                var session = sessionService?.Session;

                var partyCharacters = GetCharactersList();
                var playerSlotsList = session.PlayerSlotsList;

                var activeContender = ActiveContender;
                var activeContenderIndex = partyCharacters.IndexOf(activeContender);

                if (activeContenderIndex >= 0)
                {
                    for (int i = 0; i < playerSlotsList.Count; i++)
                    {
                        if (playerSlotsList[i].ControlledCharacters.Exists(x => x.Contains(activeContender.Name)))
                        {
                            partyCharacters[activeContenderIndex].ControllerId = ControllerId;
                            activePlayerController.DirtyControlledCharacters();
                        }
                    }
                }

                ActiveContender = null;
                ControllerId = -1;
            }
        }
    }
}
