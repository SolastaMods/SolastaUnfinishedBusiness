using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal static class FeatureApplicationValidation
{
    internal static void EnumerateActionPerformanceProviders(
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

    internal static void EnumerateAdditionalActionProviders(
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

    internal static FeatureDefinition ValidateAttributeModifier(FeatureDefinition feature,
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
