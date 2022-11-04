using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal static class FeatureApplicationValidation
{
    internal static IEnumerable<CodeInstruction> ValidateActionPerformanceProviders(
        this IEnumerable<CodeInstruction> instructions)
    {
        var enumerate = new Action<
            RulesetActor,
            List<FeatureDefinition>,
            Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin>
        >(EnumerateActionPerformanceProviders).Method;

        return instructions.ReplaceCode(instruction =>
                $"{instruction.operand}".Contains("EnumerateFeaturesToBrowse") &&
                $"{instruction.operand}".Contains("IActionPerformanceProvider"),
            -1,
            new CodeInstruction(OpCodes.Call, enumerate));
    }

    private static void EnumerateActionPerformanceProviders(
        RulesetActor actor,
        List<FeatureDefinition> features,
        Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin> featuresOrigin = null)
    {
        actor.EnumerateFeaturesToBrowse<IActionPerformanceProvider>(features);

        if (actor is not RulesetCharacter character)
        {
            return;
        }

        features.RemoveAll(f =>
        {
            var validator = f.GetFirstSubFeatureOfType<IDefinitionApplicationValidator>();

            return validator != null && !validator.IsValid(f, character);
        });
    }

    internal static IEnumerable<CodeInstruction> ValidateAdditionalActionProviders(
        this IEnumerable<CodeInstruction> instructions)
    {
        var enumerate = new Action<
            RulesetActor,
            List<FeatureDefinition>,
            Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin>
        >(EnumerateAdditionalActionProviders).Method;

        return instructions.ReplaceCode(instruction =>
                $"{instruction.operand}".Contains("EnumerateFeaturesToBrowse") &&
                $"{instruction.operand}".Contains("IAdditionalActionsProvider"),
            -1,
            new CodeInstruction(OpCodes.Call, enumerate));
    }

    private static void EnumerateAdditionalActionProviders(
        RulesetActor actor,
        List<FeatureDefinition> features,
        Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin> featuresOrigin = null)
    {
        actor.EnumerateFeaturesToBrowse<IAdditionalActionsProvider>(features);

        if (actor is not RulesetCharacter character)
        {
            return;
        }

        features.RemoveAll(f =>
        {
            var validator = f.GetFirstSubFeatureOfType<IDefinitionApplicationValidator>();
            return validator != null && !validator.IsValid(f, character);
        });
    }

    internal static IEnumerable<CodeInstruction> ValidateAttributeModifiersFromConditions(
        IEnumerable<CodeInstruction> instructions)
    {
        //Replaces first `IsInst` operator with custom validator

        var validate = new Func<
            FeatureDefinition,
            RulesetCharacter,
            FeatureDefinition
        >(ValidateAttributeModifier).Method;

        return instructions.ReplaceCode(instruction => instruction.opcode == OpCodes.Isinst,
            -1,
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Call, validate));
    }

    private static FeatureDefinition ValidateAttributeModifier(FeatureDefinition feature,
        RulesetCharacter character)
    {
        if (feature is not FeatureDefinitionAttributeModifier mod)
        {
            return null;
        }

        var validator = mod.GetFirstSubFeatureOfType<IDefinitionApplicationValidator>();

        return validator == null || validator.IsValid(feature, character)
            ? mod
            : null;
    }
}
