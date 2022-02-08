using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.GameUi.Inventory
{
    [HarmonyPatch(typeof(InventoryShortcutsPanel), "OnConfigurationSwitched")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class InventoryShortcutsPanel_OnConfigurationSwitched
    {
        private const int LIGHT_SOURCE = 2;

        internal static void Prefix(InventoryShortcutsPanel __instance, int rank)
        {
            var isCtrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            var characterInventory = __instance.GuiCharacter.RulesetCharacterHero.CharacterInventory;
            var itemsConfigurations = characterInventory.WieldedItemsConfigurations;
            var currentRank = characterInventory.CurrentConfiguration;

            if (!Main.Settings.EnableCtrlClickOnlySwapsMainHand || !isCtrlPressed)
            {
                return;
            }

            if (rank == LIGHT_SOURCE || currentRank == LIGHT_SOURCE)
            {
                return;
            }

            if (itemsConfigurations[rank].OffHandSlot.Disabled || itemsConfigurations[currentRank].OffHandSlot.Disabled)
            {
                return;
            }

            var equipedItem0 = itemsConfigurations[rank].OffHandSlot.EquipedItem;
            var equipedItem1 = itemsConfigurations[currentRank].OffHandSlot.EquipedItem;

            itemsConfigurations[rank].OffHandSlot.UnequipItem();
            itemsConfigurations[currentRank].OffHandSlot.UnequipItem();
            itemsConfigurations[rank].OffHandSlot.EquipItem(equipedItem1);
            itemsConfigurations[currentRank].OffHandSlot.EquipItem(equipedItem0);
        }
    }
}
