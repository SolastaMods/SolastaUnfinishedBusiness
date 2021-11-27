using HarmonyLib;

namespace SolastaCommunityExpansion.Patches
{
    internal static class CharacterInspectionScreenPatcher
    {
        [HarmonyPatch(typeof(CharacterInspectionScreen), "Bind")]
        internal static class ContainerPanel_Bind
        {
            internal static void Postfix(CharacterInspectionScreen __instance)
            {
                Models.InventoryManagementContenxt.Reset();
            }
        }

        [HarmonyPatch(typeof(CharacterInspectionScreen), "InventoryDragStopped")]
        internal static class CharacterInspectionScreen_InventoryDragStopped
        {
            internal static void Prefix(CharacterInspectionScreen __instance)
            {
                if (Main.Settings.EnableInventoryFilterAndSort)
                {
                    var inventoryPanel = __instance.InventoryPanel;
                    var containerPanel = AccessTools.Field(inventoryPanel.GetType(), "personalContainerPanel").GetValue(inventoryPanel) as ContainerPanel;

                    containerPanel.InspectedCharacter?.RulesetCharacterHero?.CharacterRefreshed?.Invoke(containerPanel.InspectedCharacter.RulesetCharacterHero);
                }
            }
        }

        [HarmonyPatch(typeof(CharacterInspectionScreen), "OnCloseCb")]
        internal static class CharacterInspectionScreen_OnCloseCb
        {
            internal static void Prefix(CharacterInspectionScreen __instance)
            {
                Models.InventoryManagementContenxt.Reset();
            }
        }
    }
}
