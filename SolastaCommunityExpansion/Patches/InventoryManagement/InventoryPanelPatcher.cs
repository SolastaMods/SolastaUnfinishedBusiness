using HarmonyLib;

namespace SolastaCommunityExpansion.Patches
{
    internal static class InventoryPanelPatcher
    {
        [HarmonyPatch(typeof(InventoryPanel), "Bind")]
        internal static class InventoryPanel_Bind
        {
            internal static void Postfix(InventoryPanel __instance)
            {
                Models.InventoryManagementContext.Refresh(__instance.MainContainerPanel);
            }
        }

        [HarmonyPatch(typeof(InventoryPanel), "Unbind")]
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
