using System.Collections.Generic;
using System.Linq;

namespace SolastaCommunityExpansion.Models
{
    internal static class PlayerControllerContext
    {
        private const int PLAYER_CONTROLLER_ID = 1;

        private static readonly Dictionary<GameLocationCharacter, int> controllersChoices = new Dictionary<GameLocationCharacter, int>();

        private static int[] playerCharactersChoices { get; set; }

        internal static readonly string[] Controllers = new string[] { "Human", "AI" };

        internal static bool IsMultiplayer => ServiceRepository.GetService<INetworkingService>().IsMultiplayerGame;

        internal static bool IsOffGame => Gui.Game == null;

        internal static List<GameLocationCharacter> PlayerCharacters { get; private set; } = new List<GameLocationCharacter>();

        internal static int[] PlayerCharactersChoices
        {
            get => playerCharactersChoices;

            set
            {
                playerCharactersChoices = value;

                for (var i = 0; i < value.Length; i++)
                {
                    var playerCharacter = PlayerCharacters[i];

                    controllersChoices[playerCharacter] = value[i];
                }
            }
        }

        internal static bool SideFlipped { get; private set; }

        internal static void RefreshGuiState()
        {
            var controllersChoicesCopy = controllersChoices.ToDictionary(x => x.Key, x => x.Value);
            var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();

            controllersChoices.Clear();
            PlayerCharacters.Clear();

            if (gameLocationCharacterService != null)
            {
                PlayerCharacters.AddRange(gameLocationCharacterService.PartyCharacters);
                PlayerCharacters.AddRange(gameLocationCharacterService.GuestCharacters);
            }

            PlayerCharacters.ForEach(x => controllersChoices.Add(x, controllersChoicesCopy.TryGetValue(x, out var choice) ? choice : 0));
            playerCharactersChoices = controllersChoices.Values.ToArray();
        }

        private static void UpdatePartyControllerIds(bool reset = false)
        {
            var activePlayerController = Gui.ActivePlayerController;

            for (var i = 0; i < PlayerCharacters.Count; i++)
            {
                var playerCharacter = PlayerCharacters[i];
                var controllerId = reset || controllersChoices[playerCharacter] == 0 ? PLAYER_CONTROLLER_ID : PlayerControllerManager.DmControllerId;

                playerCharacter.ControllerId = controllerId;
            }

            activePlayerController.DirtyControlledCharacters();
        }

        internal static void Start(GameLocationBattle battle)
        {
            var activeContender = battle.ActiveContender;
            var enemies = battle.EnemyContenders;
            var players = battle.PlayerContenders;

            if (Main.Settings.EnableHeroesControlledByComputer && players.Contains(activeContender))
            {
                UpdatePartyControllerIds();
            }

            if (Main.Settings.EnableEnemiesControlledByPlayer && enemies.Contains(activeContender))
            {
                SideFlipped = true;
                enemies.ForEach(x => x.ChangeSide(RuleDefinitions.Side.Ally));
                players.ForEach(x => x.ChangeSide(RuleDefinitions.Side.Enemy));
                Gui.ActivePlayerController.ControlledCharacters.Clear();
                Gui.ActivePlayerController.ControlledCharacters.AddRange(enemies);
            }
        }

        internal static void Stop(GameLocationBattle battle)
        {
            UpdatePartyControllerIds(true);

            if (SideFlipped)
            {
                var enemies = battle.EnemyContenders;
                var players = battle.PlayerContenders;

                SideFlipped = false;
                enemies.ForEach(x => x.ChangeSide(RuleDefinitions.Side.Enemy));
                players.ForEach(x => x.ChangeSide(RuleDefinitions.Side.Ally));
                Gui.ActivePlayerController.ControlledCharacters.Clear();
                Gui.ActivePlayerController.ControlledCharacters.AddRange(players);
            }
        }
    }
}
