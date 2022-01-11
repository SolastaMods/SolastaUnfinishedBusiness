using HarmonyLib;
using SolastaModApi.Infrastructure;
using System.Diagnostics.CodeAnalysis;

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
            if (Main.Settings.EnableCancelEditOnRightMouseClick && __instance is CursorLocationEditor && !(__instance is CursorLocationEditorDefault))
            {
                // This is a field on CursorEditor not Cursor so can't be passed in by the patch
                var userLocationEditorScreen = __instance.GetField<UserLocationEditorScreen>("userLocationEditorScreen");

                userLocationEditorScreen?.HandleInput(InputCommands.Id.Cancel);
            }
        }
    }
}
