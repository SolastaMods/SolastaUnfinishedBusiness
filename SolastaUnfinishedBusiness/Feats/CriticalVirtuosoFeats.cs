using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Validators;

namespace SolastaUnfinishedBusiness.Feats;

internal static class CriticalVirtuosoFeats
{
    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var improved = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierFeatImprovedCritical")
            .SetGuiPresentation(DatabaseHelper.FeatureDefinitionAttributeModifiers
                .AttributeModifierMartialChampionImprovedCritical.GuiPresentation)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.ForceIfWorse,
                AttributeDefinitions.CriticalThreshold, 19)
            .AddToDB();

        var superior = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierFeatSuperiorCritical")
            .SetGuiPresentation(DatabaseHelper.FeatureDefinitionAttributeModifiers
                .AttributeModifierMartialChampionSuperiorCritical.GuiPresentation)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.ForceIfWorse,
                AttributeDefinitions.CriticalThreshold, 18)
            .AddToDB();

        // Improved Critical
        var featImprovedCritical = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatImprovedCritical")
            .SetGuiPresentation(DatabaseHelper.FeatureDefinitionAttributeModifiers
                .AttributeModifierMartialChampionImprovedCritical.GuiPresentation)
            .SetFeatures(improved)
            .SetValidators(ValidatorsFeat.IsLevel4)
            .AddToDB();

        // Superior Critical
        var featSuperiorCritical = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatSuperiorCritical")
            .SetGuiPresentation(DatabaseHelper.FeatureDefinitionAttributeModifiers
                .AttributeModifierMartialChampionSuperiorCritical.GuiPresentation)
            .SetFeatures(superior)
            .SetValidators(ValidatorsFeat.IsLevel16, ValidatorsFeat.ValidateHasFeature(improved))
            .AddToDB();

        feats.AddRange(featImprovedCritical, featSuperiorCritical);

        GroupFeats.FeatGroupSupportCombat.AddFeats(
            featImprovedCritical,
            featSuperiorCritical);
    }
}
