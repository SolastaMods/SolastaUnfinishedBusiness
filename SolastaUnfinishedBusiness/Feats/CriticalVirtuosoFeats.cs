using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;

namespace SolastaUnfinishedBusiness.Feats;

internal static class CriticalVirtuosoFeats
{
    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        // Improved Critical
        var featImprovedCritical = FeatDefinitionBuilder
            .Create("FeatImprovedCritical")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierMartialChampionImprovedCritical)
            .AddToDB();

        // Superior Critical
        var featSuperiorCritical = FeatDefinitionBuilder
            .Create("FeatSuperiorCritical")
            .SetGuiPresentation(Category.Feat)
            .SetKnownFeatsPrerequisite(featImprovedCritical.Name)
            .SetFeatures(AttributeModifierMartialChampionSuperiorCritical)
            .AddToDB();

        feats.AddRange(
            featImprovedCritical,
            featSuperiorCritical);

        GroupFeats.MakeGroup("FeatGroupCriticalVirtuoso", null,
            featImprovedCritical,
            featSuperiorCritical);
    }
}
