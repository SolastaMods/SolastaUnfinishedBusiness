using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.PatchCode.CustomUI;

namespace SolastaCommunityExpansion.Patches;

internal static class RecoveredFeatureItemPatcher
{
    [HarmonyPatch(typeof(RecoveredFeatureItem), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Bind_Patch
    {
        internal static void Postfix(RecoveredFeatureItem __instance, RulesetCharacterHero character)
        {
            //PATCH: adds current character to recovered during rest feature's tooltip context, so it may properly update ts user-dependant stats
            Tooltips.AddContextToRecoveredFeature(__instance, character);
        }
    }
}
