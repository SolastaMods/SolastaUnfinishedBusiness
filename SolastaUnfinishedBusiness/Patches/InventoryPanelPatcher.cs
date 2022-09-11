using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
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

            if (Main.Settings.EnableInventoryFilteringAndSorting && !Global.IsMultiplayer)
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
                && !Global.IsMultiplayer
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
}
