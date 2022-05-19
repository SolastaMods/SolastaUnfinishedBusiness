using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.GameUi.LevelUp
{
    // replaces the hard coded experience
    [HarmonyPatch(typeof(HigherLevelFeaturesModal), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class HigherLevelFeaturesModal_Bind
    {
        internal static void Prefix(ref int achievementLevel)
        {
            var hero = Global.ActiveLevelUpHero;
            var isLevelingUp = LevelUpContext.IsLevelingUp(hero);
            var selectedClass = LevelUpContext.GetSelectedClass(hero);

            if (isLevelingUp
                && hero.ClassesAndLevels.TryGetValue(selectedClass, out var levels))
            {
                achievementLevel = levels + 1;
            }
        }
    }
}
