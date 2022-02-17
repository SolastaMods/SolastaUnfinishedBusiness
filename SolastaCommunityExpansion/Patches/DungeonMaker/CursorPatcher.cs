using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaModApi.Infrastructure;

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

                if (__instance is CursorLocationEditor && __instance is not CursorLocationEditorDefault)
                {
                    // This is a field on CursorEditor not Cursor so can't be passed in by the patch
                    var userLocationEditorScreen = __instance.GetField<UserLocationEditorScreen>("userLocationEditorScreen");

                    // NOTE: don't use userLocationEditorScreen?. which bypasses Unity object lifetime check
                    if (userLocationEditorScreen)
                    {
                        userLocationEditorScreen.HandleInput(InputCommands.Id.Cancel);
                    }
                }
                else if(__instance is CursorLocationGeometricShape)
                {
                    // This works suspiciously well - need someone to check
                    ServiceRepository.GetService<ICursorService>()?.DeactivateCursor();
                }
            }
        }
    }
}
