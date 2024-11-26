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

            var wieldedItemsConfigurations = __instance.GuiCharacter.RulesetCharacterHero.CharacterInventory
                .WieldedItemsConfigurations;
            while (__instance.configurationsTable.childCount < wieldedItemsConfigurations.Count)
            {
                Gui.GetPrefabFromPool(__instance.wieldedConfigurationButtonPrefab, __instance.configurationsTable);
            }

            for (var i = 0; i < wieldedItemsConfigurations.Count; i++)
            {
                var child = __instance.configurationsTable.GetChild(i);
                child.gameObject.SetActive(true);
                var component = child.GetComponent<WieldedConfigurationSelector>();
                component.Bind(__instance.GuiCharacter, i, wieldedItemsConfigurations[i],
                    __instance.OnConfigurationSwitched,
                    i == __instance.GuiCharacter.RulesetCharacterHero.CharacterInventory.CurrentConfiguration,
                    __instance.inMainHud,
                    __instance.forceRefresh,
                    __instance.tooltipAnchor);
                var flag = false;
                if (__instance.GuiCharacter.GameLocationCharacter != null)
                {
                    var service = ServiceRepository.GetService<IPlayerControllerService>();
                    var flag2 = service?.ActivePlayerController?.IsCharacterControlled(__instance.GuiCharacter
                        .GameLocationCharacter);
                    flag = flag2 ?? true;
                }
                else if (__instance.GuiCharacter.GameCampaignCharacter != null)
                {
                    var service2 = ServiceRepository.GetService<IPlayerControllerService>();
                    var flag3 = service2?.ActivePlayerController?.IsCharacterControlled(__instance.GuiCharacter
                        .RulesetCharacter);
                    flag = flag3 ?? true;
                }

                if (!flag)
                {
                    component.Interactable = false;
                    component.Tooltip.Content = component.TooltipContent;
                }
                else if (__instance.GuiCharacter.GameLocationCharacter != null &&
                         __instance.GuiCharacter.GameLocationCharacter.HasForcedActionOrManipulation())
                {
                    component.Interactable = false;
                    component.Tooltip.Content = component.TooltipContent;
                }
                else switch (Main.Settings.EnableUnlimitedInventoryActions)
                {
                    case false when
                        Gui.Battle != null &&
                        __instance.GuiCharacter.GameLocationCharacter != null &&
                        __instance.GuiCharacter.GameLocationCharacter.GetActionTypeStatus(ActionType.FreeOnce) ==
                        ActionStatus.Spent && !__instance.ItemSelectionInProgress:
                        component.Tooltip.Content = Gui.FormatFailure(component.TooltipContent,
                            "Failure/&FailureFlagFreeOnceActionSpent");
                        component.Interactable = false;
                        break;
                    case false when
                        Gui.Battle != null &&
                        __instance.GuiCharacter.GameLocationCharacter != null &&
                        __instance.GuiCharacter.GameLocationCharacter.GetActionTypeStatus(ActionType.FreeOnce) ==
                        ActionStatus.Unavailable && !__instance.ItemSelectionInProgress:
                        component.Tooltip.Content = Gui.FormatFailure(component.TooltipContent,
                            "Failure/&FailureFlagFreeOnceActionUnavailable");
                        component.Interactable = false;
                        break;
                    case true when
                        __instance.GuiCharacter.GameLocationCharacter != null:
                        __instance.GuiCharacter.GameLocationCharacter.RefundActionUse(ActionType.FreeOnce);
                    
                        component.Interactable = true;
                        component.Tooltip.Content = component.TooltipContent;
                        break;
                    default:
                        component.Interactable = true;
                        component.Tooltip.Content = component.TooltipContent;
                        break;
                }
            }

            for (var j = wieldedItemsConfigurations.Count; j < __instance.configurationsTable.childCount; j++)
            {
                __instance.configurationsTable.GetChild(j).gameObject.SetActive(false);
            }

            return false;
        }
    }
}
