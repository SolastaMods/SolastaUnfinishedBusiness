using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaMulticlass.Models;
using SolastaModApi.Infrastructure;
using SolastaCommunityExpansion;

namespace SolastaMulticlass.Patches.LevelUp
{
    internal static class CharacterStageSubclassSelectionPanelPatcher
    {
        // filter active features
        [HarmonyPatch(typeof(CharacterStageSubclassSelectionPanel), "OnHigherLevelCb")]
        internal static class CharacterStageSubclassSelectionPanelOnHigherLevelCb
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

                var currentHeroField = typeof(CharacterStageSubclassSelectionPanel).GetField("currentHero", BindingFlags.Instance | BindingFlags.NonPublic);

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
        [HarmonyPatch(typeof(CharacterStageSubclassSelectionPanel), "FillSubclassFeatures")]
        internal static class CharacterStageSubclassSelectionPanelFillSubclassFeatures
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

                var currentHeroField = typeof(CharacterStageSubclassSelectionPanel).GetField("currentHero", BindingFlags.Instance | BindingFlags.NonPublic);

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
        [HarmonyPatch(typeof(CharacterStageSubclassSelectionPanel), "EnumerateActiveFeatures")]
        internal static class CharacterStageSubclassSelectionPanelEnumerateActiveFeatures
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

                var currentHeroField = typeof(CharacterStageSubclassSelectionPanel).GetField("currentHero", BindingFlags.Instance | BindingFlags.NonPublic);

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
        [HarmonyPatch(typeof(CharacterStageSubclassSelectionPanel), "Refresh")]
        internal static class CharacterStageSubclassSelectionPanelRefresh
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

                var currentHeroField = typeof(CharacterStageSubclassSelectionPanel).GetField("currentHero", BindingFlags.Instance | BindingFlags.NonPublic);

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

        // disables the sub class selection screen if the deity screen was enabled
        [HarmonyPatch(typeof(CharacterStageSubclassSelectionPanel), "UpdateRelevance")]
        internal static class CharacterStageSubclassSelectionPanelUpdateRelevance
        {
            internal static void Postfix(CharacterStageSubclassSelectionPanel __instance, RulesetCharacterHero ___currentHero)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                if (LevelUpContext.IsLevelingUp(___currentHero) && LevelUpContext.RequiresDeity(___currentHero))
                {
                    __instance.SetField("isRelevant", false);
                }
            }
        }
    }
}
