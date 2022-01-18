using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CharacterExport
{
    // uses this patch to trap the input hotkey and start export process
    [HarmonyPatch(typeof(CharacterInspectionScreen), "HandleInput")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterInspectionScreen_HandleInput
    {
        public static void Postfix(CharacterInspectionScreen __instance, InputCommands.Id command)
        {
            if (Gui.Game != null && Main.Settings.EnableCharacterExport && !CharacterExportContext.InputModalVisible && command == Hotkeys.CTRL_E)
            {
                CharacterExportContext.ExportInspectedCharacter(__instance.InspectedCharacter.RulesetCharacterHero);
            }
        }
    }
}
