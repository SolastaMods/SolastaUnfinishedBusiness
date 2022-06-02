using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.Translator;

[HarmonyPatch(typeof(DialogChoiceItem), "Bind")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class DialogChoiceItem_Bind
{
    internal static void Postfix(DialogChoiceItem __instance, DialogChoiceDescription dialogChoice)
    {
        __instance.labelHighlighter.TargetLabel.Text = 
            DungeonMakerContext.ReplaceVariable(__instance.labelHighlighter.TargetLabel.Text);

        if (Main.Settings.EnableOnTheFlyTranslations)
        {
            __instance.labelHighlighter.TargetLabel.Text = Utils.UserCampaignsTranslator.Translate(
                __instance.labelHighlighter.TargetLabel.Text,
                Main.Settings.SelectedLanguageCode);
        }
    }
}
