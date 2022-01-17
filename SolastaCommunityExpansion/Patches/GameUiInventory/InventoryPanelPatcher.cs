using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUiInventory
{
    [HarmonyPatch(typeof(InventoryPanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class InventoryPanel_OnBeginShow
    {
        internal static void Postfix()
        {
            Models.InventoryManagementContext.RefreshControlsVisibility();
        }
    }
}
