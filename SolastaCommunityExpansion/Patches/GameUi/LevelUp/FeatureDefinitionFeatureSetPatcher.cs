using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.Api.AdditionalExtensions;
using SolastaCommunityExpansion.CustomDefinitions;

namespace SolastaCommunityExpansion.Patches.GameUi.LevelUp
{
    [HarmonyPatch(typeof(FeatureDefinitionFeatureSet), "FormatDescription")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FeatureDefinitionFeatureSet_FormatDescription
    {
        internal static void Postfix(FeatureDefinitionFeatureSet __instance, ref string __result)
        {
            if (!__instance.HasSubFeatureOfType<CustomSetDescription>())
            {
                return;
            }

            if (__instance.Mode != FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            {
                return;
            }

            var description = Gui.Localize(__instance.GuiPresentation.Description);

            var featureSet = __instance.FeatureSet.ToList();
            featureSet.RemoveAll(f => f.GuiPresentation.Hidden);

            if (!featureSet.Empty())
            {
                description += "\n\n" + string.Join("\n\n", featureSet.Select(f =>
                    $"{Gui.Colorize(f.FormatTitle(), Gui.ColorBrightBlue)}\n{f.FormatDescription()}"));
            }

            __result = description;
        }
    }
}
