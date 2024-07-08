using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness;

internal static class Global
{
    // true if in a multiplayer game
    internal static bool IsMultiplayer =>
        IsSettingUpMultiplayer
        || ServiceRepository.GetService<INetworkingService>().IsMultiplayerGame;

    // true if on multiplayer setup screen
    internal static bool IsSettingUpMultiplayer { get; set; }

    //PATCH: Keeps last level up hero selected
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
            var exploration = Gui.GuiService.GetScreen<GameLocationScreenExploration>();

            if (exploration.Visible)
            {
                return exploration.CharacterControlPanel.GuiCharacter?.GameLocationCharacter;
            }

            var battle = Gui.GuiService.GetScreen<GameLocationScreenBattle>();

            return battle.Visible
                ? battle.CharacterControlPanel.GuiCharacter?.GameLocationCharacter
                : null;
        }
    }

    //PATCH: used in UI references
    [CanBeNull]
    internal static RulesetCharacter CurrentCharacter =>
        InspectedHero
        ?? LevelUpHero
        ?? SelectedLocationCharacter?.RulesetCharacter;
}
