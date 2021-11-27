using HarmonyLib;

namespace SolastaCommunityExpansion.Patches
{
    internal static class ContainerPanelPatcher
    {
        [HarmonyPatch(typeof(ContainerPanel), "OnReorderCb")]
        internal static class ContainerPanel_OnReorderCb
        {
            internal static void Prefix()
            {
                Models.InventoryManagementContenxt.Reset(true);
            }
        }
    }
}
