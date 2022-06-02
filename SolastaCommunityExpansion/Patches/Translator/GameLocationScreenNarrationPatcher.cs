using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.Translator;

[HarmonyPatch(typeof(GameLocationScreenNarration), "SpeechStarted")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GameLocationScreenNarration_RefreshControlPanel
{
    internal static void Prefix(ref string speechText)
    {
        speechText = DungeonMakerContext.ReplaceVariable(speechText);

        if (Main.Settings.EnableOnTheFlyTranslations)
        {
            speechText = Utils.UserCampaignsTranslator.Translate(
                speechText,
                Main.Settings.SelectedLanguageCode);
        }
    }
}
