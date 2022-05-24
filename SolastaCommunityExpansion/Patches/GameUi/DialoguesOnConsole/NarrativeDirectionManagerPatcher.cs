using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using I2.Loc;

namespace SolastaCommunityExpansion.Patches.GameUi.DialoguesOnConsole
{
    [HarmonyPatch(typeof(NarrativeDirectionManager), "StartDialogSequence")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class NarrativeDirectionManager_StartDialogSequence
    {
        internal static void Postfix()
        {
            if (!Main.Settings.EnableLogDialoguesToConsole || LocalizationManager.CurrentLanguageCode != "en")
            {
                return;
            }

            var screen = Gui.GuiService.GetScreen<GuiConsoleScreen>();

            screen.Show();
        }
    }
}
