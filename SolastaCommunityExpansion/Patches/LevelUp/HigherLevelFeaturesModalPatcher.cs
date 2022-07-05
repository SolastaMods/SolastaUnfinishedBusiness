using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.LevelUp;

[HarmonyPatch(typeof(HigherLevelFeaturesModal), "Bind")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class HigherLevelFeaturesModal_Bind
{
    internal static void Prefix(ref List<FeatureUnlockByLevel> featureUnlocks, ref int achievementLevel)
    {
        var hero = Global.ActiveLevelUpHero;
        var isLevelingUp = LevelUpContext.IsLevelingUp(hero);
        var isClassSelectionStage = LevelUpContext.IsClassSelectionStage(hero);
        var selectedClass = LevelUpContext.GetSelectedClass(hero);

        featureUnlocks = MulticlassPatchingContext.ClassFilteredFeatureUnlocks(selectedClass, hero).ToList();

        if (isLevelingUp
            && isClassSelectionStage
            && hero.ClassesAndLevels.TryGetValue(selectedClass, out var levels))
        {
            achievementLevel = levels + 1;
        }
    }
}
