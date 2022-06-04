using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Utils;

namespace SolastaCommunityExpansion.Patches.Translator;

[HarmonyPatch(typeof(GameLocationBanterManager), "PlayLine")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class Gui_Format
{
    internal static void Prefix(ref string line)
    {
        line = DungeonMakerContext.ReplaceVariable(line);

        if (Main.Settings.EnableOnTheFlyTranslations)
        {
            line = Translations.Translate(
                line,
                Main.Settings.SelectedLanguageCode);
        }
    }
}
