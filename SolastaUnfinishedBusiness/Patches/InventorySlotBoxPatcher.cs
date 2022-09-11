using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

internal static class InventorySlotBoxPatcher
{
    //PATCH: Enable inventory taint non proficient items in red (paint them red)
    [HarmonyPatch(typeof(InventorySlotBox), "RefreshState")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RefreshState_Patch
    {
        internal static void Postfix(InventorySlotBox __instance)
        {
            if (Global.InspectedHero == null)
            {
                return;
            }

            if (!Main.Settings.EnableInventoryTaintNonProficientItemsRed || Global.IsMultiplayer)
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

    //PATCH: Enable inventory taint non proficient items in red (paint them back white)
    [HarmonyPatch(typeof(InventorySlotBox), "Unbind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Unbind_Patch
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
}
