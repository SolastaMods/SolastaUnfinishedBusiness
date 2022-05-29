using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUi.Tooltip
{
    [HarmonyPatch(typeof(RecoveredFeatureItem), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RecoveredFeatureItem_Bind
    {
        internal static void Postfix(RecoveredFeatureItem __instance, RulesetCharacterHero character)
        {
            __instance.GuiTooltip.Context = character;
        }
    }
}
