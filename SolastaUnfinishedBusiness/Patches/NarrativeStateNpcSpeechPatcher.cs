using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class NarrativeStateNpcSpeechPatcher
{
    [HarmonyPatch(typeof(NarrativeStateNpcSpeech), nameof(NarrativeStateNpcSpeech.RecordSpeechLine))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RecordSpeechLine_Patch
    {
        [UsedImplicitly]
        public static void Postfix(NarrativeStateNpcSpeech __instance, string speakerName, string textLine)
        {
            //PATCH: supports speech feature
            SpeechContext.SpeakNpc(textLine, __instance.speaker);

            //PATCH: EnableLogDialoguesToConsole
            if (Main.Settings.EnableLogDialoguesToConsole)
            {
                GameConsoleHelper.LogCharacterConversationLine(speakerName, textLine, true);
            }
        }
    }
}
