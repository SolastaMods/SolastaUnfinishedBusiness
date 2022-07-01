using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUi.Location;

[HarmonyPatch(typeof(Cursor), "OnClickSecondaryPointer")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class Cursor_OnClickSecondaryPointer
{
    /// <summary>
    ///     Note: OnClickSecondaryPointer currently not used.  Need to check usage in future updates.
    /// </summary>
    internal static void Postfix(Cursor __instance)
    {
        if (!Main.Settings.EnableCancelEditOnRightMouseClick)
        {
            return;
        }

        Main.Log($"{__instance.GetType().Name}: Right-click");

        if (__instance is CursorCampaignDefault || __instance is CursorEditableGraphDefault ||
            __instance is CursorLocationBattleDefault || __instance is CursorLocationEditorDefault ||
            __instance is CursorLocationExplorationDefault)
        {
            return;
        }

        var screen = Gui.CurrentLocationScreen;

        // Don't use ?? on Unity objec
        if (screen == null)
        {
            screen = Gui.GuiService.GetScreen<UserLocationEditorScreen>();
        }

        if (screen == null || !screen.Visible)
        {
            return;
        }

        Main.Log($"Cancelling {screen.GetType().Name} cursor");
        screen.HandleInput(InputCommands.Id.Cancel);
    }
}
