using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

internal static class HigherLevelFeaturesModalPatcher
{
    [HarmonyPatch(typeof(HigherLevelFeaturesModal), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Bind_Patch
    {
        internal static void Prefix(ref int achievementLevel)
        {
            var hero = Global.ActiveLevelUpHero;

            if (hero == null)
            {
                return;
            }

            //PATCH: filters out features already taken on class display (MULTICLASS)
            var isLevelingUp = LevelUpContext.IsLevelingUp(hero);
            var isClassSelectionStage = LevelUpContext.IsClassSelectionStage(hero);
            var selectedClass = LevelUpContext.GetSelectedClass(hero);

            if (selectedClass != null
                && isLevelingUp
                && isClassSelectionStage
                && hero.ClassesAndLevels.TryGetValue(selectedClass, out var levels))
            {
                achievementLevel = levels + 1;
            }
        }
    }
}
