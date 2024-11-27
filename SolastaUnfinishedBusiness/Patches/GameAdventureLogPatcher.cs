using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GameAdventureLogPatcher
{
    //PATCH: supports speech feature
    [HarmonyPatch(typeof(GameAdventureLog), nameof(GameAdventureLog.RecordLoreTextEntry))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RecordLoreTextEntry_Patch
    {
        [UsedImplicitly]
        public static void Prefix(string loreText)
        {
            SpeechContext.Speak(loreText, 0);
        }
    }
}
