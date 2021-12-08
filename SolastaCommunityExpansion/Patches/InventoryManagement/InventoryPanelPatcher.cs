using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.InventoryManagement
{
    internal static class InventoryPanelPatcher
    {
        [HarmonyPatch(typeof(InventoryPanel), "Bind")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class InventoryPanel_Bind
        {
            internal static void Postfix(InventoryPanel __instance)
            {
                Models.InventoryManagementContext.Refresh(__instance.MainContainerPanel);
            }
        }

        [HarmonyPatch(typeof(InventoryPanel), "Unbind")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class InventoryPanel_Unbind
        {
            internal static void Prefix(InventoryPanel __instance)
            {
                Models.InventoryManagementContext.Refresh(__instance.MainContainerPanel, clearState: true);
            }

            internal static void Postfix()
            {
                Models.InventoryManagementContext.MarkAsDirty();
            }
        }
    }
}
