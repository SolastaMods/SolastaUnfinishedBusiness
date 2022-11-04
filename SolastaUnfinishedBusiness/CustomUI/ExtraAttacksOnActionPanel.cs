using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using static ActionDefinitions;

namespace SolastaUnfinishedBusiness.CustomUI;

internal static class ExtraAttacksOnActionPanel
{
    internal static IEnumerable<CodeInstruction> ReplaceFindExtraActionAttackModesInActionPanel(
        IEnumerable<CodeInstruction> instructions)
    {
        var findAttacks = typeof(GameLocationCharacter).GetMethod("FindActionAttackMode");
        var method = new Func<
            GameLocationCharacter,
            Id,
            bool,
            bool,
            GuiCharacterAction,
            RulesetAttackMode
        >(FindExtraActionAttackModesFromGuiAction).Method;

        return instructions.ReplaceCall(findAttacks,
            new CodeInstruction(OpCodes.Ldarg_2),
            new CodeInstruction(OpCodes.Call, method));
    }

    private static RulesetAttackMode FindExtraActionAttackModesFromGuiAction(
        GameLocationCharacter character,
        Id actionId,
        bool getWithMostAttackNb,
        bool onlyIfRemainingUses,
        GuiCharacterAction guiAction)
    {
        if (actionId != Id.AttackOff || guiAction.ForcedAttackMode == null)
        {
            return character.FindActionAttackMode(actionId, getWithMostAttackNb, onlyIfRemainingUses);
        }

        return guiAction.ForcedAttackMode;
    }

    internal static IEnumerable<CodeInstruction> ReplaceFindExtraActionAttackModesInLocationCharacter(
        IEnumerable<CodeInstruction> instructions)
    {
        var findAttacks = typeof(GameLocationCharacter).GetMethod("FindActionAttackMode");
        var method = new Func<
            GameLocationCharacter,
            Id,
            bool,
            bool,
            RulesetAttackMode,
            RulesetAttackMode
        >(FindExtraActionAttackModesFromForcedAttack).Method;

        return instructions.ReplaceCall(findAttacks,
            new CodeInstruction(OpCodes.Ldarg_2),
            new CodeInstruction(OpCodes.Call, method));
    }

    private static RulesetAttackMode FindExtraActionAttackModesFromForcedAttack(
        GameLocationCharacter character,
        Id actionId,
        bool getWithMostAttackNb,
        bool onlyIfRemainingUses,
        [CanBeNull] RulesetAttackMode forcedAttack)
    {
        if (actionId != Id.AttackOff || forcedAttack == null)
        {
            return character.FindActionAttackMode(actionId, getWithMostAttackNb, onlyIfRemainingUses);
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
            actions = new List<GuiCharacterAction>();
            panel.guiActionsById.Add(actionId, actions);
        }
        else
        {
            actions.Clear();
        }

        foreach (var attackMode in panel.GuiCharacter.RulesetCharacter.AttackModes)
        {
            if (attackMode.ActionType != actionType)
            {
                continue;
            }

            var exists = actions.Any(guiCharacterAction =>
                guiCharacterAction.ActionId == actionId && guiCharacterAction.ForcedAttackMode == attackMode);

            if (exists)
            {
                continue;
            }

            var guiCharacterAction = new GuiCharacterAction(actionId) { ForcedAttackMode = attackMode };
            actions.Add(guiCharacterAction);
        }

        return actions.Count;
    }
}
