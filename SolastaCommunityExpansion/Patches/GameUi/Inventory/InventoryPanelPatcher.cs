using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUi.Inventory
{
    [HarmonyPatch(typeof(InventoryPanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class InventoryPanel_OnBeginShow
    {
        internal static void Postfix()
        {
            Models.InventoryManagementContext.RefreshControlsVisibility();
        }
    }

    [HarmonyPatch(typeof(InventoryPanel), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ContainerPanel_Bind
    {
        internal static void Postfix(InventoryPanel __instance)
        {
            if (Main.Settings.EnableInventoryFilteringAndSorting)
            {
                Models.InventoryManagementContext.SortAndFilter(__instance.MainContainerPanel?.Container);
            }
        }
    }

    [HarmonyPatch(typeof(InventoryPanel), "Unbind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ContainerPanel_Unbind
    {
        internal static void Prefix(InventoryPanel __instance)
        {
            Models.InventoryManagementContext.Flush(__instance.MainContainerPanel?.Container);
        }
    }
}
