using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Utils;

namespace SolastaCommunityExpansion.Patches.Translator;

[HarmonyPatch(typeof(NarrativeStateCharacterSpeech), "RecordSpeechLine")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class NarrativeStateCharacterSpeech_RecordSpeechLine_Getter
{
    internal static void Prefix(ref string textLine)
    {
        textLine = DungeonMakerContext.ReplaceVariable(textLine);

        if (Main.Settings.EnableOnTheFlyTranslations)
        {
            textLine = UserCampaignsTranslator.Translate(
                textLine,
                Main.Settings.SelectedLanguageCode);
        }
    }
}
