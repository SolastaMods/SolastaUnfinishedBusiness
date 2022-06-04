using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Utils;

namespace SolastaCommunityExpansion.Patches.Translator;

[HarmonyPatch(typeof(GuiQuest), "GetStepTitle")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GuiQuest_GetStepTitle
{
    internal static void Postfix(ref string __result)
    {
        __result = DungeonMakerContext.ReplaceVariable(__result);

        if (Main.Settings.EnableOnTheFlyTranslations)
        {
            __result = UserCampaignsTranslatorContext.Translate(
                __result,
                Main.Settings.SelectedLanguageCode);
        }
    }
}

[HarmonyPatch(typeof(GuiQuest), "GetStepDescription")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GuiQuest_GetStepDescription
{
    internal static void Postfix(ref string __result)
    {
        __result = DungeonMakerContext.ReplaceVariable(__result);

        if (Main.Settings.EnableOnTheFlyTranslations)
        {
            __result = UserCampaignsTranslatorContext.Translate(
                __result,
                Main.Settings.SelectedLanguageCode);
        }
    }
}
