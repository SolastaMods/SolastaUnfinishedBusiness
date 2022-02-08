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

        internal static void Postfix(InventoryShortcutsPanel __instance, int rank)
        {
            var isCtrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            var characterInventory = __instance.GuiCharacter.RulesetCharacterHero.CharacterInventory;

            if (Main.Settings.EnableCtrlClickOnlySwapsMainHand && isCtrlPressed 
                && rank < LIGHT_SOURCE && characterInventory.CurrentConfiguration < LIGHT_SOURCE)
            {
                var itemsConfigurations = characterInventory.WieldedItemsConfigurations;

                if (itemsConfigurations.Count < 2 || itemsConfigurations[0].OffHandSlot.Disabled || itemsConfigurations[1].OffHandSlot.Disabled)
                {
                    return;
                }

                var equipedItem0 = itemsConfigurations[0].OffHandSlot.EquipedItem;
                var equipedItem1 = itemsConfigurations[1].OffHandSlot.EquipedItem;

                itemsConfigurations[0].OffHandSlot.UnequipItem();
                itemsConfigurations[1].OffHandSlot.UnequipItem();
                itemsConfigurations[0].OffHandSlot.EquipItem(equipedItem1);
                itemsConfigurations[1].OffHandSlot.EquipItem(equipedItem0);

                characterInventory.RefreshWieldedItemsConfigurations();
            }
        }
    }
}
