using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Utils;

namespace SolastaCommunityExpansion.Patches.Translator;

[HarmonyPatch(typeof(FunctorRaiseBanterEvent), "Execute")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class FunctorRaiseBanterEvent_Execute
{
    internal static void Prefix([NotNull] FunctorParametersDescription functorParameters)
    {
        functorParameters.stringParameter =
            DungeonMakerContext.ReplaceVariable(Gui.Localize(functorParameters.stringParameter));

        if (Main.Settings.EnableOnTheFlyTranslations)
        {
            functorParameters.stringParameter = Translations.Translate(
                functorParameters.stringParameter,
                Main.Settings.SelectedLanguageCode);
        }
    }
}
