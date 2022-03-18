using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Multiclass.Models;

namespace SolastaCommunityExpansion.Multiclass.Patches.LevelUp
{
    internal static class CharacterStageLevelGainsPanelPatcher
    {
        // filter active features
        [HarmonyPatch(typeof(CharacterStageLevelGainsPanel), "OnHigherLevelClassCb")]
        internal static class CharacterStageLevelGainsPanelOnHigherLevelCb
        {
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    foreach (var instruction in instructions)
                    {
                        yield return instruction;
                    }

                    yield break;
                }

                var classFeatureUnlocksMethod = typeof(CharacterClassDefinition).GetMethod("get_FeatureUnlocks");
                var classFilteredFeatureUnlocksMethod = typeof(LevelUpContext).GetMethod("ClassFilteredFeatureUnlocks");

                var subclassFeatureUnlocksMethod = typeof(CharacterSubclassDefinition).GetMethod("get_FeatureUnlocks");
                var subclassFilteredFeatureUnlocksMethod = typeof(LevelUpContext).GetMethod("SubclassFilteredFeatureUnlocks");

                var currentHeroField = typeof(CharacterStageLevelGainsPanel).GetField("currentHero", BindingFlags.Instance | BindingFlags.NonPublic);

                foreach (var instruction in instructions)
                {
                    if (instruction.Calls(classFeatureUnlocksMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, currentHeroField);
                        yield return new CodeInstruction(OpCodes.Call, classFilteredFeatureUnlocksMethod);
                    }
                    else if (instruction.Calls(subclassFeatureUnlocksMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, currentHeroField);
                        yield return new CodeInstruction(OpCodes.Call, subclassFilteredFeatureUnlocksMethod);
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }
        }

        // patches the method to get my own class and level for level up
        [HarmonyPatch(typeof(CharacterStageLevelGainsPanel), "EnterStage")]
        internal static class CharacterStageLevelGainsPanelEnterStage
        {
            public static void GetLastAssignedClassAndLevel(ICharacterBuildingService _, RulesetCharacterHero hero, out CharacterClassDefinition lastClassDefinition, out int level)
            {
                if (LevelUpContext.IsLevelingUp(hero))
                {
                    LevelUpContext.GrantItemsIfRequired(hero);
                    LevelUpContext.SetIsClassSelectionStage(hero, false);
                    lastClassDefinition = LevelUpContext.GetSelectedClass(hero);
                    level = hero.ClassesHistory.Count;
                }
                else
                {
                    lastClassDefinition = null;
                    level = 0;

                    if (hero.ClassesHistory.Count > 0)
                    {
                        lastClassDefinition = hero.ClassesHistory[hero.ClassesHistory.Count - 1];
                        level = hero.ClassesAndLevels[lastClassDefinition];
                    }
                }
            }

            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    foreach (var instruction in instructions)
                    {
                        yield return instruction;
                    }

                    yield break;
                }

                var getLastAssignedClassAndLevelMethod = typeof(ICharacterBuildingService).GetMethod("GetLastAssignedClassAndLevel");
                var customGetLastAssignedClassAndLevelMethod = typeof(CharacterStageLevelGainsPanelEnterStage).GetMethod("GetLastAssignedClassAndLevel");

                foreach (var instruction in instructions)
                {
                    if (instruction.Calls(getLastAssignedClassAndLevelMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Call, customGetLastAssignedClassAndLevelMethod);
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }
        }

        // filter active features
        [HarmonyPatch(typeof(CharacterStageLevelGainsPanel), "EnumerateActiveClassFeatures")]
        internal static class CharacterStageLevelGainsPanelEnumerateActiveClassFeatures
        {
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    foreach (var instruction in instructions)
                    {
                        yield return instruction;
                    }

                    yield break;
                }

                var classFeatureUnlocksMethod = typeof(CharacterClassDefinition).GetMethod("get_FeatureUnlocks");
                var classFilteredFeatureUnlocksMethod = typeof(LevelUpContext).GetMethod("ClassFilteredFeatureUnlocks");

                var subclassFeatureUnlocksMethod = typeof(CharacterSubclassDefinition).GetMethod("get_FeatureUnlocks");
                var subclassFilteredFeatureUnlocksMethod = typeof(LevelUpContext).GetMethod("SubclassFilteredFeatureUnlocks");

                var currentHeroField = typeof(CharacterStageLevelGainsPanel).GetField("currentHero", BindingFlags.Instance | BindingFlags.NonPublic);

                foreach (var instruction in instructions)
                {
                    if (instruction.Calls(classFeatureUnlocksMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, currentHeroField);
                        yield return new CodeInstruction(OpCodes.Call, classFilteredFeatureUnlocksMethod);
                    }
                    else if (instruction.Calls(subclassFeatureUnlocksMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, currentHeroField);
                        yield return new CodeInstruction(OpCodes.Call, subclassFilteredFeatureUnlocksMethod);
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }
        }

        // filter active features
        [HarmonyPatch(typeof(CharacterStageLevelGainsPanel), "FillUnlockedClassFeatures")]
        internal static class CharacterStageLevelGainsPanelFillUnlockedClassFeatures
        {
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    foreach (var instruction in instructions)
                    {
                        yield return instruction;
                    }

                    yield break;
                }

                var classFeatureUnlocksMethod = typeof(CharacterClassDefinition).GetMethod("get_FeatureUnlocks");
                var classFilteredFeatureUnlocksMethod = typeof(LevelUpContext).GetMethod("ClassFilteredFeatureUnlocks");

                var subclassFeatureUnlocksMethod = typeof(CharacterSubclassDefinition).GetMethod("get_FeatureUnlocks");
                var subclassFilteredFeatureUnlocksMethod = typeof(LevelUpContext).GetMethod("SubclassFilteredFeatureUnlocks");

                var currentHeroField = typeof(CharacterStageLevelGainsPanel).GetField("currentHero", BindingFlags.Instance | BindingFlags.NonPublic);

                foreach (var instruction in instructions)
                {
                    if (instruction.Calls(classFeatureUnlocksMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, currentHeroField);
                        yield return new CodeInstruction(OpCodes.Call, classFilteredFeatureUnlocksMethod);
                    }
                    else if (instruction.Calls(subclassFeatureUnlocksMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, currentHeroField);
                        yield return new CodeInstruction(OpCodes.Call, subclassFilteredFeatureUnlocksMethod);
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }
        }

        // filter active features
        [HarmonyPatch(typeof(CharacterStageLevelGainsPanel), "Refresh")]
        internal static class CharacterStageLevelGainsPanelRefresh
        {
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    foreach (var instruction in instructions)
                    {
                        yield return instruction;
                    }

                    yield break;
                }

                var classFeatureUnlocksMethod = typeof(CharacterClassDefinition).GetMethod("get_FeatureUnlocks");
                var classFilteredFeatureUnlocksMethod = typeof(LevelUpContext).GetMethod("ClassFilteredFeatureUnlocks");

                var subclassFeatureUnlocksMethod = typeof(CharacterSubclassDefinition).GetMethod("get_FeatureUnlocks");
                var subclassFilteredFeatureUnlocksMethod = typeof(LevelUpContext).GetMethod("SubclassFilteredFeatureUnlocks");

                var currentHeroField = typeof(CharacterStageLevelGainsPanel).GetField("currentHero", BindingFlags.Instance | BindingFlags.NonPublic);

                foreach (var instruction in instructions)
                {
                    if (instruction.Calls(classFeatureUnlocksMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, currentHeroField);
                        yield return new CodeInstruction(OpCodes.Call, classFilteredFeatureUnlocksMethod);
                    }
                    else if (instruction.Calls(subclassFeatureUnlocksMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, currentHeroField);
                        yield return new CodeInstruction(OpCodes.Call, subclassFilteredFeatureUnlocksMethod);
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }
        }

        // only displays spell casting features from the current class
        [HarmonyPatch(typeof(CharacterStageLevelGainsPanel), "RefreshSpellcastingFeatures")]
        internal static class CharacterStageLevelGainsPanelRefreshSpellcastingFeatures
        {
            public static List<RulesetSpellRepertoire> SpellRepertoires(RulesetCharacterHero rulesetCharacterHero)
            {
                if (LevelUpContext.IsLevelingUp(rulesetCharacterHero) && LevelUpContext.IsMulticlass(rulesetCharacterHero))
                {
                    var result = new List<RulesetSpellRepertoire>();

                    result.AddRange(rulesetCharacterHero.SpellRepertoires.Where(x => CacheSpellsContext.IsRepertoireFromSelectedClassSubclass(rulesetCharacterHero, x)));

                    return result;
                }

                return rulesetCharacterHero.SpellRepertoires;
            }

            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    foreach (var instruction in instructions)
                    {
                        yield return instruction;
                    }

                    yield break;
                }

                var spellRepertoiresMethod = typeof(RulesetCharacter).GetMethod("get_SpellRepertoires");
                var filteredSpellRepertoiresMethod = typeof(CharacterStageLevelGainsPanelRefreshSpellcastingFeatures).GetMethod("SpellRepertoires");

                foreach (var instruction in instructions)
                {
                    if (instruction.Calls(spellRepertoiresMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Call, filteredSpellRepertoiresMethod);
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }
        }
    }
}
