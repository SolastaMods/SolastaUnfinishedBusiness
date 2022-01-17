using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUiInventory
{
    [HarmonyPatch(typeof(ContainerPanel), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ContainerPanel_Bind
    {
        internal static void Prefix(ContainerPanel __instance, RulesetContainer container)
        {
            if (Main.Settings.EnableInventoryFilteringAndSorting && __instance.name.StartsWith("Personal"))
            {
                Models.InventoryManagementContext.SortAndFilter(__instance, container);
            }
        }
    }

    [HarmonyPatch(typeof(ContainerPanel), "Unbind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ContainerPanel_Unbind
    {
        internal static void Prefix(ContainerPanel __instance)
        {
            if (__instance.name.StartsWith("Personal"))
            {
                Models.InventoryManagementContext.Flush(__instance);
            }
        }
    }
}
