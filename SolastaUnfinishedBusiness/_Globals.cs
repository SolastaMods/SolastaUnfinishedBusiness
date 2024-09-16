using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness;

internal static class Global
{
    // true if in a multiplayer game
    internal static bool IsMultiplayer =>
        IsSettingUpMultiplayer || ServiceRepository.GetService<INetworkingService>().IsMultiplayerGame;

    // true if on multiplayer setup screen
    internal static bool IsSettingUpMultiplayer { get; set; }

    // last level up hero name
    internal static string LastLevelUpHeroName { get; set; }

    // level up hero
    [CanBeNull]
    internal static RulesetCharacterHero LevelUpHero =>
        ServiceRepository.GetService<ICharacterBuildingService>()?.CurrentLocalHeroCharacter;

    // inspected hero on both location and pool
    [CanBeNull] internal static RulesetCharacterHero InspectedHero { get; set; }

    [CanBeNull]
    private static GameLocationCharacter SelectedLocationCharacter
    {
        get
        {
            var gameLocationScreenExploration = Gui.GuiService.GetScreen<GameLocationScreenExploration>();
            var gameLocationScreenBattle = Gui.GuiService.GetScreen<GameLocationScreenBattle>();

            return gameLocationScreenExploration.Visible
                ? gameLocationScreenExploration.CharacterControlPanel.GuiCharacter?.GameLocationCharacter
                : gameLocationScreenBattle.CharacterControlPanel.GuiCharacter?.GameLocationCharacter;
        }
    }

    // used in UI references
    [CanBeNull]
    internal static RulesetCharacter CurrentCharacter =>
        InspectedHero ?? LevelUpHero ?? SelectedLocationCharacter?.RulesetCharacter;

    internal static void RefreshControlledCharacter()
    {
        SelectedLocationCharacter?.RulesetCharacter?.RefreshAll();
    }
}
