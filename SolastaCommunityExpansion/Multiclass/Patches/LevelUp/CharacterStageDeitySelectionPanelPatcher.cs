using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaMulticlass.Models;
using SolastaModApi.Infrastructure;
using SolastaCommunityExpansion;

namespace SolastaMulticlass.Patches.LevelUp
{
    internal static class CharacterStageDeitySelectionPanelPatcher
    {
        // filter active features
        [HarmonyPatch(typeof(CharacterStageDeitySelectionPanel), "OnHigherLevelCb")]
        internal static class CharacterStageDeitySelectionPanelOnHigherLevelCb
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

                var currentHeroField = typeof(CharacterStageDeitySelectionPanel).GetField("currentHero", BindingFlags.Instance | BindingFlags.NonPublic);

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
        [HarmonyPatch(typeof(CharacterStageDeitySelectionPanel), "FillSubclassFeatures")]
        internal static class CharacterStageDeitySelectionPanelFillSubclassFeatures
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

                var currentHeroField = typeof(CharacterStageDeitySelectionPanel).GetField("currentHero", BindingFlags.Instance | BindingFlags.NonPublic);

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
        [HarmonyPatch(typeof(CharacterStageDeitySelectionPanel), "EnumerateActiveFeatures")]
        internal static class CharacterStageDeitySelectionPanelEnumerateActiveFeatures
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

                var currentHeroField = typeof(CharacterStageDeitySelectionPanel).GetField("currentHero", BindingFlags.Instance | BindingFlags.NonPublic);

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
        [HarmonyPatch(typeof(CharacterStageDeitySelectionPanel), "EnterStage")]
        internal static class CharacterStageDeitySelectionPanelEnumerateEnterStage
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

                var currentHeroField = typeof(CharacterStageDeitySelectionPanel).GetField("currentHero", BindingFlags.Instance | BindingFlags.NonPublic);

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

        // disables the deity selection screen if any classes multiclass into a Cleric or if any classes except Cleric multiclasses into a Paladin
        [HarmonyPatch(typeof(CharacterStageDeitySelectionPanel), "UpdateRelevance")]
        internal static class CharacterStageDeitySelectionPanelUpdateRelevance
        {
            internal static void Postfix(CharacterStageDeitySelectionPanel __instance, RulesetCharacterHero ___currentHero)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                if (LevelUpContext.IsLevelingUp(___currentHero))
                {
                    __instance.SetField("isRelevant", LevelUpContext.RequiresDeity(___currentHero));
                }
            }
        }
    }
}
