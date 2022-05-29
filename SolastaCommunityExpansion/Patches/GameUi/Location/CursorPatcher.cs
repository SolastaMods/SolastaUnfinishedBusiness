using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUi.Location
{
    [HarmonyPatch(typeof(Cursor), "OnClickSecondaryPointer")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Cursor_OnClickSecondaryPointer
    {
        /// <summary>
        ///     Note: OnClickSecondaryPointer currently not used.  Need to check usage in future updates.
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
                    var screen = Gui.CurrentLocationScreen;

                    // Don't use ?? on Unity objec
                    if (screen == null)
                    {
                        screen = Gui.GuiService.GetScreen<UserLocationEditorScreen>();
                    }

                    if (screen != null && screen.Visible)
                    {
                        Main.Log($"Cancelling {screen.GetType().Name} cursor");
                        screen.HandleInput(InputCommands.Id.Cancel);
                    }
                }
            }
        }
    }
}
