using HarmonyLib;

namespace SolastaCommunityExpansion.Patches
{
    internal static class CharacterInspectionScreenPatcher
    {
        [HarmonyPatch(typeof(CharacterInspectionScreen), "OnCloseCb")]
        internal static class CharacterInspectionScreen_OnCloseCb
        {
            internal static void Postfix()
            {
                Models.InventoryManagementContext.MarkAsDirty();
            }
        }
    }
}
