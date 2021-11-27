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
                try
                {
                    var inventoryPanel = __instance.InventoryPanel;
                    var containerPanel = AccessTools.Field(inventoryPanel.GetType(), "personalContainerPanel").GetValue(inventoryPanel) as ContainerPanel;
                    var filterSortDropdown = containerPanel.transform.parent.Find("FilterDropdown").GetComponent<GuiDropdown>();
                    var guiSortDropdown = containerPanel.transform.parent.Find("SortDropdown").GetComponent<GuiDropdown>();

                    filterSortDropdown.value = 0;
                    guiSortDropdown.value = Main.Settings.InventorySortDropdownValue;
                }
                catch
                {
                    Main.Log("inventory management is disabled.");
                }
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
                try
                {
                    var inventoryPanel = __instance.InventoryPanel;
                    var containerPanel = AccessTools.Field(inventoryPanel.GetType(), "personalContainerPanel").GetValue(inventoryPanel) as ContainerPanel;
                    var filterSortDropdown = containerPanel.transform.parent.Find("FilterDropdown").GetComponent<GuiDropdown>();

                    filterSortDropdown.value = 0;
                }
                catch
                {
                    Main.Log("inventory management is disabled.");
                }
            }
        }
    }
}
