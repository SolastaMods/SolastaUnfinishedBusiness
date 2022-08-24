using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterInspection;

[HarmonyPatch(typeof(InventoryShortcutsPanel), "OnConfigurationSwitched")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class InventoryShortcutsPanel_OnConfigurationSwitched
{
    private const int LIGHT_SOURCE = 2;

    internal static void Prefix(InventoryShortcutsPanel __instance, int rank)
    {
        var isCtrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        if (!Main.Settings.EnableCtrlClickOnlySwapsMainHand || !isCtrlPressed)
        {
            return;
        }

        var characterInventory = __instance.GuiCharacter.RulesetCharacterHero.CharacterInventory;
        var itemsConfigurations = characterInventory.WieldedItemsConfigurations;
        var currentRank = characterInventory.CurrentConfiguration;

        if (rank == LIGHT_SOURCE || currentRank == LIGHT_SOURCE)
        {
            return;
        }

        if (itemsConfigurations[rank].OffHandSlot.Disabled || itemsConfigurations[currentRank].OffHandSlot.Disabled)
        {
            return;
        }

        var equippedItem0 = itemsConfigurations[rank].OffHandSlot.EquipedItem;
        var equippedItem1 = itemsConfigurations[currentRank].OffHandSlot.EquipedItem;

        itemsConfigurations[rank].OffHandSlot.UnequipItem();
        itemsConfigurations[currentRank].OffHandSlot.UnequipItem();
        itemsConfigurations[rank].OffHandSlot.EquipItem(equippedItem1);
        itemsConfigurations[currentRank].OffHandSlot.EquipItem(equippedItem0);
    }
}
