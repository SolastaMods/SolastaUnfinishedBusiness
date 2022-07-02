using System.Collections.Generic;
using System.Linq;

namespace SolastaCommunityExpansion.Models;

public static class PlayerControllerContext
{
    private const int PLAYER_CONTROLLER_ID = 1;

    private static readonly Dictionary<GameLocationCharacter, int> controllersChoices = new();

    internal static readonly string[] Controllers = {"Human", "AI"};

    private static int[] playerCharactersChoices { get; set; }

    internal static List<GameLocationCharacter> PlayerCharacters { get; } = new();

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

    private static bool SideFlipped { get; set; }

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

        PlayerCharacters.ForEach(x =>
            controllersChoices.Add(x, controllersChoicesCopy.TryGetValue(x, out var choice) ? choice : 0));
        playerCharactersChoices = controllersChoices.Values.ToArray();
    }

    private static void UpdatePartyControllerIds(bool reset = false)
    {
        var activePlayerController = Gui.ActivePlayerController;

        foreach (var playerCharacter in PlayerCharacters)
        {
            var controllerId = reset || controllersChoices[playerCharacter] == 0
                ? PLAYER_CONTROLLER_ID
                : PlayerControllerManager.DmControllerId;

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

        if (!Main.Settings.EnableEnemiesControlledByPlayer || !enemies.Contains(activeContender))
        {
            return;
        }

        SideFlipped = true;
        enemies.ForEach(x => x.ChangeSide(RuleDefinitions.Side.Ally));
        players.ForEach(x => x.ChangeSide(RuleDefinitions.Side.Enemy));
        Gui.ActivePlayerController.ControlledCharacters.Clear();
        Gui.ActivePlayerController.ControlledCharacters.AddRange(enemies);
    }

    internal static void Stop(GameLocationBattle battle)
    {
        UpdatePartyControllerIds(true);

        if (!SideFlipped)
        {
            return;
        }

        var enemies = battle.EnemyContenders;
        var players = battle.PlayerContenders;

        SideFlipped = false;
        enemies.ForEach(x => x.ChangeSide(RuleDefinitions.Side.Enemy));
        players.ForEach(x => x.ChangeSide(RuleDefinitions.Side.Ally));
        Gui.ActivePlayerController.ControlledCharacters.Clear();
        Gui.ActivePlayerController.ControlledCharacters.AddRange(players);
    }
}
