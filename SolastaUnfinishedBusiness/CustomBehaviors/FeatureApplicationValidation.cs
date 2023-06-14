using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.GameExtensions;
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
        List<FeatureDefinition> features = null,
        Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin> featuresOrigin = null)
    {
        features ??= actor.FeaturesToBrowse;

        actor.EnumerateFeaturesToBrowse<IAdditionalActionsProvider>(features);

        //PATCH: move on `HasDownedAnEnemy` bonus actions to the end of the list, preserving order
        //fixes main attacks stopping working if Horde Breaker's extra action on kill is triggered after Action Surge
        var onKill = features.FindAll(x => x is IAdditionalActionsProvider
        {
            TriggerCondition: RuleDefinitions.AdditionalActionTriggerCondition.HasDownedAnEnemy
        });

        if (!onKill.Empty())
        {
            features.RemoveAll(x => onKill.Contains(x));
            features.AddRange(onKill);
        }

        if (actor is not RulesetCharacter character)
        {
            return;
        }

        features.RemoveAll(f =>
        {
            var validator = f.GetFirstSubFeatureOfType<IDefinitionApplicationValidator>();

            return validator != null && !validator.IsValid(f, character);
        });

        //remove non-triggered features to have only one placve that requires those checks
        var locChar = GameLocationCharacter.GetFromActor(actor);
        if (locChar == null || locChar.enemiesDownedByAttack > 0)
        {
            return;
        }

        features.RemoveAll(f =>
                locChar.enemiesDownedByAttack <= 0
                && (f as IAdditionalActionsProvider)?.TriggerCondition ==
                RuleDefinitions.AdditionalActionTriggerCondition.HasDownedAnEnemy);
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
