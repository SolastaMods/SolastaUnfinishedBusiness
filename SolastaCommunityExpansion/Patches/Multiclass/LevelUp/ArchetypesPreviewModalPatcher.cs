using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using static SolastaCommunityExpansion.Models.Level20Context;

namespace SolastaCommunityExpansion.Patches.Multiclass.LevelUp
{
    // filter features already taken on subclass display
    [HarmonyPatch(typeof(ArchetypesPreviewModal), "Refresh")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ArchetypesPreviewModal_Refresh
    {
        public static int Level(FeatureUnlockByLevel featureUnlockByLevel)
        {
            var hero = Global.ActiveLevelUpHero;
            var isLevelingUp = LevelUpContext.IsLevelingUp(hero);
            var selectedClass = LevelUpContext.GetSelectedClass(hero);

            if (isLevelingUp && hero.ClassesAndLevels.TryGetValue(selectedClass, out var levels)
                && featureUnlockByLevel.Level <= levels + 1)
            {
                return int.MaxValue;
            }

            return featureUnlockByLevel.Level;
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
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
            var hero = Models.Global.ActiveLevelUpHero;
            var selectedClass = Models.LevelUpContext.GetSelectedClass(hero);

            if (hero.ClassesAndSubclasses.TryGetValue(selectedClass, out var characterSubclassDefinition))
            {
                subclasses = new() { characterSubclassDefinition.Name };
            }
        }
    }
}
