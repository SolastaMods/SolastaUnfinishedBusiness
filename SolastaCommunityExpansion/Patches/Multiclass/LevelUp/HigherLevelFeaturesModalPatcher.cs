using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Multiclass.LevelUp
{
    // replaces the hard coded experience
    [HarmonyPatch(typeof(HigherLevelFeaturesModal), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class HigherLevelFeaturesModal_Bind
    {
        internal static void Prefix(ref int achievementLevel)
        {
            var hero = Models.Global.ActiveLevelUpHero;
            var selectedClass = Models.LevelUpContext.GetSelectedClass(hero);

            if (hero.ClassesAndLevels.TryGetValue(selectedClass, out var levels))
            {
                achievementLevel = levels + 1;
            }
        }
    }
}
