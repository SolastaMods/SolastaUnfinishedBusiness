using HarmonyLib;

namespace SolastaCommunityExpansion.Patches
{
    internal static class CharacterPlateGameSelectorPatcher
    {
        [HarmonyPatch(typeof(CharacterPlateGameSelector), "OnPointerClick")]
        internal static class CharacterPlateGameSelector_OnPointerClick
        {
            internal static void Prefix()
            {
                try
                {
                    var characterInspectionScreen = Gui.GuiService.GetScreen<CharacterInspectionScreen>();
                    var rightGroup = characterInspectionScreen.transform.FindChildRecursive("RightGroup");
                    var containerPanel = rightGroup.GetComponentInChildren<ContainerPanel>();
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
