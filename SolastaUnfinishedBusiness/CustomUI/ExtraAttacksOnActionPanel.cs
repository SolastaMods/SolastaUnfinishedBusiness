using System.Linq;
using JetBrains.Annotations;
using static ActionDefinitions;

namespace SolastaUnfinishedBusiness.CustomUI;

internal static class ExtraAttacksOnActionPanel
{
    internal static RulesetAttackMode FindExtraActionAttackModesFromGuiAction(
        GameLocationCharacter character,
        Id actionId,
        bool getWithMostAttackNb,
        bool onlyIfRemainingUses,
        bool onlyIfCanUseAction,
        ReadyActionType readyActionType,
        GuiCharacterAction guiAction)
    {
        if (actionId != Id.AttackOff || guiAction.ForcedAttackMode == null)
        {
            return character.FindActionAttackMode(actionId, getWithMostAttackNb, onlyIfRemainingUses,
                onlyIfCanUseAction);
        }

        return guiAction.ForcedAttackMode;
    }

    internal static RulesetAttackMode FindExtraActionAttackModesFromForcedAttack(
        GameLocationCharacter character,
        Id actionId,
        bool getWithMostAttackNb,
        bool onlyIfRemainingUses,
        bool onlyIfCanUseAction,
        ReadyActionType readyActionType,
        [CanBeNull] RulesetAttackMode forcedAttack)
    {
        if (actionId != Id.AttackOff || forcedAttack == null)
        {
            return character.FindActionAttackMode(actionId, getWithMostAttackNb, onlyIfRemainingUses,
                onlyIfCanUseAction);
        }

        return forcedAttack;
    }

    internal static int ComputeMultipleGuiCharacterActions(
        CharacterActionPanel panel,
        Id actionId,
        int def)
    {
        var multiple = CanActionHaveMultipleGuiItems(actionId);

        if (!multiple)
        {
            if (panel.guiActionsById.TryGetValue(actionId, out var actionsList))
            {
                actionsList.Clear();
            }

            if (!panel.guiActionById.ContainsKey(actionId))
            {
                panel.guiActionById.Add(actionId, new GuiCharacterAction(actionId));
            }

            return 1;
        }

        if (actionId == Id.AttackMain
            && ServiceRepository.GetService<IGameLocationCharacterService>().GuestCharacters
                .Contains(panel.GuiCharacter.GameLocationCharacter))
        {
            //this case was processed by base method
            return def;
        }

        ActionType actionType;

        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (actionId)
        {
            case Id.AttackMain:
                actionType = ActionType.Main;
                break;
            case Id.AttackOff:
                actionType = ActionType.Bonus;
                break;
            default:
                return def;
        }

        if (!panel.guiActionsById.TryGetValue(actionId, out var actions))
        {
            actions = [];
            panel.guiActionsById.Add(actionId, actions);
        }
        else
        {
            actions.Clear();
        }

        foreach (var attackMode in panel.GuiCharacter.RulesetCharacter.AttackModes
                     .Where(attackMode => attackMode.ActionType == actionType && !actions.Any(guiCharacterAction =>
                         guiCharacterAction.ActionId == actionId && guiCharacterAction.ForcedAttackMode == attackMode)))
        {
            actions.Add(new GuiCharacterAction(actionId) { ForcedAttackMode = attackMode });
        }

        return actions.Count;
    }
}
