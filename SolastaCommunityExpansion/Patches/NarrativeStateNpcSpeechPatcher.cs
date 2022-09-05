using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches;

//PATCH: EnableLogDialoguesToConsole
[HarmonyPatch(typeof(NarrativeStateNpcSpeech), "RecordSpeechLine")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class NarrativeStateNpcSpeech_RecordSpeechLine_Getter
{
    internal static void Postfix(string speakerName, string textLine)
    {
        if (!Main.Settings.EnableLogDialoguesToConsole)
        {
            return;
        }

        Gui.Game.GameConsole.LogSimpleLine($"<b><color=white>{speakerName}:</color></b> {textLine}");
    }
}
