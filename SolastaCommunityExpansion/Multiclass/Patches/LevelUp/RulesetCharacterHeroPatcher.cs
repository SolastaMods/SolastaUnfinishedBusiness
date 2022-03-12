using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Multiclass.Models;

namespace SolastaCommunityExpansion.Multiclass.Patches.LevelUp
{
    internal static class RulesetCharacterHeroPatcher
    {
        // ensures we only add the dice max value on level 1
        [HarmonyPatch(typeof(RulesetCharacterHero), "AddClassLevel")]
        internal static class RulesetCharacterHeroAddClassLevel
        {
            internal static bool Prefix(RulesetCharacterHero __instance, CharacterClassDefinition classDefinition, List<int> ___hitPointsGainHistory)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                if (!Models.LevelUpContext.IsLevelingUp(__instance))
                {
                    return true;
                }

                __instance.ClassesHistory.Add(classDefinition);

                if (__instance.ClassesAndLevels.ContainsKey(classDefinition))
                {
                    __instance.ClassesAndLevels[classDefinition]++;
                }
                else
                {
                    __instance.ClassesAndLevels.Add(classDefinition, 1);
                }

                if (__instance.ClassesAndLevels[classDefinition] == 1 && !Models.LevelUpContext.IsMulticlass(__instance))
                {
                    ___hitPointsGainHistory.Add(RuleDefinitions.DiceMaxValue[(int)classDefinition.HitDice]);
                }
                else
                {
                    ___hitPointsGainHistory.Add(HeroDefinitions.RollHitPoints(classDefinition.HitDice));
                }

                __instance.ComputeCharacterLevel();
                __instance.ComputeProficiencyBonus();

                return false;
            }
        }

        // filter active features
        [HarmonyPatch(typeof(RulesetCharacterHero), "FindClassHoldingFeature")]
        internal static class RulesetCharacterHeroFindClassHoldingFeature
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

                foreach (var instruction in instructions)
                {
                    if (instruction.Calls(classFeatureUnlocksMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Call, classFilteredFeatureUnlocksMethod);
                    }
                    else if (instruction.Calls(subclassFeatureUnlocksMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
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
        [HarmonyPatch(typeof(RulesetCharacterHero), "LookForFeatureOrigin")]
        internal static class RulesetCharacterHeroLookForFeatureOrigin
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

                foreach (var instruction in instructions)
                {
                    if (instruction.Calls(classFeatureUnlocksMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Call, classFilteredFeatureUnlocksMethod);
                    }
                    else if (instruction.Calls(subclassFeatureUnlocksMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Call, subclassFilteredFeatureUnlocksMethod);
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
