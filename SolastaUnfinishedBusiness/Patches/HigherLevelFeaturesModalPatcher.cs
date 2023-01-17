using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class HigherLevelFeaturesModalPatcher
{
    [HarmonyPatch(typeof(HigherLevelFeaturesModal), nameof(HigherLevelFeaturesModal.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [UsedImplicitly]
        public static void Prefix(ref int achievementLevel)
        {
            var hero = Global.LevelUpHero;

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
