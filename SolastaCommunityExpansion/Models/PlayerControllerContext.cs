using System.Collections.Generic;
using System.Linq;

namespace SolastaCommunityExpansion.Models
{
    internal static class PlayerControllerContext
    {
        private static readonly Dictionary<GameLocationCharacter, PlayerController.ControllerType> ControllersChoices = new Dictionary<GameLocationCharacter, PlayerController.ControllerType>();

        internal static readonly string[] Controllers = new string[] { "Human", "AI" };

        internal static bool IsOffGame => Gui.Game == null;

        internal static bool IsMultiplayer => ServiceRepository.GetService<INetworkingService>().IsMultiplayerGame;

        internal static List<GameLocationCharacter> PlayerCharacters
        {
            get
            {
                var result = new List<GameLocationCharacter>();
                var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();

                if (gameLocationCharacterService != null)
                {
                    result.AddRange(gameLocationCharacterService.PartyCharacters);
                    result.AddRange(gameLocationCharacterService.GuestCharacters);
                }

                return result;
            }
        }

        internal static bool SideFlipped { get; private set; }

        internal static int[] UiChoices 
        {
            get
            {
                var ControllersCharacters = ControllersChoices.Keys;

                foreach (var controllersCharacter in ControllersCharacters)
                {
                    if (!PlayerCharacters.Contains(controllersCharacter))
                    {
                        ControllersChoices.Remove(controllersCharacter);
                    }
                }

                PlayerCharacters.ForEach(x => ControllersChoices.TryAdd(x, PlayerController.ControllerType.Human));

                return ControllersChoices.Values.Select(x => x == PlayerController.ControllerType.Human ? 0 : 1).ToArray();
            } 
            set
            {
                for (var i = 0; i < PlayerCharacters.Count; i++)
                {
                    var partyCharacter = PlayerCharacters[i];
                    var controllerType = value[i] == 0 ? PlayerController.ControllerType.Human : PlayerController.ControllerType.AI;

                    ControllersChoices.AddOrReplace<GameLocationCharacter, PlayerController.ControllerType>(partyCharacter, controllerType);
                }
            }
        }

        private static void UpdatePartyControllerIds(bool reset = false)
        {
            var activePlayerController = Gui.ActivePlayerController;

            for (var i = 0; i < PlayerCharacters.Count; i++)
            {
                var controllerId = reset || UiChoices[i] == 0 ? Settings.PLAYER_CONTROLLER_ID : PlayerControllerManager.DmControllerId;

                PlayerCharacters[i].ControllerId = controllerId;
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
