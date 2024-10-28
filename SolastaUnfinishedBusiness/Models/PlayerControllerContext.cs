using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TA.AI;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.DecisionPackageDefinitions;

namespace SolastaUnfinishedBusiness.Models;

internal static class PlayerControllerContext
{
    private const int PlayerControllerID = 1;

    private static readonly Dictionary<GameLocationCharacter, int> ControllersChoices = [];

    private static readonly DecisionPackageDefinition[] AiDecisionPackages =
    [
        DefaultMeleeWithBackupRangeDecisions,
        DefaultMeleeWithBackupRangeDecisions,
        DefaultRangeWithBackupMeleeDecisions,
        DefaultSupportCasterWithBackupAttacksDecisions,
        ClericCombatDecisions,
        FighterCombatDecisions,
        CasterCombatDecisions,
        RogueCombatDecisions
    ];

    internal static readonly string[] Controllers =
    [
        "Human",
        "Melee",
        "Range",
        "Caster",
        "Cleric",
        "Fighter",
        "Mage",
        "Rogue"
    ];

    // ReSharper disable once InconsistentNaming
    private static int[] playerCharactersChoices { get; set; }

    internal static List<GameLocationCharacter> PlayerCharacters { get; } = [];

    internal static int[] PlayerCharactersChoices
    {
        get => playerCharactersChoices;

        set
        {
            playerCharactersChoices = value;

            for (var i = 0; i < value.Length; i++)
            {
                var playerCharacter = PlayerCharacters[i];

                ControllersChoices[playerCharacter] = value[i];
            }
        }
    }

    private static bool SideFlipped { get; set; }

    internal static void RefreshGuiState()
    {
        if (!Gui.GameCampaign)
        {
            return;
        }

        var controllersChoicesCopy = ControllersChoices.ToDictionary(x => x.Key, x => x.Value);
        var characterService = ServiceRepository.GetService<IGameLocationCharacterService>();

        ControllersChoices.Clear();
        PlayerCharacters.Clear();
        PlayerCharacters.AddRange(characterService.PartyCharacters);
        PlayerCharacters.AddRange(characterService.GuestCharacters);
        PlayerCharacters.ForEach(x =>
            ControllersChoices.Add(x, controllersChoicesCopy.TryGetValue(x, out var choice) ? choice : 0));
        playerCharactersChoices = [.. ControllersChoices.Values];
    }

    private static void UpdatePartyControllerIds(bool reset = false)
    {
        var activePlayerController = Gui.ActivePlayerController;

        foreach (var playerCharacter in PlayerCharacters)
        {
            var choice = ControllersChoices[playerCharacter];
            var controllerId = reset || choice == 0
                ? PlayerControllerID
                : PlayerControllerManager.DmControllerId;

            playerCharacter.ControllerId = controllerId;

            if (controllerId != PlayerControllerID)
            {
                continue;
            }

            var decisionPackage = AiDecisionPackages[choice];

            playerCharacter.BehaviourPackage ??= new GameLocationBehaviourPackage
            {
                BattleStartBehavior =
                    GameLocationBehaviourPackage.BattleStartBehaviorType.DoNotRaiseAlarm
            };

            playerCharacter.BehaviourPackage.DecisionPackageDefinition = decisionPackage;
        }

        activePlayerController.DirtyControlledCharacters();
    }

    internal static void Start([NotNull] GameLocationBattle battle)
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
        enemies.ForEach(x => x.ChangeSide(Side.Ally));
        players.ForEach(x => x.ChangeSide(Side.Enemy));
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
        enemies.ForEach(x => x.ChangeSide(Side.Enemy));
        players.ForEach(x => x.ChangeSide(Side.Ally));
        Gui.ActivePlayerController.ControlledCharacters.Clear();
        Gui.ActivePlayerController.ControlledCharacters.AddRange(players);
    }
}
