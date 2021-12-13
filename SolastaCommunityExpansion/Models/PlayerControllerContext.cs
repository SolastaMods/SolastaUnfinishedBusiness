using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace SolastaCommunityExpansion.Models
{
    internal static class PlayerControllerContext
    {
        private static readonly List<GameLocationCharacter> friends = new List<GameLocationCharacter>();

        private static readonly List<GameLocationCharacter> enemies = new List<GameLocationCharacter>();

        private static bool PlayerInControlOfEnemy { get; set; }

        internal static readonly string[] Controllers = new string[] { "Human", "Computer" };

        internal static readonly int[] ControllersChoices = new int[Settings.MAX_PARTY_SIZE];

        internal static bool IsOffGame => Gui.Game == null;

        internal static bool IsMultiplayer => ServiceRepository.GetService<INetworkingService>().IsMultiplayerGame;

        internal static List<GameLocationCharacter> PartyCharacters => ServiceRepository.GetService<IGameLocationCharacterService>().PartyCharacters;

        private static List<GameLocationCharacter> ValidCharacters => ServiceRepository.GetService<IGameLocationCharacterService>().ValidCharacters;

        private static bool ActiveContenderIsFriend
        {
            get
            {
                var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

                if (gameLocationBattleService == null || !gameLocationBattleService.IsBattleInProgress)
                {
                    return false;
                }

                if (!PlayerInControlOfEnemy)
                {
                    friends.Clear();
                    enemies.Clear();

                    foreach (var validCharacter in ValidCharacters)
                    {
                        if (validCharacter.Side == RuleDefinitions.Side.Ally)
                        {
                            friends.Add(validCharacter);
                        }
                        else
                        {
                            enemies.Add(validCharacter);
                        }
                    }
                }

                return friends.Exists(x => x == gameLocationBattleService.Battle.ActiveContender);
            }
        }

        private static void UpdatePlayerControllerControlledCharacters()
        {
            var activePlayerController = Gui.ActivePlayerController;
            var side = PlayerInControlOfEnemy ? RuleDefinitions.Side.Enemy : RuleDefinitions.Side.Ally;
            var controlledCharacters = PlayerInControlOfEnemy ? enemies : friends;

            AccessTools.Field(activePlayerController.GetType(), "side").SetValue(activePlayerController, side);
            activePlayerController.ControlledCharacters.Clear();
            activePlayerController.ControlledCharacters.AddRange(controlledCharacters);
        }

        private static void UpdateControlledCharactersControllerId(int[] controllers)
        {
            var activePlayerController = Gui.ActivePlayerController;

            for (var i = 0; i < PartyCharacters.Count; i++)
            {
                var controllerId = controllers[i] == 0 ? Settings.PLAYER_CONTROLLER_ID : Settings.DM_CONTROLLER_ID;

                PartyCharacters[i].ControllerId = controllerId;
            }

            activePlayerController.DirtyControlledCharacters();
        }

        private static void StartEnemyControlledByPlayer()
        {
            if (!IsMultiplayer && Main.Settings.EnableEnemiesControlledByPlayer && !ActiveContenderIsFriend)
            {
                PlayerInControlOfEnemy = true;
                UpdatePlayerControllerControlledCharacters();
            }
        }

        private static void StopEnemyControlledByPlayer()
        {
            // no check for EnableEnemiesControlledByPlayer on purpose here otherwise users might get in trouble while changing this setting during a battle
            if (!IsMultiplayer && PlayerInControlOfEnemy)
            {
                PlayerInControlOfEnemy = false;
                UpdatePlayerControllerControlledCharacters();
            }
        }

        private static void StartHeroControlledByComputer()
        {
            if (!IsMultiplayer && Main.Settings.EnableHeroesControlledByComputer)
            {
                UpdateControlledCharactersControllerId(ControllersChoices);
            }
        }

        private static void StopHeroControlledByComputer()
        {
            // no check for EnableHeroesControlledByComputer on purpose here otherwise users might get in trouble while changing this setting during a battle
            if (!IsMultiplayer) 
            {
                UpdateControlledCharactersControllerId(Enumerable.Repeat(0, Settings.MAX_PARTY_SIZE).ToArray());
            }
        }

        internal static void Start()
        {
            StartEnemyControlledByPlayer();
            StartHeroControlledByComputer();
        }

        internal static void Stop()
        {
            StopEnemyControlledByPlayer();
            StopHeroControlledByComputer();
        }
    }
}
