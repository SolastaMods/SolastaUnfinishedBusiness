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

                if (__instance is CursorLocationEditor && __instance is not CursorLocationEditorDefault)
                {
                    ServiceRepository.GetService<ICursorService>()?.DeactivateCursor();
                }
                else if (__instance is CursorLocationGeometricShape)
                {
                    ServiceRepository.GetService<ICursorService>()?.DeactivateCursor();
                }
                else if (__instance is CursorLocationSelectTarget)
                {
                    ServiceRepository.GetService<ICursorService>()?.DeactivateCursor();
                }
                else if (__instance is CursorLocationSelectSpellOrPower)
                {
                    ServiceRepository.GetService<ICursorService>()?.DeactivateCursor();
                }
                else if (__instance is CursorLocationSelectPosition)
                {
                    ServiceRepository.GetService<ICursorService>()?.DeactivateCursor();
                }
            }
        }
    }
}
