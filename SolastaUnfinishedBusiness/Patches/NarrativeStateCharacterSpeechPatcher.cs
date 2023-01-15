using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class NarrativeStateCharacterSpeechPatcher
{
    [HarmonyPatch(typeof(NarrativeStateCharacterSpeech), "RecordSpeechLine")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RecordSpeechLine_Patch
    {
        [UsedImplicitly]
        public static void Postfix(string speakerName, string textLine)
        {
            //PATCH: EnableLogDialoguesToConsole
            if (!Main.Settings.EnableLogDialoguesToConsole)
            {
                return;
            }

            GameConsoleHelper.LogCharacterConversationLine(speakerName, textLine, false);
        }
    }
}
