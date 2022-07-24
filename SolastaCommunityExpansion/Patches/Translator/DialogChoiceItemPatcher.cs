using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Utils;

namespace SolastaCommunityExpansion.Patches.Translator;

[HarmonyPatch(typeof(DialogChoiceItem), "Bind")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class DialogChoiceItem_Bind
{
    internal static void Postfix([NotNull] DialogChoiceItem __instance, DialogChoiceDescription dialogChoice)
    {
        __instance.labelHighlighter.TargetLabel.Text =
            DungeonMakerContext.ReplaceVariable(Gui.Localize(__instance.labelHighlighter.TargetLabel.Text));

        if (Main.Settings.EnableOnTheFlyTranslations)
        {
            __instance.labelHighlighter.TargetLabel.Text = Translations.Translate(
                __instance.labelHighlighter.TargetLabel.Text,
                Main.Settings.SelectedLanguageCode);
        }
    }
}
