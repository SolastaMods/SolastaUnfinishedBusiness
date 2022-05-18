using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterInspection
{
    [HarmonyPatch(typeof(InventorySlotBox), "RefreshState")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class InventorySlotBox_RefreshState
    {
        internal static void Postfix(InventorySlotBox __instance, Image ___equipedItemImage)
        {
            if (Global.IsMultiplayer || Global.InspectedHero == null)
            {
                return;
            }

            if (!Main.Settings.EnableInventoryFilteringAndSorting
                || !Main.Settings.EnableInventoryTaintNonProficientItemsRed)
            {
                return;
            }

            if (__instance.InventorySlot == null
                || __instance.InventorySlot.EquipedItem == null
                || ___equipedItemImage == null)
            {
                return;
            }

            var itemDefinition = __instance.InventorySlot.EquipedItem.ItemDefinition;

            if (!Global.InspectedHero.IsProficientWithItem(itemDefinition))
            {
                ___equipedItemImage.color = new UnityEngine.Color(1, 0, 0);
            }
        }
    }

    [HarmonyPatch(typeof(InventorySlotBox), "Unbind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class InventorySlotBox_Unbind
    {
        // this should not have any protection to keep the house clean
        internal static void Prefix(Image ___equipedItemImage)
        {
            if (___equipedItemImage == null)
            {
                return;
            }

            ___equipedItemImage.color = new UnityEngine.Color(1, 1, 1);
        }
    }
}
