using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class InventorySlotBoxPatcher
{
    [HarmonyPatch(typeof(InventorySlotBox), nameof(InventorySlotBox.RefreshState))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefreshState_Patch
    {
        [UsedImplicitly]
        public static void Postfix(InventorySlotBox __instance)
        {
            //PATCH: Enable inventory taint non proficient items in red (paint them red)
            TintNonProficientItems(__instance);
        }

        private static void TintNonProficientItems(InventorySlotBox box)
        {
            if (Global.InspectedHero == null)
            {
                return;
            }

            if (!Main.Settings.EnableInventoryTaintNonProficientItemsRed)
            {
                return;
            }

            if (box.InventorySlot?.EquipedItem == null || box.equipedItemImage == null)
            {
                return;
            }

            var itemDefinition = box.InventorySlot.EquipedItem.ItemDefinition;

            if (!Global.InspectedHero.IsProficientWithItem(itemDefinition))
            {
                box.equipedItemImage.color = Color.red;
            }
        }
    }


    [HarmonyPatch(typeof(InventorySlotBox), nameof(InventorySlotBox.Unbind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Unbind_Patch
    {
        // this should not have any protection to keep the house clean
        [UsedImplicitly]
        public static void Prefix(InventorySlotBox __instance)
        {
            //PATCH: Enable inventory taint non proficient items in red (paint them back white)
            if (__instance.equipedItemImage == null)
            {
                return;
            }

            __instance.equipedItemImage.color = new Color(1, 1, 1);
        }
    }
}
