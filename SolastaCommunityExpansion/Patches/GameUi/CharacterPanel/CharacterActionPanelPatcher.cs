using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Api.AdditionalExtensions;
using SolastaCommunityExpansion.CustomUI;
using SolastaModApi;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterPanel
{
    // Adds extra items to the action panel if character has more than 1 attack mode available for action type of this panel
    [HarmonyPatch(typeof(CharacterActionPanel), "RefreshActions")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterActionPanel_RefreshActions
    {
        internal static void Postfix(CharacterActionPanel __instance)
        {
            var actionsTable = __instance.GetField<GuiTable>("characterActionsTable");
            var itemPrefab = __instance.GetField<GameObject>("actionItemPrefab");
            var filteredActions = __instance.GetField<List<ActionDefinitions.Id>>("filteredActions");
            var startIndex = filteredActions.Count;
            var guiCharacter = __instance.GetProperty<GuiCharacter>("GuiCharacter");
            var character = guiCharacter.RulesetCharacter;
            var actionItems = __instance.GetField<List<CharacterActionItem>>("actionItems");

            if (character == null)
            {
                return;
            }

            var actionType = __instance.ActionType;
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

            while (actionsTable.RectTransform.childCount < startIndex + newItems)
            {
                Gui.GetPrefabFromPool(itemPrefab, actionsTable.RectTransform);
            }

            for (var i = 0; i < newItems; i++)
            {
                var child = actionsTable.RectTransform.GetChild(i + startIndex);
                child.gameObject.SetActive(true);

                var component = child.GetComponent<CharacterActionItem>();
                var guiCharacterAction = new CustomGuiCharacterAction(actionId, i + 1);
                guiCharacterAction.Bind(guiCharacter.GameLocationCharacter, __instance.ActionScope);

                component.name = guiCharacterAction.Name;
                CustomBindItem(component, __instance, guiCharacterAction);
                component.Refresh();
                actionItems.Add(component);
            }

            var tmp = new List<(Transform, int, ActionDefinitions.Id)>();
            for (var i = 0; i < startIndex + newItems; i++)
            {
                var child = actionsTable.RectTransform.GetChild(i);
                tmp.Add((child, i, GetId(child)));
            }

            tmp.Sort((a, b) =>
            {
                if (a.Item3 == b.Item3)
                {
                    return a.Item2.CompareTo(b.Item2);
                }

                return a.Item3.CompareTo(b.Item3);
            });

            for (var i = 0; i < tmp.Count; i++)
            {
                var (child, _, _) = tmp[i];
                child.SetSiblingIndex(i);
            }

            actionsTable.DispatchChildren(76f);
            __instance.RectTransform
                .SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0.0f, actionsTable.RectTransform.rect.width);
        }

        private static ActionDefinitions.Id GetId(Transform transform)
        {
            return transform.GetComponent<CharacterActionItem>().CurrentItemForm.GuiCharacterAction.ActionId;
        }

        private static void CustomBindItem(CharacterActionItem item, CharacterActionPanel panel,
            GuiCharacterAction guiAction)
        {
            var actionDefinition = ServiceRepository.GetService<IGameLocationActionService>()
                .AllActionDefinitions[guiAction.ActionId];

            var textColor = panel.GetField<Color>("textColor");
            var brightColor = panel.GetField<Color>("brightColor");
            var darkColor = panel.GetField<Color>("darkColor");
            var darkDisabledColor = panel.GetField<Color>("darkDisabledColor");
            var tooltipAnchor = panel.GetField<RectTransform>("tooltipAnchor");

            var smallItemForm = item.GetField<CharacterActionItemForm>("smallItemForm");
            var largeItemForm = item.GetField<CharacterActionItemForm>("largeItemForm");
            var currentItemForm = item.GetField<CharacterActionItemForm>("currentItemForm");

            if (actionDefinition.FormType == ActionDefinitions.ActionFormType.Small)
            {
                smallItemForm.gameObject.SetActive(true);
                largeItemForm.gameObject.SetActive(false);
                currentItemForm = smallItemForm;
            }
            else if (actionDefinition.FormType == ActionDefinitions.ActionFormType.Large)
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
                currentItemForm.GetField<DynamicItemPropertiesEnumerator>("dynamicItemPropertiesEnumerator");
            enumerator.Unbind();



            var itemDefinition = DatabaseHelper.ItemDefinitions.UnarmedStrikeBase;
            enumerator.Bind(new RulesetItem(itemDefinition));
        }
    }
}