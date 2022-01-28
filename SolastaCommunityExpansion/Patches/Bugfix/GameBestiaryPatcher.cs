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
                    // Either we know nothing (level = 0) or we know enough to sort on
                    // all categories (level > 0) except CR which requires level >= 2.
                    // Move all level = 0 to the end and only sort level > 0
                    __result = leftLevel == 0 ? 1 : (rightLevel == 0 ? -1 : 0);
                    break;
                case BestiaryDefinitions.SortCategory.ChallengeRating:
                    // Need at least level = 2 to know the CR
                    // Move all level < 2 to the end and only sort level >= 2
                    __result = leftLevel < 2 ? 1 : (rightLevel < 2 ? -1 : 0);
                    break;
            }

            return false;
        }
    }
}
