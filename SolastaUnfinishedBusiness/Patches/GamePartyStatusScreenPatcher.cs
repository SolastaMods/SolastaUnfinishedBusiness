using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GamePartyStatusScreenPatcher
{
    [HarmonyPatch(typeof(GamePartyStatusScreen), nameof(GamePartyStatusScreen.StartAllModifiers))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class StartAllModifiers_Patch
    {
        [UsedImplicitly]
        public static void Prefix(bool forward)
        {
            if (Main.Settings.EnableSpeech && !forward)
            {
                SpeechContext.WaveOutEvent.Stop();
            }
        }
    }
}
