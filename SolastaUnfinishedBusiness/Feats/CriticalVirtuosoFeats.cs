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
        const string Description = "Feat/&FeatCriticalVirtuosoDescription";
        var nameImproved = DatabaseHelper.FeatureDefinitionAttributeModifiers
            .AttributeModifierMartialChampionImprovedCritical.GuiPresentation.Title;
        var nameSuperior = DatabaseHelper.FeatureDefinitionAttributeModifiers
            .AttributeModifierMartialChampionSuperiorCritical.GuiPresentation.Title;

        var improved = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierFeatImprovedCritical")
            .SetGuiPresentation(nameImproved, Description)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.CriticalThreshold, -1)
            .AddToDB();

        var superior = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierFeatSuperiorCritical")
            .SetGuiPresentation(nameSuperior, Description)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.CriticalThreshold, -1)
            .AddToDB();

        // Improved Critical
        var featImprovedCritical = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatImprovedCritical")
            .SetGuiPresentation(nameImproved, Description)
            .SetFeatures(improved)
            .SetValidators(ValidatorsFeat.IsLevel4)
            .AddToDB();

        // Superior Critical
        var featSuperiorCritical = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatSuperiorCritical")
            .SetGuiPresentation(nameSuperior, Description)
            .SetFeatures(superior)
            .SetValidators(ValidatorsFeat.IsLevel16, ValidatorsFeat.ValidateHasFeature(improved))
            .AddToDB();

        feats.AddRange(featImprovedCritical, featSuperiorCritical);

        GroupFeats.MakeGroup("FeatGroupCriticalVirtuoso", null,
            featImprovedCritical,
            featSuperiorCritical);
    }
}
