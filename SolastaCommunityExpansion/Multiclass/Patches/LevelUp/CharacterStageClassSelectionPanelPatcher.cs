using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Multiclass.Models;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.Multiclass.Patches.LevelUp
{
    internal static class CharacterStageClassSelectionPanelPatcher
    {
        // flag displaying the class panel / apply in/out logic
        [HarmonyPatch(typeof(CharacterStageClassSelectionPanel), "OnBeginShow")]
        internal static class CharacterStageClassSelectionPanelOnBeginShow
        {
            internal static void Prefix(CharacterStageClassSelectionPanel __instance, RulesetCharacterHero ___currentHero, ref int ___selectedClass)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                if (!Models.LevelUpContext.IsLevelingUp(___currentHero))
                {
                    return;
                }

                var compatibleClasses = __instance.GetField<CharacterStageClassSelectionPanel, List<CharacterClassDefinition>>("compatibleClasses");

                Models.LevelUpContext.SetIsClassSelectionStage(___currentHero, true);
                Models.InOutRulesContext.EnumerateHeroAllowedClassDefinitions(___currentHero, compatibleClasses, ref ___selectedClass);

                var commonData = __instance.CommonData;

                // NOTE: don't use AttackModesPanel?. which bypasses Unity object lifetime check
                if (commonData.AttackModesPanel)
                {
                    commonData.AttackModesPanel.RefreshNow();
                }

                // NOTE: don't use PersonalityMapPanel?. which bypasses Unity object lifetime check
                if (commonData.PersonalityMapPanel)
                {
                    commonData.PersonalityMapPanel.RefreshNow();
                }
            }
        }

        // filter active features
        [HarmonyPatch(typeof(CharacterStageClassSelectionPanel), "OnHigherLevelCb")]
        internal static class CharacterStageClassSelectionPanelOnHigherLevelCb
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

                var currentHeroField = typeof(CharacterStageClassSelectionPanel).GetField("currentHero", BindingFlags.Instance | BindingFlags.NonPublic);

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
        [HarmonyPatch(typeof(CharacterStageClassSelectionPanel), "EnumerateActiveFeatures")]
        internal static class CharacterStageClassSelectionPanelEnumerateActiveFeatures
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

                var currentHeroField = typeof(CharacterStageClassSelectionPanel).GetField("currentHero", BindingFlags.Instance | BindingFlags.NonPublic);

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

        // get my own classLevel / filter active features
        [HarmonyPatch(typeof(CharacterStageClassSelectionPanel), "FillClassFeatures")]
        internal static class CharacterStageClassSelectionPanelFillClassFeatures
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

                var currentHeroField = typeof(CharacterStageClassSelectionPanel).GetField("currentHero", BindingFlags.Instance | BindingFlags.NonPublic);

                var classesAndLevelsMethod = typeof(RulesetCharacterHero).GetMethod("get_ClassesAndLevels");
                var getClassLevelMethod = typeof(Models.LevelUpContext).GetMethod("GetClassLevel");

                var instructionsToBypass = 0;

                foreach (var instruction in instructions)
                {
                    if (instructionsToBypass > 0)
                    {
                        instructionsToBypass--;
                    }
                    else if (instruction.Calls(classesAndLevelsMethod))
                    {
                        yield return instruction;
                        yield return new CodeInstruction(OpCodes.Call, getClassLevelMethod);
                        instructionsToBypass = 2;
                    }
                    else if (instruction.Calls(classFeatureUnlocksMethod))
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

        // get my own classLevel
        [HarmonyPatch(typeof(CharacterStageClassSelectionPanel), "RefreshCharacter")]
        internal static class CharacterStageClassSelectionPanelRefreshCharacter
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

                var classesAndLevelsMethod = typeof(RulesetCharacterHero).GetMethod("get_ClassesAndLevels");
                var getClassLevelMethod = typeof(Models.LevelUpContext).GetMethod("GetClassLevel");
                var instructionsToBypass = 0;

                foreach (var instruction in instructions)
                {
                    if (instructionsToBypass > 0)
                    {
                        instructionsToBypass--;
                    }
                    else if (instruction.Calls(classesAndLevelsMethod))
                    {
                        yield return instruction;
                        yield return new CodeInstruction(OpCodes.Call, getClassLevelMethod);
                        instructionsToBypass = 2;
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }
        }

        // hide the equipment panel group
        [HarmonyPatch(typeof(CharacterStageClassSelectionPanel), "Refresh")]
        internal static class CharacterStageClassSelectionPanelRefresh
        {
            public static bool SetActive(RulesetCharacterHero currentHero)
            {
                return !(Models.LevelUpContext.IsLevelingUp(currentHero) && Models.LevelUpContext.IsClassSelectionStage(currentHero));
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

                var setActiveFound = 0;
                var setActiveMethod = typeof(GameObject).GetMethod("SetActive");
                var mySetActiveMethod = typeof(CharacterStageClassSelectionPanelRefresh).GetMethod("SetActive");
                var currentHeroField = typeof(CharacterStageClassSelectionPanel).GetField("currentHero", BindingFlags.Instance | BindingFlags.NonPublic);

                foreach (var instruction in instructions)
                {
                    if (instruction.Calls(setActiveMethod) && ++setActiveFound == 4)
                    {
                        yield return new CodeInstruction(OpCodes.Pop);
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, currentHeroField);
                        yield return new CodeInstruction(OpCodes.Call, mySetActiveMethod);
                    }

                    yield return instruction;
                }
            }
        }
    }
}
