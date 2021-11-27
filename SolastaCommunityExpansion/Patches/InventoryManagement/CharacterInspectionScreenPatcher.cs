using HarmonyLib;

namespace SolastaCommunityExpansion.Patches
{
    internal static class CharacterInspectionScreenPatcher
    {
        [HarmonyPatch(typeof(CharacterInspectionScreen), "Bind")]
        internal static class ContainerPanel_Bind
        {
            internal static void Postfix()
            {
                Models.InventoryManagementContenxt.ResetDropdowns(filterDropdown: true, sortDropdown: false);
            }
        }

        [HarmonyPatch(typeof(CharacterInspectionScreen), "OnCloseCb")]
        internal static class CharacterInspectionScreen_OnCloseCb
        {
            internal static void Prefix()
            {
                Models.InventoryManagementContenxt.ResetDropdowns(filterDropdown: true, sortDropdown: false);
            }
        }
    }
}
