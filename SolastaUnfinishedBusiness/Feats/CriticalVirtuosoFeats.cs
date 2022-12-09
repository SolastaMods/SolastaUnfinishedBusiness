using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomBehaviors;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;

namespace SolastaUnfinishedBusiness.Feats;

internal static class CriticalVirtuosoFeats
{
    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        // Improved Critical
        var featImprovedCritical = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatImprovedCritical")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierMartialChampionImprovedCritical)
            .SetValidators(
                ValidatorsFeat.ValidateNotFeature(AttributeModifierMartialChampionImprovedCritical))
            .AddToDB();

        // Superior Critical
        var featSuperiorCritical = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatSuperiorCritical")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierMartialChampionSuperiorCritical)
            .SetValidators(
                ValidatorsFeat.ValidateHasFeature(AttributeModifierMartialChampionImprovedCritical),
                ValidatorsFeat.ValidateNotFeature(AttributeModifierMartialChampionSuperiorCritical))
            .AddToDB();

        feats.AddRange(featImprovedCritical, featSuperiorCritical);

        GroupFeats.MakeGroup("FeatGroupCriticalVirtuoso", null,
            featImprovedCritical,
            featSuperiorCritical);
    }
}
