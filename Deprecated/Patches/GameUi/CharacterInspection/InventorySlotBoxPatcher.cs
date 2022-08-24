using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterInspection;

[HarmonyPatch(typeof(InventorySlotBox), "RefreshState")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class InventorySlotBox_RefreshState
{
    internal static void Postfix(InventorySlotBox __instance)
    {
        if (Global.InspectedHero == null)
        {
            return;
        }

        if (!Main.Settings.EnableInventoryFilteringAndSorting
            || Global.IsMultiplayer
            || !Main.Settings.EnableInventoryTaintNonProficientItemsRed)
        {
            return;
        }

        if (__instance.InventorySlot?.EquipedItem == null || __instance.equipedItemImage == null)
        {
            return;
        }

        var itemDefinition = __instance.InventorySlot.EquipedItem.ItemDefinition;

        if (!Global.InspectedHero.IsProficientWithItem(itemDefinition))
        {
            __instance.equipedItemImage.color = Color.red;
        }
    }
}

[HarmonyPatch(typeof(InventorySlotBox), "Unbind")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class InventorySlotBox_Unbind
{
    // this should not have any protection to keep the house clean
    internal static void Prefix(InventorySlotBox __instance)
    {
        if (__instance.equipedItemImage == null)
        {
            return;
        }

        __instance.equipedItemImage.color = new Color(1, 1, 1);
    }
}
