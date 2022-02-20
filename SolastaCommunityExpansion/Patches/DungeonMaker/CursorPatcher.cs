using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.DungeonMaker
{
    [HarmonyPatch(typeof(Cursor), "OnClickSecondaryPointer")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Cursor_OnClickSecondaryPointer
    {
        /// <summary>
        /// Note: OnClickSecondaryPointer currently not used.  Need to check usage in future updates.
        /// </summary>
        internal static void Postfix(Cursor __instance)
        {
            if (Main.Settings.EnableCancelEditOnRightMouseClick)
            {
                Main.Log($"{__instance.GetType().Name}: Right-click");

                if (__instance is not CursorCampaignDefault &&
                    __instance is not CursorEditableGraphDefault &&
                    __instance is not CursorLocationBattleDefault &&
                    __instance is not CursorLocationEditorDefault &&
                    __instance is not CursorLocationExplorationDefault)
                {
                    ServiceRepository.GetService<ICursorService>()?.DeactivateCursor();
                }
            }
        }
    }
}
