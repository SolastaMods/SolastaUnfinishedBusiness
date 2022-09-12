using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches;

internal static class NarrativeStateCharacterSpeechPatcher
{
    //PATCH: EnableLogDialoguesToConsole
    [HarmonyPatch(typeof(NarrativeStateCharacterSpeech), "RecordSpeechLine")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RecordSpeechLine_Patch
    {
        internal static void Postfix(string speakerName, string textLine)
        {
            if (!Main.Settings.EnableLogDialoguesToConsole)
            {
                return;
            }

            Gui.Game.GameConsole.LogSimpleLine($"<b><color=yellow>{speakerName}:</color></b> {textLine}");
        }
    }
}
