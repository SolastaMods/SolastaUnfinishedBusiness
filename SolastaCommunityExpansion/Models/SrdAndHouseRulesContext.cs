using static SolastaModApi.DatabaseHelper.ConditionDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionActionAffinitys;

namespace SolastaCommunityExpansion.Models
{
    internal static class SrdAndHouseRulesContext
    {
        public static void ApplyConditionBlindedShouldNotAllowOpportunityAttack()
        {
            // Use the shocked condition affinity which has the desired effect
            if (Main.Settings.EnableConditionBlindedShouldNotAllowOpportunityAttack)
            {
                if (!ConditionBlinded.Features.Contains(ActionAffinityConditionShocked))
                {
                    ConditionBlinded.Features.Add(ActionAffinityConditionShocked);
                }
            }
            else
            {
                if (ConditionBlinded.Features.Contains(ActionAffinityConditionShocked))
                {
                    ConditionBlinded.Features.Remove(ActionAffinityConditionShocked);
                }
            }
        }
    }
}