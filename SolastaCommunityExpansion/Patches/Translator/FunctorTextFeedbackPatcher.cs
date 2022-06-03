using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Utils;

namespace SolastaCommunityExpansion.Patches.Translator;

[HarmonyPatch(typeof(FunctorTextFeedback), "Execute")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class FunctorTextFeedback_Execute
{
    internal static void Prefix(FunctorParametersDescription functorParameters)
    {
        functorParameters.stringParameter = DungeonMakerContext.ReplaceVariable(functorParameters.stringParameter);

        if (Main.Settings.EnableOnTheFlyTranslations)
        {
            functorParameters.stringParameter = UserCampaignsTranslator.Translate(
                functorParameters.stringParameter,
                Main.Settings.SelectedLanguageCode);
        }
    }
}
