//using System.Collections.Generic;
//using HarmonyLib;

//namespace SolastaDungeonMakerPro.Models
//{
//    internal static class EnemyControllerContext
//    {
//        private static List<GameLocationCharacter> friends;
//        private static List<GameLocationCharacter> enemies;        
//        private static bool flip = false;

//        private static bool ActiveContenderIsFriend()
//        {
//            var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();
//            var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

//            if (gameLocationCharacterService != null && gameLocationBattleService != null && gameLocationBattleService.IsBattleInProgress)
//            {
//                if (!flip)
//                {
//                    friends = gameLocationCharacterService.ValidCharacters.FindAll(c => c.RulesetCharacter.Side == RuleDefinitions.Side.Ally);
//                    enemies = gameLocationCharacterService.ValidCharacters.FindAll(c => c.RulesetCharacter.Side == RuleDefinitions.Side.Enemy);
//                    flip = false;
//                }
//                return friends.Exists(x => x == gameLocationBattleService.Battle.ActiveContender);
//            }
//            return false;
//        }

//        private static void UpdatePlayerController(List<GameLocationCharacter> controllerCharacters)
//        {
//            var playerControllerService = ServiceRepository.GetService<IPlayerControllerService>();
//            var activePlayerController = playerControllerService.ActivePlayerController;

//            AccessTools.Field(activePlayerController.GetType(), "side").SetValue(activePlayerController, controllerCharacters[0].Side);
//            activePlayerController.ControlledCharacters.Clear();
//            activePlayerController.ControlledCharacters.AddRange(controllerCharacters);
//        }

//        internal static void Start()
//        {
//            var networkingService = ServiceRepository.GetService<INetworkingService>();

//            if (Main.Settings.GameMasterMode && !ActiveContenderIsFriend() && networkingService?.IsMultiplayerGame != true)
//            {
//                flip = true;
//                UpdatePlayerController(enemies);
//            }
//        }

//        internal static void Stop()
//        {
//            var networkingService = ServiceRepository.GetService<INetworkingService>();

//            if (flip && networkingService?.IsMultiplayerGame != true)
//            {
//                flip = false;
//                UpdatePlayerController(friends);
//            }
//        }
//    }
//}