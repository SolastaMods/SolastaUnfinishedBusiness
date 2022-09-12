using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomDefinitions;

namespace SolastaUnfinishedBusiness.CustomFeatures;

internal static class FeatureApplicationValidation
{
    public static void ValidateActionPerformanceProviders(List<CodeInstruction> codes)
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

    private static void EnumerateActionPerformanceProviders(RulesetActor actor, List<FeatureDefinition> features,
        Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin> featuresOrigin = null)
    {
        actor.EnumerateFeaturesToBrowse<IActionPerformanceProvider>(features);
        if (actor is not RulesetCharacter character)
        {
            return;
        }

        features.RemoveAll(f =>
        {
            var validator = f.GetFirstSubFeatureOfType<IFeatureApplicationValidator>();
            return validator != null && !validator.IsValid(character);
        });
    }

    public static void ValidateAdditionalActionProviders(List<CodeInstruction> codes)
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

    private static void EnumerateAdditionalActionProviders(RulesetActor actor, List<FeatureDefinition> features,
        Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin> featuresOrigin = null)
    {
        actor.EnumerateFeaturesToBrowse<IAdditionalActionsProvider>(features);
        if (actor is not RulesetCharacter character)
        {
            return;
        }

        features.RemoveAll(f =>
        {
            var validator = f.GetFirstSubFeatureOfType<IFeatureApplicationValidator>();
            return validator != null && !validator.IsValid(character);
        });
    }
}
