using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

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

    // active player character
    // internal static GameLocationCharacter ActionCharacter { get; private set; }

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
    internal static CharacterAction CurrentAction { get; private set; }

    // last attack was a critical hit
    internal static bool CriticalHit { get; set; }

    // special case for our powers that add a turn off stop provider
    internal static HashSet<FeatureDefinitionPower> PowersThatIgnoreInterruptions { get; } = new();

    // keep a tab on last rolled dices
    internal static int FirstAttackRoll { get; set; }

    internal static int SecondAttackRoll { get; set; }

    // restate globals on every new action
    internal static void ActionStarted([NotNull] CharacterAction characterAction)
    {
        CurrentAction = characterAction;

        // ActionCharacter = characterAction.ActingCharacter;

        switch (characterAction)
        {
            case CharacterActionCastSpell or CharacterActionSpendSpellSlot:
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

    internal static void ActionFinished(CharacterAction action)
    {
        var actingCharacter = action.ActingCharacter;

        foreach (var feature in actingCharacter.RulesetCharacter.GetSubFeaturesByType<IOnAfterActionFeature>())
        {
            feature.OnAfterAction(action);
        }

        //PATCH: allows characters with condition surged to be able to cast spells on each of them
        if (action.ActionType == ActionDefinitions.ActionType.Main)
        {
            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;

            if (rulesetCharacter != null && rulesetCharacter.HasAnyConditionOfType(RuleDefinitions.ConditionSurged))
            {
                // required for Martial Royal Knight surge powers so targets can double cast main spell too
                action.ActingCharacter.UsedMainSpell = false;
                action.ActingCharacter.UsedMainCantrip = false;
            }
        }

        CurrentAction = null;
    }
}
