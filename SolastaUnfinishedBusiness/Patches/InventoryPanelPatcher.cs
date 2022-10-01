using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

internal static class InventoryPanelPatcher
{
    //PATCH: Enable Inventory Filtering and Sorting
    [HarmonyPatch(typeof(InventoryPanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class OnBeginShow_Patch
    {
        internal static void Postfix()
        {
            InventoryManagementContext.RefreshControlsVisibility();

            if (Main.Settings.EnableInventoryFilteringAndSorting)
            {
                InventoryManagementContext.SelectionChanged();
            }
        }
    }

    //PATCH: Enable Inventory Filtering and Sorting
    [HarmonyPatch(typeof(InventoryPanel), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Bind_Patch
    {
        internal static void Postfix(InventoryPanel __instance)
        {
            // NOTE: don't use MainContainerPanel?. which bypasses Unity object lifetime check
            if (Main.Settings.EnableInventoryFilteringAndSorting
                && __instance.MainContainerPanel)
            {
                InventoryManagementContext.SortAndFilter(__instance.MainContainerPanel.Container);
            }
        }
    }

    //PATCH: Enable Inventory Filtering and Sorting
    [HarmonyPatch(typeof(InventoryPanel), "Unbind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Unbind_Patch
    {
        internal static void Prefix(InventoryPanel __instance)
        {
            if (__instance.MainContainerPanel)
            {
                // NOTE: don't use MainContainerPanel?. which bypasses Unity object lifetime check
                InventoryManagementContext.Flush(__instance.MainContainerPanel.Container);
            }
        }
    }

    [HarmonyPatch(typeof(InventoryPanel), "RefreshSlotsList")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RefreshSlotsList_Patch
    {
        internal static void Postfix(InventoryPanel __instance)
        {
            //PATCH: support for customized filtering of items for ItemProperty effect form
            CustomItemFilter.FilterItems(__instance);
        }
    }
}
