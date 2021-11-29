using HarmonyLib;

namespace SolastaCommunityExpansion.Patches
{
    // uses this patch to trap the input hotkey and start export process
    [HarmonyPatch(typeof(CharacterInspectionScreen), "HandleInput")]
    internal static class CharacterInspectionScreen_HandleInput
    {
        public static void Prefix(CharacterInspectionScreen __instance, InputCommands.Id command, ref bool __result)
        {
            if (Gui.Game != null && Main.Settings.EnableCharacterExport && !Models.CharacterExportContext.InputModalVisible && command == Settings.CTRL_E)
            {
                Models.CharacterExportContext.ExportInspectedCharacter(__instance.InspectedCharacter.RulesetCharacterHero);
            }
        }
    }
}
