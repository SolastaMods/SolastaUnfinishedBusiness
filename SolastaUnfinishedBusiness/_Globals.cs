using JetBrains.Annotations;
using UnityEngine;

namespace SolastaUnfinishedBusiness;

internal static class Global
{
    // true if in a multiplayer game to prevent
    // SFX, default party, alternate voting system, multi-heroes reaction order, PCG random, formation, encounters
    internal static bool IsMultiplayer =>
        IsSettingUpMultiplayer || ServiceRepository.GetService<INetworkingService>().IsMultiplayerGame;

    // true if on multiplayer setup screen to prevent default party
    internal static bool IsSettingUpMultiplayer { get; set; }

    // last level up hero name to allow an easier level up on characters pool with less mouse clicks
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

    internal static bool IsShiftPressed => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

    internal static void RefreshControlledCharacter()
    {
        SelectedLocationCharacter?.RulesetCharacter?.RefreshAll();
    }
}
