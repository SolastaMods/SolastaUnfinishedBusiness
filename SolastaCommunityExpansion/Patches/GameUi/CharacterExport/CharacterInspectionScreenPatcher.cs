using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Utils;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterExport;

// uses this patch to trap the input hotkey and start export process
[HarmonyPatch(typeof(CharacterInspectionScreen), "HandleInput")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterInspectionScreen_HandleInput
{
    public static void Postfix(CharacterInspectionScreen __instance, InputCommands.Id command)
    {
        if (Main.Settings.EnableCharacterExport && command == Hotkeys.CTRL_SHIFT_E && Gui.Game != null &&
            !CharacterExportContext.InputModalVisible)
        {
            CharacterExportContext.ExportInspectedCharacter(__instance.InspectedCharacter.RulesetCharacterHero);
        }
    }
}
