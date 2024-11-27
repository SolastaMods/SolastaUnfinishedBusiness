using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GameLocationBanterManagerPatcher
{
    // remove banter for forced off stealth from options on Mod UI
    [HarmonyPatch(typeof(GameLocationBanterManager), nameof(GameLocationBanterManager.StealthMayBeBrokenByAction))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class HandleInput_Patch
    {
        [UsedImplicitly]
        public static bool Prefix()
        {
            return CharacterActionPatcher.ApplyStealthBreakerBehavior_Patch.ShouldBanter;
        }
    }

    //PATCH: supports speech feature
    [HarmonyPatch(typeof(GameLocationBanterManager), nameof(GameLocationBanterManager.ForceBanterLine))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ForceBanterLine_Patch
    {
        [UsedImplicitly]
        public static void Prefix(string line, GameLocationCharacter speaker)
        {
            SpeechContext.Speak(line, speaker);
        }
    }
}
