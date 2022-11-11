using SolastaUnfinishedBusiness.Builders;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Models;

/**
 * Place for generic conditions that mey be reused between several features
 */
internal static class CustomConditions
{
    internal static ConditionDefinition StopMovement;

    internal static void Load()
    {
        StopMovement = ConditionDefinitionBuilder
            .Create("ConditionStopMovement")
            .SetGuiPresentation(Category.Condition, Gui.NoLocalization, ConditionDefinitions.ConditionRestrained)
            .SetConditionType(RuleDefinitions.ConditionType.Detrimental)
            .SetFeatures(
                FeatureDefinitionMovementAffinitys.MovementAffinityConditionRestrained,
                FeatureDefinitionActionAffinitys.ActionAffinityConditionRestrained)
            .AddToDB();
    }
}
