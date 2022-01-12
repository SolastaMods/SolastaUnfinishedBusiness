using HarmonyLib;

namespace SolastaCommunityExpansionMulticlass.Patches.BugFix
{
    //
    // TODO: remove this patch when TA fixes this inventory bug on chars. pool
    //
    internal static class InventoryPanelPatcher
    {
        [HarmonyPatch(typeof(InventoryPanel), "EndInteraction")]
        internal static class InventoryPanelEndInteraction
        {
            internal static bool Prefix(InventoryPanel __instance)
            {
                if (__instance.personalContainerPanel == null || __instance.GuiCharacter == null || __instance.DraggedItem == null)
                {
                    __instance.CancelDrag();
                }
                else
                {
                    var containerPanel = (ContainerPanel)null;
                    if (__instance.personalContainerPanel.Visible && __instance.personalContainerPanel.ContainsMouseCursor(__instance.transform.lossyScale))
                    {
                        containerPanel = __instance.personalContainerPanel;
                    }

                    if (containerPanel == null && __instance.externalContainerPanel != null && (__instance.externalContainerPanel.Visible && __instance.externalContainerPanel.ContainsMouseCursor(__instance.transform.lossyScale)))
                    {
                        containerPanel = __instance.externalContainerPanel;
                    }

                    if (containerPanel == __instance.personalContainerPanel && !__instance.GuiCharacter.RulesetCharacterHero.CanCarryItem(__instance.DraggedItem))
                    {
                        Gui.GuiService.ShowAlert("Screen/&InventoryMaxWeightCapacityDescription", "EA7171");
                    }
                    else if (containerPanel == __instance.externalContainerPanel && __instance.DraggedItem.ItemDefinition.ItemTags.Contains("Quest"))
                    {
                        Gui.GuiService.ShowAlert("Screen/&InventoryCannotDropQuestItemDescription", "C39D63");
                    }
                    else if (containerPanel == __instance.externalContainerPanel && ServiceRepository.GetService<IRulesetItemFactoryService>().IsPersonalSpellbook(__instance.DraggedItem, __instance.GuiCharacter.RulesetCharacterHero))
                    {
                        Gui.GuiService.ShowAlert("Screen/&InventoryCannotDropPersonalSpellbookDescription", "C39D63");
                    }
                    else
                    {
                        var slotBox = (InventorySlotBox)null;
                        var ground = false;
                        var externalContainer = false;
                        if (__instance.externalContainerPanel != null && __instance.externalContainerPanel.DropAreaActive && __instance.externalContainerPanel.DropAreaContainsCursor(__instance.transform.lossyScale))
                        {
                            ServiceRepository.GetService<IInventoryCommandService>().AddContainerSubItem(__instance.externalContainerPanel.Container, __instance.DraggedItem, __instance.GuiCharacter != null ? (__instance.GuiCharacter.GameLocationCharacter != null ? __instance.GuiCharacter.GameLocationCharacter.LocationPosition : TA.int3.zero) : TA.int3.zero);
                            __instance.SpendInventoryActionAsNeeded();
                            __instance.SignalItemInteraction(__instance.DraggedItem, null, InventoryPanel.InteractionType.Store);
                            __instance.StopDrag(false);
                        }
                        else if (__instance.TryPickInventorySlot(out slotBox, out ground, out externalContainer))
                        {
                            var service = ServiceRepository.GetService<IInventoryCommandService>();
                            if (slotBox.InventorySlot.EquipedItem == null)
                            {
                                var count = -1;
                                if (__instance.DraggedItem.ItemDefinition.CanBeStacked)
                                {
                                    count = __instance.DraggedItem.StackCount;
                                }

                                if (count <= 1 || slotBox.InventorySlot.SlotTypeDefinition.CanStack)
                                {
                                    __instance.SignalItemInteraction(__instance.DraggedItem, slotBox.InventorySlot, InventoryPanel.InteractionType.Store);
                                    service?.EquipItem(slotBox.InventorySlot, __instance.DraggedItem, __instance.GuiCharacter?.RulesetCharacterHero, count);
                                    __instance.SpendInventoryActionAsNeeded(slotBox.InventorySlot);
                                    __instance.StopDrag(false);
                                }
                                else
                                {
                                    __instance.SplitItemAndHandleStacks(__instance.DraggedItem, 1, -1, slotBox.InventorySlot);
                                    if (!ServiceRepository.GetService<INetworkingService>().IsMultiplayerGame)
                                    {
                                        return false;
                                    }

                                    __instance.CancelDrag();
                                }
                            }
                            else
                            {
                                var equipedItem1 = slotBox.InventorySlot.EquipedItem;
                                var count1 = 0;
                                if (__instance.DraggedItem.ItemDefinition.CanBeStacked)
                                {
                                    count1 = __instance.DraggedItem.StackCount;
                                }

                                if (slotBox.InventorySlot.SlotTypeDefinition.CanStack && RulesetItem.CanStackItems(equipedItem1, __instance.DraggedItem))
                                {
                                    var count2 = equipedItem1.StackCount + count1;
                                    if (count2 <= __instance.DraggedItem.ItemDefinition.StackSize)
                                    {
                                        __instance.SignalItemInteraction(__instance.DraggedItem, slotBox.InventorySlot, InventoryPanel.InteractionType.Store);
                                        service?.EquipItem(slotBox.InventorySlot, equipedItem1, __instance.GuiCharacter?.RulesetCharacterHero, count2);
                                        __instance.SpendInventoryActionAsNeeded();
                                        __instance.StopDrag(true);
                                    }
                                    else
                                    {
                                        if (equipedItem1.StackCount >= equipedItem1.ItemDefinition.StackSize)
                                        {
                                            return false;
                                        }

                                        var attribute = __instance.DraggedItem.GetAttribute("ItemStackCount");
                                        attribute.BaseValue -= equipedItem1.ItemDefinition.StackSize - equipedItem1.StackCount;
                                        attribute.Refresh();
                                        __instance.draggedItemStackCountLabel.Text = attribute.CurrentValue.ToString();
                                        slotBox.InventorySlot.EquipItem(equipedItem1, equipedItem1.ItemDefinition.StackSize);
                                    }
                                }
                                else if (slotBox.InventorySlot.SlotTypeDefinition.CanStack && equipedItem1 != null && equipedItem1.ItemDefinition != __instance.DraggedItem.ItemDefinition)
                                {
                                    var equipedItem2 = slotBox.InventorySlot.EquipedItem;
                                    slotBox.UnequipItem();
                                    slotBox.EquipItem(__instance.DraggedItem, count1);
                                    __instance.SignalItemInteraction(__instance.DraggedItem, slotBox.InventorySlot, InventoryPanel.InteractionType.Store);
                                    if (__instance.DragWithoutReleaseMode | __instance.SpendInventoryActionAsNeeded())
                                    {
                                        service.ReleaseItem(__instance.GuiCharacter.RulesetCharacterHero, equipedItem2);
                                        __instance.StopDrag(false);
                                    }
                                    else
                                    {
                                        __instance.StartDrag(equipedItem2);
                                    }
                                }
                                else
                                {
                                    if (slotBox.InventorySlot.SlotTypeDefinition.CanStack || equipedItem1 == null || !(equipedItem1.ItemDefinition != __instance.DraggedItem.ItemDefinition) && !(equipedItem1.AttunedToCharacter != __instance.DraggedItem.AttunedToCharacter))
                                    {
                                        return false;
                                    }

                                    if (!__instance.DraggedItem.CanStillBeStacked || count1 <= 1)
                                    {
                                        var equipedItem2 = slotBox.InventorySlot.EquipedItem;
                                        slotBox.UnequipItem();
                                        slotBox.EquipItem(__instance.DraggedItem);
                                        __instance.SignalItemInteraction(__instance.DraggedItem, slotBox.InventorySlot, InventoryPanel.InteractionType.Store);
                                        if (__instance.DragWithoutReleaseMode)
                                        {
                                            service.ReleaseItem(__instance.GuiCharacter.RulesetCharacterHero, equipedItem2);
                                            __instance.SpendInventoryActionAsNeeded();
                                            __instance.StopDrag(false);
                                        }
                                        else
                                        {
                                            __instance.StartDrag(equipedItem2);
                                        }
                                    }
                                    else
                                    {
                                        __instance.SignalItemInteraction(__instance.DraggedItem, slotBox.InventorySlot, InventoryPanel.InteractionType.Store);
                                        __instance.SplitItemAndHandleStacks(__instance.DraggedItem, 1, -1, slotBox.InventorySlot);
                                        __instance.CancelDrag();
                                        if (__instance.DragWithoutReleaseMode)
                                        {
                                            service.ReleaseItem(__instance.GuiCharacter.RulesetCharacterHero, equipedItem1);
                                            __instance.SpendInventoryActionAsNeeded();
                                            __instance.StopDrag(false);
                                        }
                                        else
                                        {
                                            __instance.StartDrag(equipedItem1);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!__instance.DragWithoutReleaseMode)
                            {
                                return false;
                            }

                            if (ServiceRepository.GetService<IRulesetItemFactoryService>().IsPersonalSpellbook(__instance.DraggedItem, __instance.GuiCharacter.RulesetCharacterHero))
                            {
                                Gui.GuiService.ShowAlert("Screen/&InventoryCannotTransferPersonalSpellbookDescription", "C39D63");
                            }
                            else
                            {
                                for (var index = 0; index < __instance.characterPlatesTable.childCount; ++index)
                                {
                                    var component = __instance.characterPlatesTable.GetChild(index).GetComponent<CharacterPlateGameSelector>();
                                    if (component.gameObject.activeSelf && component.GuiCharacter != null && component.ContainsMouseCursor(__instance.transform.lossyScale))
                                    {
                                        component.OnPointerClick(null);
                                    }
                                }
                                __instance.CancelDrag();
                            }
                        }
                    }
                }
                return false;
            }
        }
    }
}
