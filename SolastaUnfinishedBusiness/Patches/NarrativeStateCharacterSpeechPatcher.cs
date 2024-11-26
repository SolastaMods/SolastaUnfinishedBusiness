using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class NarrativeStateCharacterSpeechPatcher
{
    [HarmonyPatch(typeof(NarrativeStateCharacterSpeech), nameof(NarrativeStateCharacterSpeech.RecordSpeechLine))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RecordSpeechLine_Patch
    {
        [UsedImplicitly]
        public static void Postfix(NarrativeStateCharacterSpeech __instance, string speakerName, string textLine)
        {
            //PATCH: EnableLogDialoguesToConsole
            if (!Main.Settings.EnableLogDialoguesToConsole)
            {
                return;
            }

            if (Main.Settings.EnableSpeech)
            {
                var heroId = Gui.Game.GameCampaign.Party.CharactersList.FindIndex(x => x.Name == speakerName) + 1;
                SpeechContext.Speak(textLine, heroId);
            }

            GameConsoleHelper.LogCharacterConversationLine(speakerName, textLine, false);
        }
    }
}
