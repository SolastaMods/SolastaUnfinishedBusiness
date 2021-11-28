using HarmonyLib;

namespace SolastaCommunityExpansion.Patches
{
    internal static class ContainerPanelPatcher
    {
        [HarmonyPatch(typeof(ContainerPanel), "OnReorderCb")]
        internal static class ContainerPanel_OnReorderCb
        {
            internal static bool Prefix(ContainerPanel __instance)
            {
                if (Main.Settings.EnableInventoryFilterAndSort)
                {
                    Models.InventoryManagementContext.Reset(__instance);
                }

                return !Main.Settings.EnableInventoryFilterAndSort;
            }
        }
    }
}
