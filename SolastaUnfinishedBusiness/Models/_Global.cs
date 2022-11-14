using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomBehaviors;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Models;

internal static class Global
{
    // true if in a multiplayer game
    internal static bool IsMultiplayer => ServiceRepository.GetService<INetworkingService>().IsMultiplayerGame;

    // level up hero
    [CanBeNull]
    internal static RulesetCharacterHero LevelUpHero =>
        ServiceRepository.GetService<ICharacterBuildingService>()?.CurrentLocalHeroCharacter;

    // last level up hero name
    internal static string LastLevelUpHeroName { get; set; }

    // inspected hero on both location and pool
    [CanBeNull] internal static RulesetCharacterHero InspectedHero { get; set; }

    // active player character
    internal static GameLocationCharacter ActionCharacter { get; private set; }

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
                                                         ?? ControlledLocationCharacter?.RulesetCharacter
                                                         ?? ActionCharacter?.RulesetCharacter;

    // current action from any character on the map
    internal static CharacterAction CurrentAction { get; private set; }

    // last attack was a critical hit
    internal static bool CriticalHit { get; set; }

    // conditions that should display on char panel even if set to silent
    internal static HashSet<ConditionDefinition> CharacterLabelEnabledConditions { get; } = new();

    // special case for our powers that add a turn off stop provider
    internal static HashSet<FeatureDefinitionPower> PowersThatIgnoreInterruptions { get; } = new();

    // restate globals on every new action
    internal static void ActionStarted([NotNull] CharacterAction characterAction)
    {
        CurrentAction = characterAction;
        ActionCharacter = characterAction.ActingCharacter;

        Main.Log($"{ActionCharacter?.Name} -> {CurrentAction.ActionDefinition.Name}");

        switch (characterAction)
        {
            case CharacterActionCastSpell:
                // Hold the state of the SHIFT key on BOOL PARAM 5. Used to determine which slot to use on MC Warlock
                var isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

                characterAction.actionParams.BoolParameter5 = isShiftPressed;
                break;

            case CharacterActionReady:
                CustomReactionsContext.ReadReadyActionPreferredCantrip(characterAction.actionParams);
                break;

            case CharacterActionSpendPower spendPower:
                PowerBundle.SpendBundledPowerIfNeeded(spendPower);
                break;
        }
    }
}
