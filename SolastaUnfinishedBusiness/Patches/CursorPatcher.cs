using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CursorPatcher
{
    [HarmonyPatch(typeof(Cursor), nameof(Cursor.OnClickSecondaryPointer))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnClickSecondaryPointer_Patch
    {
        [UsedImplicitly]
        public static void Postfix(Cursor __instance)
        {
            //PATCH: Enable cancel action on right mouse click
            if (!Main.Settings.EnableCancelEditOnRightMouseClick)
            {
                return;
            }

            if (__instance is CursorCampaignDefault or CursorEditableGraphDefault or CursorLocationBattleDefault
                or CursorLocationEditorDefault or CursorLocationExplorationDefault)
            {
                return;
            }

            var screen = Gui.CurrentLocationScreen;

            // Don't use ?? on Unity object
            if (!screen)
            {
                screen = Gui.GuiService.GetScreen<UserLocationEditorScreen>();
            }

            if (!screen || !screen.Visible)
            {
                return;
            }

            // ReSharper disable once InvocationIsSkipped
            Main.Log($"Cancelling {screen.GetType().Name} cursor");
            screen.HandleInput(InputCommands.Id.Cancel);
        }
    }
}
