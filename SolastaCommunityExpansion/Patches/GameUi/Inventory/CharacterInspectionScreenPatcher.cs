using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUi.Inventory
{
    [HarmonyPatch(typeof(CharacterInspectionScreen), "OnBeginHide")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterInspectionScreen_OnBeginHide
    {
        internal static void Prefix()
        {
            Models.InventoryManagementContext.ResetControls();
        }
    }
}
