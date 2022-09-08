using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.PatchCode.SrdAndHouseRules;

namespace SolastaCommunityExpansion.Patches;

internal static class RulesetAttributePatcher
{
// non stacked AC
    [HarmonyPatch(typeof(RulesetAttribute), "Refresh")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Refresh_Patch
    {
        internal static bool Prefix([NotNull] RulesetAttribute __instance)
        {
            //PATCH: makes AC modifiers not stack, when they are not supposed to
            //completely skips base method for `ArmorClass` attribute
            return ArmorClassStacking.UnstackAC(__instance);
        }
    }
}
