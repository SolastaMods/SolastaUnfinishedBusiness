using HarmonyLib;

namespace SolastaCommunityExpansion.Patches
{
    // uses this patch to trap the input hotkey and start export process
    [HarmonyPatch(typeof(CharacterInspectionScreen), "HandleInput")]
    internal static class CharacterInspectionScreen_HandleInput
    {
        public static bool Prefix(CharacterInspectionScreen __instance, InputCommands.Id command)
        {
            bool trap = command == Settings.CTRL_E;

            if (trap)
            {
                Models.CharacterExportContext.ExportInspectedCharacter(__instance.InspectedCharacter.RulesetCharacterHero);
            }

            return !trap;
        }
    }
}
