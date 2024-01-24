using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomSpecificBehaviors;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.CustomValidators;

internal static class ValidateFeatureApplication
{
    internal static void EnumerateActionPerformanceProviders(
        RulesetActor actor,
        List<FeatureDefinition> features,
#pragma warning disable IDE0060
        Dictionary<FeatureDefinition, FeatureOrigin> featuresOrigin = null)
#pragma warning restore IDE0060
    {
        actor.EnumerateFeaturesToBrowse<IActionPerformanceProvider>(features);

        if (actor is not RulesetCharacter character)
        {
            return;
        }

        features.RemoveAll(f =>
        {
            var validator = f.GetFirstSubFeatureOfType<IValidateDefinitionApplication>();

            return validator != null && !validator.IsValid(f, character);
        });
    }

    internal static void EnumerateAdditionalActionProviders(
        RulesetActor actor,
        List<FeatureDefinition> features,
#pragma warning disable IDE0060
        Dictionary<FeatureDefinition, FeatureOrigin> featuresOrigin)
#pragma warning restore IDE0060
    {
        var customFeatures = Main.Settings.EnableActionSwitching
            ? ActionSwitching.EnumerateActorFeatures<IAdditionalActionsProvider>(actor)
            : null;

        if (customFeatures == null)
        {
            actor.EnumerateFeaturesToBrowse<IAdditionalActionsProvider>(features);

            //This extra code is needed here only for cases when actions switching is disabled
            //PATCH: move on `HasDownedAnEnemy` bonus actions to the end of the list, preserving order
            //fixes main attacks stopping working if Horde Breaker's extra action on kill is triggered after Action Surge
            var onKill = features.FindAll(x => x is IAdditionalActionsProvider
            {
                TriggerCondition: AdditionalActionTriggerCondition.HasDownedAnEnemy
            });

            if (onKill.Count == 0)
            {
                return;
            }

            features.RemoveAll(x => onKill.Contains(x));
            features.AddRange(onKill);
        }
        else
        {
            features.SetRange(customFeatures.Select(x => x.feature).Where(x =>
                //leave only non-triggered features - all triggered features are reworked to grant conditions
                x is IAdditionalActionsProvider { TriggerCondition: AdditionalActionTriggerCondition.None }));
        }
    }

    internal static FeatureDefinition ValidateAttributeModifier(
        FeatureDefinition feature,
        RulesetCharacter character)
    {
        if (feature is not FeatureDefinitionAttributeModifier mod)
        {
            return null;
        }

        var validator = mod.GetFirstSubFeatureOfType<IValidateDefinitionApplication>();

        return validator == null || validator.IsValid(feature, character)
            ? mod
            : null;
    }
}
