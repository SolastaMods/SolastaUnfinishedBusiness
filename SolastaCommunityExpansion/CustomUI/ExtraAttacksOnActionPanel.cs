using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Api.AdditionalExtensions;
using SolastaModApi;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.CustomUI
{
    public static class ExtraAttacksOnActionPanel
    {
        // Uses attack mode from cursor's ActionParams, instead of first one matching action type
        // Without this when you click on any attack in actions panel targeting would work as if you clicked on the 1st
        public static void ApplyCursorLocationSelectTargetTranspile(List<CodeInstruction> instructions)
        {
            var insertionIndex = instructions.FindIndex(x => x.opcode == OpCodes.Ldloc_2);

            if (insertionIndex > 0)
            {
                var method = new Func<RulesetAttackMode, CursorLocationSelectTarget, RulesetAttackMode>(
                    GetAttackModeFromCursorActionParams).Method;

                instructions.InsertRange(insertionIndex + 1,
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Call, method),
                        new CodeInstruction(OpCodes.Stloc_2), new CodeInstruction(OpCodes.Ldloc_2)
                    });
            }
        }

        private static RulesetAttackMode GetAttackModeFromCursorActionParams(RulesetAttackMode def,
            CursorLocationSelectTarget cursor)
        {
            if (cursor == null)
            {
                return def;
            }

            return cursor.ActionParams?.AttackMode ?? def;
        }

        // Adds extra items to the action panel if character has more than 1 attack mode available for action type of this panel
        public static void AddExtraAttakItems(CharacterActionPanel panel)
        {
            var actionsTable = panel.characterActionsTable;
            var itemPrefab = panel.actionItemPrefab;
            var filteredActions = panel.filteredActions;
            var startIndex = filteredActions.Count;
            var guiCharacter = panel.GetProperty<GuiCharacter>("GuiCharacter");
            var character = guiCharacter.RulesetCharacter;
            var actionItems = panel.actionItems;

            if (character == null)
            {
                return;
            }

            var actionType = panel.ActionType;
            ActionDefinitions.Id actionId;
            switch (actionType)
            {
                case ActionDefinitions.ActionType.Main:
                    actionId = ActionDefinitions.Id.AttackMain;
                    break;
                case ActionDefinitions.ActionType.Bonus:
                    actionId = ActionDefinitions.Id.AttackOff;
                    break;
                default:
                    return;
            }

            if (!filteredActions.Contains(actionId))
            {
                return;
            }

            var newItems = character.GetAttackModesByActionType(actionType).Count - 1;
            if (newItems <= 0)
            {
                return;
            }

            var index = 0;
            var tableRectTransform = actionsTable.RectTransform;
            for (var i = 0; i < tableRectTransform.childCount; i++)
            {
                var child = tableRectTransform.GetChild(i);
                if (!child.gameObject.activeSelf)
                {
                    continue;
                }

                var component = child.GetComponent<CharacterActionItem>();
                if (component.CurrentItemForm.GuiCharacterAction.ActionId == actionId)
                {
                    index = i + 1;
                    break;
                }
            }

            while (tableRectTransform.childCount < startIndex + newItems)
            {
                Gui.GetPrefabFromPool(itemPrefab, tableRectTransform);
            }

            for (var i = 0; i < newItems; i++)
            {
                var child = tableRectTransform.GetChild(i + startIndex);
                child.gameObject.SetActive(true);

                var component = child.GetComponent<CharacterActionItem>();
                var guiCharacterAction = new CustomGuiCharacterAction(actionId, newItems - i);
                guiCharacterAction.Bind(guiCharacter.GameLocationCharacter, panel.ActionScope);

                component.name = guiCharacterAction.Name;
                CustomBindItem(component, panel, guiCharacterAction, newItems > 1);
                component.Refresh();
                actionItems.Add(component);
                child.SetSiblingIndex(index);
            }

            actionsTable.DispatchChildren(76f);
            panel.RectTransform
                .SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0.0f, tableRectTransform.rect.width);
        }

        private static void CustomBindItem(CharacterActionItem item, CharacterActionPanel panel,
            GuiCharacterAction guiAction, bool small)
        {
            var textColor = panel.textColor;
            var brightColor = panel.brightColor;
            var darkColor = panel.darkColor;
            var darkDisabledColor = panel.darkDisabledColor;
            var tooltipAnchor = panel.tooltipAnchor;

            var smallItemForm = item.smallItemForm;
            var largeItemForm = item.largeItemForm;
            CharacterActionItemForm currentItemForm;

            if (small)
            {
                smallItemForm.gameObject.SetActive(true);
                largeItemForm.gameObject.SetActive(false);
                currentItemForm = smallItemForm;
            }
            else
            {
                smallItemForm.gameObject.SetActive(false);
                largeItemForm.gameObject.SetActive(true);
                currentItemForm = largeItemForm;
            }

            item.SetField("currentItemForm", currentItemForm);

            if (currentItemForm == null)
            {
                return;
            }

            var rectTransform = item.RectTransform;
            var formTransform = currentItemForm.RectTransform.rect;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, formTransform.width);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, formTransform.height);

            void ActionActivated(CharacterActionItemForm form)
            {
                var action = guiAction as CustomGuiCharacterAction;
                // Applies attack mode skipping during activation processing, so that proper attack mode would be used
                action?.ApplySkip();
                panel.OnActivateAction(form);
                action?.RemoveSkip();
            }

            currentItemForm.Bind(guiAction, textColor, brightColor, darkColor, darkDisabledColor, tooltipAnchor,
                ActionActivated, panel.OnPointerEnter, panel.OnPointerExit);

            var enumerator =
                currentItemForm.dynamicItemPropertiesEnumerator;
            enumerator.Unbind();


            var itemDefinition = DatabaseHelper.ItemDefinitions.UnarmedStrikeBase;
            enumerator.Bind(new RulesetItem(itemDefinition));
        }
    }
}
