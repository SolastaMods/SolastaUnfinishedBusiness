using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Models;

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

        //BUGFIX: game isn't spending power on this scenario [maybe others like OnAttackHit but for now this is ok]
        if (action is CharacterActionSpendPower spendPower &&
            spendPower.activePower.PowerDefinition.rechargeRate == RuleDefinitions.RechargeRate.TurnStart &&
            spendPower.activePower.PowerDefinition.activationTime is RuleDefinitions.ActivationTime.OnAttackHitMelee)
        {
            spendPower.activePower.UsablePower.ForceSpentPoints(spendPower.activePower.PowerDefinition.CostPerUse);
        }

        CurrentAction = null;
    }
}
