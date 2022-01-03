using HarmonyLib;
using SolastaModApi.Infrastructure;
using System.Diagnostics.CodeAnalysis;

namespace SolastaCommunityExpansion.Patches.DungeonMaker
{
    [HarmonyPatch(typeof(Cursor), "OnClickSecondaryPointer")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Cursor_OnClickSecondaryPointer
    {
        internal static void Postfix(Cursor __instance)
        {
            if (Main.Settings.EnableCancelCursorDragOnRightMouseClick && __instance is CursorEditor)
            {
                var userLocationEditorScreen = __instance.GetField<UserLocationEditorScreen>("userLocationEditorScreen");

                userLocationEditorScreen?.HandleInput(InputCommands.Id.Cancel);
            }
        }
    }
}
