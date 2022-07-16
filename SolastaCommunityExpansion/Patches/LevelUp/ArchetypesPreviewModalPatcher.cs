using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.LevelUp;

// filter features already taken on subclass display
[HarmonyPatch(typeof(ArchetypesPreviewModal), "Refresh")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class ArchetypesPreviewModal_Refresh
{
    public static int Level([NotNull] FeatureUnlockByLevel featureUnlockByLevel)
    {
        var hero = Global.ActiveLevelUpHero;

        if (hero == null)
        {
            return featureUnlockByLevel.Level;
        }

        var isLevelingUp = LevelUpContext.IsLevelingUp(hero);
        var selectedClass = LevelUpContext.GetSelectedClass(hero);

        if (selectedClass != null
            && isLevelingUp
            && hero.ClassesAndLevels.TryGetValue(selectedClass, out var levels)
            && featureUnlockByLevel.Level <= levels + 1)
        {
            return int.MaxValue;
        }

        return featureUnlockByLevel.Level;
    }

    [NotNull]
    internal static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
    {
        var levelMethod = typeof(FeatureUnlockByLevel).GetMethod("get_Level");
        var myLevelMethod = typeof(ArchetypesPreviewModal_Refresh).GetMethod("Level");

        foreach (var instruction in instructions)
        {
            if (instruction.Calls(levelMethod))
            {
                yield return new CodeInstruction(OpCodes.Call, myLevelMethod);
            }
            else
            {
                yield return instruction;
            }
        }
    }
}

// only presents the subclass already taken
[HarmonyPatch(typeof(ArchetypesPreviewModal), "Show")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class ArchetypesPreviewModal_Show
{
    internal static void Prefix(ref List<string> subclasses)
    {
        var hero = Global.ActiveLevelUpHero;

        if (hero == null)
        {
            return;
        }

        var selectedClass = LevelUpContext.GetSelectedClass(hero);

        if (selectedClass != null
            && hero.ClassesAndSubclasses.TryGetValue(selectedClass, out var characterSubclassDefinition))
        {
            subclasses = new List<string> {characterSubclassDefinition.Name};
        }
    }
}
