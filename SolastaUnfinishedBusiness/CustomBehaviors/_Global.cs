using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal static class Global
{
    // true if in a multiplayer game
    internal static bool IsMultiplayer => IsSettingUpMultiplayer
                                          || ServiceRepository.GetService<INetworkingService>().IsMultiplayerGame;

    internal static bool IsSettingUpMultiplayer { get; set; }

    // level up hero
    [CanBeNull]
    internal static RulesetCharacterHero LevelUpHero =>
        ServiceRepository.GetService<ICharacterBuildingService>()?.CurrentLocalHeroCharacter;

    // last level up hero name
    internal static string LastLevelUpHeroName { get; set; }

    // inspected hero on both location and pool
    [CanBeNull] internal static RulesetCharacterHero InspectedHero { get; set; }

    private static GameLocationCharacter ControlledLocationCharacter
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

    internal static RulesetCharacter CurrentCharacter => InspectedHero
                                                         ?? LevelUpHero
                                                         ?? ControlledLocationCharacter?.RulesetCharacter;

    // current action from any character on the map
    internal static CharacterAction CurrentAction { get; set; }

    // BUGFIX: saving throw not passing correct saving delta on attack actions
    // PATCH: Way of Discordance validator logic
    internal static CharacterAction CurrentAttackAction { get; set; }

    // special case for our powers that add a turn off stop provider
    internal static HashSet<FeatureDefinitionPower> PowersThatIgnoreInterruptions { get; } = new();

    // keep a tab on last rolled dices (devastating strike, fell handed)
    internal static int FirstAttackRoll { get; set; }
    internal static int SecondAttackRoll { get; set; }

    // keep a tab on last attack status [i.e: Resonating Strike]
    internal static bool LastAttackWasCantripWeaponAttackHit { get; set; }
}
