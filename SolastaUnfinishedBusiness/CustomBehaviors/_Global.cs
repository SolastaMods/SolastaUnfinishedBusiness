using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal static class Global
{
    // true if in a multiplayer game
    internal static bool IsMultiplayer =>
        IsSettingUpMultiplayer
        || ServiceRepository.GetService<INetworkingService>().IsMultiplayerGame;

    internal static bool IsSettingUpMultiplayer { get; set; }

    //PATCH: Keeps last level up hero selected
    internal static string LastLevelUpHeroName { get; set; }

    // level up hero
    [CanBeNull]
    internal static RulesetCharacterHero LevelUpHero =>
        ServiceRepository.GetService<ICharacterBuildingService>()?.CurrentLocalHeroCharacter;

    // inspected hero on both location and pool
    [CanBeNull] internal static RulesetCharacterHero InspectedHero { get; set; }

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
    internal static RulesetCharacter CurrentCharacter =>
        InspectedHero
        ?? LevelUpHero
        ?? SelectedLocationCharacter?.RulesetCharacter;

    //BUGFIX: saving throw not passing correct saving delta on attack actions
    internal static CharacterAction CurrentAttackAction { get; set; }

    // special case for our powers that add a turn off stop provider
    internal static HashSet<FeatureDefinitionPower> PowersThatIgnoreInterruptions { get; } = new();

    // keep a tab on last rolled dices [Devastating Strike, Fell Handed]
    internal static int LowestAttackRoll { get; set; }
    internal static int HighestAttackRoll { get; set; }

    // keep a tab on last attack status [Booming Blade, Burning Blade, Resonating Strike]
    internal static bool LastAttackWasCantripWeaponAttackHit { get; set; }
}
