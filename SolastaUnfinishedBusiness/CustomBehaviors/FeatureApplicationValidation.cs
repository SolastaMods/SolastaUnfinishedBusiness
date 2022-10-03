using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal static class FeatureApplicationValidation
{
    internal static void ValidateActionPerformanceProviders(List<CodeInstruction> codes)
    {
        var enumerate = new Action<
            RulesetActor,
            List<FeatureDefinition>,
            Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin>
        >(EnumerateActionPerformanceProviders).Method;

        var bindIndex = codes.FindIndex(x =>
        {
            if (x.operand == null)
            {
                return false;
            }

            var operand = x.operand.ToString();

            return operand.Contains("EnumerateFeaturesToBrowse") && operand.Contains("IActionPerformanceProvider");
        });

        if (bindIndex > 0)
        {
            codes[bindIndex] = new CodeInstruction(OpCodes.Call, enumerate);
        }
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

    internal static void ValidateAdditionalActionProviders(List<CodeInstruction> codes)
    {
        var enumerate = new Action<
            RulesetActor,
            List<FeatureDefinition>,
            Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin>
        >(EnumerateAdditionalActionProviders).Method;

        var bindIndex2 = codes.FindIndex(x =>
        {
            if (x.operand == null)
            {
                return false;
            }

            var operand = x.operand.ToString();

            return operand.Contains("EnumerateFeaturesToBrowse") && operand.Contains("IAdditionalActionsProvider");
        });

        if (bindIndex2 > 0)
        {
            codes[bindIndex2] = new CodeInstruction(OpCodes.Call, enumerate);
        }
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

    internal static void ValidateAttributeModifiersFromConditions(List<CodeInstruction> codes)
    {
        //Replaces first `IsInst` operator with custom validator

        var validate = new Func<
            FeatureDefinition,
            RulesetCharacter,
            FeatureDefinition
        >(ValidateAttributeModifier).Method;

        var index = codes.FindIndex(x => x.opcode == OpCodes.Isinst);

        if (index > 0)
        {
            codes[index] = new CodeInstruction(OpCodes.Call, validate);
            //add `this` (RulesetCharacter) as second argument
            codes.Insert(index, new CodeInstruction(OpCodes.Ldarg_0));
        }
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
