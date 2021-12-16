using System.Collections.Generic;
using System.Linq;

namespace SolastaCommunityExpansion.Models
{
    internal static class PlayerControllerContext
    {
        private static readonly List<GameLocationCharacter> playerCharacters = new List<GameLocationCharacter>();

        private static Dictionary<GameLocationCharacter, PlayerController.ControllerType> ControllersChoices = new Dictionary<GameLocationCharacter, PlayerController.ControllerType>();

        internal static readonly string[] Controllers = new string[] { "Human", "AI" };

        internal static bool IsOffGame => Gui.Game == null;

        internal static bool IsMultiplayer => ServiceRepository.GetService<INetworkingService>().IsMultiplayerGame;

        internal static List<GameLocationCharacter> PlayerCharacters
        {
            get
            {
                var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();

                playerCharacters.Clear();

                if (gameLocationCharacterService != null)
                {
                    playerCharacters.AddRange(gameLocationCharacterService.PartyCharacters);
                    playerCharacters.AddRange(gameLocationCharacterService.GuestCharacters);
                }

                return playerCharacters;
            }
        }

        internal static bool SideFlipped { get; private set; }

        internal static int[] GetUiChoices()
        {
            ControllersChoices =
                PlayerCharacters.ToDictionary(pc => pc, pc => ControllersChoices.TryGetValue(pc, out var choice)
                        ? choice
                        : PlayerController.ControllerType.Human);

            return ControllersChoices.Values.Select(x => x == PlayerController.ControllerType.Human ? 0 : 1).ToArray();
        }

        internal static void SetUiChoices(int[] choices)
        {
            if (choices.Length != PlayerCharacters.Count)
            {
                Main.Warning("Encounters UI: choices and players are out of sync...");
                return;
            }

            for (var i = 0; i < PlayerCharacters.Count; i++)
            {
                var partyCharacter = PlayerCharacters[i];
                var controllerType = choices[i] == 0 ? PlayerController.ControllerType.Human : PlayerController.ControllerType.AI;

                ControllersChoices.AddOrReplace(partyCharacter, controllerType);
            }
        }

        private static void UpdatePartyControllerIds(bool reset = false)
        {
            var activePlayerController = Gui.ActivePlayerController;

            PlayerCharacters.ForEach(x => x.ControllerId = reset || ControllersChoices[x] == PlayerController.ControllerType.Human ? 0 : 1);
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
            UpdatePartyControllerIds(reset: true);

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
