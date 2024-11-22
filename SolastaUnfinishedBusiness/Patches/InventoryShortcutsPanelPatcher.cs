using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using UnityEngine;
using static ActionDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class InventoryShortcutsPanelPatcher
{
    [HarmonyPatch(typeof(InventoryShortcutsPanel), nameof(InventoryShortcutsPanel.OnConfigurationSwitched))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnConfigurationSwitched_Patch
    {
        [UsedImplicitly]
        public static void Prefix(ref int rank)
        {
            var isCtrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

            if (Main.Settings.EnableCtrlClickOnlySwapsMainHand && isCtrlPressed)
            {
                rank += 100;
            }
        }

        [UsedImplicitly]
        public static void Postfix(InventoryShortcutsPanel __instance, int rank)
        {
            if (rank < 100)
            {
                return;
            }

            rank -= 100;

            var itemsConfigurations = __instance.GuiCharacter.RulesetCharacterHero.CharacterInventory
                .WieldedItemsConfigurations;

            for (var index = 0; index < itemsConfigurations.Count; ++index)
            {
                __instance.configurationsTable.GetChild(index).GetComponent<WieldedConfigurationSelector>().Selected =
                    index == rank;
            }
        }
    }

    //PATCH: QuickCastLightCantripOnWornItemsFirst
    [HarmonyPatch(typeof(InventoryShortcutsPanel), nameof(InventoryShortcutsPanel.OnCastLightCb))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    internal static class OnCastLightCb_Patch
    {
        [UsedImplicitly]
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var oldMethod = typeof(RulesetCharacter).GetMethod(nameof(RulesetCharacter.TryFindTargetWieldedItem));
            var newMethod = typeof(OnCastLightCb_Patch).GetMethod(nameof(MyTryFindTargetWieldedItem));

            return instructions.ReplaceCall(oldMethod, 1, "InventoryShortcutsPanel.OnCastLightCb",
                new CodeInstruction(OpCodes.Call, newMethod));
        }

        public static bool MyTryFindTargetWieldedItem([NotNull] RulesetCharacter rulesetCharacter,
            out RulesetItem targetItem, bool fallbackOnTorsoArmor = false)
        {
            if (!Main.Settings.QuickCastLightCantripOnWornItemsFirst ||
                rulesetCharacter is not RulesetCharacterHero hero)
            {
                return rulesetCharacter.TryFindTargetWieldedItem(out targetItem, fallbackOnTorsoArmor);
            }

            var slots = hero.CharacterInventory.InventorySlotsByName;

            targetItem = slots[EquipmentDefinitions.SlotTypeHead].EquipedItem
                         ?? slots[EquipmentDefinitions.SlotTypeNeck].EquipedItem
                         ?? slots[EquipmentDefinitions.SlotTypeTorso].EquipedItem;

            return targetItem != null || hero.TryFindTargetWieldedItem(out targetItem);
        }
    }

    //PATCH: enable unlimited inventory actions
    [HarmonyPatch(typeof(InventoryShortcutsPanel), nameof(InventoryShortcutsPanel.BindConfigurations))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class BindConfigurationsPatch
    {
        public static bool Prefix(InventoryShortcutsPanel __instance)
        {
            if (__instance.GuiCharacter == null || __instance.GuiCharacter.RulesetCharacterHero == null)
            {
                return false;
            }

            List<RulesetWieldedConfiguration> wieldedItemsConfigurations = __instance.GuiCharacter.RulesetCharacterHero.CharacterInventory.WieldedItemsConfigurations;
            while (__instance.configurationsTable.childCount < wieldedItemsConfigurations.Count)
            {
                Gui.GetPrefabFromPool(__instance.wieldedConfigurationButtonPrefab, __instance.configurationsTable);
            }
            for (int i = 0; i < wieldedItemsConfigurations.Count; i++)
            {
                Transform child = __instance.configurationsTable.GetChild(i);
                child.gameObject.SetActive(true);
                WieldedConfigurationSelector component = child.GetComponent<WieldedConfigurationSelector>();
                component.Bind(__instance.GuiCharacter, i, wieldedItemsConfigurations[i], new
                    WieldedConfigurationSelector.OnConfigurationSwitchedHandler(__instance.OnConfigurationSwitched),
                    i == __instance.GuiCharacter.RulesetCharacterHero.CharacterInventory.CurrentConfiguration,
                    __instance.inMainHud,
                    __instance.forceRefresh,
                    __instance.tooltipAnchor);
                bool flag = false;
                if (__instance.GuiCharacter.GameLocationCharacter != null)
                {
                    IPlayerControllerService service = ServiceRepository.GetService<IPlayerControllerService>();
                    bool? flag2 = service?.ActivePlayerController?.IsCharacterControlled(__instance.GuiCharacter.GameLocationCharacter);
                    flag = (flag2 ?? true);
                }
                else if (__instance.GuiCharacter.GameCampaignCharacter != null)
                {
                    IPlayerControllerService service2 = ServiceRepository.GetService<IPlayerControllerService>();
                    bool? flag3 = service2?.ActivePlayerController?.IsCharacterControlled(__instance.GuiCharacter.RulesetCharacter);
                    flag = (flag3 ?? true);
                }
                if (!flag)
                {
                    component.Interactable = false;
                    component.Tooltip.Content = component.TooltipContent;
                }
                else if (__instance.GuiCharacter.GameLocationCharacter != null && __instance.GuiCharacter.GameLocationCharacter.HasForcedActionOrManipulation())
                {
                    component.Interactable = false;
                    component.Tooltip.Content = component.TooltipContent;
                }
                else if (!Main.Settings.EnableUnlimitedInventoryActions && Gui.Battle != null &&
                    __instance.GuiCharacter.GameLocationCharacter.GetActionTypeStatus(ActionType.FreeOnce, ActionScope.Battle, false) ==
                    ActionStatus.Spent && !__instance.ItemSelectionInProgress)
                {
                    component.Tooltip.Content = Gui.FormatFailure(component.TooltipContent, "Failure/&FailureFlagFreeOnceActionSpent", true);
                    component.Interactable = false;
                }
                else if (!Main.Settings.EnableUnlimitedInventoryActions && Gui.Battle != null &&
                    __instance.GuiCharacter.GameLocationCharacter.GetActionTypeStatus(ActionType.FreeOnce, ActionScope.Battle, false) ==
                    ActionStatus.Unavailable && !__instance.ItemSelectionInProgress)
                {
                    component.Tooltip.Content = Gui.FormatFailure(component.TooltipContent, "Failure/&FailureFlagFreeOnceActionUnavailable", true);
                    component.Interactable = false;
                }
                else if (Main.Settings.EnableUnlimitedInventoryActions)
                {
                    var gameLocationCharacter = __instance.GuiCharacter.GameLocationCharacter;
                    gameLocationCharacter.RefundActionUse(ActionType.FreeOnce);
                    component.Interactable = true;
                    component.Tooltip.Content = component.TooltipContent;
                }
                else
                {
                    component.Interactable = true;
                    component.Tooltip.Content = component.TooltipContent;
                }
            }
            for (int j = wieldedItemsConfigurations.Count; j < __instance.configurationsTable.childCount; j++)
            {
                __instance.configurationsTable.GetChild(j).gameObject.SetActive(false);
            }
            return false;
        }
    }
}
