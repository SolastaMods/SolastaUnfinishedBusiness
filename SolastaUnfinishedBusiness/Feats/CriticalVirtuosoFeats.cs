using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Validators;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;

namespace SolastaUnfinishedBusiness.Feats;

internal static class CriticalVirtuosoFeats
{
    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var improved = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierFeatImprovedCritical")
            .SetGuiPresentation(
                AttributeModifierMartialChampionImprovedCritical.GuiPresentation.Title,
                AttributeModifierMartialChampionImprovedCritical.GuiPresentation.Description)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.ForceIfWorse,
                AttributeDefinitions.CriticalThreshold, 19)
            .AddToDB();

        var superior = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierFeatSuperiorCritical")
            .SetGuiPresentation(
                AttributeModifierMartialChampionSuperiorCritical.GuiPresentation.Title,
                AttributeModifierMartialChampionSuperiorCritical.GuiPresentation.Description)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.ForceIfWorse,
                AttributeDefinitions.CriticalThreshold, 18)
            .AddToDB();

        // Improved Critical
        var featImprovedCritical = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatImprovedCritical")
            .SetGuiPresentation(
                AttributeModifierMartialChampionImprovedCritical.GuiPresentation.Title,
                AttributeModifierMartialChampionImprovedCritical.GuiPresentation.Description)
            .SetFeatures(improved)
            .SetValidators(ValidatorsFeat.IsLevel4)
            .AddToDB();

        // Superior Critical
        var featSuperiorCritical = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatSuperiorCritical")
            .SetGuiPresentation(
                AttributeModifierMartialChampionSuperiorCritical.GuiPresentation.Title,
                AttributeModifierMartialChampionSuperiorCritical.GuiPresentation.Description)
            .SetFeatures(superior)
            .SetValidators(ValidatorsFeat.IsLevel16, ValidatorsFeat.ValidateHasFeature(improved))
            .AddToDB();

        feats.AddRange(featImprovedCritical, featSuperiorCritical);

        GroupFeats.FeatGroupSupportCombat.AddFeats(
            featImprovedCritical,
            featSuperiorCritical);
    }
}
