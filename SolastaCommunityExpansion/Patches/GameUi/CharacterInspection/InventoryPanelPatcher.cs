using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterInspection;

[HarmonyPatch(typeof(InventoryPanel), "OnBeginShow")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class InventoryPanel_OnBeginShow
{
    internal static void Postfix()
    {
        if (Global.IsMultiplayer)
        {
            return;
        }

        InventoryManagementContext.RefreshControlsVisibility();

        if (Main.Settings.EnableInventoryFilteringAndSorting)
        {
            InventoryManagementContext.SelectionChanged();
        }
    }
}

[HarmonyPatch(typeof(InventoryPanel), "Bind")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class ContainerPanel_Bind
{
    internal static void Postfix(InventoryPanel __instance)
    {
        if (Global.IsMultiplayer)
        {
            return;
        }

        // NOTE: don't use MainContainerPanel?. which bypasses Unity object lifetime check
        if (Main.Settings.EnableInventoryFilteringAndSorting && __instance.MainContainerPanel)
        {
            InventoryManagementContext.SortAndFilter(__instance.MainContainerPanel.Container);
        }
    }
}

[HarmonyPatch(typeof(InventoryPanel), "Unbind")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class ContainerPanel_Unbind
{
    internal static void Prefix(InventoryPanel __instance)
    {
        if (Global.IsMultiplayer)
        {
            return;
        }

        if (__instance.MainContainerPanel)
        {
            // NOTE: don't use MainContainerPanel?. which bypasses Unity object lifetime check
            InventoryManagementContext.Flush(__instance.MainContainerPanel.Container);
        }
    }
}
