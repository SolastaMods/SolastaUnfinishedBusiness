using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class AiLocationDefinitionsPatcher
{
    //PATCH: supports `UseOfficialLightingObscurementAndVisionRules`
    [HarmonyPatch(typeof(AiLocationDefinitions), nameof(AiLocationDefinitions.IsValidMagicEffect))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CanCastMagic_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
            ref bool __result,
            AiLocationCharacter aiCharacter,
            IMagicEffect magicEffect,
            GameLocationCharacter optionalTarget)
        {
            var locationCharacter = aiCharacter.GameLocationCharacter;

            if (__result &&
                !LightingAndObscurementContext.IsMagicEffectValidIfHeavilyObscuredOrInNaturalDarkness(
                    locationCharacter, magicEffect, optionalTarget))
            {
                __result = false;
            }
        }
    }
    
    //BUGFIX: fix vanilla to consider darkness condition as a parent
    [HarmonyPatch(typeof(AiLocationDefinitions), nameof(AiLocationDefinitions.ComputeAoERawScore))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ComputeAoERawScore_Patch
    {
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var conditionDefinitionMethod = typeof(ConditionForm).GetMethod("get_ConditionDefinition");
            var myConditionDefinitionMethod =
                new Func<ConditionForm, ConditionDefinition>(MyConditionDefinition).Method;

            return instructions.ReplaceCalls(conditionDefinitionMethod, "AiLocationDefinitions.ComputeRawScore",
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, myConditionDefinitionMethod));
        }

        private static ConditionDefinition MyConditionDefinition(ConditionForm conditionForm)
        {
            if (conditionForm.ConditionDefinition.Name == RuleDefinitions.ConditionDarkness ||
                conditionForm.ConditionDefinition.parentCondition.Name == RuleDefinitions.ConditionDarkness)
            {
                return DatabaseHelper.ConditionDefinitions.ConditionDarkness;
            }
            
            return conditionForm.ConditionDefinition;
        }
    }
}
