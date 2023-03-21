using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
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
            .SetGuiPresentation("MartialChampionImprovedCritical", Category.Feature)
            .SetFeatures(AttributeModifierMartialChampionImprovedCritical)
            .SetValidators(
                ValidatorsFeat.IsLevel4,
                ValidatorsFeat.ValidateNotFeature(AttributeModifierMartialChampionImprovedCritical))
            .AddToDB();

        // Superior Critical
        var featSuperiorCritical = FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatSuperiorCritical")
            .SetGuiPresentation("MartialChampionSuperiorCritical", Category.Feature)
            .SetFeatures(AttributeModifierMartialChampionSuperiorCritical)
            .SetValidators(
                ValidatorsFeat.IsLevel16,
                ValidatorsFeat.ValidateHasFeature(AttributeModifierMartialChampionImprovedCritical),
                ValidatorsFeat.ValidateNotFeature(AttributeModifierMartialChampionSuperiorCritical))
            .AddToDB();

        feats.AddRange(featImprovedCritical, featSuperiorCritical);

        GroupFeats.MakeGroup("FeatGroupCriticalVirtuoso", null,
            featImprovedCritical,
            featSuperiorCritical);
    }
}
