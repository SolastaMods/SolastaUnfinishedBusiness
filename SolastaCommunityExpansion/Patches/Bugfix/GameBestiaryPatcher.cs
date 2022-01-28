using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Bugfix
{
    [HarmonyPatch(typeof(GameBestiary), "SplitByUnknownVsKnown")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameBestiary_SplitByUnknownVsKnown
    {
        public static bool Prefix(int leftLevel, int rightLevel, BestiaryDefinitions.SortCategory ___sortCategory, ref int __result)
        {
            if (!Main.Settings.BugFixBestiarySorting)
            {
                return true;
            }

            switch (___sortCategory)
            {
                default:
                    // All categories are known if level > 0 except CR.
                    // Move all level = 0 to the end and only sort level > 0
                    __result = leftLevel == 0 ? 1 : (rightLevel == 0 ? -1 : 0);
                    break;
                case BestiaryDefinitions.SortCategory.ChallengeRating:
                    // Need at least level = 1 to know the CR
                    // Move all level < 1 to the end and only sort level >= 1
                    __result = leftLevel < 1 ? 1 : (rightLevel >= 1 ? -1 : 0);
                    break;
            }

            return false;
        }
    }
}
